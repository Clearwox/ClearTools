using Clear;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace ClearTools.Tests;

public sealed class ApiClientConcurrencyTests
{
    public static IEnumerable<object[]> HttpVersionDefaults()
    {
        foreach (var method in new[] { "GET", "POST", "PUT", "DELETE" })
        {
            yield return new object[] { method, 1, 1, HttpVersionPolicy.RequestVersionOrLower };
            foreach (var major in new[] { 2, 3 })
                foreach (var policy in Enum.GetValues<HttpVersionPolicy>())
                    yield return new object[] { method, major, 0, policy };
        }
    }

    [Theory]
    [MemberData(nameof(HttpVersionDefaults))]
    public async Task Requests_PreserveConfiguredHttpVersionDefaults(
        string method, int major, int minor, HttpVersionPolicy policy)
    {
        var version = new Version(major, minor);
        int sent = 0;
        using var http = new HttpClient(new Handler(request =>
        {
            Assert.Equal(method, request.Method.Method);
            Assert.Equal(version, request.Version);
            Assert.Equal(policy, request.VersionPolicy);
            sent++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent("{\"Id\":\"ok\"}") };
        }));
        var client = new ApiClient(http);
        // Match HttpClient helpers: use settings at send time, even if configured
        // after ApiClient construction but before the first request.
        http.DefaultRequestVersion = version;
        http.DefaultVersionPolicy = policy;

        await Send(client, method, CancellationToken.None);

        Assert.Equal(1, sent);
    }

    [Fact]
    public async Task RequestCredentials_DoNotLeakToFollowingRequests()
    {
        var observed = new List<(string? Key, string? Authorization)>();
        using var http = new HttpClient(new Handler(request =>
        {
            observed.Add((request.Headers.TryGetValues("key", out var values) ? string.Join(",", values) : null,
                request.Headers.Authorization?.ToString()));
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") };
        }));
        var client = new ApiClient(http);

        await client.GetAsync<object>("https://test.invalid/first", "synthetic-token",
            new Dictionary<string, string> { ["key"] = "synthetic-key" });
        await client.GetAsync<object>("https://test.invalid/second");

        Assert.Equal(("synthetic-key", "Bearer synthetic-token"), observed[0]);
        Assert.Null(observed[1].Key);
        Assert.Null(observed[1].Authorization);
    }

    [Fact]
    public async Task ConcurrentRequests_PreserveCredentialsAndResponseOwnership_AndRemainUsable()
    {
        var invalidHeaders = new ConcurrentQueue<string>();
        using var http = new HttpClient(new Handler(request =>
        {
            if (!request.Headers.TryGetValues("key", out var values) ||
                !values.SequenceEqual(new[] { "synthetic-key" }))
                invalidHeaders.Enqueue(request.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new Reply(request.RequestUri!.AbsolutePath)))
            };
        }, yieldResponse: true));
        var client = new ApiClient(http);
        var errors = new ConcurrentQueue<string>();

        await Task.WhenAll(Enumerable.Range(0, 32).Select(worker => Task.Run(async () =>
        {
            for (int i = 0; i < 200; i++)
            {
                var path = $"/{worker}/{i}";
                var url = "https://test.invalid" + path;
                var headers = new Dictionary<string, string> { ["key"] = "synthetic-key" };
                try
                {
                    var reply = (i % 4) switch
                    {
                        0 => await client.GetAsync<Reply>(url, headers),
                        1 => await client.PostAsync<object, Reply>(url, new { }, headers),
                        2 => await client.PutAsync<object, Reply>(url, new { }, headers),
                        _ => await client.DeleteWithResultAsync<Reply>(url, headers)
                    };
                    if (reply.Id != path) errors.Enqueue($"Response mismatch for {path}");
                }
                catch (Exception ex) { errors.Enqueue(ex.GetType().Name + ": " + ex.Message); }
            }
        })));

        Assert.Empty(invalidHeaders);
        Assert.Empty(errors);
        // Continuing with the same instance must work after concurrent load ends.
        for (int i = 0; i < 100; i++)
            Assert.Equal("/recovery", (await client.GetAsync<Reply>("https://test.invalid/recovery",
                new Dictionary<string, string> { ["key"] = "synthetic-key" })).Id);
    }

    [Fact]
    public async Task UnauthorizedResponse_DoesNotPoisonTheClientOrRetryTheRequest()
    {
        int sent = 0;
        using var http = new HttpClient(new Handler(_ => new HttpResponseMessage(
            ++sent == 1 ? HttpStatusCode.Unauthorized : HttpStatusCode.OK)
        { Content = new StringContent("{\"Id\":\"recovered\"}") }));
        var client = new ApiClient(http);

        var error = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync<Reply>("https://test.invalid/first"));
        Assert.Equal(HttpStatusCode.Unauthorized, error.StatusCode);
        Assert.Equal(1, sent);
        Assert.Equal(HttpStatusCode.Unauthorized, client.LastResponse!.StatusCode);
        Assert.Equal("recovered", (await client.GetAsync<Reply>("https://test.invalid/second")).Id);
        Assert.Equal(2, sent);
    }

    public sealed record Reply(string Id);

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    public async Task Cancellation_InterruptsAStalledResponseBody(string method)
    {
        var content = new StalledBody();
        using var http = new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));
        var client = new ApiClient(http);
        using var cancellation = new CancellationTokenSource();
        var pending = Send(client, method, cancellation.Token);
        await content.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task HttpClientTimeout_InterruptsAStalledResponseBody()
    {
        using var http = new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StalledBody() }))
        { Timeout = TimeSpan.FromMilliseconds(100) };
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Send(new ApiClient(http), "POST", CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5)));
    }

    private static Task Send(ApiClient client, string method, CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string>();
        const string url = "https://test.invalid/stalled";
        return method switch
        {
            "GET" => client.GetAsync<Reply>(url, "", headers, cancellationToken),
            "POST" => client.PostAsync(url, new { }, "", headers, true, cancellationToken),
            "PUT" => client.PutAsync(url, new { }, "", headers, true, cancellationToken),
            _ => client.DeleteAsync(url, "", headers, true, cancellationToken)
        };
    }

    private sealed class StalledBody : HttpContent
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
            => SerializeToStreamAsync(stream, context, CancellationToken.None);
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
        {
            Started.TrySetResult();
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        protected override bool TryComputeLength(out long length) { length = 0; return false; }
    }

    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond, bool yieldResponse = false) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = respond(request);
            if (yieldResponse) await Task.Yield();
            return response;
        }
    }
}

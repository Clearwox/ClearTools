using System.Net;
using System.Text;
using System.Text.Json;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Data.AppConfiguration;
using ClearTools.Abstraction.Attributes;
using ClearTools.Extensions;

namespace ClearTools.Tests;

public class AppConfigurationSnapshotTests
{
    public class Settings
    {
        [AppConfigurationKey("db-key")]
        public string Database { get; set; } = "default";
        public int Count { get; set; } = 7;
        public bool Enabled { get; set; }
        public int? Optional { get; set; }
        public string Missing { get; set; } = "unchanged";
        [FeatureFlag("new-ui")]
        public bool NewUi { get; set; }
        [FeatureFlag("missing")]
        public bool? MissingFlag { get; set; }
    }

    public class PlainSettings
    {
        public string? Value { get; set; }
    }

    public class InvalidSettings
    {
        [FeatureFlag("invalid")]
        public string? Invalid { get; set; }
    }

    [Fact]
    public async Task BindsExistingAttributesTypesAndFlagsWithOneHttpRequest()
    {
        using var handler = new RecordingHandler(Page(new()
        {
            ["db-key"] = "connection", ["Count"] = "12", ["Enabled"] = "true",
            ["Optional"] = "23", [".appconfig.featureflag/new-ui"] = "{\"enabled\":true}",
            ["unrelated"] = "ignored"
        }));
        var settings = await AppConfigurationExtensions.CreateSettings<Settings>(Client(handler));
        Assert.Equal("connection", settings.Database);
        Assert.Equal(12, settings.Count);
        Assert.True(settings.Enabled);
        Assert.Equal(23, settings.Optional);
        Assert.True(settings.NewUi);
        Assert.Null(settings.MissingFlag);
        Assert.Equal("unchanged", settings.Missing);
        Assert.Single(handler.Requests);
        AssertBulkRequest(handler.Requests[0], "\0");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FollowsAllPagesBeforeBinding(bool headerContinuation)
    {
        const string next = "https://test.azconfig.io/kv?after=page2&api-version=1.0";
        var first = Page(new() { ["db-key"] = "first" }, headerContinuation ? null : next);
        if (headerContinuation) first.Headers.Add("Link", $"<{next}>; rel=\"next\"");
        using var handler = new RecordingHandler(first, Page(new()
        {
            ["Count"] = "42", [".appconfig.featureflag/new-ui"] = "{\"enabled\":true}"
        }));
        var settings = await AppConfigurationExtensions.CreateSettings<Settings>(Client(handler));
        Assert.Equal("first", settings.Database);
        Assert.Equal(42, settings.Count);
        Assert.True(settings.NewUi);
        Assert.Equal(2, handler.Requests.Count);
        Assert.Contains("after=page2", handler.Requests[1].Query);
        Assert.All(handler.Requests, uri => Assert.Equal("/kv", uri.AbsolutePath));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("dev")]
    [InlineData("dev*,test\\label")]
    public async Task UsesExactLabelsAndPreservesUnlabelledFeatureFlags(string? label)
    {
        using var handler = new RecordingHandler(
            Page(new() { ["Count"] = "17" }),
            Page(new() { [".appconfig.featureflag/new-ui"] = "{\"enabled\":true}" }));
        var settings = await AppConfigurationExtensions.CreateSettings<Settings>(Client(handler), label);
        var named = !string.IsNullOrEmpty(label);
        Assert.Equal(17, settings.Count);
        Assert.Equal(named, settings.NewUi);
        Assert.Equal(named ? 2 : 1, handler.Requests.Count);
        var escaped = named ? label!.Replace("\\", "\\\\").Replace("*", "\\*").Replace(",", "\\,") : "\0";
        AssertBulkRequest(handler.Requests[0], escaped);
        if (named)
        {
            AssertBulkRequest(handler.Requests[1], "\0");
            Assert.Contains("key=.appconfig.featureflag/*", Uri.UnescapeDataString(handler.Requests[1].Query));
        }
    }

    [Fact]
    public async Task NamedLabelWithoutFlagsNeedsOnlyOneQuery()
    {
        using var handler = new RecordingHandler(Page(new() { ["Value"] = "named" }));
        var settings = await AppConfigurationExtensions.CreateSettings<PlainSettings>(Client(handler), "dev");
        Assert.Equal("named", settings.Value);
        Assert.Single(handler.Requests);
    }

    [Theory]
    [InlineData("DB-*")]
    [InlineData("DB-KEY")]
    public async Task PreservesCaseInsensitiveLocalFilterAndIndependentFeatureFlags(string filter)
    {
        using var handler = new RecordingHandler(Page(new()
        {
            ["db-key"] = "selected", ["Count"] = "99",
            [".appconfig.featureflag/new-ui"] = "{\"enabled\":true}"
        }));
        var settings = await AppConfigurationExtensions.CreateSettings<Settings>(Client(handler), keyFilter: filter);
        Assert.Equal("selected", settings.Database);
        Assert.Equal(7, settings.Count);
        Assert.True(settings.NewUi);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task MissingCaseMismatchedAndNullValuesKeepDefaults()
    {
        using var handler = new RecordingHandler(Page(new() { ["count"] = "99", ["db-key"] = null }));
        var settings = await AppConfigurationExtensions.CreateSettings<Settings>(Client(handler));
        Assert.Equal(7, settings.Count);
        Assert.Equal("default", settings.Database);
        Assert.Null(settings.Optional);
        Assert.Null(settings.MissingFlag);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("{\"enabled\":false}")]
    [InlineData("{}")]
    public async Task MalformedOrDisabledFlagsRemainFalse(string value)
    {
        using var handler = new RecordingHandler(Page(new() { [".appconfig.featureflag/new-ui"] = value }));
        var settings = await AppConfigurationExtensions.CreateSettings<Settings>(Client(handler));
        Assert.False(settings.NewUi);
    }

    [Fact]
    public async Task InvalidFlagPropertyFailsBeforeRequestingConfiguration()
    {
        using var handler = new RecordingHandler();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            AppConfigurationExtensions.CreateSettings<InvalidSettings>(Client(handler)));
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task InvalidConversionDoesNotReturnPartiallyBoundSettings()
    {
        using var handler = new RecordingHandler(Page(new() { ["Count"] = "invalid" }));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            AppConfigurationExtensions.CreateSettings<Settings>(Client(handler)));
    }

    [Theory]
    [InlineData(401)]
    [InlineData(403)]
    public async Task AccessDeniedIncludesOriginalFailure(int status)
    {
        using var handler = new RecordingHandler(new HttpResponseMessage((HttpStatusCode)status));
        var error = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            AppConfigurationExtensions.CreateSettings<Settings>(Client(handler)));
        Assert.Equal(status, Assert.IsType<RequestFailedException>(error.InnerException).Status);
    }

    [Theory]
    [InlineData(429)]
    [InlineData(500)]
    [InlineData(404)]
    public async Task FailedLaterPageFailsWholeLoad(int status)
    {
        using var handler = new RecordingHandler(
            Page(new() { ["Count"] = "12" }, "https://test.azconfig.io/kv?after=next"),
            new HttpResponseMessage((HttpStatusCode)status));
        var error = await Assert.ThrowsAsync<RequestFailedException>(() =>
            AppConfigurationExtensions.CreateSettings<Settings>(Client(handler)));
        Assert.Equal(status, error.Status);
        Assert.Equal(2, handler.Requests.Count);
    }

    private static ConfigurationClient Client(RecordingHandler handler)
    {
        var options = new ConfigurationClientOptions
        {
            Transport = new HttpClientTransport(new HttpClient(handler)),
            Retry = { MaxRetries = 0 }
        };
        return new ConfigurationClient(new Uri("https://test.azconfig.io"), new FakeCredential(), options);
    }

    private static HttpResponseMessage Page(Dictionary<string, string?> values, string? next = null)
    {
        var items = values.Select(kv => new { key = kv.Key, value = kv.Value, label = (string?)null });
        var body = new Dictionary<string, object?> { ["items"] = items, ["@nextLink"] = next };
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
    }

    private static void AssertBulkRequest(Uri uri, string label)
    {
        Assert.Equal("/kv", uri.AbsolutePath);
        Assert.Contains($"label={label}", Uri.UnescapeDataString(uri.Query));
    }

    private sealed class FakeCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext context, CancellationToken token) =>
            new("synthetic-token", DateTimeOffset.UtcNow.AddHours(1));
        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext context, CancellationToken token) =>
            new(GetToken(context, token));
    }

    private sealed class RecordingHandler(params HttpResponseMessage[] responses) : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> remaining = new(responses);
        public List<Uri> Requests { get; } = [];
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Requests.Add(request.RequestUri!);
            Assert.NotEmpty(remaining);
            return Task.FromResult(remaining.Dequeue());
        }
    }
}

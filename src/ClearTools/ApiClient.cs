using Clear.Exceptions;
using Clear.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clear
{
    /// <summary>
    /// Interface for an API client.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Gets the last response string.
        /// </summary>
        string LastResponseString { get; }

        /// <summary>
        /// Sends a GET request to the specified URL and returns the response as an entity.
        /// </summary>
        Task<TEntity> GetAsync<TEntity>(string requestUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a GET request to the specified URL with a token and returns the response as an entity.
        /// </summary>
        Task<TEntity> GetAsync<TEntity>(string requestUrl, string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a GET request to the specified URL with headers and returns the response as an entity.
        /// </summary>
        Task<TEntity> GetAsync<TEntity>(string requestUrl, Dictionary<string, string> headers, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a GET request to the specified URL with a token and headers, and returns the response as an entity.
        /// </summary>
        Task<TEntity> GetAsync<TEntity>(string requestUrl, string token, Dictionary<string, string> headers, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request to the specified URL with content and returns the response as a string.
        /// </summary>
        Task<HttpResponseMessage> PostAsync<TEntity>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request to the specified URL with content and a token, and returns the response as a string.
        /// </summary>
        Task<HttpResponseMessage> PostAsync<TEntity>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request to the specified URL with content and headers, and returns the response as a string.
        /// </summary>
        Task<HttpResponseMessage> PostAsync<TEntity>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request to the specified URL with content, a token, and headers, and returns the response as a string.
        /// </summary>
        Task<HttpResponseMessage> PostAsync<TEntity>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request to the specified URL with content and returns the response as a result entity.
        /// </summary>
        Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request to the specified URL with content and a token, and returns the response as a result entity.
        /// </summary>
        Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request to the specified URL with content and headers, and returns the response as a result entity.
        /// </summary>
        Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request to the specified URL with content, a token, and headers, and returns the response as a result entity.
        /// </summary>
        Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and returns the response as a string.
        /// </summary>
        Task<HttpResponseMessage> PutAsync<TEntity>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and a token, and returns the response as a string.
        /// </summary>
        Task<HttpResponseMessage> PutAsync<TEntity>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and headers, and returns the response as a string.
        /// </summary>
        Task<HttpResponseMessage> PutAsync<TEntity>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request to the specified URL with content, a token, and headers, and returns the response as a string.
        /// </summary>
        Task<HttpResponseMessage> PutAsync<TEntity>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and returns the response as a result entity.
        /// </summary>
        Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and a token, and returns the response as a result entity.
        /// </summary>
        Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and headers, and returns the response as a result entity.
        /// </summary>
        Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request to the specified URL with content, a token, and headers, and returns the response as a result entity.
        /// </summary>
        Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified URL.
        /// </summary>
        Task DeleteAsync(string requestUrl, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified URL with a token.
        /// </summary>
        Task DeleteAsync(string requestUrl, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified URL with headers.
        /// </summary>
        Task DeleteAsync(string requestUrl, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified URL with a token and headers.
        /// </summary>
        Task DeleteAsync(string requestUrl, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified URL and returns the response as a result entity.
        /// </summary>
        Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified URL with a token and returns the response as a result entity.
        /// </summary>
        Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified URL with headers and returns the response as a result entity.
        /// </summary>
        Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified URL with a token and headers, and returns the response as a result entity.
        /// </summary>
        Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates the Google reCAPTCHA response.
        /// </summary>
        Task<CaptcherResponse> ValidateGoogleCaptcharAsync(string secretKey, string recaptchaResponse, string remoteip, string requestUrl = "https://www.google.com/recaptcha/api/siteverify", CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Implementation of the IApiClient interface.
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        private ResponseSnapshot? _lastResponse;

        /// <summary>
        /// Initializes a new instance of the ApiClient class.
        /// </summary>
        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Gets the last completed response for legacy diagnostics. This is not request-specific.
        /// </summary>
        public HttpResponseMessage? LastResponse => Volatile.Read(ref _lastResponse)?.Response;

        /// <summary>
        /// Gets the last completed response body for legacy diagnostics. Concurrent callers
        /// must use their returned result/response, not this property, for application decisions.
        /// </summary>
        public string LastResponseString => Volatile.Read(ref _lastResponse)?.Body ?? string.Empty;

        #region get data

        /// <summary>
        /// Sends a GET request to the specified URL and returns the response as an entity.
        /// </summary>
        public async Task<TEntity> GetAsync<TEntity>(string requestUrl, CancellationToken cancellationToken = default) =>
            await GetAsync<TEntity>(requestUrl, string.Empty, new Dictionary<string, string>(), cancellationToken);

        /// <summary>
        /// Sends a GET request to the specified URL with a token and returns the response as an entity.
        /// </summary>
        public async Task<TEntity> GetAsync<TEntity>(string requestUrl, string token, CancellationToken cancellationToken = default)
        => await GetAsync<TEntity>(requestUrl, token, new Dictionary<string, string>(), cancellationToken);

        /// <summary>
        /// Sends a GET request to the specified URL with headers and returns the response as an entity.
        /// </summary>
        public async Task<TEntity> GetAsync<TEntity>(string requestUrl, Dictionary<string, string> headers, CancellationToken cancellationToken = default)
        => await GetAsync<TEntity>(requestUrl, string.Empty, headers, cancellationToken);

        /// <summary>
        /// Sends a GET request to the specified URL with a token and headers, and returns the response as an entity.
        /// </summary>
        public async Task<TEntity> GetAsync<TEntity>(string requestUrl, string token, Dictionary<string, string> headers, CancellationToken cancellationToken = default)
        {
            var result = await SendAsync(HttpMethod.Get, requestUrl, token, headers, null, true, cancellationToken);
            return Deserialize<TEntity>(result.Body);
        }

        #endregion

        #region post data

        /// <summary>
        /// Sends a POST request to the specified URL with content and returns the response as a string.
        /// </summary>
        public async Task<HttpResponseMessage> PostAsync<TEntity>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PostAsync(requestUrl, content, string.Empty, ensureSuccess);

        /// <summary>
        /// Sends a POST request to the specified URL with content and a token, and returns the response as a string.
        /// </summary>
        public async Task<HttpResponseMessage> PostAsync<TEntity>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PostAsync(requestUrl, content, token, new Dictionary<string, string>(), ensureSuccess);

        /// <summary>
        /// Sends a POST request to the specified URL with content and headers, and returns the response as a string.
        /// </summary>
        public async Task<HttpResponseMessage> PostAsync<TEntity>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PostAsync(requestUrl, content, string.Empty, headers, ensureSuccess);

        /// <summary>
        /// Sends a POST request to the specified URL with content, a token, and headers, and returns the response as a string.
        /// </summary>
        public async Task<HttpResponseMessage> PostAsync<TEntity>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            var result = await SendAsync(HttpMethod.Post, requestUrl, token, headers,
                CreateHttpContent(content), ensureSuccess, cancellationToken);
            return result.Response;
        }

        // ========

        /// <summary>
        /// Sends a POST request to the specified URL with content and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PostAsync<TEntity, TResult>(requestUrl, content, string.Empty, ensureSuccess);

        /// <summary>
        /// Sends a POST request to the specified URL with content and a token, and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PostAsync<TEntity, TResult>(requestUrl, content, token, new Dictionary<string, string>(), ensureSuccess);

        /// <summary>
        /// Sends a POST request to the specified URL with content and headers, and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PostAsync<TEntity, TResult>(requestUrl, content, string.Empty, headers, ensureSuccess);

        /// <summary>
        /// Sends a POST request to the specified URL with content, a token, and headers, and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            var result = await SendAsync(HttpMethod.Post, requestUrl, token, headers,
                CreateHttpContent(content), ensureSuccess, cancellationToken);
            return Deserialize<TResult>(result.Body);
        }

        // ========

        #endregion

        #region put data

        /// <summary>
        /// Sends a PUT request to the specified URL with content and returns the response as a string.
        /// </summary>
        public async Task<HttpResponseMessage> PutAsync<TEntity>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PutAsync(requestUrl, content, string.Empty, ensureSuccess);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and a token, and returns the response as a string.
        /// </summary>
        public async Task<HttpResponseMessage> PutAsync<TEntity>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PutAsync(requestUrl, content, token, new Dictionary<string, string>(), ensureSuccess);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and headers, and returns the response as a string.
        /// </summary>
        public async Task<HttpResponseMessage> PutAsync<TEntity>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PutAsync(requestUrl, content, string.Empty, headers, ensureSuccess);

        /// <summary>
        /// Sends a PUT request to the specified URL with content, a token, and headers, and returns the response as a string.
        /// </summary>
        public async Task<HttpResponseMessage> PutAsync<TEntity>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            var result = await SendAsync(HttpMethod.Put, requestUrl, token, headers,
                CreateHttpContent(content), ensureSuccess, cancellationToken);
            return result.Response;
        }

        // ========

        /// <summary>
        /// Sends a PUT request to the specified URL with content and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PutAsync<TEntity, TResult>(requestUrl, content, string.Empty, ensureSuccess);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and a token, and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PutAsync<TEntity, TResult>(requestUrl, content, token, new Dictionary<string, string>(), ensureSuccess);

        /// <summary>
        /// Sends a PUT request to the specified URL with content and headers, and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await PutAsync<TEntity, TResult>(requestUrl, content, string.Empty, headers, ensureSuccess);

        /// <summary>
        /// Sends a PUT request to the specified URL with content, a token, and headers, and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            var result = await SendAsync(HttpMethod.Put, requestUrl, token, headers,
                CreateHttpContent(content), ensureSuccess, cancellationToken);
            return Deserialize<TResult>(result.Body);
        }

        #endregion

        #region delete data

        /// <summary>
        /// Sends a DELETE request to the specified URL.
        /// </summary>
        public async Task DeleteAsync(string requestUrl, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await DeleteAsync(requestUrl, string.Empty, ensureSuccess);

        /// <summary>
        /// Sends a DELETE request to the specified URL with a token.
        /// </summary>
        public async Task DeleteAsync(string requestUrl, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await DeleteAsync(requestUrl, token, new Dictionary<string, string>(), ensureSuccess);

        /// <summary>
        /// Sends a DELETE request to the specified URL with headers.
        /// </summary>
        public async Task DeleteAsync(string requestUrl, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await DeleteAsync(requestUrl, string.Empty, headers, ensureSuccess);

        /// <summary>
        /// Sends a DELETE request to the specified URL with a token and headers.
        /// </summary>
        public async Task DeleteAsync(string requestUrl, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            await SendAsync(HttpMethod.Delete, requestUrl, token, headers, null, ensureSuccess, cancellationToken);
        }

        // =======

        /// <summary>
        /// Sends a DELETE request to the specified URL and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await DeleteWithResultAsync<TResult>(requestUrl, string.Empty, new Dictionary<string, string>());

        /// <summary>
        /// Sends a DELETE request to the specified URL with a token and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await DeleteWithResultAsync<TResult>(requestUrl, token, new Dictionary<string, string>());

        /// <summary>
        /// Sends a DELETE request to the specified URL with headers and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        => await DeleteWithResultAsync<TResult>(requestUrl, string.Empty, headers);

        /// <summary>
        /// Sends a DELETE request to the specified URL with a token and headers, and returns the response as a result entity.
        /// </summary>
        public async Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            var result = await SendAsync(HttpMethod.Delete, requestUrl, token, headers, null, ensureSuccess, cancellationToken);
            return Deserialize<TResult>(result.Body);
        }

        #endregion

        /// <summary>
        /// Validates the Google reCAPTCHA response.
        /// </summary>
        public async Task<CaptcherResponse> ValidateGoogleCaptcharAsync(
            string secretKey, string recaptchaResponse, string remoteIp,
            string requestUrl = "https://www.google.com/recaptcha/api/siteverify",
            CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>
            {
                {"secret", secretKey},
                {"response", recaptchaResponse},
                {"remoteip", remoteIp}
            };

            return await PostAsync<FormUrlEncodedContent, CaptcherResponse>(
                requestUrl, new FormUrlEncodedContent(parameters), true, cancellationToken
            );
        }

        #region misc funtions

        /// <summary>
        /// Deserializes the specified data to an entity.
        /// </summary>
        private static TEntity Deserialize<TEntity>(string data)
        {
            return JsonConvert.DeserializeObject<TEntity>(data) ??
                throw new DataDeserializationException(typeof(TEntity), data);
        }

        /// <summary>
        /// Creates HTTP content from the specified content.
        /// </summary>
        private static HttpContent CreateHttpContent<T>(T content)
        {
            return new StringContent(
                JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings),
                Encoding.UTF8, "application/json"
            );
        }

        /// <summary>
        /// Gets the JSON serializer settings for Microsoft date format.
        /// </summary>
        private static JsonSerializerSettings MicrosoftDateFormatSettings => new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
        };

        private async Task<ResponseSnapshot> SendAsync(HttpMethod method, string requestUrl,
            string token, Dictionary<string, string> headers, HttpContent? content,
            bool ensureSuccess, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(method, requestUrl) { Content = content };
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
            }
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Remove(header.Key);
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            var response = await _http.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            var result = new ResponseSnapshot(response, await response.Content.ReadAsStringAsync());
            Volatile.Write(ref _lastResponse, result);
            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }
            return result;
        }

        private sealed class ResponseSnapshot
        {
            public ResponseSnapshot(HttpResponseMessage response, string body)
            {
                Response = response;
                Body = body;
            }
            public HttpResponseMessage Response { get; }
            public string Body { get; }
        }

        /// <summary>
        /// Sets the last response.
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public async Task SetLastResponse(HttpResponseMessage httpResponse)
        {
            Volatile.Write(ref _lastResponse,
                new ResponseSnapshot(httpResponse, await httpResponse.Content.ReadAsStringAsync()));
        }

        #endregion
    }
}

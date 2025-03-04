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
        private HttpResponseMessage? _lastResponse;
        private string _lastResponseString = string.Empty;

        /// <summary>
        /// Initializes a new instance of the ApiClient class.
        /// </summary>
        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Gets the last response string.
        /// </summary>
        public HttpResponseMessage? LastResponse => _lastResponse;

        /// <summary>
        /// Gets the last response string.
        /// </summary>
        public string LastResponseString => _lastResponseString;

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
            SetHeaders(headers, token);

            var response = await _http.GetAsync(requestUrl,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken
            );

            await SetLastResponse(response);

            response.EnsureSuccessStatusCode();

            return Deserialize<TEntity>(_lastResponseString);
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
            SetHeaders(headers, token);

            var response = await _http.PostAsync(requestUrl, CreateHttpContent(content), cancellationToken);

            await SetLastResponse(response);

            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            return response;
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
            await PostAsync(requestUrl, content, token, headers, ensureSuccess, cancellationToken);
            return Deserialize<TResult>(_lastResponseString);
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
            SetHeaders(headers, token);

            var response = await _http.PutAsync(
                requestUrl, CreateHttpContent(content), cancellationToken
            );

            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            await SetLastResponse(response);

            return response;
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
            await PutAsync(requestUrl, content, token, headers, ensureSuccess, cancellationToken);
            return Deserialize<TResult>(_lastResponseString);
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
            SetHeaders(headers, token);

            var response = await _http.DeleteAsync(requestUrl, cancellationToken);

            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            await SetLastResponse(response);
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
            await DeleteAsync(requestUrl, token, headers, ensureSuccess, cancellationToken);
            return Deserialize<TResult>(_lastResponseString); // set inside the DeleteAsync method
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

        private void SetHeaders(Dictionary<string, string> headers, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                AddToken(token);
            }

            if (headers?.Count > 0)
            {
                AddHeaders(headers);
            }
        }

        /// <summary>
        /// Adds the specified token to the HTTP headers.
        /// </summary>
        private void AddToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;
            AddHeaders(new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            });
        }

        /// <summary>
        /// Adds the specified headers to the HTTP headers.
        /// </summary>
        private void AddHeaders(Dictionary<string, string> headers)
        {
            if (headers == null) return;
            foreach (var header in headers)
            {
                _http.DefaultRequestHeaders.Remove(header.Key);
                _http.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        /// <summary>
        /// Sets the last response.
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public async Task SetLastResponse(HttpResponseMessage httpResponse)
        {
            _lastResponse = httpResponse;
            _lastResponseString = await httpResponse.Content.ReadAsStringAsync();
        }

        #endregion
    }
}
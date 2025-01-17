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
    public interface IApiClient
    {
        string LastResponseString { get; }

        Task<TEntity> GetAsync<TEntity>(string requestUrl, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync<TEntity>(string requestUrl, string token, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync<TEntity>(string requestUrl, Dictionary<string, string> headers, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync<TEntity>(string requestUrl, string token, Dictionary<string, string> headers, CancellationToken cancellationToken = default);

        Task<string> PostAsync<TEntity>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<string> PostAsync<TEntity>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<string> PostAsync<TEntity>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<string> PostAsync<TEntity>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        Task<string> PutAsync<TEntity>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<string> PutAsync<TEntity>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<string> PutAsync<TEntity>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<string> PutAsync<TEntity>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        Task DeleteAsync(string requestUrl, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task DeleteAsync(string requestUrl, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task DeleteAsync(string requestUrl, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task DeleteAsync(string requestUrl, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);
        Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default);

        Task<CaptcherResponse> ValidateGoogleCaptcharAsync(string secretKey, string recaptchaResponse, string remoteip, string requestUrl = "https://www.google.com/recaptcha/api/siteverify", CancellationToken cancellationToken = default);
    }
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        private string _lastResponseString = string.Empty;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        public string LastResponseString => _lastResponseString;

        #region get data

        public async Task<TEntity> GetAsync<TEntity>(string requestUrl, CancellationToken cancellationToken = default) =>
            await GetAsync<TEntity>(requestUrl, string.Empty, new Dictionary<string, string>(), cancellationToken);

        public async Task<TEntity> GetAsync<TEntity>(string requestUrl, string token, CancellationToken cancellationToken = default) =>
            await GetAsync<TEntity>(requestUrl, token, new Dictionary<string, string>(), cancellationToken);

        public async Task<TEntity> GetAsync<TEntity>(string requestUrl, Dictionary<string, string> headers, CancellationToken cancellationToken = default) =>
            await GetAsync<TEntity>(requestUrl, string.Empty, headers, cancellationToken);

        public async Task<TEntity> GetAsync<TEntity>(string requestUrl, string token, Dictionary<string, string> headers, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(token))
            {
                AddToken(token);
            }

            if (headers?.Count > 0)
            {
                AddHeaders(headers);
            }

            var response = await _http.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            _lastResponseString = await response.Content.ReadAsStringAsync();

            return Deserialize<TEntity>(_lastResponseString);
        }

        #endregion

        #region post data

        public async Task<string> PostAsync<TEntity>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PostAsync(requestUrl, content, string.Empty, ensureSuccess);

        public async Task<string> PostAsync<TEntity>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PostAsync(requestUrl, content, token, new Dictionary<string, string>(), ensureSuccess);

        public async Task<string> PostAsync<TEntity>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PostAsync(requestUrl, content, string.Empty, headers, ensureSuccess);

        public async Task<string> PostAsync<TEntity>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(token))
            {
                AddToken(token);
            }

            if (headers?.Count > 0)
            {
                AddHeaders(headers);
            }

            var response = await _http.PostAsync(requestUrl, CreateHttpContent(content), cancellationToken);

            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            _lastResponseString = await response.Content.ReadAsStringAsync();

            return _lastResponseString;
        }

        // ========

        public async Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PostAsync<TEntity, TResult>(requestUrl, content, string.Empty, ensureSuccess);

        public async Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PostAsync<TEntity, TResult>(requestUrl, content, token, new Dictionary<string, string>(), ensureSuccess);

        public async Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PostAsync<TEntity, TResult>(requestUrl, content, string.Empty, headers, ensureSuccess);

        public async Task<TResult> PostAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            _lastResponseString = await PostAsync(requestUrl, content, token, headers, ensureSuccess, cancellationToken);
            return Deserialize<TResult>(_lastResponseString);
        }

        // ========

        #endregion

        #region put data

        public async Task<string> PutAsync<TEntity>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PutAsync(requestUrl, content, string.Empty, ensureSuccess);

        public async Task<string> PutAsync<TEntity>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PutAsync(requestUrl, content, token, new Dictionary<string, string>(), ensureSuccess);

        public async Task<string> PutAsync<TEntity>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PutAsync(requestUrl, content, string.Empty, headers, ensureSuccess);

        public async Task<string> PutAsync<TEntity>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(token))
            {
                AddToken(token);
            }

            if (headers?.Count > 0)
            {
                AddHeaders(headers);
            }

            var response = await _http.PutAsync(requestUrl, CreateHttpContent(content), cancellationToken);

            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            _lastResponseString = await response.Content.ReadAsStringAsync();

            return _lastResponseString;
        }

        // ========

        public async Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PutAsync<TEntity, TResult>(requestUrl, content, string.Empty, ensureSuccess);

        public async Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PutAsync<TEntity, TResult>(requestUrl, content, token, new Dictionary<string, string>(), ensureSuccess);

        public async Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await PutAsync<TEntity, TResult>(requestUrl, content, string.Empty, headers, ensureSuccess);

        public async Task<TResult> PutAsync<TEntity, TResult>(string requestUrl, TEntity content, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            _lastResponseString = await PutAsync(requestUrl, content, token, headers, ensureSuccess, cancellationToken);
            return Deserialize<TResult>(_lastResponseString);
        }

        #endregion

        #region delete data

        public async Task DeleteAsync(string requestUrl, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await DeleteAsync(requestUrl, string.Empty, ensureSuccess);

        public async Task DeleteAsync(string requestUrl, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await DeleteAsync(requestUrl, token, new Dictionary<string, string>(), ensureSuccess);

        public async Task DeleteAsync(string requestUrl, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await DeleteAsync(requestUrl, string.Empty, headers, ensureSuccess);

        public async Task DeleteAsync(string requestUrl, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(token))
            {
                AddToken(token);
            }

            if (headers?.Count > 0)
            {
                AddHeaders(headers);
            }

            var response = await _http.DeleteAsync(requestUrl, cancellationToken);

            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            _lastResponseString = await response.Content.ReadAsStringAsync();
        }

        // =======

        public async Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await DeleteWithResultAsync<TResult>(requestUrl, string.Empty, new Dictionary<string, string>());

        public async Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, string token, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await DeleteWithResultAsync<TResult>(requestUrl, token, new Dictionary<string, string>());

        public async Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default) =>
            await DeleteWithResultAsync<TResult>(requestUrl, string.Empty, headers);

        public async Task<TResult> DeleteWithResultAsync<TResult>(string requestUrl, string token, Dictionary<string, string> headers, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(requestUrl, token, headers, ensureSuccess, cancellationToken);
            return Deserialize<TResult>(_lastResponseString); // set inside the DeleteAsync method
        }

        #endregion

        public async Task<CaptcherResponse> ValidateGoogleCaptcharAsync(string secretKey, string response, 
            string remoteIp, string requestUrl = "https://www.google.com/recaptcha/api/siteverify", 
            CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>
            {
                {"secret", secretKey},
                {"response", response},
                {"remoteip", remoteIp}
            };

            return await PostAsync<FormUrlEncodedContent, CaptcherResponse>(
                requestUrl, new FormUrlEncodedContent(parameters), true, cancellationToken
            );
        }

        #region misc funtions

        private static TEntity Deserialize<TEntity>(string data)
        {
            return JsonConvert.DeserializeObject<TEntity>(data) ??
                throw new DataDeserializationException(typeof(TEntity));
        }

        private HttpContent CreateHttpContent<T>(T content)
        {
            return new StringContent(
                JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings), Encoding.UTF8, "application/json"
            );
        }

        private static JsonSerializerSettings MicrosoftDateFormatSettings => new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
        };

        private void AddToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;
            AddHeaders(new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            });
        }

        private void AddHeaders(Dictionary<string, string> headers)
        {
            if (headers == null) return;
            foreach (var header in headers)
            {
                _http.DefaultRequestHeaders.Remove(header.Key);
                _http.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        #endregion
    }
}
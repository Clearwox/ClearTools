using Clear;
using Clear.Models;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace ClearTools.Tests
{
    public class ApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly ApiClient _apiClient;

        public ApiClientTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _apiClient = new ApiClient(_httpClient);
        }

        [Fact]
        public async Task GetAsync_ValidRequest_ReturnsEntity()
        {
            // Arrange
            var requestUrl = "https://api.example.com/data";
            var responseData = ReturnData.Create();
            var responseJson = JsonConvert.SerializeObject(responseData);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            // Act
            var result = await _apiClient.GetAsync<ReturnData>(requestUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(responseData.Id, result.Id);
            Assert.Equal(responseData.Name, result.Name);
        }

        [Fact]
        public async Task PostAsync_ValidRequest_ReturnsResponseString()
        {
            // Arrange
            var requestUrl = "https://api.example.com/data";
            var requestData = new { Id = 1, Name = "Test" };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(requestData))
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiClient.PostAsync(requestUrl, requestData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response, result);
        }

        [Fact]
        public async Task PutAsync_ValidRequest_ReturnsResponseString()
        {
            // Arrange
            var requestUrl = "https://api.example.com/data";
            var requestData = new { Id = 1, Name = "Test" };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(requestData))
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiClient.PutAsync(requestUrl, requestData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response, result);
        }

        [Fact]
        public async Task DeleteAsync_ValidRequest_EnsuresSuccess()
        {
            // Arrange
            var requestUrl = "https://api.example.com/data";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                });

            // Act
            await _apiClient.DeleteAsync(requestUrl);

            // Assert
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ValidateGoogleCaptcharAsync_ValidRequest_ReturnsCaptcherResponse()
        {
            // Arrange
            var requestUrl = "https://www.google.com/recaptcha/api/siteverify";
            var secretKey = "secret";
            var recaptchaResponse = "response";
            var remoteIp = "127.0.0.1";
            var responseData = new CaptcherResponse { Success = true };
            var responseJson = JsonConvert.SerializeObject(responseData);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            // Act
            var result = await _apiClient.ValidateGoogleCaptcharAsync(secretKey, recaptchaResponse, remoteIp);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }
    }

    class ReturnData
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        internal static ReturnData Create() => new() { Id = 1, Name = "Test" };
    }
}

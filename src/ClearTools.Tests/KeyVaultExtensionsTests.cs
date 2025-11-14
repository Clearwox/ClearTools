using Azure;
using Azure.Security.KeyVault.Secrets;
using ClearTools.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace ClearTools.Tests;

public class KeyVaultExtensionsTests
{
    private class TestSettings
    {
        public string? ConnectionString { get; set; }
        
        [KeyVaultKey("api-key")]
        public string? ApiKey { get; set; }
        
        public int MaxRetryCount { get; set; }
        
        public bool EnableLogging { get; set; }
        
        public double? OptionalValue { get; set; }
    }

    private class EmptySettings
    {
        public string? EmptyProperty { get; set; }
    }

    [Fact]
    public void KeyVaultKeyAttribute_ShouldSetKeyName()
    {
        // Arrange & Act
        var attribute = new KeyVaultKeyAttribute("test-key");

        // Assert
        Assert.Equal("test-key", attribute.KeyName);
    }

    [Fact]
    public void AddKeyVaultConfiguration_WithDevelopmentEnvironment_ShouldSkipKeyVault()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        var mockEnvironment = new Mock<IHostEnvironment>();
        // Mock the EnvironmentName property instead of IsDevelopment() extension method
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);
        
        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Services).Returns(services);

        // Act
        var result = mockBuilder.Object.AddKeyVaultForWebApplication<TestSettings>(
            "https://test.vault.azure.net/", 
            out var settings, 
            skipDevelopment: false
        );

        // Assert
        Assert.NotNull(settings);
        Assert.NotNull(result);
        Assert.Null(settings.ConnectionString); // Should be empty in development
    }

    [Fact]
    public void AddKeyVaultConfiguration_WithConfigurationValues_ShouldPopulateSettings()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            ["ConnectionString"] = "Server=localhost;Database=Test;",
            ["api-key"] = "test-api-key",
            ["MaxRetryCount"] = "5",
            ["EnableLogging"] = "true",
            ["OptionalValue"] = "3.14"
        };

        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        
        // Setup configuration indexer
        mockConfiguration.Setup(x => x["ConnectionString"]).Returns("Server=localhost;Database=Test;");
        mockConfiguration.Setup(x => x["api-key"]).Returns("test-api-key");
        mockConfiguration.Setup(x => x["MaxRetryCount"]).Returns("5");
        mockConfiguration.Setup(x => x["EnableLogging"]).Returns("true");
        mockConfiguration.Setup(x => x["OptionalValue"]).Returns("3.14");
        
        var mockEnvironment = new Mock<IHostEnvironment>();
        // Mock the EnvironmentName property instead of IsDevelopment() extension method
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);
        
        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Services).Returns(services);

        // Act
        var result = mockBuilder.Object.AddKeyVaultForWebApplication<TestSettings>(
            "https://test.vault.azure.net/", 
            out var settings
        );

        // Assert
        Assert.NotNull(settings);
        Assert.Equal("Server=localhost;Database=Test;", settings.ConnectionString);
        Assert.Equal("test-api-key", settings.ApiKey);
        Assert.Equal(5, settings.MaxRetryCount);
        Assert.True(settings.EnableLogging);
        Assert.Equal(3.14, settings.OptionalValue);
        
        // Verify service registration
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TestSettings));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
    }

    [Fact]
    public async Task CreateSettingsFromKeyVaultAsync_WithValidSecrets_ShouldPopulateSettings()
    {
        // Arrange
        var mockSecretClient = new Mock<SecretClient>();

        // Setup mock responses for different secrets
        var connectionStringSecret = CreateMockSecret("ConnectionString", "Server=test;Database=MyDB;");
        var apiKeySecret = CreateMockSecret("api-key", "secret-api-key");
        var retryCountSecret = CreateMockSecret("MaxRetryCount", "3");
        var loggingSecret = CreateMockSecret("EnableLogging", "false");

        mockSecretClient.Setup(x => x.GetSecretAsync("ConnectionString", null, default))
            .ReturnsAsync(Response.FromValue(connectionStringSecret, Mock.Of<Response>()));
        
        mockSecretClient.Setup(x => x.GetSecretAsync("api-key", null, default))
            .ReturnsAsync(Response.FromValue(apiKeySecret, Mock.Of<Response>()));
        
        mockSecretClient.Setup(x => x.GetSecretAsync("MaxRetryCount", null, default))
            .ReturnsAsync(Response.FromValue(retryCountSecret, Mock.Of<Response>()));
        
        mockSecretClient.Setup(x => x.GetSecretAsync("EnableLogging", null, default))
            .ReturnsAsync(Response.FromValue(loggingSecret, Mock.Of<Response>()));

        // Setup 404 for OptionalValue to test missing secret handling
        mockSecretClient.Setup(x => x.GetSecretAsync("OptionalValue", null, default))
            .ThrowsAsync(new RequestFailedException(404, "Secret not found"));

        // Use reflection to call private method for testing
        var method = typeof(KeyVaultExtensions).GetMethod("CreateSettings", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
            new[] { typeof(SecretClient) });
        
        Assert.NotNull(method);
        var genericMethod = method.MakeGenericMethod(typeof(TestSettings));

        // Act
        var task = (Task<TestSettings>)genericMethod.Invoke(null, new object[] { mockSecretClient.Object })!;
        var settings = await task;

        // Assert
        Assert.NotNull(settings);
        Assert.Equal("Server=test;Database=MyDB;", settings.ConnectionString);
        Assert.Equal("secret-api-key", settings.ApiKey);
        Assert.Equal(3, settings.MaxRetryCount);
        Assert.False(settings.EnableLogging);
        Assert.Null(settings.OptionalValue); // Should be null due to 404
    }

    [Fact]
    public async Task CreateSettingsFromKeyVaultAsync_WithUnauthorizedAccess_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var mockSecretClient = new Mock<SecretClient>();
        
        mockSecretClient.Setup(x => x.GetSecretAsync("ConnectionString", null, default))
            .ThrowsAsync(new RequestFailedException(403, "Forbidden"));

        // Use reflection to call private method
        var method = typeof(KeyVaultExtensions).GetMethod("CreateSettings", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
            new[] { typeof(SecretClient) });
        
        Assert.NotNull(method);
        var genericMethod = method.MakeGenericMethod(typeof(TestSettings));

        // Act & Assert
        var task = (Task<TestSettings>)genericMethod.Invoke(null, new object[] { mockSecretClient.Object })!;
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => task);
        
        Assert.Contains("Access denied to Key Vault secret 'ConnectionString'", exception.Message);
    }

    [Fact]
    public void AddKeyVaultConfigurationDirect_WithEnvironmentVariables_ShouldPopulateFromEnvironment()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ConnectionString", "env-connection");
        Environment.SetEnvironmentVariable("api-key", "env-api-key");
        Environment.SetEnvironmentVariable("MaxRetryCount", "10");
        
        try
        {
            var services = new ServiceCollection();
            var mockConfiguration = new Mock<IConfigurationManager>();
            var mockEnvironment = new Mock<IHostEnvironment>();
            // Mock the EnvironmentName property instead of IsDevelopment() extension method
            mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);
            
            var mockBuilder = new Mock<IHostApplicationBuilder>();
            mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);
            mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
            mockBuilder.Setup(x => x.Services).Returns(services);

            // Act - skipDevelopment defaults to true, which skips Key Vault and uses environment variables
            var result = mockBuilder.Object.AddKeyVaultForAzureFunctions<TestSettings>(
                "https://test.vault.azure.net/", 
                out var settings
            );

            // Assert
            Assert.NotNull(settings);
            Assert.Equal("env-connection", settings.ConnectionString);
            Assert.Equal("env-api-key", settings.ApiKey);
            Assert.Equal(10, settings.MaxRetryCount);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("ConnectionString", null);
            Environment.SetEnvironmentVariable("api-key", null);
            Environment.SetEnvironmentVariable("MaxRetryCount", null);
        }
    }

    [Fact]
    public void SetPropertyValue_WithInvalidTypeConversion_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var settings = new TestSettings();
        var property = typeof(TestSettings).GetProperty(nameof(TestSettings.MaxRetryCount))!;
        
        // Use reflection to call private method
        var method = typeof(KeyVaultExtensions).GetMethod("SetPropertyValue", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        Assert.NotNull(method);

        // Act & Assert
        var exception = Assert.Throws<System.Reflection.TargetInvocationException>(() =>
            method.Invoke(null, new object[] { property, settings, "invalid-number" }));
        
        Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Contains("Failed to set property 'MaxRetryCount'", exception.InnerException!.Message);
    }

    [Fact]
    public void GetKeyVaultKey_WithAttribute_ShouldReturnAttributeValue()
    {
        // Arrange
        var property = typeof(TestSettings).GetProperty(nameof(TestSettings.ApiKey))!;
        
        // Use reflection to call private method
        var method = typeof(KeyVaultExtensions).GetMethod("GetKeyVaultKey", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        Assert.NotNull(method);

        // Act
        var result = (string)method.Invoke(null, new object[] { property })!;

        // Assert
        Assert.Equal("api-key", result);
    }

    [Fact]
    public void GetKeyVaultKey_WithoutAttribute_ShouldReturnPropertyName()
    {
        // Arrange
        var property = typeof(TestSettings).GetProperty(nameof(TestSettings.ConnectionString))!;
        
        // Use reflection to call private method
        var method = typeof(KeyVaultExtensions).GetMethod("GetKeyVaultKey", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        Assert.NotNull(method);

        // Act
        var result = (string)method.Invoke(null, new object[] { property })!;

        // Assert
        Assert.Equal("ConnectionString", result);
    }

    private static KeyVaultSecret CreateMockSecret(string name, string value)
    {
        var secretProperties = SecretModelFactory.SecretProperties(name: name);
        return SecretModelFactory.KeyVaultSecret(secretProperties, value);
    }
}

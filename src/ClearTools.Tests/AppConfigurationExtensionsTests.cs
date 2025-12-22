using Azure;
using Azure.Data.AppConfiguration;
using ClearTools.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace ClearTools.Tests;

public class AppConfigurationExtensionsTests
{
    private class TestSettings
    {
        public string? DatabaseConnectionString { get; set; }
        
        [AppConfigurationKey("api-key")]
        public string? ApiKey { get; set; }
        
        public int MaxRetryCount { get; set; }
        
        public bool EnableLogging { get; set; }
        
        public double? OptionalValue { get; set; }
        
        [FeatureFlag("enable-new-ui")]
        public bool NewUiEnabled { get; set; }
        
        [FeatureFlag("beta-features")]
        public bool? BetaFeaturesEnabled { get; set; }
    }

    private class InvalidFeatureFlagSettings
    {
        [FeatureFlag("invalid-flag")]
        public string? InvalidFlag { get; set; }
    }

    private class EmptySettings
    {
        public string? EmptyProperty { get; set; }
    }

    [Fact]
    public void AppConfigurationKeyAttribute_ShouldSetKeyName()
    {
        // Arrange & Act
        var attribute = new AppConfigurationKeyAttribute("test-key");

        // Assert
        Assert.Equal("test-key", attribute.KeyName);
    }

    [Fact]
    public void FeatureFlagAttribute_ShouldSetFlagName()
    {
        // Arrange & Act
        var attribute = new FeatureFlagAttribute("test-flag");

        // Assert
        Assert.Equal("test-flag", attribute.FlagName);
    }

    [Fact]
    public void AppConfigurationRefreshOptions_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var options = new AppConfigurationRefreshOptions();

        // Assert
        Assert.False(options.EnableRefresh);
        Assert.Equal(TimeSpan.FromMinutes(1), options.RefreshInterval);
        Assert.Empty(options.SentinelKeys);
        Assert.Null(options.OnRefreshError);
    }

    [Fact]
    public void AppConfigurationRefreshOptions_ShouldAllowCustomValues()
    {
        // Arrange
        Exception? capturedError = null;
        var errorHandler = new Action<Exception>(ex => capturedError = ex);

        // Act
        var options = new AppConfigurationRefreshOptions
        {
            EnableRefresh = true,
            RefreshInterval = TimeSpan.FromMinutes(5),
            SentinelKeys = new[] { "Config:Version", "App:Sentinel" },
            OnRefreshError = errorHandler
        };

        // Assert
        Assert.True(options.EnableRefresh);
        Assert.Equal(TimeSpan.FromMinutes(5), options.RefreshInterval);
        Assert.Equal(2, options.SentinelKeys.Length);
        Assert.Equal("Config:Version", options.SentinelKeys[0]);
        Assert.Equal("App:Sentinel", options.SentinelKeys[1]);
        Assert.NotNull(options.OnRefreshError);
    }

    [Fact]
    public void AddAppConfigurationForWebApplication_WithDevelopmentEnvironment_ShouldSkipAppConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act
        mockBuilder.Object.AddAppConfigurationForWebApplication<EmptySettings>(
            new Uri("https://test.azconfig.io"),
            out var settings,
            label: null,
            keyFilter: null,
            skipDevelopment: true,
            credential: null);

        // Assert
        Assert.NotNull(settings);
        var registeredSettings = services.BuildServiceProvider().GetService<EmptySettings>();
        Assert.NotNull(registeredSettings);
    }

    [Fact]
    public void AddAppConfigurationForAzureFunctions_WithDevelopmentEnvironment_ShouldUseEnvironmentVariables()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        try
        {
            // Set environment variables for testing
            Environment.SetEnvironmentVariable("DatabaseConnectionString", "test-connection");
            Environment.SetEnvironmentVariable("api-key", "test-api-key");
            Environment.SetEnvironmentVariable("MaxRetryCount", "5");
            Environment.SetEnvironmentVariable("EnableLogging", "true");

            // Act
            mockBuilder.Object.AddAppConfigurationForAzureFunctions<TestSettings>(
                "https://test.azconfig.io",
                out var settings,
                skipDevelopment: true);

            // Assert
            Assert.NotNull(settings);
            Assert.Equal("test-connection", settings.DatabaseConnectionString);
            Assert.Equal("test-api-key", settings.ApiKey);
            Assert.Equal(5, settings.MaxRetryCount);
            Assert.True(settings.EnableLogging);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DatabaseConnectionString", null);
            Environment.SetEnvironmentVariable("api-key", null);
            Environment.SetEnvironmentVariable("MaxRetryCount", null);
            Environment.SetEnvironmentVariable("EnableLogging", null);
        }
    }

    [Fact]
    public void CreateSettings_FromIConfiguration_ShouldMapProperties()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "DatabaseConnectionString", "Server=test" },
            { "api-key", "secret-key" },
            { "MaxRetryCount", "3" },
            { "EnableLogging", "true" },
            { "OptionalValue", "3.14" }
        };

        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        foreach (var kvp in configData)
        {
            mockConfiguration.Setup(x => x[kvp.Key]).Returns(kvp.Value);
        }
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act
        mockBuilder.Object.AddAppConfigurationForWebApplication<TestSettings>(
            "test-connection-string",
            out var settings,
            skipDevelopment: false);

        // Assert
        Assert.NotNull(settings);
        Assert.Equal("Server=test", settings.DatabaseConnectionString);
        Assert.Equal("secret-key", settings.ApiKey);
        Assert.Equal(3, settings.MaxRetryCount);
        Assert.True(settings.EnableLogging);
        Assert.Equal(3.14, settings.OptionalValue);
    }

    [Fact]
    public void CreateSettings_WithFeatureFlagOnNonBoolProperty_ShouldThrowException()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "FeatureManagement:invalid-flag", "true" }
        };

        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        foreach (var kvp in configData)
        {
            mockConfiguration.Setup(x => x[kvp.Key]).Returns(kvp.Value);
        }
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            mockBuilder.Object.AddAppConfigurationForWebApplication<InvalidFeatureFlagSettings>(
                "test-connection-string",
                out var settings,
                skipDevelopment: false));

        Assert.Contains("InvalidFlag", exception.Message);
        Assert.Contains("FeatureFlag", exception.Message);
        Assert.Contains("bool", exception.Message);
    }

    [Fact]
    public void CreateSettings_WithFeatureFlags_ShouldMapBooleanProperties()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "DatabaseConnectionString", "Server=test" },
            { "FeatureManagement:enable-new-ui", "true" },
            { "FeatureManagement:beta-features", "false" }
        };

        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        foreach (var kvp in configData)
        {
            mockConfiguration.Setup(x => x[kvp.Key]).Returns(kvp.Value);
        }
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act
        mockBuilder.Object.AddAppConfigurationForWebApplication<TestSettings>(
            "test-connection-string",
            out var settings,
            skipDevelopment: false);

        // Assert
        Assert.NotNull(settings);
        Assert.True(settings.NewUiEnabled);
        Assert.False(settings.BetaFeaturesEnabled);
    }

    [Fact]
    public void CreateSettings_WithMissingValues_ShouldSkipProperties()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "DatabaseConnectionString", "Server=test" }
            // Other properties missing
        };

        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        foreach (var kvp in configData)
        {
            mockConfiguration.Setup(x => x[kvp.Key]).Returns(kvp.Value);
        }
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act
        mockBuilder.Object.AddAppConfigurationForWebApplication<TestSettings>(
            "test-connection-string",
            out var settings,
            skipDevelopment: false);

        // Assert
        Assert.NotNull(settings);
        Assert.Equal("Server=test", settings.DatabaseConnectionString);
        Assert.Null(settings.ApiKey);
        Assert.Equal(0, settings.MaxRetryCount); // Default value
        Assert.False(settings.EnableLogging); // Default value
        Assert.Null(settings.OptionalValue);
    }

    [Fact]
    public void CreateSettings_WithInvalidTypeConversion_ShouldThrowException()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "MaxRetryCount", "not-a-number" }
        };

        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        foreach (var kvp in configData)
        {
            mockConfiguration.Setup(x => x[kvp.Key]).Returns(kvp.Value);
        }
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            mockBuilder.Object.AddAppConfigurationForWebApplication<TestSettings>(
                "test-connection-string",
                out var settings,
                skipDevelopment: false));

        Assert.Contains("MaxRetryCount", exception.Message);
    }

    [Fact]
    public void CreateSettings_WithNullableTypes_ShouldConvertCorrectly()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "OptionalValue", "2.5" }
        };

        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        foreach (var kvp in configData)
        {
            mockConfiguration.Setup(x => x[kvp.Key]).Returns(kvp.Value);
        }
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act
        mockBuilder.Object.AddAppConfigurationForWebApplication<TestSettings>(
            "test-connection-string",
            out var settings,
            skipDevelopment: false);

        // Assert
        Assert.NotNull(settings);
        Assert.Equal(2.5, settings.OptionalValue);
    }

    [Fact]
    public void AppConfigurationExtensions_ShouldRegisterSettingsAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act
        mockBuilder.Object.AddAppConfigurationForWebApplication<EmptySettings>(
            "test-connection-string",
            out var settings,
            skipDevelopment: false);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var registeredSettings1 = serviceProvider.GetService<EmptySettings>();
        var registeredSettings2 = serviceProvider.GetService<EmptySettings>();
        
        Assert.NotNull(registeredSettings1);
        Assert.NotNull(registeredSettings2);
        Assert.Same(registeredSettings1, registeredSettings2); // Should be same instance (singleton)
    }

    [Fact]
    public void CreateSettingsFromEnvironment_WithFeatureFlags_ShouldMapCorrectly()
    {
        try
        {
            // Arrange
            Environment.SetEnvironmentVariable("DatabaseConnectionString", "env-connection");
            Environment.SetEnvironmentVariable("enable-new-ui", "true");
            Environment.SetEnvironmentVariable("beta-features", "false");

            var services = new ServiceCollection();
            var mockConfiguration = new Mock<IConfigurationManager>();
            var mockEnvironment = new Mock<IHostEnvironment>();
            mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);

            var mockBuilder = new Mock<IHostApplicationBuilder>();
            mockBuilder.Setup(x => x.Services).Returns(services);
            mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
            mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

            // Act
            mockBuilder.Object.AddAppConfigurationForAzureFunctions<TestSettings>(
                "https://test.azconfig.io",
                out var settings,
                skipDevelopment: true);

            // Assert
            Assert.NotNull(settings);
            Assert.Equal("env-connection", settings.DatabaseConnectionString);
            Assert.True(settings.NewUiEnabled);
            Assert.False(settings.BetaFeaturesEnabled);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DatabaseConnectionString", null);
            Environment.SetEnvironmentVariable("enable-new-ui", null);
            Environment.SetEnvironmentVariable("beta-features", null);
        }
    }

    [Fact]
    public void AddAppConfigurationForWebApplication_WithConnectionString_ShouldReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act
        var result = mockBuilder.Object.AddAppConfigurationForWebApplication<EmptySettings>(
            "Endpoint=https://test.azconfig.io;Id=test;Secret=secret",
            out var settings,
            skipDevelopment: true);

        // Assert
        Assert.NotNull(result);
        Assert.Same(mockBuilder.Object, result);
    }

    [Fact]
    public void AddAppConfigurationForAzureFunctions_WithConnectionString_ShouldReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfigurationManager>();
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);

        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(x => x.Services).Returns(services);
        mockBuilder.Setup(x => x.Configuration).Returns(mockConfiguration.Object);
        mockBuilder.Setup(x => x.Environment).Returns(mockEnvironment.Object);

        // Act
        var result = mockBuilder.Object.AddAppConfigurationForAzureFunctions<EmptySettings>(
            "Endpoint=https://test.azconfig.io;Id=test;Secret=secret",
            out var settings,
            skipDevelopment: true);

        // Assert
        Assert.NotNull(result);
        Assert.Same(mockBuilder.Object, result);
    }

    [Fact]
    public void RefreshOptions_WithNullErrorHandler_ShouldBeSilent()
    {
        // Arrange
        var options = new AppConfigurationRefreshOptions
        {
            EnableRefresh = true,
            RefreshInterval = TimeSpan.FromMinutes(2),
            SentinelKeys = new[] { "Config:Version" },
            OnRefreshError = null // Silent by default
        };

        // Act & Assert
        Assert.Null(options.OnRefreshError);
        // No exception should be thrown when error handler is null
    }

    [Fact]
    public void RefreshOptions_WithCustomErrorHandler_ShouldCaptureErrors()
    {
        // Arrange
        Exception? capturedError = null;
        var testException = new Exception("Test error");

        var options = new AppConfigurationRefreshOptions
        {
            EnableRefresh = true,
            OnRefreshError = ex => capturedError = ex
        };

        // Act
        options.OnRefreshError?.Invoke(testException);

        // Assert
        Assert.NotNull(capturedError);
        Assert.Same(testException, capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }
}

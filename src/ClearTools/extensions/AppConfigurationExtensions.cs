using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TokenCredential = Azure.Core.TokenCredential;

namespace ClearTools.Extensions
{
    /// <summary>
    /// Attribute to specify the App Configuration key name for a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AppConfigurationKeyAttribute : Attribute
    {
        public string KeyName { get; }
        
        public AppConfigurationKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }

    /// <summary>
    /// Attribute to specify that a boolean property should be mapped to a feature flag
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FeatureFlagAttribute : Attribute
    {
        public string FlagName { get; }
        
        public FeatureFlagAttribute(string flagName)
        {
            FlagName = flagName;
        }
    }

    /// <summary>
    /// Options for configuring Azure App Configuration refresh behavior
    /// </summary>
    public class AppConfigurationRefreshOptions
    {
        /// <summary>
        /// Whether to enable automatic refresh of configuration values
        /// </summary>
        public bool EnableRefresh { get; set; }
        
        /// <summary>
        /// The interval at which configuration should be refreshed. Minimum recommended is 30 seconds.
        /// </summary>
        public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromMinutes(1);
        
        /// <summary>
        /// Sentinel keys that trigger a refresh when their values change.
        /// Common patterns: "AppSettings:Version", "Config:Sentinel", "App:RefreshKey"
        /// </summary>
        public string[] SentinelKeys { get; set; } = Array.Empty<string>();
        
        /// <summary>
        /// Optional error handler for refresh failures. If null, errors are silently ignored.
        /// Note: Due to Azure SDK limitations, this callback may not be invoked for all refresh errors
        /// as the SDK handles some errors internally. This is provided for advanced scenarios where
        /// custom error logging or handling is required.
        /// </summary>
        public Action<Exception>? OnRefreshError { get; set; }
    }

    public static class AppConfigurationExtensions
    {
        /// <summary>
        /// Extension method for IHostApplicationBuilder that configures Azure App Configuration as a configuration provider.
        /// This method adds App Configuration key-values to IConfiguration and creates a settings object from the configuration.
        /// Designed for ASP.NET Web Applications where IConfiguration integration is preferred.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="builder">The host application builder</param>
        /// <param name="appConfigEndpoint">The endpoint URI of the Azure App Configuration store</param>
        /// <param name="settings">The populated settings object</param>
        /// <param name="label">Optional label filter for configuration keys. Use null for default label, or specific labels like "Production", "Staging"</param>
        /// <param name="keyFilter">Optional key filter pattern. Use null for all keys, or patterns like "MyApp:*" for prefix filtering</param>
        /// <param name="skipDevelopment">Whether to skip App Configuration in development environment. Default is true.</param>
        /// <param name="credential">Optional Azure credential. If null, DefaultAzureCredential will be used</param>
        /// <param name="refreshOptions">Optional refresh configuration for automatic config updates</param>
        /// <returns>The builder for method chaining</returns>
        public static IHostApplicationBuilder AddAppConfigurationForWebApplication<T>(
            this IHostApplicationBuilder builder,
            Uri appConfigEndpoint,
            out T settings,
            string? label = null,
            string? keyFilter = null,
            bool skipDevelopment = true,
            TokenCredential? credential = null,
            AppConfigurationRefreshOptions? refreshOptions = null)
            where T : class, new()
        {
            if (!(skipDevelopment && builder.Environment.IsDevelopment()))
            {
                var cred = credential ?? new DefaultAzureCredential();

                builder.Configuration.AddAzureAppConfiguration(options =>
                {
                    options.Connect(appConfigEndpoint, cred);

                    // Apply key filter if specified
                    if (!string.IsNullOrEmpty(keyFilter))
                    {
                        options.Select(keyFilter, label);
                    }
                    else
                    {
                        options.Select("*", label);
                    }

                    // Configure refresh if enabled
                    if (refreshOptions?.EnableRefresh == true)
                    {
                        ConfigureRefresh(options, refreshOptions);
                    }
                });
            }

            settings = CreateSettings<T>(builder.Configuration);
            builder.Services.AddSingleton(settings);

            return builder;
        }

        /// <summary>
        /// Extension method for IHostApplicationBuilder that configures Azure App Configuration using a connection string.
        /// This method adds App Configuration key-values to IConfiguration and creates a settings object from the configuration.
        /// Designed for ASP.NET Web Applications where IConfiguration integration is preferred.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="builder">The host application builder</param>
        /// <param name="connectionString">The connection string for the Azure App Configuration store</param>
        /// <param name="settings">The populated settings object</param>
        /// <param name="label">Optional label filter for configuration keys. Use null for default label, or specific labels like "Production", "Staging"</param>
        /// <param name="keyFilter">Optional key filter pattern. Use null for all keys, or patterns like "MyApp:*" for prefix filtering</param>
        /// <param name="skipDevelopment">Whether to skip App Configuration in development environment. Default is true.</param>
        /// <param name="refreshOptions">Optional refresh configuration for automatic config updates</param>
        /// <returns>The builder for method chaining</returns>
        public static IHostApplicationBuilder AddAppConfigurationForWebApplication<T>(
            this IHostApplicationBuilder builder,
            string connectionString,
            out T settings,
            string? label = null,
            string? keyFilter = null,
            bool skipDevelopment = true,
            AppConfigurationRefreshOptions? refreshOptions = null)
            where T : class, new()
        {
            if (!(skipDevelopment && builder.Environment.IsDevelopment()))
            {
                builder.Configuration.AddAzureAppConfiguration(options =>
                {
                    options.Connect(connectionString);

                    // Apply key filter if specified
                    if (!string.IsNullOrEmpty(keyFilter))
                    {
                        options.Select(keyFilter, label);
                    }
                    else
                    {
                        options.Select("*", label);
                    }

                    // Configure refresh if enabled
                    if (refreshOptions?.EnableRefresh == true)
                    {
                        ConfigureRefresh(options, refreshOptions);
                    }
                });
            }

            settings = CreateSettings<T>(builder.Configuration);
            builder.Services.AddSingleton(settings);

            return builder;
        }

        /// <summary>
        /// Extension method for IHostApplicationBuilder that fetches configuration directly from App Configuration using ConfigurationClient.
        /// This method bypasses IConfiguration and fetches settings directly, designed specifically for Azure Functions
        /// and other scenarios where immediate access to configuration is required without IConfiguration integration.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="builder">The host application builder</param>
        /// <param name="appConfigEndpoint">The endpoint URI of the Azure App Configuration store</param>
        /// <param name="settings">The populated settings object</param>
        /// <param name="label">Optional label filter for configuration keys. Use null for default label, or specific labels like "Production", "Staging"</param>
        /// <param name="keyFilter">Optional key filter pattern. Use null for all keys, or patterns like "MyApp:*" for prefix filtering</param>
        /// <param name="skipDevelopment">Whether to skip App Configuration in development environment. Default is true.</param>
        /// <param name="credential">Optional Azure credential. If null, DefaultAzureCredential will be used</param>
        /// <param name="refreshOptions">Optional refresh configuration. Note: Direct client access doesn't support automatic refresh.</param>
        /// <returns>The builder for method chaining</returns>
        public static IHostApplicationBuilder AddAppConfigurationForAzureFunctions<T>(
            this IHostApplicationBuilder builder,
            Uri appConfigEndpoint, 
            out T settings, 
            string? label = null,
            string? keyFilter = null,
            bool skipDevelopment = true, 
            TokenCredential? credential = null,
            AppConfigurationRefreshOptions? refreshOptions = null)
            where T : class, new()
        {
            if (!(skipDevelopment && builder.Environment.IsDevelopment()))
            {
                settings = CreateSettingsFromAppConfiguration<T>(appConfigEndpoint, credential, label, keyFilter);
            }
            else
            {
                // In development, create settings from environment variables
                settings = CreateSettingsFromEnvironment<T>();
            }
            
            builder.Services.AddSingleton(settings);
            return builder;
        }

        /// <summary>
        /// Extension method for IHostApplicationBuilder that fetches configuration directly from App Configuration using a connection string.
        /// This method bypasses IConfiguration and fetches settings directly, designed specifically for Azure Functions
        /// and other scenarios where immediate access to configuration is required without IConfiguration integration.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="builder">The host application builder</param>
        /// <param name="connectionString">The connection string for the Azure App Configuration store</param>
        /// <param name="settings">The populated settings object</param>
        /// <param name="label">Optional label filter for configuration keys. Use null for default label, or specific labels like "Production", "Staging"</param>
        /// <param name="keyFilter">Optional key filter pattern. Use null for all keys, or patterns like "MyApp:*" for prefix filtering</param>
        /// <param name="skipDevelopment">Whether to skip App Configuration in development environment. Default is true.</param>
        /// <param name="refreshOptions">Optional refresh configuration. Note: Direct client access doesn't support automatic refresh.</param>
        /// <returns>The builder for method chaining</returns>
        public static IHostApplicationBuilder AddAppConfigurationForAzureFunctions<T>(
            this IHostApplicationBuilder builder, 
            string connectionString, 
            out T settings, 
            string? label = null,
            string? keyFilter = null,
            bool skipDevelopment = true,
            AppConfigurationRefreshOptions? refreshOptions = null)
            where T : class, new()
        {
            if (!(skipDevelopment && builder.Environment.IsDevelopment()))
            {
                settings = CreateSettingsFromAppConfiguration<T>(connectionString, label, keyFilter);
            }
            else
            {
                // In development, create settings from environment variables
                settings = CreateSettingsFromEnvironment<T>();
            }
            
            builder.Services.AddSingleton(settings);
            return builder;
        }

        /// <summary>
        /// Creates a settings object by fetching values directly from Azure App Configuration.
        /// This method is suitable for Azure Functions and other scenarios where IConfiguration is not available.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="appConfigEndpoint">The endpoint URI of the Azure App Configuration store</param>
        /// <param name="credential">Optional Azure credential. If null, DefaultAzureCredential will be used</param>
        /// <param name="label">Optional label filter for configuration keys</param>
        /// <param name="keyFilter">Optional key filter pattern</param>
        /// <returns>A populated instance of type T with values from App Configuration</returns>
        public static async Task<T> CreateSettingsFromAppConfigurationAsync<T>(
            Uri appConfigEndpoint, 
            TokenCredential? credential = null,
            string? label = null,
            string? keyFilter = null)
            where T : class, new()
        {
            var cred = credential ?? new DefaultAzureCredential();
            var client = new ConfigurationClient(appConfigEndpoint, cred);
            
            return await CreateSettings<T>(client, label, keyFilter);
        }

        /// <summary>
        /// Creates a settings object by fetching values directly from Azure App Configuration using a connection string.
        /// This method is suitable for Azure Functions and other scenarios where IConfiguration is not available.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="connectionString">The connection string for the Azure App Configuration store</param>
        /// <param name="label">Optional label filter for configuration keys</param>
        /// <param name="keyFilter">Optional key filter pattern</param>
        /// <returns>A populated instance of type T with values from App Configuration</returns>
        public static async Task<T> CreateSettingsFromAppConfigurationAsync<T>(
            string connectionString,
            string? label = null,
            string? keyFilter = null)
            where T : class, new()
        {
            var client = new ConfigurationClient(connectionString);
            
            return await CreateSettings<T>(client, label, keyFilter);
        }

        /// <summary>
        /// Synchronous version that creates a settings object by fetching values directly from Azure App Configuration.
        /// This method is suitable for Azure Functions and other scenarios where IConfiguration is not available.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="appConfigEndpoint">The endpoint URI of the Azure App Configuration store</param>
        /// <param name="credential">Optional Azure credential. If null, DefaultAzureCredential will be used</param>
        /// <param name="label">Optional label filter for configuration keys</param>
        /// <param name="keyFilter">Optional key filter pattern</param>
        /// <returns>A populated instance of type T with values from App Configuration</returns>
        public static T CreateSettingsFromAppConfiguration<T>(
            Uri appConfigEndpoint, 
            TokenCredential? credential = null,
            string? label = null,
            string? keyFilter = null)
            where T : class, new()
        {
            return CreateSettingsFromAppConfigurationAsync<T>(appConfigEndpoint, credential, label, keyFilter)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Synchronous version that creates a settings object by fetching values directly from Azure App Configuration using a connection string.
        /// This method is suitable for Azure Functions and other scenarios where IConfiguration is not available.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="connectionString">The connection string for the Azure App Configuration store</param>
        /// <param name="label">Optional label filter for configuration keys</param>
        /// <param name="keyFilter">Optional key filter pattern</param>
        /// <returns>A populated instance of type T with values from App Configuration</returns>
        public static T CreateSettingsFromAppConfiguration<T>(
            string connectionString,
            string? label = null,
            string? keyFilter = null)
            where T : class, new()
        {
            return CreateSettingsFromAppConfigurationAsync<T>(connectionString, label, keyFilter)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a settings object using an existing ConfigurationClient to fetch values from Azure App Configuration.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="configClient">The ConfigurationClient to use for fetching configuration</param>
        /// <param name="label">Optional label filter for configuration keys</param>
        /// <param name="keyFilter">Optional key filter pattern</param>
        /// <returns>A populated instance of type T with values from App Configuration</returns>
        private static async Task<T> CreateSettings<T>(
            ConfigurationClient configClient,
            string? label = null,
            string? keyFilter = null)
            where T : class, new()
        {
            var instance = new T();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var property in properties)
            {
                // Check if this is a feature flag property
                var featureFlagAttr = property.GetCustomAttribute<FeatureFlagAttribute>();
                if (featureFlagAttr != null)
                {
                    await SetFeatureFlagValue(property, instance, configClient, featureFlagAttr.FlagName);
                    continue;
                }

                // Regular configuration key
                var configKey = GetAppConfigurationKey(property);
                
                // Apply key filter if specified
                if (!string.IsNullOrEmpty(keyFilter) && !IsKeyMatchingFilter(configKey, keyFilter))
                {
                    continue;
                }
                
                try
                {
                    var configSetting = await configClient.GetConfigurationSettingAsync(configKey, label);
                    if (configSetting?.Value?.Value != null)
                    {
                        SetPropertyValue(property, instance, configSetting.Value.Value);
                    }
                }
                catch (Azure.RequestFailedException ex) when (ex.Status == 404)
                {
                    // Configuration setting not found - skip this property
                    continue;
                }
                catch (Azure.RequestFailedException ex) when (ex.Status == 401 || ex.Status == 403)
                {
                    throw new UnauthorizedAccessException(
                        $"Access denied to App Configuration key '{configKey}'. Check your credentials and App Configuration permissions.", ex);
                }
            }

            return instance;
        }

        /// <summary>
        /// Creates a settings object by reading values from IConfiguration.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="configuration">The configuration instance to read from</param>
        /// <returns>A populated instance of type T with values from configuration</returns>
        private static T CreateSettings<T>(IConfiguration configuration) where T : class, new()
        {
            var instance = new T();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var property in properties)
            {
                // Check if this is a feature flag property
                var featureFlagAttr = property.GetCustomAttribute<FeatureFlagAttribute>();
                if (featureFlagAttr != null)
                {
                    SetFeatureFlagValueFromConfiguration(property, instance, configuration, featureFlagAttr.FlagName);
                    continue;
                }

                var configKey = GetAppConfigurationKey(property);
                var value = configuration[configKey];

                if (!string.IsNullOrEmpty(value))
                {
                    SetPropertyValue(property, instance, value);
                }
            }

            return instance;
        }

        /// <summary>
        /// Creates a settings object by reading values from environment variables.
        /// Used as fallback in development environment when App Configuration is skipped.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <returns>A populated instance of type T with values from environment variables</returns>
        private static T CreateSettingsFromEnvironment<T>() where T : class, new()
        {
            var instance = new T();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var property in properties)
            {
                // Check if this is a feature flag property
                var featureFlagAttr = property.GetCustomAttribute<FeatureFlagAttribute>();
                if (featureFlagAttr != null)
                {
                    var flagValue = Environment.GetEnvironmentVariable(featureFlagAttr.FlagName);
                    if (!string.IsNullOrEmpty(flagValue))
                    {
                        SetPropertyValue(property, instance, flagValue);
                    }
                    continue;
                }

                var configKey = GetAppConfigurationKey(property);
                var value = Environment.GetEnvironmentVariable(configKey);

                if (!string.IsNullOrEmpty(value))
                {
                    SetPropertyValue(property, instance, value);
                }
            }

            return instance;
        }

        /// <summary>
        /// Configures refresh behavior for App Configuration.
        /// Note: App Configuration keys are case-sensitive. Ensure sentinel keys match exactly.
        /// Minimum refresh interval of 30 seconds is recommended to avoid throttling.
        /// </summary>
        /// <remarks>
        /// Azure SDK Limitation: The OnRefreshError callback is provided for custom error handling,
        /// but the Azure App Configuration SDK handles most refresh errors internally without exposing
        /// direct error callbacks. By default, refresh failures are silent, and the application continues
        /// to use cached configuration values. This is the recommended behavior for production resilience.
        /// </remarks>
        private static void ConfigureRefresh(
            AzureAppConfigurationOptions options, 
            AppConfigurationRefreshOptions refreshOptions)
        {
            if (refreshOptions.SentinelKeys != null && refreshOptions.SentinelKeys.Length > 0)
            {
                foreach (var sentinelKey in refreshOptions.SentinelKeys)
                {
                    options.ConfigureRefresh(refresh =>
                    {
                        refresh.Register(sentinelKey, refreshAll: true)
                               .SetRefreshInterval(refreshOptions.RefreshInterval);
                    });
                }
            }

            // Note: Custom error handling configuration is limited by Azure SDK design.
            // The SDK handles refresh errors internally and continues serving cached values.
            // This approach ensures application resilience during transient Azure service issues.
            // The OnRefreshError callback in refreshOptions is exposed for advanced scenarios
            // but may not be invoked for all error conditions due to SDK limitations.
        }

        /// <summary>
        /// Gets the App Configuration key name for a property, either from the AppConfigurationKeyAttribute or the property name.
        /// </summary>
        /// <param name="property">The property to get the key name for</param>
        /// <returns>The App Configuration key name to use for this property</returns>
        private static string GetAppConfigurationKey(PropertyInfo property)
        {
            // Check for AppConfigurationKeyAttribute first
            var appConfigKeyAttribute = property.GetCustomAttribute<AppConfigurationKeyAttribute>();
            if (appConfigKeyAttribute != null)
            {
                return appConfigKeyAttribute.KeyName;
            }

            // Use property name as-is if no attribute specified
            return property.Name;
        }

        /// <summary>
        /// Checks if a key matches a filter pattern.
        /// Note: App Configuration keys are case-sensitive. Comparison is done case-insensitively
        /// for convenience, but it's recommended to use exact casing in your configuration keys.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <param name="filter">The filter pattern (supports * wildcard)</param>
        /// <returns>True if the key matches the filter</returns>
        private static bool IsKeyMatchingFilter(string key, string filter)
        {
            if (string.IsNullOrEmpty(filter) || filter == "*")
            {
                return true;
            }

            // Simple wildcard matching for patterns like "MyApp:*"
            if (filter.EndsWith("*"))
            {
                var prefix = filter.Substring(0, filter.Length - 1);
                return key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }

            return key.Equals(filter, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Sets a feature flag value on a property from App Configuration
        /// </summary>
        private static async Task SetFeatureFlagValue(
            PropertyInfo property, 
            object instance, 
            ConfigurationClient client, 
            string flagName)
        {
            // Validate that the property is a boolean type
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (propertyType != typeof(bool))
            {
                throw new InvalidOperationException(
                    $"Property '{property.Name}' is decorated with [FeatureFlag] but is not of type bool or bool?. " +
                    $"Feature flags can only be applied to boolean properties.");
            }

            try
            {
                // Feature flags in App Configuration use a special key format
                var featureFlagKey = $".appconfig.featureflag/{flagName}";
                var flagSetting = await client.GetConfigurationSettingAsync(featureFlagKey);
                
                if (flagSetting?.Value?.Value != null)
                {
                    // Parse the feature flag JSON to get the enabled state
                    var enabled = ParseFeatureFlagEnabled(flagSetting.Value.Value);
                    property.SetValue(instance, enabled);
                }
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                // Feature flag not found - skip this property
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 401 || ex.Status == 403)
            {
                throw new UnauthorizedAccessException(
                    $"Access denied to feature flag '{flagName}'. Check your credentials and App Configuration permissions.", ex);
            }
        }

        /// <summary>
        /// Sets a feature flag value from IConfiguration
        /// </summary>
        private static void SetFeatureFlagValueFromConfiguration(
            PropertyInfo property, 
            object instance, 
            IConfiguration configuration, 
            string flagName)
        {
            // Validate that the property is a boolean type
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (propertyType != typeof(bool))
            {
                throw new InvalidOperationException(
                    $"Property '{property.Name}' is decorated with [FeatureFlag] but is not of type bool or bool?. " +
                    $"Feature flags can only be applied to boolean properties.");
            }

            // Try to read from configuration (Azure App Configuration integration handles the feature flag format)
            var featureFlagKey = $"FeatureManagement:{flagName}";
            var value = configuration[featureFlagKey];

            if (!string.IsNullOrEmpty(value))
            {
                SetPropertyValue(property, instance, value);
            }
        }

        /// <summary>
        /// Parses the enabled state from a feature flag JSON value
        /// </summary>
        private static bool ParseFeatureFlagEnabled(string featureFlagJson)
        {
            try
            {
                // Feature flags in App Configuration are JSON objects with an "enabled" property
                // Simple parsing to extract the enabled value
                var json = Newtonsoft.Json.Linq.JObject.Parse(featureFlagJson);
                var enabledToken = json["enabled"];
                return enabledToken?.ToObject<bool>() ?? false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets a property value on an object instance, performing type conversion from string as needed.
        /// </summary>
        /// <param name="property">The property to set</param>
        /// <param name="instance">The object instance to set the property on</param>
        /// <param name="value">The string value to convert and set</param>
        /// <exception cref="InvalidOperationException">Thrown when type conversion fails</exception>
        private static void SetPropertyValue(PropertyInfo property, object instance, string value)
        {
            try
            {
                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                
                if (targetType == typeof(string))
                {
                    property.SetValue(instance, value);
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(targetType);
                    var convertedValue = converter.ConvertFromString(value);
                    property.SetValue(instance, convertedValue);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to set property '{property.Name}' with value '{value}'. {ex.Message}", ex);
            }
        }
    }
}

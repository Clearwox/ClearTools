using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TokenCredential = Azure.Core.TokenCredential;

namespace ClearTools.Extensions
{
    /// <summary>
    /// Attribute to specify the Key Vault key name for a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyVaultKeyAttribute : Attribute
    {
        public string KeyName { get; }
        
        public KeyVaultKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }

    public static class KeyVaultExtensions
    {
        /// <summary>
        /// Extension method for IHostApplicationBuilder that configures Azure Key Vault as a configuration provider.
        /// This method adds Key Vault secrets to IConfiguration and creates a settings object from the configuration.
        /// Designed for ASP.NET Web Applications where IConfiguration integration is preferred.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="builder">The host application builder</param>
        /// <param name="keyVaultUri">The URI of the Azure Key Vault</param>
        /// <param name="settings">The populated settings object</param>
        /// <param name="skipDevelopment">Whether to skip Key Vault in development environment. Default is true.</param>
        /// <param name="credential">Optional Azure credential. If null, DefaultAzureCredential will be used</param>
        /// <returns>The builder for method chaining</returns>
        public static IHostApplicationBuilder AddKeyVaultForWebApplication<T>(
            this IHostApplicationBuilder builder, string keyVaultUri, out T settings, bool skipDevelopment = true, TokenCredential? credential = null)
            where T : class, new()
        {
            if (!(skipDevelopment && builder.Environment.IsDevelopment()))
            {
                var cred = credential ?? new DefaultAzureCredential();
                builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), cred);
            }

            settings = CreateSettings<T>(builder.Configuration);
            builder.Services.AddSingleton(settings);

            return builder;
        }

        /// <summary>
        /// Extension method for IHostApplicationBuilder that fetches secrets directly from Key Vault using SecretClient.
        /// This method bypasses IConfiguration and fetches secrets directly, designed specifically for Azure Functions
        /// and other scenarios where immediate access to secrets is required without IConfiguration integration.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="builder">The host application builder</param>
        /// <param name="keyVaultUri">The URI of the Azure Key Vault</param>
        /// <param name="settings">The populated settings object</param>
        /// <param name="skipDevelopment">Whether to skip Key Vault in development environment. Default is true.</param>
        /// <param name="credential">Optional Azure credential. If null, DefaultAzureCredential will be used</param>
        /// <returns>The builder for method chaining</returns>
        public static IHostApplicationBuilder AddKeyVaultForAzureFunctions<T>(
            this IHostApplicationBuilder builder, string keyVaultUri, out T settings, bool skipDevelopment = true, TokenCredential? credential = null)
            where T : class, new()
        {
            if (!(skipDevelopment && builder.Environment.IsDevelopment()))
            {
                settings = CreateSettingsFromKeyVault<T>(keyVaultUri, credential);
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
        /// Creates a settings object by fetching values directly from Azure Key Vault.
        /// This method is suitable for Azure Functions and other scenarios where IConfiguration is not available.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="keyVaultUri">The URI of the Azure Key Vault</param>
        /// <param name="credential">Optional Azure credential. If null, DefaultAzureCredential will be used</param>
        /// <returns>A populated instance of type T with values from Key Vault</returns>
        public static async Task<T> CreateSettingsFromKeyVaultAsync<T>(string keyVaultUri, TokenCredential? credential = null)
            where T : class, new()
        {
            var cred = credential ?? new DefaultAzureCredential();
            var client = new SecretClient(new Uri(keyVaultUri), cred);
            
            return await CreateSettings<T>(client);
        }

        /// <summary>
        /// Synchronous version that creates a settings object by fetching values directly from Azure Key Vault.
        /// This method is suitable for Azure Functions and other scenarios where IConfiguration is not available.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="keyVaultUri">The URI of the Azure Key Vault</param>
        /// <param name="credential">Optional Azure credential. If null, DefaultAzureCredential will be used</param>
        /// <returns>A populated instance of type T with values from Key Vault</returns>
        public static T CreateSettingsFromKeyVault<T>(string keyVaultUri, TokenCredential? credential = null)
            where T : class, new()
        {
            return CreateSettingsFromKeyVaultAsync<T>(keyVaultUri, credential).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a settings object using an existing SecretClient to fetch values from Azure Key Vault.
        /// </summary>
        /// <typeparam name="T">The type of settings object to create</typeparam>
        /// <param name="secretClient">The SecretClient to use for fetching secrets</param>
        /// <returns>A populated instance of type T with values from Key Vault</returns>
        private static async Task<T> CreateSettings<T>(SecretClient secretClient)
            where T : class, new()
        {
            var instance = new T();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var property in properties)
            {
                var keyVaultKey = GetKeyVaultKey(property);
                
                try
                {
                    var secret = await secretClient.GetSecretAsync(keyVaultKey);
                    if (secret?.Value?.Value != null)
                    {
                        SetPropertyValue(property, instance, secret.Value.Value);
                    }
                }
                catch (Azure.RequestFailedException ex) when (ex.Status == 404)
                {
                    // Secret not found - skip this property
                    continue;
                }
                catch (Azure.RequestFailedException ex) when (ex.Status == 401 || ex.Status == 403)
                {
                    throw new UnauthorizedAccessException(
                        $"Access denied to Key Vault secret '{keyVaultKey}'. Check your credentials and Key Vault permissions.", ex);
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
                var keyVaultKey = GetKeyVaultKey(property);
                var value = configuration[keyVaultKey];

                if (!string.IsNullOrEmpty(value))
                {
                    SetPropertyValue(property, instance, value);
                }
            }

            return instance;
        }

        /// <summary>
        /// Creates a settings object by reading values from environment variables.
        /// Used as fallback in development environment when Key Vault is skipped.
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
                var keyVaultKey = GetKeyVaultKey(property);
                var value = Environment.GetEnvironmentVariable(keyVaultKey);

                if (!string.IsNullOrEmpty(value))
                {
                    SetPropertyValue(property, instance, value);
                }
            }

            return instance;
        }

        /// <summary>
        /// Gets the Key Vault key name for a property, either from the KeyVaultKeyAttribute or the property name.
        /// </summary>
        /// <param name="property">The property to get the key name for</param>
        /// <returns>The Key Vault key name to use for this property</returns>
        private static string GetKeyVaultKey(PropertyInfo property)
        {
            // Check for KeyVaultKeyAttribute first
            var keyVaultKeyAttribute = property.GetCustomAttribute<KeyVaultKeyAttribute>();
            if (keyVaultKeyAttribute != null)
            {
                return keyVaultKeyAttribute.KeyName;
            }

            // Use property name as-is if no attribute specified
            return property.Name;
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

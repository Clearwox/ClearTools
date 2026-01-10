using System;
using ClearTools.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClearTools.Extensions
{
    /// <summary>
    /// Provides extension methods for registering strongly-typed connection strings with dependency injection.
    /// </summary>
    public static class ConnectionStringExtensions
    {
        /// <summary>
        /// Registers a strongly-typed connection string configuration as a singleton service.
        /// Automatically detects whether the type uses a parameterized constructor or parameterless constructor pattern.
        /// </summary>
        /// <typeparam name="T">The connection string type that inherits from <see cref="ConnectionStringBase"/>.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="connectionString">The raw connection string value to parse.</param>
        /// <param name="options">Optional parsing options for delimiter, escaping, and case sensitivity.</param>
        /// <returns>The service collection for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="connectionString"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when parsing fails or required properties are missing.</exception>
        /// <example>
        /// <code>
        /// // Works with constructor-based types
        /// services.AddConnectionString&lt;SqlServerConnectionString&gt;(
        ///     "Server=localhost;Database=MyDb;User Id=sa;Password=mypass");
        /// 
        /// // Also works with parameterless constructor types
        /// services.AddConnectionString&lt;CustomConnectionString&gt;(
        ///     "ApiUrl=https://api.example.com;ApiKey=secret");
        /// 
        /// // Later, inject it into a service:
        /// public class MyService
        /// {
        ///     private readonly SqlServerConnectionString _connectionString;
        ///     
        ///     public MyService(SqlServerConnectionString connectionString)
        ///     {
        ///         _connectionString = connectionString;
        ///     }
        /// }
        /// </code>
        /// </example>
        public static IServiceCollection AddConnectionString<T>(
            this IServiceCollection services,
            string connectionString,
            ConnectionStringParsingOptions? options = null)
            where T : ConnectionStringBase
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            T instance;
            try
            {
                // Try constructor with both parameters
                instance = (T)Activator.CreateInstance(typeof(T), connectionString, options)!;
            }
            catch (MissingMethodException)
            {
                try
                {
                    // Try constructor with single parameter
                    instance = (T)Activator.CreateInstance(typeof(T), connectionString)!;
                }
                catch (MissingMethodException)
                {
                    // Fallback to parameterless constructor + Initialize
                    instance = (T)Activator.CreateInstance(typeof(T))!;
                    instance.Initialize(connectionString, options);
                }
            }
            
            services.AddSingleton(instance);

            return services;
        }

        /// <summary>
        /// Registers a strongly-typed connection string configuration as a singleton service,
        /// retrieving the raw connection string value from <see cref="IConfiguration"/>.
        /// </summary>
        /// <typeparam name="T">The connection string type that inherits from <see cref="ConnectionStringBase"/>.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration instance to retrieve the connection string from.</param>
        /// <param name="configurationKey">The configuration key path (e.g., "ConnectionStrings:MyDatabase").</param>
        /// <param name="options">Optional parsing options for delimiter, escaping, and case sensitivity.</param>
        /// <returns>The service collection for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/>, <paramref name="configuration"/>, or <paramref name="configurationKey"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the configuration key is not found, or when parsing fails or required properties are missing.</exception>
        /// <example>
        /// <code>
        /// // In appsettings.json:
        /// // {
        /// //   "ConnectionStrings": {
        /// //     "MyDatabase": "Server=localhost;Database=MyDb;User Id=sa;Password=mypass"
        /// //   }
        /// // }
        /// 
        /// services.AddConnectionString&lt;SqlServerConnectionString&gt;(
        ///     configuration,
        ///     "ConnectionStrings:MyDatabase");
        /// </code>
        /// </example>
        public static IServiceCollection AddConnectionString<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationKey,
            ConnectionStringParsingOptions? options = null)
            where T : ConnectionStringBase
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (configurationKey == null)
                throw new ArgumentNullException(nameof(configurationKey));

            var connectionString = configuration[configurationKey];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Configuration key '{configurationKey}' was not found or is empty.");
            }

            return AddConnectionString<T>(services, connectionString, options);
        }
    }
}

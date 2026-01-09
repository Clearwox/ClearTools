namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed Azure App Configuration connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new AppConfigurationConnectionString("Endpoint=https://myappconfig.azconfig.io;Id=abc123;Secret=mysecret");
    /// Console.WriteLine(connStr.Endpoint); // "https://myappconfig.azconfig.io"
    /// </code>
    /// </example>
    public class AppConfigurationConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the App Configuration endpoint URL.
        /// </summary>
        [ConnectionStringKey("Endpoint")]
        [Required]
        public string? Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the credential ID.
        /// </summary>
        [ConnectionStringKey("Id")]
        [Required]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the credential secret.
        /// </summary>
        [ConnectionStringKey("Secret")]
        [Required]
        public string? Secret { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigurationConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public AppConfigurationConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

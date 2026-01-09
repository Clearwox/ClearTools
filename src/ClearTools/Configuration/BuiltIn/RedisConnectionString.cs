namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed Redis connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new RedisConnectionString("Host=localhost;Port=6379;Password=mypass;Ssl=true;Database=0");
    /// Console.WriteLine(connStr.Host); // "localhost"
    /// Console.WriteLine(connStr.Port); // 6379
    /// </code>
    /// </example>
    public class RedisConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the Redis server host or IP address.
        /// </summary>
        [ConnectionStringKey("Host")]
        [Required]
        public string? Host { get; set; }

        /// <summary>
        /// Gets or sets the Redis server port number.
        /// </summary>
        [ConnectionStringKey("Port")]
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        [ConnectionStringKey("Password")]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets whether to use SSL/TLS.
        /// </summary>
        [ConnectionStringKey("Ssl")]
        public bool? Ssl { get; set; }

        /// <summary>
        /// Gets or sets the database number to use.
        /// </summary>
        [ConnectionStringKey("Database")]
        public int? Database { get; set; }

        /// <summary>
        /// Gets or sets the connection timeout in milliseconds.
        /// </summary>
        [ConnectionStringKey("ConnectTimeout")]
        public int? ConnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets the sync timeout in milliseconds.
        /// </summary>
        [ConnectionStringKey("SyncTimeout")]
        public int? SyncTimeout { get; set; }

        /// <summary>
        /// Gets or sets whether to abort on connection failures.
        /// </summary>
        [ConnectionStringKey("AbortOnConnectFail")]
        public bool? AbortOnConnectFail { get; set; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        [ConnectionStringKey("ClientName")]
        public string? ClientName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public RedisConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

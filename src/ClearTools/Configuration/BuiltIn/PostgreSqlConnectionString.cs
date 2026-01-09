namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed PostgreSQL connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new PostgreSqlConnectionString("Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=mypass");
    /// Console.WriteLine(connStr.Host); // "localhost"
    /// Console.WriteLine(connStr.Database); // "mydb"
    /// </code>
    /// </example>
    public class PostgreSqlConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the PostgreSQL server host or IP address.
        /// </summary>
        [ConnectionStringKey("Host")]
        [Required]
        public string? Host { get; set; }

        /// <summary>
        /// Gets or sets the PostgreSQL server port number.
        /// </summary>
        [ConnectionStringKey("Port")]
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [ConnectionStringKey("Database")]
        [Required]
        public string? Database { get; set; }

        /// <summary>
        /// Gets or sets the username for authentication.
        /// </summary>
        [ConnectionStringKey("Username")]
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        [ConnectionStringKey("Password")]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the connection timeout in seconds.
        /// </summary>
        [ConnectionStringKey("Timeout")]
        public int? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the command timeout in seconds.
        /// </summary>
        [ConnectionStringKey("CommandTimeout")]
        public int? CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets the SSL mode (Disable, Allow, Prefer, Require, VerifyCA, VerifyFull).
        /// </summary>
        [ConnectionStringKey("SslMode")]
        public string? SslMode { get; set; }

        /// <summary>
        /// Gets or sets whether to use pooling.
        /// </summary>
        [ConnectionStringKey("Pooling")]
        public bool? Pooling { get; set; }

        /// <summary>
        /// Gets or sets the minimum pool size.
        /// </summary>
        [ConnectionStringKey("MinPoolSize")]
        public int? MinPoolSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum pool size.
        /// </summary>
        [ConnectionStringKey("MaxPoolSize")]
        public int? MaxPoolSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public PostgreSqlConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

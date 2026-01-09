namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed SQL Server connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new SqlServerConnectionString("Server=localhost;Database=MyDb;User Id=sa;Password=mypass;Encrypt=true");
    /// Console.WriteLine(connStr.Server); // "localhost"
    /// Console.WriteLine(connStr.Database); // "MyDb"
    /// </code>
    /// </example>
    public class SqlServerConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the SQL Server instance name or IP address.
        /// </summary>
        [ConnectionStringKey("Server")]
        [Required]
        public string? Server { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [ConnectionStringKey("Database")]
        [Required]
        public string? Database { get; set; }

        /// <summary>
        /// Gets or sets the user ID for SQL Server authentication.
        /// </summary>
        [ConnectionStringKey("User Id")]
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the password for SQL Server authentication.
        /// </summary>
        [ConnectionStringKey("Password")]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets whether to use integrated security (Windows authentication).
        /// </summary>
        [ConnectionStringKey("Integrated Security")]
        public bool? IntegratedSecurity { get; set; }

        /// <summary>
        /// Gets or sets whether to encrypt the connection.
        /// </summary>
        [ConnectionStringKey("Encrypt")]
        public bool? Encrypt { get; set; }

        /// <summary>
        /// Gets or sets whether to trust the server certificate.
        /// </summary>
        [ConnectionStringKey("TrustServerCertificate")]
        public bool? TrustServerCertificate { get; set; }

        /// <summary>
        /// Gets or sets the connection timeout in seconds.
        /// </summary>
        [ConnectionStringKey("Connection Timeout")]
        public int? ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        [ConnectionStringKey("Application Name")]
        public string? ApplicationName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public SqlServerConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

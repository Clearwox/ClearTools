namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed MongoDB connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new MongoDbConnectionString("mongodb://username:password@localhost:27017/mydatabase?authSource=admin");
    /// // Or using key-value format:
    /// var connStr2 = new MongoDbConnectionString("Host=localhost;Port=27017;Database=mydatabase;Username=user;Password=pass");
    /// </code>
    /// </example>
    public class MongoDbConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the MongoDB host or server address.
        /// </summary>
        [ConnectionStringKey("Host")]
        [Required]
        public string? Host { get; set; }

        /// <summary>
        /// Gets or sets the MongoDB port number.
        /// </summary>
        [ConnectionStringKey("Port")]
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [ConnectionStringKey("Database")]
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
        /// Gets or sets the authentication database.
        /// </summary>
        [ConnectionStringKey("AuthSource")]
        public string? AuthSource { get; set; }

        /// <summary>
        /// Gets or sets the replica set name.
        /// </summary>
        [ConnectionStringKey("ReplicaSet")]
        public string? ReplicaSet { get; set; }

        /// <summary>
        /// Gets or sets whether to use TLS/SSL.
        /// </summary>
        [ConnectionStringKey("Tls")]
        public bool? Tls { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public MongoDbConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

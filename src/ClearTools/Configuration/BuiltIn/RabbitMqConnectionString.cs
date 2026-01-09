namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed RabbitMQ connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new RabbitMqConnectionString("Host=localhost;Port=5672;VirtualHost=/;Username=guest;Password=guest");
    /// Console.WriteLine(connStr.Host); // "localhost"
    /// Console.WriteLine(connStr.VirtualHost); // "/"
    /// </code>
    /// </example>
    public class RabbitMqConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the RabbitMQ server host or IP address.
        /// </summary>
        [ConnectionStringKey("Host")]
        [Required]
        public string? Host { get; set; }

        /// <summary>
        /// Gets or sets the RabbitMQ server port number.
        /// </summary>
        [ConnectionStringKey("Port")]
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the virtual host.
        /// </summary>
        [ConnectionStringKey("VirtualHost")]
        public string? VirtualHost { get; set; }

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
        [ConnectionStringKey("RequestedConnectionTimeout")]
        public int? RequestedConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the heartbeat interval in seconds.
        /// </summary>
        [ConnectionStringKey("RequestedHeartbeat")]
        public int? RequestedHeartbeat { get; set; }

        /// <summary>
        /// Gets or sets whether to use SSL/TLS.
        /// </summary>
        [ConnectionStringKey("Ssl")]
        public bool? Ssl { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public RabbitMqConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

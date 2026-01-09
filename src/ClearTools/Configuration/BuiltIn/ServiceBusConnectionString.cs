namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed Azure Service Bus connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new ServiceBusConnectionString("Endpoint=sb://myservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mykey123");
    /// Console.WriteLine(connStr.Endpoint); // "sb://myservicebus.servicebus.windows.net/"
    /// </code>
    /// </example>
    public class ServiceBusConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the Service Bus endpoint URL.
        /// </summary>
        [ConnectionStringKey("Endpoint")]
        [Required]
        public string? Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the shared access key name.
        /// </summary>
        [ConnectionStringKey("SharedAccessKeyName")]
        public string? SharedAccessKeyName { get; set; }

        /// <summary>
        /// Gets or sets the shared access key.
        /// </summary>
        [ConnectionStringKey("SharedAccessKey")]
        public string? SharedAccessKey { get; set; }

        /// <summary>
        /// Gets or sets the shared access signature.
        /// </summary>
        [ConnectionStringKey("SharedAccessSignature")]
        public string? SharedAccessSignature { get; set; }

        /// <summary>
        /// Gets or sets the entity path (queue or topic name).
        /// </summary>
        [ConnectionStringKey("EntityPath")]
        public string? EntityPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public ServiceBusConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

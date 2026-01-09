namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed Azure Blob Storage connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new BlobStorageConnectionString("DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=mykey123;EndpointSuffix=core.windows.net");
    /// Console.WriteLine(connStr.AccountName); // "myaccount"
    /// </code>
    /// </example>
    public class BlobStorageConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the default endpoints protocol (http or https).
        /// </summary>
        [ConnectionStringKey("DefaultEndpointsProtocol")]
        public string? DefaultEndpointsProtocol { get; set; }

        /// <summary>
        /// Gets or sets the storage account name.
        /// </summary>
        [ConnectionStringKey("AccountName")]
        [Required]
        public string? AccountName { get; set; }

        /// <summary>
        /// Gets or sets the storage account key.
        /// </summary>
        [ConnectionStringKey("AccountKey")]
        public string? AccountKey { get; set; }

        /// <summary>
        /// Gets or sets the shared access signature (SAS) token.
        /// </summary>
        [ConnectionStringKey("SharedAccessSignature")]
        public string? SharedAccessSignature { get; set; }

        /// <summary>
        /// Gets or sets the endpoint suffix (e.g., core.windows.net).
        /// </summary>
        [ConnectionStringKey("EndpointSuffix")]
        public string? EndpointSuffix { get; set; }

        /// <summary>
        /// Gets or sets the blob endpoint.
        /// </summary>
        [ConnectionStringKey("BlobEndpoint")]
        public string? BlobEndpoint { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public BlobStorageConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

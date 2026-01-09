namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed Azure Cosmos DB connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new CosmosDbConnectionString("AccountEndpoint=https://myaccount.documents.azure.com:443/;AccountKey=mykey123");
    /// Console.WriteLine(connStr.AccountEndpoint); // "https://myaccount.documents.azure.com:443/"
    /// </code>
    /// </example>
    public class CosmosDbConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the Cosmos DB account endpoint URL.
        /// </summary>
        [ConnectionStringKey("AccountEndpoint")]
        [Required]
        public string? AccountEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the Cosmos DB account key.
        /// </summary>
        [ConnectionStringKey("AccountKey")]
        [Required]
        public string? AccountKey { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [ConnectionStringKey("Database")]
        public string? Database { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public CosmosDbConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

namespace ClearTools.Configuration.BuiltIn
{
    /// <summary>
    /// Represents a strongly-typed Azure Key Vault connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// var connStr = new KeyVaultConnectionString("VaultUri=https://myvault.vault.azure.net/;TenantId=mytenant;ClientId=myclient;ClientSecret=mysecret");
    /// Console.WriteLine(connStr.VaultUri); // "https://myvault.vault.azure.net/"
    /// </code>
    /// </example>
    public class KeyVaultConnectionString : ConnectionStringBase
    {
        /// <summary>
        /// Gets or sets the Key Vault URI.
        /// </summary>
        [ConnectionStringKey("VaultUri")]
        [Required]
        public string? VaultUri { get; set; }

        /// <summary>
        /// Gets or sets the Azure AD tenant ID.
        /// </summary>
        [ConnectionStringKey("TenantId")]
        public string? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the Azure AD client ID (application ID).
        /// </summary>
        [ConnectionStringKey("ClientId")]
        public string? ClientId { get; set; }

        /// <summary>
        /// Gets or sets the Azure AD client secret.
        /// </summary>
        [ConnectionStringKey("ClientSecret")]
        public string? ClientSecret { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options.</param>
        public KeyVaultConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
            : base(connectionString, options)
        {
        }
    }
}

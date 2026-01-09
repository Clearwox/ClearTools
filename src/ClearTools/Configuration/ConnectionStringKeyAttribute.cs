using System;

namespace ClearTools.Configuration
{
    /// <summary>
    /// Specifies the key name that a property maps to in a connection string.
    /// When parsing a connection string, properties decorated with this attribute will be populated
    /// from the corresponding key-value pair in the connection string.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MyConnectionString : ConnectionStringBase
    /// {
    ///     [ConnectionStringKey("Server")]
    ///     public string? ServerName { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConnectionStringKeyAttribute : Attribute
    {
        /// <summary>
        /// Gets the key name in the connection string that this property maps to.
        /// </summary>
        public string KeyName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringKeyAttribute"/> class.
        /// </summary>
        /// <param name="keyName">The key name in the connection string.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="keyName"/> is null or whitespace.</exception>
        public ConnectionStringKeyAttribute(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentNullException(nameof(keyName), "Key name cannot be null or whitespace.");

            KeyName = keyName;
        }
    }
}

using System;

namespace ClearTools.Configuration
{
    /// <summary>
    /// Indicates that a property is required in the connection string.
    /// If the corresponding key is missing during parsing, an <see cref="InvalidOperationException"/> will be thrown.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MyConnectionString : ConnectionStringBase
    /// {
    ///     [ConnectionStringKey("Server")]
    ///     [Required]
    ///     public string? ServerName { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredAttribute : Attribute
    {
    }
}

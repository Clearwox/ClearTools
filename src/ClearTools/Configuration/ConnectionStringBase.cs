using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ClearTools.Configuration
{
    /// <summary>
    /// Abstract base class for strongly-typed connection string configurations.
    /// Parses key-value pair connection strings into strongly-typed properties and supports serialization back to string format.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class uses reflection to map properties decorated with <see cref="ConnectionStringKeyAttribute"/> to their
    /// corresponding key-value pairs in a connection string. Properties marked with <see cref="RequiredAttribute"/>
    /// will cause an exception if the key is missing from the connection string.
    /// </para>
    /// <para>
    /// The default delimiter is semicolon (;), but this can be customized via <see cref="ConnectionStringParsingOptions"/>.
    /// Escaping is supported when enabled, allowing literal delimiter characters in values (e.g., \;).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyConnectionString : ConnectionStringBase
    /// {
    ///     [ConnectionStringKey("Server")]
    ///     [Required]
    ///     public string? Server { get; set; }
    ///     
    ///     [ConnectionStringKey("Port")]
    ///     public int Port { get; set; } = 1433;
    /// }
    /// 
    /// var connStr = new MyConnectionString("Server=localhost;Port=5432");
    /// Console.WriteLine(connStr.Server); // "localhost"
    /// Console.WriteLine(connStr.ToString()); // "Server=localhost;Port=5432"
    /// </code>
    /// </example>
    public abstract class ConnectionStringBase
    {
        /// <summary>
        /// Gets the parsing options used for this connection string instance.
        /// </summary>
        protected ConnectionStringParsingOptions Options { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringBase"/> class with default options.
        /// Use <see cref="Initialize"/> to populate properties from a connection string.
        /// </summary>
        protected ConnectionStringBase()
        {
            Options = new ConnectionStringParsingOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringBase"/> class.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options. If null, default options will be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionString"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a required property is missing from the connection string.</exception>
        public ConnectionStringBase(string connectionString, ConnectionStringParsingOptions? options = null)
        {
            Options = new ConnectionStringParsingOptions();
            Initialize(connectionString, options);
        }

        /// <summary>
        /// Initializes the connection string by parsing a raw connection string value.
        /// Can be called on an instance created with the parameterless constructor, or to re-initialize an existing instance.
        /// </summary>
        /// <param name="connectionString">The raw connection string to parse.</param>
        /// <param name="options">Optional parsing options. If null, current options will be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionString"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a required property is missing from the connection string.</exception>
        /// <example>
        /// <code>
        /// // Using parameterless constructor + Initialize
        /// var connStr = new SqlServerConnectionString();
        /// connStr.Initialize("Server=localhost;Database=MyDb;User Id=sa;Password=pass");
        /// 
        /// // Or using constructor (internally calls Initialize)
        /// var connStr2 = new SqlServerConnectionString("Server=localhost;Database=MyDb;User Id=sa;Password=pass");
        /// </code>
        /// </example>
        public void Initialize(string connectionString, ConnectionStringParsingOptions? options = null)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            if (options != null)
                Options = options;
            
            var keyValuePairs = ParseConnectionString(connectionString);
            PopulateProperties(keyValuePairs);
            ValidateRequiredProperties(keyValuePairs);
        }

        /// <summary>
        /// Parses the connection string into a dictionary of key-value pairs.
        /// </summary>
        private Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            var comparer = Options.KeyComparison switch
            {
                StringComparison.Ordinal => StringComparer.Ordinal,
                StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
                StringComparison.CurrentCulture => StringComparer.CurrentCulture,
                StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
                StringComparison.InvariantCulture => StringComparer.InvariantCulture,
                StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase,
                _ => StringComparer.OrdinalIgnoreCase
            };

            var result = new Dictionary<string, string>(comparer);

            if (string.IsNullOrWhiteSpace(connectionString))
                return result;

            var pairs = SplitByDelimiter(connectionString, Options.Delimiter, Options.EnableEscaping);

            foreach (var pair in pairs)
            {
                if (string.IsNullOrWhiteSpace(pair))
                    continue;

                var equalsIndex = pair.IndexOf('=');
                if (equalsIndex <= 0)
                    continue;

                var key = pair.Substring(0, equalsIndex).Trim();
                var value = pair.Substring(equalsIndex + 1).Trim();

                if (Options.EnableEscaping)
                    value = UnescapeValue(value, Options.Delimiter);

                if (!string.IsNullOrEmpty(key))
                    result[key] = value;
            }

            return result;
        }

        /// <summary>
        /// Splits a string by delimiter, respecting escape sequences when enabled.
        /// </summary>
        private List<string> SplitByDelimiter(string input, string delimiter, bool enableEscaping)
        {
            var result = new List<string>();

            if (!enableEscaping)
            {
                result.AddRange(input.Split(new[] { delimiter }, StringSplitOptions.None));
                return result;
            }

            var currentPart = new StringBuilder();
            var i = 0;

            while (i < input.Length)
            {
                // Check for escape sequence
                if (i < input.Length - 1 && input[i] == '\\' && input.Substring(i + 1, Math.Min(delimiter.Length, input.Length - i - 1)) == delimiter)
                {
                    // Escaped delimiter - add the delimiter to current part
                    currentPart.Append(delimiter);
                    i += 1 + delimiter.Length;
                }
                else if (input.Substring(i, Math.Min(delimiter.Length, input.Length - i)) == delimiter)
                {
                    // Unescaped delimiter - end current part
                    result.Add(currentPart.ToString());
                    currentPart.Clear();
                    i += delimiter.Length;
                }
                else
                {
                    currentPart.Append(input[i]);
                    i++;
                }
            }

            // Add the last part
            result.Add(currentPart.ToString());

            return result;
        }

        /// <summary>
        /// Unescapes a value by removing escape sequences.
        /// </summary>
        private string UnescapeValue(string value, string delimiter)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Replace($"\\{delimiter}", delimiter);
        }

        /// <summary>
        /// Escapes a value by adding escape sequences for delimiters.
        /// </summary>
        private string EscapeValue(string value, string delimiter, bool enableEscaping)
        {
            if (!enableEscaping || string.IsNullOrEmpty(value))
                return value;

            return value.Replace(delimiter, $"\\{delimiter}");
        }

        /// <summary>
        /// Populates properties using reflection based on the parsed key-value pairs.
        /// </summary>
        private void PopulateProperties(Dictionary<string, string> keyValuePairs)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var property in properties)
            {
                var keyAttribute = property.GetCustomAttribute<ConnectionStringKeyAttribute>();
                if (keyAttribute == null)
                    continue;

                var keyName = keyAttribute.KeyName;

                if (keyValuePairs.TryGetValue(keyName, out var value))
                {
                    try
                    {
                        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        var convertedValue = Convert.ChangeType(value, propertyType);
                        property.SetValue(this, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"Failed to convert value '{value}' for property '{property.Name}' (key: '{keyName}') to type {property.PropertyType.Name}.",
                            ex);
                    }
                }
            }
        }

        /// <summary>
        /// Validates that all required properties have been populated.
        /// </summary>
        private void ValidateRequiredProperties(Dictionary<string, string> keyValuePairs)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute == null)
                    continue;

                var keyAttribute = property.GetCustomAttribute<ConnectionStringKeyAttribute>();
                if (keyAttribute == null)
                {
                    throw new InvalidOperationException(
                        $"Property '{property.Name}' is marked as [Required] but does not have a [ConnectionStringKey] attribute.");
                }

                var keyName = keyAttribute.KeyName;

                if (!keyValuePairs.ContainsKey(keyName))
                {
                    throw new InvalidOperationException(
                        $"Required key '{keyName}' for property '{property.Name}' is missing from the connection string.");
                }
            }
        }

        /// <summary>
        /// Serializes the connection string object back to its string representation.
        /// Only includes properties with non-null values.
        /// Validates that all required properties have values before serialization.
        /// </summary>
        /// <returns>The serialized connection string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a required property is missing or has a null value.</exception>
        public override string ToString()
        {
            // Validate required properties before serialization
            ValidateRequiredPropertiesForToString();

            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead);

            var parts = new List<string>();

            foreach (var property in properties)
            {
                var keyAttribute = property.GetCustomAttribute<ConnectionStringKeyAttribute>();
                if (keyAttribute == null)
                    continue;

                var value = property.GetValue(this);
                if (value == null)
                    continue;

                var valueString = value.ToString();
                if (string.IsNullOrEmpty(valueString))
                    continue;

                var escapedValue = EscapeValue(valueString, Options.Delimiter, Options.EnableEscaping);
                parts.Add($"{keyAttribute.KeyName}={escapedValue}");
            }

            return string.Join(Options.Delimiter, parts);
        }

        /// <summary>
        /// Validates that all required properties have non-null values for ToString serialization.
        /// </summary>
        private void ValidateRequiredPropertiesForToString()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var missingProperties = new List<string>();

            foreach (var property in properties)
            {
                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute == null)
                    continue;

                var keyAttribute = property.GetCustomAttribute<ConnectionStringKeyAttribute>();
                if (keyAttribute == null)
                    continue;

                var value = property.GetValue(this);
                if (value == null || (value is string str && string.IsNullOrEmpty(str)))
                {
                    missingProperties.Add($"{property.Name} (key: '{keyAttribute.KeyName}')");
                }
            }

            if (missingProperties.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Cannot serialize connection string: Required properties are missing or have null values: {string.Join(", ", missingProperties)}");
            }
        }
    }
}

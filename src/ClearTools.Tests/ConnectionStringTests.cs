using System;
using ClearTools.Configuration;
using ClearTools.Configuration.BuiltIn;
using ClearTools.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ClearTools.Tests
{
    public class ConnectionStringTests
    {
        #region Custom Connection String for Testing

        private class TestConnectionString : ConnectionStringBase
        {
            [ConnectionStringKey("Server")]
            [Required]
            public string? Server { get; set; }

            [ConnectionStringKey("Port")]
            public int? Port { get; set; }

            [ConnectionStringKey("Database")]
            public string? Database { get; set; }

            [ConnectionStringKey("Timeout")]
            public int Timeout { get; set; }

            public TestConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
                : base(connectionString, options)
            {
            }
        }

        private class OptionalOnlyConnectionString : ConnectionStringBase
        {
            [ConnectionStringKey("Key1")]
            public string? Key1 { get; set; }

            [ConnectionStringKey("Key2")]
            public string? Key2 { get; set; }

            public OptionalOnlyConnectionString(string connectionString, ConnectionStringParsingOptions? options = null)
                : base(connectionString, options)
            {
            }
        }

        #endregion

        #region Basic Parsing Tests

        [Fact]
        public void Parse_ValidConnectionString_PopulatesProperties()
        {
            // Arrange
            var connStr = "Server=localhost;Port=5432;Database=mydb";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Server);
            Assert.Equal(5432, result.Port);
            Assert.Equal("mydb", result.Database);
        }

        [Fact]
        public void Parse_MissingOptionalProperty_LeavesPropertyNull()
        {
            // Arrange
            var connStr = "Server=localhost";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Server);
            Assert.Null(result.Port);
            Assert.Null(result.Database);
        }

        [Fact]
        public void Parse_MissingRequiredProperty_ThrowsException()
        {
            // Arrange
            var connStr = "Port=5432;Database=mydb";

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => new TestConnectionString(connStr));
            Assert.Contains("Required key 'Server'", ex.Message);
            Assert.Contains("property 'Server'", ex.Message);
        }

        [Fact]
        public void Parse_EmptyConnectionString_ThrowsExceptionForRequiredProperties()
        {
            // Arrange
            var connStr = "";

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => new TestConnectionString(connStr));
            Assert.Contains("Required key 'Server'", ex.Message);
        }

        [Fact]
        public void Parse_CaseInsensitiveKeys_MatchesProperties()
        {
            // Arrange
            var connStr = "server=localhost;PORT=5432;DaTaBaSe=mydb";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Server);
            Assert.Equal(5432, result.Port);
            Assert.Equal("mydb", result.Database);
        }

        [Fact]
        public void Parse_CaseSensitiveKeys_DoesNotMatchIncorrectCase()
        {
            // Arrange
            var connStr = "server=localhost";
            var options = new ConnectionStringParsingOptions
            {
                KeyComparison = StringComparison.Ordinal
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => new TestConnectionString(connStr, options));
            Assert.Contains("Required key 'Server'", ex.Message);
        }

        [Fact]
        public void Parse_WhitespaceAroundKeyValue_TrimsCorrectly()
        {
            // Arrange
            var connStr = " Server = localhost ; Port = 5432 ";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Server);
            Assert.Equal(5432, result.Port);
        }

        #endregion

        #region Escaping Tests

        [Fact]
        public void Parse_EscapedDelimiter_IncludesDelimiterInValue()
        {
            // Arrange
            var connStr = @"Server=my\;server;Database=my\;database";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal("my;server", result.Server);
            Assert.Equal("my;database", result.Database);
        }

        [Fact]
        public void Parse_DisableEscaping_TreatsBackslashAsLiteral()
        {
            // Arrange
            var connStr = @"Server=my\;server;Database=test";
            var options = new ConnectionStringParsingOptions { EnableEscaping = false };

            // Act
            var result = new TestConnectionString(connStr, options);

            // Assert
            Assert.Equal(@"my\", result.Server); // Splits at semicolon, so "my\" is the value
        }

        [Fact]
        public void ToString_EscapedValue_EscapesDelimiter()
        {
            // Arrange
            var connStr = @"Server=my\;server;Port=5432";
            var result = new TestConnectionString(connStr);

            // Act
            var serialized = result.ToString();

            // Assert
            Assert.Contains(@"Server=my\;server", serialized);
            Assert.Contains("Port=5432", serialized);
        }

        #endregion

        #region Custom Delimiter Tests

        [Fact]
        public void Parse_CustomDelimiter_ParsesCorrectly()
        {
            // Arrange
            var connStr = "Server=localhost|Port=5432|Database=mydb";
            var options = new ConnectionStringParsingOptions { Delimiter = "|" };

            // Act
            var result = new TestConnectionString(connStr, options);

            // Assert
            Assert.Equal("localhost", result.Server);
            Assert.Equal(5432, result.Port);
            Assert.Equal("mydb", result.Database);
        }

        [Fact]
        public void ToString_CustomDelimiter_UsesCustomDelimiter()
        {
            // Arrange
            var connStr = "Server=localhost|Port=5432";
            var options = new ConnectionStringParsingOptions { Delimiter = "|" };
            var result = new TestConnectionString(connStr, options);

            // Act
            var serialized = result.ToString();

            // Assert
            Assert.Contains("|", serialized);
            Assert.DoesNotContain(";", serialized);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_WithValues_ReturnsFormattedConnectionString()
        {
            // Arrange
            var connStr = "Server=localhost;Port=5432;Database=mydb";
            var result = new TestConnectionString(connStr);

            // Act
            var serialized = result.ToString();

            // Assert
            Assert.Contains("Server=localhost", serialized);
            Assert.Contains("Port=5432", serialized);
            Assert.Contains("Database=mydb", serialized);
        }

        [Fact]
        public void ToString_ExcludesNullProperties()
        {
            // Arrange
            var connStr = "Server=localhost;Port=5432";
            var result = new TestConnectionString(connStr);

            // Act
            var serialized = result.ToString();

            // Assert
            Assert.Contains("Server=localhost", serialized);
            Assert.Contains("Port=5432", serialized);
            Assert.DoesNotContain("Database", serialized);
        }

        [Fact]
        public void ToString_RoundTrip_PreservesValues()
        {
            // Arrange
            var original = "Server=localhost;Port=5432;Database=mydb";
            var parsed = new TestConnectionString(original);

            // Act
            var serialized = parsed.ToString();
            var reparsed = new TestConnectionString(serialized);

            // Assert
            Assert.Equal(parsed.Server, reparsed.Server);
            Assert.Equal(parsed.Port, reparsed.Port);
            Assert.Equal(parsed.Database, reparsed.Database);
        }

        #endregion

        #region Type Conversion Tests

        [Fact]
        public void Parse_IntegerProperty_ConvertsCorrectly()
        {
            // Arrange
            var connStr = "Server=localhost;Port=5432";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal(5432, result.Port);
        }

        [Fact]
        public void Parse_InvalidIntegerValue_ThrowsException()
        {
            // Arrange
            var connStr = "Server=localhost;Port=invalid";

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => new TestConnectionString(connStr));
            Assert.Contains("Failed to convert value 'invalid'", ex.Message);
            Assert.Contains("property 'Port'", ex.Message);
        }

        #endregion

        #region Built-In Connection String Tests

        [Fact]
        public void SqlServerConnectionString_ParsesCorrectly()
        {
            // Arrange
            var connStr = "Server=localhost;Database=MyDb;User Id=sa;Password=mypass;Encrypt=true";

            // Act
            var result = new SqlServerConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Server);
            Assert.Equal("MyDb", result.Database);
            Assert.Equal("sa", result.UserId);
            Assert.Equal("mypass", result.Password);
            Assert.True(result.Encrypt);
        }

        [Fact]
        public void ServiceBusConnectionString_ParsesCorrectly()
        {
            // Arrange
            var connStr = "Endpoint=sb://myservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mykey123";

            // Act
            var result = new ServiceBusConnectionString(connStr);

            // Assert
            Assert.Equal("sb://myservicebus.servicebus.windows.net/", result.Endpoint);
            Assert.Equal("RootManageSharedAccessKey", result.SharedAccessKeyName);
            Assert.Equal("mykey123", result.SharedAccessKey);
        }

        [Fact]
        public void CosmosDbConnectionString_ParsesCorrectly()
        {
            // Arrange
            var connStr = "AccountEndpoint=https://myaccount.documents.azure.com:443/;AccountKey=mykey123;Database=mydb";

            // Act
            var result = new CosmosDbConnectionString(connStr);

            // Assert
            Assert.Equal("https://myaccount.documents.azure.com:443/", result.AccountEndpoint);
            Assert.Equal("mykey123", result.AccountKey);
            Assert.Equal("mydb", result.Database);
        }

        [Fact]
        public void RedisConnectionString_ParsesCorrectly()
        {
            // Arrange
            var connStr = "Host=localhost;Port=6379;Password=mypass;Ssl=true;Database=0";

            // Act
            var result = new RedisConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Host);
            Assert.Equal(6379, result.Port);
            Assert.Equal("mypass", result.Password);
            Assert.True(result.Ssl);
            Assert.Equal(0, result.Database);
        }

        [Fact]
        public void PostgreSqlConnectionString_ParsesCorrectly()
        {
            // Arrange
            var connStr = "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=mypass";

            // Act
            var result = new PostgreSqlConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Host);
            Assert.Equal(5432, result.Port);
            Assert.Equal("mydb", result.Database);
            Assert.Equal("postgres", result.Username);
            Assert.Equal("mypass", result.Password);
        }

        [Fact]
        public void MongoDbConnectionString_ParsesCorrectly()
        {
            // Arrange
            var connStr = "Host=localhost;Port=27017;Database=mydb;Username=user;Password=pass;AuthSource=admin";

            // Act
            var result = new MongoDbConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Host);
            Assert.Equal(27017, result.Port);
            Assert.Equal("mydb", result.Database);
            Assert.Equal("user", result.Username);
            Assert.Equal("pass", result.Password);
            Assert.Equal("admin", result.AuthSource);
        }

        #endregion

        #region DI Integration Tests

        [Fact]
        public void AddConnectionString_RegistersAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();
            var connStr = "Server=localhost;Port=5432;Database=mydb";

            // Act
            services.AddConnectionString<TestConnectionString>(connStr);
            var provider = services.BuildServiceProvider();

            // Assert
            var instance1 = provider.GetRequiredService<TestConnectionString>();
            var instance2 = provider.GetRequiredService<TestConnectionString>();
            Assert.Same(instance1, instance2);
            Assert.Equal("localhost", instance1.Server);
        }

        [Fact]
        public void AddConnectionString_WithConfiguration_RetrievesFromConfigKey()
        {
            // Arrange
            var configData = new System.Collections.Generic.Dictionary<string, string?>
            {
                { "ConnectionStrings:MyDatabase", "Server=localhost;Port=5432;Database=mydb" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var services = new ServiceCollection();

            // Act
            services.AddConnectionString<TestConnectionString>(configuration, "ConnectionStrings:MyDatabase");
            var provider = services.BuildServiceProvider();

            // Assert
            var instance = provider.GetRequiredService<TestConnectionString>();
            Assert.Equal("localhost", instance.Server);
            Assert.Equal(5432, instance.Port);
            Assert.Equal("mydb", instance.Database);
        }

        [Fact]
        public void AddConnectionString_WithMissingConfigKey_ThrowsException()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                services.AddConnectionString<TestConnectionString>(configuration, "ConnectionStrings:Missing"));
            Assert.Contains("Configuration key 'ConnectionStrings:Missing'", ex.Message);
        }

        [Fact]
        public void AddConnectionString_NullConnectionString_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                services.AddConnectionString<TestConnectionString>(null!));
        }

        [Fact]
        public void AddConnectionString_WithOptions_UsesProvidedOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var connStr = "Server=localhost|Port=5432";
            var options = new ConnectionStringParsingOptions { Delimiter = "|" };

            // Act
            services.AddConnectionString<TestConnectionString>(connStr, options);
            var provider = services.BuildServiceProvider();

            // Assert
            var instance = provider.GetRequiredService<TestConnectionString>();
            Assert.Equal("localhost", instance.Server);
            Assert.Equal(5432, instance.Port);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Parse_EmptyValue_SetsPropertyToEmptyString()
        {
            // Arrange
            var connStr = "Server=;Database=mydb";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal("", result.Server);
            Assert.Equal("mydb", result.Database);
        }

        [Fact]
        public void Parse_DuplicateKeys_UsesLastValue()
        {
            // Arrange
            var connStr = "Server=first;Server=second;Port=5432";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal("second", result.Server);
        }

        [Fact]
        public void Parse_KeyWithoutValue_Ignored()
        {
            // Arrange
            var connStr = "Server=localhost;InvalidKey;Port=5432";

            // Act
            var result = new TestConnectionString(connStr);

            // Assert
            Assert.Equal("localhost", result.Server);
            Assert.Equal(5432, result.Port);
        }

        [Fact]
        public void Parse_OnlyOptionalProperties_DoesNotThrow()
        {
            // Arrange
            var connStr = "Key1=value1";

            // Act
            var result = new OptionalOnlyConnectionString(connStr);

            // Assert
            Assert.Equal("value1", result.Key1);
            Assert.Null(result.Key2);
        }

        [Fact]
        public void Parse_EmptyStringWithOnlyOptional_DoesNotThrow()
        {
            // Arrange
            var connStr = "";

            // Act
            var result = new OptionalOnlyConnectionString(connStr);

            // Assert
            Assert.Null(result.Key1);
            Assert.Null(result.Key2);
        }

        #endregion
    }
}

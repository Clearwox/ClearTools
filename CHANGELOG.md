# Changelog

All notable changes to this project will be documented in this file.

## [3.3.5] - 2026-02-24
### Added
- **ENHANCED**: Request Validation Middleware - Path Exclusion Support
  - Added `excludedPaths` parameter to `RequestValidationMiddleware` constructors
  - Added `ExcludedPaths` property to `RequestValidationOption` for specifying paths to exclude from validation
  - Case-insensitive path matching with support for exact matches and prefix matching (e.g., "/health", "/api/public")
  - Paths can be excluded using exact match or by prefix (e.g., "/api/public" excludes all routes starting with that path)
  - **Use cases**: Health check endpoints, public APIs, webhook receivers, static file paths
  - **Benefits**:
    - Fine-grained control over which endpoints require validation
    - Eliminates need for multiple middleware configurations
    - Cleaner code for mixed public/private API scenarios
    - Maintains backward compatibility (excludedPaths is optional)

### Fixed
- **BUGFIX**: `RequestValidationMiddleware` constructor using `RequestValidationOption` now correctly sets `_skipRoot` field
  - Previously, the field was not being assigned from `option.SkipForRootEndPoint`
  - This bug caused inconsistent behavior when using the options-based constructor

### Changed
- **IMPROVED**: `AddRequestValidation()` extension method signature updated to include `excludedPaths` parameter
- Updated version from 3.3.0 to 3.3.5

### Technical Details
- Path exclusion uses case-insensitive comparison (`StringComparison.OrdinalIgnoreCase`)
- Supports both exact path matching and prefix-based matching
- Empty or null `excludedPaths` safely defaults to empty array
- All existing code remains fully compatible (optional parameter)

## [3.3.1] - 2026-01-10
### Added
- **ENHANCED**: Connection String Configuration Framework - Flexible Instantiation Patterns
  - Added `Initialize(string connectionString, ConnectionStringParsingOptions? options = null)` public method to `ConnectionStringBase`
  - Added protected parameterless constructor to `ConnectionStringBase` for minimal boilerplate in derived classes
  - Made `Options` property settable to support Initialize pattern
  - Made parameterized constructor public for direct base class usage
  - **Three instantiation patterns now supported**:
    1. **Constructor-Based** (traditional): Pass connection string to constructor via `base(connectionString, options)`
    2. **Initialize Method** (minimal boilerplate): Use parameterless constructor + call `Initialize()` explicitly
    3. **Manual Property Setting**: Set properties directly, serialize with `ToString()`
  - Enhanced `ToString()` validation: Now validates required properties before serialization (catches manual property setting errors)
  - Auto-detection in DI: `AddConnectionString<T>()` automatically detects which pattern the type supports via triple fallback
  - **Benefits**:
    - Derived classes can omit constructors entirely (use inherited parameterless constructor)
    - Reduces boilerplate for simple connection string types
    - Maintains backward compatibility with all existing built-in types
    - Provides flexibility for different usage scenarios

### Changed
- **IMPROVED**: `ConnectionStringExtensions.AddConnectionString<T>()` now uses triple try-catch fallback:
  1. Try `(string connectionString, ConnectionStringParsingOptions? options)` constructor
  2. Fall back to `(string connectionString)` constructor
  3. Fall back to parameterless constructor + `Initialize(connectionString, options)`
- **IMPROVED**: Validation now occurs in both `Initialize()` (for connection string parsing) and `ToString()` (for manual property setting)

### Technical Details
- 11 new tests added covering Initialize() method, manual property setting, ToString() validation, and DI auto-detection
- Total test count increased from 172 to 182 tests (all passing)
- No breaking changes - all existing code continues to work unchanged

## [3.3.0] - 2026-01-09
### Added
- **NEW**: Connection String Configuration Framework (`ClearTools.Configuration`)
  - Strongly-typed, attribute-driven parsing and management of connection strings
  - Core framework components:
    - `ConnectionStringBase` - Abstract base class with reflection-based parsing and validation
    - `ConnectionStringKeyAttribute` - Maps properties to connection string keys
    - `RequiredAttribute` - Marks properties as mandatory (fail-fast validation)
    - `ConnectionStringParsingOptions` - Configurable delimiter, escaping, case sensitivity
  - 10 built-in connection string types in `ClearTools.Configuration.BuiltIn`:
    - **Azure Services**: `ServiceBusConnectionString`, `AppConfigurationConnectionString`, `KeyVaultConnectionString`, `SqlServerConnectionString`, `CosmosDbConnectionString`, `BlobStorageConnectionString`
    - **Non-Azure Services**: `MongoDbConnectionString`, `PostgreSqlConnectionString`, `RedisConnectionString`, `RabbitMqConnectionString`
  - Advanced features:
    - Custom delimiter support (default `;`, configurable to `|`, `,`, etc.)
    - Delimiter escaping with backslash (e.g., `my\;value` includes semicolon in value)
    - Configurable case sensitivity for key matching (default case-insensitive)
    - Bidirectional conversion: parse from string, serialize to string with `ToString()`
    - Automatic type conversion for int, bool, and other primitive types
    - Only non-null properties included in `ToString()` serialization
  - Dependency Injection integration via `ConnectionStringExtensions`:
    - `AddConnectionString<T>(string)` - Register from raw string value
    - `AddConnectionString<T>(IConfiguration, string)` - Register from configuration key
    - Registers as singleton for constructor injection
  - Comprehensive test suite (33 tests) covering parsing, validation, escaping, delimiters, case sensitivity, DI, and edge cases
  - Full documentation in USAGE.md with examples for built-in types, custom types, DI integration, and best practices

### Use Cases
- Traditional database connection strings (SQL Server, PostgreSQL, MongoDB, Redis)
- Azure service connection strings (Service Bus, Key Vault, Cosmos DB, Blob Storage)
- Custom service configurations stored as single strings in Key Vault or App Configuration
- Type-safe configuration management with compile-time property checking
- Simplified DI registration for connection string-based configurations

## [3.2.1] - 2026-01-09
### Added
- **NEW**: StringBuilder extension methods (`StringBuilderExtensions`)
  - `AppendIfTrue()` - Conditionally append text based on boolean condition
  - `AppendLineIfTrue()` - Conditionally append line based on boolean condition
  - Enables fluent, readable conditional string building

### Changed
- **IMPROVED**: Azure Storage Manager API flexibility
  - Made `folder` parameter nullable in all download methods
  - Made `folder` parameter nullable in delete methods
  - Allows direct container root operations without specifying folder paths
  - Backwards compatible - existing code continues to work

### Fixed
- Improved nullable reference type annotations for Azure Storage Manager methods

## [3.2.0] - 2025-12-22
### Added
- **NEW**: Comprehensive Azure App Configuration integration (`AppConfigurationExtensions`)
  - Dual authentication support: endpoint + credential (Managed Identity) or connection string
  - Label-based environment-specific configuration (Production, Staging, etc.)
  - Key filter support with wildcard patterns (`MyApp:*`)
  - Attribute-based feature flag integration with `[FeatureFlag]` attribute
    - Runtime validation ensuring feature flags only on bool/bool? properties
    - Automatic feature flag JSON parsing from Azure App Configuration
  - Optional configuration refresh with sentinel keys and custom intervals
    - User-controlled error handling (silent by default, custom handler support)
    - Minimum 30-second refresh interval recommended
  - Custom key mapping with `[AppConfigurationKey]` attribute
  - Development environment fallback to environment variables
  - Automatic type conversion with comprehensive error handling
  - Singleton DI registration with fluent API support
- Extension methods for both Web Applications and Azure Functions
  - `AddAppConfigurationForWebApplication<T>()` - IConfiguration integration
  - `AddAppConfigurationForAzureFunctions<T>()` - Direct ConfigurationClient access
  - `CreateSettingsFromAppConfigurationAsync<T>()` - Standalone async usage
  - `CreateSettingsFromAppConfiguration<T>()` - Standalone sync usage
- New NuGet dependencies:
  - `Azure.Data.AppConfiguration` (1.4.1)
  - `Microsoft.Extensions.Configuration.AzureAppConfiguration` (8.0.0)
- Comprehensive test coverage (18 unit tests) covering all scenarios
- Extensive XML documentation with usage examples and best practices

### Documentation
- Added detailed App Configuration examples in USAGE.md
- Documented authentication method recommendations (Managed Identity vs connection string)
- Refresh interval guidance and sentinel key patterns
- Case-sensitivity warnings for configuration keys
- SDK limitation notes for refresh error handling

## [3.1.1] - 2025-11-14
### Added
- **NEW**: `FluentDictionary<TKey, TValue>` class for fluent dictionary operations
  - Chainable `Add()` and `Remove()` methods that return the dictionary instance
  - Static `Create()` factory method for convenient initialization
  - Inherits all base `Dictionary<TKey, TValue>` functionality
- Comprehensive unit tests for `FluentDictionary` (20+ test cases covering all scenarios)
- Comprehensive unit tests for `SmartEnum` (20+ test cases covering parsing, equality, and type safety)

### Fixed
- Fixed `KeyVaultExtensionsTests` to properly mock `IHostEnvironment.EnvironmentName` property
- Updated test mocking from deprecated `IsDevelopment()` extension method to direct `EnvironmentName` property access
- Improved test reliability for Azure Key Vault configuration tests

### Changed
- Enhanced test coverage for core utility classes
- Improved code quality and maintainability

## [3.1.0] - 2025-11-13
### Changed
- **BREAKING**: Renamed `SmartEnum.FromValue()` to `SmartEnum.Parse(int value)` for better API consistency
- **BREAKING**: Renamed `SmartEnum.FromName()` to `SmartEnum.Parse(string name)` for better API consistency

### Fixed
- Fixed SmartEnum static constructor initialization issues
- Added explicit static constructor invocation in `List()`, `Parse(int)`, and `Parse(string)` methods to ensure proper enum registration
- Resolved potential race conditions in SmartEnum initialization

## [3.0.9] - 2025-08-21
### Fixed
- Fixed StringUtility.StripSymbols method to preserve spaces between alphanumeric characters
- Updated regex pattern from `[^a-zA-Z0-9]` to `[^a-zA-Z0-9\s]` to retain whitespace
- Enhanced test coverage with additional test cases for space preservation

### Changed
- Improved StringUtility.StripSymbols behavior for better text processing

## [3.0.8] - 2025-07-03
### Added
- Key Vault functionality: Extension on IHostBuilder for Azure Key Vault integration
- Service extension for automatically adding services by common interface

### Changed
- Refactored SmartEnum class for improved functionality and performance
- Updated version to 3.0.8

## [3.0.7] - 2025-06-30
### Added
- Comprehensive unit tests for API client and utility classes
- Enhanced API client with improved error handling and response management
- New utility classes and enhanced existing functionality
- Service extension for automatically adding services by common interface
- EditorJS models updated to latest version
- OTP utility code generation without timespan dependency

### Changed
- Refactored project structure and updated dependencies
- Enhanced API client configuration and functionality
- Improved project configuration and added new features
- Updated Azure.Storage.Blobs package to latest version
- Enhanced README documentation

### Fixed
- Various bug fixes and performance improvements

## [3.0.5] - 2025-06-24
### Changed
- Updated and added more symbols to StripSymbols method in StringUtility
- Enhanced string manipulation capabilities

## [2.1.17] - 2021-08-24
### Added
- Time ago function added to StringUtility class
- File upload to Azure with spread out functionality

### Changed
- Released NuGet package version 2.1.17

## [2.1.16] - 2022-07-21
### Added
- Azure uploads both synchronous and asynchronous methods
- HttpClientFactory implementation
- API validation middleware for request authentication
- Google reCAPTCHA resolution functionality

### Changed
- Azure blob search functionality refactored (2022-09-17)

## [2.1.2] - 2019-07-16
### Added
- PUT and DELETE request methods to IApiClient interface
- Enhanced HTTP client capabilities

### Changed
- Version updated to 2.1.2

## [2.0.x] - 2021
### Added
- Generate and validate code functionality for OTP operations
- Provision for country and currency features support
- Comprehensive file management utilities

### Changed
- Major refactoring and feature additions throughout 2021

## [1.0.0] - 2018-12-29
### Added
- Initial project setup and first commit
- Basic utility classes and functions
- Git repository initialization
- Initial README.md documentation
- Base project structure and configuration

### Features Included in Initial Release
- String manipulation utilities
- Basic cryptography functions  
- File management operations
- Image processing capabilities
- HTTP client utilities
- Number base conversion tools

# ClearTools Library Documentation

## Overview

ClearTools is a comprehensive .NET utility library that provides various tools, extensions, and utilities for common development tasks. This library is designed to accelerate development by providing well-tested, reusable components for string manipulation, cryptography, file operations, image processing, API communication, and more.

## Table of Contents

- [Installation](#installation)
- [String Utilities](#string-utilities)
- [String Extensions](#string-extensions)
- [Cryptography Tools](#cryptography-tools)
- [Encryption Utilities](#encryption-utilities)
- [Base Conversion](#base-conversion)
- [Date Utilities](#date-utilities)
- [File Management](#file-management)
- [Image Processing](#image-processing)
- [OTP (One-Time Password) Utilities](#otp-one-time-password-utilities)
- [EditorJS Parser](#editorjs-parser)
- [Azure Storage Manager](#azure-storage-manager)
- [API Client](#api-client)
- [Request Validation Middleware](#request-validation-middleware)
- [Common Utilities](#common-utilities)
- [Smart Enums](#smart-enums)
- [Models and Data Types](#models-and-data-types)
- [Extensions](#extensions)
- [Enums](#enums)
- [Exceptions](#exceptions)

## Installation

Add the ClearTools package to your project:

```xml
<PackageReference Include="ClearTools" Version="3.0.5" />
```

## String Utilities

The `StringUtility` class provides various string manipulation and generation methods.

### Namespace
```csharp
using Clear.Tools;
```

### Methods

#### Date and Time Utilities

```csharp
// Generate a date-based string with current date/time values
string dateString = StringUtility.AddUpDate();
string dateStringWithPadding = StringUtility.AddUpDate(100);

// Generate a date code from current file time
string dateCode = StringUtility.GetDateCode();
```

#### URL and SEO Utilities

```csharp
// Generate SEO-friendly URL keys
string urlKey = StringUtility.GenerateUrlKey("Hello World & More!");
// Result: "hello-world-more"

// Generate comma-separated tags
string tags = StringUtility.GenerateTags("tag1", "tag2", "tag3");
// Result: "tag1,tag2,tag3"
```

#### HTML and Text Processing

```csharp
// Strip HTML tags and clean text
string cleanText = StringUtility.StripHTML("<p>Hello <b>World</b></p>");
// Result: "Hello World"

// Strip symbols from text (preserves spaces)
string cleanSymbols = StringUtility.StripSymbols("Hello @World# 2023!");
// Result: "Hello World 2023"

// Combine both operations  
string clean = StringUtility.StripSymbolsAndHTML("<p>Hello @World# 2023!</p>");
// Result: "Hello World 2023"
```

#### String Manipulation

```csharp
// Get substring from start index
string sub1 = StringUtility.GetSubstring("Hello World", 6);
// Result: "World"

// Get substring with count
string sub2 = StringUtility.GetSubstring("Hello World", 0, 5);
// Result: "Hello"
```

#### File Name Generation

```csharp
// Generate unique file names
string fileName1 = StringUtility.GenerateFileName("My Document", "pdf");
string fileName2 = StringUtility.GenerateFileName("My Document", "pdf", "MySite");
// Result examples: "MySite-My-Document-Site-abc12.pdf"
```

#### Time Display

```csharp
DateTime pastDate = DateTime.Now.AddHours(-2);
DateTime currentTime = DateTime.Now;

// Display time since a date
string timeSince = StringUtility.TimeSince(pastDate, currentTime);
// Result: "2 hours ago"

// Alternative time ago format
string timeAgo = StringUtility.TimeAgo(pastDate, currentTime);
// Result: "about 2 hours ago"
```

#### Name Processing

```csharp
// Extract initials from full name
string initials = StringUtility.ExtractInitialsFromName("John Doe Smith");
// Result: "JS"
```

#### Token Generation

```csharp
// Generate secure random tokens
string token = StringUtility.GenerateToken(
    length: 16,
    includeSmallLetters: true,
    includeCapitalLetters: true,
    includeNumbers: true,
    includeSpecialCharacters: false
);
// Result: Random 16-character alphanumeric string
```

#### XML/HTML Conversion

```csharp
// Convert line breaks to HTML paragraphs
string html = StringUtility.CreateParagraphsFromReturns("Line 1\nLine 2\nLine 3");
// Result: "<div><p>Line 1</p><p>Line 2</p><p>Line 3</p></div>"

// Convert HTML paragraphs back to line breaks
string text = StringUtility.CreateReturnsFromParagraphs(html);
// Result: "Line 1\nLine 2\nLine 3"
```

#### Validation Code Generation (Legacy)

```csharp
// Generate validation codes (deprecated - use OtpUtility instead)
DateTime expiry = DateTime.Now.AddHours(1);
string validationCode = StringUtility.GenerateValidationCode("user@example.com", expiry, 12345);

// Validate codes
bool isValid = StringUtility.ValidationCode(validationCode, "user@example.com", expiry, 12345);
```

#### SQL Serialization

```csharp
// Escape strings for SQL
string escaped = StringUtility.SQLSerialize("Don't escape me");
// Result: "Don''t escape me"

// Convert boolean to SQL
string boolSql = StringUtility.SQLSerialize(true);
// Result: "1"

// Convert DateTime to SQL
string dateSql = StringUtility.SQLSerialize(DateTime.Now);
// Result: "15/Aug/2025 14:30:00"
```

## String Extensions

Extension methods for the `string` type to provide additional functionality.

### Namespace
```csharp
using ClearTools.Extensions;
```

### Methods

```csharp
string text = "Hello World";

// Toggle between values
string toggled = text.Toggle("Hello World", "Goodbye World");
// Result: "Goodbye World"

// Toggle with single value (empty/value)
string toggledSingle = "".Toggle("Default Value");
// Result: "Default Value"

// Case-insensitive search
bool found = text.Search("WORLD");
// Result: true

// Strip specific symbols (extension method - preserves spaces and other characters)
string clean = text.StripSymbols();
// Removes only: ;\/:"<>|&'+`',/()[]{}\"#*
// Preserves: spaces, letters, numbers, and other symbols not in the list

// Note: StringUtility.StripSymbols() removes ALL symbols except alphanumeric and spaces

// Extract only numbers
string numbers = "Price: $123.45".ExtractNumbers();
// Result: "123.45"

// Convert to numeric types
int intVal = "123.45".ToInt32();
decimal decVal = "123.45".ToDecimal();
double doubleVal = "123.45".ToDouble();

// Convert to DateTime
DateTime date = "2025-08-15".ToDateTime();

// Base64 operations
byte[] bytes = "SGVsbG8gV29ybGQ=".FromBase64String();

// Case-insensitive comparison
bool equal = text.EqualsNoCase("HELLO WORLD");
// Result: true

// CSV to collections
List<string> list = "apple,banana,cherry".ToListFromCsv();
HashSet<string> set = "apple,banana,cherry".ToHashSetFromCsv();
```

## Cryptography Tools

The `Crypto` class provides hashing and encoding utilities.

### Namespace
```csharp
using Clear.Tools;
```

### Methods

```csharp
// Generate cryptographic salt
string salt = Crypto.CreateSalt(128); // Default size is 128

// SHA512 hashing with optional salt
string hash512 = Crypto.EncodeSHA512("password", salt);

// SHA256 hashing with optional salt
string hash256 = Crypto.EncodeSHA256("password", salt);

// SHA1 hashing (without salt)
string hash1 = Crypto.EncodeSHA1("password");

// Base64 encoding/decoding
string encoded = Crypto.EncodeBase64("Hello World");
string decoded = Crypto.DecodeBase64(encoded);
```

## Encryption Utilities

The `Encryption` class provides AES encryption and decryption.

### Namespace
```csharp
using Clear.Tools;
```

### Requirements
- Encryption key must be at least 32 characters long

### Methods

```csharp
string key = "MySecretEncryptionKeyThatIsLongEnough123"; // Must be 32+ chars
string plainText = "Sensitive information";

// Encrypt data
string encrypted = Encryption.Encrypt(plainText, key);

// Decrypt data
string decrypted = Encryption.Decrypt(encrypted, key);
```

## Base Conversion

The `BaseConverter` class provides number base conversion utilities.

### Namespace
```csharp
using Clear.Tools;
```

### Methods

```csharp
// Convert from any base (2-36) to decimal
long decimal_value = BaseConverter.ConvertToDecimal("FF", 16);
// Result: 255

// Convert decimal to any base (2-36)
string hex_value = BaseConverter.ConvertFromDecimal(255, 16);
// Result: "FF"

// Convert decimal to alphabetic representation
string alpha = BaseConverter.ConvertToAlpha(26);
// Result: "BA" (base-26 using A-Z)
```

## Date Utilities

The `DateUtility` class provides date comparison utilities.

### Namespace
```csharp
using Clear.Tools;
```

### Methods

```csharp
DateTime date1 = new DateTime(2025, 8, 12); // Monday
DateTime date2 = new DateTime(2025, 8, 15); // Thursday

// Check if dates are in the same week
bool sameWeek = DateUtility.IsSameWeek(date1, date2);
// Result: true (both in same week)

// Specify first day of week
bool sameWeekSunday = DateUtility.IsSameWeek(date1, date2, DayOfWeek.Sunday);
```

## File Management

The `FileManager` class provides file I/O operations.

### Namespace
```csharp
using Clear.Tools;
```

### Synchronous Methods

```csharp
string filePath = @"C:\temp\myfile.txt";
string content = "Hello, World!";

// Write to file
FileManager.Write(filePath, content);

// Read from file
string readContent = FileManager.Read(filePath);
```

### Asynchronous Methods

```csharp
// Write to file asynchronously
await FileManager.WriteAsync(filePath, content);

// Read from file asynchronously
string readContent = await FileManager.ReadAsync(filePath);
```

## Image Processing

The `ImageUtility` class provides image manipulation utilities.

### Namespace
```csharp
using Clear.Tools;
using System.Drawing;
using System.Drawing.Imaging;
```

### Methods

```csharp
// Convert bitmap to byte array
Bitmap bitmap = new Bitmap("image.jpg");
byte[] imageBytes = ImageUtility.ConvertBitmapToBytes(bitmap, ImageFormat.Jpeg);

// Scale image maintaining aspect ratio
Image scaledImage = ImageUtility.ScaleImage(bitmap, 800, 600);

// Scale with size preference
Image widthScaled = ImageUtility.ScaleImage(bitmap, 800, 600, ImageSizePreference.Width);

// Crop image
Image croppedImage = ImageUtility.CropImage(bitmap, 400, 300);

// Resize image (may distort)
Bitmap resizedImage = ImageUtility.ResizeImage(bitmap, 800, 600);

// Save JPEG with quality control
ImageUtility.SaveJpegToFile("output.jpg", bitmap, 90); // 90% quality

// Save JPEG to memory stream
using var stream = new MemoryStream();
ImageUtility.SaveJpegToStream(stream, bitmap, 85);

// Convert image to Base64
string base64 = ImageUtility.ConvertImageToBase64(bitmap, ImageFormat.Png);

// Convert Base64 to image
Image imageFromBase64 = ImageUtility.ConvertBase64ToImage(base64);
```

## OTP (One-Time Password) Utilities

The `OtpUtility` class provides secure OTP generation and validation.

### Namespace
```csharp
using Clear.Tools;
using Clear.Tools.Models;
```

### Methods

```csharp
string identifier = "user@example.com";
int secretKey = 123456;

// Generate OTP with expiry duration
ValidationCodeResult result1 = OtpUtility.GenerateCode(
    identifier, 
    secretKey, 
    TimeSpan.FromMinutes(5), 
    6 // code length
);

// Generate OTP with automatic expiry (next hour)
ValidationCodeResult result2 = OtpUtility.GenerateCode(identifier, secretKey, 6);

// Access the generated code and expiry
string code = result1.Code;
DateTime expiry = result1.ExpiryTime;

// Validate OTP with expiry time
bool isValid1 = OtpUtility.ValidateCode(identifier, secretKey, code, expiry, 6);

// Validate OTP with automatic expiry calculation
bool isValid2 = OtpUtility.ValidateCode(identifier, secretKey, code, 6);
```

## EditorJS Parser

The `EditorJS` class converts EditorJS JSON content to HTML.

### Namespace
```csharp
using Clear.Tools;
using Clear.Models.EditorJS;
```

### Supported Block Types
- Header (h1-h5)
- Paragraph
- List (ordered/unordered)
- Image
- Embed (YouTube, Vimeo)

### Methods

```csharp
// Parse from JSON string
string editorJsonContent = @"{
    ""time"": 1692105600000,
    ""blocks"": [
        {
            ""id"": ""block1"",
            ""type"": ""header"",
            ""data"": {
                ""text"": ""My Title"",
                ""level"": 1
            }
        },
        {
            ""id"": ""block2"",
            ""type"": ""paragraph"",
            ""data"": {
                ""text"": ""This is a paragraph.""
            }
        }
    ],
    ""version"": ""2.28.0""
}";

string html = EditorJS.Parse(editorJsonContent);

// Parse from Content object
Content content = JsonConvert.DeserializeObject<Content>(editorJsonContent);
string htmlFromObject = EditorJS.Parse(content);
```

## Azure Storage Manager

The `AzureStorageManager` class provides Azure Blob Storage operations.

### Namespace
```csharp
using Clear.Tools;
```

### Prerequisites
```xml
<PackageReference Include="Azure.Storage.Blobs" Version="12.x.x" />
```

### Upload Operations

```csharp
string connectionString = "DefaultEndpointsProtocol=https;AccountName=...";
string containerName = "mycontainer";
string folder = "documents";
string contentType = "application/pdf";

// Upload from Stream
using var stream = new FileStream("document.pdf", FileMode.Open);
AzureStorageManager.UploadToAzure(connectionString, containerName, stream, contentType, "document.pdf", folder);

// Upload from MemoryStream
using var memoryStream = new MemoryStream(bytes);
AzureStorageManager.UploadToAzure(connectionString, containerName, memoryStream, contentType, "document.pdf", folder);

// Upload from FileInfo
var fileInfo = new FileInfo("document.pdf");
AzureStorageManager.UploadToAzure(connectionString, containerName, fileInfo, contentType, folder);

// Async versions
await AzureStorageManager.UploadToAzureAsync(connectionString, containerName, stream, contentType, "document.pdf", folder);
```

### Download Operations

```csharp
// Download to FileInfo
var downloadFile = new FileInfo("downloaded.pdf");
AzureStorageManager.DownloadFromAzure(connectionString, containerName, downloadFile, folder);

// Download to MemoryStream
using var downloadStream = new MemoryStream();
AzureStorageManager.DownloadFromAzure(connectionString, containerName, "document.pdf", downloadStream, folder);

// Async versions
await AzureStorageManager.DownloadFromAzureAsync(connectionString, containerName, downloadFile, folder);
```

### Other Operations

```csharp
// Check if folder exists
bool folderExists = AzureStorageManager.AzureFolderExists(connectionString, containerName, folder);

// Delete file
AzureStorageManager.DeleteFromAzure(connectionString, containerName, "document.pdf", folder);

// Delete file (async)
await AzureStorageManager.DeleteFromAzureAsync(connectionString, containerName, "document.pdf", folder);
```

## API Client

The `ApiClient` class provides HTTP client functionality with built-in JSON serialization.

### Namespace
```csharp
using Clear;
using Clear.Models;
```

### Setup

```csharp
// Register with dependency injection
services.AddHttpClient<IApiClient, ApiClient>();

// Or create directly
var httpClient = new HttpClient();
var apiClient = new ApiClient(httpClient);
```

### GET Requests

```csharp
// Basic GET request
User user = await apiClient.GetAsync<User>("https://api.example.com/users/1");

// GET with authorization token
User userWithAuth = await apiClient.GetAsync<User>("https://api.example.com/users/1", "bearer_token");

// GET with custom headers
var headers = new Dictionary<string, string> { ["X-Custom"] = "value" };
User userWithHeaders = await apiClient.GetAsync<User>("https://api.example.com/users/1", headers);

// GET with token and headers
User userComplete = await apiClient.GetAsync<User>("https://api.example.com/users/1", "bearer_token", headers);
```

### POST Requests

```csharp
var newUser = new User { Name = "John Doe", Email = "john@example.com" };

// Basic POST (returns HttpResponseMessage)
HttpResponseMessage response = await apiClient.PostAsync("https://api.example.com/users", newUser);

// POST with result type
User createdUser = await apiClient.PostAsync<User, User>("https://api.example.com/users", newUser);

// POST with authorization
HttpResponseMessage authResponse = await apiClient.PostAsync("https://api.example.com/users", newUser, "bearer_token");

// POST with custom headers
HttpResponseMessage headerResponse = await apiClient.PostAsync("https://api.example.com/users", newUser, headers);
```

### PUT Requests

```csharp
var updatedUser = new User { Id = 1, Name = "Jane Doe", Email = "jane@example.com" };

// Basic PUT
HttpResponseMessage putResponse = await apiClient.PutAsync("https://api.example.com/users/1", updatedUser);

// PUT with result
User updated = await apiClient.PutAsync<User, User>("https://api.example.com/users/1", updatedUser);
```

### DELETE Requests

```csharp
// Basic DELETE
await apiClient.DeleteAsync("https://api.example.com/users/1");

// DELETE with result
DeleteResult result = await apiClient.DeleteWithResultAsync<DeleteResult>("https://api.example.com/users/1");

// DELETE with authorization
await apiClient.DeleteAsync("https://api.example.com/users/1", "bearer_token");
```

### reCAPTCHA Validation

```csharp
CaptcherResponse captchaResult = await apiClient.ValidateGoogleCaptcharAsync(
    secretKey: "your_secret_key",
    recaptchaResponse: "captcha_response_from_client",
    remoteip: "client_ip_address"
);

bool isHuman = captchaResult.Success;
```

### Response Inspection

```csharp
// Access last response details
string lastResponseJson = apiClient.LastResponseString;
HttpResponseMessage lastResponse = apiClient.LastResponse;
```

## Request Validation Middleware

ASP.NET Core middleware for API key validation.

### Namespace
```csharp
using Clear;
```

### Setup

```csharp
// In Program.cs or Startup.cs
services.AddRequestValidation("your_secret_api_key", skipForDevelopment: true, skipRootEndPoint: true);

// In middleware pipeline
app.UseRequestValidation();
```

### Advanced Configuration

```csharp
var options = new RequestValidationOption(
    validationKey: "your_secret_api_key",
    skipForDevelopment: true,
    skipForRootEndPoint: true
);

services.AddSingleton(options);
services.AddSingleton<RequestValidationMiddleware>();

app.UseMiddleware<RequestValidationMiddleware>();
```

### Client Usage

```csharp
// Include API key in request headers
var client = new HttpClient();
client.DefaultRequestHeaders.Add("key", "your_secret_api_key");

var response = await client.GetAsync("https://yourapi.com/protected-endpoint");
```

## Common Utilities

The `Common` class provides general utility functions.

### Namespace
```csharp
using Clear.Tools;
using Clear;
```

### Methods

```csharp
// Extract all exception messages (including inner exceptions)
try 
{
    // Some operation that might throw
}
catch (Exception ex)
{
    string allMessages = Common.GetAllExceptionMessage(ex);
    // Result: "Main message \n[INNER: Inner exception message]"
}

// Generate social media sharing links
string facebookLink = Common.GetShareLink(
    url: "https://example.com/article",
    sharer: Sharers.Facebook,
    description: "Check out this article",
    imageUrl: "https://example.com/image.jpg"
);

string twitterLink = Common.GetShareLink(
    url: "https://example.com/article",
    sharer: Sharers.Twitter,
    description: "Check out this article",
    imageUrl: "https://example.com/image.jpg"
);

string pinterestLink = Common.GetShareLink(
    url: "https://example.com/article",
    sharer: Sharers.Pinterest,
    description: "Check out this article",
    imageUrl: "https://example.com/image.jpg"
);
```

## Smart Enums

Type-safe enum alternatives with additional functionality.

### Namespace
```csharp
using Clear;
```

### Usage

```csharp
// Define a smart enum
public class Priority : SmartEnum<int>
{
    public static readonly Priority Low = new Priority("Low Priority", 1);
    public static readonly Priority Medium = new Priority("Medium Priority", 2);
    public static readonly Priority High = new Priority("High Priority", 3);

    private Priority(string name, int value) : base(name, value) { }
}

// Usage
Priority priority = Priority.High;
string name = priority.Name; // "High Priority"
int value = priority.Value; // 3
string display = priority.ToString(); // "High Priority"

// Comparison
bool isEqual = Priority.High.Equals(Priority.High); // true
```

## Models and Data Types

### ValidationCodeResult

Represents the result of OTP generation.

```csharp
using Clear.Tools.Models;

ValidationCodeResult result = OtpUtility.GenerateCode("user@example.com", 123456);
string code = result.Code;        // The generated OTP
DateTime expiry = result.ExpiryTime; // When the OTP expires
```

### CaptcherResponse

Represents Google reCAPTCHA validation response.

```csharp
using Clear.Models;

CaptcherResponse response = await apiClient.ValidateGoogleCaptcharAsync(...);
bool isValid = response.Success;
DateTime? timestamp = response.Challenge_ts;
string hostname = response.Hostname;
string[] errors = response.Errorcodes;

// Example usage
if (response.Success)
{
    // CAPTCHA validation passed
    Console.WriteLine($"CAPTCHA validated for {response.Hostname}");
}
else
{
    // CAPTCHA validation failed
    Console.WriteLine($"CAPTCHA failed with errors: {string.Join(", ", response.Errorcodes)}");
}
```

### EditorJS Models

Data models for EditorJS content parsing.

```csharp
using Clear.Models.EditorJS;

// Main content structure
Content content = JsonConvert.DeserializeObject<Content>(jsonString);
Block[] blocks = content.blocks;
string version = content.version;
long timestamp = content.time;

// Individual block
Block block = blocks[0];
string blockType = block.type; // "header", "paragraph", "list", etc.
Data blockData = block.data;
string blockText = blockData.text;
```

## Extensions

Additional extension methods for various types.

### ByteExtensions

```csharp
using ClearTools.Extensions;

byte[] data = new byte[] { 72, 101, 108, 108, 111 };
string base64 = data.ToBase64String(); // Convert to Base64
```

### DateTimeExtensions

```csharp
using ClearTools.Extensions;

DateTime date = DateTime.Now;

// Format as date string (dd/MMM/yyyy)
string dateString = date.ToDateString();
// Result: "15/Aug/2025"

// Format as date-time string (dd/MMM/yyyy HH:mm:ss)
string dateTimeString = date.ToDateTimeString();
// Result: "15/Aug/2025 14:30:45"
```

### KeyVaultExtensions

Comprehensive Azure Key Vault integration for configuration management.

```csharp
using ClearTools.Extensions;

// Define your settings class
public class AppSettings
{
    public string DatabaseConnectionString { get; set; }
    
    [KeyVaultKey("api-key")]
    public string ApiKey { get; set; }
    
    public string EmailServiceUrl { get; set; }
}

// For ASP.NET Web Applications (uses IConfiguration)
var builder = WebApplication.CreateBuilder(args);
builder.AddKeyVaultForWebApplication<AppSettings>(
    keyVaultUri: "https://your-keyvault.vault.azure.net/",
    out AppSettings settings,
    skipDevelopment: true
);

// For Azure Functions (direct SecretClient access)
var hostBuilder = new HostBuilder();
hostBuilder.ConfigureFunctionsWorkerDefaults()
    .AddKeyVaultForAzureFunctions<AppSettings>(
        keyVaultUri: "https://your-keyvault.vault.azure.net/",
        out AppSettings functionSettings,
        skipDevelopment: true
    );

// Direct Key Vault access (async)
AppSettings directSettings = await KeyVaultExtensions
    .CreateSettingsFromKeyVaultAsync<AppSettings>("https://your-keyvault.vault.azure.net/");

// Direct Key Vault access (sync)
AppSettings syncSettings = KeyVaultExtensions
    .CreateSettingsFromKeyVault<AppSettings>("https://your-keyvault.vault.azure.net/");
```

#### Key Vault Key Attribute

Use the `KeyVaultKeyAttribute` to map properties to different Key Vault secret names:

```csharp
public class Settings
{
    // Maps to "DatabaseConnectionString" secret
    public string DatabaseConnectionString { get; set; }
    
    // Maps to "custom-api-key" secret
    [KeyVaultKey("custom-api-key")]
    public string ApiKey { get; set; }
}
```

### ServicesExtensions

Automatic service registration by interface for dependency injection.

```csharp
using ClearTools.Extensions;

// Register all services implementing IService as Singletons
services.AddSingletonServicesByInterface<IService>();

// Register all services implementing IRepository as Scoped
services.AddScopedServicesByInterface<IRepository>();

// Register all services implementing IValidator as Transient
services.AddTransientServicesByInterface<IValidator>();

// Register from specific assembly
services.AddScopedServicesByInterface<IService>(typeof(MyService).Assembly);
```

Example usage:
```csharp
// Define interfaces and implementations
public interface IUserService { }
public interface IEmailService { }

public class UserService : IUserService { }
public class EmailService : IEmailService { }

// Auto-register all services
services.AddScopedServicesByInterface<object>(); // Registers all services

// Or register by specific interface hierarchy
public interface IApplicationService { }
public class UserService : IApplicationService { }
public class EmailService : IApplicationService { }

services.AddScopedServicesByInterface<IApplicationService>();
```

### AppConfigurationExtensions

Comprehensive Azure App Configuration integration for configuration management with support for labels, key filters, feature flags, and automatic refresh.

```csharp
using ClearTools.Extensions;

// Define your settings class with regular configuration and feature flags
public class AppSettings
{
    public string DatabaseConnectionString { get; set; }
    
    // Use custom key mapping
    [AppConfigurationKey("api-key")]
    public string ApiKey { get; set; }
    
    public string EmailServiceUrl { get; set; }
    
    public int MaxRetryCount { get; set; }
    
    // Feature flags - must be bool or bool?
    [FeatureFlag("enable-new-ui")]
    public bool NewUiEnabled { get; set; }
    
    [FeatureFlag("beta-features")]
    public bool? BetaFeaturesEnabled { get; set; }
}

// For ASP.NET Web Applications (uses IConfiguration) - Endpoint + Managed Identity
var builder = WebApplication.CreateBuilder(args);
builder.AddAppConfigurationForWebApplication<AppSettings>(
    appConfigEndpoint: "https://myapp.azconfig.io",
    out AppSettings settings,
    label: "Production",  // or null for default label
    keyFilter: "MyApp:*", // or null for all keys
    skipDevelopment: true
);

// For ASP.NET Web Applications - Connection String (flexible authentication)
var builder = WebApplication.CreateBuilder(args);
builder.AddAppConfigurationForWebApplication<AppSettings>(
    connectionString: "Endpoint=https://myapp.azconfig.io;Id=xxx;Secret=xxx",
    out AppSettings settings,
    label: "Staging",
    keyFilter: "MyApp:Database:*", // Nested key filter
    skipDevelopment: true
);

// For Azure Functions (direct ConfigurationClient access) - Endpoint + Managed Identity
var hostBuilder = new HostBuilder();
hostBuilder.ConfigureFunctionsWorkerDefaults()
    .AddAppConfigurationForAzureFunctions<AppSettings>(
        appConfigEndpoint: "https://myapp.azconfig.io",
        out AppSettings functionSettings,
        label: "Production",
        skipDevelopment: true
    );

// For Azure Functions - Connection String
var hostBuilder = new HostBuilder();
hostBuilder.ConfigureFunctionsWorkerDefaults()
    .AddAppConfigurationForAzureFunctions<AppSettings>(
        connectionString: "Endpoint=https://myapp.azconfig.io;Id=xxx;Secret=xxx",
        out AppSettings functionSettings,
        label: null, // Use default label
        keyFilter: null, // Get all keys
        skipDevelopment: true
    );

// Direct App Configuration access (async)
AppSettings directSettings = await AppConfigurationExtensions
    .CreateSettingsFromAppConfigurationAsync<AppSettings>(
        "https://myapp.azconfig.io",
        label: "Production",
        keyFilter: "MyApp:*"
    );

// Direct App Configuration access (sync)
AppSettings syncSettings = AppConfigurationExtensions
    .CreateSettingsFromAppConfiguration<AppSettings>(
        "https://myapp.azconfig.io",
        label: "Production"
    );
```

#### App Configuration Key Attribute

Use the `AppConfigurationKeyAttribute` to map properties to different App Configuration key names:

```csharp
public class Settings
{
    // Maps to "DatabaseConnectionString" key
    public string DatabaseConnectionString { get; set; }
    
    // Maps to "custom-api-key" key
    [AppConfigurationKey("custom-api-key")]
    public string ApiKey { get; set; }
    
    // Maps to "MyApp:Email:Url" key
    [AppConfigurationKey("MyApp:Email:Url")]
    public string EmailUrl { get; set; }
}
```

#### Feature Flag Attribute

Use the `FeatureFlagAttribute` to map boolean properties to App Configuration feature flags:

```csharp
public class FeatureSettings
{
    // Maps to feature flag "enable-dark-mode"
    [FeatureFlag("enable-dark-mode")]
    public bool DarkModeEnabled { get; set; }
    
    // Maps to feature flag "experimental-features" (nullable)
    [FeatureFlag("experimental-features")]
    public bool? ExperimentalEnabled { get; set; }
}
```

**Important Notes:**
- Feature flags can ONLY be applied to `bool` or `bool?` properties
- Using `[FeatureFlag]` on non-boolean properties throws `InvalidOperationException`
- Feature flags use a special key format in App Configuration: `.appconfig.featureflag/{flagName}`

#### Configuration Refresh

Configure automatic configuration refresh for dynamic updates:

```csharp
// Create refresh options
var refreshOptions = new AppConfigurationRefreshOptions
{
    EnableRefresh = true,
    RefreshInterval = TimeSpan.FromMinutes(5), // Minimum recommended: 30 seconds
    SentinelKeys = new[] 
    { 
        "AppSettings:Version",  // Common pattern: version key
        "Config:Sentinel",      // Common pattern: sentinel/trigger key
        "App:RefreshKey"        // Common pattern: refresh trigger
    },
    OnRefreshError = null // Silent by default (no-op)
};

// Apply refresh options to web application
builder.AddAppConfigurationForWebApplication<AppSettings>(
    appConfigEndpoint: "https://myapp.azconfig.io",
    out AppSettings settings,
    refreshOptions: refreshOptions
);

// Custom error handler for refresh failures
var refreshWithErrorHandler = new AppConfigurationRefreshOptions
{
    EnableRefresh = true,
    RefreshInterval = TimeSpan.FromMinutes(2),
    SentinelKeys = new[] { "Config:Version" },
    OnRefreshError = ex => 
    {
        // Log error or take custom action
        Console.WriteLine($"Config refresh failed: {ex.Message}");
        // Application continues with cached values
    }
};
```

**Refresh Best Practices:**
- **Minimum Interval**: Use at least 30 seconds to avoid Azure App Configuration throttling
- **Recommended Interval**: 1-5 minutes for most production scenarios
- **Sentinel Keys**: Use dedicated keys that change only when config should refresh
- **Error Handling**: Default is silent (null handler); opt-in to custom logging/alerting
- **Rate Limits**: Be aware of Azure App Configuration service tier limits

#### Labels and Environments

Use labels to maintain environment-specific configurations:

```csharp
// Production environment
builder.AddAppConfigurationForWebApplication<AppSettings>(
    "https://myapp.azconfig.io",
    out var prodSettings,
    label: "Production"  // Reads keys with "Production" label
);

// Staging environment
builder.AddAppConfigurationForWebApplication<AppSettings>(
    "https://myapp.azconfig.io",
    out var stagingSettings,
    label: "Staging"  // Reads keys with "Staging" label
);

// Default label (no label)
builder.AddAppConfigurationForWebApplication<AppSettings>(
    "https://myapp.azconfig.io",
    out var defaultSettings,
    label: null  // Reads keys with no label
);
```

#### Key Filters

Use key filters to load specific subsets of configuration:

```csharp
// Load all keys
builder.AddAppConfigurationForWebApplication<AppSettings>(
    "https://myapp.azconfig.io",
    out var allSettings,
    keyFilter: null  // or "*"
);

// Load keys with specific prefix
builder.AddAppConfigurationForWebApplication<AppSettings>(
    "https://myapp.azconfig.io",
    out var appSettings,
    keyFilter: "MyApp:*"  // Loads MyApp:Database, MyApp:Email, etc.
);

// Load nested keys
builder.AddAppConfigurationForWebApplication<DatabaseSettings>(
    "https://myapp.azconfig.io",
    out var dbSettings,
    keyFilter: "MyApp:Database:*"  // Loads MyApp:Database:ConnectionString, etc.
);
```

#### Authentication Methods

**Managed Identity (Recommended for Azure environments):**
```csharp
// Uses DefaultAzureCredential (Managed Identity, Azure CLI, etc.)
builder.AddAppConfigurationForWebApplication<AppSettings>(
    appConfigEndpoint: "https://myapp.azconfig.io",
    out AppSettings settings,
    credential: new DefaultAzureCredential() // Optional - used by default
);
```

**Connection String (Flexible for all environments):**
```csharp
// Uses connection string with access key
builder.AddAppConfigurationForWebApplication<AppSettings>(
    connectionString: configuration["AppConfigConnectionString"],
    out AppSettings settings
);
```

**Best Practice:** Use Managed Identity (endpoint + credential) in Azure environments for better security. Connection strings are suitable for local development, testing, or non-Azure environments.

#### Multiple Settings Objects with Different Configurations

Load different settings objects with different labels or filters:

```csharp
// Database settings with Production label
builder.AddAppConfigurationForWebApplication<DatabaseSettings>(
    "https://myapp.azconfig.io",
    out var dbSettings,
    label: "Production",
    keyFilter: "MyApp:Database:*"
);

// Feature flags with Staging label
builder.AddAppConfigurationForWebApplication<FeatureSettings>(
    "https://myapp.azconfig.io",
    out var features,
    label: "Staging",
    keyFilter: null
);

// Email settings with default label
builder.AddAppConfigurationForWebApplication<EmailSettings>(
    "https://myapp.azconfig.io",
    out var emailSettings,
    label: null,
    keyFilter: "MyApp:Email:*"
);
```

Each settings object is registered as a singleton in dependency injection and can be injected independently.

#### Important Notes

**Case Sensitivity:**
- Azure App Configuration keys are **case-sensitive**
- `DatabaseConnectionString` ≠ `databaseconnectionstring`
- Ensure exact key name matches in your settings classes

**Key Naming Conventions:**
- Use hierarchical keys with colons: `MyApp:Database:ConnectionString`
- Use hyphens in feature flag names: `enable-new-ui`
- Maintain consistent naming across environments

**When to Use App Configuration vs Key Vault:**
| Use App Configuration | Use Key Vault |
|----------------------|---------------|
| Application settings and configuration | Secrets, certificates, and keys |
| Feature flags | API keys and passwords |
| Environment-specific values | Connection strings with credentials |
| Frequently changing values with refresh | Rarely changing sensitive data |
| Non-sensitive configuration | Highly sensitive data requiring audit logs |

## Enums

### ImageSizePreference

```csharp
using Clear;

ImageSizePreference preference = ImageSizePreference.Width;
// Values: None, Width, Height
```

### Sharers

```csharp
using Clear;

Sharers platform = Sharers.Facebook;
// Values: Facebook, Twitter, Pinterest, Google
```

## Exceptions

### DataDeserializationException

Thrown when JSON deserialization fails.

```csharp
using Clear.Exceptions;

try 
{
    var obj = JsonConvert.DeserializeObject<MyClass>(invalidJson);
}
catch (DataDeserializationException ex)
{
    // Exception includes the target type and the data that failed to deserialize
    Console.WriteLine(ex.Message);
}
```

## Best Practices

### Security
- Always use strong encryption keys (32+ characters) with the Encryption class
- Store API keys and secrets securely, not in source code
- Use HTTPS for all API communications
- Validate all inputs before processing

### Performance
- Use async methods for I/O operations (file, network)
- Dispose of resources properly (images, streams)
- Consider caching for frequently accessed data

### Error Handling
- Wrap API calls in try-catch blocks
- Use the Common.GetAllExceptionMessage() method for detailed error logging
- Validate inputs before calling utility methods

### Thread Safety
- Most utility classes are thread-safe for read operations
- Use separate instances for concurrent operations when modifying state
- Consider using dependency injection for API clients

## Examples

### Complete OTP Implementation

```csharp
public class OtpService
{
    private readonly int _secretKey = 123456;

    public string GenerateOtp(string email)
    {
        var result = OtpUtility.GenerateCode(email, _secretKey, TimeSpan.FromMinutes(5));
        
        // Store result.ExpiryTime in database for validation
        return result.Code;
    }

    public bool ValidateOtp(string email, string code, DateTime storedExpiry)
    {
        return OtpUtility.ValidateCode(email, _secretKey, code, storedExpiry);
    }
}
```

### Image Processing Pipeline

```csharp
public class ImageProcessor
{
    public byte[] ProcessAndOptimize(byte[] imageData, int maxWidth, int maxHeight)
    {
        using var originalStream = new MemoryStream(imageData);
        using var originalImage = Image.FromStream(originalStream);
        
        // Scale maintaining aspect ratio
        using var scaledImage = ImageUtility.ScaleImage(originalImage, maxWidth, maxHeight);
        
        // Convert to optimized JPEG
        using var outputStream = new MemoryStream();
        ImageUtility.SaveJpegToStream(outputStream, scaledImage, 85);
        
        return outputStream.ToArray();
    }
}
```

### Complete API Service

```csharp
public class UserService
{
    private readonly IApiClient _apiClient;
    private readonly string _baseUrl = "https://api.example.com";
    private readonly string _apiKey;

    public UserService(IApiClient apiClient, IConfiguration config)
    {
        _apiClient = apiClient;
        _apiKey = config["ApiKey"];
    }

    public async Task<User> GetUserAsync(int userId)
    {
        var headers = new Dictionary<string, string> { ["X-API-Key"] = _apiKey };
        return await _apiClient.GetAsync<User>($"{_baseUrl}/users/{userId}", headers);
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var headers = new Dictionary<string, string> { ["X-API-Key"] = _apiKey };
        return await _apiClient.PostAsync<CreateUserRequest, User>($"{_baseUrl}/users", request, headers);
    }
}
```

This documentation covers all public APIs and provides practical examples for integrating ClearTools into your applications. The library is designed to be intuitive for both human developers and AI code generators, with consistent naming conventions and comprehensive functionality.

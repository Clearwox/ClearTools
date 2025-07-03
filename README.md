# ClearTools

A comprehensive .NET Standard 2.1 utility library providing robust, production-ready tools for common development tasks. ClearTools offers utilities for string manipulation, cryptography, image processing, Azure services integration, HTTP client operations, and much more.

[![NuGet Version](https://img.shields.io/nuget/v/ClearTools)](https://www.nuget.org/packages/ClearTools/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 🚀 Quick Start

### Installation

```bash
dotnet add package ClearTools
```

### Basic Usage

```csharp
using ClearTools.Extensions;
using Clear.Tools;

// String extensions
string text = "Hello123World!";
int number = text.ToInt32(); // Extracts numbers: 123
string cleaned = text.StripSymbols(); // Removes symbols: "Hello123World"

// Image processing
var scaledImage = ImageUtility.ScaleImage(originalImage, 800, 600);

// Cryptography
string encrypted = Crypto.Encrypt("sensitive data", "password");
string hash = Crypto.Hash("data to hash");

// OTP generation
var otpResult = OtpUtility.GenerateCode("user@example.com", 12345, TimeSpan.FromMinutes(5));
```

## 📋 Table of Contents

- [Core Utilities](#-core-utilities)
- [Extension Methods](#-extension-methods)
- [Azure Integration](#-azure-integration)
- [HTTP Client](#-http-client)
- [Middleware](#-middleware)
- [Data Models](#-data-models)
- [Installation & Setup](#-installation--setup)
- [Examples](#-examples)
- [Contributing](#-contributing)
- [License](#-license)

## 🔧 Core Utilities

### String Utilities (`StringUtility`)

Comprehensive string manipulation and processing tools:

```csharp
// URL and SEO-friendly string generation
string urlKey = StringUtility.GenerateUrlKey("My Blog Post Title!"); // "my-blog-post-title"

// HTML and symbol stripping
string cleanText = StringUtility.StripHTML("<p>Hello <b>World</b></p>"); // "Hello World"
string alphanumeric = StringUtility.StripSymbols("Hello@World!123"); // "HelloWorld123"

// Date-based unique identifiers
string dateCode = StringUtility.GetDateCode(); // File time-based code
string updateId = StringUtility.AddUpDate(); // Timestamp-based ID

// Text processing
string truncated = StringUtility.TruncateString("Long text here", 10); // "Long text..."
string substring = StringUtility.GetSubstring("Hello World", 6, 5); // "World"

// Tag generation
string tags = StringUtility.GenerateTags("tag1", "tag2", "tag3"); // "tag1,tag2,tag3"
```

### Image Processing (`ImageUtility`)

Advanced image manipulation capabilities:

```csharp
// Image scaling with aspect ratio preservation
Image scaledImage = ImageUtility.ScaleImage(sourceImage, 800, 600, ImageSizePreference.Width);

// Image cropping
Image croppedImage = ImageUtility.CropImage(sourceBitmap, 300, 300);

// Image resizing
Bitmap resizedImage = ImageUtility.ResizeImage(sourceImage, 400, 300);

// Format conversion
byte[] imageBytes = ImageUtility.ConvertBitmapToBytes(bitmap, ImageFormat.Jpeg);
string base64Image = ImageUtility.ConvertImageToBase64(image, ImageFormat.Png);
Image imageFromBase64 = ImageUtility.ConvertBase64ToImage(base64String);

// High-quality JPEG saving
ImageUtility.SaveJpegToFile("output.jpg", image, quality: 85);
```

### Cryptography & Security

#### Encryption (`Encryption`)
```csharp
// Simple encryption/decryption
string encrypted = Encryption.Encrypt("sensitive data", "password");
string decrypted = Encryption.Decrypt(encrypted, "password");
```

#### Advanced Cryptography (`Crypto`)
```csharp
// Hashing
string hash = Crypto.Hash("data to hash");
string hashWithSalt = Crypto.Hash("data", "salt");

// Encryption with salt
string encrypted = Crypto.Encrypt("data", "password");
string decrypted = Crypto.Decrypt(encrypted, "password");

// Secure random number generation
int randomNumber = Crypto.GenerateRandomNumber(1, 100);
```

#### OTP (One-Time Password) Utilities (`OtpUtility`)
```csharp
// Generate OTP with expiry
var otpResult = OtpUtility.GenerateCode("user@example.com", 12345, TimeSpan.FromMinutes(5));
Console.WriteLine($"Code: {otpResult.Code}, Expires: {otpResult.ExpiryTime}");

// Generate simple OTP (24-hour expiry)
var simpleOtp = OtpUtility.GenerateCode("user@example.com", 12345);

// Validate OTP
bool isValid = OtpUtility.ValidateCode("user@example.com", 12345, "123456", otpResult.ExpiryTime);
```

### Number Base Conversion (`BaseConverter`)

Convert numbers between different bases and formats:

```csharp
// Convert from any base to decimal
long decimal = BaseConverter.ConvertToDecimal("1010", 2); // Binary to decimal: 10
long hexDecimal = BaseConverter.ConvertToDecimal("FF", 16); // Hex to decimal: 255

// Convert decimal to any base
string binary = BaseConverter.ConvertFromDecimal(10, 2); // "1010"
string hex = BaseConverter.ConvertFromDecimal(255, 16); // "FF"

// Convert to alphabetic representation
string alpha = BaseConverter.ConvertToAlpha(26); // "Z"
```

### Date Utilities (`DateUtility`)

Date and time helper functions:

```csharp
// Check if dates are in the same week
bool sameWeek = DateUtility.IsSameWeek(date1, date2, DayOfWeek.Monday);
```

### Common Utilities (`Common`)

General-purpose helper methods:

```csharp
// Exception message extraction
string allMessages = Common.GetAllExceptionMessage(exception);

// Social media sharing links
string facebookLink = Common.GetShareLink("https://example.com", Sharers.Facebook, "Description", "image.jpg");
string twitterLink = Common.GetShareLink("https://example.com", Sharers.Twitter, "Check this out", "");
```

### EditorJS Integration (`EditorJS`)

Parse and convert EditorJS content:

```csharp
// Parse EditorJS JSON to HTML
string html = EditorJS.Parse(editorJsJsonString);

// Parse structured content
Content content = JsonConvert.DeserializeObject<Content>(jsonString);
string html = EditorJS.Parse(content);
```

### File Management (`FileManager`)

File system operations and utilities:

```csharp
// Various file operations available
// (Implementation details available in FileManager class)
```

## 🔌 Extension Methods

### String Extensions (`StringExtensions`)

Powerful string manipulation extensions:

```csharp
// Value toggling
string toggled = "active".Toggle("inactive"); // "inactive"
string empty = "".Toggle("default"); // "default"

// Case-insensitive operations
bool contains = "Hello World".Search("WORLD"); // true
bool equals = "Hello".EqualsNoCase("HELLO"); // true

// Symbol and number extraction
string numbers = "abc123def456".ExtractNumbers(); // "123456"
string clean = "Hello@World!".StripSymbols(); // "HelloWorld"

// Type conversions
int number = "abc123".ToInt32(); // 123
decimal price = "Price: $29.99".ToDecimal(); // 29.99
double value = "Value: 3.14159".ToDouble(); // 3.14159
DateTime date = "2023-12-25".ToDateTime();

// Base64 operations
byte[] bytes = base64String.FromBase64String();

// CSV processing
List<string> items = "apple,banana,cherry".ToListFromCsv();
HashSet<string> uniqueItems = "apple,banana,apple".ToHashSetFromCsv();
```

### DateTime Extensions (`DateTimeExtensions`)

Enhanced DateTime formatting:

```csharp
DateTime now = DateTime.Now;

// Formatted date strings
string dateStr = now.ToDateString(); // "25/Dec/2023"
string dateTimeStr = now.ToDateTimeString(); // "25/Dec/2023 14:30:15"
```

### Byte Extensions (`ByteExtensions`)

Byte array utilities:

```csharp
byte[] data = GetSomeBytes();
string base64 = data.ToBase64String();
```

## ☁️ Azure Integration

### Key Vault Integration (`KeyVaultExtensions`)

Robust Azure Key Vault integration for both ASP.NET Core applications and Azure Functions:

#### ASP.NET Core Web Applications

```csharp
// Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Basic Key Vault integration
    services.AddKeyVaultForWebApplication(configuration);
    
    // With custom configuration
    services.AddKeyVaultForWebApplication(
        configuration,
        keyVaultUrl: "https://your-keyvault.vault.azure.net/",
        credential: new DefaultAzureCredential()
    );
}

// Access secrets through IConfiguration
public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    
    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public IActionResult Index()
    {
        string secret = _configuration["MySecret"];
        return View();
    }
}
```

#### Azure Functions

```csharp
// Function startup
[assembly: FunctionsStartup(typeof(Startup))]
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Basic setup
        builder.Services.AddKeyVaultForAzureFunctions();
        
        // With custom configuration
        builder.Services.AddKeyVaultForAzureFunctions(
            keyVaultUrl: "https://your-keyvault.vault.azure.net/",
            credential: new ManagedIdentityCredential()
        );
    }
}

// In your functions
public class MyFunction
{
    private readonly SecretClient _secretClient;
    
    public MyFunction(SecretClient secretClient)
    {
        _secretClient = secretClient;
    }
    
    [FunctionName("GetSecret")]
    public async Task<IActionResult> Run([HttpTrigger] HttpRequest req)
    {
        string secretValue = await _secretClient.GetSecretValueAsync("MySecret");
        return new OkObjectResult(secretValue);
    }
}
```

#### Direct Key Vault Operations

```csharp
// Get secret synchronously
string secret = KeyVaultExtensions.GetSecretValue(secretClient, "MySecretName");

// Get secret asynchronously
string secret = await KeyVaultExtensions.GetSecretValueAsync(secretClient, "MySecretName");

// With custom credential
var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
string secret = await KeyVaultExtensions.GetSecretValueAsync(
    secretClient, 
    "MySecretName", 
    credential
);
```

### Azure Storage (`AzureStorageManager`)

Azure Blob Storage operations:

```csharp
// Check if folder exists
bool exists = AzureStorageManager.AzureFolderExists(connectionString, "container", "folder/path");

// Upload from stream
AzureStorageManager.UploadToAzure(connectionString, "container", stream, "path/file.txt");

// Upload from memory stream
AzureStorageManager.UploadToAzure(connectionString, "container", memoryStream, "path/file.txt");
```

### Service Collection Extensions (`ServiceCollectionExtensions`)

Dependency injection helpers for Azure services.

## 🌐 HTTP Client

### API Client (`ApiClient`)

Comprehensive HTTP client with built-in error handling, token management, and serialization:

```csharp
// Initialize
var httpClient = new HttpClient();
var apiClient = new ApiClient(httpClient);

// GET requests
var user = await apiClient.GetAsync<User>("https://api.example.com/users/1");
var userWithAuth = await apiClient.GetAsync<User>("https://api.example.com/users/1", bearerToken);

// POST requests
var response = await apiClient.PostAsync("https://api.example.com/users", newUser);
var createdUser = await apiClient.PostAsync<User, User>("https://api.example.com/users", newUser);

// PUT requests
var updateResponse = await apiClient.PutAsync("https://api.example.com/users/1", updatedUser);
var updatedUser = await apiClient.PutAsync<User, User>("https://api.example.com/users/1", updatedUser);

// DELETE requests
var deleteResponse = await apiClient.DeleteAsync("https://api.example.com/users/1");

// With custom headers
var headers = new Dictionary<string, string> { { "X-Custom-Header", "value" } };
var result = await apiClient.GetAsync<User>("https://api.example.com/users/1", bearerToken, headers);
```

Features:
- Automatic JSON serialization/deserialization
- Built-in authentication token handling
- Custom header support
- Error handling and response validation
- Support for all HTTP methods (GET, POST, PUT, DELETE)
- Cancellation token support
- Generic type support for strong typing

## 🛡️ Middleware

### Request Validation Middleware (`RequestValidationMiddleware`)

ASP.NET Core middleware for request validation and processing:

```csharp
// In Startup.cs or Program.cs
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseMiddleware<RequestValidationMiddleware>();
    // ... other middleware
}
```

## 📊 Data Models

### Validation Models
- **`ValidationCodeResult`**: OTP validation results with code and expiry information
- **`CaptcherResponse`**: CAPTCHA validation response model

### EditorJS Models (`editorjs.cs`)
Complete EditorJS data structure support:
- **`Content`**: Main content container
- **`Block`**: Individual content blocks
- **`BlockData`**: Block-specific data structures
- Support for paragraphs, headers, lists, images, and more

## 📦 Installation & Setup

### Prerequisites
- .NET Standard 2.1 or higher
- For Azure features: Azure account and appropriate service configurations

### Package Installation

```bash
# Via .NET CLI
dotnet add package ClearTools

# Via Package Manager Console
Install-Package ClearTools

# Via PackageReference
<PackageReference Include="ClearTools" Version="3.0.7" />
```

### Configuration

#### For Azure Key Vault (ASP.NET Core)
```json
{
  "AzureKeyVault": {
    "Url": "https://your-keyvault.vault.azure.net/"
  }
}
```

#### For Azure Storage
```json
{
  "ConnectionStrings": {
    "AzureStorage": "your-azure-storage-connection-string"
  }
}
```

## 📖 Examples

### Complete Web Application Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add ClearTools services
builder.Services.AddKeyVaultForWebApplication(builder.Configuration);
builder.Services.AddScoped<ApiClient>();

var app = builder.Build();

// Add ClearTools middleware
app.UseMiddleware<RequestValidationMiddleware>();

app.Run();
```

### Azure Function with Key Vault

```csharp
[assembly: FunctionsStartup(typeof(Startup))]
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddKeyVaultForAzureFunctions();
    }
}

public class MyFunction
{
    private readonly SecretClient _secretClient;
    
    public MyFunction(SecretClient secretClient)
    {
        _secretClient = secretClient;
    }
    
    [FunctionName("ProcessData")]
    public async Task<IActionResult> Run([HttpTrigger] HttpRequest req)
    {
        // Get database connection from Key Vault
        string connectionString = await _secretClient.GetSecretValueAsync("DatabaseConnection");
        
        // Process request data
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        string processedData = requestBody.StripHTML().StripSymbols();
        
        return new OkObjectResult(new { processed = processedData });
    }
}
```

### Image Processing Pipeline

```csharp
public class ImageProcessor
{
    public async Task<string> ProcessAndUploadImage(IFormFile file)
    {
        // Convert uploaded file to image
        using var stream = file.OpenReadStream();
        var image = Image.FromStream(stream);
        
        // Process image
        var scaledImage = ImageUtility.ScaleImage(image, 800, 600, ImageSizePreference.Width);
        var imageBytes = ImageUtility.ConvertBitmapToBytes((Bitmap)scaledImage, ImageFormat.Jpeg);
        
        // Upload to Azure Storage
        using var uploadStream = new MemoryStream(imageBytes);
        var fileName = $"processed_{StringUtility.GetDateCode()}.jpg";
        
        AzureStorageManager.UploadToAzure(connectionString, "images", uploadStream, fileName);
        
        return fileName;
    }
}
```

## 🏗️ Advanced Features

### Smart Enums (`SmartEnum`)
Type-safe enumeration base class for creating intelligent enum types with additional behavior.

### Custom Exceptions (`Exceptions`)
Specialized exception types for better error handling and debugging.

### Captcha Integration
Built-in support for CAPTCHA validation with response models.

## 🔍 Testing

ClearTools includes comprehensive unit tests covering all major functionality:

```bash
# Run tests
dotnet test ClearTools.Tests
```

Test coverage includes:
- String manipulation and extensions
- Cryptography operations
- Image processing
- OTP generation and validation
- API client operations
- Date utilities
- Base conversion

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup
1. Clone the repository
2. Install .NET SDK 6.0 or higher
3. Run `dotnet restore`
4. Run `dotnet build`
5. Run tests: `dotnet test`

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

- **Documentation**: [GitHub Wiki](https://github.com/Clearwox/ClearTools/wiki)
- **Issues**: [GitHub Issues](https://github.com/Clearwox/ClearTools/issues)
- **NuGet Package**: [ClearTools on NuGet](https://www.nuget.org/packages/ClearTools/)

## 🏷️ Tags

`csharp` `dotnet` `utility-library` `azure` `keyvault` `image-processing` `cryptography` `http-client` `string-utilities` `extensions` `netstandard21`

---

**ClearTools** - Making .NET development clearer, one utility at a time. 🚀

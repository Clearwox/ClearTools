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
string text = "Hello 123 World!";
int number = text.ToInt32(); // Extracts numbers: 123
string cleaned = StringUtility.StripSymbols(text); // Removes symbols, preserves spaces: "Hello 123 World"

// Image processing
var scaledImage = ImageUtility.ScaleImage(originalImage, 800, 600);

// Cryptography
string salt = Crypto.CreateSalt();
string hash = Crypto.EncodeSHA256("password", salt);

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
- [Documentation](#-documentation)
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
string alphanumeric = StringUtility.StripSymbols("Hello @World! 123"); // "Hello World 123"

// Date-based unique identifiers
string dateCode = StringUtility.GetDateCode(); // File time-based code
string updateId = StringUtility.AddUpDate(); // Timestamp-based ID

// Tag generation
string tags = StringUtility.GenerateTags("tag1", "tag2", "tag3"); // "tag1,tag2,tag3"
```

### Image Processing (`ImageUtility`)

Advanced image manipulation capabilities:

```csharp
// Image scaling with aspect ratio preservation
Image scaledImage = ImageUtility.ScaleImage(sourceImage, 800, 600, ImageSizePreference.Width);

// Image cropping and resizing
Image croppedImage = ImageUtility.CropImage(sourceBitmap, 300, 300);
Bitmap resizedImage = ImageUtility.ResizeImage(sourceImage, 400, 300);

// Format conversion
byte[] imageBytes = ImageUtility.ConvertBitmapToBytes(bitmap, ImageFormat.Jpeg);
string base64Image = ImageUtility.ConvertImageToBase64(image, ImageFormat.Png);

// High-quality JPEG saving
ImageUtility.SaveJpegToFile("output.jpg", image, quality: 85);
```

### Cryptography & Security

#### Encryption (`Encryption`)
```csharp
string key = "MySecretEncryptionKeyThatIsLongEnough123"; // 32+ chars required
string encrypted = Encryption.Encrypt("sensitive data", key);
string decrypted = Encryption.Decrypt(encrypted, key);
```

#### Cryptography (`Crypto`)
```csharp
// Hashing algorithms
string salt = Crypto.CreateSalt(128);
string sha256Hash = Crypto.EncodeSHA256("password", salt);
string sha512Hash = Crypto.EncodeSHA512("password", salt);
string sha1Hash = Crypto.EncodeSHA1("password");

// Base64 encoding/decoding
string encoded = Crypto.EncodeBase64("text to encode");
string decoded = Crypto.DecodeBase64(encoded);
```

#### OTP (One-Time Password) Utilities (`OtpUtility`)
```csharp
// Generate OTP with custom expiry
var otpResult = OtpUtility.GenerateCode("user@example.com", 12345, TimeSpan.FromMinutes(5));
Console.WriteLine($"Code: {otpResult.Code}, Expires: {otpResult.ExpiryTime}");

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
string alpha = BaseConverter.ConvertToAlpha(26); // "BA"
```

### EditorJS Integration (`EditorJS`)

Parse and convert EditorJS content to HTML:

```csharp
// Parse EditorJS JSON to HTML
string html = EditorJS.Parse(editorJsJsonString);

// Supports: headers, paragraphs, lists, images, embeds (YouTube, Vimeo)
```

## 🔌 Extension Methods

### String Extensions (`StringExtensions`)

Powerful string manipulation extensions:

```csharp
// Value toggling
string toggled = "active".Toggle("inactive"); // "inactive"

// Case-insensitive operations  
bool contains = "Hello World".Search("WORLD"); // true
bool equals = "Hello".EqualsNoCase("HELLO"); // true

// Number and symbol extraction
string numbers = "abc123def456".ExtractNumbers(); // "123456"
string clean = "Hello;World*".StripSymbols(); // "HelloWorld" (removes specific symbols)

// Type conversions
int number = "abc123".ToInt32(); // 123
decimal price = "Price: $29.99".ToDecimal(); // 29.99

// CSV processing
List<string> items = "apple,banana,cherry".ToListFromCsv();
HashSet<string> uniqueItems = "apple,banana,apple".ToHashSetFromCsv();
```

### DateTime Extensions (`DateTimeExtensions`)

Enhanced DateTime formatting:

```csharp
DateTime now = DateTime.Now;
string dateStr = now.ToDateString(); // "15/Aug/2025"
string dateTimeStr = now.ToDateTimeString(); // "15/Aug/2025 14:30:15"
```

## ☁️ Azure Integration

### Key Vault Integration (`KeyVaultExtensions`)

Robust Azure Key Vault integration for configuration management:

```csharp
// Define your settings class
public class AppSettings
{
    public string DatabaseConnectionString { get; set; }
    
    [KeyVaultKey("api-key")]
    public string ApiKey { get; set; }
}

// For ASP.NET Web Applications
var builder = WebApplication.CreateBuilder(args);
builder.AddKeyVaultForWebApplication<AppSettings>(
    keyVaultUri: "https://your-keyvault.vault.azure.net/",
    out AppSettings settings
);

// For Azure Functions
hostBuilder.AddKeyVaultForAzureFunctions<AppSettings>(
    keyVaultUri: "https://your-keyvault.vault.azure.net/",
    out AppSettings functionSettings
);
```

### Azure Storage (`AzureStorageManager`)

Azure Blob Storage operations:

```csharp
string connectionString = "DefaultEndpointsProtocol=https;AccountName=...";

// Upload operations
AzureStorageManager.UploadToAzure(connectionString, "container", stream, "application/pdf", "file.pdf", "folder");

// Download operations  
AzureStorageManager.DownloadFromAzure(connectionString, "container", fileInfo, "folder");

// Check folder existence
bool exists = AzureStorageManager.AzureFolderExists(connectionString, "container", "folder");
```

### Service Collection Extensions (`ServicesExtensions`)

Automatic service registration by interface:

```csharp
// Register all services implementing IService
services.AddScopedServicesByInterface<IService>();
services.AddSingletonServicesByInterface<IRepository>();
services.AddTransientServicesByInterface<IValidator>();
```

## 🌐 HTTP Client

### API Client (`ApiClient`)

Comprehensive HTTP client with built-in error handling and serialization:

```csharp
// Initialize
var apiClient = new ApiClient(httpClient);

// GET with various options
var user = await apiClient.GetAsync<User>("https://api.example.com/users/1");
var userWithAuth = await apiClient.GetAsync<User>("https://api.example.com/users/1", bearerToken);

// POST operations
var response = await apiClient.PostAsync("https://api.example.com/users", newUser);
var createdUser = await apiClient.PostAsync<User, User>("https://api.example.com/users", newUser);

// reCAPTCHA validation
var captchaResult = await apiClient.ValidateGoogleCaptcharAsync(secretKey, response, remoteIp);
```

## 🛡️ Middleware

### Request Validation Middleware (`RequestValidationMiddleware`)

ASP.NET Core middleware for API key validation:

```csharp
// Setup
services.AddRequestValidation("your_secret_api_key", skipForDevelopment: true);
app.UseRequestValidation();

// Client usage - include key in headers
client.DefaultRequestHeaders.Add("key", "your_secret_api_key");
```

## 📊 Data Models

### Smart Enums (`SmartEnum<TEnum>`)

Type-safe enumeration base class with value and name support:

```csharp
// Define your SmartEnum
public class OrderStatus : SmartEnum<OrderStatus>
{
    public static readonly OrderStatus Pending = new OrderStatus("Pending", 1);
    public static readonly OrderStatus Processing = new OrderStatus("Processing", 2);
    public static readonly OrderStatus Shipped = new OrderStatus("Shipped", 3);
    public static readonly OrderStatus Delivered = new OrderStatus("Delivered", 4);
    
    private OrderStatus(string name, int value) : base(name, value) { }
}

// Usage examples
var status = OrderStatus.Parse(2); // Get by value: Processing
var statusByName = OrderStatus.Parse("Shipped"); // Get by name: Shipped

// List all values
foreach (var status in OrderStatus.List())
{
    Console.WriteLine($"{status.Name}: {status.Value}");
}

// Type-safe comparisons
OrderStatus current = OrderStatus.Processing;
if (current.Equals(OrderStatus.Processing))
{
    Console.WriteLine("Order is being processed");
}
```

**Features:**
- Type-safe enumeration pattern
- Parse by value or name (case-insensitive)
- List all enum values
- Strongly-typed with compile-time safety
- Thread-safe initialization

- **`ValidationCodeResult`**: OTP generation results with code and expiry
- **`CaptcherResponse`**: Google reCAPTCHA validation response
- **EditorJS Models**: Complete data structures for EditorJS content parsing

## 📦 Installation & Setup

### Package Installation

```bash
# Via .NET CLI
dotnet add package ClearTools

# Via PackageReference  
<PackageReference Include="ClearTools" Version="3.1.0" />
```

### Dependencies

ClearTools targets .NET Standard 2.1 and includes:
- Azure.Storage.Blobs (12.25.0)
- Azure.Security.KeyVault.Secrets (4.8.0)
- Newtonsoft.Json (13.0.3)
- System.Drawing.Common (9.0.8)
- Microsoft.AspNetCore.Http.Abstractions (2.3.0)

## 📖 Examples

### Complete Image Processing Pipeline

```csharp
public class ImageProcessor
{
    public async Task<string> ProcessAndUploadImage(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var image = Image.FromStream(stream);
        
        // Process image
        var scaledImage = ImageUtility.ScaleImage(image, 800, 600);
        var imageBytes = ImageUtility.ConvertBitmapToBytes((Bitmap)scaledImage, ImageFormat.Jpeg);
        
        // Generate unique filename
        var fileName = StringUtility.GenerateFileName(file.FileName, "jpg");
        
        // Upload to Azure
        using var uploadStream = new MemoryStream(imageBytes);
        AzureStorageManager.UploadToAzure(connectionString, "images", uploadStream, "image/jpeg", fileName, "uploads");
        
        return fileName;
    }
}
```

### OTP Service Implementation

```csharp
public class OtpService
{
    private readonly int _secretKey = 123456;

    public ValidationCodeResult GenerateOtp(string email)
    {
        return OtpUtility.GenerateCode(email, _secretKey, TimeSpan.FromMinutes(5));
    }

    public bool ValidateOtp(string email, string code, DateTime expiry)
    {
        return OtpUtility.ValidateCode(email, _secretKey, code, expiry);
    }
}
```

## 📚 Documentation

For comprehensive documentation covering all methods and use cases, see:

- **[Complete Usage Guide](docs/USAGE.md)** - Detailed documentation with examples for every class and method
- **[API Reference](https://github.com/Clearwox/ClearTools/wiki)** - Complete API documentation
- **[Changelog](CHANGELOG.md)** - Version history and changes

## 🏷️ Features Summary

✅ **String Utilities**: URL generation, HTML stripping, text processing  
✅ **Image Processing**: Scaling, cropping, format conversion, quality optimization  
✅ **Cryptography**: Hashing, encryption, salt generation, secure tokens  
✅ **OTP Management**: Generation, validation, expiry handling  
✅ **Azure Integration**: Key Vault, Blob Storage, managed identity support  
✅ **HTTP Client**: RESTful API client with authentication and serialization  
✅ **Extensions**: String, DateTime, Byte array, and service collection extensions  
✅ **Middleware**: Request validation, API key authentication  
✅ **EditorJS**: Content parsing and HTML conversion  
✅ **Base Conversion**: Number base conversion utilities  
✅ **File Management**: File I/O operations and utilities  

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/Clearwox/ClearTools/issues)
- **NuGet Package**: [ClearTools on NuGet](https://www.nuget.org/packages/ClearTools/)
- **Documentation**: [Complete Usage Guide](docs/USAGE.md)

---

**ClearTools** - Making .NET development clearer, one utility at a time. 🚀

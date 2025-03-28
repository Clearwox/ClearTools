using Clear.Tools.Models;

namespace ClearTools.Tests;

public class OtpUtilityTests
{
    [Fact]
    public void GenerateCode_ValidParameters_ReturnsValidCode()
    {
        // Arrange
        string text = "test";
        int secretKey = 123456;
        TimeSpan expiryDuration = TimeSpan.FromMinutes(5);
        int codeLength = 6;

        // Act
        ValidationCodeResult result = OtpUtility.GenerateCode(text, secretKey, expiryDuration, codeLength);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(codeLength, result.Code.Length);
        Assert.True(result.ExpiryTime > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateCode_ValidParametersWithoutExpiryDuration_ReturnsValidCode()
    {
        // Arrange
        string text = "test";
        int secretKey = 123456;
        int codeLength = 6;

        // Act
        ValidationCodeResult result = OtpUtility.GenerateCode(text, secretKey, codeLength);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(codeLength, result.Code.Length);
        Assert.True(result.ExpiryTime > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateCode_InvalidCodeLength_ThrowsArgumentException()
    {
        // Arrange
        string text = "test";
        int secretKey = 123456;
        TimeSpan expiryDuration = TimeSpan.FromMinutes(5);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => OtpUtility.GenerateCode(text, secretKey, expiryDuration, 0));
        Assert.Throws<ArgumentException>(() => OtpUtility.GenerateCode(text, secretKey, expiryDuration, 11));
        Assert.Throws<ArgumentException>(() => OtpUtility.GenerateCode(text, secretKey, 0));
        Assert.Throws<ArgumentException>(() => OtpUtility.GenerateCode(text, secretKey, 11));
    }

    [Fact]
    public void ValidateCode_ValidCode_ReturnsTrue()
    {
        // Arrange
        string text = "test";
        int secretKey = 123456;
        TimeSpan expiryDuration = TimeSpan.FromMinutes(5);
        ValidationCodeResult result = OtpUtility.GenerateCode(text, secretKey, expiryDuration);
        string code = result.Code;
        DateTime expiryTime = result.ExpiryTime;

        // Act
        bool isValid = OtpUtility.ValidateCode(text, secretKey, code, expiryTime);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateCode_ValidCodeWithoutExpiryTime_ReturnsTrue()
    {
        // Arrange
        string text = "test";
        int secretKey = 123456;
        ValidationCodeResult result = OtpUtility.GenerateCode(text, secretKey);
        string code = result.Code;

        // Act
        bool isValid = OtpUtility.ValidateCode(text, secretKey, code);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateCode_InvalidCode_ReturnsFalse()
    {
        // Arrange
        string text = "test";
        int secretKey = 123456;
        TimeSpan expiryDuration = TimeSpan.FromMinutes(5);
        ValidationCodeResult result = OtpUtility.GenerateCode(text, secretKey, expiryDuration);
        string invalidCode = "000000";
        DateTime expiryTime = result.ExpiryTime;

        // Act
        bool isValid = OtpUtility.ValidateCode(text, secretKey, invalidCode, expiryTime);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateCode_InvalidCodeWithoutExpiryTime_ReturnsFalse()
    {
        // Arrange
        string text = "test";
        int secretKey = 123456;
        ValidationCodeResult result = OtpUtility.GenerateCode(text, secretKey);
        string invalidCode = "000000";

        // Act
        bool isValid = OtpUtility.ValidateCode(text, secretKey, invalidCode);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateCode_ExpiredCode_ReturnsFalse()
    {
        // Arrange
        string text = "test";
        int secretKey = 123456;
        TimeSpan expiryDuration = TimeSpan.FromMinutes(-5); // Expired duration
        ValidationCodeResult result = OtpUtility.GenerateCode(text, secretKey, expiryDuration);
        string code = result.Code;
        DateTime expiryTime = result.ExpiryTime;

        // Act
        bool isValid = OtpUtility.ValidateCode(text, secretKey, code, expiryTime);

        // Assert
        Assert.False(isValid);
    }
}

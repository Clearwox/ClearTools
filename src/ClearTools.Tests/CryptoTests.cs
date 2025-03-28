namespace ClearTools.Tests;

public class CryptoTests
{
    [Fact]
    public void CreateSalt_ValidSize_ReturnsSalt()
    {
        // Arrange
        int size = 128;

        // Act
        string salt = Crypto.CreateSalt(size);

        // Assert
        Assert.NotNull(salt);
        Assert.Equal(size, Convert.FromBase64String(salt).Length);
    }

    [Fact]
    public void EncodeSHA512_ValidInput_ReturnsHash()
    {
        // Arrange
        string input = "test";
        string salt = "salt";

        // Act
        string hash = Crypto.EncodeSHA512(input, salt);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(88, hash.Length); // SHA-512 hash length in Base64
    }

    [Fact]
    public void EncodeSHA256_ValidInput_ReturnsHash()
    {
        // Arrange
        string input = "test";
        string salt = "salt";

        // Act
        string hash = Crypto.EncodeSHA256(input, salt);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(44, hash.Length); // SHA-256 hash length in Base64
    }

    [Fact]
    public void EncodeSHA1_ValidInput_ReturnsHash()
    {
        // Arrange
        string input = "test";

        // Act
        string hash = Crypto.EncodeSHA1(input);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(28, hash.Length); // SHA-1 hash length in Base64
    }

    [Fact]
    public void DecodeSHA1_ValidInput_ReturnsOriginalString()
    {
        // Arrange
        string input = "test";
        string encoded = Crypto.EncodeBase64(input);

        // Act
        string decoded = Crypto.DecodeBase64(encoded);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void DecodeSHA1_InvalidBase64Input_ThrowsFormatException()
    {
        // Arrange
        string invalidBase64 = "invalid_base64";

        // Act & Assert
        Assert.Throws<FormatException>(() => Crypto.DecodeBase64(invalidBase64));
    }
}

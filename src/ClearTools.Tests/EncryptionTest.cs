namespace ClearTools.Tests;

public class EncryptionTests
{
    private const string TestKey = "ThisIsASecretKeyThatIsLongEnoughToBeValidThisIsASecretKeyThatIsLongEnoughToBeValidrluhriuklrnjlriulhi^^893h";
    private const string TestText = "Hello, World!";

    [Fact]
    public void Encrypt_ValidInput_ReturnsEncryptedString()
    {
        var encryptedText = Encryption.Encrypt(TestText, TestKey);
        Assert.False(string.IsNullOrEmpty(encryptedText));
    }

    [Fact]
    public void Decrypt_ValidInput_ReturnsOriginalString()
    {
        var encryptedText = Encryption.Encrypt(TestText, TestKey);
        var decryptedText = Encryption.Decrypt(encryptedText, TestKey);
        Assert.Equal(TestText, decryptedText);
    }

    [Fact]
    public void Encrypt_InvalidKey_ThrowsArgumentException()
    {
        var shortKey = "short";
        Assert.Throws<ArgumentException>(() => Encryption.Encrypt(TestText, shortKey));
    }

    [Fact]
    public void Decrypt_InvalidKey_ThrowsArgumentException()
    {
        var encryptedText = Encryption.Encrypt(TestText, TestKey);
        var shortKey = "short";
        Assert.Throws<ArgumentException>(() => Encryption.Decrypt(encryptedText, shortKey));
    }
}
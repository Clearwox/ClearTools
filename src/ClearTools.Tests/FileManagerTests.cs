namespace ClearTools.Tests;

public class FileManagerTests
{
    private const string TestFileName = "testfile.txt";

    [Fact]
    public void Write_ValidInput_WritesToFile()
    {
        // Arrange
        string text = "Hello, World!";

        // Act
        FileManager.Write(TestFileName, text);

        // Assert
        string result = File.ReadAllText(TestFileName);
        Assert.Equal(text, result);

        // Cleanup
        File.Delete(TestFileName);
    }

    [Fact]
    public void Read_ValidFile_ReadsFromFile()
    {
        // Arrange
        string text = "Hello, World!";
        File.WriteAllText(TestFileName, text);

        // Act
        string result = FileManager.Read(TestFileName);

        // Assert
        Assert.Equal(text, result);

        // Cleanup
        File.Delete(TestFileName);
    }

    [Fact]
    public async Task WriteAsync_ValidInput_WritesToFile()
    {
        // Arrange
        string text = "Hello, Async World!";

        // Act
        await FileManager.WriteAsync(TestFileName, text);

        // Assert
        string result = await File.ReadAllTextAsync(TestFileName);
        Assert.Equal(text, result);

        // Cleanup
        File.Delete(TestFileName);
    }

    [Fact]
    public async Task ReadAsync_ValidFile_ReadsFromFile()
    {
        // Arrange
        string text = "Hello, Async World!";
        await File.WriteAllTextAsync(TestFileName, text);

        // Act
        string result = await FileManager.ReadAsync(TestFileName);

        // Assert
        Assert.Equal(text, result);

        // Cleanup
        File.Delete(TestFileName);
    }
}

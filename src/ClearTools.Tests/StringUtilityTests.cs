namespace ClearTools.Tests;

public class StringUtilityTests
{
    [Fact]
    public void AddUpDate_ShouldReturnNonEmptyString()
    {
        var result = StringUtility.AddUpDate();
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void GetDateCode_ShouldReturnNonEmptyString()
    {
        var result = StringUtility.GetDateCode();
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void GenerateUrlKey_ShouldReturnLowerCaseString()
    {
        var result = StringUtility.GenerateUrlKey("Test String");
        Assert.Equal("test-string", result);
    }

    [Fact]
    public void GenerateTags_ShouldReturnCommaSeparatedString()
    {
        var result = StringUtility.GenerateTags("tag1", "tag2", "tag3");
        Assert.Equal("tag1,tag2,tag3", result);
    }

    [Fact]
    public void StripHTML_ShouldRemoveHtmlTags()
    {
        var result = StringUtility.StripHTML("<p>Test</p>");
        Assert.Equal("Test", result);
    }

    [Fact]
    public void StripSymbols_ShouldRemoveSymbols()
    {
        var result = StringUtility.StripSymbols("Test:;*+&String#");
        Assert.Equal("TestString", result);
    }

    [Fact]
    public void StripSymbols_ShouldPreserveSpaces()
    {
        var result = StringUtility.StripSymbols("Hello World! @#$%");
        Assert.Equal("Hello World ", result);
    }

    [Fact]
    public void StripSymbols_ShouldPreserveAlphanumericAndSpaces()
    {
        var result = StringUtility.StripSymbols("Test 123 String @#$% 456");
        Assert.Equal("Test 123 String  456", result);
    }

    [Fact]
    public void GetSubstring_ShouldReturnSubstring()
    {
        var result = StringUtility.GetSubstring("TestString", 4);
        Assert.Equal("String", result);
    }

    [Fact]
    public void GenerateFileName_ShouldReturnFormattedFileName()
    {
        var result = StringUtility.GenerateFileName("Test Title", "txt");
        Assert.EndsWith(".txt", result);
    }

    [Fact]
    public void TruncateString_ShouldReturnTruncatedString()
    {
        var result = StringUtility.TruncateString("1234567890");
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void CreateParagraphsFromReturns_ShouldWrapTextInParagraphs()
    {
        var result = StringUtility.CreateParagraphsFromReturns("Line1\nLine2");
        Assert.Contains("<p>Line1</p>", result);
        Assert.Contains("<p>Line2</p>", result);
    }

    [Fact]
    public void CreateReturnsFromParagraphs_ShouldRemoveHtmlTags()
    {
        var result = StringUtility.CreateReturnsFromParagraphs("<div><p>Line1</p><p>Line2</p></div>");
        Assert.Equal("Line1\nLine2", result);
    }

    [Fact]
    public void GenerateValidationCode_ShouldReturnNonEmptyString()
    {
        var result = StringUtility.GenerateValidationCode("input", DateTime.Now, 123);
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void ValidationCode_ShouldReturnTrueForValidCode()
    {
        var input = "input";
        var expiryDate = DateTime.Now;
        var secretKey = 123;
        var code = StringUtility.GenerateValidationCode(input, expiryDate, secretKey);
        var isValid = StringUtility.ValidationCode(code, input, expiryDate, secretKey);
        Assert.True(isValid);
    }

    [Fact]
    public void SQLSerialize_ShouldEscapeSingleQuotes()
    {
        var result = StringUtility.SQLSerialize("O'Reilly");
        Assert.Equal("O''Reilly", result);
    }

    [Fact]
    public void TimeSince_ShouldReturnCorrectTimeDescription()
    {
        var result = StringUtility.TimeSince(DateTime.Now.AddSeconds(-10), DateTime.Now);
        Assert.Equal("10 seconds ago", result);
    }

    [Fact]
    public void ExtractInitialsFromName_ShouldReturnInitials()
    {
        var result = StringUtility.ExtractInitialsFromName("John Doe");
        Assert.Equal("JD", result);
    }

    [Fact]
    public void GenerateToken_ShouldReturnTokenOfSpecifiedLength()
    {
        var result = StringUtility.GenerateToken(10, true, true, true, true);
        Assert.Equal(10, result.Length);
    }
}

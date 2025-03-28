using Clear.Models.EditorJS;
using Newtonsoft.Json;

namespace ClearTools.Tests;

public class EditorJSTests
{
    [Fact]
    public void Parse_NullOrEmptyContent_ThrowsArgumentException()
    {
        // Arrange
        string content = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => EditorJS.Parse(content));
    }

    [Fact]
    public void Parse_InvalidJsonContent_ThrowsJsonException()
    {
        // Arrange
        string content = "invalid json";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => EditorJS.Parse(content));
    }

    [Fact]
    public void Parse_ValidJsonContent_ReturnsHtml()
    {
        // Arrange
        var jsonContent = """
        {
            "time": 1742798728409,
            "blocks": [
                {
                    "id": "wY3q4MhMtR",
                    "type": "header",
                    "data": {
                        "text": "Header Text",
                        "level": 1
                    }
                },
                {
                    "id": "TN5IZecAx6",
                    "type": "paragraph",
                    "data": {
                        "text": "Paragraph Text"
                    }
                },
                {
                    "id": "cWcfZIEmmz",
                    "type": "list",
                    "data": {
                        "style": "unordered",
                        "meta": {},
                        "items": [
                            {
                                "content": "Item 1",
                                "meta": {},
                                "items": []
                            },
                            {
                                "content": "Item 2",
                                "meta": {},
                                "items": []
                            }
                        ]
                    }
                },
                {
                    "id": "u0iR0_7__J",
                    "type": "list",
                    "data": {
                        "style": "unordered",
                        "items": [
                            "text list 1."
                        ]
                    }
                },
                {
                    "id": "cWcfZIEmmi",
                    "type": "list",
                    "data": {
                        "style": "unordered",
                        "meta": {},
                        "items": [
                            {
                                "content": "Item 1",
                                "meta": {},
                                "items": []
                            },
                            {
                                "content": "Item 2",
                                "meta": {},
                                "items": [
                                    {
                                        "content": "Sub Item 1",
                                        "meta": {},
                                        "items": []
                                    },
                                    {
                                        "content": "Sub Item 2",
                                        "meta": {},
                                        "items": []
                                    }
                                ]
                            }
                        ]
                    }
                }
            ],
            "version": "2.31.0-rc.7"
        }
        """;

        var expectedResponse = "<h1>Header Text</h1><p>Paragraph Text</p><ul><li>Item 1</li><li>Item 2</li></ul><ul><li>text list 1.</li></ul><ul><li>Item 1</li><li>Item 2<ul><li>Sub Item 1</li><li>Sub Item 2</li></ul></li></ul>";

        // Act
        string result = EditorJS.Parse(jsonContent);

        // Assert
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public void Parse_ContentWithUnsupportedBlockType_ReturnsEmptyString()
    {
        // Arrange
        var content = new Content
        {
            time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            blocks = new[]
            {
                new Block
                {
                    type = "unsupported",
                    data = new Data
                    {
                        text = "Unsupported Text"
                    }
                }
            },
            version = "2.19.3"
        };
        string jsonContent = JsonConvert.SerializeObject(content);

        // Act
        string result = EditorJS.Parse(jsonContent);

        // Assert
        Assert.DoesNotContain("Unsupported Text", result);
    }
}

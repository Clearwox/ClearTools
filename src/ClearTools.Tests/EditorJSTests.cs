using Clear.Tools;
using Clear.Models.EditorJS;
using Newtonsoft.Json;
using System;
using Xunit;

namespace ClearTools.Tests
{
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
            var content = new Content
            {
                time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                blocks = new[]
                {
                    new Block
                    {
                        type = "header",
                        data = new Data
                        {
                            level = 1,
                            text = "Header Text"
                        }
                    },
                    new Block
                    {
                        type = "paragraph",
                        data = new Data
                        {
                            text = "Paragraph Text"
                        }
                    },
                    new Block
                    {
                        type = "list",
                        data = new Data
                        {
                            style = "unordered",
                            items = new[] { "Item 1", "Item 2" }
                        }
                    },
                    new Block
                    {
                        type = "image",
                        data = new Data
                        {
                            url = "https://example.com/image.jpg",
                            caption = "Image Caption"
                        }
                    },
                    new Block
                    {
                        type = "embed",
                        data = new Data
                        {
                            service = "youtube",
                            embed = "https://www.youtube.com/embed/example"
                        }
                    }
                },
                version = "2.19.3"
            };
            string jsonContent = JsonConvert.SerializeObject(content);

            // Act
            string result = EditorJS.Parse(jsonContent);

            // Assert
            Assert.Contains("<h1>Header Text</h1>", result);
            Assert.Contains("<p>Paragraph Text</p>", result);
            Assert.Contains("<ul><li>Item 1</li><li>Item 2</li></ul>", result);
            Assert.Contains("<img style=\"max-width: 100%;\" src=\"https://example.com/image.jpg\" /><p>Image Caption</p>", result);
            Assert.Contains("<iframe width=\"100%\" height=\"100%\" src=\"https://www.youtube.com/embed/example?modestbranding=1&rel=0\"", result);
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
}

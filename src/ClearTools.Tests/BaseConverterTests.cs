using Clear.Tools;
using System;
using Xunit;

namespace ClearTools.Tests
{
    public class BaseConverterTests
    {
        [Fact]
        public void ConvertToDecimal_ValidBase36String_ReturnsDecimalValue()
        {
            // Arrange
            string value = "1Z";
            int baseValue = 36;

            // Act
            long result = BaseConverter.ConvertToDecimal(value, baseValue);

            // Assert
            Assert.Equal(71, result);
        }

        [Fact]
        public void ConvertToDecimal_InvalidCharacter_ThrowsArgumentException()
        {
            // Arrange
            string value = "1Z@";
            int baseValue = 36;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => BaseConverter.ConvertToDecimal(value, baseValue));
        }

        [Fact]
        public void ConvertToDecimal_NullOrWhitespaceValue_ThrowsArgumentException()
        {
            // Arrange
            string value = " ";
            int baseValue = 36;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => BaseConverter.ConvertToDecimal(value, baseValue));
        }

        [Fact]
        public void ConvertFromDecimal_ValidDecimalValue_ReturnsBase36String()
        {
            // Arrange
            long value = 71;
            int baseValue = 36;

            // Act
            string result = BaseConverter.ConvertFromDecimal(value, baseValue);

            // Assert
            Assert.Equal("1Z", result);
        }

        [Fact]
        public void ConvertFromDecimal_BaseValueOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            long value = 71;
            int baseValue = 37;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => BaseConverter.ConvertFromDecimal(value, baseValue));
        }

        [Fact]
        public void ConvertToAlpha_ValidDecimalValue_ReturnsAlphaString()
        {
            // Arrange
            long value = 27;

            // Act
            string result = BaseConverter.ConvertToAlpha(value);

            // Assert
            Assert.Equal("BB", result);
        }

        [Fact]
        public void ConvertToAlpha_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            long value = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => BaseConverter.ConvertToAlpha(value));
        }

        [Fact]
        public void ConvertToAlpha_ZeroValue_ReturnsA()
        {
            // Arrange
            long value = 0;

            // Act
            string result = BaseConverter.ConvertToAlpha(value);

            // Assert
            Assert.Equal("A", result);
        }
    }
}

using Clear.Tools;
using System;
using Xunit;

namespace ClearTools.Tests
{
    public class DateUtilityTests
    {
        [Fact]
        public void IsSameWeek_BothDatesNull_ReturnsFalse()
        {
            // Arrange
            DateTime? date1 = null;
            DateTime? date2 = null;

            // Act
            var result = DateUtility.IsSameWeek(date1, date2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameWeek_OneDateNull_ReturnsFalse()
        {
            // Arrange
            DateTime? date1 = DateTime.Now;
            DateTime? date2 = null;

            // Act
            var result = DateUtility.IsSameWeek(date1, date2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameWeek_DatesInSameWeek_ReturnsTrue()
        {
            // Arrange
            DateTime date1 = new DateTime(2025, 2, 17); // Monday
            DateTime date2 = new DateTime(2025, 2, 19); // Wednesday

            // Act
            var result = DateUtility.IsSameWeek(date1, date2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameWeek_DatesInDifferentWeeks_ReturnsFalse()
        {
            // Arrange
            DateTime date1 = new DateTime(2025, 2, 17); // Monday
            DateTime date2 = new DateTime(2025, 2, 24); // Next Monday

            // Act
            var result = DateUtility.IsSameWeek(date1, date2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameWeek_DatesInSameWeekWithDifferentFirstDayOfWeek_ReturnsTrue()
        {
            // Arrange
            DateTime date1 = new DateTime(2025, 2, 17); // Monday
            DateTime date2 = new DateTime(2025, 2, 19); // Wednesday

            // Act
            var result = DateUtility.IsSameWeek(date1, date2, DayOfWeek.Sunday);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameWeek_DatesInDifferentYearsSameWeekNumber_ReturnsFalse()
        {
            // Arrange
            DateTime date1 = new DateTime(2024, 12, 30); // Last week of 2024
            DateTime date2 = new DateTime(2025, 1, 1); // First week of 2025

            // Act
            var result = DateUtility.IsSameWeek(date1, date2);

            // Assert
            Assert.False(result);
        }
    }
}

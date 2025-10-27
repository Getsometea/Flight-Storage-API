using Xunit;
using FlightStorageService.Helpers;

namespace FlightStorageService.Tests
{
    public class DateValidatorTests
    {
        [Theory]
        [InlineData("2025-10-26", true)]
        [InlineData("1700-01-01", false)]
        [InlineData("10000-01-01", false)] 
        [InlineData("2025-01-01", true)]
        public void IsDateValid_ReturnsExpectedResult(string dateString, bool expected)
        {
            bool parsed = DateTime.TryParse(dateString, out var date);
            if (!parsed)
            {
                Assert.False(expected);
                return;
            }

            bool result = DateValidator.IsDateValid(date);

            Assert.Equal(expected, result);
        }
    }
}

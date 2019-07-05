using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class IsoDayOfWeekExtensionsTests
    {
        #region NextDay

        [Fact]
        public void NextDay_Monday_Tuesday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Monday;

            // Act
            var nextDay = dayOfWeek.NextDay();

            // Assert
            nextDay.Should().Be( IsoDayOfWeek.Tuesday );
        }

        [Fact]
        public void NextDay_Tuesday_Wednesday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Tuesday;

            // Act
            var nextDay = dayOfWeek.NextDay();

            // Assert
            nextDay.Should().Be( IsoDayOfWeek.Wednesday );
        }

        [Fact]
        public void NextDay_Wednesday_Thursday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Wednesday;

            // Act
            var nextDay = dayOfWeek.NextDay();

            // Assert
            nextDay.Should().Be( IsoDayOfWeek.Thursday );
        }

        [Fact]
        public void NextDay_Thursday_Friday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Thursday;

            // Act
            var nextDay = dayOfWeek.NextDay();

            // Assert
            nextDay.Should().Be( IsoDayOfWeek.Friday );
        }

        [Fact]
        public void NextDay_Friday_Saturday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Friday;

            // Act
            var nextDay = dayOfWeek.NextDay();

            // Assert
            nextDay.Should().Be( IsoDayOfWeek.Saturday );
        }

        [Fact]
        public void NextDay_Saturday_Sunday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Saturday;

            // Act
            var nextDay = dayOfWeek.NextDay();

            // Assert
            nextDay.Should().Be( IsoDayOfWeek.Sunday );
        }

        [Fact]
        public void NextDay_Sunday_Monday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Sunday;

            // Act
            var nextDay = dayOfWeek.NextDay();

            // Assert
            nextDay.Should().Be( IsoDayOfWeek.Monday );
        }

        #endregion NextDay

        #region PreviousDay

        [Fact]
        public void PreviousDay_Monday_Sunday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Monday;

            // Act
            var previousDay = dayOfWeek.PreviousDay();

            // Assert
            previousDay.Should().Be( IsoDayOfWeek.Sunday );
        }

        [Fact]
        public void PreviousDay_Tuesday_Monday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Tuesday;

            // Act
            var previousDay = dayOfWeek.PreviousDay();

            // Assert
            previousDay.Should().Be( IsoDayOfWeek.Monday );
        }

        [Fact]
        public void PreviousDay_Wednesday_Tuesday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Wednesday;

            // Act
            var previousDay = dayOfWeek.PreviousDay();

            // Assert
            previousDay.Should().Be( IsoDayOfWeek.Tuesday );
        }

        [Fact]
        public void PreviousDay_Thursday_Wednesday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Thursday;

            // Act
            var previousDay = dayOfWeek.PreviousDay();

            // Assert
            previousDay.Should().Be( IsoDayOfWeek.Wednesday );
        }

        [Fact]
        public void PreviousDay_Friday_Thursday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Friday;

            // Act
            var previousDay = dayOfWeek.PreviousDay();

            // Assert
            previousDay.Should().Be( IsoDayOfWeek.Thursday );
        }

        [Fact]
        public void PreviousDay_Saturday_Friday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Saturday;

            // Act
            var previousDay = dayOfWeek.PreviousDay();

            // Assert
            previousDay.Should().Be( IsoDayOfWeek.Friday );
        }

        [Fact]
        public void PreviousDay_Sunday_Saturday()
        {
            // Arrange
            var dayOfWeek = IsoDayOfWeek.Sunday;

            // Act
            var previousDay = dayOfWeek.PreviousDay();

            // Assert
            previousDay.Should().Be( IsoDayOfWeek.Saturday );
        }

        #endregion PreviousDay
    }
}
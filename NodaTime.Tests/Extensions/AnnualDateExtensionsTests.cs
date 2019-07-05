using AutoFixture;
using FluentAssertions;
using NodaTime.Tests.AutoFixture;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class AnnualDateExtensionsTests
    {
        private static readonly IFixture Fixture = new Fixture().CustomizeForNodaTime();

        [Fact]
        public void NextOccurrence_Feb29()
        {
            // Arrange
            var annualDate = new AnnualDate( 02, 29 );
            var fromDate = Fixture.Create<LocalDate>( x => x.Month > 2 && x.Calendar.IsLeapYear( x.Year + 1 ) );

            // Act
            var date = annualDate.NextOccurrence( fromDate );

            // Assert
            date.Month.Should().Be( 02 );
            date.Day.Should().Be( 29 );
        }
    }
}
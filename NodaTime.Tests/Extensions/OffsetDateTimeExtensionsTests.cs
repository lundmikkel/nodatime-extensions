using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class OffsetDateTimeExtensionsTests
    {
        [Fact]
        public void ToSqlString_WithoutOffset()
        {
            // Arrange
            var dateTime = new LocalDateTime( 2015, 12, 31, 23, 59, 59 );
            var offset = Offset.Zero;
            var offsetDateTime = new OffsetDateTime( dateTime, offset );

            // Act
            var sqlString = offsetDateTime.ToSqlString();

            // Assert
            sqlString.Should().Be( "2015-12-31 23:59:59+00:00" );
        }

        [Fact]
        public void ToSqlString_WithPositiveOffset()
        {
            // Arrange
            var dateTime = new LocalDateTime( 2013, 07, 17, 07, 20, 35 );
            var offset = Offset.FromHours( 7 );
            var offsetDateTime = new OffsetDateTime( dateTime, offset );

            // Act
            var sqlString = offsetDateTime.ToSqlString();

            // Assert
            sqlString.Should().Be( "2013-07-17 07:20:35+07:00" );
        }

        [Fact]
        public void ToSqlString_WithNegativeOffset()
        {
            // Arrange
            var dateTime = new LocalDateTime( 2012, 11, 24, 17, 45, 00 );
            var offset = Offset.FromHoursAndMinutes( -12, -45 );
            var offsetDateTime = new OffsetDateTime( dateTime, offset );

            // Act
            var sqlString = offsetDateTime.ToSqlString();

            // Assert
            sqlString.Should().Be( "2012-11-24 17:45:00-12:45" );
        }
    }
}
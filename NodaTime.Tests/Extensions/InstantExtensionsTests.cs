using System;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class InstantExtensionsTests
    {
        [Fact]
        public void ToSqlString_InstantWithTicks()
        {
            // Arrange
            var instant = Instant.FromUtc( 1234, 05, 06, 07, 34, 56 ) + Duration.FromTicks( 3740124 );

            // Act
            var sqlString = instant.ToSqlString();

            // Assert
            sqlString.Should().Be( "1234-05-06 07:34:56.3740124" );
        }

        [Fact]
        public void ToSqlString_NegativeDecimalPlaces()
        {
            // Arrange
            var instant = Instant.FromUtc( 1234, 05, 06, 07, 34, 56 ) + Duration.FromTicks( 3740124 );

            // Act
            Action act = () => instant.ToSqlString(-1);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ToSqlString_EightDecimalPlaces()
        {
            // Arrange
            var instant = Instant.FromUtc( 1234, 05, 06, 07, 34, 56 ) + Duration.FromTicks( 3740124 );

            // Act
            Action act = () => instant.ToSqlString(8);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ToSqlString_InstantWithTicksAndDecimalPlaces()
        {
            // Arrange
            var instant = Instant.FromUtc( 1234, 05, 06, 07, 34, 56 ) + Duration.FromTicks( 3740124 );
            var decimalPlaces = new Random().Next(7) + 1;
            var expected = "1234-05-06 07:34:56.3740124".Substring(0, 20 + decimalPlaces).TrimEnd('0');

            // Act
            var sqlString = instant.ToSqlString(decimalPlaces);

            // Assert
            sqlString.Should().Be( expected );
        }

        [Fact]
        public void ToSqlString_InstantWithTicksAndWithoutDecimalPlaces()
        {
            // Arrange
            var instant = Instant.FromUtc( 1234, 05, 06, 07, 34, 56 ) + Duration.FromTicks( 3740124 );

            // Act
            var sqlString = instant.ToSqlString(0);

            // Assert
            sqlString.Should().Be("1234-05-06 07:34:56" );
        }

        [Fact]
        public void ToSqlString_InstantWithoutMilliseconds()
        {
            // Arrange
            var instant = Instant.FromUtc( 2017, 05, 23, 07, 30, 01 );

            // Act
            var sqlString = instant.ToSqlString();

            // Assert
            sqlString.Should().Be( "2017-05-23 07:30:01" );
        }

        [Fact]
        public void ToSqlString_InstantWithoutTime()
        {
            // Arrange
            var instant = Instant.FromUtc( 2017, 09, 17, 00, 00, 00 );

            // Act
            var sqlString = instant.ToSqlString();

            // Assert
            sqlString.Should().Be( "2017-09-17" );
        }
    }
}
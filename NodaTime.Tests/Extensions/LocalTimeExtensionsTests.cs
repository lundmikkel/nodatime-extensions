using System;
using AutoFixture;
using FluentAssertions;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class LocalTimeExtensionsTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        #region ToSqlString

        [Fact]
        public void ToSqlString_FullHour()
        {
            // Arrange
            var random = new Random();
            var hour = random.Next( 24 );
            var localTime = new LocalTime( hour, 00 );
            var expectedSqlString = $"{localTime.Hour:D2}:00:00";

            // Act
            var sqlString = localTime.ToSqlString();

            // Assert
            sqlString.Should().Be( expectedSqlString );
        }

        [Fact]
        public void ToSqlString_HoursAndMinutes()
        {
            // Arrange
            var localTime = Fixture.Create<LocalTime>();
            var expectedSqlString = $"{localTime.Hour:D2}:{localTime.Minute:D2}:00";

            // Act
            var sqlString = localTime.ToSqlString();

            // Assert
            sqlString.Should().Be( expectedSqlString );
        }

        [Fact]
        public void ToSqlString_HoursMinutesSecondsAndMilliseconds()
        {
            // Arrange
            var localTime = new LocalTime( 05, 13, 47, 023 ) + Period.FromTicks( 1247 );

            // Act
            var sqlString = localTime.ToSqlString();

            // Assert
            sqlString.Should().Be( "05:13:47.0231247" );
        }

        #endregion ToSqlString
    }
}
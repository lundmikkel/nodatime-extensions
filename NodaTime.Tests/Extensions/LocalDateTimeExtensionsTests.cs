using System;
using AutoFixture;
using FluentAssertions;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using NodaTime.Tzdb;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class LocalDateTimeExtensionsTests
    {
        private static readonly IFixture Fixture = new Fixture().CustomizeForNodaTime();
        private static readonly IDateTimeZoneProvider TimeZoneProvider = DateTimeZoneProviderFactory.Create();
        private static readonly DateTimeZone TimeZone = TimeZoneProvider[ Europe.Copenhagen ];

        #region IsAmbiguousIn

        private static readonly LocalDateTime AmbiguousTimeStart = new LocalDateTime( 2017, 10, 29, 02, 00 );
        private static readonly LocalDateTime AmbiguousTimeEnd = new LocalDateTime( 2017, 10, 29, 03, 00 );

        [Fact]
        public void IsAmbiguousIn_JustBeforeAmbiguous()
        {
            // Arrange
            var dateTime = AmbiguousTimeStart - Period.FromTicks( 1 );

            // Act
            var isAmbiguous = dateTime.IsAmbiguousIn( TimeZone );

            // Assert
            isAmbiguous.Should().BeFalse();
        }

        [Fact]
        public void IsAmbiguousIn_FirstAmbiguousTime()
        {
            // Arrange
            var dateTime = AmbiguousTimeStart;

            // Act
            var isAmbiguous = dateTime.IsAmbiguousIn( TimeZone );

            // Assert
            isAmbiguous.Should().BeTrue();
        }

        [Fact]
        public void IsAmbiguousIn_SomeAmbiguousTime()
        {
            // Arrange
            var random = new Random();
            var difference = AmbiguousTimeEnd - AmbiguousTimeStart;
            var period = Period.FromTicks( random.Next( (int)difference.Ticks ) );
            var dateTime = AmbiguousTimeStart.Date.At( AmbiguousTimeStart.TimeOfDay + period );

            // Act
            var isAmbiguous = dateTime.IsAmbiguousIn( TimeZone );

            // Assert
            isAmbiguous.Should().BeTrue();
        }

        [Fact]
        public void IsAmbiguousIn_LastAmbiguousTime()
        {
            // Arrange
            var dateTime = AmbiguousTimeEnd - Period.FromTicks( 1 );

            // Act
            var isAmbiguous = dateTime.IsAmbiguousIn( TimeZone );

            // Assert
            isAmbiguous.Should().BeTrue();
        }

        [Fact]
        public void IsAmbiguousIn_JustAfterAmbiguousTime()
        {
            // Arrange
            var dateTime = AmbiguousTimeEnd;

            // Act
            var isAmbiguous = dateTime.IsAmbiguousIn( TimeZone );

            // Assert
            isAmbiguous.Should().BeFalse();
        }

        [Fact]
        public void IsAmbiguousIn_AmbiguousInOtherTimeZone()
        {
            // Arrange
            var dateTime = AmbiguousTimeStart;
            var timeZone = TimeZoneProvider[ America.NewYork ];

            // Act
            var isAmbiguous = dateTime.IsAmbiguousIn( timeZone );

            // Assert
            isAmbiguous.Should().BeFalse();
        }

        #endregion IsAmbiguousIn

        #region IsSkippedIn

        private static readonly LocalDateTime SkippedTimeStart = new LocalDateTime( 2017, 03, 26, 02, 00 );
        private static readonly LocalDateTime SkippedTimeEnd = new LocalDateTime( 2017, 03, 26, 03, 00 );

        [Fact]
        public void IsSkippedIn_JustBeforeSkipped()
        {
            // Arrange
            var dateTime = SkippedTimeStart - Period.FromTicks( 1 );

            // Act
            var isSkipped = dateTime.IsSkippedIn( TimeZone );

            // Assert
            isSkipped.Should().BeFalse();
        }

        [Fact]
        public void IsSkippedIn_FirstSkippedTime()
        {
            // Arrange
            var dateTime = SkippedTimeStart;

            // Act
            var isSkipped = dateTime.IsSkippedIn( TimeZone );

            // Assert
            isSkipped.Should().BeTrue();
        }

        [Fact]
        public void IsSkippedIn_SomeSkippedTime()
        {
            // Arrange
            var random = new Random();
            var difference = SkippedTimeEnd - SkippedTimeStart;
            var period = Period.FromTicks( random.Next( (int)difference.Ticks ) );
            var dateTime = SkippedTimeStart.Date.At( SkippedTimeStart.TimeOfDay + period );

            // Act
            var isSkipped = dateTime.IsSkippedIn( TimeZone );

            // Assert
            isSkipped.Should().BeTrue();
        }

        [Fact]
        public void IsSkippedIn_LastSkippedTime()
        {
            // Arrange
            var dateTime = SkippedTimeEnd - Period.FromTicks( 1 );

            // Act
            var isSkipped = dateTime.IsSkippedIn( TimeZone );

            // Assert
            isSkipped.Should().BeTrue();
        }

        [Fact]
        public void IsSkippedIn_JustAfterSkippedTime()
        {
            // Arrange
            var dateTime = SkippedTimeEnd;

            // Act
            var isSkipped = dateTime.IsSkippedIn( TimeZone );

            // Assert
            isSkipped.Should().BeFalse();
        }

        [Fact]
        public void IsSkippedIn_SkippedInOtherTimeZone()
        {
            // Arrange
            var dateTime = SkippedTimeStart;
            var timeZone = TimeZoneProvider[ America.NewYork ];

            // Act
            var isSkipped = dateTime.IsSkippedIn( timeZone );

            // Assert
            isSkipped.Should().BeFalse();
        }

        #endregion IsSkippedIn

        #region ToSqlString

        [Fact]
        public void ToSqlString_DateHoursMinutesSecondsAndMilliseconds()
        {
            // Arrange
            var localDateTime = new LocalDateTime( 1234, 05, 06, 07, 34, 56 ) + Period.FromTicks( 837400 );

            // Act
            var sqlString = localDateTime.ToSqlString();

            // Assert
            sqlString.Should().Be( "1234-05-06 07:34:56.08374" );
        }

        [Fact]
        public void ToSqlString_DateHoursMinutesAndSeconds()
        {
            // Arrange
            var localDateTime = new LocalDateTime( 2017, 05, 23, 07, 30, 01 );

            // Act
            var sqlString = localDateTime.ToSqlString();

            // Assert
            sqlString.Should().Be( "2017-05-23 07:30:01" );
        }

        [Fact]
        public void ToSqlString_DateOnly()
        {
            // Arrange
            var localDateTime = Fixture.Create<LocalDate>().AtMidnight();
            var expectedSqlString = $"{localDateTime.Year:D4}-{localDateTime.Month:D2}-{localDateTime.Day:D2}";

            // Act
            var sqlString = localDateTime.ToSqlString();

            // Assert
            sqlString.Should().Be( expectedSqlString );
        }

        #endregion ToSqlString

        #region ToSinglePointInterval

        [Fact]
        public void ToSinglePointInterval()
        {
            // Arrange
            var dateTime = Fixture.Create<LocalDateTime>();

            // Act
            var dateTimeInterval = dateTime.ToSinglePointInterval();

            // Assert
            dateTimeInterval.Start.Should().Be( dateTime );
            dateTimeInterval.End.Should().Be( dateTime );
        }

        #endregion
    }
}
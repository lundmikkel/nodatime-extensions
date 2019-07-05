using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NodaTime.Tests.AutoFixture;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class DateIntervalExtensionsTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        [Fact]
        public void Dates()
        {
            // Arrange
            var dateInterval = Fixture.Create<DateInterval>();

            // Act
            var dates = dateInterval.Dates().ToList();

            // Assert
            dates.First().Should().Be( dateInterval.Start );
            dates.Last().Should().Be( dateInterval.End );
            dates.Should().BeInAscendingOrder();
            dates.Distinct().Should().Equal( dates );
            dates.Count.Should().Be( dateInterval.Length );
        }

        #region Overlaps

        [Fact]
        public void Overlaps_Before()
        {
            // Arrange
            var (first, second) = Fixture.Create<DateInterval>( ( x, y ) => x.End < y.Start || y.End < x.Start );

            // Act
            var overlaps = first.Overlaps( second );
            var overlapsInverted = first.Overlaps( second );

            // Assert
            overlaps.Should().BeFalse();
            overlapsInverted.Should().BeFalse();
        }

        [Fact]
        public void Overlaps_Meets()
        {
            // Arrange
            var (interval, date) = Fixture.Create<DateInterval, LocalDate>( ( i, d ) => i.End + Period.FromDays( 1 ) < d );
            var secondInterval = new DateInterval( interval.End + Period.FromDays( 1 ), date );

            // Act
            var overlaps = interval.Overlaps( secondInterval );
            var overlapsInverted = secondInterval.Overlaps( interval );

            // Assert
            overlaps.Should().BeFalse();
            overlapsInverted.Should().BeFalse();
        }

        [Fact]
        public void Overlaps_Overlaps()
        {
            // Arrange
            var (firstInterval, secondInterval) = Fixture.Create<DateInterval>( ( x, y ) => x.Start < y.Start && y.Start < x.End && x.End < y.End );

            // Act
            var overlaps = firstInterval.Overlaps( secondInterval );
            var overlapsInverted = secondInterval.Overlaps( firstInterval );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Starts()
        {
            // Arrange
            var (interval, date) = Fixture.Create<DateInterval, LocalDate>( ( i, d ) => i.End < d );
            var secondInterval = new DateInterval( interval.Start, date );

            // Act
            var overlaps = interval.Overlaps( secondInterval );
            var overlapsInverted = secondInterval.Overlaps( interval );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_During()
        {
            // Arrange
            var (firstInterval, secondInterval) = Fixture.Create<DateInterval>( ( x, y ) => y.Start < x.Start && x.End < y.End );

            // Act
            var overlaps = firstInterval.Overlaps( secondInterval );
            var overlapsInverted = secondInterval.Overlaps( firstInterval );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Finishes()
        {
            // Arrange
            var (date, interval) = Fixture.Create<LocalDate, DateInterval>( ( d, i ) => d < i.Start );
            var secondInterval = new DateInterval( date, interval.End );

            // Act
            var overlaps = interval.Overlaps( secondInterval );
            var overlapsInverted = secondInterval.Overlaps( interval );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Equal()
        {
            // Arrange
            var interval = Fixture.Create<DateInterval>();

            // Act
            var overlaps = interval.Overlaps( interval );

            // Assert
            overlaps.Should().BeTrue();
        }

        #endregion Overlaps

        #region GetOverlapWith

        [Fact]
        public void Overlap_Before()
        {
            // Arrange
            var (first, second) = Fixture.Create<DateInterval>( ( x, y ) => x.End < y.Start || y.End < x.Start );

            // Act
            var overlap = first.GetOverlapWith( second );
            var overlapInverted = first.GetOverlapWith( second );

            // Assert
            overlap.Should().BeNull();
            overlapInverted.Should().BeNull();
        }

        [Fact]
        public void Overlap_Meets()
        {
            // Arrange
            var (interval, date) = Fixture.Create<DateInterval, LocalDate>( ( i, d ) => i.End + Period.FromDays( 1 ) < d );
            var secondInterval = new DateInterval( interval.End + Period.FromDays( 1 ), date );

            // Act
            var overlap = interval.GetOverlapWith( secondInterval );
            var overlapInverted = secondInterval.GetOverlapWith( interval );

            // Assert
            overlap.Should().BeNull();
            overlapInverted.Should().BeNull();
        }

        [Fact]
        public void Overlap_Overlap()
        {
            // Arrange
            var (firstInterval, secondInterval) = Fixture.Create<DateInterval>( ( x, y ) => x.Start < y.Start && y.Start < x.End && x.End < y.End );
            var expected = new DateInterval( secondInterval.Start, firstInterval.End );

            // Act
            var overlap = firstInterval.GetOverlapWith( secondInterval );
            var overlapInverted = secondInterval.GetOverlapWith( firstInterval );

            // Assert
            overlap.Should().Be( expected );
            overlapInverted.Should().Be( expected );
        }

        [Fact]
        public void Overlap_Starts()
        {
            // Arrange
            var (interval, date) = Fixture.Create<DateInterval, LocalDate>( ( i, d ) => i.End < d );
            var secondInterval = new DateInterval( interval.Start, date );

            // Act
            var overlap = interval.GetOverlapWith( secondInterval );
            var overlapInverted = secondInterval.GetOverlapWith( interval );

            // Assert
            overlap.Should().Be( interval );
            overlapInverted.Should().Be( interval );
        }

        [Fact]
        public void Overlap_During()
        {
            // Arrange
            var (firstInterval, secondInterval) = Fixture.Create<DateInterval>( ( x, y ) => y.Start < x.Start && x.End < y.End );

            // Act
            var overlap = firstInterval.GetOverlapWith( secondInterval );
            var overlapInverted = secondInterval.GetOverlapWith( firstInterval );

            // Assert
            overlap.Should().Be( firstInterval );
            overlapInverted.Should().Be( firstInterval );
        }

        [Fact]
        public void Overlap_Finishes()
        {
            // Arrange
            var (date, interval) = Fixture.Create<LocalDate, DateInterval>( ( d, i ) => d < i.Start );
            var secondInterval = new DateInterval( date, interval.End );

            // Act
            var overlap = interval.GetOverlapWith( secondInterval );
            var overlapInverted = secondInterval.GetOverlapWith( interval );

            // Assert
            overlap.Should().Be( interval );
            overlapInverted.Should().Be( interval );
        }

        [Fact]
        public void Overlap_Equal()
        {
            // Arrange
            var interval = Fixture.Create<DateInterval>();

            // Act
            var overlap = interval.GetOverlapWith( interval );

            // Assert
            overlap.Should().Be( interval );
        }

        #endregion GetOverlapWith

        #region ToDateTimeInterval

        [Fact]
        public void ToDateTimeInterval_NullInterval_ThrowsException()
        {
            // Arrange
            DateInterval dateInterval = null;

            // Act
            Action act = () => dateInterval.ToDateTimeInterval();

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>().WithMessage( "Value cannot be null.\r\nParameter name: dateInterval" );
        }

        [Fact]
        public void ToDateTimeInterval_SingleDayInterval_StartToEndOfDay()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var dateInterval = date.ToSingleDayInterval();

            // Act
            var dateTimeInterval = dateInterval.ToDateTimeInterval();

            // Assert
            dateTimeInterval.Start.Should().Be( date.AtMidnight() );
            dateTimeInterval.End.Should().Be( date.NextDay().AtMidnight() );
        }

        [Fact]
        public void ToDateTimeInterval_RandomDateInterval()
        {
            // Arrange
            var dateInterval = Fixture.Create<DateInterval>();

            // Act
            var dateTimeInterval = dateInterval.ToDateTimeInterval();

            // Assert
            dateTimeInterval.Start.Should().Be( dateInterval.Start.AtMidnight() );
            dateTimeInterval.End.Should().Be( dateInterval.End.NextDay().AtMidnight() );
        }

        #endregion ToDateTimeInterval
    }
}
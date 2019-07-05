using System;
using AutoFixture;
using FluentAssertions;
using NodaTime;
using NodaTime.Tests.AutoFixture;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using Xunit;

// ReSharper disable EqualExpressionComparison
// CS1718: Comparison made to same variable.  This is intentional to test operator ==.
#pragma warning disable CS1718 // Comparison made to same variable

namespace NodaTime.Tests
{
    public class DateTimeIntervalTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        #region Constructor

        [Fact]
        public void Construction_DifferentCalendars()
        {
            // Arrange
            var (start, end) = Fixture.Create<LocalDateTime>( ( x, y ) => x < y );
            end = end.WithCalendar( CalendarSystem.Julian );

            // Act
            Action act = () => new DateTimeInterval( start, end );

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage( "Calendars of start and end date times must be the same.\r\nParameter name: end" );
        }

        [Fact]
        public void Construction_EndBeforeStart()
        {
            // Arrange
            var (start, end) = Fixture.Create<LocalDateTime>( ( x, y ) => x > y );

            // Act
            Action act = () => new DateTimeInterval( start, end );

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage( "End date time must not be earlier than the start date time.\r\nParameter name: end" );
        }

        [Fact]
        public void Construction_EqualStartAndEnd()
        {
            // Arrange
            var localDateTime = Fixture.Create<LocalDateTime>();

            // Act
            var interval = new DateTimeInterval( localDateTime, localDateTime );

            // Assert
            interval.Start.Should().Be( localDateTime );
            interval.End.Should().Be( localDateTime );
        }

        [Fact]
        public void Construction_Properties()
        {
            // Arrange
            var (start, end) = Fixture.Create<LocalDateTime>( ( x, y ) => x < y );

            // Act
            var interval = new DateTimeInterval( start, end );

            // Assert
            interval.Start.Should().Be( start );
            interval.End.Should().Be( end );
        }

        #endregion Constructor

        #region Equality

        [Fact]
        public void Equals_SameInstance()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();

            // Assert
            interval.Should().Be( interval );
            interval.GetHashCode().Should().Be( interval.GetHashCode() );
            ( interval == interval ).Should().BeTrue();
            ( interval != interval ).Should().BeFalse();
            interval.Equals( interval ).Should().BeTrue(); // IEquatable implementation
        }

        [Fact]
        public void Equals_EqualValues()
        {
            // Arrange
            var (start, end) = Fixture.Create<LocalDateTime>( ( x, y ) => x < y );
            var interval1 = new DateTimeInterval( start, end );
            var interval2 = new DateTimeInterval( start, end );

            // Assert
            interval1.Should().Be( interval2 );
            interval1.GetHashCode().Should().Be( interval2.GetHashCode() );
            ( interval1 == interval2 ).Should().BeTrue();
            ( interval1 != interval2 ).Should().BeFalse();
            interval1.Equals( interval2 ).Should().BeTrue(); // IEquatable implementation
        }

        [Fact]
        public void Equals_DifferentCalendars()
        {
            // Arrange
            var (start1, end1) = Fixture.Create<LocalDateTime>( ( x, y ) => x < y );
            var interval1 = new DateTimeInterval( start1, end1 );

            // This is a really, really similar calendar to ISO, but we do distinguish.
            var start2 = start1.WithCalendar( CalendarSystem.Gregorian );
            var end2 = end1.WithCalendar( CalendarSystem.Gregorian );
            var interval2 = new DateTimeInterval( start2, end2 );

            // Assert
            interval1.Should().NotBe( interval2 );
            interval1.GetHashCode().Should().NotBe( interval2.GetHashCode() );
            ( interval1 == interval2 ).Should().BeFalse();
            ( interval1 != interval2 ).Should().BeTrue();
            interval1.Equals( interval2 ).Should().BeFalse(); // IEquatable implementation
        }

        [Fact]
        public void Equals_DifferentStart()
        {
            // Arrange
            var end = Fixture.Create<LocalDateTime>();
            var start1 = Fixture.Create<LocalDateTime>( x => x <= end );
            var start2 = Fixture.Create<LocalDateTime>( x => x <= end );
            var interval1 = new DateTimeInterval( start1, end );
            var interval2 = new DateTimeInterval( start2, end );

            // Assert
            interval1.Should().NotBe( interval2 );
            interval1.GetHashCode().Should().NotBe( interval2.GetHashCode() );
            ( interval1 == interval2 ).Should().BeFalse();
            ( interval1 != interval2 ).Should().BeTrue();
            interval1.Equals( interval2 ).Should().BeFalse(); // IEquatable implementation
        }

        [Fact]
        public void Equals_DifferentEnd()
        {
            // Arrange
            var start = Fixture.Create<LocalDateTime>();
            var end1 = Fixture.Create<LocalDateTime>( x => start <= x );
            var end2 = Fixture.Create<LocalDateTime>( x => start <= x );
            var interval1 = new DateTimeInterval( start, end1 );
            var interval2 = new DateTimeInterval( start, end2 );

            // Assert
            interval1.Should().NotBe( interval2 );
            interval1.GetHashCode().Should().NotBe( interval2.GetHashCode() );
            ( interval1 == interval2 ).Should().BeFalse();
            ( interval1 != interval2 ).Should().BeTrue();
            interval1.Equals( interval2 ).Should().BeFalse(); // IEquatable implementation
        }

        [Fact]
        public void Equals_DifferentToNull()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();

            // Assert
            interval.Should().NotBe( null );
        }

        [Fact]
        public void Equals_DifferentToOtherType()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();
            var instant = Fixture.Create<Instant>();

            // Assert
            interval.Should().NotBe( instant );
        }

        #endregion Equality

        #region Period

        [Fact]
        public void Period_()
        {
            var start = new LocalDateTime( 2000, 1, 1, 00, 00, 00 );
            var end = new LocalDateTime( 2000, 2, 10, 00, 00, 00 );
            var interval = new DateTimeInterval( start, end );
            var period = new PeriodBuilder { Months = 1, Days = 9 }.Build();
            period.Should().Be( interval.Period );
        }

        #endregion Period

        #region Contains

        [Fact]
        public void Contains_DifferentCalendar()
        {
            // Arrange
            var (interval, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>( ( x, i ) => x.Start <= i && i < x.End );
            var candidate = dateTime.WithCalendar( CalendarSystem.Julian );

            // Act
            Action action = () => interval.Contains( candidate );

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Contains_Before()
        {
            // Arrange
            var (interval, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>( ( x, y ) => y < x.Start );

            // Act
            var contains = interval.Contains( dateTime );

            // Assert
            contains.Should().BeFalse();
        }

        [Fact]
        public void Contains_Meets()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();
            var dateTime = interval.Start - Period.FromTicks( 1 );

            // Act
            var contains = interval.Contains( dateTime );

            // Assert
            contains.Should().BeFalse();
        }

        [Fact]
        public void Contains_Start()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();

            // Act
            var contains = interval.Contains( interval.Start );

            // Assert
            contains.Should().BeTrue();
        }

        [Fact]
        public void Contains_Contains()
        {
            // Arrange
            var (interval, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>( ( x, i ) => x.Start <= i && i < x.End );

            // Act
            var contains = interval.Contains( dateTime );

            // Assert
            contains.Should().BeTrue();
        }

        [Fact]
        public void Contains_Ends()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();
            var dateTime = interval.End - Period.FromTicks( 1 );

            // Act
            var contains = interval.Contains( dateTime );

            // Assert
            contains.Should().BeTrue();
        }

        [Fact]
        public void Contains_End()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();

            // Act
            var contains = interval.Contains( interval.End );

            // Assert
            contains.Should().BeFalse();
        }

        [Fact]
        public void Contains_After()
        {
            // Arrange
            var (interval, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>( ( x, y ) => x.End < y );

            // Act
            var contains = interval.Contains( dateTime );

            // Assert
            contains.Should().BeFalse();
        }

        #endregion Contains

        #region Overlaps

        [Fact]
        public void Overlaps_Null_ThrowsException()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();

            // Act
            Action act = () => interval.Overlaps( null );

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>().WithMessage( "Value cannot be null.\r\nParameter name: interval" );
        }

        [Fact]
        public void Overlaps_DifferentCalendars_ThrowsException()
        {
            // Arrange
            var first = Fixture.Create<DateTimeInterval>();
            var (start, end) = Fixture.Create<LocalDateTime>( ( x, y ) => x <= y );
            var second = new DateTimeInterval( start.WithCalendar( CalendarSystem.Julian ), end.WithCalendar( CalendarSystem.Julian ) );

            // Act
            Action act = () => first.Overlaps( second );

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage( "The given interval must be in the same calendar as this interval.\r\nParameter name: interval" );
        }

        [Fact]
        public void Overlaps_SinglePointIntervalOverlapsItself_True()
        {
            // Arrange
            var dateTime = Fixture.Create<LocalDateTime>();
            var dateTimeInterval = dateTime.ToSinglePointInterval();

            // Act
            var overlaps = dateTimeInterval.Overlaps( dateTimeInterval );

            // Assert
            overlaps.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Before_False()
        {
            // Arrange
            var (first, second) = Fixture.Create<DateTimeInterval>( ( x, y ) => x.End < y.Start || y.End < x.Start );

            // Act
            var overlaps = first.Overlaps( second );
            var overlapsInverted = second.Overlaps( first );

            // Assert
            overlaps.Should().BeFalse();
            overlapsInverted.Should().BeFalse();
        }

        [Fact]
        public void Overlaps_Meets_False()
        {
            // Arrange
            var (first, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>( ( x, y ) => x.End < y );
            var second = new DateTimeInterval( first.End, dateTime );

            // Act
            var overlaps = first.Overlaps( second );
            var overlapsInverted = second.Overlaps( first );

            // Assert
            overlaps.Should().BeFalse();
            overlapsInverted.Should().BeFalse();
        }

        [Fact]
        public void Overlaps_Overlaps_True()
        {
            // Arrange
            var (first, second) = Fixture.Create<DateTimeInterval>( ( x, y ) => x.Start < y.Start && y.Start < x.End && x.End < y.End );

            // Act
            var overlaps = first.Overlaps( second );
            var overlapsInverted = second.Overlaps( first );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Starts_True()
        {
            // Arrange
            var (first, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>( ( x, y ) => x.End < y );
            var second = new DateTimeInterval( first.Start, dateTime );

            // Act
            var overlaps = first.Overlaps( second );
            var overlapsInverted = second.Overlaps( first );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Contains_True()
        {
            // Arrange
            var (first, second) = Fixture.Create<DateTimeInterval>( ( x, y ) => x.Start < y.Start && y.End < x.End );

            // Act
            var overlaps = first.Overlaps( second );
            var overlapsInverted = second.Overlaps( first );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Finishes_True()
        {
            // Arrange
            var (dateTime, first) = Fixture.Create<LocalDateTime, DateTimeInterval>( ( d, i ) => d < i.Start );
            var second = new DateTimeInterval( dateTime, first.End );

            // Act
            var overlaps = first.Overlaps( second );
            var overlapsInverted = second.Overlaps( first );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Equal_True()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();

            // Act
            var overlaps = interval.Overlaps( interval );

            // Assert
            overlaps.Should().BeTrue();
        }

        #region Single point interval

        [Fact]
        public void Overlaps_SinglePointBefore_False()
        {
            // Arrange
            var (dateTime, interval) = Fixture.Create<LocalDateTime, DateTimeInterval>( ( x, y ) => x < y.Start );
            var singlePointInterval = dateTime.ToSinglePointInterval();

            // Act
            var overlaps = singlePointInterval.Overlaps( interval );
            var overlapsInverted = interval.Overlaps( singlePointInterval );

            // Assert
            overlaps.Should().BeFalse();
            overlapsInverted.Should().BeFalse();
        }

        [Fact]
        public void Overlaps_SinglePointStarts_True()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();
            var singlePointInterval = interval.Start.ToSinglePointInterval();

            // Act
            var overlaps = singlePointInterval.Overlaps( interval );
            var overlapsInverted = interval.Overlaps( singlePointInterval );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_SinglePointContains_True()
        {
            // Arrange
            var (interval, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>( ( x, y ) => x.Start < y && y < x.End );
            var singlePointInterval = dateTime.ToSinglePointInterval();

            // Act
            var overlaps = singlePointInterval.Overlaps( interval );
            var overlapsInverted = interval.Overlaps( singlePointInterval );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_Ends_False()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();
            var singlePointInterval = interval.End.ToSinglePointInterval();

            // Act
            var overlaps = singlePointInterval.Overlaps( interval );
            var overlapsInverted = interval.Overlaps( singlePointInterval );

            // Assert
            overlaps.Should().BeFalse();
            overlapsInverted.Should().BeFalse();
        }

        [Fact]
        public void Overlaps_SinglePointAfter_False()
        {
            // Arrange
            var (interval, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>( ( x, y ) => x.End < y );
            var singlePointInterval = dateTime.ToSinglePointInterval();

            // Act
            var overlaps = singlePointInterval.Overlaps( interval );
            var overlapsInverted = interval.Overlaps( singlePointInterval );

            // Assert
            overlaps.Should().BeFalse();
            overlapsInverted.Should().BeFalse();
        }

        [Fact]
        public void Overlaps_SinglePointEqual_True()
        {
            // Arrange
            var dateTime = Fixture.Create<LocalDateTime>();
            var singlePointInterval1 = dateTime.ToSinglePointInterval();
            var singlePointInterval2 = dateTime.ToSinglePointInterval();

            // Act
            var overlaps = singlePointInterval1.Overlaps( singlePointInterval2 );
            var overlapsInverted = singlePointInterval1.Overlaps( singlePointInterval2 );

            // Assert
            overlaps.Should().BeTrue();
            overlapsInverted.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_SinglePointUnequal_False()
        {
            // Arrange
            var (dateTime1, dateTime2) = Fixture.Create<LocalDateTime>( ( x, y ) => x != y );
            var singlePointInterval1 = dateTime1.ToSinglePointInterval();
            var singlePointInterval2 = dateTime2.ToSinglePointInterval();

            // Act
            var overlaps = singlePointInterval1.Overlaps( singlePointInterval2 );
            var overlapsInverted = singlePointInterval2.Overlaps( singlePointInterval1 );

            // Assert
            overlaps.Should().BeFalse();
            overlapsInverted.Should().BeFalse();
        }

        #endregion Single point interval

        #endregion Overlaps

        #region ToString

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            var start = new LocalDateTime( 2000, 1, 1, 09, 59, 33 );
            var end = new LocalDateTime( 2001, 6, 19, 13, 11, 42 );
            var interval = new DateTimeInterval( start, end );

            // Act
            var s = interval.ToString();

            // Assert
            s.Should().Be( "2000-01-01T09:59:33/2001-06-19T13:11:42" );
        }

        #endregion ToString
    }
}
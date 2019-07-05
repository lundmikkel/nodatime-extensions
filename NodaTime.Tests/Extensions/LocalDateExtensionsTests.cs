using AutoFixture;
using FluentAssertions;
using NodaTime.Tests.AutoFixture;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class LocalDateExtensionsTests
    {
        private static readonly IFixture Fixture = new Fixture().CustomizeForNodaTime();

        [Fact]
        public void ToSqlString()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var expectedSqlString = $"{localDate.Year:D4}-{localDate.Month:D2}-{localDate.Day:D2}";

            // Act
            var sqlString = localDate.ToSqlString( );

            // Assert
            sqlString.Should().Be( expectedSqlString );
        }

        #region PreviousOrSame

        [Fact]
        public void PreviousOrSame_SameDay()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var dayOfWeek = localDate.DayOfWeek;

            // Act
            var previousOrSame = localDate.PreviousOrSame(dayOfWeek);

            // Assert
            previousOrSame.Should().Be(localDate);
        }

        [Fact]
        public void PreviousOrSame_NextDay()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var expected = localDate - Period.FromDays(6);
            var dayOfWeek = expected.DayOfWeek;

            // Act
            var previousOrSame = localDate.PreviousOrSame(dayOfWeek);

            // Assert
            previousOrSame.Should().Be(expected);
        }

        [Fact]
        public void PreviousOrSame_PreviousDay()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var expected = localDate.PreviousDay();
            var dayOfWeek = expected.DayOfWeek;

            // Act
            var previousOrSame = localDate.PreviousOrSame(dayOfWeek);

            // Assert
            previousOrSame.Should().Be(expected);
        }

        [Fact]
        public void PreviousOrSame_RandomDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var offset = new System.Random().Next(7);
            var expected = localDate - Period.FromDays(offset);
            var dayOfWeek = expected.DayOfWeek;

            // Act
            var previousOrSame = localDate.PreviousOrSame(dayOfWeek);

            // Assert
            previousOrSame.Should().Be(expected);
        }

        #endregion

        #region NextOrSame

        [Fact]
        public void NextOrSame_SameDay()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var dayOfWeek = localDate.DayOfWeek;

            // Act
            var nextOrSame = localDate.NextOrSame(dayOfWeek);

            // Assert
            nextOrSame.Should().Be(localDate);
        }

        [Fact]
        public void NextOrSame_NextDay()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var expected = localDate.NextDay();
            var dayOfWeek = expected.DayOfWeek;

            // Act
            var nextOrSame = localDate.NextOrSame(dayOfWeek);

            // Assert
            nextOrSame.Should().Be(expected);
        }

        [Fact]
        public void NextOrSame_PreviousDay()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var expected = localDate + Period.FromDays(6);
            var dayOfWeek = expected.DayOfWeek;

            // Act
            var nextOrSame = localDate.NextOrSame(dayOfWeek);

            // Assert
            nextOrSame.Should().Be(expected);
        }

        [Fact]
        public void NextOrSame_LastDayOf2016()
        {
            // Arrange
            var localDate = new LocalDate(2016, 12, 31);
            var dayOfWeek = IsoDayOfWeek.Sunday;
            var expectedDate = new LocalDate(2017, 1, 1);

            // Act
            var nextOrSame = localDate.NextOrSame(dayOfWeek);

            // Assert
            nextOrSame.Should().Be(expectedDate);
        }

        [Fact]
        public void NextOrSame_RandomDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var offset = new System.Random().Next(7);
            var expectedDate = localDate.Plus(Period.FromDays(offset));
            var firstDayOfWeek = expectedDate.DayOfWeek;

            // Act
            var nextOrSame = localDate.NextOrSame(firstDayOfWeek);

            // Assert
            nextOrSame.Should().Be(expectedDate);
        }

        #endregion NextOrSame

        #region ToSingleDayInterval

        [Fact]
        public void ToSingleDayInterval()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();

            // Act
            var dateInterval = date.ToSingleDayInterval();

            // Assert
            dateInterval.Start.Should().Be( date );
            dateInterval.End.Should().Be( date );
        }

        #endregion ToSingleDayInterval

        #region At

        [Fact]
        public void At_StartTimeIsLessThanEndTime()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var (startTime, endTime) = Fixture.Create<LocalTime>( ( x, y ) => x < y );

            // Act
            var interval = date.At( startTime, endTime );

            // Assert
            interval.Start.Should().Be( date.At( startTime ) );
            interval.End.Should().Be( date.At( endTime ) );
        }

        [Fact]
        public void At_StartTimeIsEqualToEndTime()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var time = Fixture.Create<LocalTime>();

            // Act
            var interval = date.At( time, time );

            // Assert
            interval.Start.Should().Be( date.At( time ) );
            interval.End.Should().Be( date.NextDay().At( time ) );
        }

        [Fact]
        public void At_StartTimeIsGreaterThanEndTime()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var (startTime, endTime) = Fixture.Create<LocalTime>( ( x, y ) => x > y );

            // Act
            var interval = date.At( startTime, endTime );

            // Assert
            interval.Start.Should().Be( date.At( startTime ) );
            interval.End.Should().Be( date.NextDay().At( endTime ) );
        }

        #endregion At

        #region Nullable At

        [Fact]
        public void At_NullableStartTimeIsLessThanNullableEndTime()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            (LocalTime? startTime, LocalTime? endTime) = Fixture.Create<LocalTime>( ( x, y ) => x < y );

            // Act
            var interval = date.At( startTime, endTime );

            // Assert
            interval.StartDateTime.Should().Be( date.At( startTime.Value ) );
            interval.EndDateTime.Should().Be( date.At( endTime.Value ) );
        }

        [Fact]
        public void At_NullableStartTimeIsEqualToNullableEndTime()
        {
            // Arrange
            LocalDate date = Fixture.Create<LocalDate>();
            LocalTime? time = Fixture.Create<LocalTime>();

            // Act
            var interval = date.At( time, time );

            // Assert
            interval.StartDateTime.Should().Be( date.At( time.Value ) );
            interval.EndDateTime.Should().Be( date.NextDay().At( time.Value ) );
        }

        [Fact]
        public void At_NullableStartTimeIsGreaterThanNullableEndTime()
        {
            // Arrange
            LocalDate date = Fixture.Create<LocalDate>();
            (LocalTime? startTime, LocalTime? endTime) = Fixture.Create<LocalTime>( ( x, y ) => x > y );

            // Act
            var interval = date.At( startTime, endTime );

            // Assert
            interval.StartDateTime.Should().Be( date.At( startTime.Value ) );
            interval.EndDateTime.Should().Be( date.NextDay().At( endTime.Value ) );
        }

        [Fact]
        public void At_StartWithoutValue()
        {
            // Arrange
            LocalDate date = Fixture.Create<LocalDate>();
            LocalTime? time = Fixture.Create<LocalTime>(x => x != LocalTime.Midnight);

            // Act
            var interval = date.At( null, time );

            // Assert
            interval.StartDateTime.HasValue.Should().BeFalse(); // TODO: Add LocalDateTimeAssertions!
            interval.EndDateTime.Should().Be( date.At( time.Value ) );
        }

        [Fact]
        public void At_StartMidnightWithoutValue()
        {
            // Arrange
            LocalDate date = Fixture.Create<LocalDate>();
            LocalTime? time = LocalTime.Midnight;

            // Act
            var interval = date.At( null, time );

            // Assert
            interval.StartDateTime.HasValue.Should().BeFalse(); // TODO: Add LocalDateTimeAssertions!
            interval.EndDateTime.Should().Be( date.NextDay().At( time.Value ) );
        }

        [Fact]
        public void At_EndWithoutValue()
        {
            // Arrange
            LocalDate date = Fixture.Create<LocalDate>();
            LocalTime? time = Fixture.Create<LocalTime>();

            // Act
            var interval = date.At( time, null );

            // Assert
            interval.StartDateTime.Should().Be( date.At( time.Value ) );
            interval.EndDateTime.HasValue.Should().BeFalse(); // TODO: Add LocalDateTimeAssertions!
        }

        [Fact]
        public void At_BothWithoutValue()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();

            // Act
            var interval = date.At( null, null );

            // Assert
            interval.StartDateTime.HasValue.Should().BeFalse(); // TODO: Add LocalDateTimeAssertions!
            interval.EndDateTime.HasValue.Should().BeFalse(); // TODO: Add LocalDateTimeAssertions!
        }

        #endregion Nullable At

        #region StartOfWeek

        [Fact]
        public void StartOfWeek_FirstDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var firstDayOfWeek = localDate.DayOfWeek;

            // Act
            var startOfWeek = localDate.StartOfWeek( firstDayOfWeek );

            // Assert
            startOfWeek.Should().Be( localDate );
        }

        [Fact]
        public void StartOfWeek_LastDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var expected = localDate.Minus( Period.FromDays( 6 ) );
            var firstDayOfWeek = expected.DayOfWeek;

            // Act
            var startOfWeek = localDate.StartOfWeek( firstDayOfWeek );

            // Assert
            startOfWeek.Should().Be( expected );
        }

        [Fact]
        public void StartOfWeek_RandomDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var offset = new System.Random().Next( 7 );
            var expected = localDate.Minus( Period.FromDays( offset ) );
            var firstDayOfWeek = expected.DayOfWeek;

            // Act
            var startOfWeek = localDate.StartOfWeek( firstDayOfWeek );

            // Assert
            startOfWeek.Should().Be( expected );
        }

        #endregion StartOfWeek

        #region EndOfWeek

        [Fact]
        public void EndOfWeek_FirstDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var firstDayOfWeek = localDate.DayOfWeek;
            var expectedDate = localDate.PlusDays( 6 );

            // Act
            var endOfWeek = localDate.EndOfWeek( firstDayOfWeek );

            // Assert
            endOfWeek.Should().Be( expectedDate );
        }

        [Fact]
        public void EndOfWeek_LastDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var firstDayOfWeek = localDate.NextDay().DayOfWeek;

            // Act
            var endOfWeek = localDate.EndOfWeek( firstDayOfWeek );

            // Assert
            endOfWeek.Should().Be( localDate );
        }

        [Fact]
        public void EndOfWeek_LastDayOf2016()
        {
            // Arrange
            var localDate = new LocalDate( 2016, 12, 31 );
            var firstDayOfWeek = IsoDayOfWeek.Monday;
            var expectedDate = new LocalDate( 2017, 1, 1 );

            // Act
            var endOfWeek = localDate.EndOfWeek( firstDayOfWeek );

            // Assert
            endOfWeek.Should().Be( expectedDate );
        }

        [Fact]
        public void EndOfWeek_RandomDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var offset = new System.Random().Next( 7 );
            var expectedDate = localDate.Plus( Period.FromDays( offset ) );
            var firstDayOfWeek = expectedDate.NextDay().DayOfWeek;

            // Act
            var endOfWeek = localDate.EndOfWeek( firstDayOfWeek );

            // Assert
            endOfWeek.Should().Be( expectedDate );
        }

        #endregion EndOfWeek

        #region StartOfMonth

        [Fact]
        public void StartOfMonth()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();

            // Act
            var startOfMonth = date.StartOfMonth();

            // Assert
            startOfMonth.Day.Should().Be( 1 );
            startOfMonth.Month.Should().Be( date.Month );
            startOfMonth.Year.Should().Be( date.Year );
        }

        #endregion StartOfMonth

        #region EndOfMonth

        [Fact]
        public void EndOfMonth_28DayFebruary()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>( x => x.Month == 2 && !x.Calendar.IsLeapYear( x.Year ) );

            // Act
            var endOfMonth = date.EndOfMonth();

            // Assert
            endOfMonth.Day.Should().Be( 28 );
            endOfMonth.Month.Should().Be( date.Month );
            endOfMonth.Year.Should().Be( date.Year );
            endOfMonth.NextDay().Day.Should().Be( 1 );
        }

        [Fact]
        public void EndOfMonth_29DayFebruary()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>( x => x.Month == 2 && x.Calendar.IsLeapYear( x.Year ) );

            // Act
            var endOfMonth = date.EndOfMonth();

            // Assert
            endOfMonth.Day.Should().Be( 29 );
            endOfMonth.Month.Should().Be( date.Month );
            endOfMonth.Year.Should().Be( date.Year );
            endOfMonth.NextDay().Day.Should().Be( 1 );
        }

        [Fact]
        public void EndOfMonth()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();

            // Act
            var endOfMonth = date.EndOfMonth();

            // Assert
            endOfMonth.Month.Should().Be( date.Month );
            endOfMonth.Year.Should().Be( date.Year );
            endOfMonth.NextDay().Day.Should().Be( 1 );
        }

        #endregion EndOfMonth

        #region GetContainingWeekInterval

        [Fact]
        public void GetContainingWeekInterval_FirstDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var firstDayOfWeek = localDate.DayOfWeek;

            // Act
            var weekInterval = localDate.GetContainingWeekInterval( firstDayOfWeek );

            // Assert
            weekInterval.Start.Should().Be( localDate );
            weekInterval.End.Should().Be( localDate.PlusDays( 6 ) );
            weekInterval.Length.Should().Be( 7 );
        }

        [Fact]
        public void GetContainingWeekInterval_LastDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var firstDayOfWeek = localDate.NextDay().DayOfWeek;

            // Act
            var weekInterval = localDate.GetContainingWeekInterval( firstDayOfWeek );

            // Assert
            weekInterval.Start.Should().Be( localDate - Period.FromDays( 6 ) );
            weekInterval.End.Should().Be( localDate );
            weekInterval.Length.Should().Be( 7 );
        }

        [Fact]
        public void GetContainingWeekInterval_SomeDayOfWeek()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var firstDayOfWeek = Fixture.Create<IsoDayOfWeek>();

            // Act
            var weekInterval = localDate.GetContainingWeekInterval( firstDayOfWeek );

            // Assert
            weekInterval.Start.Should().Be( localDate.PreviousOrSame( firstDayOfWeek ) );
            weekInterval.End.Should().Be( localDate.NextOrSame( firstDayOfWeek.PreviousDay() ) );
            weekInterval.Length.Should().Be( 7 );
        }

        #endregion GetContainingWeekInterval

        #region GetContainingMonthInterval

        [Fact]
        public void GetContainingMonthInterval_FirstDayOfMonth()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>().StartOfMonth();
            var daysInMonth = localDate.Calendar.GetDaysInMonth( localDate.Year, localDate.Month );

            // Act
            var weekInterval = localDate.GetContainingMonthInterval();

            // Assert
            weekInterval.Start.Should().Be( localDate );
            weekInterval.End.Should().Be( localDate.EndOfMonth() );
            weekInterval.Length.Should().Be( daysInMonth );
        }

        [Fact]
        public void GetContainingMonthInterval_LastDayOfMonth()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>().EndOfMonth();
            var daysInMonth = localDate.Calendar.GetDaysInMonth( localDate.Year, localDate.Month );

            // Act
            var weekInterval = localDate.GetContainingMonthInterval();

            // Assert
            weekInterval.Start.Should().Be( localDate.StartOfMonth() );
            weekInterval.End.Should().Be( localDate );
            weekInterval.Length.Should().Be( daysInMonth );
        }

        [Fact]
        public void GetContainingMonthInterval_SomeDayOfMonth()
        {
            // Arrange
            var localDate = Fixture.Create<LocalDate>();
            var daysInMonth = localDate.Calendar.GetDaysInMonth( localDate.Year, localDate.Month );

            // Act
            var weekInterval = localDate.GetContainingMonthInterval();

            // Assert
            weekInterval.Start.Should().Be( localDate.StartOfMonth() );
            weekInterval.End.Should().Be( localDate.EndOfMonth() );
            weekInterval.Length.Should().Be( daysInMonth );
        }

        #endregion GetContainingMonthInterval

        #region NextOccurrenceOfDate

        [Fact]
        public void NextOccurrenceOfDate_Yesterday_NextYear()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var today = date.NextDay();

            // Act
            var nextOccurrence = date.NextOccurrenceOfDate( today );

            // Assert
            nextOccurrence.Should().Be( date + Period.FromYears( 1 ) );
        }

        [Fact]
        public void NextOccurrenceOfDate_Today_Today()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var today = date;

            // Act
            var nextOccurrence = date.NextOccurrenceOfDate( today );

            // Assert
            nextOccurrence.Should().Be( date );
        }

        [Fact]
        public void NextOccurrenceOfDate_Tomorrow_Tomorrow()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var today = date.PreviousDay();

            // Act
            var nextOccurrence = date.NextOccurrenceOfDate( today );

            // Assert
            nextOccurrence.Should().Be( date );
        }

        [Fact]
        public void NextOccurrenceOfDate_AlreadyThisYear_NextYear()
        {
            // Arrange
            var (date, today) = Fixture.Create<LocalDate>( ( x, y ) => x.Year == y.Year && x < y );

            // Act
            var nextOccurrence = date.NextOccurrenceOfDate( today );

            // Assert
            nextOccurrence.Should().Be( date + Period.FromYears( 1 ) );
        }

        [Fact]
        public void NextOccurrenceOfDate_IsLaterSameYear_Date()
        {
            // Arrange
            var (date, today) = Fixture.Create<LocalDate>( ( x, y ) => x.Year == y.Year && x > y );

            // Act
            var nextOccurrence = date.NextOccurrenceOfDate( today );

            // Assert
            nextOccurrence.Should().Be( date );
        }

        [Fact]
        public void NextOccurrenceOfDate_FutureDate_DateThisYear()
        {
            // Arrange
            var (date, today) = Fixture.Create<LocalDate>( ( x, y ) => y < x && y.Year < x.Year );
            var nextOccurrenceIsSameYear = date.Month < today.Month && date.Day < today.Day;

            // Act
            var nextOccurrence = date.NextOccurrenceOfDate( today );

            // Assert
            Period.Between( nextOccurrence, today, PeriodUnits.Years ).Years.Should().Be( 0 );
            // TODO: How does it work with leap days?
        }

        [Fact]
        public void NextOccurrenceOfDate_February29_February28()
        {
            // Arrange
            var leapYear = Fixture.Create<LocalDate>( date => date.Calendar.IsLeapYear( date.Year ) ).Year;
            var leapDate = new LocalDate( leapYear, 02, 29 );
            var today = leapDate.NextDay();
            var actualNextOccurrence = new LocalDate( leapYear + 1, 02, 28 );

            // Act
            var nextOccurrence = leapDate.NextOccurrenceOfDate( today );

            // Assert
            nextOccurrence.Should().Be( actualNextOccurrence );
        }

        #endregion NextOccurrenceOfDate

        #region ToAnnualDate

        [Fact]
        public void ToAnnualDate_RandomDate()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();

            // Act
            var annualDate = date.ToAnnualDate();

            // Assert
            annualDate.Month.Should().Be( date.Month );
            annualDate.Day.Should().Be( date.Day );
        }

        #endregion ToAnnualDate

        #region IsOnAnnualDate

        [Fact]
        public void IsOnAnnualDate_DifferentAnnualDates()
        {
            // Arrange
            var (annualDate1, annualDate2) = Fixture.Create<AnnualDate>((x, y) => x != y);
            var date = annualDate1.InYear(2020);

            // Act
            var isOnAnnualDate = date.IsOnAnnualDate(annualDate2);

            // Assert
            isOnAnnualDate.Should().BeFalse();
        }

        [Fact]
        public void IsOnAnnualDate_Same_True()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var annualDate = date.ToAnnualDate();

            // Act
            var isOnAnnualDate = date.IsOnAnnualDate(annualDate);

            // Assert
            isOnAnnualDate.Should().BeTrue();
        }

        [Fact]
        public void IsOnAnnualDate_Feb29OnNonLeapYear()
        {
            // Arrange
            var date = new LocalDate(2019, 02, 28);
            var annualDate = new AnnualDate(02, 29);

            // Act
            var isOnAnnualDate = date.IsOnAnnualDate(annualDate);

            // Assert
            isOnAnnualDate.Should().BeTrue();
        }

        [Fact]
        public void IsOnAnnualDate_Feb28OnNonLeapYear()
        {
            // Arrange
            var date = new LocalDate(2019, 02, 28);
            var annualDate = new AnnualDate(02, 28);

            // Act
            var isOnAnnualDate = date.IsOnAnnualDate(annualDate);

            // Assert
            isOnAnnualDate.Should().BeTrue();
        }

        [Fact]
        public void IsOnAnnualDate_Feb29OnLeapYear()
        {
            // Arrange
            var date = new LocalDate(2020, 02, 29);
            var annualDate = new AnnualDate(02, 29);

            // Act
            var isOnAnnualDate = date.IsOnAnnualDate(annualDate);

            // Assert
            isOnAnnualDate.Should().BeTrue();
        }

        [Fact]
        public void IsOnAnnualDate_Feb28OnLeapYear()
        {
            // Arrange
            var date = new LocalDate(2020, 02, 28);
            var annualDate = new AnnualDate(02, 29);

            // Act
            var isOnAnnualDate = date.IsOnAnnualDate(annualDate);

            // Assert
            isOnAnnualDate.Should().BeFalse();
        }

        #endregion ToAnnualDate
    }
}
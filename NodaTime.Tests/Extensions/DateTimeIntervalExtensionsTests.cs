using System;
using AutoFixture;
using FluentAssertions;
using NodaTime;
using NodaTime.Tests.AutoFixture;
using NodaTime.AutoFixture;
using Xunit;
using NodaTime.Extensions;

namespace NodaTime.Tests.Extensions
{
    public class DateTimeIntervalExtensionsTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        #region InZone

        [Fact]
        public void InZone_NullDateTimeInterval_ThrowsException()
        {
            // Arrange
            DateTimeInterval dateTimeInterval = null;
            var timeZone = Fixture.Create<DateTimeZone>();

            // Act
            Action act = () => dateTimeInterval.InZone(timeZone);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null.\r\nParameter name: dateTimeInterval");
        }

        [Fact]
        public void InZone_NullTimeZone_ThrowsException()
        {
            // Arrange
            var dateTimeInterval = Fixture.Create<DateTimeInterval>();
            DateTimeZone timeZone = null;

            // Act
            Action act = () => dateTimeInterval.InZone(timeZone);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: timeZone");
        }

        [Fact]
        public void InZone_InUtc_SameInterval()
        {
            // Arrange
            var dateTimeInterval = Fixture.Create<DateTimeInterval>();
            var timeZone = DateTimeZone.Utc;

            // Act
            var interval = dateTimeInterval.InZone(timeZone);

            // Arrange
            interval.Start.Should().Be(dateTimeInterval.Start.InUtc().ToInstant());
            interval.End.Should().Be(dateTimeInterval.End.InUtc().ToInstant());
        }

        [Fact]
        public void InZone_SinglePointDateTimeInterval_SinglePointInterval()
        {
            // Arrange
            var dateTime = Fixture.Create<LocalDateTime>();
            var dateTimeInterval = new DateTimeInterval(dateTime, dateTime);
            var timeZone = new FixedOffsetDateTimeZone(Offset.FromHours(3));

            // Act
            var interval = dateTimeInterval.InZone(timeZone);

            // Assert
            interval.Start.Should().Be(interval.End);
            interval.Start.Should().Be(timeZone.AtLeniently(dateTime).ToInstant());
        }

        [Fact]
        public void InZone_InSomeTimeZone()
        {
            // Arrange
            var dateTimeInterval = Fixture.Create<DateTimeInterval>();
            var timeZone = Fixture.Create<DateTimeZone>();

            // Act
            var interval = dateTimeInterval.InZone(timeZone);

            // Arrange
            interval.Start.Should().Be(timeZone.AtLeniently(dateTimeInterval.Start).ToInstant());
            interval.End.Should().Be(timeZone.AtLeniently(dateTimeInterval.End).ToInstant());
        }

        #endregion InZone

        #region ToDateInterval

        [Fact]
        public void ToDateInterval_SingleDateTimeInterval_SingleDayInterval()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var dateTimeInterval = date.ToSingleDayDateTimeInterval();

            // Act
            var dateInterval = dateTimeInterval.ToDateInterval();

            // Assert

            dateInterval.Should().Be(date.ToSingleDayInterval());
        }

        [Fact]
        public void ToDateInterval_SinglePointInterval_SingleDate()
        {
            // Arrange
            var dateTime = Fixture.Create<LocalDateTime>(x => x.TimeOfDay != LocalTime.Midnight);
            var dateTimeInterval = dateTime.ToSinglePointInterval();

            // Act
            var dateInterval = dateTimeInterval.ToDateInterval();

            // Assert
            dateInterval.Should().Be(dateTime.Date.ToSingleDayInterval());
        }

        [Fact]
        public void ToDateInterval_SinglePointIntervalAtMidnight_SingleDate()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var dateTimeInterval = date.AtMidnight().ToSinglePointInterval();

            // Act
            var dateInterval = dateTimeInterval.ToDateInterval();

            // Assert
            dateInterval.Should().Be(date.ToSingleDayInterval());
        }

        [Fact]
        public void ToDateInterval_MultiDayMidnightInterval_MultiDateInterval()
        {
            // Arrange
            var expectedDateInterval = Fixture.Create<DateInterval>();
            var dateTimeInterval = expectedDateInterval.ToDateTimeInterval();

            // Act
            var dateInterval = dateTimeInterval.ToDateInterval();

            // Assert
            dateInterval.Should().Be(expectedDateInterval);
        }

        [Fact]
        public void ToDateInterval_MultiDayDateInterval_MultiDateInterval()
        {
            // Arrange
            var dateTimeInterval = Fixture.Create<DateTimeInterval>(x => x.Start.TimeOfDay != LocalTime.Midnight && x.End.TimeOfDay != LocalTime.Midnight);
            var expectedDateInterval = new DateInterval(dateTimeInterval.Start.Date, dateTimeInterval.End.Date);

            // Act
            var dateInterval = dateTimeInterval.ToDateInterval();

            // Assert
            dateInterval.Should().Be(expectedDateInterval);
        }

        #endregion ToDateInterval

        #region IsSinglePointInterval

        [Fact]
        public void IsSinglePointInterval_Null()
        {
            // Arrange
            DateTimeInterval interval = null;

            // Act
            Action act = () => interval.IsSinglePointInterval();

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: dateTimeInterval");
        }

        [Fact]
        public void IsSinglePointInterval_NonSinglePoint()
        {
            // Arrange
            var (start, end) = Fixture.Create<LocalDateTime>((x, y) => x < y);
            var interval = new DateTimeInterval(start, end);

            // Act
            var isSinglePointInterval = interval.IsSinglePointInterval();

            // Assert
            isSinglePointInterval.Should().BeFalse();
        }

        [Fact]
        public void IsSinglePointInterval_SinglePoint()
        {
            // Arrange
            var localDateTime = Fixture.Create<LocalDateTime>();
            var interval = new DateTimeInterval(localDateTime, localDateTime);

            // Act
            var isSinglePointInterval = interval.IsSinglePointInterval();

            // Assert
            isSinglePointInterval.Should().BeTrue();
        }

        #endregion IsSinglePointInterval

        #region GetOverlapWith

        [Fact]
        public void Overlap_NullFirst_ThrowsException()
        {
            // Arrange
            DateTimeInterval first = null;
            var second = Fixture.Create<DateTimeInterval>();

            // Act
            Action act = () => first.GetOverlapWith(second);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: first");
        }

        [Fact]
        public void Overlap_NullSecond_ThrowsException()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();

            // Act
            Action act = () => interval.GetOverlapWith(null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: second");
        }

        [Fact]
        public void Overlap_DifferentCalendars_ThrowsException()
        {
            // Arrange
            var first = Fixture.Create<DateTimeInterval>();
            var (start, end) = Fixture.Create<LocalDateTime>((x, y) => x <= y);
            var second = new DateTimeInterval(start.WithCalendar(CalendarSystem.Julian), end.WithCalendar(CalendarSystem.Julian));

            // Act
            Action act = () => first.GetOverlapWith(second);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage("The given interval must be in the same calendar as this interval.\r\nParameter name: second");
        }

        [Fact]
        public void Overlap_Before_Null()
        {
            // Arrange
            var (first, second) = Fixture.Create<DateTimeInterval>((x, y) => x.End < y.Start || y.End < x.Start);

            // Act
            var overlap = first.GetOverlapWith(second);
            var overlapInverted = second.GetOverlapWith(first);

            // Assert
            overlap.Should().BeNull();
            overlapInverted.Should().BeNull();
        }

        [Fact]
        public void Overlap_Meets_Null()
        {
            // Arrange
            var (first, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>((x, y) => x.End < y);
            var second = new DateTimeInterval(first.End, dateTime);

            // Act
            var overlap = first.GetOverlapWith(second);
            var overlapInverted = second.GetOverlapWith(first);

            // Assert
            overlap.Should().BeNull();
            overlapInverted.Should().BeNull();
        }

        [Fact]
        public void Overlap_Overlaps_Overlap()
        {
            // Arrange
            var (first, second) = Fixture.Create<DateTimeInterval>((x, y) => x.Start < y.Start && y.Start < x.End && x.End < y.End);
            var expectedOverlap = new DateTimeInterval(second.Start, first.End);

            // Act
            var overlap = first.GetOverlapWith(second);
            var overlapInverted = second.GetOverlapWith(first);

            // Assert
            overlap.Should().Be(expectedOverlap);
            overlapInverted.Should().Be(expectedOverlap);
        }

        [Fact]
        public void Overlap_Starts_StartingInterval()
        {
            // Arrange
            var (first, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>((x, y) => x.End < y);
            var second = new DateTimeInterval(first.Start, dateTime);

            // Act
            var overlap = first.GetOverlapWith(second);
            var overlapInverted = second.GetOverlapWith(first);

            // Assert
            overlap.Should().Be(first);
            overlapInverted.Should().Be(first);
            ReferenceEquals(overlap, first).Should().BeFalse();
            ReferenceEquals(overlapInverted, first).Should().BeFalse();
        }

        [Fact]
        public void Overlap_Contains_InnerInterval()
        {
            // Arrange
            var (first, second) = Fixture.Create<DateTimeInterval>((x, y) => x.Start < y.Start && y.End < x.End);

            // Act
            var overlap = first.GetOverlapWith(second);
            var overlapInverted = second.GetOverlapWith(first);

            // Assert
            overlap.Should().Be(second);
            overlapInverted.Should().Be(second);
            ReferenceEquals(overlap, second).Should().BeFalse();
            ReferenceEquals(overlapInverted, second).Should().BeFalse();
        }

        [Fact]
        public void Overlap_Finishes_FinishingInterval()
        {
            // Arrange
            var (dateTime, first) = Fixture.Create<LocalDateTime, DateTimeInterval>((d, i) => d < i.Start);
            var second = new DateTimeInterval(dateTime, first.End);

            // Act
            var overlap = first.GetOverlapWith(second);
            var overlapInverted = second.GetOverlapWith(first);

            // Assert
            overlap.Should().Be(first);
            overlapInverted.Should().Be(first);
            ReferenceEquals(overlap, first).Should().BeFalse();
            ReferenceEquals(overlapInverted, first).Should().BeFalse();
        }

        [Fact]
        public void Overlap_Equal_Interval()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();

            // Act
            var overlap = interval.GetOverlapWith(interval);

            // Assert
            overlap.Should().Be(interval);
            ReferenceEquals(overlap, interval).Should().BeFalse();
        }

        #region SinglePoint interval

        [Fact]
        public void Overlap_SinglePointBefore_Null()
        {
            // Arrange
            var (dateTime, interval) = Fixture.Create<LocalDateTime, DateTimeInterval>((x, y) => x < y.Start);
            var singlePointInterval = dateTime.ToSinglePointInterval();

            // Act
            var overlap = singlePointInterval.GetOverlapWith(interval);
            var overlapInverted = interval.GetOverlapWith(singlePointInterval);

            // Assert
            overlap.Should().BeNull();
            overlapInverted.Should().BeNull();
        }

        [Fact]
        public void Overlap_SinglePointStarts_SinglePointInterval()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();
            var singlePointInterval = interval.Start.ToSinglePointInterval();

            // Act
            var overlap = singlePointInterval.GetOverlapWith(interval);
            var overlapInverted = interval.GetOverlapWith(singlePointInterval);

            // Assert
            overlap.Should().Be(singlePointInterval);
            overlapInverted.Should().Be(singlePointInterval);
            ReferenceEquals(overlap, singlePointInterval).Should().BeFalse();
            ReferenceEquals(overlapInverted, singlePointInterval).Should().BeFalse();
        }

        [Fact]
        public void Overlap_SinglePointContains_SinglePointInterval()
        {
            // Arrange
            var (interval, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>((x, y) => x.Start < y && y < x.End);
            var singlePointInterval = dateTime.ToSinglePointInterval();

            // Act
            var overlap = singlePointInterval.GetOverlapWith(interval);
            var overlapInverted = interval.GetOverlapWith(singlePointInterval);

            // Assert
            overlap.Should().Be(singlePointInterval);
            overlapInverted.Should().Be(singlePointInterval);
            ReferenceEquals(overlap, singlePointInterval).Should().BeFalse();
            ReferenceEquals(overlapInverted, singlePointInterval).Should().BeFalse();
        }

        [Fact]
        public void Overlap_Ends_Null()
        {
            // Arrange
            var interval = Fixture.Create<DateTimeInterval>();
            var singlePointInterval = interval.End.ToSinglePointInterval();

            // Act
            var overlap = singlePointInterval.GetOverlapWith(interval);
            var overlapInverted = interval.GetOverlapWith(singlePointInterval);

            // Assert
            overlap.Should().BeNull();
            overlapInverted.Should().BeNull();
        }

        [Fact]
        public void Overlap_SinglePointAfter_Null()
        {
            // Arrange
            var (interval, dateTime) = Fixture.Create<DateTimeInterval, LocalDateTime>((x, y) => x.End < y);
            var singlePointInterval = dateTime.ToSinglePointInterval();

            // Act
            var overlap = singlePointInterval.GetOverlapWith(interval);
            var overlapInverted = interval.GetOverlapWith(singlePointInterval);

            // Assert
            overlap.Should().BeNull();
            overlapInverted.Should().BeNull();
        }

        [Fact]
        public void Overlap_SinglePointEqual_SinglePointInterval()
        {
            // Arrange
            var dateTime = Fixture.Create<LocalDateTime>();
            var singlePointInterval1 = dateTime.ToSinglePointInterval();
            var singlePointInterval2 = dateTime.ToSinglePointInterval();

            // Act
            var overlap = singlePointInterval1.GetOverlapWith(singlePointInterval2);
            var overlapInverted = singlePointInterval1.GetOverlapWith(singlePointInterval2);

            // Assert
            overlap.Should().Be(singlePointInterval1).And.Be(singlePointInterval2);
            overlapInverted.Should().Be(singlePointInterval1).And.Be(singlePointInterval2);
            ReferenceEquals(overlap, singlePointInterval1).Should().BeFalse();
            ReferenceEquals(overlap, singlePointInterval2).Should().BeFalse();
            ReferenceEquals(overlapInverted, singlePointInterval1).Should().BeFalse();
            ReferenceEquals(overlapInverted, singlePointInterval2).Should().BeFalse();
        }

        [Fact]
        public void Overlap_SinglePointUnequal_Null()
        {
            // Arrange
            var (dateTime1, dateTime2) = Fixture.Create<LocalDateTime>((x, y) => x != y);
            var singlePointInterval1 = dateTime1.ToSinglePointInterval();
            var singlePointInterval2 = dateTime2.ToSinglePointInterval();

            // Act
            var overlap = singlePointInterval1.GetOverlapWith(singlePointInterval2);
            var overlapInverted = singlePointInterval2.GetOverlapWith(singlePointInterval1);

            // Assert
            overlap.Should().BeNull();
            overlapInverted.Should().BeNull();
        }

        #endregion

        #endregion

        #region Overlaps

        [Fact]
        public void Overlaps_NullDateTimeInterval_ThrowsException()
        {
            // Arrange
            DateTimeInterval dateTimeInterval = null;
            var timeInterval = Fixture.Create<TimeInterval>();

            // Act
            Action act = () => timeInterval.Overlaps(dateTimeInterval);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Should().Contain(nameof(dateTimeInterval));
        }

        [Fact]
        public void Overlaps_NullTimeInterval_ThrowsException()
        {
            // Arrange
            var dateTimeInterval = Fixture.Create<DateTimeInterval>();
            TimeInterval timeInterval = null;

            // Act
            Action act = () => timeInterval.Overlaps(dateTimeInterval);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Should().Contain(nameof(timeInterval));
        }

        public static TheoryData<Period> OverlapLongerThanADayExamples = new TheoryData<Period>
        {
            Period.FromYears(1),
            Period.FromMonths(1),
            Period.FromWeeks(1),
            Period.FromHours(24),
            Period.FromDays(1) + Period.FromNanoseconds(1),
            Period.FromDays(1),
            Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(24)
        };

        [Theory]
        [MemberData(nameof(OverlapLongerThanADayExamples))]
        public void Overlaps_LongerThanADay_True(Period period)
        {
            // Arrange
            var startDateTime = Fixture.Create<LocalDateTime>();
            var dateTimeInterval = new DateTimeInterval(startDateTime, period);
            var timeInterval = Fixture.Create<TimeInterval>();

            // Act
            var overlaps = timeInterval.Overlaps(dateTimeInterval);

            // Assert
            overlaps.Should().BeTrue();
        }

        [Fact]
        public void Overlaps_SingleDateInterval_True()
        {
            // Arrange
            var dateTimeInterval = Fixture.Create<LocalDate>().ToSingleDayDateTimeInterval();
            var timeInterval = Fixture.Create<TimeInterval>();

            // Act
            var overlaps = timeInterval.Overlaps(dateTimeInterval);

            // Assert
            overlaps.Should().BeTrue();
        }

        public static LocalTime Time0 = LocalTime.Midnight;
        public static LocalTime Time1 = new LocalTime(02, 24, 14);
        public static LocalTime Time2 = new LocalTime(05, 55, 27);
        public static LocalTime Time3 = new LocalTime(08, 00);
        public static LocalTime Time4 = new LocalTime(09, 57, 44);
        public static LocalTime Time5 = new LocalTime(13, 44, 31);
        public static LocalTime Time6 = new LocalTime(16, 00);
        public static LocalTime Time7 = new LocalTime(18, 07, 08);
        public static LocalTime Time8 = new LocalTime(21, 27, 13);

        public static TheoryData<LocalTime, LocalTime, LocalTime, bool> OverlapEmptyIntervalExamples = new TheoryData<LocalTime, LocalTime, LocalTime, bool>
        {
            { Time0, Time0, Time2, true  },
            { Time0, Time4, Time5, false  },
            { Time0, Time5, Time0, false  },
            { Time0, Time5, Time2, true },
            { Time0, Time0, Time0, true },
            { Time0, Time4, Time4, true },

            { Time3, Time0, Time0, true  },
            { Time3, Time0, Time2, false },
            { Time3, Time0, Time3, false },
            { Time3, Time0, Time4, true  },

            { Time3, Time1, Time0, true  },
            { Time3, Time1, Time1, true  },
            { Time3, Time1, Time2, false },
            { Time3, Time1, Time3, false },
            { Time3, Time1, Time4, true  },

            { Time3, Time2, Time1, true  },

            { Time3, Time3, Time3, true  },
            { Time3, Time3, Time4, true  },
            { Time3, Time3, Time0, true  },
            { Time3, Time3, Time2, true  },

            { Time3, Time4, Time0, false },
            { Time3, Time4, Time2, false },
            { Time3, Time4, Time3, false },
            { Time3, Time4, Time4, true  },
            { Time3, Time4, Time5, false },

            { Time3, Time5, Time4, true  }
        };

        [Theory]
        [MemberData(nameof(OverlapEmptyIntervalExamples))]
        public void Overlaps_EmptyInterval(LocalTime emptyIntervalTime, LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var dateTimeInterval = date.At(emptyIntervalTime).ToSinglePointInterval();
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.Overlaps(dateTimeInterval);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> OverlapsFromMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time6, true },
            { Time0, Time7, true },
            { Time0, Time0, true },
            { Time0, Time1, true },
            { Time0, Time3, true },
            { Time0, Time5, true },

            { Time1, Time6, true },
            { Time1, Time7, true },
            { Time1, Time0, true },
            { Time1, Time1, true },
            { Time1, Time2, true },
            { Time1, Time3, true },
            { Time1, Time5, true },

            { Time2, Time1, true },

            { Time3, Time6, false },
            { Time3, Time7, false },
            { Time3, Time0, false },
            { Time3, Time1, true },
            { Time3, Time3, true },
            { Time3, Time5, false },

            { Time4, Time6, false },
            { Time4, Time8, false },
            { Time4, Time0, false },
            { Time4, Time1, true },
            { Time4, Time3, true },
            { Time4, Time4, true },
            { Time4, Time5, false },

            { Time5, Time4, true },

            { Time6, Time6, true },
            { Time6, Time8, false },
            { Time6, Time0, false },
            { Time6, Time1, true },
            { Time6, Time3, true },
            { Time6, Time4, true },

            { Time7, Time6, true },
            { Time7, Time7, true },
            { Time7, Time8, false },
            { Time7, Time0, false },
            { Time7, Time1, true },
            { Time7, Time3, true },
            { Time7, Time5, true },

            { Time8, Time7, true }
        };

        [Theory]
        [MemberData(nameof(OverlapsFromMidnightExamples))]
        public void Overlaps_FromMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var interval1 = new TimeInterval(Time0, Time3);
            var interval2 = date.At(startTime, endTime);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> DuringDayExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time2, false },
            { Time0, Time3, false },
            { Time0, Time4, true },
            { Time0, Time6, true },
            { Time0, Time7, true },

            { Time1, Time0, true },
            { Time1, Time1, true },
            { Time1, Time2, false },
            { Time1, Time3, false },
            { Time1, Time4, true },
            { Time1, Time6, true },
            { Time1, Time8, true },

            { Time2, Time1, true },

            { Time3, Time0, true },
            { Time3, Time1, true },
            { Time3, Time3, true },
            { Time3, Time4, true },
            { Time3, Time6, true },
            { Time3, Time8, true },

            { Time4, Time0, true },
            { Time4, Time1, true },
            { Time4, Time3, true },
            { Time4, Time4, true },
            { Time4, Time5, true },
            { Time4, Time6, true },
            { Time4, Time8, true },

            { Time5, Time4, true },

            { Time6, Time0, false },
            { Time6, Time1, false },
            { Time6, Time3, false },
            { Time6, Time4, true },
            { Time6, Time6, true },
            { Time6, Time8, false },

            { Time7, Time0, false },
            { Time7, Time2, false },
            { Time7, Time3, false },
            { Time7, Time4, true },
            { Time7, Time6, true },
            { Time7, Time7, true },
            { Time7, Time8, false },

            { Time8, Time7, true }
        };

        [Theory]
        [MemberData(nameof(DuringDayExamples))]
        public void Overlaps_DuringDay(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var interval1 = new TimeInterval(Time3, Time6);
            var interval2 = date.At(startTime, endTime);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> UntilMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time3, false },
            { Time0, Time4, false },
            { Time0, Time6, false },
            { Time0, Time7, true },
            { Time0, Time0, true },
            { Time0, Time2, false },

            { Time1, Time3, false },
            { Time1, Time5, false },
            { Time1, Time6, false },
            { Time1, Time7, true },
            { Time1, Time0, true },
            { Time1, Time1, true },
            { Time1, Time2, false },

            { Time2, Time1, true },

            { Time3, Time3, true },
            { Time3, Time5, false },
            { Time3, Time6, false },
            { Time3, Time7, true },
            { Time3, Time0, true },
            { Time3, Time1, true },

            { Time4, Time3, true },
            { Time4, Time4, true },
            { Time4, Time5, false },
            { Time4, Time6, false },
            { Time4, Time7, true },
            { Time4, Time0, true },
            { Time4, Time2, true },

            { Time5, Time4, true },

            { Time6, Time3, true },
            { Time6, Time4, true },
            { Time6, Time6, true },
            { Time6, Time7, true },
            { Time6, Time0, true },
            { Time6, Time2, true },

            { Time7, Time3, true },
            { Time7, Time4, true },
            { Time7, Time6, true },
            { Time7, Time7, true },
            { Time7, Time8, true },
            { Time7, Time0, true },
            { Time7, Time2, true },

            { Time8, Time7, true }
        };

        [Theory]
        [MemberData(nameof(UntilMidnightExamples))]
        public void Overlaps_UntilMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var interval1 = new TimeInterval(Time6, Time0);
            var interval2 = date.At(startTime, endTime);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> OverlapsOverMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time2, true },
            { Time0, Time3, true },
            { Time0, Time4, true },
            { Time0, Time6, true },
            { Time0, Time7, true },
            { Time1, Time0, true },
            { Time1, Time1, true },
            { Time1, Time2, true },
            { Time1, Time3, true },
            { Time1, Time4, true },
            { Time1, Time6, true },
            { Time1, Time7, true },
            { Time2, Time1, true },
            { Time3, Time0, true },
            { Time3, Time2, true },
            { Time3, Time3, true },
            { Time3, Time5, false },
            { Time3, Time6, false },
            { Time3, Time8, true },
            { Time4, Time0, true },
            { Time4, Time2, true },
            { Time4, Time3, true },
            { Time4, Time4, true },
            { Time4, Time5, false },
            { Time4, Time6, false },
            { Time4, Time8, true },
            { Time5, Time4, true },
            { Time6, Time6, true },
            { Time6, Time7, true },
            { Time6, Time0, true },
            { Time6, Time2, true },
            { Time6, Time3, true },
            { Time6, Time4, true },
            { Time7, Time0, true },
            { Time7, Time1, true },
            { Time7, Time3, true },
            { Time7, Time5, true },
            { Time7, Time6, true },
            { Time7, Time7, true },
            { Time7, Time8, true },
            { Time8, Time7, true }
        };

        [Theory]
        [MemberData(nameof(OverlapsOverMidnightExamples))]
        public void Overlaps_OverMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var interval1 = new TimeInterval(Time6, Time3);
            var interval2 = date.At(startTime, endTime);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> MidnightToMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time3, true },
            { Time3, Time0, true },
            { Time3, Time2, true },
            { Time3, Time3, true },
            { Time3, Time7, true }
        };

        [Theory]
        [MemberData(nameof(MidnightToMidnightExamples))]
        public void Overlaps_MidnightToMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var interval1 = new TimeInterval(Time0, Time0);
            var interval2 = date.At(startTime, endTime);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> TimeToTimeExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time1, true },
            { Time0, Time3, true },
            { Time0, Time7, true },
            { Time1, Time0, true },
            { Time1, Time1, true },
            { Time1, Time2, true },
            { Time1, Time3, true },
            { Time1, Time5, true },
            { Time2, Time1, true },
            { Time3, Time3, true },
            { Time3, Time5, true },
            { Time3, Time0, true },
            { Time3, Time2, true },
            { Time4, Time5, true },
            { Time5, Time0, true },
            { Time5, Time2, true },
            { Time5, Time3, true },
            { Time5, Time4, true },
            { Time5, Time5, true },
            { Time5, Time8, true }
        };

        [Theory]
        [MemberData(nameof(TimeToTimeExamples))]
        public void Overlaps_TimeToTime(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var interval1 = new TimeInterval(Time3, Time3);
            var interval2 = date.At(startTime, endTime);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        #endregion

        #region ToTimeInterval

        [Fact]
        public void ToTimeInterval_Null_ThrowsException()
        {
            // Arrange
            DateTimeInterval interval = null;

            // Act
            Action act = () => interval.ToTimeInterval();

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Should().Contain(nameof(interval));
        }

        [Fact]
        public void ToTimeInterval_LongerThanADay_ThrowsException()
        {
            // Arrange
            var startDateTime = Fixture.Create<LocalDateTime>();
            var period = Period.FromDays(1) + Period.FromNanoseconds(1);
            var interval = new DateTimeInterval(startDateTime, period);

            // Act
            Action act = () => interval.ToTimeInterval();

            // Assert
            act.Should().Throw<NotSupportedException>().And.Message.Should().Contain(nameof(interval));
        }

        [Fact]
        public void ToTimeInterval_SingleDateInterval_MidnightToMidnight()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var interval = date.ToSingleDayDateTimeInterval();

            // Act
            var timeInterval = interval.ToTimeInterval();

            // Assert
            timeInterval.Start.Should().Be(LocalTime.Midnight);
            timeInterval.End.Should().Be(LocalTime.Midnight);
        }

        [Fact]
        public void ToTimeInterval_OneDayLongInterval_OneDayTimeInterval()
        {
            // Arrange
            var startDateTime = Fixture.Create<LocalDateTime>();
            var period = Period.FromDays(1);
            var interval = new DateTimeInterval(startDateTime, period);
            var time = startDateTime.TimeOfDay;

            // Act
            var timeInterval = interval.ToTimeInterval();

            // Assert
            timeInterval.Start.Should().Be(time);
            timeInterval.End.Should().Be(time);
        }

        [Fact]
        public void ToTimeInterval_SameDayStartAndEnd_SameTimeInterval()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var (startTime, endTime) = Fixture.Create<LocalTime>((x, y) => x < y);
            var interval = date.At(startTime, endTime);

            // Act
            var timeInterval = interval.ToTimeInterval();

            // Assert
            timeInterval.Start.Should().Be(startTime);
            timeInterval.End.Should().Be(endTime);
        }

        [Fact]
        public void ToTimeInterval_OverMidnightInterval()
        {
            // Arrange
            var date = Fixture.Create<LocalDate>();
            var (startTime, endTime) = Fixture.Create<LocalTime>((x, y) => x >= y);
            var interval = date.At(startTime, endTime);

            // Act
            var timeInterval = interval.ToTimeInterval();

            // Assert
            timeInterval.Start.Should().Be(startTime);
            timeInterval.End.Should().Be(endTime);
        }

        [Fact]
        public void ToTimeInterval_EmptyInterval_ThrowsException()
        {
            // Arrange
            var interval = Fixture.Create<LocalDateTime>().ToSinglePointInterval();

            // Act
            Action act = () => interval.ToTimeInterval();

            // Assert
            act.Should().Throw<NotSupportedException>().And.Message.Should().Contain(nameof(interval));
        }

        #endregion
    }
}
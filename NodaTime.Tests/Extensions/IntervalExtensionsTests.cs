using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NodaTime.Testing.TimeZones;
using NodaTime.Tests.AutoFixture;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class IntervalExtensionsTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        private static Instant Instant1 = Instant.FromUtc(2018, 01, 01, 03, 34, 56);
        private static Instant Instant2 = Instant.FromUtc(2018, 01, 05, 14, 49, 19);
        private static Instant Instant3 = Instant.FromUtc(2018, 01, 14, 12, 19, 02);
        private static Instant Instant4 = Instant.FromUtc(2018, 01, 16, 04, 07, 57);

        #region InZoneStrictly

        [Fact]
        public void InZoneStrictly_ValidInterval()
        {
            // Arrange
            var timeZone = Fixture.Create<FixedOffsetDateTimeZone>();
            var interval = Fixture.Create<Interval>();
            var startDateTime = interval.Start.InZone( timeZone ).LocalDateTime;
            var endDateTime = interval.End.InZone( timeZone ).LocalDateTime;

            // Act
            var dateTimeInterval = interval.InZoneStrictly( timeZone );

            // Assert
            dateTimeInterval.Start.Should().Be( startDateTime );
            dateTimeInterval.End.Should().Be( endDateTime );
        }

        [Fact]
        public void InZoneStrictly_InvalidInterval()
        {
            // Arrange
            var instant = Fixture.Create<Instant>();
            var timeZone = new SingleTransitionDateTimeZone( instant, 5, 4 );
            var start = instant - Duration.FromMinutes( 15 );
            var end = instant + Duration.FromMinutes( 15 );
            var interval = new Interval( start, end );

            // Act
            Action act = () => interval.InZoneStrictly( timeZone );

            // Assert
            act.Should().ThrowExactly<InvalidLocalDateTimeInterval>();
        }

        [Fact]
        public void InZoneStrictly_SinglePointInterval_SinglePointDateTimeInterval()
        {
            // Arrange
            var timeZone = Fixture.Create<DateTimeZone>();
            var instant = Fixture.Create<Instant>();
            var interval = new Interval( instant, instant );
            var dateTime = instant.InZone( timeZone ).LocalDateTime;

            // Act
            var dateTimeInterval = interval.InZoneStrictly( timeZone );

            // Assert
            dateTimeInterval.Should().Be( dateTime.ToSinglePointInterval() );
        }

        #endregion

        #region GetOverlapWith

        public static TheoryData<string, Interval, Interval, Interval?> OverlapExamples = new TheoryData<string, Interval, Interval, Interval?>
            {
            { "Before          ", new Interval(Instant1, Instant2), new Interval(Instant3, Instant4), null },
            { "After           ", new Interval(Instant3, Instant4), new Interval(Instant1, Instant2), null },
            { "Meets           ", new Interval(Instant1, Instant2), new Interval(Instant2, Instant3), null },
            { "Is met by       ", new Interval(Instant2, Instant3), new Interval(Instant1, Instant2), null },
            { "Overlaps        ", new Interval(Instant1, Instant3), new Interval(Instant2, Instant4), new Interval(Instant2, Instant3) },
            { "Is overlapped by", new Interval(Instant2, Instant4), new Interval(Instant1, Instant3), new Interval(Instant2, Instant3) },
            { "Starts          ", new Interval(Instant1, Instant2), new Interval(Instant1, Instant3), new Interval(Instant1, Instant2) },
            { "Is started by   ", new Interval(Instant1, Instant3), new Interval(Instant1, Instant2), new Interval(Instant1, Instant2) },
            { "During          ", new Interval(Instant2, Instant3), new Interval(Instant1, Instant4), new Interval(Instant2, Instant3) },
            { "Contains        ", new Interval(Instant1, Instant4), new Interval(Instant2, Instant3), new Interval(Instant2, Instant3) },
            { "Finishes        ", new Interval(Instant2, Instant3), new Interval(Instant1, Instant3), new Interval(Instant2, Instant3) },
            { "Is finished by  ", new Interval(Instant1, Instant3), new Interval(Instant2, Instant3), new Interval(Instant2, Instant3) },
            { "Equals          ", new Interval(Instant1, Instant2), new Interval(Instant1, Instant2), new Interval(Instant1, Instant2) }
        };

        [Theory]
        [MemberData(nameof(OverlapExamples))]
        public void OverlapForNonOverlappingIntervals(string name, Interval interval1, Interval interval2, Interval? expectedOverlap)
        {
            // Act
            var overlap = interval1.GetOverlapWith(interval2);

            // Assert
            overlap.Should().Be(expectedOverlap);
        }

        #endregion

        #region DurationBefore

        [Fact]
        public void DurationBefore_Before_NoResult()
        {
            // Arrange
            var (interval, instant) = Fixture.Create<Interval, Instant>((x, y) => y < x.Start);

            // Act
            var durationBefore = interval.DurationBefore(instant);

            // Assert
            durationBefore.HasValue.Should().BeFalse();
        }

        [Fact]
        public void DurationBefore_Start_Zero()
        {
            // Arrange
            var interval = Fixture.Create<Interval>();
            var instant = interval.Start;

            // Act
            var durationBefore = interval.DurationBefore(instant);

            // Assert
            durationBefore.Should().Be(Duration.Zero);
        }

        [Fact]
        public void DurationBefore_Between_DurationBefore()
        {
            // Arrange
            var (interval, instant) = Fixture.Create<Interval, Instant>((x, y) => x.Contains(y));
            var expected = instant - interval.Start;

            // Act
            var durationBefore = interval.DurationBefore(instant);

            // Assert
            durationBefore.Should().Be(expected);
        }

        [Fact]
        public void DurationBefore_End_IntervalLDuration()
        {
            // Arrange
            var interval = Fixture.Create<Interval>();
            var instant = interval.End;

            // Act
            var durationBefore = interval.DurationBefore(instant);

            // Assert
            durationBefore.Should().Be(interval.Duration);
        }

        [Fact]
        public void DurationBefore_After_IntervalLDuration()
        {
            // Arrange
            var (interval, instant) = Fixture.Create<Interval, Instant>((x, y) => x.End < y);

            // Act
            var durationBefore = interval.DurationBefore(instant);

            // Assert
            durationBefore.Should().Be(interval.Duration);
        }

        #endregion

        #region GetOverlapsWithDayInTimeZone

        public static TheoryData<Interval, IsoDayOfWeek, DateTimeZone, IEnumerable<Interval>> GetOverlapsWithDayInTimeZoneExamples = new TheoryData<Interval, IsoDayOfWeek, DateTimeZone, IEnumerable<Interval>>
        {
            {   // No overlap in given time zone
                new Interval(Instant.FromUtc(2018, 05, 11, 00, 00), Instant.FromUtc(2018, 05, 11, 04, 00)),
                IsoDayOfWeek.Friday,
                DateTimeZoneProviders.Tzdb["America/New_York"],
                Enumerable.Empty<Interval>()
            },
            {   // Cut short because interval begins Thursday
                new Interval(Instant.FromUtc(2018, 05, 11, 02, 00), Instant.FromUtc(2018, 05, 11, 10, 00)),
                IsoDayOfWeek.Friday,
                DateTimeZoneProviders.Tzdb["America/New_York"],
                new [] { new Interval(Instant.FromUtc(2018, 05, 11, 04, 00), Instant.FromUtc(2018, 05, 11, 10, 00)) }
            },
            {   // Remains unchanged because everything overlaps in given time zone
                new Interval(Instant.FromUtc(2018, 05, 11, 02, 00), Instant.FromUtc(2018, 05, 11, 10, 00)),
                IsoDayOfWeek.Friday,
                DateTimeZoneProviders.Tzdb["Europe/Copenhagen"],
                new [] { new Interval(Instant.FromUtc(2018, 05, 11, 02, 00), Instant.FromUtc(2018, 05, 11, 10, 00)) }
            },
            {   // Cut short because interval begins Saturday and day starts at 01:00 (Spring forward)
                new Interval(Instant.FromUtc(2018, 11, 04, 02, 15), Instant.FromUtc(2018, 11, 04, 06, 30)),
                IsoDayOfWeek.Sunday,
                DateTimeZoneProviders.Tzdb["America/Sao_Paulo"],
                new [] { new Interval(Instant.FromUtc(2018, 11, 04, 03, 00), Instant.FromUtc(2018, 11, 04, 06, 30)) }
            },
            {   // Cut short because interval begins Saturday and day starts later (Fall back)
                new Interval(Instant.FromUtc(2018, 02, 18, 01, 00), Instant.FromUtc(2018, 02, 18, 07, 30)),
                IsoDayOfWeek.Sunday,
                DateTimeZoneProviders.Tzdb["America/Sao_Paulo"],
                new [] { new Interval(Instant.FromUtc(2018, 02, 18, 03, 00), Instant.FromUtc(2018, 02, 18, 07, 30)) }
            },
            {   // Overlaps multiple times (middle overlap is during DST transition)
                new Interval(Instant.FromUtc(2018, 10, 28, 16, 15), Instant.FromUtc(2018, 11, 11, 12, 30)),
                IsoDayOfWeek.Sunday,
                DateTimeZoneProviders.Tzdb["America/New_York"],
                new []
                {
                    new Interval(Instant.FromUtc(2018, 10, 28, 16, 15), Instant.FromUtc(2018, 10, 29, 04, 00)),
                    new Interval(Instant.FromUtc(2018, 11, 04, 04, 00), Instant.FromUtc(2018, 11, 05, 05, 00)),
                    new Interval(Instant.FromUtc(2018, 11, 11, 05, 00), Instant.FromUtc(2018, 11, 11, 12, 30))
                }
            },
            {   // Results in an invalid date time interval
                new Interval(Instant.FromUtc(2018, 10, 28, 00, 45), Instant.FromUtc(2018, 10, 28, 01, 15)),
                IsoDayOfWeek.Sunday,
                DateTimeZoneProviders.Tzdb["Europe/Copenhagen"],
                new [] { new Interval(Instant.FromUtc(2018, 10, 28, 00, 45), Instant.FromUtc(2018, 10, 28, 01, 15)) }
            }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithDayInTimeZoneExamples))]
        public void GetOverlapsWithDayInTimeZone(Interval interval, IsoDayOfWeek dayOfWeek, DateTimeZone timeZone, IEnumerable<Interval> expected)
        {
            // Act
            var overlaps = interval.GetOverlapsWithDayInTimeZone(dayOfWeek, timeZone);

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        #endregion
    }
}
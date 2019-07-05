using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NodaTime;
using NodaTime.AutoFixture;
using Xunit;
using NodaTime.Extensions;

namespace NodaTime.Tests.Extensions
{
    public class TimeIntervalExtensionsTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        public static LocalTime Time0 = LocalTime.Midnight;
        public static LocalTime Time1 = new LocalTime(02, 24, 14);
        public static LocalTime Time2 = new LocalTime(05, 55, 27);
        public static LocalTime Time3 = new LocalTime(08, 00);
        public static LocalTime Time4 = new LocalTime(09, 57, 44);
        public static LocalTime Time5 = new LocalTime(13, 44, 31);
        public static LocalTime Time6 = new LocalTime(16, 00);
        public static LocalTime Time7 = new LocalTime(18, 07, 08);
        public static LocalTime Time8 = new LocalTime(21, 27, 13);

        private static readonly LocalDate Date = Fixture.Create<LocalDate>();

        private static readonly IEnumerable<TimeInterval> EmptyTimeIntervals = Enumerable.Empty<TimeInterval>();
        private static readonly IEnumerable<DateTimeInterval> EmptyDateTimeIntervals = Enumerable.Empty<DateTimeInterval>();

        #region Contains time interval

        [Fact]
        public void Contains_NullFirst_ThrowsException()
        {
            // Arrange
            TimeInterval interval1 = null;
            var interval2 = Fixture.Create<TimeInterval>();

            // Act
            Action act = () => interval1.Contains(interval2);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Contains_NullSecond_ThrowsException()
        {
            // Arrange
            var interval1 = Fixture.Create<TimeInterval>();
            TimeInterval interval2 = null;

            // Act
            Action act = () => interval1.Contains(interval2);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        public static TheoryData<LocalTime, LocalTime, bool> ContainsTimeIntervalFromMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time1, false },
            { Time0, Time3, true },
            { Time0, Time5, true },
            { Time0, Time6, true },
            { Time0, Time7, true },

            { Time1, Time0, false },
            { Time1, Time1, false },
            { Time1, Time2, false },
            { Time1, Time3, false },
            { Time1, Time5, false },
            { Time1, Time6, false },
            { Time1, Time7, false },

            { Time2, Time1, false },

            { Time3, Time0, false },
            { Time3, Time1, false },
            { Time3, Time3, true },
            { Time3, Time5, false },
            { Time3, Time6, false },
            { Time3, Time7, false },

            { Time4, Time0, false },
            { Time4, Time1, false },
            { Time4, Time3, true },
            { Time4, Time4, true },
            { Time4, Time5, false },
            { Time4, Time6, false },
            { Time4, Time8, false },

            { Time5, Time4, true },

            { Time6, Time0, false },
            { Time6, Time1, false },
            { Time6, Time3, true },
            { Time6, Time4, true },
            { Time6, Time6, true },
            { Time6, Time8, false },

            { Time7, Time0, false },
            { Time7, Time1, false },
            { Time7, Time3, true },
            { Time7, Time5, true },
            { Time7, Time6, true },
            { Time7, Time7, true },
            { Time7, Time8, false },

            { Time8, Time7, true }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeIntervalFromMidnightExamples))]
        public void Contains_FromMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time0, Time3);

            // Act
            var contains = interval1.Contains(interval2);

            // Assert
            contains.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> ContainsTimeIntervalDuringDayExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time2, false },
            { Time0, Time3, false },
            { Time0, Time4, false },
            { Time0, Time6, true },
            { Time0, Time7, true },

            { Time1, Time0, true },
            { Time1, Time1, true },
            { Time1, Time2, false },
            { Time1, Time3, false },
            { Time1, Time4, false },
            { Time1, Time6, true },
            { Time1, Time8, true },

            { Time2, Time1, true },

            { Time3, Time0, true },
            { Time3, Time1, true },
            { Time3, Time3, true },
            { Time3, Time4, false },
            { Time3, Time6, true },
            { Time3, Time8, true },

            { Time4, Time0, false },
            { Time4, Time1, false },
            { Time4, Time3, false },
            { Time4, Time4, false },
            { Time4, Time5, false },
            { Time4, Time6, false },
            { Time4, Time8, false },

            { Time5, Time4, false },

            { Time6, Time0, false },
            { Time6, Time1, false },
            { Time6, Time3, false },
            { Time6, Time4, false },
            { Time6, Time6, true },
            { Time6, Time8, false },

            { Time7, Time0, false },
            { Time7, Time2, false },
            { Time7, Time3, false },
            { Time7, Time4, false },
            { Time7, Time6, true },
            { Time7, Time7, true },
            { Time7, Time8, false },

            { Time8, Time7, true }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeIntervalDuringDayExamples))]
        public void Contains_DuringDay(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time3, Time6);

            // Act
            var contains = interval1.Contains(interval2);

            // Assert
            contains.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> ContainsTimeIntervalUntilMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time2, false },
            { Time0, Time3, false },
            { Time0, Time4, false },
            { Time0, Time6, false },
            { Time0, Time7, false },

            { Time1, Time0, true },
            { Time1, Time1, true },
            { Time1, Time2, false },
            { Time1, Time3, false },
            { Time1, Time5, false },
            { Time1, Time6, false },
            { Time1, Time7, false },

            { Time2, Time1, true },

            { Time3, Time0, true },
            { Time3, Time1, true },
            { Time3, Time3, true },
            { Time3, Time5, false },
            { Time3, Time6, false },
            { Time3, Time7, false },

            { Time4, Time0, true },
            { Time4, Time2, true },
            { Time4, Time3, true },
            { Time4, Time4, true },
            { Time4, Time5, false },
            { Time4, Time6, false },
            { Time4, Time7, false },

            { Time5, Time4, true },

            { Time6, Time0, true },
            { Time6, Time2, true },
            { Time6, Time3, true },
            { Time6, Time4, true },
            { Time6, Time6, true },
            { Time6, Time7, false },

            { Time7, Time0, false },
            { Time7, Time2, false },
            { Time7, Time3, false },
            { Time7, Time4, false },
            { Time7, Time6, false },
            { Time7, Time7, false },
            { Time7, Time8, false },

            { Time8, Time7, false }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeIntervalUntilMidnightExamples))]
        public void Contains_UntilMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time6, Time0);

            // Act
            var contains = interval1.Contains(interval2);

            // Assert
            contains.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> ContainsTimeIntervalContainsOverMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, false },
            { Time0, Time2, false },
            { Time0, Time3, false },
            { Time0, Time4, false },
            { Time0, Time6, false },
            { Time0, Time7, false },

            { Time1, Time0, false },
            { Time1, Time1, false },
            { Time1, Time2, false },
            { Time1, Time3, false },
            { Time1, Time4, false },
            { Time1, Time6, false },
            { Time1, Time7, false },

            { Time2, Time1, false },

            { Time3, Time0, false },
            { Time3, Time2, false },
            { Time3, Time3, true },
            { Time3, Time5, false },
            { Time3, Time6, false },
            { Time3, Time8, false },

            { Time4, Time0, false },
            { Time4, Time2, false },
            { Time4, Time3, true },
            { Time4, Time4, true },
            { Time4, Time5, false },
            { Time4, Time6, false },
            { Time4, Time8, false },

            { Time5, Time4, true },

            { Time6, Time0, false },
            { Time6, Time2, false },
            { Time6, Time3, true },
            { Time6, Time4, true },
            { Time6, Time6, true },
            { Time6, Time7, false },

            { Time7, Time0, false },
            { Time7, Time1, false },
            { Time7, Time3, false },
            { Time7, Time5, false },
            { Time7, Time6, false },
            { Time7, Time7, false },
            { Time7, Time8, false },

            { Time8, Time7, false }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeIntervalContainsOverMidnightExamples))]
        public void Contains_OverMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time6, Time3);

            // Act
            var contains = interval1.Contains(interval2);

            // Assert
            contains.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> ContainsTimeIntervalMidnightToMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time3, false },
            { Time3, Time0, false },
            { Time3, Time2, false },
            { Time3, Time3, false },
            { Time3, Time7, false }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeIntervalMidnightToMidnightExamples))]
        public void Contains_MidnightToMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time0, Time0);

            // Act
            var contains = interval1.Contains(interval2);

            // Assert
            contains.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> ContainsTimeIntervalTimeToTimeExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, false },
            { Time0, Time1, false },
            { Time0, Time3, false },
            { Time0, Time7, false },
            { Time1, Time0, false },
            { Time1, Time1, false },
            { Time1, Time2, false },
            { Time1, Time3, false },
            { Time1, Time5, false },
            { Time2, Time1, false },
            { Time3, Time3, true },
            { Time3, Time5, false },
            { Time3, Time0, false },
            { Time3, Time2, false },
            { Time4, Time5, false },
            { Time5, Time0, false },
            { Time5, Time2, false },
            { Time5, Time3, false },
            { Time5, Time4, false },
            { Time5, Time5, false },
            { Time5, Time8, false }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeIntervalTimeToTimeExamples))]
        public void Contains_TimeToTime(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time3, Time3);

            // Act
            var contains = interval1.Contains(interval2);

            // Assert
            contains.Should().Be(expected);
        }

        #endregion

        #region GetOverlapsWithTimeInterval

        [Fact]
        public void GetOverlapsWithTimeInterval_NullFirst_ThrowsException()
        {
            // Arrange
            var interval1 = Fixture.Create<TimeInterval>();
            TimeInterval interval2 = null;

            // Act
            Action act = () => interval1.GetOverlapsWith(interval2).ToList();

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void GetOverlapsWithTimeInterval_NullSecond_ThrowsException()
        {
            // Arrange
            TimeInterval interval1 = null;
            var interval2 = Fixture.Create<TimeInterval>();

            // Act
            Action act = () => interval1.GetOverlapsWith(interval2).ToList();

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>> GetOverlapsWithTimeIntervalFromMidnightExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>>
        {
            { Time0, Time0, new[]{ new TimeInterval(Time0, Time3) }},
            { Time0, Time1, new[]{ new TimeInterval(Time0, Time1) }},
            { Time0, Time3, new[]{ new TimeInterval(Time0, Time3) }},
            { Time0, Time5, new[]{ new TimeInterval(Time0, Time3) }},
            { Time0, Time6, new[]{ new TimeInterval(Time0, Time3) }},
            { Time0, Time7, new[]{ new TimeInterval(Time0, Time3) }},

            { Time1, Time0, new[]{ new TimeInterval(Time1, Time3) }},
            { Time1, Time1, new[]{ new TimeInterval(Time0, Time1), new TimeInterval(Time1, Time3) }},
            { Time1, Time2, new[]{ new TimeInterval(Time1, Time2) }},
            { Time1, Time3, new[]{ new TimeInterval(Time1, Time3) }},
            { Time1, Time5, new[]{ new TimeInterval(Time1, Time3) }},
            { Time1, Time6, new[]{ new TimeInterval(Time1, Time3) }},
            { Time1, Time7, new[]{ new TimeInterval(Time1, Time3) }},
            
            { Time2, Time1, new[]{ new TimeInterval(Time0, Time1), new TimeInterval(Time2, Time3) }},

            { Time3, Time0, EmptyTimeIntervals },
            { Time3, Time1, new[]{ new TimeInterval(Time0, Time1) } },
            { Time3, Time3, new[]{ new TimeInterval(Time0, Time3) } },
            { Time3, Time5, EmptyTimeIntervals },
            { Time3, Time6, EmptyTimeIntervals },
            { Time3, Time7, EmptyTimeIntervals },

            { Time4, Time0, EmptyTimeIntervals },
            { Time4, Time1, new[]{ new TimeInterval(Time0, Time1) } },
            { Time4, Time3, new[]{ new TimeInterval(Time0, Time3) } },
            { Time4, Time4, new[]{ new TimeInterval(Time0, Time3) } },
            { Time4, Time5, EmptyTimeIntervals },
            { Time4, Time6, EmptyTimeIntervals },
            { Time4, Time8, EmptyTimeIntervals },

            { Time5, Time4, new[]{ new TimeInterval(Time0, Time3) } },

            { Time6, Time0, EmptyTimeIntervals },
            { Time6, Time1, new[]{ new TimeInterval(Time0, Time1) } },
            { Time6, Time3, new[]{ new TimeInterval(Time0, Time3) } },
            { Time6, Time4, new[]{ new TimeInterval(Time0, Time3) } },
            { Time6, Time6, new[]{ new TimeInterval(Time0, Time3) } },
            { Time6, Time8, EmptyTimeIntervals },

            { Time7, Time0, EmptyTimeIntervals },
            { Time7, Time1, new[]{ new TimeInterval(Time0, Time1) } },
            { Time7, Time3, new[]{ new TimeInterval(Time0, Time3) } },
            { Time7, Time5, new[]{ new TimeInterval(Time0, Time3) } },
            { Time7, Time6, new[]{ new TimeInterval(Time0, Time3) } },
            { Time7, Time7, new[]{ new TimeInterval(Time0, Time3) } },
            { Time7, Time8, EmptyTimeIntervals },

            { Time8, Time7, new[]{ new TimeInterval(Time0, Time3) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithTimeIntervalFromMidnightExamples))]
        public void GetOverlapsWithTimeInterval_FromMidnight(LocalTime startTime, LocalTime endTime, IEnumerable<TimeInterval> expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time0, Time3);

            // Act
            var overlaps = interval1.GetOverlapsWith(interval2).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>> GetOverlapsWithTimeIntervalDuringDayExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>>
        {
            { Time0, Time0, new[]{ new TimeInterval(Time3, Time6) } },
            { Time0, Time2, EmptyTimeIntervals },
            { Time0, Time3, EmptyTimeIntervals },
            { Time0, Time4, new[]{ new TimeInterval(Time3, Time4) } },
            { Time0, Time6, new[]{ new TimeInterval(Time3, Time6) } },
            { Time0, Time7, new[]{ new TimeInterval(Time3, Time6) } },

            { Time1, Time0, new[]{ new TimeInterval(Time3, Time6) } },
            { Time1, Time1, new[]{ new TimeInterval(Time3, Time6) } },
            { Time1, Time2, EmptyTimeIntervals },
            { Time1, Time3, EmptyTimeIntervals },
            { Time1, Time4, new[]{ new TimeInterval(Time3, Time4) } },
            { Time1, Time6, new[]{ new TimeInterval(Time3, Time6) } },
            { Time1, Time8, new[]{ new TimeInterval(Time3, Time6) } },

            { Time2, Time1, new[]{ new TimeInterval(Time3, Time6) } },

            { Time3, Time0, new[]{ new TimeInterval(Time3, Time6) } },
            { Time3, Time1, new[]{ new TimeInterval(Time3, Time6) } },
            { Time3, Time3, new[]{ new TimeInterval(Time3, Time6) } },
            { Time3, Time4, new[]{ new TimeInterval(Time3, Time4) } },
            { Time3, Time6, new[]{ new TimeInterval(Time3, Time6) } },
            { Time3, Time8, new[]{ new TimeInterval(Time3, Time6) } },

            { Time4, Time0, new[]{ new TimeInterval(Time4, Time6) } },
            { Time4, Time1, new[]{ new TimeInterval(Time4, Time6) } },
            { Time4, Time3, new[]{ new TimeInterval(Time4, Time6) } },
            { Time4, Time4, new[]{ new TimeInterval(Time3, Time4), new TimeInterval(Time4, Time6) } },
            { Time4, Time5, new[]{ new TimeInterval(Time4, Time5) } },
            { Time4, Time6, new[]{ new TimeInterval(Time4, Time6) } },
            { Time4, Time8, new[]{ new TimeInterval(Time4, Time6) } },

            { Time5, Time4, new[]{ new TimeInterval(Time3, Time4), new TimeInterval(Time5, Time6) } },

            { Time6, Time0, EmptyTimeIntervals },
            { Time6, Time1, EmptyTimeIntervals },
            { Time6, Time3, EmptyTimeIntervals },
            { Time6, Time4, new[]{ new TimeInterval(Time3, Time4) } },
            { Time6, Time6, new[]{ new TimeInterval(Time3, Time6) } },
            { Time6, Time8, EmptyTimeIntervals },

            { Time7, Time0, EmptyTimeIntervals },
            { Time7, Time2, EmptyTimeIntervals },
            { Time7, Time3, EmptyTimeIntervals },
            { Time7, Time4, new[]{ new TimeInterval(Time3, Time4) } },
            { Time7, Time6, new[]{ new TimeInterval(Time3, Time6) } },
            { Time7, Time7, new[]{ new TimeInterval(Time3, Time6) } },
            { Time7, Time8, EmptyTimeIntervals },

            { Time8, Time7, new[]{ new TimeInterval(Time3, Time6) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithTimeIntervalDuringDayExamples))]
        public void GetOverlapsWithTimeInterval_DuringDay(LocalTime startTime, LocalTime endTime, IEnumerable<TimeInterval> expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time3, Time6);

            // Act
            var overlaps = interval1.GetOverlapsWith(interval2).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>> GetOverlapsWithTimeIntervalUntilMidnightExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>>
        {
            // Time6, Time0

            { Time0, Time0, new[]{ new TimeInterval(Time6, Time0) } },
            { Time0, Time2, EmptyTimeIntervals },
            { Time0, Time3, EmptyTimeIntervals },
            { Time0, Time4, EmptyTimeIntervals },
            { Time0, Time6, EmptyTimeIntervals },
            { Time0, Time7, new[]{ new TimeInterval(Time6, Time7) } },

            { Time1, Time0, new[]{ new TimeInterval(Time6, Time0) } },
            { Time1, Time1, new[]{ new TimeInterval(Time6, Time0) } },
            { Time1, Time2, EmptyTimeIntervals },
            { Time1, Time3, EmptyTimeIntervals },
            { Time1, Time5, EmptyTimeIntervals },
            { Time1, Time6, EmptyTimeIntervals },
            { Time1, Time7, new[]{ new TimeInterval(Time6, Time7) } },

            { Time2, Time1, new[]{ new TimeInterval(Time6, Time0) } },

            { Time3, Time0, new[]{ new TimeInterval(Time6, Time0) } },
            { Time3, Time1, new[]{ new TimeInterval(Time6, Time0) } },
            { Time3, Time3, new[]{ new TimeInterval(Time6, Time0) } },
            { Time3, Time5, EmptyTimeIntervals },
            { Time3, Time6, EmptyTimeIntervals },
            { Time3, Time7, new[]{ new TimeInterval(Time6, Time7) } },

            { Time4, Time0, new[]{ new TimeInterval(Time6, Time0) } },
            { Time4, Time2, new[]{ new TimeInterval(Time6, Time0) } },
            { Time4, Time3, new[]{ new TimeInterval(Time6, Time0) } },
            { Time4, Time4, new[]{ new TimeInterval(Time6, Time0) } },
            { Time4, Time5, EmptyTimeIntervals },
            { Time4, Time6, EmptyTimeIntervals },
            { Time4, Time7, new[]{ new TimeInterval(Time6, Time7) } },

            { Time5, Time4, new[]{ new TimeInterval(Time6, Time0) } },

            { Time6, Time0, new[]{ new TimeInterval(Time6, Time0) } },
            { Time6, Time2, new[]{ new TimeInterval(Time6, Time0) } },
            { Time6, Time3, new[]{ new TimeInterval(Time6, Time0) } },
            { Time6, Time4, new[]{ new TimeInterval(Time6, Time0) } },
            { Time6, Time6, new[]{ new TimeInterval(Time6, Time0) } },
            { Time6, Time7, new[]{ new TimeInterval(Time6, Time7) } },

            { Time7, Time0, new[]{ new TimeInterval(Time7, Time0) } },
            { Time7, Time2, new[]{ new TimeInterval(Time7, Time0) } },
            { Time7, Time3, new[]{ new TimeInterval(Time7, Time0) } },
            { Time7, Time4, new[]{ new TimeInterval(Time7, Time0) } },
            { Time7, Time6, new[]{ new TimeInterval(Time7, Time0) } },
            { Time7, Time7, new[]{ new TimeInterval(Time6, Time7), new TimeInterval(Time7, Time0) } },
            { Time7, Time8, new[]{ new TimeInterval(Time7, Time8) } },

            { Time8, Time7, new[]{ new TimeInterval(Time6, Time7), new TimeInterval(Time8, Time0) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithTimeIntervalUntilMidnightExamples))]
        public void GetOverlapsWithTimeInterval_UntilMidnight(LocalTime startTime, LocalTime endTime, IEnumerable<TimeInterval> expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time6, Time0);

            // Act
            var overlaps = interval1.GetOverlapsWith(interval2).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>> GetOverlapsWithTimeIntervalOverMidnightExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>>
        {
            // Time6, Time3

            { Time0, Time0, new []{ new TimeInterval(Time0, Time3), new TimeInterval(Time6, Time0) } },
            { Time0, Time2, new []{ new TimeInterval(Time0, Time2) } },
            { Time0, Time3, new []{ new TimeInterval(Time0, Time3) } },
            { Time0, Time4, new []{ new TimeInterval(Time0, Time3) } },
            { Time0, Time6, new []{ new TimeInterval(Time0, Time3) } },
            { Time0, Time7, new []{ new TimeInterval(Time0, Time3), new TimeInterval(Time6, Time7) } },

            { Time1, Time0, new []{ new TimeInterval(Time1, Time3), new TimeInterval(Time6, Time0) } },
            { Time1, Time1, new []{ new TimeInterval(Time1, Time3), new TimeInterval(Time6, Time1) } },
            { Time1, Time2, new []{ new TimeInterval(Time1, Time2) } },
            { Time1, Time3, new []{ new TimeInterval(Time1, Time3) } },
            { Time1, Time4, new []{ new TimeInterval(Time1, Time3) } },
            { Time1, Time6, new []{ new TimeInterval(Time1, Time3) } },
            { Time1, Time7, new []{ new TimeInterval(Time1, Time3), new TimeInterval(Time6, Time7) } },

            { Time2, Time1, new []{ new TimeInterval(Time2, Time3), new TimeInterval(Time6, Time1) } },

            { Time3, Time0, new []{ new TimeInterval(Time6, Time0) } },
            { Time3, Time2, new []{ new TimeInterval(Time6, Time2) } },
            { Time3, Time3, new []{ new TimeInterval(Time6, Time3) } },
            { Time3, Time5, EmptyTimeIntervals },
            { Time3, Time6, EmptyTimeIntervals },
            { Time3, Time8, new []{ new TimeInterval(Time6, Time8) } },

            { Time4, Time0, new []{ new TimeInterval(Time6, Time0) } },
            { Time4, Time2, new []{ new TimeInterval(Time6, Time2) } },
            { Time4, Time3, new []{ new TimeInterval(Time6, Time3) } },
            { Time4, Time4, new []{ new TimeInterval(Time6, Time3) } },
            { Time4, Time5, EmptyTimeIntervals },
            { Time4, Time6, EmptyTimeIntervals },
            { Time4, Time8, new []{ new TimeInterval(Time6, Time8) } },

            { Time5, Time4, new []{ new TimeInterval(Time6, Time3) } },

            { Time6, Time0, new []{ new TimeInterval(Time6, Time0) } },
            { Time6, Time2, new []{ new TimeInterval(Time6, Time2) } },
            { Time6, Time3, new []{ new TimeInterval(Time6, Time3) } },
            { Time6, Time4, new []{ new TimeInterval(Time6, Time3) } },
            { Time6, Time6, new []{ new TimeInterval(Time6, Time3) } },
            { Time6, Time7, new []{ new TimeInterval(Time6, Time7) } },

            { Time7, Time0, new []{ new TimeInterval(Time7, Time0) } },
            { Time7, Time1, new []{ new TimeInterval(Time7, Time1) } },
            { Time7, Time3, new []{ new TimeInterval(Time7, Time3) } },
            { Time7, Time5, new []{ new TimeInterval(Time7, Time3) } },
            { Time7, Time6, new []{ new TimeInterval(Time7, Time3) } },
            { Time7, Time7, new []{ new TimeInterval(Time6, Time7), new TimeInterval(Time7, Time3) } },
            { Time7, Time8, new []{ new TimeInterval(Time7, Time8) } },

            { Time8, Time7, new []{ new TimeInterval(Time8, Time3), new TimeInterval(Time6, Time7) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithTimeIntervalOverMidnightExamples))]
        public void GetOverlapsWithTimeInterval_OverMidnight(LocalTime startTime, LocalTime endTime, IEnumerable<TimeInterval> expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time6, Time3);

            // Act
            var overlaps = interval1.GetOverlapsWith(interval2).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>> GetOverlapsWithTimeIntervalMidnightToMidnightExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>>
        {
            { Time0, Time0, new []{ new TimeInterval(Time0, Time0) } },
            { Time0, Time3, new []{ new TimeInterval(Time0, Time3) } },

            { Time3, Time0, new []{ new TimeInterval(Time3, Time0) } },
            { Time3, Time2, new []{ new TimeInterval(Time0, Time2), new TimeInterval(Time3, Time0) } },
            { Time3, Time3, new []{ new TimeInterval(Time0, Time3), new TimeInterval(Time3, Time0) } },
            { Time3, Time7, new []{ new TimeInterval(Time3, Time7) } } 
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithTimeIntervalMidnightToMidnightExamples))]
        public void GetOverlapsWithTimeInterval_MidnightToMidnight(LocalTime startTime, LocalTime endTime, IEnumerable<TimeInterval> expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time0, Time0);

            // Act
            var overlaps = interval1.GetOverlapsWith(interval2).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>> GetOverlapsWithTimeIntervalTimeToTimeExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<TimeInterval>>
        {
            { Time0, Time0, new []{ new TimeInterval(Time0, Time3), new TimeInterval(Time3, Time0) } },
            { Time0, Time1, new []{ new TimeInterval(Time0, Time1) } },
            { Time0, Time3, new []{ new TimeInterval(Time0, Time3) } },
            { Time0, Time7, new []{ new TimeInterval(Time0, Time3), new TimeInterval(Time3, Time7) } },

            { Time1, Time0, new []{ new TimeInterval(Time1, Time3), new TimeInterval(Time3, Time0) } },
            { Time1, Time1, new []{ new TimeInterval(Time1, Time3), new TimeInterval(Time3, Time1) } },
            { Time1, Time2, new []{ new TimeInterval(Time1, Time2) } },
            { Time1, Time3, new []{ new TimeInterval(Time1, Time3) } },
            { Time1, Time5, new []{ new TimeInterval(Time1, Time3), new TimeInterval(Time3, Time5) } },

            { Time2, Time1, new []{ new TimeInterval(Time2, Time3), new TimeInterval(Time3, Time1) } },

            { Time3, Time0, new []{ new TimeInterval(Time3, Time0) } },
            { Time3, Time2, new []{ new TimeInterval(Time3, Time2) } },
            { Time3, Time3, new []{ new TimeInterval(Time3, Time3) } },
            { Time3, Time5, new []{ new TimeInterval(Time3, Time5) } },

            { Time4, Time5, new []{ new TimeInterval(Time4, Time5) } },

            { Time5, Time0, new []{ new TimeInterval(Time5, Time0) } },
            { Time5, Time2, new []{ new TimeInterval(Time5, Time2) } },
            { Time5, Time3, new []{ new TimeInterval(Time5, Time3) } },
            { Time5, Time4, new []{ new TimeInterval(Time3, Time4), new TimeInterval(Time5, Time3) } },
            { Time5, Time5, new []{ new TimeInterval(Time3, Time5), new TimeInterval(Time5, Time3) } },
            { Time5, Time8, new []{ new TimeInterval(Time5, Time8) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithTimeIntervalTimeToTimeExamples))]
        public void GetOverlapsWithTimeInterval_TimeToTime(LocalTime startTime, LocalTime endTime, IEnumerable<TimeInterval> expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time3, Time3);

            // Act
            var overlaps = interval1.GetOverlapsWith(interval2).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region GetOverlapsWith

        // TODO: Longer than a day intervals

        [Fact]
        public void GetOverlapsWith_NullFirst_ThrowsException()
        {
            // Arrange
            var dateTimeInterval = Fixture.Create<DateTimeInterval>();
            TimeInterval timeInterval = null;

            // Act
            Action act = () => timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void GetOverlapsWith_NullSecond_ThrowsException()
        {
            // Arrange
            DateTimeInterval dateTimeInterval = null;
            var timeInterval = Fixture.Create<TimeInterval>();

            // Act
            Action act = () => timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>> GetOverlapsWithFromMidnightExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>>
        {
            { Time0, Time0, new[]{ Date.At(Time0, Time3) }},
            { Time0, Time1, new[]{ Date.At(Time0, Time1) }},
            { Time0, Time3, new[]{ Date.At(Time0, Time3) }},
            { Time0, Time5, new[]{ Date.At(Time0, Time3) }},
            { Time0, Time6, new[]{ Date.At(Time0, Time3) }},
            { Time0, Time7, new[]{ Date.At(Time0, Time3) }},

            { Time1, Time0, new[]{ Date.At(Time1, Time3) }},
            { Time1, Time1, new[]{ Date.At(Time0, Time1), Date.At(Time1, Time3) }},
            { Time1, Time2, new[]{ Date.At(Time1, Time2) }},
            { Time1, Time3, new[]{ Date.At(Time1, Time3) }},
            { Time1, Time5, new[]{ Date.At(Time1, Time3) }},
            { Time1, Time6, new[]{ Date.At(Time1, Time3) }},
            { Time1, Time7, new[]{ Date.At(Time1, Time3) }},

            { Time2, Time1, new[]{ Date.At(Time0, Time1), Date.At(Time2, Time3) }},

            { Time3, Time0, EmptyDateTimeIntervals },
            { Time3, Time1, new[]{ Date.At(Time0, Time1) } },
            { Time3, Time3, new[]{ Date.At(Time0, Time3) } },
            { Time3, Time5, EmptyDateTimeIntervals },
            { Time3, Time6, EmptyDateTimeIntervals },
            { Time3, Time7, EmptyDateTimeIntervals },

            { Time4, Time0, EmptyDateTimeIntervals },
            { Time4, Time1, new[]{ Date.At(Time0, Time1) } },
            { Time4, Time3, new[]{ Date.At(Time0, Time3) } },
            { Time4, Time4, new[]{ Date.At(Time0, Time3) } },
            { Time4, Time5, EmptyDateTimeIntervals },
            { Time4, Time6, EmptyDateTimeIntervals },
            { Time4, Time8, EmptyDateTimeIntervals },

            { Time5, Time4, new[]{ Date.At(Time0, Time3) } },

            { Time6, Time0, EmptyDateTimeIntervals },
            { Time6, Time1, new[]{ Date.At(Time0, Time1) } },
            { Time6, Time3, new[]{ Date.At(Time0, Time3) } },
            { Time6, Time4, new[]{ Date.At(Time0, Time3) } },
            { Time6, Time6, new[]{ Date.At(Time0, Time3) } },
            { Time6, Time8, EmptyDateTimeIntervals },

            { Time7, Time0, EmptyDateTimeIntervals },
            { Time7, Time1, new[]{ Date.At(Time0, Time1) } },
            { Time7, Time3, new[]{ Date.At(Time0, Time3) } },
            { Time7, Time5, new[]{ Date.At(Time0, Time3) } },
            { Time7, Time6, new[]{ Date.At(Time0, Time3) } },
            { Time7, Time7, new[]{ Date.At(Time0, Time3) } },
            { Time7, Time8, EmptyDateTimeIntervals },

            { Time8, Time7, new[]{ Date.At(Time0, Time3) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithFromMidnightExamples))]
        public void GetOverlapsWith_FromMidnight(LocalTime startTime, LocalTime endTime, IEnumerable<DateTimeInterval> expected)
        {
            // Arrange
            var dateTimeInterval = Date.At(Time0, Time3);
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>> GetOverlapsWithDuringDayExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>>
        {
            { Time0, Time0, new[]{ Date.At(Time3, Time6) } },
            { Time0, Time2, EmptyDateTimeIntervals },
            { Time0, Time3, EmptyDateTimeIntervals },
            { Time0, Time4, new[]{ Date.At(Time3, Time4) } },
            { Time0, Time6, new[]{ Date.At(Time3, Time6) } },
            { Time0, Time7, new[]{ Date.At(Time3, Time6) } },

            { Time1, Time0, new[]{ Date.At(Time3, Time6) } },
            { Time1, Time1, new[]{ Date.At(Time3, Time6) } },
            { Time1, Time2, EmptyDateTimeIntervals },
            { Time1, Time3, EmptyDateTimeIntervals },
            { Time1, Time4, new[]{ Date.At(Time3, Time4) } },
            { Time1, Time6, new[]{ Date.At(Time3, Time6) } },
            { Time1, Time8, new[]{ Date.At(Time3, Time6) } },

            { Time2, Time1, new[]{ Date.At(Time3, Time6) } },

            { Time3, Time0, new[]{ Date.At(Time3, Time6) } },
            { Time3, Time1, new[]{ Date.At(Time3, Time6) } },
            { Time3, Time3, new[]{ Date.At(Time3, Time6) } },
            { Time3, Time4, new[]{ Date.At(Time3, Time4) } },
            { Time3, Time6, new[]{ Date.At(Time3, Time6) } },
            { Time3, Time8, new[]{ Date.At(Time3, Time6) } },

            { Time4, Time0, new[]{ Date.At(Time4, Time6) } },
            { Time4, Time1, new[]{ Date.At(Time4, Time6) } },
            { Time4, Time3, new[]{ Date.At(Time4, Time6) } },
            { Time4, Time4, new[]{ Date.At(Time3, Time4), Date.At(Time4, Time6) } },
            { Time4, Time5, new[]{ Date.At(Time4, Time5) } },
            { Time4, Time6, new[]{ Date.At(Time4, Time6) } },
            { Time4, Time8, new[]{ Date.At(Time4, Time6) } },

            { Time5, Time4, new[]{ Date.At(Time3, Time4), Date.At(Time5, Time6) } },

            { Time6, Time0, EmptyDateTimeIntervals },
            { Time6, Time1, EmptyDateTimeIntervals },
            { Time6, Time3, EmptyDateTimeIntervals },
            { Time6, Time4, new[]{ Date.At(Time3, Time4) } },
            { Time6, Time6, new[]{ Date.At(Time3, Time6) } },
            { Time6, Time8, EmptyDateTimeIntervals },

            { Time7, Time0, EmptyDateTimeIntervals },
            { Time7, Time2, EmptyDateTimeIntervals },
            { Time7, Time3, EmptyDateTimeIntervals },
            { Time7, Time4, new[]{ Date.At(Time3, Time4) } },
            { Time7, Time6, new[]{ Date.At(Time3, Time6) } },
            { Time7, Time7, new[]{ Date.At(Time3, Time6) } },
            { Time7, Time8, EmptyDateTimeIntervals },

            { Time8, Time7, new[]{ Date.At(Time3, Time6) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithDuringDayExamples))]
        public void GetOverlapsWith_DuringDay(LocalTime startTime, LocalTime endTime, IEnumerable<DateTimeInterval> expected)
        {
            // Arrange
            var dateTimeInterval = Date.At(Time3, Time6);
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>> GetOverlapsWithUntilMidnightExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>>
        {
            // Time6, Time0

            { Time0, Time0, new[]{ Date.At(Time6, Time0) } },
            { Time0, Time2, EmptyDateTimeIntervals },
            { Time0, Time3, EmptyDateTimeIntervals },
            { Time0, Time4, EmptyDateTimeIntervals },
            { Time0, Time6, EmptyDateTimeIntervals },
            { Time0, Time7, new[]{ Date.At(Time6, Time7) } },

            { Time1, Time0, new[]{ Date.At(Time6, Time0) } },
            { Time1, Time1, new[]{ Date.At(Time6, Time0) } },
            { Time1, Time2, EmptyDateTimeIntervals },
            { Time1, Time3, EmptyDateTimeIntervals },
            { Time1, Time5, EmptyDateTimeIntervals },
            { Time1, Time6, EmptyDateTimeIntervals },
            { Time1, Time7, new[]{ Date.At(Time6, Time7) } },

            { Time2, Time1, new[]{ Date.At(Time6, Time0) } },

            { Time3, Time0, new[]{ Date.At(Time6, Time0) } },
            { Time3, Time1, new[]{ Date.At(Time6, Time0) } },
            { Time3, Time3, new[]{ Date.At(Time6, Time0) } },
            { Time3, Time5, EmptyDateTimeIntervals },
            { Time3, Time6, EmptyDateTimeIntervals },
            { Time3, Time7, new[]{ Date.At(Time6, Time7) } },

            { Time4, Time0, new[]{ Date.At(Time6, Time0) } },
            { Time4, Time2, new[]{ Date.At(Time6, Time0) } },
            { Time4, Time3, new[]{ Date.At(Time6, Time0) } },
            { Time4, Time4, new[]{ Date.At(Time6, Time0) } },
            { Time4, Time5, EmptyDateTimeIntervals },
            { Time4, Time6, EmptyDateTimeIntervals },
            { Time4, Time7, new[]{ Date.At(Time6, Time7) } },

            { Time5, Time4, new[]{ Date.At(Time6, Time0) } },

            { Time6, Time0, new[]{ Date.At(Time6, Time0) } },
            { Time6, Time2, new[]{ Date.At(Time6, Time0) } },
            { Time6, Time3, new[]{ Date.At(Time6, Time0) } },
            { Time6, Time4, new[]{ Date.At(Time6, Time0) } },
            { Time6, Time6, new[]{ Date.At(Time6, Time0) } },
            { Time6, Time7, new[]{ Date.At(Time6, Time7) } },

            { Time7, Time0, new[]{ Date.At(Time7, Time0) } },
            { Time7, Time2, new[]{ Date.At(Time7, Time0) } },
            { Time7, Time3, new[]{ Date.At(Time7, Time0) } },
            { Time7, Time4, new[]{ Date.At(Time7, Time0) } },
            { Time7, Time6, new[]{ Date.At(Time7, Time0) } },
            { Time7, Time7, new[]{ Date.At(Time6, Time7), Date.At(Time7, Time0) } },
            { Time7, Time8, new[]{ Date.At(Time7, Time8) } },

            { Time8, Time7, new[]{ Date.At(Time6, Time7), Date.At(Time8, Time0) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithUntilMidnightExamples))]
        public void GetOverlapsWith_UntilMidnight(LocalTime startTime, LocalTime endTime, IEnumerable<DateTimeInterval> expected)
        {
            // Arrange
            var dateTimeInterval = Date.At(Time6, Time0);
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>> GetOverlapsWithOverMidnightExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>>
        {
            // Time6, Time3

            { Time0, Time0, new []{ Date.At(Time6, Time0), Date.NextDay().At(Time0, Time3) } },
            { Time0, Time2, new []{ Date.NextDay().At(Time0, Time2) } },
            { Time0, Time3, new []{ Date.NextDay().At(Time0, Time3) } },
            { Time0, Time4, new []{ Date.NextDay().At(Time0, Time3) } },
            { Time0, Time6, new []{ Date.NextDay().At(Time0, Time3) } },
            { Time0, Time7, new []{ Date.At(Time6, Time7), Date.NextDay().At(Time0, Time3) } },

            { Time1, Time0, new []{ Date.At(Time6, Time0), Date.NextDay().At(Time1, Time3) } },
            { Time1, Time1, new []{ Date.At(Time6, Time1), Date.NextDay().At(Time1, Time3) } },
            { Time1, Time2, new []{ Date.NextDay().At(Time1, Time2) } },
            { Time1, Time3, new []{ Date.NextDay().At(Time1, Time3) } },
            { Time1, Time4, new []{ Date.NextDay().At(Time1, Time3) } },
            { Time1, Time6, new []{ Date.NextDay().At(Time1, Time3) } },
            { Time1, Time7, new []{ Date.At(Time6, Time7), Date.NextDay().At(Time1, Time3) } },

            { Time2, Time1, new []{ Date.At(Time6, Time1), Date.NextDay().At(Time2, Time3) } },

            { Time3, Time0, new []{ Date.At(Time6, Time0) } },
            { Time3, Time2, new []{ Date.At(Time6, Time2) } },
            { Time3, Time3, new []{ Date.At(Time6, Time3) } },
            { Time3, Time5, EmptyDateTimeIntervals },
            { Time3, Time6, EmptyDateTimeIntervals },
            { Time3, Time8, new []{ Date.At(Time6, Time8) } },

            { Time4, Time0, new []{ Date.At(Time6, Time0) } },
            { Time4, Time2, new []{ Date.At(Time6, Time2) } },
            { Time4, Time3, new []{ Date.At(Time6, Time3) } },
            { Time4, Time4, new []{ Date.At(Time6, Time3) } },
            { Time4, Time5, EmptyDateTimeIntervals },
            { Time4, Time6, EmptyDateTimeIntervals },
            { Time4, Time8, new []{ Date.At(Time6, Time8) } },

            { Time5, Time4, new []{ Date.At(Time6, Time3) } },

            { Time6, Time0, new []{ Date.At(Time6, Time0) } },
            { Time6, Time2, new []{ Date.At(Time6, Time2) } },
            { Time6, Time3, new []{ Date.At(Time6, Time3) } },
            { Time6, Time4, new []{ Date.At(Time6, Time3) } },
            { Time6, Time6, new []{ Date.At(Time6, Time3) } },
            { Time6, Time7, new []{ Date.At(Time6, Time7) } },

            { Time7, Time0, new []{ Date.At(Time7, Time0) } },
            { Time7, Time1, new []{ Date.At(Time7, Time1) } },
            { Time7, Time3, new []{ Date.At(Time7, Time3) } },
            { Time7, Time5, new []{ Date.At(Time7, Time3) } },
            { Time7, Time6, new []{ Date.At(Time7, Time3) } },
            { Time7, Time7, new []{ Date.At(Time6, Time7), Date.At(Time7, Time3) } },
            { Time7, Time8, new []{ Date.At(Time7, Time8) } },

            { Time8, Time7, new []{ Date.At(Time8, Time3), Date.At(Time6, Time7) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithOverMidnightExamples))]
        public void GetOverlapsWith_OverMidnight(LocalTime startTime, LocalTime endTime, IEnumerable<DateTimeInterval> expected)
        {
            // Arrange
            var dateTimeInterval = Date.At(Time6, Time3);
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>> GetOverlapsWithMidnightToMidnightExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>>
        {
            { Time0, Time0, new []{ Date.At(Time0, Time0) } },
            { Time0, Time3, new []{ Date.At(Time0, Time3) } },

            { Time3, Time0, new []{ Date.At(Time3, Time0) } },
            { Time3, Time2, new []{ Date.At(Time0, Time2), Date.At(Time3, Time0) } },
            { Time3, Time3, new []{ Date.At(Time0, Time3), Date.At(Time3, Time0) } },
            { Time3, Time7, new []{ Date.At(Time3, Time7) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithMidnightToMidnightExamples))]
        public void GetOverlapsWith_MidnightToMidnight(LocalTime startTime, LocalTime endTime, IEnumerable<DateTimeInterval> expected)
        {
            // Arrange
            var dateTimeInterval = Date.At(Time0, Time0);
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>> GetOverlapsWithTimeToTimeExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>>
        {
            { Time0, Time0, new []{ Date.At(Time3, Time0), Date.NextDay().At(Time0, Time3) } },
            { Time0, Time1, new []{ Date.NextDay().At(Time0, Time1) } },
            { Time0, Time3, new []{ Date.NextDay().At(Time0, Time3) } },
            { Time0, Time7, new []{ Date.At(Time3, Time7), Date.NextDay().At(Time0, Time3) } },

            { Time1, Time0, new []{ Date.At(Time3, Time0), Date.NextDay().At(Time1, Time3) } },
            { Time1, Time1, new []{ Date.At(Time3, Time1), Date.NextDay().At(Time1, Time3) } },
            { Time1, Time2, new []{ Date.NextDay().At(Time1, Time2) } },
            { Time1, Time3, new []{ Date.NextDay().At(Time1, Time3) } },
            { Time1, Time5, new []{ Date.At(Time3, Time5), Date.NextDay().At(Time1, Time3) } },

            { Time2, Time1, new []{ Date.At(Time3, Time1), Date.NextDay().At(Time2, Time3) } },

            { Time3, Time0, new []{ Date.At(Time3, Time0) } },
            { Time3, Time2, new []{ Date.At(Time3, Time2) } },
            { Time3, Time3, new []{ Date.At(Time3, Time3) } },
            { Time3, Time5, new []{ Date.At(Time3, Time5) } },

            { Time4, Time5, new []{ Date.At(Time4, Time5) } },

            { Time5, Time0, new []{ Date.At(Time5, Time0) } },
            { Time5, Time2, new []{ Date.At(Time5, Time2) } },
            { Time5, Time3, new []{ Date.At(Time5, Time3) } },
            { Time5, Time4, new []{ Date.At(Time3, Time4), Date.At(Time5, Time3) } },
            { Time5, Time5, new []{ Date.At(Time3, Time5), Date.At(Time5, Time3) } },
            { Time5, Time8, new []{ Date.At(Time5, Time8) } }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithTimeToTimeExamples))]
        public void GetOverlapsWith_TimeToTime(LocalTime startTime, LocalTime endTime, IEnumerable<DateTimeInterval> expected)
        {
            // Arrange
            var dateTimeInterval = Date.At(Time3, Time3);
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>> GetOverlapsWithMidnightPointIntervalExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>>
        {
            { Time0, Time0, new []{ Date.At(Time0).ToSinglePointInterval() } },
            { Time0, Time3, new []{ Date.At(Time0).ToSinglePointInterval() } },

            { Time3, Time0, EmptyDateTimeIntervals },
            { Time3, Time2, new []{ Date.At(Time0).ToSinglePointInterval() } },
            { Time3, Time3, new []{ Date.At(Time0).ToSinglePointInterval() } },
            { Time3, Time7, EmptyDateTimeIntervals }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithMidnightPointIntervalExamples))]
        public void GetOverlapsWith_MidnightPointInterval(LocalTime startTime, LocalTime endTime, IEnumerable<DateTimeInterval> expected)
        {
            // Arrange
            var dateTimeInterval = Date.At(Time0).ToSinglePointInterval();
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>> GetOverlapsWithNonMidnightPointIntervalExamples = new TheoryData<LocalTime, LocalTime, IEnumerable<DateTimeInterval>>
        {
            { Time0, Time0, new []{ Date.At(Time3).ToSinglePointInterval() } },
            { Time0, Time1, EmptyDateTimeIntervals },
            { Time0, Time3, EmptyDateTimeIntervals },
            { Time0, Time7, new []{ Date.At(Time3).ToSinglePointInterval() } },

            { Time1, Time0, new []{ Date.At(Time3).ToSinglePointInterval() } },
            { Time1, Time1, new []{ Date.At(Time3).ToSinglePointInterval() } },
            { Time1, Time2, EmptyDateTimeIntervals },
            { Time1, Time3, EmptyDateTimeIntervals },
            { Time1, Time5, new []{ Date.At(Time3).ToSinglePointInterval() } },

            { Time2, Time1, new []{ Date.At(Time3).ToSinglePointInterval() } },

            { Time3, Time0, new []{ Date.At(Time3).ToSinglePointInterval() } },
            { Time3, Time2, new []{ Date.At(Time3).ToSinglePointInterval() } },
            { Time3, Time3, new []{ Date.At(Time3).ToSinglePointInterval() } },
            { Time3, Time5, new []{ Date.At(Time3).ToSinglePointInterval() } },

            { Time4, Time5, EmptyDateTimeIntervals },

            { Time5, Time0, EmptyDateTimeIntervals },
            { Time5, Time2, EmptyDateTimeIntervals },
            { Time5, Time3, EmptyDateTimeIntervals },
            { Time5, Time4, new []{ Date.At(Time3).ToSinglePointInterval() } },
            { Time5, Time5, new []{ Date.At(Time3).ToSinglePointInterval() } },
            { Time5, Time8, EmptyDateTimeIntervals }
        };

        [Theory]
        [MemberData(nameof(GetOverlapsWithNonMidnightPointIntervalExamples))]
        public void GetOverlapsWith_NonMidnightPointInterval(LocalTime startTime, LocalTime endTime, IEnumerable<DateTimeInterval> expected)
        {
            // Arrange
            var dateTimeInterval = Date.At(Time3).ToSinglePointInterval();
            var timeInterval = new TimeInterval(startTime, endTime);

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetOverlapsWith_MultiDayDateTimeInterval_AllOverlaps()
        {
            // Arrange
            var dateTimeInterval = new DateTimeInterval(Date.At(19, 30), Date.PlusDays(5).At(07, 45));
            var timeInterval = new TimeInterval(new LocalTime(16, 00), new LocalTime(10, 00));
            var expected = new[]
            {
                new DateTimeInterval(Date.PlusDays(0).At(19, 30), Date.PlusDays(1).At(10, 00)),
                new DateTimeInterval(Date.PlusDays(1).At(16, 00), Date.PlusDays(2).At(10, 00)),
                new DateTimeInterval(Date.PlusDays(2).At(16, 00), Date.PlusDays(3).At(10, 00)),
                new DateTimeInterval(Date.PlusDays(3).At(16, 00), Date.PlusDays(4).At(10, 00)),
                new DateTimeInterval(Date.PlusDays(4).At(16, 00), Date.PlusDays(5).At(07, 45))
            };

            // Act
            var overlaps = timeInterval.GetOverlapsWith(dateTimeInterval).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        #endregion
    }
}
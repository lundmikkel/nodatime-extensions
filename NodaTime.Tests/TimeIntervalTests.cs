using System;
using AutoFixture;
using FluentAssertions;
using NodaTime;
using NodaTime.Tests.AutoFixture;
using NodaTime.AutoFixture;
using Xunit;

// ReSharper disable EqualExpressionComparison
// CS1718: Comparison made to same variable.  This is intentional to test operator ==.
#pragma warning disable CS1718 // Comparison made to same variable

namespace NodaTime.Tests
{
    public class TimeIntervalTests
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

        #region Constructor

        [Fact]
        public void Construction_StartBeforeEnd()
        {
            // Arrange
            var (start, end) = Fixture.Create<LocalTime>((x, y) => x < y);

            // Act
            var interval = new TimeInterval(start, end);

            // Assert
            interval.Start.Should().Be(start);
            interval.End.Should().Be(end);
        }

        [Fact]
        public void Construction_EqualStartAndEnd()
        {
            // Arrange
            var time = Fixture.Create<LocalTime>();

            // Act
            var interval = new TimeInterval(time, time);

            // Assert
            interval.Start.Should().Be(time);
            interval.End.Should().Be(time);
        }

        [Fact]
        public void Construction_StartAfterEnd()
        {
            // Arrange
            var (start, end) = Fixture.Create<LocalTime>((x, y) => x > y);

            // Act
            var interval = new TimeInterval(start, end);

            // Assert
            interval.Start.Should().Be(start);
            interval.End.Should().Be(end);
        }

        [Fact]
        public void Construction_Default()
        {
            // Arrange
            var time = default(LocalTime);

            // Act
            var interval = new TimeInterval(time, time);

            // Assert
            interval.Start.Should().Be(time);
            interval.End.Should().Be(time);
        }

        #endregion Constructor

        #region Equality

        [Fact]
        public void Equals_SameInstance()
        {
            // Arrange
            var interval = Fixture.Create<TimeInterval>();

            // Assert
            interval.Should().Be(interval);
            interval.GetHashCode().Should().Be(interval.GetHashCode());
            (interval == interval).Should().BeTrue();
            (interval != interval).Should().BeFalse();
            interval.Equals(interval).Should().BeTrue(); // IEquatable implementation
        }

        [Fact]
        public void Equals_EqualValues()
        {
            // Arrange
            var interval1 = Fixture.Create<TimeInterval>();
            var interval2 = new TimeInterval(interval1.Start, interval1.End);

            // Assert
            interval1.Should().Be(interval2);
            interval1.GetHashCode().Should().Be(interval2.GetHashCode());
            (interval1 == interval2).Should().BeTrue();
            (interval1 != interval2).Should().BeFalse();
            interval1.Equals(interval2).Should().BeTrue(); // IEquatable implementation
        }

        [Fact]
        public void Equals_DifferentStart()
        {
            // Arrange
            var (start1, start2) = Fixture.Create<LocalTime>((x, y) => x != y);
            var end = Fixture.Create<LocalTime>();
            var interval1 = new TimeInterval(start1, end);
            var interval2 = new TimeInterval(start2, end);

            // Assert
            interval1.Should().NotBe(interval2);
            interval1.GetHashCode().Should().NotBe(interval2.GetHashCode());
            (interval1 == interval2).Should().BeFalse();
            (interval1 != interval2).Should().BeTrue();
            interval1.Equals(interval2).Should().BeFalse(); // IEquatable implementation
        }

        [Fact]
        public void Equals_DifferentEnd()
        {
            // Arrange
            var start = Fixture.Create<LocalTime>();
            var (end1, end2) = Fixture.Create<LocalTime>((x, y) => x != y);
            var interval1 = new TimeInterval(start, end1);
            var interval2 = new TimeInterval(start, end2);

            // Assert
            interval1.Should().NotBe(interval2);
            interval1.GetHashCode().Should().NotBe(interval2.GetHashCode());
            (interval1 == interval2).Should().BeFalse();
            (interval1 != interval2).Should().BeTrue();
            interval1.Equals(interval2).Should().BeFalse(); // IEquatable implementation
        }

        [Fact]
        public void Equals_DifferentToNull()
        {
            // Arrange
            var interval = Fixture.Create<TimeInterval>();

            // Assert
            interval.Should().NotBe(null);
        }

        [Fact]
        public void Equals_DifferentToOtherType()
        {
            // Arrange
            var interval = Fixture.Create<TimeInterval>();
            var instant = Fixture.Create<Instant>();

            // Assert
            interval.Should().NotBe(instant);
        }

        #endregion Equality

        #region Contains time

        public static TheoryData<LocalTime, bool> ContainsTimeStartBeforeEndExamples = new TheoryData<LocalTime, bool>
        {
            { new LocalTime(00, 00, 00), false },
            { new LocalTime(03, 41, 57), false },
            { new LocalTime(08, 00, 00) - Period.FromNanoseconds(1), false },
            { new LocalTime(08, 00, 00), true },
            { new LocalTime(12, 00, 00), true },
            { new LocalTime(14, 33, 23), true },
            { new LocalTime(16, 00, 00) - Period.FromNanoseconds(1), true },
            { new LocalTime(16, 00, 00), false },
            { new LocalTime(22, 18, 00), false }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeStartBeforeEndExamples))]
        public void Contains_StartBeforeEnd(LocalTime time, bool expected)
        {
            // Arrange
            var interval = new TimeInterval(
                new LocalTime(08, 00),
                new LocalTime(16, 00)
            );

            // Act
            var contains = interval.Contains(time);

            // Assert
            contains.Should().Be(expected);
        }

        public static TheoryData<LocalTime, bool> ContainsTimeStartAfterEndExamples = new TheoryData<LocalTime, bool>
        {
            { new LocalTime(00, 00, 00), true },
            { new LocalTime(03, 41, 57), true },
            { new LocalTime(08, 00, 00) - Period.FromNanoseconds(1), true },
            { new LocalTime(08, 00, 00), false },
            { new LocalTime(12, 00, 00), false },
            { new LocalTime(16, 33, 23), false },
            { new LocalTime(20, 00, 00) - Period.FromNanoseconds(1), false },
            { new LocalTime(20, 00, 00), true },
            { new LocalTime(22, 18, 00), true }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeStartAfterEndExamples))]
        public void Contains_StartAfterEnd(LocalTime time, bool expected)
        {
            // Arrange
            var interval = new TimeInterval(
                new LocalTime(20, 00),
                new LocalTime(08, 00)
            );

            // Act
            var contains = interval.Contains(time);

            // Assert
            contains.Should().Be(expected);
        }

        public static TheoryData<LocalTime, bool> ContainsTimeEqualStartAndEndExamples = new TheoryData<LocalTime, bool>
        {
            { new LocalTime(00, 00, 00), true },
            { new LocalTime(03, 41, 57), true },
            { new LocalTime(08, 00, 00), true },
            { new LocalTime(12, 00, 00), true },
            { new LocalTime(16, 33, 23), true },
            { new LocalTime(20, 00, 00), true },
            { new LocalTime(22, 18, 00), true },
            { Fixture.Create<LocalTime>(), true }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeEqualStartAndEndExamples))]
        public void Contains_EqualStartAndEnd(LocalTime time, bool expected)
        {
            // Arrange
            var startAndEnd = Fixture.Create<LocalTime>();
            var interval = new TimeInterval(startAndEnd, startAndEnd);

            // Act
            var contains = interval.Contains(time);

            // Assert
            contains.Should().Be(expected);
        }

        public static TheoryData<LocalTime, bool> ContainsTimeMidnightStartAndEndExamples = new TheoryData<LocalTime, bool>
        {
            { new LocalTime(00, 00, 00), true },
            { new LocalTime(03, 41, 57), true },
            { new LocalTime(08, 00, 00), true },
            { new LocalTime(12, 00, 00), true },
            { new LocalTime(16, 33, 23), true },
            { new LocalTime(20, 00, 00), true },
            { new LocalTime(22, 18, 00), true },
            { Fixture.Create<LocalTime>(), true }
        };

        [Theory]
        [MemberData(nameof(ContainsTimeMidnightStartAndEndExamples))]
        public void Contains_MidnightStartAndEnd(LocalTime time, bool expected)
        {
            // Arrange
            var interval = new TimeInterval(LocalTime.Midnight, LocalTime.Midnight);

            // Act
            var contains = interval.Contains(time);

            // Assert
            contains.Should().Be(expected);
        }

        #endregion Contains time

        #region Overlaps

        [Fact]
        public void Overlaps_Null_ThrowsException()
        {
            // Arrange
            var interval = Fixture.Create<TimeInterval>();

            // Act
            Action act = () => interval.Overlaps(null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: interval");
        }

        public static TheoryData<LocalTime, LocalTime, bool> OverlapsFromMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time1, true },
            { Time0, Time3, true },
            { Time0, Time5, true },
            { Time0, Time6, true },
            { Time0, Time7, true },

            { Time1, Time0, true },
            { Time1, Time1, true },
            { Time1, Time2, true },
            { Time1, Time3, true },
            { Time1, Time5, true },
            { Time1, Time6, true },
            { Time1, Time7, true },

            { Time2, Time1, true },

            { Time3, Time0, false },
            { Time3, Time1, true },
            { Time3, Time3, true },
            { Time3, Time5, false },
            { Time3, Time6, false },
            { Time3, Time7, false },

            { Time4, Time0, false },
            { Time4, Time1, true },
            { Time4, Time3, true },
            { Time4, Time4, true },
            { Time4, Time5, false },
            { Time4, Time6, false },
            { Time4, Time8, false },

            { Time5, Time4, true },

            { Time6, Time0, false },
            { Time6, Time1, true },
            { Time6, Time3, true },
            { Time6, Time4, true },
            { Time6, Time6, true },
            { Time6, Time8, false },

            { Time7, Time0, false },
            { Time7, Time1, true },
            { Time7, Time3, true },
            { Time7, Time5, true },
            { Time7, Time6, true },
            { Time7, Time7, true },
            { Time7, Time8, false },

            { Time8, Time7, true }
        };

        [Theory]
        [MemberData(nameof(OverlapsFromMidnightExamples))]
        public void Overlaps_FromMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time0, Time3);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> OverlapsDuringDayExamples = new TheoryData<LocalTime, LocalTime, bool>
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
        [MemberData(nameof(OverlapsDuringDayExamples))]
        public void Overlaps_DuringDay(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time3, Time6);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> OverlapsUntilMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
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
        [MemberData(nameof(OverlapsUntilMidnightExamples))]
        public void Overlaps_UntilMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time6, Time0);

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

            { Time6, Time0, true },
            { Time6, Time2, true },
            { Time6, Time3, true },
            { Time6, Time4, true },
            { Time6, Time6, true },
            { Time6, Time7, true },

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
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time6, Time3);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> OverlapsMidnightToMidnightExamples = new TheoryData<LocalTime, LocalTime, bool>
        {
            { Time0, Time0, true },
            { Time0, Time3, true },

            { Time3, Time0, true },
            { Time3, Time2, true },
            { Time3, Time3, true },
            { Time3, Time7, true }
        };

        [Theory]
        [MemberData(nameof(OverlapsMidnightToMidnightExamples))]
        public void Overlaps_MidnightToMidnight(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time0, Time0);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        public static TheoryData<LocalTime, LocalTime, bool> OverlapsTimeToTimeExamples = new TheoryData<LocalTime, LocalTime, bool>
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

            { Time3, Time0, true },
            { Time3, Time2, true },
            { Time3, Time3, true },
            { Time3, Time5, true },

            { Time4, Time5, true },

            { Time5, Time0, true },
            { Time5, Time2, true },
            { Time5, Time3, true },
            { Time5, Time4, true },
            { Time5, Time5, true },
            { Time5, Time8, true }
        };

        [Theory]
        [MemberData(nameof(OverlapsTimeToTimeExamples))]
        public void Overlaps_TimeToTime(LocalTime startTime, LocalTime endTime, bool expected)
        {
            // Arrange
            var interval1 = new TimeInterval(startTime, endTime);
            var interval2 = new TimeInterval(Time3, Time3);

            // Act
            var overlaps = interval1.Overlaps(interval2);

            // Assert
            overlaps.Should().Be(expected);
        }

        #endregion Overlaps

        #region Period

        [Fact]
        public void Period_StartBeforeEnd()
        {
            // Arrange
            var start = new LocalTime(08, 59, 24);
            var end = new LocalTime(15, 17, 35);
            var interval = new TimeInterval(start, end);

            var expected = new PeriodBuilder
            {
                Hours = 6,
                Minutes = 18,
                Seconds = 11
            }.Build();

            // Act
            var period = interval.Period;

            // Assert
            period.Should().Be(expected);
        }

        [Fact]
        public void Period_StartAfterEnd()
        {
            // Arrange
            var start = new LocalTime(15, 17, 35);
            var end = new LocalTime(08, 59, 24);
            var interval = new TimeInterval(start, end);

            var expected = new PeriodBuilder
            {
                Hours = 17,
                Minutes = 41,
                Seconds = 49
            }.Build();

            // Act
            var period = interval.Period;

            // Assert
            period.Should().Be(expected);
        }

        [Fact]
        public void Period_EqualStartAndEnd()
        {
            // Arrange
            var time = Fixture.Create<LocalTime>();
            var interval = new TimeInterval(time, time);

            var expected = new PeriodBuilder
            {
                Days = 1
            }.Build();

            // Act
            var period = interval.Period;

            // Assert
            period.Should().Be(expected);
        }

        #endregion Period

        #region ToString

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            var start = new LocalTime(09, 59, 33);
            var end = new LocalTime(13, 11, 42);
            var interval = new TimeInterval(start, end);

            // Act
            var s = interval.ToString();

            // Assert
            s.Should().Be("09:59:33/13:11:42");
        }

        #endregion ToString
    }
}
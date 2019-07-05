using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests.Extensions
{
    public class IntervalSequenceExtensionsTests
    {

        private static Instant Instant1 = Instant.FromUtc(2018, 01, 01, 03, 34, 56);
        private static Instant Instant2 = Instant.FromUtc(2018, 01, 05, 14, 49, 19);
        private static Instant Instant3 = Instant.FromUtc(2018, 01, 14, 12, 19, 02);
        private static Instant Instant4 = Instant.FromUtc(2018, 01, 16, 04, 07, 57);

        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        [Fact]
        public void IntersectOfEmptySetsOfIntervalsIsEmpty()
        {
            var setA = new List<Interval>
            {

            };

            var setB = new List<Interval>
            {

            };

            var intersect = new[] { setA, setB }.GetOverlapsBetweenSets();

            intersect.Should().BeEmpty();
        }

        [Fact]
        public void IntersectOfDistinctSetsOfIntervalsIsEmpty()
        {
            var setA = new List<Interval>
            {
                new Interval(SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1)),SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1)).Plus(Duration.FromHours(1)))
            };

            var setB = new List<Interval>
            {
                new Interval(SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(2)),SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(2)).Plus(Duration.FromHours(1)))
            };

            var intersect = new[] { setA, setB }.GetOverlapsBetweenSets();

            intersect.Should().BeEmpty();
        }

        [Fact]
        public void IntersectOfIdenticalSetsOfIntervals()
        {
            var start1 = new LocalDate(2018, 1, 1).At(new LocalTime(09, 00)).InUtc().ToInstant();
            var end1 = new LocalDate(2018, 1, 1).At(new LocalTime(17, 00)).InUtc().ToInstant();

            var setA = new List<Interval>
            {
                new Interval(start1, end1)
            };

            var setB = new List<Interval>
            {
                new Interval(start1, end1)
            };

            var intersect = new[] { setA, setB }.GetOverlapsBetweenSets().ToArray();

            intersect.Should().Contain(i => i == setA.Single());
        }

        [Fact]
        public void IntersectOfSetsOfIntervals()
        {
            var start1 = new LocalDate(2018, 1, 1).At(new LocalTime(09, 00)).InUtc().ToInstant();
            var end1 = new LocalDate(2018, 1, 1).At(new LocalTime(17, 00)).InUtc().ToInstant();

            var setA = new List<Interval>
            {
                new Interval(start1, end1)
            };

            var setB = new List<Interval>
            {
                new Interval(start1, end1)
            };

            var intersect = new[] { setA, setB }.GetOverlapsBetweenSets().ToArray();

            intersect.Should().Contain(i => i == setA.Single());
        }

        /// <summary>
        /// Qualifer:
        ///     All
        ///         Not
        ///             Breaks
        ///         Any
        ///             Between 6 and 9
        ///             Between 17 and 20
        /// 
        /// Pay +25%
        /// 
        /// Shift
        ///     Start at 7
        ///     Ends at 18
        ///     Break from 8:30 to 9:30
        ///     Break from 12 to 13
        ///     Break from 16:30 to 17:15
        /// </summary>
        [Fact]
        public void IntersectOfSetsOfIntervalsExample()
        {
            // [7, 8:30), [9:30, 12), [13, 16:30), [17:15, 18)
            var invert1 = CreateInterval(07, 00, 08, 30);
            var invert2 = CreateInterval(09, 30, 12, 00);
            var invert3 = CreateInterval(13, 00, 16, 30);
            var invert4 = CreateInterval(17, 15, 18, 00);

            var setA = new List<Interval>
            {
                invert1,
                invert2,
                invert3,
                invert4
            };

            // Any: [6, 7:15), [7:15, 9), [15, 20)
            var timeOfDay1 = CreateInterval(06, 00, 07, 15);
            var timeOfDay2 = CreateInterval(07, 15, 09, 00);
            var timeOfDay3 = CreateInterval(15, 00, 20, 00);

            var setB = new List<Interval>
            {
                timeOfDay1,
                timeOfDay2,
                timeOfDay3
            };

            var intersect = setA.GetOverlapsWith(setB).ToArray();

            // Intersection: [7, 8:30), [15, 16:30), [17:15, 18)
            intersect.Should().BeEquivalentTo(
                new[]
                {
                    CreateInterval(07, 00, 08, 30),
                    CreateInterval(15, 00, 16, 30),
                    CreateInterval(17, 15, 18, 00)
                }
            );
        }

        #region Subtract

        [Fact]
        public void Subtract_NullFirst_ThrowsException()
        {
            // Arrange
            IEnumerable<Interval> intervals = null;
            var intervalsToSubtract = Fixture.CreateMany<Interval>();

            // Act
            Action act = () => intervals.Subtract(intervalsToSubtract).ToList();

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Should().Contain(nameof(intervals));
        }

        [Fact]
        public void Subtract_NullSecond_ThrowsException()
        {
            // Arrange
            var intervals = Fixture.CreateMany<Interval>();
            IEnumerable<Interval> intervalsToSubtract = null;

            // Act
            Action act = () => intervals.Subtract(intervalsToSubtract).ToList();

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Should().Contain(nameof(intervalsToSubtract));
        }

        [Fact]
        public void Subtract_EmptyFirst_Empty()
        {
            // Arrange
            var intervals = Enumerable.Empty<Interval>();
            var intervalsToSubtract = Fixture.CreateMany<Interval>();

            // Act
            var overlaps = intervals.Subtract(intervalsToSubtract).ToList();

            // Assert
            overlaps.Should().BeEmpty();
        }

        [Fact]
        public void Subtract_EmptySecond_Empty()
        {
            // Arrange
            var intervals = Fixture.CreateMany<Interval>().ToList();
            var intervalsToSubtract = Enumerable.Empty<Interval>();
            var expected = intervals.CombineIntervals();

            // Act
            var overlaps = intervals.Subtract(intervalsToSubtract).ToList();

            // Assert
            overlaps.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<string, Interval, Interval, IEnumerable<Interval>> SubtractExamples = new TheoryData<string, Interval, Interval, IEnumerable<Interval>>
        {
            { "Before          ", new Interval(Instant1, Instant2), new Interval(Instant3, Instant4), new[]{new Interval(Instant1, Instant2)} },
            { "After           ", new Interval(Instant3, Instant4), new Interval(Instant1, Instant2), new[]{new Interval(Instant3, Instant4)} },
            { "Meets           ", new Interval(Instant1, Instant2), new Interval(Instant2, Instant3), new[]{new Interval(Instant1, Instant2)} },
            { "Is met by       ", new Interval(Instant2, Instant3), new Interval(Instant1, Instant2), new[]{new Interval(Instant2, Instant3)} },
            { "Overlaps        ", new Interval(Instant1, Instant3), new Interval(Instant2, Instant4), new[]{new Interval(Instant1, Instant2)} },
            { "Is overlapped by", new Interval(Instant2, Instant4), new Interval(Instant1, Instant3), new[]{new Interval(Instant3, Instant4)} },
            { "Starts          ", new Interval(Instant1, Instant2), new Interval(Instant1, Instant3), Enumerable.Empty<Interval>() },
            { "Is started by   ", new Interval(Instant1, Instant3), new Interval(Instant1, Instant2), new[]{new Interval(Instant2, Instant3)} },
            { "During          ", new Interval(Instant2, Instant3), new Interval(Instant1, Instant4), Enumerable.Empty<Interval>() },
            { "Contains        ", new Interval(Instant1, Instant4), new Interval(Instant2, Instant3), new[]{ new Interval(Instant1, Instant2), new Interval(Instant3, Instant4) } },
            { "Finishes        ", new Interval(Instant2, Instant3), new Interval(Instant1, Instant3), Enumerable.Empty<Interval>() },
            { "Is finished by  ", new Interval(Instant1, Instant3), new Interval(Instant2, Instant3), new[]{new Interval(Instant1, Instant2)} },
            { "Equals          ", new Interval(Instant1, Instant2), new Interval(Instant1, Instant2), Enumerable.Empty<Interval>() }
        };

        [Theory]
        [MemberData(nameof(SubtractExamples))]
        public void Subtract_SingleInterval(string name, Interval interval, Interval intervalToSubtract, IEnumerable<Interval> expected)
        {
            // Arrange
            var intervals = new[] { interval };
            var intervalsToSubtract = new[] { intervalToSubtract };

            // Act
            var result = intervals.Subtract(intervalsToSubtract).ToList();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        //     --------
        //
        // ---   - --   ---
        //          --
        //
        //     -- -   -
        [Fact]
        public void Subtract_Example1()
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(08, 00, 16, 00)
            };

            var intervalsToSubtract = new[]
            {
                CreateInterval(04, 00, 07, 00),
                CreateInterval(10, 00, 11, 00),
                CreateInterval(12, 00, 14, 00),
                CreateInterval(13, 00, 15, 00),
                CreateInterval(17, 00, 20, 00)
            };

            var expected = new[]
            {
                CreateInterval(08, 00, 10, 00),
                CreateInterval(11, 00, 12, 00),
                CreateInterval(15, 00, 16, 00)
            };

            // Act
            var leftovers = intervals.Subtract(intervalsToSubtract);

            // Assert
            leftovers.Should().BeEquivalentTo(expected);
        }

        //     --------
        //
        // -- --- -   ---   --
        //
        //       - ---
        [Fact]
        public void Subtract_Example2()
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(08, 00, 16, 00)
            };

            var intervalsToSubtract = new[]
            {
                CreateInterval(04, 00, 06, 00),
                CreateInterval(07, 00, 10, 00),
                CreateInterval(11, 00, 12, 00),
                CreateInterval(15, 00, 18, 00),
                CreateInterval(21, 00, 23, 00)
            };

            var expected = new[]
            {
                CreateInterval(10, 00, 11, 00),
                CreateInterval(12, 00, 15, 00)
            };

            // Act
            var leftovers = intervals.Subtract(intervalsToSubtract);

            // Assert
            leftovers.Should().BeEquivalentTo(expected);
        }

        //  -----
        //
        //    --  --- -   ---
        //
        //  --  -
        [Fact]
        public void Subtract_Example3()
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(05, 00, 10, 00)
            };

            var intervalsToSubtract = new[]
            {
                CreateInterval(07, 00, 09, 00),
                CreateInterval(11, 00, 14, 00),
                CreateInterval(16, 00, 17, 00),
                CreateInterval(19, 00, 22, 00)
            };

            var expected = new[]
            {
                CreateInterval(05, 00, 07, 00),
                CreateInterval(09, 00, 10, 00)
            };

            // Act
            var leftovers = intervals.Subtract(intervalsToSubtract);

            // Assert
            leftovers.Should().BeEquivalentTo(expected);
        }

        //  ----   ----  -----
        //    ---  -----  ---
        //        -----
        //
        //    --  --- -  ---
        //
        //  --  -    - -    --
        [Fact]
        public void Subtract_Example4()
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(05, 09),
                CreateInterval(07, 10),
                CreateInterval(11, 16),
                CreateInterval(12, 16),
                CreateInterval(12, 17),
                CreateInterval(18, 23),
                CreateInterval(19, 22)
            };

            var intervalsToSubtract = new[]
            {
                CreateInterval(07, 09),
                CreateInterval(11, 14),
                CreateInterval(15, 16),
                CreateInterval(18, 21)
            };

            var expected = new[]
            {
                CreateInterval(05, 07),
                CreateInterval(09, 10),
                CreateInterval(14, 15),
                CreateInterval(16, 17),
                CreateInterval(21, 23)
            };

            // Act
            var leftovers = intervals.Subtract(intervalsToSubtract).ToList();

            // Assert
            leftovers.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void Subtract_Example5()
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(07, 12),
                CreateInterval(14, 17)
            };

            var intervalsToSubtract = new[]
            {
                CreateInterval(08, 09),
                CreateInterval(10, 11),
                CreateInterval(15, 16) // Missing
            };

            var expected = new[]
            {
                CreateInterval(07, 08),
                CreateInterval(09, 10),
                CreateInterval(11, 12),
                CreateInterval(14, 15),
                CreateInterval(16, 17)
            };

            // Act
            var leftovers = intervals.Subtract(intervalsToSubtract).ToList();

            // Assert
            leftovers.Should().BeEquivalentTo(expected);
        }

        private Interval CreateInterval(int startHour, int startMinute, int endHour, int endMinute)
        {
            var start = Instant.FromUtc(2018, 01, 01, startHour, startMinute);
            var end = Instant.FromUtc(2018, 01, 01, endHour, endMinute);
            return new Interval(start, end);
        }

        private Interval CreateInterval(int startHour, int endHour)
        {
            var start = Instant.FromUtc(2018, 01, 01, startHour, 00);
            var end = Instant.FromUtc(2018, 01, 01, endHour, 00);
            return new Interval(start, end);
        }

        #endregion

        #region GetOverlapsWith

        [Fact]
        public void GetOverlapsWith_NullFirst_ThrowsException()
        {
            // Arrange
            IEnumerable<Interval> first = null;
            var second = Fixture.CreateMany<Interval>();

            // Act
            Action act = () => first.GetOverlapsWith(second).ToList();

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Should().Contain(nameof(first));
        }

        [Fact]
        public void GetOverlapsWith_NullSecond_ThrowsException()
        {
            // Arrange
            var first = Fixture.CreateMany<Interval>();
            IEnumerable<Interval> second = null;

            // Act
            Action act = () => first.GetOverlapsWith(second).ToList();

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Should().Contain(nameof(second));
        }

        [Fact]
        public void GetOverlapsWith_EmptyFirst_Empty()
        {
            // Arrange
            var first = Enumerable.Empty<Interval>();
            var second = Fixture.CreateMany<Interval>();

            // Act
            var overlaps = first.GetOverlapsWith(second).ToList();

            // Assert
            overlaps.Should().BeEmpty();
        }

        [Fact]
        public void GetOverlapsWith_EmptySecond_Empty()
        {
            // Arrange
            var first = Fixture.CreateMany<Interval>();
            var second = Enumerable.Empty<Interval>();

            // Act
            var overlaps = first.GetOverlapsWith(second).ToList();

            // Assert
            overlaps.Should().BeEmpty();
        }

        public static TheoryData<string, Interval, Interval, Interval?> SingleIntervalOverlapExamples = new TheoryData<string, Interval, Interval, Interval?>
        {
            { "Before          ", new Interval(Instant1, Instant2), new Interval(Instant3, Instant4), default(Interval?) },
            { "After           ", new Interval(Instant3, Instant4), new Interval(Instant1, Instant2), default(Interval?) },
            { "Meets           ", new Interval(Instant1, Instant2), new Interval(Instant2, Instant3), default(Interval?) },
            { "Is met by       ", new Interval(Instant2, Instant3), new Interval(Instant1, Instant2), default(Interval?) },
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
        [MemberData(nameof(SingleIntervalOverlapExamples))]
        public void GetOverlapsWith_SingleInterval(string name, Interval firstInterval, Interval secondInterval, Interval? overlap)
        {
            // Arrange
            var first = new[] { firstInterval };
            var second = new[] { secondInterval };

            // Act
            var overlaps = first.GetOverlapsWith(second).ToList();

            // Assert
            var singleOrDefault = overlaps.Cast<Interval?>().SingleOrDefault();
            singleOrDefault.Should().Be(overlap);
        }

        #endregion

        #region MyRegion
        
        [Fact]
        public void DetermineWhenDurationHasElapsed_Null_ThrowsException()
        {
            // Arrange
            IEnumerable<Interval> intervals = null;
            var duration = Fixture.Create<Duration>();

            // Act
            Action act = () => intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Should().Contain(nameof(intervals));
        }
        
        [Fact]
        public void DetermineWhenDurationHasElapsed_NegativeDuration_ThrowsException()
        {
            // Arrange
            var intervals = Fixture.CreateMany<Interval>();
            var duration = Duration.FromHours(-5);

            // Act
            Action act = () => intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().And.Message.Should().Contain(nameof(duration));
        }

        [Fact]
        public void DetermineWhenDurationHasElapsed_NoIntervalsZeroDuration_NoResult()
        {
            // Arrange
            var intervals = Enumerable.Empty<Interval>();
            var duration = Duration.Zero;

            // Act
            var result = intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.HasValue.Should().BeFalse();
        }

        [Fact]
        public void DetermineWhenDurationHasElapsed_NoIntervalsSomeDuration_NoResult()
        {
            // Arrange
            var intervals = Enumerable.Empty<Interval>();
            var duration = Fixture.Create<Duration>();

            // Act
            var result = intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.HasValue.Should().BeFalse();
        }

        [Fact]
        public void DetermineWhenDurationHasElapsed_ZeroDuration_StartOfFirstShift()
        {
            // Arrange
            var intervals = Fixture.CreateMany<Interval>().ToList();
            var duration = Duration.Zero;
            var expected = intervals.Min(x => x.Start);

            // Act
            var result = intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void DetermineWhenDurationHasElapsed_ShiftDuration_EndOfShift()
        {
            // Arrange
            var interval = Fixture.Create<Interval>();
            var duration = interval.Duration;
            var expected = interval.End;

            // Act
            var result = new[]{ interval }.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void DetermineWhenDurationHasElapsed_ShiftDurationForFixedShift_EndOfShift()
        {
            // Arrange
            var start = Instant.FromUnixTimeTicks(-4995577169482928);
            var end = Instant.FromUnixTimeTicks(35989830569884344);
            var interval = new Interval(start, end);
            var duration = interval.Duration;
            var expected = start + duration;

            // Act
            var result = new[]{ interval }.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Test3()
        {
            // Arrange
            var interval = new Interval(
                Instant.FromUnixTimeTicks(1),
                Instant.FromUnixTimeTicks(1000000000000000)
            );

            // Act
            var x =  Duration.FromNanoseconds(interval.Duration.ToBigIntegerNanoseconds());

            // Assert
            Assert.Equal(x, interval.Duration);
        }

        [Fact]
        public void DetermineWhenDurationHasElapsed_NonOverlappingIntervals()
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(02, 06),
                CreateInterval(07, 12),
                CreateInterval(12, 17),
                CreateInterval(19, 23)
            };
            var duration = Duration.FromHours(8);
            var expected = Instant.FromUtc(2018, 01, 01, 11, 00, 00);

            // Act
            var result = intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void DetermineWhenDurationHasElapsed_FewerHoursThanDuration_NoResult()
        {
            // Arrange
            var intervals = Fixture.CreateMany<Interval>().ToList();
            var duration = intervals.Sum(i => i.Duration) + Duration.FromHours(1);

            // Act
            var result = intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.HasValue.Should().BeFalse();
        }

        [Fact]
        public void DetermineWhenDurationHasElapsed_SameStart()
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(05, 07),
                CreateInterval(05, 12),
                CreateInterval(05, 10),
                CreateInterval(05, 17),
                CreateInterval(06, 12)
            };
            var duration = Duration.FromHours(6.5);
            var expected = Instant.FromUtc(2018, 01, 01, 06, 30);

            // Act
            var result = intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.Should().Be(expected);
        }

        public static TheoryData<Duration, Instant?> Example1 = new TheoryData<Duration, Instant?>
        {
            { Duration.FromHours(20), Instant.FromUtc(2018, 01, 01, 11, 20) },
            { Duration.FromHours(00), Instant.FromUtc(2018, 01, 01, 00, 00) },
            { Duration.FromHours(02.25), Instant.FromUtc(2018, 01, 01, 02, 15) },
            { Duration.FromHours(04), Instant.FromUtc(2018, 01, 01, 04, 00) },
            { Duration.FromHours(08), Instant.FromUtc(2018, 01, 01, 06, 00) },
            { Duration.FromHours(26), Instant.FromUtc(2018, 01, 01, 14, 00) },
            { Duration.FromHours(32), Instant.FromUtc(2018, 01, 01, 17, 00) },
            { Duration.FromHours(33), Instant.FromUtc(2018, 01, 01, 18, 00) },
            { Duration.FromHours(34), default(Instant?) }
        };

        [Theory]
        [MemberData(nameof(Example1))]
        public void DetermineWhenDurationHasElapsed_Example1(Duration duration, Instant? expected)
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(00, 08),
                CreateInterval(04, 12),
                CreateInterval(09, 17),
                CreateInterval(09, 18)
            };

            // Act
            var result = intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.Should().Be(expected);
        }

            [Fact]
        public void DetermineWhenDurationHasElapsed_Example2()
        {
            // Arrange
            var intervals = new[]
            {
                CreateInterval(00, 16), // 11:30
                CreateInterval(04, 10), // 07:30
                CreateInterval(07, 13)  // 04:30
            };
            var duration = Duration.FromHours(23.5);
            var expected = Instant.FromUtc(2018, 01, 01, 12, 15);

            // Act
            var result = intervals.DetermineWhenDurationHasElapsed(duration);

            // Assert
            result.Should().Be(expected);
        }

        #endregion
    }
}
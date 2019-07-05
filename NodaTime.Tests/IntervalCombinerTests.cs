using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NodaTime.Tests.AutoFixture;
using NodaTime.AutoFixture;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests
{
    public class IntervalCombinerTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();
        private static readonly Random Random = new Random();

        private static Instant Instant1 = Instant.FromUtc(2018, 01, 01, 03, 34, 56);
        private static Instant Instant2 = Instant.FromUtc(2018, 01, 05, 14, 49, 19);
        private static Instant Instant3 = Instant.FromUtc(2018, 01, 14, 12, 19, 02);
        private static Instant Instant4 = Instant.FromUtc(2018, 01, 16, 04, 07, 57);

        #region CombineOverlappingIntervals

        [Fact]
        public void CombineOverlappingIntervals_Null_ThrowsException()
        {
            // Arrange
            IEnumerable<Interval> intervals = null;

            // Act
            Action act = () => intervals.CombineOverlappingIntervals().ToList();

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Contains("source");
        }

        [Fact]
        public void CombineOverlappingIntervals_EmptyEnumerable_Empty()
        {
            // Arrange
            var intervals = Enumerable.Empty<Interval>();

            // Act
            var overlappingIntervals = intervals.CombineOverlappingIntervals();

            // Assert
            overlappingIntervals.Should().BeEmpty();
        }

        [Fact]
        public void CombineOverlappingIntervals_SingleInterval()
        {

            // Arrange
            var interval = Fixture.Create<Interval>();
            var intervals = new[] { interval };

            // Act
            var overlappingIntervals = intervals.CombineOverlappingIntervals();

            // Assert
            overlappingIntervals.Single().Should().Be(interval);
        }

        [Fact]
        public void CombineOverlappingIntervals_NonOverlappingMeetingIntervals_Same()
        {
            // Arrange
            var intervals = ToIntervals(Fixture.CreateMany<Instant>()).ToList();

            // Act
            var overlappingIntervals = intervals.CombineOverlappingIntervals();

            // Assert
            overlappingIntervals.SequenceEqual(intervals).Should().BeTrue();
        }

        public static TheoryData<string, IEnumerable<Interval>, IEnumerable<Interval>> CombineOverlappingIntervalsTwoIntervalExamples = new TheoryData<string, IEnumerable<Interval>, IEnumerable<Interval>>
        {
            { "Before          ", new[] { new Interval(Instant1, Instant2), new Interval(Instant3, Instant4) },  new[] { new Interval(Instant1, Instant2), new Interval(Instant3, Instant4) } },
            { "After           ", new[] { new Interval(Instant3, Instant4), new Interval(Instant1, Instant2) },  new[] { new Interval(Instant1, Instant2), new Interval(Instant3, Instant4) } },
            { "Meets           ", new[] { new Interval(Instant1, Instant2), new Interval(Instant2, Instant3) },  new[] { new Interval(Instant1, Instant2), new Interval(Instant2, Instant3) } },
            { "Is met by       ", new[] { new Interval(Instant2, Instant3), new Interval(Instant1, Instant2) },  new[] { new Interval(Instant1, Instant2), new Interval(Instant2, Instant3) } },
            { "Overlaps        ", new[] { new Interval(Instant1, Instant3), new Interval(Instant2, Instant4) },  new[] { new Interval(Instant1, Instant4) } },
            { "Is overlapped by", new[] { new Interval(Instant2, Instant4), new Interval(Instant1, Instant3) },  new[] { new Interval(Instant1, Instant4) } },
            { "Starts          ", new[] { new Interval(Instant1, Instant2), new Interval(Instant1, Instant3) },  new[] { new Interval(Instant1, Instant3) } },
            { "Is started by   ", new[] { new Interval(Instant1, Instant3), new Interval(Instant1, Instant2) },  new[] { new Interval(Instant1, Instant3) } },
            { "During          ", new[] { new Interval(Instant2, Instant3), new Interval(Instant1, Instant4) },  new[] { new Interval(Instant1, Instant4) } },
            { "Contains        ", new[] { new Interval(Instant1, Instant4), new Interval(Instant2, Instant3) },  new[] { new Interval(Instant1, Instant4) } },
            { "Finishes        ", new[] { new Interval(Instant2, Instant3), new Interval(Instant1, Instant3) },  new[] { new Interval(Instant1, Instant3) } },
            { "Is finished by  ", new[] { new Interval(Instant1, Instant3), new Interval(Instant2, Instant3) },  new[] { new Interval(Instant1, Instant3) } },
            { "Equals          ", new[] { new Interval(Instant1, Instant2), new Interval(Instant1, Instant2) },  new[] { new Interval(Instant1, Instant2) } }
        };

        [Theory]
        [MemberData(nameof(CombineOverlappingIntervalsTwoIntervalExamples))]
        public void CombineOverlappingIntervals_TwoIntervals(string name, IEnumerable<Interval> intervals, IEnumerable<Interval> expected)
        {
            // Act
            var overlappingIntervals = intervals.CombineOverlappingIntervals();

            // Assert
            overlappingIntervals.SequenceEqual(expected).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(CombineOverlappingIntervalsTwoIntervalExamples))]
        public void CombineOverlappingIntervals_TwoIntervalsWithOneBefore(string name, IEnumerable<Interval> intervals, IEnumerable<Interval> expected)
        {
            // Arrange
            var interval = Fixture.Create<Interval>(x => x.End <= Instant1);
            intervals = new[] { interval }.Concat(intervals);
            expected = new[] { interval }.Concat(expected);

            // Act
            var overlappingIntervals = intervals.CombineOverlappingIntervals();

            // Assert
            overlappingIntervals.SequenceEqual(expected).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(CombineOverlappingIntervalsTwoIntervalExamples))]
        public void CombineOverlappingIntervals_TwoIntervalsWithOneAfter(string name, IEnumerable<Interval> intervals, IEnumerable<Interval> expected)
        {
            // Arrange
            var interval = Fixture.Create<Interval>(x => Instant4 <= x.Start);
            intervals = intervals.Concat(new[] { interval });
            expected = expected.Concat(new[] { interval });

            // Act
            var overlappingIntervals = intervals.CombineOverlappingIntervals();

            // Assert
            overlappingIntervals.SequenceEqual(expected).Should().BeTrue();
        }

        [Fact]
        public void CombineOverlappingIntervals_ContainingInterval()
        {
            // Arrange
            var containedIntervals = Fixture.CreateMany<Interval>().ToList();
            var start = containedIntervals.Min(i => i.Start);
            var end = containedIntervals.Max(i => i.End);
            var containingInterval = new Interval(start, end);
            var index = Random.Next(containedIntervals.Count);
            containedIntervals.Insert(index, containingInterval);

            // Act
            var overlappingIntervals = containedIntervals.CombineOverlappingIntervals();

            // Assert
            overlappingIntervals.Single().Should().Be(containingInterval);
        }

        #endregion

        #region CombineIntervals

        [Fact]
        public void CombineIntervals_Null_ThrowsException()
        {
            // Arrange
            IEnumerable<Interval> intervals = null;

            // Act
            Action act = () => intervals.CombineIntervals().ToList();

            // Assert
            act.Should().Throw<ArgumentNullException>().And.Message.Contains("source");
        }

        [Fact]
        public void CombineIntervals_EmptyEnumerable_Empty()
        {
            // Arrange
            var intervals = Enumerable.Empty<Interval>();

            // Act
            var overlappingIntervals = intervals.CombineIntervals();

            // Assert
            overlappingIntervals.Should().BeEmpty();
        }

        [Fact]
        public void CombineIntervals_SingleInterval()
        {

            // Arrange
            var interval = Fixture.Create<Interval>();
            var intervals = new[] { interval };

            // Act
            var overlappingIntervals = intervals.CombineIntervals();

            // Assert
            overlappingIntervals.Single().Should().Be(interval);
        }

        [Fact]
        public void CombineIntervals_NonOverlappingMeetingIntervals_WholeSpan()
        {
            // Arrange
            var instants = Fixture.CreateMany<Instant>().ToList();
            var intervals = ToIntervals(instants).ToList();
            var expected = new[] { new Interval(instants.Min(), instants.Max()) };

            // Act
            var overlappingIntervals = intervals.CombineIntervals();

            // Assert
            overlappingIntervals.SequenceEqual(expected).Should().BeTrue();
        }

        public static TheoryData<string, IEnumerable<Interval>, IEnumerable<Interval>> CombineIntervalsTwoIntervalExamples = new TheoryData<string, IEnumerable<Interval>, IEnumerable<Interval>>
        {
            { "Before          ", new[] { new Interval(Instant1, Instant2), new Interval(Instant3, Instant4) },  new[] { new Interval(Instant1, Instant2), new Interval(Instant3, Instant4) } },
            { "After           ", new[] { new Interval(Instant3, Instant4), new Interval(Instant1, Instant2) },  new[] { new Interval(Instant1, Instant2), new Interval(Instant3, Instant4) } },
            { "Meets           ", new[] { new Interval(Instant1, Instant2), new Interval(Instant2, Instant3) },  new[] { new Interval(Instant1, Instant3) } },
            { "Is met by       ", new[] { new Interval(Instant2, Instant3), new Interval(Instant1, Instant2) },  new[] { new Interval(Instant1, Instant3) } },
            { "Overlaps        ", new[] { new Interval(Instant1, Instant3), new Interval(Instant2, Instant4) },  new[] { new Interval(Instant1, Instant4) } },
            { "Is overlapped by", new[] { new Interval(Instant2, Instant4), new Interval(Instant1, Instant3) },  new[] { new Interval(Instant1, Instant4) } },
            { "Starts          ", new[] { new Interval(Instant1, Instant2), new Interval(Instant1, Instant3) },  new[] { new Interval(Instant1, Instant3) } },
            { "Is started by   ", new[] { new Interval(Instant1, Instant3), new Interval(Instant1, Instant2) },  new[] { new Interval(Instant1, Instant3) } },
            { "During          ", new[] { new Interval(Instant2, Instant3), new Interval(Instant1, Instant4) },  new[] { new Interval(Instant1, Instant4) } },
            { "Contains        ", new[] { new Interval(Instant1, Instant4), new Interval(Instant2, Instant3) },  new[] { new Interval(Instant1, Instant4) } },
            { "Finishes        ", new[] { new Interval(Instant2, Instant3), new Interval(Instant1, Instant3) },  new[] { new Interval(Instant1, Instant3) } },
            { "Is finished by  ", new[] { new Interval(Instant1, Instant3), new Interval(Instant2, Instant3) },  new[] { new Interval(Instant1, Instant3) } },
            { "Equals          ", new[] { new Interval(Instant1, Instant2), new Interval(Instant1, Instant2) },  new[] { new Interval(Instant1, Instant2) } }
        };

        [Theory]
        [MemberData(nameof(CombineIntervalsTwoIntervalExamples))]
        public void CombineIntervals_TwoIntervals(string name, IEnumerable<Interval> intervals, IEnumerable<Interval> expected)
        {
            // Act
            var overlappingIntervals = intervals.CombineIntervals();

            // Assert
            overlappingIntervals.SequenceEqual(expected).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(CombineIntervalsTwoIntervalExamples))]
        public void CombineIntervals_TwoIntervalsWithOneBefore(string name, IEnumerable<Interval> intervals, IEnumerable<Interval> expected)
        {
            // Arrange
            var interval = Fixture.Create<Interval>(x => x.End <= Instant1);
            intervals = new[] { interval }.Concat(intervals);
            expected = new[] { interval }.Concat(expected);

            // Act
            var overlappingIntervals = intervals.CombineIntervals();

            // Assert
            overlappingIntervals.SequenceEqual(expected).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(CombineIntervalsTwoIntervalExamples))]
        public void CombineIntervals_TwoIntervalsWithOneAfter(string name, IEnumerable<Interval> intervals, IEnumerable<Interval> expected)
        {
            // Arrange
            var interval = Fixture.Create<Interval>(x => Instant4 <= x.Start);
            intervals = intervals.Concat(new[] { interval });
            expected = expected.Concat(new[] { interval });

            // Act
            var overlappingIntervals = intervals.CombineIntervals();

            // Assert
            overlappingIntervals.SequenceEqual(expected).Should().BeTrue();
        }

        [Fact]
        public void CombineIntervals_ContainingInterval()
        {
            // Arrange
            var containedIntervals = Fixture.CreateMany<Interval>().ToList();
            var start = containedIntervals.Min(i => i.Start);
            var end = containedIntervals.Max(i => i.End);
            var containingInterval = new Interval(start, end);
            var index = Random.Next(containedIntervals.Count);
            containedIntervals.Insert(index, containingInterval);

            // Act
            var overlappingIntervals = containedIntervals.CombineIntervals();

            // Assert
            overlappingIntervals.Single().Should().Be(containingInterval);
        }

        #endregion

        #region Helper methods

        private static IEnumerable<Interval> ToIntervals(IEnumerable<Instant> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return ConsecutivePairsOfElements(source.OrderBy(x => x)).Select<(Instant, Instant), Interval>(pair => new Interval(pair.Item1, pair.Item2));
        }

        private static IEnumerable<(T, T)> ConsecutivePairsOfElements<T>(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }

                var previous = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;

                    yield return (previous, current);

                    previous = current;
                }
            }
        }

        #endregion
    }
}

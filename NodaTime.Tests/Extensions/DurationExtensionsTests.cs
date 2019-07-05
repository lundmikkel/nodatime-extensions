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
    public class DurationExtensionsTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        #region Sum

        [Fact]
        public void Sum_Null_ThrowsException()
        {
            // Arrange
            IEnumerable<Duration> durations = null;

            // Act
            Action act = () => durations.Sum();

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Sum_Empty_Zero()
        {
            // Arrange
            var durations = Enumerable.Empty<Duration>();

            // Act
            var sum = durations.Sum();

            // Assert
            sum.Should().Be(Duration.Zero);
        }

        [Fact]
        public void Sum_Zeroes_Zero()
        {
            // Arrange
            var durations = Enumerable.Range( 0, 10 ).Select(x => Duration.Zero);

            // Act
            var sum = durations.Sum();

            // Assert
            sum.Should().Be(Duration.Zero);
        }

        [Fact]
        public void Sum()
        {
            // Arrange
            var durations = Fixture.CreateMany<Duration>().ToList();
            var expected = Duration.FromTicks(durations.Select(d => d.TotalTicks).Sum());

            // Act
            var sum = durations.Sum();

            // Assert
            sum.Should().Be(expected);
        }

        #endregion

        #region Nullable sum

        [Fact]
        public void NullableSum_Null_ThrowsException()
        {
            // Arrange
            IEnumerable<Duration?> durations = null;

            // Act
            Action act = () => durations.Sum();

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void NullableSum_Empty_Zero()
        {
            // Arrange
            var durations = Enumerable.Empty<Duration?>();

            // Act
            var sum = durations.Sum();

            // Assert
            sum.Should().Be(Duration.Zero);
        }

        [Fact]
        public void NullableSum_Zeroes_Zero()
        {
            // Arrange
            var durations = Enumerable.Range( 0, 10 ).Select(x => (Duration?) Duration.Zero);

            // Act
            var sum = durations.Sum();

            // Assert
            sum.Should().Be(Duration.Zero);
        }

        [Fact]
        public void NullableSum_Nulls_Zero()
        {
            // Arrange
            var durations = Enumerable.Range( 0, 10 ).Select(x => default(Duration?));

            // Act
            var sum = durations.Sum();

            // Assert
            sum.Should().Be(Duration.Zero);
        }

        [Fact]
        public void NullableSum()
        {
            // Arrange
            var durations = Fixture.CreateMany<Duration?>().ToList();
            var expected = Duration.FromTicks(durations.Select(d => d?.TotalTicks ?? 0).Sum());

            // Act
            var sum = durations.Sum();

            // Assert
            sum.Should().Be(expected);
        }

        [Fact]
        public void NullableSum_Mixed()
        {
            // Arrange
            var durations = new[] { default(Duration?), Duration.Epsilon, default( Duration? ), Duration.Zero, Duration.FromTicks(1234), Duration.Zero  };
            var expected = Duration.Epsilon + Duration.FromTicks(1234);

            // Act
            var sum = durations.Sum();

            // Assert
            sum.Should().Be(expected);
        }

        #endregion

        #region Sum selector

        private class Break
        {
            public string Description { get; set; }
            public Duration Duration { get; set; }
            public int Number { get; set; }
        }

        private static readonly Func<Break, Duration> DurationSelector = b => b.Duration;

        [Fact]
        public void SumSelector_NullSource_ThrowsException()
        {
            // Arrange
            IEnumerable<Break> breaks = null;

            // Act
            Action act = () => breaks.Sum( DurationSelector );

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void SumSelector_NullSelector_ThrowsException()
        {
            // Arrange
            var breaks = Fixture.CreateMany<Break>();
            Func<Break, Duration> selector = null;

            // Act
            Action act = () => breaks.Sum(selector);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void SumSelector_Empty_Zero()
        {
            // Arrange
            var breaks = Enumerable.Empty<Break>();

            // Act
            var sum = breaks.Sum( DurationSelector );

            // Assert
            sum.Should().Be( Duration.Zero );
        }

        [Fact]
        public void SumSelector_Zeroes_Zero()
        {
            // Arrange
            var breaks = Enumerable.Range( 0, 10 ).Select( x => new Break{ Duration = Duration.Zero } );

            // Act
            var sum = breaks.Sum( DurationSelector );

            // Assert
            sum.Should().Be( Duration.Zero );
        }

        [Fact]
        public void SumSelector()
        {
            // Arrange
            var breaks = Fixture.CreateMany<Break>().ToList();
            var expected = Duration.FromTicks( breaks.Select( d => d.Duration.TotalTicks ).Sum() );

            // Act
            var sum = breaks.Sum(DurationSelector);

            // Assert
            sum.Should().Be( expected );
        }

        #endregion
    }
}

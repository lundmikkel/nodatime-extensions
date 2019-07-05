using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NodaTime;
using NodaTime.AutoFixture;
using Xunit;

namespace NodaTime.Tests
{
    public class IsoDayOfWeekSetTests
    {
        private static readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();
        
        #region Get enumerator

        [Fact]
        public void GetEnumerator_Empty_Empty()
        {
            // Arrange
            var set = new IsoDayOfWeekSet();

            // Act
            var list = set.ToList();

            // Assert
            list.Should().BeEmpty();
        }

        [Fact]
        public void GetEnumerator_SingleDay_SingleDay()
        {
            // Arrange
            var day = Fixture.Create<IsoDayOfWeek>();
            var set = new IsoDayOfWeekSet { day };
            var expected = new[] { day };

            // Act
            var list = set.ToList();

            // Assert
            list.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetEnumerator_SomeDay()
        {
            // Arrange
            var days = Fixture.CreateMany<IsoDayOfWeek>().ToList();
            var set = new IsoDayOfWeekSet(days);
            var expected = days.Distinct().ToList();

            // Act
            var list = set.ToList();

            // Assert
            list.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetEnumerator_AllDays()
        {
            // Arrange
            var allDays = IsoDayOfWeekSet.WithAllDays;
            var set = new IsoDayOfWeekSet(allDays);

            // Act
            var list = set.ToList();

            // Assert
            list.Should().BeEquivalentTo(allDays);
        }

        #endregion

        #region Add

        [Theory]
        [InlineData(IsoDayOfWeek.None)]
        [InlineData((IsoDayOfWeek)8)]
        public void Add_InvalidDay_ThrowsException(IsoDayOfWeek day)
        {
            // Arrange
            var set = new IsoDayOfWeekSet();

            // Act
            Action act = () => set.Add(day);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Add_Empty_Added()
        {
            // Arrange
            var set = new IsoDayOfWeekSet();
            var day = Fixture.Create<IsoDayOfWeek>();

            // Act
            set.Add(day);

            // Assert
            set.Should().Contain(day);
        }

        [Fact]
        public void Add_NonContainedDay_Added()
        {
            // Arrange
            var day = Fixture.Create<IsoDayOfWeek>();
            var days = Fixture.CreateMany<IsoDayOfWeek>().Where(d => d != day).ToList();
            var set = new IsoDayOfWeekSet(days);
            set.Should().NotContain(day);

            // Act
            set.Add(day);

            // Assert
            set.Should().Contain(day);
        }

        [Fact]
        public void Add_ContainedDay_True()
        {
            // Arrange
            var days = Fixture.CreateMany<IsoDayOfWeek>().ToList();
            var set = new IsoDayOfWeekSet(days);
            var day = days.Last();

            // Act
            set.Add(day);

            // Assert
            set.Should().Contain(day);
        }

        [Fact]
        public void Add_AllDays_True()
        {
            // Arrange
            var days = IsoDayOfWeekSet.WithAllDays;
            var set = new IsoDayOfWeekSet(days);
            var day = Fixture.Create<IsoDayOfWeek>();

            // Act
            set.Add(day);

            // Assert
            set.Should().Contain(day);
        }

        #endregion

        #region Clear

        [Fact]
        public void Clear_Empty_Empty()
        {
            // Arrange
            var set = new IsoDayOfWeekSet();

            // Act
            set.Clear();

            // Assert
            set.Should().BeEmpty();
        }

        [Fact]
        public void Clear_RandomDays_Empty()
        {
            // Arrange
            var days = Fixture.CreateMany<IsoDayOfWeek>();
            var set = new IsoDayOfWeekSet(days);

            // Act
            set.Clear();

            // Assert
            set.Should().BeEmpty();
        }

        [Fact]
        public void Count_AllDays_Empty()
        {
            // Arrange
            var days = IsoDayOfWeekSet.WithAllDays;
            var set = new IsoDayOfWeekSet(days);

            // Act
            set.Clear();

            // Assert
            set.Should().BeEmpty();
        }

        #endregion

        #region Contains

        [Theory]
        [InlineData(IsoDayOfWeek.None)]
        [InlineData((IsoDayOfWeek)8)]
        public void Contains_InvalidDay_False(IsoDayOfWeek day)
        {
            // Arrange
            var set = new IsoDayOfWeekSet();

            // Act
            var contains = set.Contains(day);

            // Assert
            contains.Should().BeFalse();
        }

        [Fact]
        public void Contains_Empty_False()
        {
            // Arrange
            var set = new IsoDayOfWeekSet();
            var day = Fixture.Create<IsoDayOfWeek>();

            // Act
            var contains = set.Contains(day);

            // Assert
            contains.Should().BeFalse();
        }

        [Fact]
        public void Contains_NonContainedDay_False()
        {
            // Arrange
            var day = Fixture.Create<IsoDayOfWeek>();
            var days = Fixture.CreateMany<IsoDayOfWeek>().Where(d => d != day).ToList();
            var set = new IsoDayOfWeekSet(days);
            set.Should().NotContain(day);

            // Act
            var contains = set.Contains(day);

            // Assert
            contains.Should().BeFalse();
        }

        [Fact]
        public void Contains_ContainedDay_True()
        {
            // Arrange
            var days = Fixture.CreateMany<IsoDayOfWeek>().ToList();
            var set = new IsoDayOfWeekSet(days);
            var day = days.Last();

            // Act
            var contains = set.Contains(day);

            // Assert
            contains.Should().BeTrue();
        }

        [Fact]
        public void Count_AnyDayInAllDays_True()
        {
            // Arrange
            var days = IsoDayOfWeekSet.WithAllDays;
            var set = new IsoDayOfWeekSet(days);
            var day = Fixture.Create<IsoDayOfWeek>();

            // Act
            var contains = set.Contains(day);

            // Assert
            contains.Should().BeTrue();
        }

        #endregion

        #region Remove

        [Theory]
        [InlineData(IsoDayOfWeek.None)]
        [InlineData((IsoDayOfWeek)8)]
        public void Remove_InvalidDay_False(IsoDayOfWeek day)
        {
            // Arrange
            var set = new IsoDayOfWeekSet();

            // Act
            var removed = set.Remove(day);

            // Assert
            removed.Should().BeFalse();
        }

        [Fact]
        public void Remove_Empty_False()
        {
            // Arrange
            var set = new IsoDayOfWeekSet();
            var day = Fixture.Create<IsoDayOfWeek>();

            // Act
            var removed = set.Remove(day);

            // Assert
            removed.Should().BeFalse();
            set.Should().BeEmpty();
        }

        [Fact]
        public void Remove_NonContainedDay_False()
        {
            // Arrange
            var day = Fixture.Create<IsoDayOfWeek>();
            var days = Fixture.CreateMany<IsoDayOfWeek>().Where(d => d != day).ToList();
            var set = new IsoDayOfWeekSet(days);
            set.Should().NotContain(day);

            // Act
            var removed = set.Remove(day);

            // Assert
            removed.Should().BeFalse();
            set.Should().NotContain(day);
        }

        [Fact]
        public void Remove_ContainedDay_True()
        {
            // Arrange
            var days = Fixture.CreateMany<IsoDayOfWeek>().ToList();
            var set = new IsoDayOfWeekSet(days);
            var day = days.Last();

            // Act
            var removed = set.Remove(day);

            // Assert
            removed.Should().BeTrue();
            set.Should().NotContain(day);
        }

        #endregion

        #region Count

        [Fact]
        public void Count_Empty_Zero()
        {
            // Arrange
            var set = new IsoDayOfWeekSet();

            // Act
            var count = set.Count;

            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public void Count_SingleDay_One()
        {
            // Arrange
            var day = Fixture.Create<IsoDayOfWeek>();
            var set = new IsoDayOfWeekSet { day };

            // Act
            var count = set.Count;

            // Assert
            count.Should().Be(1);
        }

        [Fact]
        public void Count_RepeatedDay_One()
        {
            // Arrange
            var day = Fixture.Create<IsoDayOfWeek>();
            var days = Enumerable.Repeat(day, 10);
            var set = new IsoDayOfWeekSet(days);

            // Act
            var count = set.Count;

            // Assert
            count.Should().Be(1);
        }

        [Fact]
        public void Count_RandomDays()
        {
            // Arrange
            var days = Fixture.CreateMany<IsoDayOfWeek>().ToList();
            var set = new IsoDayOfWeekSet(days);
            var expected = days.Distinct().Count();

            // Act
            var count = set.Count;

            // Assert
            count.Should().Be(expected);
        }

        [Fact]
        public void Count_AllDays_Seven()
        {
            // Arrange
            var days = IsoDayOfWeekSet.WithAllDays;
            var set = new IsoDayOfWeekSet(days);

            // Act
            var count = set.Count;

            // Assert
            count.Should().Be(7);
        }

        #endregion

        #region Is read-only

        [Fact]
        public void IsReadOnly_False()
        {
            // Arrange
            var set = new IsoDayOfWeekSet();

            // Act
            var isReadOnly = set.IsReadOnly;

            // Assert
            isReadOnly.Should().BeFalse();
        }

        #endregion
    }
}
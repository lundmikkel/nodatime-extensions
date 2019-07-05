using AutoFixture;
using FluentAssertions;
using NodaTime.AutoFixture;
using System.Linq;
using Xunit;

namespace NodaTime.Tests.AutoFixture
{
    public class GeneratorTests
    {
        private readonly Fixture Fixture = new Fixture().CustomizeForNodaTime();

        [Fact]
        public void CreateDateInterval()
        {
            // Arrange
            var generator = Fixture.Create<Generator<DateInterval>>();
            bool Predicate(DateInterval interval) => new LocalDate(1900, 1, 1) <= interval.Start && interval.End < new LocalDate(2100, 1, 1);

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateDateTimeZone()
        {
            // Arrange
            var generator = Fixture.Create<Generator<DateTimeZone>>();
            bool Predicate(DateTimeZone timeZone) => timeZone != null;

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateDuration()
        {
            // Arrange
            var generator = Fixture.Create<Generator<Duration>>();
            var twoDays = Duration.FromDays(2);
            bool Predicate(Duration duration) => Duration.Zero < duration && duration < twoDays;

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateInstant()
        {
            // Arrange
            var generator = Fixture.Create<Generator<Instant>>();
            bool Predicate(Instant instant) => Instant.FromUtc(1900, 01, 01, 00, 00) <= instant && instant < Instant.FromUtc(2100, 01, 01, 00, 00);

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateInterval()
        {
            // Arrange
            var generator = Fixture.Create<Generator<Interval>>();
            bool Predicate(Interval interval) => Instant.FromUtc(1900, 01, 01, 00, 00) <= interval.Start && interval.End < Instant.FromUtc(2100, 01, 01, 00, 00);

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateIsoDayOfWeek()
        {
            // Arrange
            var generator = Fixture.Create<Generator<IsoDayOfWeek>>();
            bool Predicate(IsoDayOfWeek dayOfWeek) => 1 <= (int)dayOfWeek && (int)dayOfWeek <= 7;

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateLocalDate()
        {
            // Arrange
            var generator = Fixture.Create<Generator<LocalDate>>();
            bool Predicate(LocalDate date) => new LocalDate(1900, 1, 1) <= date && date < new LocalDate(2100, 1, 1);

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateLocalDateTime()
        {
            // Arrange
            var generator = Fixture.Create<Generator<LocalDateTime>>();
            bool Predicate(LocalDateTime dateTime) => new LocalDate(1900, 1, 1) <= dateTime.Date && dateTime.Date < new LocalDate(2100, 1, 1) && dateTime.TimeOfDay.Minute % 10 == 0;

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateLocalTime()
        {
            // Arrange
            var generator = Fixture.Create<Generator<LocalTime>>();
            bool Predicate(LocalTime time) => time.Minute % 10 == 0;

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreateOffset()
        {
            // Arrange
            var generator = Fixture.Create<Generator<Offset>>();
            bool Predicate(Offset offset) => offset.Seconds / NodaConstants.SecondsPerMinute % 15 == 0;

            // Act
            var allSatisfyPredicate = generator.Take(100).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }

        [Fact]
        public void CreatePeriod()
        {
            // Arrange
            var generator = Fixture.Create<Generator<Period>>();
            bool Predicate(Period period)
            {
                period = period.Normalize();

                return
                    period.Years == 0 &&
                    period.Months == 0 &&
                    period.Weeks == 0 &&
                    period.Minutes % 10 == 0 &&
                    period.Days + period.Hours + period.Minutes > 0;
            }

            // Act
            var allSatisfyPredicate = generator.Take(250).All(Predicate);

            // Assert
            allSatisfyPredicate.Should().BeTrue();
        }
    }
}
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

namespace NodaTime.Tests
{
    public class DateTimeAdjustersTests
    {
        #region NextOrSame

        public static TheoryData<LocalDateTime, LocalTime, LocalDateTime> NextOrSameExamples = new TheoryData<LocalDateTime, LocalTime, LocalDateTime>
        {
            { new LocalDateTime(2018, 04, 01, 00, 00, 00),                             LocalTime.Midnight,        new LocalDateTime(2018, 04, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00) - Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 04, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00) + Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 04, 02, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 30, 00, 00, 00) + Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 05, 01, 00, 00, 00) },
            { new LocalDateTime(2017, 12, 31, 00, 00, 00) + Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 01, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00),                             new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 01, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17),                             new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 01, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17),                             LocalTime.Midnight,        new LocalDateTime(2018, 04, 02, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17) - Period.FromNanoseconds(1), new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 01, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17) + Period.FromNanoseconds(1), new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 02, 15, 13, 17) }
        };

        [Theory]
        [MemberData(nameof(NextOrSameExamples))]
        public void NextOrSame(LocalDateTime dateTime, LocalTime time, LocalDateTime expected)
        {
            // Arrange
            var adjuster = DateTimeAdjusters.NextOrSame(time);

            // Act
            var nextOrSame = adjuster(dateTime);

            // Assert
            nextOrSame.Should().Be(expected);
        }

        #endregion

        #region Next

        public static TheoryData<LocalDateTime, LocalTime, LocalDateTime> NextExamples = new TheoryData<LocalDateTime, LocalTime, LocalDateTime>
        {
            { new LocalDateTime(2018, 04, 01, 00, 00, 00),                             LocalTime.Midnight,        new LocalDateTime(2018, 04, 02, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00) - Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 04, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00) + Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 04, 02, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 30, 00, 00, 00) + Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 05, 01, 00, 00, 00) },
            { new LocalDateTime(2017, 12, 31, 00, 00, 00) + Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 01, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00),                             new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 01, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17),                             new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 02, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17),                             LocalTime.Midnight,        new LocalDateTime(2018, 04, 02, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17) - Period.FromNanoseconds(1), new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 01, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17) + Period.FromNanoseconds(1), new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 02, 15, 13, 17) }
        };

        [Theory]
        [MemberData(nameof(NextExamples))]
        public void Next(LocalDateTime dateTime, LocalTime time, LocalDateTime expected)
        {
            // Arrange
            var adjuster = DateTimeAdjusters.Next(time);

            // Act
            var next = adjuster(dateTime);

            // Assert
            next.Should().Be(expected);
        }

        #endregion

        #region PreviousOrSame

        public static TheoryData<LocalDateTime, LocalTime, LocalDateTime> PreviousOrSameExamples = new TheoryData<LocalDateTime, LocalTime, LocalDateTime>
        {
            { new LocalDateTime(2018, 04, 01, 00, 00, 00),                             LocalTime.Midnight,        new LocalDateTime(2018, 04, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00) - Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 03, 31, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00) + Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 04, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 05, 01, 00, 00, 00) - Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 04, 30, 00, 00, 00) },
            { new LocalDateTime(2018, 01, 01, 00, 00, 00) - Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2017, 12, 31, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00),                             new LocalTime(15, 13, 17), new LocalDateTime(2018, 03, 31, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17),                             new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 01, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17),                             LocalTime.Midnight,        new LocalDateTime(2018, 04, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17) - Period.FromNanoseconds(1), new LocalTime(15, 13, 17), new LocalDateTime(2018, 03, 31, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17) + Period.FromNanoseconds(1), new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 01, 15, 13, 17) }
        };

        [Theory]
        [MemberData(nameof(PreviousOrSameExamples))]
        public void PreviousOrSame(LocalDateTime dateTime, LocalTime time, LocalDateTime expected)
        {
            // Arrange
            var adjuster = DateTimeAdjusters.PreviousOrSame(time);

            // Act
            var previousOrSame = adjuster(dateTime);

            // Assert
            previousOrSame.Should().Be(expected);
        }

        #endregion

        #region Previous

        public static TheoryData<LocalDateTime, LocalTime, LocalDateTime> PreviousExamples = new TheoryData<LocalDateTime, LocalTime, LocalDateTime>
        {
            { new LocalDateTime(2018, 04, 01, 00, 00, 00),                             LocalTime.Midnight,        new LocalDateTime(2018, 03, 31, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00) - Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 03, 31, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00) + Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 04, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 05, 01, 00, 00, 00) - Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2018, 04, 30, 00, 00, 00) },
            { new LocalDateTime(2018, 01, 01, 00, 00, 00) - Period.FromNanoseconds(1), LocalTime.Midnight,        new LocalDateTime(2017, 12, 31, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 00, 00, 00),                             new LocalTime(15, 13, 17), new LocalDateTime(2018, 03, 31, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17),                             new LocalTime(15, 13, 17), new LocalDateTime(2018, 03, 31, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17),                             LocalTime.Midnight,        new LocalDateTime(2018, 04, 01, 00, 00, 00) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17) - Period.FromNanoseconds(1), new LocalTime(15, 13, 17), new LocalDateTime(2018, 03, 31, 15, 13, 17) },
            { new LocalDateTime(2018, 04, 01, 15, 13, 17) + Period.FromNanoseconds(1), new LocalTime(15, 13, 17), new LocalDateTime(2018, 04, 01, 15, 13, 17) }
        };

        [Theory]
        [MemberData(nameof(PreviousExamples))]
        public void Previous(LocalDateTime dateTime, LocalTime time, LocalDateTime expected)
        {
            // Arrange
            var adjuster = DateTimeAdjusters.Previous(time);

            // Act
            var previous = adjuster(dateTime);

            // Assert
            previous.Should().Be(expected);
        }

        #endregion
    }
}

using FluentAssertions;
using NodaTime;
using Xunit;

namespace NodaTime.Tests
{
    public class PatternTests
    {
        #region LocalTimeMinimalExtendedPattern

        public static TheoryData<LocalTime, string> LocalTimeExamples = new TheoryData<LocalTime, string>
        {
            { new LocalTime(15, 00, 00, 000), "15:00"},
            { new LocalTime(06, 34, 00, 000), "06:34"},
            { new LocalTime(14, 13, 34, 000), "14:13:34"},
            { new LocalTime(16, 01, 29, 120), "16:01:29.12"},
            { new LocalTime(16, 01, 29, 123) + Period.FromTicks(4567), "16:01:29.1234567" },
            { new LocalTime(01, 00) + Period.FromTicks(1), "01:00:00.0000001" },
            { LocalTime.Midnight, "00:00" }
        };

        [Theory]
        [MemberData(nameof(LocalTimeExamples))]
        public void LocalTimeMinimalExtendedPattern_Format(LocalTime time, string expected)
        {
            // Act
            var formattedTime = Patterns.LocalTimePattern.Format(time);

            // Assert
            formattedTime.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(LocalTimeExamples))]
        public void LocalTimeMinimalExtendedPattern_Parse(LocalTime expected, string value)
        {
            // Act
            var time = Patterns.LocalTimePattern.Parse(value);

            // Assert
            time.Success.Should().BeTrue();
            time.Value.Should().Be(expected);
        }

        [Fact]
        public void LocalTimeMinimalExtendedPattern_Parse_TwoDigitTime()
        {
            // Arrange
            var value = "12";

            // Act
            var time = Patterns.LocalTimePattern.Parse(value);

            // Assert
            time.Success.Should().BeTrue();
            time.Value.Should().Be(new LocalTime(12, 00));
        }

        #endregion

        #region DurationMinimalExtendedPattern

        public static TheoryData<Duration, string> DurationExamples = new TheoryData<Duration, string>
        {
            { CreateDuration(+3, 00, 00, 00, 000, 0000), "3:00:00"},
            { CreateDuration(+2, 15, 00, 00, 000, 0000), "2:15:00"},
            { CreateDuration(+0, 06, 34, 00, 000, 0000), "0:06:34"},
            { CreateDuration(+6, 14, 13, 34, 000, 0000), "6:14:13:34"},
            { CreateDuration(+0, 16, 01, 29, 120, 0000), "0:16:01:29.12"},
            { CreateDuration(+4, 16, 01, 29, 123, 4567), "4:16:01:29.1234567" },
            { CreateDuration(+1, 00, 00, 00, 000, 0001), "1:00:00:00.0000001" },
            { CreateDuration(+0, 12, 34, 56, 789, 0123), "0:12:34:56.7890123" },
            { CreateDuration(-4, -16, -01, -29, -123, -4567), "-4:16:01:29.1234567" },
            { CreateDuration(-1, 00, 00, 00, 000, 0001), "-0:23:59:59.9999999" }
        };

        private static Duration CreateDuration(double days, double hours, double minutes, double seconds, double milliseconds = 0d, double ticks = 0d)
        {
            return Duration.FromDays(days)
                 + Duration.FromHours(hours)
                 + Duration.FromMinutes(minutes)
                 + Duration.FromSeconds(seconds)
                 + Duration.FromMilliseconds(milliseconds)
                 + Duration.FromTicks(ticks);
        }

        [Theory]
        [MemberData(nameof(DurationExamples))]
        public void DurationMinimalExtendedPattern_Format(Duration time, string expected)
        {
            // Act
            var formattedTime = Patterns.DurationPattern.Format(time);

            // Assert
            formattedTime.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(DurationExamples))]
        public void DurationMinimalExtendedPattern_Parse(Duration expected, string value)
        {
            // Act
            var time = Patterns.DurationPattern.Parse(value);

            // Assert
            time.Success.Should().BeTrue();
            time.Value.Should().Be(expected);
        }
        
        #endregion
    }
}

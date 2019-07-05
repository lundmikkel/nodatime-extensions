using System;
using NodaTime.Tzdb;
using Xunit;

namespace NodaTime.Tests
{
    public class UtcDateOffsetTests
    {
        private static readonly IDateTimeZoneProvider TimeZoneProvider = DateTimeZoneProviderFactory.Create();

        private static readonly string[] TimeZoneIds = {
            "Etc/GMT-14",
            "Etc/GMT-13",
            "Etc/GMT-12",
            "Etc/GMT-11",
            "Etc/GMT-10",
            "Etc/GMT-9",
            "Etc/GMT-8",
            "Etc/GMT-7",
            "Etc/GMT-6",
            "Etc/GMT-5",
            "Etc/GMT-4",
            "Etc/GMT-3",
            "Etc/GMT-2",
            "Etc/GMT-1",
            "Etc/GMT",
            "Etc/GMT+1",
            "Etc/GMT+2",
            "Etc/GMT+3",
            "Etc/GMT+4",
            "Etc/GMT+5",
            "Etc/GMT+6",
            "Etc/GMT+7",
            "Etc/GMT+8",
            "Etc/GMT+9",
            "Etc/GMT+10",
            "Etc/GMT+11",
            "Etc/GMT+12"
        };

        [Fact]
        public void Test()
        {
            // Arrange
            var date = new LocalDate( 2017, 01, 01 );
            var period = Period.FromHours( 10 );

            foreach ( var timeZoneId in TimeZoneIds )
            {
                var timeZone = TimeZoneProvider[ timeZoneId ];
                var dateTime = timeZone.AtStartOfDay( date ).ToDateTimeUtc();
                Console.WriteLine( $"{timeZoneId}: {dateTime} ({dateTime + TimeSpan.FromHours( 12 )})" );
            }
        }
    }
}
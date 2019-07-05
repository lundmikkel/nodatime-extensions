using System;

namespace NodaTime.Extensions
{
    public class InvalidLocalDateTimeInterval : Exception
    {
        public InvalidLocalDateTimeInterval( Interval interval, DateTimeZone timeZone, LocalDateTime startDateTime, LocalDateTime endDateTime )
            : base( $"The interval {interval} maps to an invalid date time interval in the time zone {timeZone}, because the local end time {endDateTime} is before the local start time {startDateTime}." )
        { }
    }
}
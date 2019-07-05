using System;

namespace NodaTime.Extensions
{
    public static class DateTimeZoneExtensions
    {
        /// <summary>
        /// Maps two <see cref="LocalDateTime"/>s to the corresponding <see cref="Interval"/> in the given time zone.
        /// </summary>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="startLocalDateTime">The date/time mapped to the start of the interval.</param>
        /// <param name="endLocalDateTime">The date/time mapped to the end of the interval.</param>
        /// <returns></returns>
        public static Interval AtInterval( this DateTimeZone timeZone, LocalDateTime startLocalDateTime, LocalDateTime endLocalDateTime ) => new Interval(
            timeZone.AtLeniently( startLocalDateTime ).ToInstant(),
            timeZone.AtLeniently( endLocalDateTime ).ToInstant()
        );

        /// <summary>
        /// Maps the given <see cref="DateTimeInterval"/> to the corresponding <see cref="Interval"/> in the given time zone in a lenient manner:
        /// ambiguous values map to the earlier of the alternatives, and "skipped" values are shifted forward by the duration of the "gap".
        /// </summary>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="dateTimeInterval">The date/time interval.</param>
        /// <returns>The <see cref="Interval"/> corresponding to the given date/time interval in the given time zone.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="timeZone"/> or <paramref name="dateTimeInterval"/> is <c>null</c>.</exception>
        public static Interval AtLeniently( this DateTimeZone timeZone, DateTimeInterval dateTimeInterval )
        {
            if (timeZone == null)
            {
                throw new ArgumentNullException(nameof(timeZone));
            }

            if (dateTimeInterval == null)
            {
                throw new ArgumentNullException(nameof(dateTimeInterval));
            }

            return new Interval(
                timeZone.AtLeniently(dateTimeInterval.Start).ToInstant(),
                timeZone.AtLeniently(dateTimeInterval.End).ToInstant()
            );
        }

        /// <summary>
        /// Gets the current date in the given time zone according to the given clock.
        /// </summary>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="clock">The clock from which the current time is taken.</param>
        /// <returns>Today's date in the given time zone.</returns>
        public static LocalDate Today( this DateTimeZone timeZone, IClock clock ) => ( clock ?? SystemClock.Instance ).GetCurrentInstant().InZone( timeZone ).Date;

        /// <summary>
        /// Returns the earliest valid <see cref="ZonedDateTime"/> after the given local date.
        /// </summary>
        /// <remarks>
        /// If midnight exists unambiguously on the day after the given date, its midnight is returned.
        /// If the date has an ambiguous start time (e.g. the clocks go back from 1am to midnight) then
        /// the earlier <see cref="ZonedDateTime"/> is returned. If it has no midnight
        /// (e.g. the clocks go forward from midnight to 1am) then the earliest valid value is returned;
        /// this will be the instant of the transition.
        /// </remarks>
        /// <param name="timeZone">A time zone.</param>
        /// <param name="date">The local date to map in the given time zone.</param>
        /// <exception cref="SkippedTimeException">The entire day was skipped due to a very large time zone transition.
        /// (This is extremely rare.)</exception>
        /// <returns>The <see cref="ZonedDateTime"/> representing the earliest time in the given date, in this time zone.</returns>
        public static ZonedDateTime AtEndOfDay( this DateTimeZone timeZone, LocalDate date ) => timeZone.AtStartOfDay( date.NextDay() );

        /// <summary>
        /// Serializes the <see cref="DateTimeZone"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="dateTimeZone">A time zone.</param>
        /// <returns>A machine-readable string representation of <paramref name="dateTimeZone"/>.</returns>
        public static string ToStandardString( this DateTimeZone dateTimeZone ) => dateTimeZone.ToString();
    }
}
using System.Globalization;

namespace NodaTime.Extensions
{
    public static class LocalDateExtensions
    {
        /// <summary>
        /// Returns the next date with the specified day-of-week, or the original date, if the day is already correct.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <param name="dayOfWeek">The day-of-week to find.</param>
        /// <returns>The next date with the specified day-of-week, or the original date, if the day is already correct.</returns>
        public static LocalDate NextOrSame( this LocalDate date, IsoDayOfWeek dayOfWeek ) => DateAdjusters.NextOrSame(dayOfWeek)(date);

        /// <summary>
        /// Returns the previous date with the specified day-of-week, or the original date, if the day is already correct.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <param name="dayOfWeek">The day-of-week to find.</param>
        /// <returns>The previous date with the specified day-of-week, or the original date, if the day is already correct.</returns>
        public static LocalDate PreviousOrSame(this LocalDate date, IsoDayOfWeek dayOfWeek) => DateAdjusters.PreviousOrSame(dayOfWeek)(date);

        /// <summary>
        /// Returns the first date in the week that <paramref name="date"/> is in, given the week starts on <paramref name="firstDayOfWeek"/>.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <param name="firstDayOfWeek">The week day considered the first day of the week.</param>
        /// <returns>The first date of the week</returns>
        /// <remarks>Returns <paramref name="date"/> if it is the first day of the week; otherwise a date at most six days before <paramref name="date"/>.</remarks>
        public static LocalDate StartOfWeek( this LocalDate date, IsoDayOfWeek firstDayOfWeek ) => date.PreviousOrSame( firstDayOfWeek );

        /// <summary>
        /// Returns the last date in the week that <paramref name="date"/> is in, given the week starts on <paramref name="firstDayOfWeek"/>.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <param name="firstDayOfWeek">The week day considered the first day of the week.</param>
        /// <returns>The last date of the week</returns>
        /// <remarks>Returns <paramref name="date"/> if it is the last day of the week; otherwise a date at most six days after <paramref name="date"/>.</remarks>
        public static LocalDate EndOfWeek( this LocalDate date, IsoDayOfWeek firstDayOfWeek ) => date.NextOrSame( firstDayOfWeek.PreviousDay() );

        /// <summary>
        /// Returns a date interval starting and ending on the first and last day of the week that <paramref name="date"/> is in,
        /// given the week starts on <paramref name="firstDayOfWeek"/>.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <param name="firstDayOfWeek">The week day considered the first day of the week.</param>
        /// <returns>The week date interval.</returns>
        public static DateInterval GetContainingWeekInterval( this LocalDate date, IsoDayOfWeek firstDayOfWeek ) => new DateInterval( date.StartOfWeek( firstDayOfWeek ), date.EndOfWeek( firstDayOfWeek ) );

        /// <summary>
        /// Returns the first date of the given date's month.
        /// </summary>
        /// <param name="date">The date whose month should be used.</param>
        /// <returns>The first date of the given date's month.</returns>
        public static LocalDate StartOfMonth(this LocalDate date) => DateAdjusters.StartOfMonth(date);

        /// <summary>
        /// Returns the last date of the given date's month.
        /// </summary>
        /// <param name="date">The date whose month should be used.</param>
        /// <returns>The last date of the given date's month.</returns>
        public static LocalDate EndOfMonth( this LocalDate date ) => DateAdjusters.EndOfMonth(date);

        /// <summary>
        /// Returns a date interval with the first and last date of the month that <paramref name="date"/> is in.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <returns>The week date interval.</returns>
        public static DateInterval GetContainingMonthInterval( this LocalDate date ) => new DateInterval( date.StartOfMonth(), date.EndOfMonth() );

        /// <summary>
        /// Finds the next time the date (month and day) occurs, starting from <paramref name="fromDate"/>.
        /// </summary>
        // TODO: Add documentation
        public static LocalDate NextOccurrenceOfDate( this LocalDate date, LocalDate fromDate ) => date.ToAnnualDate().NextOccurrence( fromDate );

        // TODO: Test
        /// <summary>
        /// Returns a <see cref="DateTimeInterval"/> that starts on <paramref name="date"/> at <paramref name="timeInterval"/>'s start and ends the next time it is <paramref name="timeInterval"/>'s end.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <param name="timeInterval">The time interval.</param>
        /// <returns>The date time interval.</returns>
        public static DateTimeInterval At(this LocalDate date, TimeInterval timeInterval) => date.At(timeInterval.Start, timeInterval.End);

        /// <summary>
        /// Returns a <see cref="DateTimeInterval"/> that starts on <paramref name="date"/> at <paramref name="startTime"/> and ends the next time it is <paramref name="endTime"/>.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time. If <paramref name="endTime"/> is less than or equal to <paramref name="startTime"/>, <paramref name="endTime"/> is considered to be on the following day.</param>
        /// <returns>The date time interval.</returns>
        public static DateTimeInterval At( this LocalDate date, LocalTime startTime, LocalTime endTime )
        {
            var endDate = startTime < endTime ? date : date.NextDay();
            return new DateTimeInterval( date.At( startTime ), endDate.At( endTime ) );
        }

        /// <summary>
        /// Returns a <see cref="LocalDateTime"/> pair representing the start and end date time of an interval
        /// that starts on <paramref name="date"/> at <paramref name="startTime"/> and ends the next time it is <paramref name="endTime"/>.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time. If <paramref name="endTime"/> is less than or equal to <paramref name="startTime"/>, <paramref name="endTime"/> is considered to be on the following day.</param>
        /// <returns>The start and end date time of the interval.</returns>
        public static NullableDateTimeInterval At( this LocalDate date, LocalTime? startTime, LocalTime? endTime )
        {
            var startDateTime = startTime.HasValue ? date.At( startTime.Value ) : default( LocalDateTime? );

            var endDate = endTime <= startTime || endTime.HasValue && endTime.Value == LocalTime.Midnight ? date.NextDay() : date;
            var endDateTime = endTime.HasValue ? endDate.At( endTime.Value ) : default( LocalDateTime? );

            return new NullableDateTimeInterval
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime
            };
        }

        public struct NullableDateTimeInterval
        {
            public LocalDateTime? StartDateTime { get; set; }
            public LocalDateTime? EndDateTime { get; set; }
        }

        /// <summary>
        /// Serializes the <see cref="LocalDate"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="localDate">A local date.</param>
        /// <returns>A machine-readable string representation of <paramref name="localDate"/>.</returns>
        public static string ToStandardString( this LocalDate localDate ) => Patterns.LocalDatePattern.Format(localDate);

        /// <summary>
        /// Serializes the <see cref="LocalDate"/> for SQL Server queries.
        /// </summary>
        /// <param name="localDate">A local date.</param>
        /// <returns>A SQL formatted string representation of <paramref name="localDate"/>.</returns>
        /// <seealso cref="ToQuotedSqlString"/>
        public static string ToSqlString( this LocalDate localDate ) => localDate.ToString( @"yyyy'-'MM'-'dd", CultureInfo.InvariantCulture );

        /// <summary>
        /// Serializes the <see cref="LocalDate"/> for SQL Server queries and wraps it in single quotes.
        /// </summary>
        /// <param name="localDate">A local date.</param>
        /// <returns>A SQL formatted string representation of <paramref name="localDate"/> wrapped in single quotes.</returns>
        /// <seealso cref="ToSqlString"/>
        public static string ToQuotedSqlString( this LocalDate localDate ) => $"\'{localDate.ToSqlString()}\'";

        #region Ported

        /// <summary>
        /// Combines the <see cref="LocalDate"/> with the time at the given hour, minute, second and millisecond.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <param name="second">The second of the minute.</param>
        /// <param name="millisecond">The millisecond of the second.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The parameters do not form a valid time.</exception>
        /// <returns>The resulting date time.</returns>
        public static LocalDateTime At(this LocalDate date, int hour, int minute, int second = 0, int millisecond = 0) => date.At(new LocalTime(hour, minute, second, millisecond));

        /// <summary>
        /// Converts the date to an annual date, ignoring the year.
        /// </summary>
        /// <param name="date">A local date.</param>
        /// <returns>An annual date with the same month and day as the given date.</returns>
        [System.Obsolete("Ported")]
        public static AnnualDate ToAnnualDate(this LocalDate date) => new AnnualDate(date.Month, date.Day);

        /// <summary>
        /// Determines whether the local date is on the annual date, i.e. whether the months and days are equal.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the annual date represents February 29th, and the specified date represents February 28th in a non-leap
        /// year, the returned value will be <c>true</c>; if it is a leap year, only a date representing February 29th
        /// will return <c>true</c>.
        /// </para>
        /// </remarks>
        /// <param name="date">The local date.</param>
        /// <param name="annualDate">The annual date.</param>
        /// <returns><c>true</c>, if the date is on the annual date; <c>false</c>, otherwise.</returns>
        [System.Obsolete("Ported")]
        public static bool IsOnAnnualDate(this LocalDate date, AnnualDate annualDate) => annualDate.InYear(date.Year) == date;

        /// <summary>
        /// Returns a single-day interval starting and ending on the given date.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <returns>A single-day interval starting and ending on the given date.</returns>
        [System.Obsolete("Ported")]
        public static DateInterval ToSingleDayInterval(this LocalDate date) => new DateInterval(date, date);

        /// <summary>
        /// Returns the date following the given date, i.e. the given date's tomorrow.
        /// </summary>
        /// <param name="date">A local date.</param>
        /// <returns>The date following the given date.</returns>
        [System.Obsolete("Ported")]
        public static LocalDate NextDay(this LocalDate date) => date.PlusDays(1);

        /// <summary>
        /// Returns the date preceding the given date, i.e. the given date's yesterday.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>The date preceding the given date.</returns>
        [System.Obsolete("Ported")]
        public static LocalDate PreviousDay(this LocalDate date) => date.PlusDays(-1);

        /// <summary>
        /// Returns an <see cref="Interval"/> representing the local date in the given time zone.
        /// </summary>
        /// <param name="date">A local date.</param>
        /// <param name="timeZone">A time zone.</param>
        /// <returns></returns>
        public static Interval ToIntervalInZone(this LocalDate date, DateTimeZone timeZone)
        {
            var start = timeZone.AtStartOfDay(date);
            var end = timeZone.AtEndOfDay(date);
            return new Interval(start.ToInstant(), end.ToInstant());
        }

        #endregion
    }
}
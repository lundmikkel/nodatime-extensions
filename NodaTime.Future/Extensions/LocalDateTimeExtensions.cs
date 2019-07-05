using NodaTime.Text;

namespace NodaTime.Extensions
{
    public static class LocalDateTimeExtensions
    {
        private static readonly IPattern<LocalDateTime> LocalDateTimeSqlPattern = new CompositePatternBuilder<LocalDateTime>
        {
            { LocalDateTimePattern.CreateWithInvariantCulture(@"yyyy'-'MM'-'dd HH':'mm':'ss.FFFFFFF"), _ => true },
            { LocalDateTimePattern.CreateWithInvariantCulture(@"yyyy'-'MM'-'dd"), dateTime => dateTime.TimeOfDay == LocalTime.Midnight }
        }
        .Build();

        /// <summary>
        /// Determines whether a <see cref="LocalDateTime"/> is ambiguous in a given time zone.
        /// </summary>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="time">The possibly ambiguous date time.</param>
        /// <returns><c>true</c> if <paramref name="time"/> is ambiguous in <paramref name="timeZone"/>; otherwise, <c>false</c>.</returns>
        public static bool IsAmbiguousIn( this LocalDateTime time, DateTimeZone timeZone ) => timeZone.MapLocal( time ).Count > 1;

        /// <summary>
        /// Determines whether a <see cref="LocalDateTime"/> is skipped in a given time zone.
        /// </summary>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="time">The possibly skipped date time.</param>
        /// <returns><c>true</c> if <paramref name="time"/> is skipped in <paramref name="timeZone"/>; otherwise, <c>false</c>.</returns>
        public static bool IsSkippedIn( this LocalDateTime time, DateTimeZone timeZone ) => timeZone.MapLocal( time ).Count < 1;

        /// <summary>
        /// Serializes the <see cref="LocalDateTime"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="localDateTime">A local date time.</param>
        /// <returns>A machine-readable string representation of <paramref name="localDateTime"/>.</returns>
        public static string ToStandardString( this LocalDateTime localDateTime ) => Patterns.LocalDateTimePattern.Format(localDateTime);

        /// <summary>
        /// Serializes the <see cref="LocalDateTime"/> for SQL Server queries.
        /// </summary>
        /// <param name="localDateTime">A local date time.</param>
        /// <returns>A SQL formatted string representation of <paramref name="localDateTime"/>.</returns>
        /// <seealso cref="ToQuotedSqlString"/>
        public static string ToSqlString( this LocalDateTime localDateTime ) => LocalDateTimeSqlPattern.Format( localDateTime );

        /// <summary>
        /// Serializes the <see cref="LocalDateTime"/> for SQL Server queries and wraps it in single quotes.
        /// </summary>
        /// <param name="localDateTime">A local date time.</param>
        /// <returns>A SQL formatted string representation of <paramref name="localDateTime"/> wrapped in single quotes.</returns>
        /// <seealso cref="ToSqlString"/>
        public static string ToQuotedSqlString( this LocalDateTime localDateTime ) => $"\'{localDateTime.ToSqlString()}\'";

        [System.Obsolete("Use the " + nameof(DateTimeInterval) + " constructor that only takes a single " + nameof(LocalDateTime) + " instead. This method will be removed in the next major version.")]
        public static DateTimeInterval ToEmptyInterval( this LocalDateTime localDateTime ) => new DateTimeInterval(localDateTime);

        /// <summary>
        /// Returns a single-point date time interval that starts and ends at the given <see cref="LocalDateTime"/>.
        /// </summary>
        /// <param name="localDateTime">A local date time.</param>
        /// <returns>A single-point date time interval that starts and ends at the given <see cref="LocalDateTime"/>.</returns>
        public static DateTimeInterval ToSinglePointInterval(this LocalDateTime localDateTime) => new DateTimeInterval(localDateTime);

        /// <summary>
        /// Returns the next date time with the specified time, or the original date time, if the time is already correct.
        /// </summary>
        /// <param name="dateTime">The local date time.</param>
        /// <param name="time">The time to adjust date times to.</param>
        /// <returns>The next date time with the specified time, or the original date time, if the time is already correct.</returns>
        public static LocalDateTime NextOrSame(this LocalDateTime dateTime, LocalTime time) => DateTimeAdjusters.NextOrSame(time)(dateTime);

        /// <summary>
        /// Returns the next date time with the specified time, adding a day if the time is already correct.
        /// </summary>
        /// <param name="dateTime">The local date time.</param>
        /// <param name="time">The time to adjust date times to.</param>
        /// <returns>The next date time with the specified time, adding a day if the time is already correct.</returns>
        public static LocalDateTime Next(this LocalDateTime dateTime, LocalTime time) => DateTimeAdjusters.Next(time)(dateTime);

        /// <summary>
        /// Returns the previous date time with the specified time, or the original date time, if the time is already correct.
        /// </summary>
        /// <param name="dateTime">The local date time.</param>
        /// <param name="time">The time to adjust date times to.</param>
        /// <returns>The previous date time with the specified time, or the original date time, if the time is already correct.</returns>
        public static LocalDateTime PreviousOrSame(this LocalDateTime dateTime, LocalTime time) => DateTimeAdjusters.PreviousOrSame(time)(dateTime);

        /// <summary>
        /// Returns the previous date time with the specified time, subtracting a day if the time is already correct.
        /// </summary>
        /// <param name="dateTime">The local date time.</param>
        /// <param name="time">The time to adjust date times to.</param>
        /// <returns>The previous date time with the specified time, subtracting a day if the time is already correct.</returns>
        public static LocalDateTime Previous(this LocalDateTime dateTime, LocalTime time) => DateTimeAdjusters.Previous(time)(dateTime);
    }
}
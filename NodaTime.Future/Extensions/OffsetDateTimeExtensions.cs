using System.Globalization;

namespace NodaTime.Extensions
{
    public static class OffsetDateTimeExtensions
    {
        /// <summary>
        /// Converts a <see cref="OffsetDateTime"/> to a UNIX Timestamp (seconds since 1970-01-01T00:00:00Z, ignoring leap seconds).
        /// </summary>
        /// <param name="offsetDateTime">The offset date time.</param>
        /// <returns>The number of seconds since midnight on January 1st, 1970 UTC.</returns>
        public static long ToUnixTimeSeconds(this OffsetDateTime offsetDateTime) => offsetDateTime.ToInstant().ToUnixTimeSeconds();

        /// <summary>
        /// Serializes the <see cref="OffsetDateTime"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="offsetDateTime">An offset date time.</param>
        /// <returns>A machine-readable string representation of <paramref name="offsetDateTime"/>.</returns>
        public static string ToStandardString(this OffsetDateTime offsetDateTime) => Patterns.OffsetDateTimePattern.Format(offsetDateTime);

        /// <summary>
        /// Serializes the <see cref="OffsetDateTime"/> for SQL Server queries.
        /// </summary>
        /// <param name="offsetDateTime">An offset date time.</param>
        /// <returns>A SQL formatted string representation of <paramref name="offsetDateTime"/>.</returns>
        /// <seealso cref="ToQuotedSqlString"/>
        public static string ToSqlString(this OffsetDateTime offsetDateTime) => offsetDateTime.ToString(@"yyyy'-'MM'-'dd HH':'mm':'ss.FFFFFFFo<m>", CultureInfo.InvariantCulture);

        /// <summary>
        /// Serializes the <see cref="OffsetDateTime"/> for SQL Server queries and wraps it in single quotes.
        /// </summary>
        /// <param name="offsetDateTime">An offset date time.</param>
        /// <returns>A SQL formatted string representation of <paramref name="offsetDateTime"/> wrapped in single quotes.</returns>
        /// <seealso cref="ToSqlString"/>
        public static string ToQuotedSqlString(this OffsetDateTime offsetDateTime) => offsetDateTime.ToString(@"yyyy'-'MM'-'dd HH':'mm':'ss.FFFFFFFo<m>", CultureInfo.InvariantCulture);
    }
}
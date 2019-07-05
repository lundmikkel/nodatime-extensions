namespace NodaTime.Extensions
{
    public static class ZonedDateTimeExtensions
    {
        /// <summary>
        /// Converts a <see cref="ZonedDateTime"/> to a UNIX Timestamp (seconds since 1970-01-01T00:00:00Z, ignoring leap seconds).
        /// </summary>
        /// <param name="zonedDateTime">The zoned date time.</param>
        /// <returns>The number of seconds since midnight on January 1st, 1970 UTC.</returns>
        public static long ToUnixTimeSeconds( this ZonedDateTime zonedDateTime ) => zonedDateTime.ToInstant().ToUnixTimeSeconds();

        /// <summary>
        /// Serializes the <see cref="ZonedDateTime"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="zonedDateTime">A zoned datetime.</param>
        /// <returns>A machine-readable string representation of <paramref name="zonedDateTime"/>.</returns>
        public static string ToStandardString( this ZonedDateTime zonedDateTime ) => Patterns.ZonedDateTimePattern.Format(zonedDateTime);

        // TODO: Include this or force the user to convert to Instant/OffsetDateTime? Can SQL Server handle both formats without problems? Are the formats interchangable?
        // /// <summary>
        // /// Serializes the <see cref="ZonedDateTime"/> for SQL Server queries.
        // /// </summary>
        // /// <param name="zonedDateTime">A zoned datetime.</param>
        // /// <returns>A string representation the instant of <paramref name="zonedDateTime"/> that can be used directly in an SQL query.</returns>
        // public static string ToSqlString( this ZonedDateTime zonedDateTime ) => zonedDateTime.ToInstant().ToSqlString();
    }
}
using System.Globalization;

namespace NodaTime.Extensions
{
    public static class LocalTimeExtensions
    {
        /// <summary>
        /// Serializes the <see cref="LocalTime"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="localTime">A local time.</param>
        /// <returns>A machine-readable string representation of <paramref name="localTime"/>.</returns>
        public static string ToStandardString( this LocalTime localTime ) => Patterns.LocalTimePattern.Format(localTime);

        /// <summary>
        /// Serializes the <see cref="LocalTime"/> for SQL Server queries.
        /// </summary>
        /// <param name="localTime">A local time.</param>
        /// <returns>A SQL formatted string representation of <paramref name="localTime"/>.</returns>
        /// <seealso cref="ToQuotedSqlString"/>
        public static string ToSqlString( this LocalTime localTime ) => localTime.ToString( @"HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture );

        /// <summary>
        /// Serializes the <see cref="LocalTime"/> for SQL Server queries and wraps it in single quotes.
        /// </summary>
        /// <param name="localTime">A local time.</param>
        /// <returns>A SQL formatted string representation of <paramref name="localTime"/> wrapped in single quotes.</returns>
        /// <seealso cref="ToSqlString"/>
        public static string ToQuotedSqlString( this LocalTime localTime ) => $"\'{localTime.ToSqlString()}\'";
    }
}
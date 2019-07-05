using System;
using NodaTime.Text;

namespace NodaTime.Extensions
{
    public static class InstantExtensions
    {
        private static readonly InstantPattern PatternWithoutTime = InstantPattern.CreateWithInvariantCulture(@"yyyy'-'MM'-'dd");
        private static readonly InstantPattern PatternWithNoDecimalPlaces = InstantPattern.CreateWithInvariantCulture(@"yyyy'-'MM'-'dd HH':'mm':'ss");
        private static readonly InstantPattern PatternWithSevenDecimalPlaces = InstantPattern.CreateWithInvariantCulture(@"yyyy'-'MM'-'dd HH':'mm':'ss.FFFFFFF");
        
        /// <summary>
        /// Serializes the <see cref="Instant"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="instant">An instant.</param>
        /// <returns>A machine-readable string representation of <paramref name="instant"/>.</returns>
        public static string ToStandardString( this Instant instant ) => Patterns.InstantPattern.Format(instant);

        /// <summary>
        /// Serializes the <see cref="Instant"/> for SQL Server queries.
        /// </summary>
        /// <param name="instant">An instant.</param>
        /// <param name="decimalPlaces">Number of digits after the decimal. Must be between 0 and 7 (both included).</param>
        /// <returns>A SQL formatted string representation of <paramref name="instant"/>.</returns>
        /// <seealso cref="ToQuotedSqlString(Instant)"/>
        public static string ToSqlString( this Instant instant, int decimalPlaces )
        {
            if (decimalPlaces < 0 || 7 < decimalPlaces)
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Value must be between 0 and 7 (both included).");
            }

            if (instant.InUtc().TimeOfDay == LocalTime.Midnight)
            {
                return PatternWithoutTime.Format(instant);
            }

            if (decimalPlaces == 7)
            {
                return PatternWithSevenDecimalPlaces.Format(instant);
            }

            if (decimalPlaces == 0)
            {
                return PatternWithNoDecimalPlaces.Format(instant);
            }

            return InstantPattern.CreateWithInvariantCulture(@"yyyy'-'MM'-'dd HH':'mm':'ss." + new string('F', decimalPlaces)).Format(instant);
        }

        /// <summary>
        /// Serializes the <see cref="Instant"/> for SQL Server queries and wraps it in single quotes.
        /// </summary>
        /// <param name="instant">An instant.</param>
        /// <param name="decimalPlaces">Number of digits after the decimal. Must be between 0 and 7 (both included).</param>
        /// <returns>A SQL formatted string representation of <paramref name="instant"/> wrapped in single quotes.</returns>
        /// <seealso cref="ToSqlString(Instant)"/>
        public static string ToQuotedSqlString( this Instant instant, int decimalPlaces) => $"\'{instant.ToSqlString( decimalPlaces )}\'";

        /// <summary>
        /// Serializes the <see cref="Instant"/> for SQL Server queries.
        /// </summary>
        /// <param name="instant">An instant.</param>
        /// <returns>A SQL formatted string representation of <paramref name="instant"/>.</returns>
        /// <seealso cref="ToQuotedSqlString(Instant)"/>
        public static string ToSqlString(this Instant instant) => instant.ToSqlString(7);

        /// <summary>
        /// Serializes the <see cref="Instant"/> for SQL Server queries and wraps it in single quotes.
        /// </summary>
        /// <param name="instant">An instant.</param>
        /// <returns>A SQL formatted string representation of <paramref name="instant"/> wrapped in single quotes.</returns>
        /// <seealso cref="ToSqlString(Instant)"/>
        public static string ToQuotedSqlString( this Instant instant ) => $"\'{instant.ToSqlString()}\'";
    }
}
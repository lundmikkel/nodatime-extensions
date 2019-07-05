using System.Collections.Generic;

namespace NodaTime.Extensions
{
    public static class IsoDayOfWeekExtensions
    {
        /// <summary>
        /// Returns the next day of the week.
        /// </summary>
        /// <param name="dayOfWeek">The day of the week.</param>
        /// <returns>Returns the day following the given day of week.</returns>
        public static IsoDayOfWeek NextDay( this IsoDayOfWeek dayOfWeek ) => (IsoDayOfWeek)( (int)dayOfWeek % 7 + 1 );

        /// <summary>
        /// Returns the previous day of the week.
        /// </summary>
        /// <param name="dayOfWeek">The day of the week.</param>
        /// <returns>Returns the day before the given day of week.</returns>
        public static IsoDayOfWeek PreviousDay( this IsoDayOfWeek dayOfWeek ) => (IsoDayOfWeek)( (int)( dayOfWeek + 5 ) % 7 + 1 );

        /// <summary>
        /// Returns an <see cref="IsoDayOfWeekSet"/> containing the days in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">An enumerable of days of week.</param>
        /// <returns>An <see cref="IsoDayOfWeekSet"/> containing the days in <paramref name="source"/>.</returns>
        public static IsoDayOfWeekSet ToSet(this IEnumerable<IsoDayOfWeek> source) => new IsoDayOfWeekSet(source);
    }
}
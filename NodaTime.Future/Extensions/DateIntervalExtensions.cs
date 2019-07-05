using System.Collections.Generic;
using NodaTime.IntervalComparers;

namespace NodaTime.Extensions
{
    public static class DateIntervalExtensions
    {
        private static readonly IntervalCombiner<DateInterval, LocalDate> IntervalCombiner = new IntervalCombiner<DateInterval, LocalDate>(
            i => i.Start,
            i => i.End,
            (start, end) => new DateInterval(start, end),
            new DateIntervalComparer()
        );

        /// <summary>
        /// Returns the dates in the <see cref="DateInterval"/> in chronological order.
        /// </summary>
        /// <param name="interval">A date interval.</param>
        /// <returns>All dates in the interval from <paramref name="interval.Start"/> to <paramref name="interval.End"/> (both included) in chronological order.</returns>
        public static IEnumerable<LocalDate> Dates( this DateInterval interval )
        {
            for ( var date = interval.Start; date <= interval.End; date = date.NextDay() )
            {
                yield return date;
            }
        }

        /// <summary>
        /// Determines whether the two date intervals overlap, i.e. both intervals contain at least one common date.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="otherInterval">The other interval to check for overlap with the first interval.</param>
        /// <exception cref="System.ArgumentException"><paramref name="otherInterval"/> is not in the same calendar as this interval.</exception>
        /// <returns><c>true</c> if <paramref name="otherInterval"/> overlaps this interval; <c>false</c> otherwise.</returns>
        public static bool Overlaps( this DateInterval interval, DateInterval otherInterval )
        {
            if ( !interval.Start.Calendar.Equals( otherInterval.Start.Calendar ) )
            {
                throw new System.ArgumentException( "The given interval must be in the same calendar as this interval", nameof( otherInterval ) );
            }

            return interval.Start <= otherInterval.End && otherInterval.Start <= interval.End;
        }

        [System.Obsolete("Use " + nameof(GetOverlapWith) + " instead. This method will be removed in the next major version.")]
        public static DateInterval Overlap(this DateInterval interval, DateInterval otherInterval) => interval.GetOverlapWith(otherInterval);

        /// <summary>
        /// Returns the overlap between the two intervals.
        /// </summary>
        /// <param name="interval">The interval to find an overlap with.</param>
        /// <param name="otherInterval">The other interval to find an overlap with.</param>
        /// <exception cref="System.ArgumentException"><paramref name="otherInterval"/> is not in the same calendar as this interval.</exception>
        /// <returns>A date interval equal to the overlap of the two intervals, if they overlap; <c>null</c> otherwise.</returns>
        public static DateInterval GetOverlapWith( this DateInterval interval, DateInterval otherInterval )
        {
            if ( !interval.Start.Calendar.Equals( otherInterval.Start.Calendar ) )
            {
                throw new System.ArgumentException( "The given interval must be in the same calendar as this interval", nameof( otherInterval ) );
            }

            var start = interval.Start >= otherInterval.Start ? interval.Start : otherInterval.Start;
            var end = interval.End <= otherInterval.End ? interval.End : otherInterval.End;

            return start <= end ? new DateInterval( start, end ) : null;
        }

        /// <summary>
        /// Returns the <see cref="DateTimeInterval"/> that represents the same interval as the date interval.
        /// The date time interval starts at the beginning of the start date and ends at the end of end date.
        /// </summary>
        /// <param name="dateInterval">A date interval.</param>
        /// <returns>The <see cref="DateTimeInterval"/> that represents the same interval as the date interval.</returns>
        public static DateTimeInterval ToDateTimeInterval( this DateInterval dateInterval )
        {
            if ( dateInterval == null )
            {
                throw new System.ArgumentNullException( nameof( dateInterval ) );
            }

            return new DateTimeInterval( dateInterval.Start.AtMidnight(), dateInterval.End.NextDay().AtMidnight() );
        }

        /// <summary>
        /// Combines the date intervals in <paramref name="source"/> that overlap or meet and returns them in chronological order.
        /// </summary>
        /// <param name="source">The date intervals.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="source"/> or a date interval is <c>null</c>.</exception>
        /// <remarks>
        /// <para>The returned date intervals cover the same date as <paramref name="source"/>.</para>
        /// <para>Date intervals that meet, but do not overlap, are also combined.</para>
        /// <para>The result is ordered by both start and end of the date intervals.</para>
        /// </remarks>
        public static IEnumerable<DateInterval> CombineIntervals(this IEnumerable<DateInterval> source) => IntervalCombiner.CombineIntervals(source);

        /// <summary>
        /// Combines the overlapping date intervals in <paramref name="source"/> and returns them in chronological order.
        /// </summary>
        /// <param name="source">The date intervals.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="source"/> or a date interval is <c>null</c>.</exception>
        /// <remarks>
        /// <para>The returned date intervals cover the same dates as <paramref name="source"/>.</para>
        /// <para>Date intervals that meet, but do not overlap, are not combined.</para>
        /// <para>The result is ordered by both start and end.</para>
        /// </remarks>
        public static IEnumerable<DateInterval> CombineOverlappingIntervals(this IEnumerable<DateInterval> source) => IntervalCombiner.CombineOverlappingIntervals(source);

        /// <summary>
        /// Returns the dates in the period that fall on the given day of week.
        /// </summary>
        /// <param name="dateInterval">A date interval.</param>
        /// <param name="dayOfWeek">A day of the week.</param>
        /// <returns>The dates in the period that fall on the given day of week.</returns>
        public static IEnumerable<LocalDate> GetDatesThatFallOn(this DateInterval dateInterval, IsoDayOfWeek dayOfWeek)
        {
            for (var date = dateInterval.Start.With(DateAdjusters.NextOrSame(dayOfWeek));
                dateInterval.Contains(date);
                date = date.With(DateAdjusters.Next(dayOfWeek))
            )
            {
                yield return date;
            }
        }
    }
}
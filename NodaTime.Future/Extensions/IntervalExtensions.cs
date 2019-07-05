using System.Collections.Generic;
using System.Linq;
using NodaTime.IntervalComparers;

namespace NodaTime.Extensions
{
    public static class IntervalExtensions
    {
        private static readonly IntervalCombiner<Interval, Instant> IntervalCombiner = new IntervalCombiner<Interval, Instant>(
            i => i.Start,
            i => i.End,
            (start, end) => new Interval(start, end),
            new IntervalComparer()
        );

        /// <summary>
        /// Maps an <see cref="Interval"/> strictly to the given time zone.
        /// </summary>
        /// <param name="interval">An interval.</param>
        /// <param name="timeZone">A time zone.</param>
        /// <returns>A <see cref="DateTimeInterval"/> representing <paramref name="interval"/> in the given time zone.</returns>
        /// <exception cref="InvalidLocalDateTimeInterval">If the local date time interval is invalid,
        /// because the local end date time is before the local start date time, i.e. due to an offset transition between <paramref name="interval.Start"/>
        /// and <paramref name="interval.End"/> (most likely Daylight Saving Time).</exception>
        public static DateTimeInterval InZoneStrictly(this Interval interval, DateTimeZone timeZone)
        {
            var startDateTime = interval.Start.InZone(timeZone).LocalDateTime;
            var endDateTime = interval.End.InZone(timeZone).LocalDateTime;

            if (endDateTime < startDateTime)
            {
                throw new InvalidLocalDateTimeInterval(interval, timeZone, startDateTime, endDateTime);
            }

            return new DateTimeInterval(startDateTime, endDateTime);
        }

        /// <summary>
        /// Determines if two intervals overlap.
        /// </summary>
        /// <param name="first">The first interval.</param>
        /// <param name="second">The second interval.</param>
        /// <returns><c>true</c>, if the intervals overlap, i.e. their intersection is non-empty; otherwise, <c>false</c>.</returns>
        public static bool Overlaps(this Interval first, Interval second) => first.Start < second.End && second.Start < first.End;

        /// <summary>
        /// Determines if the first interval contains the whole of the second interval, possibly sharing endpoints.
        /// </summary>
        /// <param name="first">The container interval.</param>
        /// <param name="second">The contained interval.</param>
        /// <returns><c>true</c>, if the second interval is contained in the first; otherwise, <c>false</c>.</returns>
        public static bool Contains(this Interval first, Interval second) => first.Start <= second.Start && first.End >= second.End;

        /// <summary>
        /// Creates a new interval with the given start and the given interval's end.
        /// </summary>
        /// <param name="interval">The interval whose end should be used in the new interval.</param>
        /// <param name="start">The start in the new interval.</param>
        public static Interval WithStart(this Interval interval, Instant start) => new Interval(start, interval.End);

        /// <summary>
        /// Creates a new interval with the given interval's start and the given end.
        /// </summary>
        /// <param name="interval">The interval whose start should be used in the new interval.</param>
        /// <param name="end">The end in the new interval.</param>
        public static Interval WithEnd(this Interval interval, Instant end) => new Interval(interval.Start, end);

        /// <summary>
        /// Serializes the <see cref="Interval"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="interval">An interval.</param>
        /// <returns>A machine-readable string representation of <paramref name="interval"/>.</returns>
        public static string ToStandardString(this Interval interval) => $"{interval.Start.ToStandardString()}/{interval.End.ToStandardString()}";

        /// <summary>
        /// Combines the intervals in <paramref name="source"/> that overlap or meet and returns them in chronological order.
        /// </summary>
        /// <param name="source">The intervals.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        /// <remarks>
        /// <para>The returned intervals cover the same instants as <paramref name="source"/>.</para>
        /// <para>Intervals that meet, but do not overlap, are combined.</para>
        /// <para>The result is ordered by both start and end of the intervals.</para>
        /// </remarks>
        public static IEnumerable<Interval> CombineIntervals(this IEnumerable<Interval> source) => IntervalCombiner.CombineIntervals(source);

        /// <summary>
        /// Combines the overlapping intervals in <paramref name="source"/> and returns them in chronological order.
        /// </summary>
        /// <param name="source">The intervals.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        /// <remarks>
        /// <para>The returned intervals cover the same instants as <paramref name="source"/>.</para>
        /// <para>Intervals that meet, but do not overlap, are not combined.</para>
        /// <para>The result is ordered by both start and end.</para>
        /// </remarks>
        public static IEnumerable<Interval> CombineOverlappingIntervals(this IEnumerable<Interval> source) => IntervalCombiner.CombineOverlappingIntervals(source);

        [System.Obsolete("Use " + nameof(GetOverlapWith) + " instead. This method will be removed in the next major version.")]
        public static Interval? Overlap(this Interval first, Interval second) => first.GetOverlapWith(second);

        public static Interval? GetOverlapWith(this Interval first, Interval second)
        {
            var start = first.Start >= second.Start ? first.Start : second.Start;
            var end = first.End <= second.End ? first.End : second.End;

            return start < end ? new Interval(start, end) : default(Interval?);
        }

        public static IEnumerable<Interval> GetOverlapsWithDayInTimeZone(this Interval interval, IsoDayOfWeek dayOfWeek, DateTimeZone timeZone)
        {
            // Get a range of dates that covers the input interval. This is deliberately
            // larger than it may need to be, to handle days starting at different instants
            // in different time zones. 
            var period = new DateInterval(
                interval.Start.InZone(DateTimeZone.Utc).Date.PlusDays(-1),
                interval.End.InZone(DateTimeZone.Utc).Date.PlusDays(2)
            );

            // Convert those dates into intervals, each of which may or may not overlap with our input.
            var intervals = period
                .GetDatesThatFallOn(dayOfWeek)
                .Select(date => date.ToIntervalInZone(timeZone));

            // Find the intersection of each date-interval with our input, and discard any non-overlaps
            return intervals
                .Select(dateInterval => dateInterval.GetOverlapWith(interval))
                .Where(x => x != null)
                .Select(x => x.Value);
        }

        public static Duration? DurationBefore(this Interval interval, Instant instant)
        {
            if (instant < interval.Start)
            {
                return default(Duration?);
            }

            if (instant < interval.End)
            {
                return instant - interval.Start;
            }

            return interval.Duration;
        }
    }
}
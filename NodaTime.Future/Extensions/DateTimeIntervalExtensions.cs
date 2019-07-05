using System.Collections.Generic;
using NodaTime.IntervalComparers;

namespace NodaTime.Extensions
{
    public static class DateTimeIntervalExtensions
    {
        private static readonly IntervalCombiner<DateTimeInterval, LocalDateTime> IntervalCombiner = new IntervalCombiner<DateTimeInterval, LocalDateTime>(
            i => i.Start,
            i => i.End,
            (start, end) => new DateTimeInterval(start, end),
            new DateTimeIntervalComparer()
        );

        /// <summary>
        /// Returns a single-day date time interval starting at the beginning of the given date and ending at the end of the given date.
        /// </summary>
        /// <param name="date">The local date.</param>
        /// <returns>A single-day interval starting and ending on the given date.</returns>
        public static DateTimeInterval ToSingleDayDateTimeInterval(this LocalDate date) => date.ToSingleDayInterval().ToDateTimeInterval();

        /// <summary>
        /// Maps the <see cref="DateTimeInterval"/> leniently to the given time zone.
        /// </summary>
        /// <returns>The <see cref="Interval"/> that was leniently mapping for the given time zone.</returns>
        public static Interval InZone(this DateTimeInterval dateTimeInterval, DateTimeZone timeZone)
        {
            if (dateTimeInterval == null)
            {
                throw new System.ArgumentNullException(nameof(dateTimeInterval));
            }
            if (timeZone == null)
            {
                throw new System.ArgumentNullException(nameof(timeZone));
            }

            var start = timeZone.AtLeniently(dateTimeInterval.Start).ToInstant();
            var end = timeZone.AtLeniently(dateTimeInterval.End).ToInstant();
            return new Interval(start, end);
        }

        /// <summary>
        /// Converts the given <see cref="DateTimeInterval"/> to a <see cref="DateInterval"/> covering the same dates.
        /// </summary>
        /// <param name="dateTimeInterval">A date time interval</param>
        /// <returns>The <see cref="DateInterval"/> that covers the same dates as <paramref name="dateTimeInterval"/>.</returns>
        public static DateInterval ToDateInterval(this DateTimeInterval dateTimeInterval)
        {
            var startDate = dateTimeInterval.Start.Date;
            var endDate = dateTimeInterval.End.TimeOfDay == LocalTime.Midnight && !dateTimeInterval.IsSinglePointInterval()
                ? dateTimeInterval.End.Date.PreviousDay()
                : dateTimeInterval.End.Date;

            return new DateInterval(startDate, endDate);
        }

        [System.Obsolete("Use " + nameof(IsSinglePointInterval) + " instead. This method will be removed in the next major version.")]
        public static bool IsEmptyInterval(this DateTimeInterval dateTimeInterval) => dateTimeInterval.IsSinglePointInterval();

        /// <summary>
        /// Determines whether the date time interval is a single-point interval where start and end are equal.
        /// </summary>
        /// <param name="dateTimeInterval">The date time interval.</param>
        /// <returns><c>true</c> if <paramref name="dateTimeInterval"/>'s start and end are equal; <c>false</c>, otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="dateTimeInterval"/> is <c>null</c>.</exception>
        public static bool IsSinglePointInterval(this DateTimeInterval dateTimeInterval)
        {
            if (dateTimeInterval == null)
            {
                throw new System.ArgumentNullException(nameof(dateTimeInterval));
            }

            return dateTimeInterval.Start == dateTimeInterval.End;
        }

        [System.Obsolete("Use " + nameof(GetOverlapWith) + " instead. This method will be removed in the next major version.")]
        public static DateTimeInterval Overlap(this DateTimeInterval first, DateTimeInterval second) => first.GetOverlapWith(second);

        /// <summary>
        /// Returns the overlap between the two intervals.
        /// </summary>
        /// <param name="first">An interval.</param>
        /// <param name="second">The other interval to find an overlap with.</param>
        /// <returns>A date time interval equal to the overlap of the two intervals, if they overlap; <c>null</c> otherwise.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="first"/> is not in the same calendar as <paramref name="second"/>.</exception>
        /// <exception cref="System.ArgumentNullException">If <paramref name="first"/> or <paramref name="second"/> is <c>null</c>.</exception>
        public static DateTimeInterval GetOverlapWith(this DateTimeInterval first, DateTimeInterval second)
        {
            if (first == null)
            {
                throw new System.ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new System.ArgumentNullException(nameof(second));
            }

            if (!first.Start.Calendar.Equals(second.Start.Calendar))
            {
                throw new System.ArgumentException("The given interval must be in the same calendar as this interval.", nameof(second));
            }

            var start = first.Start >= second.Start ? first.Start : second.Start;
            var end = first.End <= second.End ? first.End : second.End;

            return first.Overlaps(second) ? new DateTimeInterval(start, end) : null;
        }

        /// <summary>
        /// Converts the given <see cref="DateTimeInterval"/> to a <see cref="TimeInterval"/> covering the same local time.
        /// </summary>
        /// <param name="interval">A date time interval.</param>
        /// <returns>The time interval that covers the same times as <paramref name="interval"/>.</returns>
        /// <remarks>
        /// This only works for date time intervals with a duration of a day or less.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">If <paramref name="interval"/> is <c>null</c>.</exception>
        /// <exception cref="System.NotSupportedException">If <paramref name="interval"/> is empty or longer than a day.</exception>
        public static TimeInterval ToTimeInterval(this DateTimeInterval interval)
        {
            if (interval == null)
            {
                throw new System.ArgumentNullException(nameof(interval));
            }

            if (interval.IsSinglePointInterval())
            {
                throw new System.NotSupportedException("Empty time intervals are not supported.");
            }

            var maxEnd = interval.Start + Period.FromDays(1);
            if (maxEnd < interval.End)
            {
                throw new System.NotSupportedException("The interval can at most be one day long.");
            }

            return new TimeInterval(interval.Start.TimeOfDay, interval.End.TimeOfDay);
        }

        /// <summary>
        /// Combines the date time intervals in <paramref name="source"/> that overlap or meet and returns them in chronological order.
        /// </summary>
        /// <param name="source">The date time intervals.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="source"/> or a date time interval is <c>null</c>.</exception>
        /// <remarks>
        /// <para>The returned date time intervals cover the same date times as <paramref name="source"/>.</para>
        /// <para>Date time intervals that meet, but do not overlap, are also combined.</para>
        /// <para>The result is ordered by both start and end of the date time intervals.</para>
        /// </remarks>
        public static IEnumerable<DateTimeInterval> CombineIntervals(this IEnumerable<DateTimeInterval> source) => IntervalCombiner.CombineIntervals(source);

        /// <summary>
        /// Combines the overlapping date time intervals in <paramref name="source"/> and returns them in chronological order.
        /// </summary>
        /// <param name="source">The date time intervals.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="source"/> or a date time interval is <c>null</c>.</exception>
        /// <remarks>
        /// <para>The returned date time intervals cover the same time dates as <paramref name="source"/>.</para>
        /// <para>Date time intervals that meet, but do not overlap, are not combined.</para>
        /// <para>The result is ordered by both start and end.</para>
        /// </remarks>
        public static IEnumerable<DateTimeInterval> CombineOverlappingIntervals(this IEnumerable<DateTimeInterval> source) => IntervalCombiner.CombineOverlappingIntervals(source);
    }
}
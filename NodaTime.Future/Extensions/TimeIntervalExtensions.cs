using System.Collections.Generic;

namespace NodaTime.Extensions
{
    public static class TimeIntervalExtensions
    {
        /// <summary>
        /// Checks whether the given time interval is within this time interval.
        /// </summary>
        /// <param name="first">The time interval to check if contains the other interval.</param>
        /// <param name="second">The time interval to check for containment within the first interval.</param>
        /// <returns><c>true</c> if <paramref name="second"/> is within this interval; <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="second"/> is <c>null</c>.</exception>
        public static bool Contains(this TimeInterval first, TimeInterval second)
        {
            if (first == null)
            {
                throw new System.ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new System.ArgumentNullException(nameof(second));
            }

            var overMidnight = first.Start >= first.End;
            var otherOverMidnight = second.Start >= second.End;

            if (overMidnight && !otherOverMidnight)
            {
                return first.Start <= second.Start && first.Start <= second.End || second.Start <= first.End && second.End <= first.End;
            }

            if (!overMidnight && otherOverMidnight)
            {
                return false;
            }

            return first.Start <= second.Start && second.End <= first.End;
        }

        /// <summary>
        /// Determines whether the time interval overlaps the date time interval.
        /// </summary>
        /// <param name="timeInterval">The time interval.</param>
        /// <param name="dateTimeInterval">The date time interval.</param>
        /// <returns><c>true</c> if the time interval overlaps the date time interval; <c>false</c>, otherwise.</returns>
        /// <remarks>
        /// This will always be <c>true</c> for date time intervals that are longer than a day.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">If <paramref name="timeInterval"/> or <paramref name="dateTimeInterval"/> is <c>null</c>.</exception>
        public static bool Overlaps(this TimeInterval timeInterval, DateTimeInterval dateTimeInterval)
        {
            if (timeInterval == null)
            {
                throw new System.ArgumentNullException(nameof(timeInterval));
            }

            if (dateTimeInterval == null)
            {
                throw new System.ArgumentNullException(nameof(dateTimeInterval));
            }

            if (dateTimeInterval.IsSinglePointInterval())
            {
                return timeInterval.Contains(dateTimeInterval.Start.TimeOfDay);
            }

            var maxEnd = dateTimeInterval.Start + Period.FromDays(1);
            return maxEnd < dateTimeInterval.End || dateTimeInterval.ToTimeInterval().Overlaps(timeInterval);
        }

        /// <summary>
        /// Returns the overlaps between the two time intervals.
        /// </summary>
        /// <remarks>
        /// The result with contain either 0, 1 or 2 intervals. Both <paramref name="first"/> and <paramref name="second"/>
        /// will always contain (see <see cref="Contains"/>) the returned time intervals.
        /// </remarks>
        /// <param name="first">A time interval.</param>
        /// <param name="second">The time interval to find overlaps with.</param>
        /// <returns>The overlaps between the two time intervals.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="first"/> or <paramref name="second"/> is <c>null</c>.</exception>
        public static IEnumerable<TimeInterval> GetOverlapsWith(this TimeInterval first, TimeInterval second)
        {
            if (first == null)
            {
                throw new System.ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new System.ArgumentNullException(nameof(second));
            }

            // TODO: Try to simplify

            if (!first.Overlaps(second))
            {
                yield break;
            }

            if (first.Contains(second))
            {
                yield return second;
            }
            else if (second.Contains(first))
            {
                yield return first;
            }
            else
            {
                var interval1 = new TimeInterval(first.Start, second.End);
                if (first.Contains(interval1) && second.Contains(interval1))
                {
                    yield return interval1;
                }

                var interval2 = new TimeInterval(second.Start, first.End);
                if (first.Contains(interval2) && second.Contains(interval2))
                {
                    yield return interval2;
                }
            }
        }

        /// <summary>
        /// Returns the date time intervals where <paramref name="timeInterval"/> overlaps <paramref name="dateTimeInterval"/>.
        /// </summary>
        /// <param name="timeInterval">The time interval.</param>
        /// <param name="dateTimeInterval">The date time interval.</param>
        /// <returns>Empty result, if the two intervals don't overlap; otherwise, each date time interval where they overlap.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="timeInterval"/> or <paramref name="dateTimeInterval"/> is <c>null</c>.</exception>
        public static IEnumerable<DateTimeInterval> GetOverlapsWith(this TimeInterval timeInterval, DateTimeInterval dateTimeInterval)
        {
            if (timeInterval == null)
            {
                throw new System.ArgumentNullException(nameof(timeInterval));
            }

            if (dateTimeInterval == null)
            {
                throw new System.ArgumentNullException(nameof(dateTimeInterval));
            }
            
            if (!timeInterval.Overlaps(dateTimeInterval))
            {
                yield break;
            }

            if (dateTimeInterval.IsSinglePointInterval())
            {
                yield return dateTimeInterval;
                yield break;
            }

            var start = timeInterval.Contains(dateTimeInterval.Start.TimeOfDay)
                ? dateTimeInterval.Start
                : dateTimeInterval.Start.NextOrSame(timeInterval.Start);

            var end = start.Next(timeInterval.End);

            while (start < dateTimeInterval.End)
            {
                if (dateTimeInterval.End < end)
                {
                    end = dateTimeInterval.End;
                }

                yield return new DateTimeInterval(start, end);

                start = start.Next(timeInterval.Start);
                end = start.Next(timeInterval.End);
            }
        }

        /// <summary>
        /// Combines the time intervals in <paramref name="source"/> that overlap or meet and returns them in "chronological" order.
        /// </summary>
        /// <param name="source">The time intervals.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="source"/> or a time interval is <c>null</c>.</exception>
        /// <remarks>
        /// <para>The returned time intervals cover the same times as <paramref name="source"/>.</para>
        /// <para>Time intervals that meet, but do not overlap, are also combined.</para>
        /// <para>The result is ordered by both start and end of the time intervals.</para>
        /// </remarks>
        public static IEnumerable<TimeInterval> CombineIntervals(this IEnumerable<TimeInterval> source)
        {
            // TODO: Define how this should work if resulting interval is 24 hours.
            throw new System.NotImplementedException();
        }
    }
}
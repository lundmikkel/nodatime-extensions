using System.Collections.Generic;
using System.Linq;
using NodaTime.IntervalComparers;

namespace NodaTime.Extensions
{
    public static class IntervalSequenceExtensions
    {
        public static IEnumerable<Interval> GetOverlapsBetweenSets(this IEnumerable<IEnumerable<Interval>> intervals)
        {
            var intervalsArray = intervals.ToArray();

            if (intervalsArray.Any(i => !i.Any()))
            {
                return new List<Interval>();
            }

            return intervalsArray.Skip(1).Aggregate(intervalsArray[0], (agg, next) => agg.GetOverlapsWith(next));
        }
        public static IEnumerable<Interval> GetOverlapsBetweenSets(this IEnumerable<IEnumerable<Interval>> intervals, int minimumNumberOverlapping)
        {
            var intervalsArray = intervals.ToArray();

            var intervalsIntersect = intervalsArray.GetOverlapsBetweenSets();

            return intervalsIntersect.Where(i => intervalsArray.Where(x => x.Any(y => y.Overlaps(i))).Skip(minimumNumberOverlapping).Any());
        }

        /// <summary>
        /// Finds the intervals of overlap between the two enumerables of <seealso cref="Interval"/>.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>The intervals of overlap between the two enumerables.</returns>
        public static IEnumerable<Interval> GetOverlapsWith(this IEnumerable<Interval> first, IEnumerable<Interval> second)
        {
            // Normalize sources
            first = first?.CombineIntervals() ?? throw new System.ArgumentNullException(nameof(first));
            second = second?.CombineIntervals() ?? throw new System.ArgumentNullException(nameof(second));

            using (var firstEnumerator = first.GetEnumerator())
            using (var secondEnumerator = second.GetEnumerator())
            {
                var advanceFirst = true;
                var advanceSecond = true;

                while (!(advanceFirst && !firstEnumerator.MoveNext()) && !(advanceSecond && !secondEnumerator.MoveNext()))
                {
                    advanceFirst = advanceSecond = false;

                    var x = firstEnumerator.Current;
                    var y = secondEnumerator.Current;

                    var overlap = x.GetOverlapWith(y);

                    if (overlap.HasValue)
                    {
                        yield return overlap.Value;
                    }

                    var xEndsFirst = x.End <= y.End;
                    if (xEndsFirst)
                    {
                        advanceFirst = true;
                    }
                    else
                    {
                        advanceSecond = true;
                    }
                }
            }
        }

        public static IEnumerable<Interval> Subtract(this IEnumerable<Interval> intervals, IEnumerable<Interval> intervalsToSubtract)
        {
            // Normalize sources
            intervals = intervals?.CombineIntervals() ?? throw new System.ArgumentNullException(nameof(intervals));
            intervalsToSubtract = intervalsToSubtract?.CombineIntervals() ?? throw new System.ArgumentNullException(nameof(intervalsToSubtract));

            using (var firstEnumerator = intervals.GetEnumerator())
            using (var secondEnumerator = intervalsToSubtract.GetEnumerator())
            {
                // Nothing to remove from
                if (!firstEnumerator.MoveNext())
                {
                    yield break;
                }

                // Nothing to remove; just return the combined intervals
                if (!secondEnumerator.MoveNext())
                {
                    do { yield return firstEnumerator.Current; }
                    while (firstEnumerator.MoveNext());
                    yield break;
                }

                // Start of the next interval returned
                var nextStart = firstEnumerator.Current.Start;
                var nextEnd = firstEnumerator.Current.End;

                var y = secondEnumerator.Current;

                while (true)
                {
                    if (nextEnd <= y.Start)
                    {
                        yield return new Interval(nextStart, nextEnd);
                    }
                    else if (nextStart < y.End)
                    {
                        if (nextStart < y.Start)
                        {
                            yield return new Interval(nextStart, y.Start);
                        }

                        if (y.End < nextEnd) // TODO: Needed? Doesn't seem like it
                        {
                            nextStart = y.End;
                        }
                    }

                    var advanceX = y.End >= nextEnd;
                    var advanceY = y.End <= nextEnd;

                    if (advanceX)
                    {
                        if (!firstEnumerator.MoveNext())
                        {
                            yield break;
                        }

                        nextStart = firstEnumerator.Current.Start;
                        nextEnd = firstEnumerator.Current.End;
                    }

                    if (advanceY)
                    {
                        if (!secondEnumerator.MoveNext())
                        {
                            yield return new Interval(nextStart, nextEnd);

                            while (firstEnumerator.MoveNext())
                            {
                                yield return firstEnumerator.Current;
                            }
                            yield break;
                        }

                        y = secondEnumerator.Current;
                    }
                }
            }
        }

        public static Instant? DetermineWhenDurationHasElapsed(this IEnumerable<Interval> intervals, Duration duration)
        {
            #region Setup

            if (intervals == null)
            {
                throw new System.ArgumentNullException(nameof(intervals));
            }

            if (duration < Duration.Zero)
            {
                throw new System.ArgumentOutOfRangeException(nameof(duration), "Duration cannot be negative.");
            }

            var list = intervals.ToList();

            if (!list.Any())
            {
                return default(Instant?);
            }

            if (duration == Duration.Zero)
            {
                return list.Min(i => i.Start);
            }

            list.Sort(new IntervalComparer());

            #endregion

            using (var enumerator = list.GetEnumerator())
            {
                // Get first endpoint
                enumerator.MoveNext();
                var previousEndpoint = enumerator.Current.Start;

                var endpoints = new SortedSet<Instant> // TODO: Use priority list instead
                {
                    enumerator.Current.End
                };

                var index = 0;
                var previousDuration = Duration.Zero;
                
                while (endpoints.Count > 0)
                {
                    var hasMoreIntervals = false;
                    while (enumerator.Current.Start < endpoints.Min && (hasMoreIntervals = enumerator.MoveNext()))
                    {
                        ++index;
                        endpoints.Add(enumerator.Current.Start);
                        endpoints.Add(enumerator.Current.End);
                    }

                    if (!hasMoreIntervals)
                    {
                        ++index;
                    }

                    var currentEndpoint = endpoints.Min;
                    
                    // ReSharper disable once PossibleInvalidOperationException
                    var durationBeforeCurrentEndpoint = list
                        .Take(index)
                        .Select(interval => interval.DurationBefore(currentEndpoint))
                        .Sum()
                        .Value;
                    
                    // Went too far, so we need to calculate the right intermediate
                    if (durationBeforeCurrentEndpoint >= duration)
                    {
                        var a = duration - previousDuration;
                        var b = durationBeforeCurrentEndpoint - previousDuration;

                        var factor = a / b;
                        var difference = currentEndpoint - previousEndpoint;
                        // Multiplying with 1 doesn't round trip
                        var offset = factor == 1 ? difference : difference * factor;
                        
                        var instant = previousEndpoint + offset;
                        return instant;
                    }

                    endpoints.Remove(currentEndpoint);
                    previousEndpoint = currentEndpoint;
                    previousDuration = durationBeforeCurrentEndpoint;
                }

                return null;
            }
        }

        private static IEnumerable<Instant> Endpoints(this IEnumerable<Interval> intervals)
        {
            var sortedSet = new SortedSet<Instant>();

            foreach (var interval in intervals)
            {
                sortedSet.Add(interval.Start);
                sortedSet.Add(interval.End);
            }

            return sortedSet;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime
{
    internal class IntervalCombiner<TInterval, TEndpoint> where TEndpoint : IComparable<TEndpoint>
    {
        private readonly Func<TInterval, TEndpoint> _getStart;
        private readonly Func<TInterval, TEndpoint> _getEnd;
        private readonly Func<TEndpoint, TEndpoint, TInterval> _createInterval;
        private readonly IComparer<TInterval> _comparer;

        public IntervalCombiner(
            Func<TInterval, TEndpoint> getStart,
            Func<TInterval, TEndpoint> getEnd,
            Func<TEndpoint, TEndpoint, TInterval> createInterval,
            IComparer<TInterval> comparer
        )
        {
            _getStart = getStart;
            _getEnd = getEnd;
            _createInterval = createInterval;
            _comparer = comparer;
        }

        public IEnumerable<TInterval> CombineIntervals(IEnumerable<TInterval> source) => CombineIntervals(source, true);

        public IEnumerable<TInterval> CombineOverlappingIntervals(IEnumerable<TInterval> source) => CombineIntervals(source, false);

        private IEnumerable<TInterval> CombineIntervals(IEnumerable<TInterval> source, bool combineMeetingIntervals)
        {
            var list = source?.ToList() ?? throw new ArgumentNullException(nameof(source));
            list.Sort(_comparer);

            using (var enumerator = list.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }

                var start = _getStart(enumerator.Current);
                var end = _getEnd(enumerator.Current);

                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    var currentStart = _getStart(current);
                    var currentEnd = _getEnd(current);

                    // Overlapping interval
                    var compareTo = currentStart.CompareTo(end);
                    if (compareTo < 0 || combineMeetingIntervals && compareTo == 0)
                    {
                        // Update end
                        if (end.CompareTo(currentEnd) < 0)
                        {
                            end = currentEnd;
                        }
                    }
                    // Non-overlapping interval
                    else
                    {
                        yield return _createInterval(start, end);

                        start = currentStart;
                        end = currentEnd;
                    }
                }

                yield return _createInterval(start, end);
            }
        }
    }
}
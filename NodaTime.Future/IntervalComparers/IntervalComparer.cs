using System.Collections.Generic;

namespace NodaTime.IntervalComparers
{
    public class IntervalComparer : IComparer<Interval>
    {
        public int Compare(Interval x, Interval y)
        {
            var compareStart = x.Start.CompareTo(y.Start);
            return compareStart != 0 ? compareStart : x.End.CompareTo(y.End);
        }
    }
}
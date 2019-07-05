using System.Collections.Generic;

namespace NodaTime.IntervalComparers
{
    public class DateIntervalComparer : IComparer<DateInterval>
    {
        public int Compare(DateInterval x, DateInterval y)
        {
            if (x is null && y is null)
            {
                return 0;
            }
            if (x is null)
            {
                return -1;
            }
            if (y is null)
            {
                return 1;
            }

            var compareStart = x.Start.CompareTo(y.Start);
            return compareStart != 0 ? compareStart : x.End.CompareTo(y.End);
        }
    }
}
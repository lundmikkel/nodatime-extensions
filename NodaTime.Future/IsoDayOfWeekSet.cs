using System;
using System.Collections;
using System.Collections.Generic;

namespace NodaTime
{
    public class IsoDayOfWeekSet : ICollection<IsoDayOfWeek>, IEquatable<IsoDayOfWeekSet>
    {
        private byte _days;

        public IsoDayOfWeekSet()
        { }

        public IsoDayOfWeekSet(IEnumerable<IsoDayOfWeek> daysOfWeek)
        {
            foreach (var dayOfWeek in daysOfWeek)
            {
                Add(dayOfWeek);
            }
        }

        public static IsoDayOfWeekSet WithAllDays => new IsoDayOfWeekSet { _days = 0b11111110 };

        public IEnumerator<IsoDayOfWeek> GetEnumerator()
        {
            // TODO: Improve?
            for (var day = IsoDayOfWeek.Monday; day <= IsoDayOfWeek.Sunday; ++day)
            {
                if (Contains(day))
                {
                    yield return day;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IsoDayOfWeek dayOfWeek)
        {
            if (dayOfWeek < IsoDayOfWeek.Monday || IsoDayOfWeek.Sunday < dayOfWeek)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
            }

            _days |= ToByte(dayOfWeek);
        }

        public void Clear() => _days = 0;

        public bool Contains(IsoDayOfWeek dayOfWeek) => (_days & ToByte(dayOfWeek)) > 0;

        public void CopyTo(IsoDayOfWeek[] array, int arrayIndex)
        {
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    array[arrayIndex++] = enumerator.Current;
                }
            }
        }

        // TODO: Test
        public bool IntersectsWith(IsoDayOfWeekSet other) => (_days & other._days) > 0;

        public bool Remove(IsoDayOfWeek dayOfWeek)
        {
            var result = Contains(dayOfWeek);
            _days &= (byte)~ToByte(dayOfWeek);
            return result;
        }

        public int Count
        {
            get {
                var i = (int)_days;
                i = i - ((i >> 1) & 0b01010101);
                i = (i & 0b00110011) + ((i >> 2) & 0b00110011);
                return (i + (i >> 4)) & 0b00001111;
            }
        }

        public bool IsReadOnly => false;

        private byte ToByte(IsoDayOfWeek dayOfWeek) => (byte)(1 << (int)dayOfWeek);

        #region Equality

        public bool Equals(IsoDayOfWeekSet other) => !(other is null) && (ReferenceEquals(this, other) || _days == other._days);

        public override bool Equals(object obj) => !(obj is null) && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((IsoDayOfWeekSet) obj));

        public override int GetHashCode() => _days.GetHashCode();

        public static bool operator ==(IsoDayOfWeekSet left, IsoDayOfWeekSet right) => Equals(left, right);

        public static bool operator !=(IsoDayOfWeekSet left, IsoDayOfWeekSet right) => !Equals(left, right);

        #endregion
    }
}
namespace NodaTime
{
    /// <summary>
    /// An interval between two local times.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The interval includes the start time and excludes the end time.
    /// If <see cref="End"/> is less than <see cref="Start"/>, then the interval crosses midnight.
    /// If <see cref="End"/> is equal to <see cref="Start"/>, then the interval is 24 hours long.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    /// <inheritdoc />
    // TODO: [Immutable]
    public sealed class TimeInterval : System.IEquatable<TimeInterval>
    {
        /// <summary>
        /// Gets the start time of the interval.
        /// </summary>
        /// <value>The start time of the interval.</value>
        public LocalTime Start { get; }

        /// <summary>
        /// Gets the end time of the interval.
        /// </summary>
        /// <value>The end time of the interval.</value>
        public LocalTime End { get; }

        /// <summary>
        /// Constructs a time interval from a start time and an end time,
        /// where the start is included in the interval, and the end is excluded.
        /// </summary>
        /// <param name="start">Start time of the interval</param>
        /// <param name="end">End time of the interval</param>
        /// <returns>A time interval between the specified times.</returns>
        public TimeInterval(LocalTime start, LocalTime end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Returns the hash code for this interval, consistent with <see cref="Equals(TimeInterval)"/>.
        /// </summary>
        /// <returns>The hash code for this interval.</returns>
        public override int GetHashCode() => (17 * 37 + Start.GetHashCode()) * 37 + End.GetHashCode();

        /// <summary>
        /// Compares two <see cref="TimeInterval" /> values for equality.
        /// </summary>
        /// <remarks>
        /// Time intervals are equal if they have the same start and end times.
        /// </remarks>
        /// <param name="lhs">The first value to compare.</param>
        /// <param name="rhs">The second value to compare.</param>
        /// <returns><c>true</c> if the two time intervals have the same properties; <c>false</c> otherwise.</returns>
        public static bool operator ==(TimeInterval lhs, TimeInterval rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs is null || rhs is null)
            {
                return false;
            }

            return lhs.Start == rhs.Start && lhs.End == rhs.End;
        }

        /// <summary>
        /// Compares two <see cref="TimeInterval" /> values for inequality.
        /// </summary>
        /// <remarks>
        /// Time intervals are equal if they have the same start and end times.
        /// </remarks>
        /// <param name="lhs">The first value to compare.</param>
        /// <param name="rhs">The second value to compare.</param>
        /// <returns><c>false</c> if the two time intervals have the same properties; <c>true</c> otherwise.</returns>
        public static bool operator !=(TimeInterval lhs, TimeInterval rhs) => !(lhs == rhs);

        /// <summary>
        /// Compares the given time interval for equality with this one.
        /// </summary>
        /// <remarks>
        /// Time intervals are equal if they have the same start and end times.
        /// </remarks>
        /// <param name="other">The time interval to compare this one with.</param>
        /// <returns><c>true</c> if this time interval has the same properties as the one specified; <c>false</c> otherwise.</returns>
        public bool Equals(TimeInterval other) => this == other;

        /// <summary>
        /// Compares the given object for equality with this one, as per <see cref="Equals(TimeInterval)"/>.
        /// </summary>
        /// <param name="obj">The value to compare this one with.</param>
        /// <returns><c>true</c> if the other object is a time interval equal to this one, consistent with <see cref="Equals(TimeInterval)"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj) => this == obj as TimeInterval;

        /// <summary>
        /// Checks whether the given time is within this time interval.
        /// </summary>
        /// <param name="time">The time to check for containment within this interval.</param>
        /// <returns><c>true</c> if <paramref name="time"/> is within this interval; <c>false</c> otherwise.</returns>
        public bool Contains(LocalTime time)
        {
            var a = Start <= time;
            var b = time < End;

            return Start < End
                ? a && b
                : a || b;
        }

        /// <summary>
        /// Determines whether the time interval overlaps this interval, i.e. both intervals contain at least one common time.
        /// </summary>
        /// <param name="interval">The interval to check for overlap with this interval.</param>
        /// <returns><c>true</c> if <paramref name="interval"/> overlaps this interval; <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="interval"/> is <c>null</c>.</exception>
        public bool Overlaps(TimeInterval interval)
        {
            if (interval == null)
            {
                throw new System.ArgumentNullException(nameof(interval));
            }

            var thisOverMidnight = Start >= End;
            var thatOverMidnight = interval.Start >= interval.End;

            if (!thisOverMidnight && !thatOverMidnight)
            {
                return Start < interval.End && interval.Start < End;
            }

            return thisOverMidnight && thatOverMidnight ||
                   Start < interval.End || interval.Start < End;
        }

        /// <summary>
        /// Gets the period of this time interval.
        /// </summary>
        /// <value>The period of this time interval.</value>
        public Period Period => Start < End
            ? Period.Between(Start, End)
            : (Period.Between(Start, End) + Period.FromDays(1)).Normalize();

        /// <summary>
        /// Returns a string representation of this interval.
        /// </summary>
        /// <returns>
        /// A string representation of this interval, as <c>start/end</c>,
        /// where "start" and "end" are the times formatted using an ISO-8601 compatible pattern.
        /// </returns>
        public override string ToString()
        {
            var pattern = Patterns.LocalTimePattern;
            return pattern.Format(Start) + "/" + pattern.Format(End);
        }
    }
}
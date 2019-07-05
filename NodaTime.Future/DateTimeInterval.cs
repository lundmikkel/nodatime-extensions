using System;
using NodaTime.Text;

namespace NodaTime
{
    /// <summary>
    /// An interval between two date times.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The two date times must be in the same calendar. The interval includes the start date time and excludes the end date time.
    /// The end must not be earlier than the start, but they may be equal (resulting in a single-point interval).
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    /// <inheritdoc />
    // TODO: [Immutable]
    public sealed class DateTimeInterval : IEquatable<DateTimeInterval>
    {
        /// <summary>
        /// Gets the start date time of the interval.
        /// </summary>
        /// <value>The start date time of the interval.</value>
        public LocalDateTime Start { get; }

        /// <summary>
        /// Gets the end date time of the interval.
        /// </summary>
        /// <value>The end date time of the interval.</value>
        public LocalDateTime End { get; }

        /// <summary>
        /// Constructs a date time interval from a start date time and an end date time,
        /// where the start is included in the interval, and the end is excluded.
        /// </summary>
        /// <param name="start">Start date time of the interval.</param>
        /// <param name="end">End date time of the interval.</param>
        /// <exception cref="ArgumentException"><paramref name="end"/> is earlier than <paramref name="start"/>
        /// or the two date times are in different calendars.
        /// </exception>
        /// <returns>A date time interval between the specified date times.</returns>
        public DateTimeInterval( LocalDateTime start, LocalDateTime end )
        {
            if ( !start.Calendar.Equals( end.Calendar ) )
            {
                throw new ArgumentException( "Calendars of start and end date times must be the same.", nameof( end ) );
            }
            if ( end < start )
            {
                throw new ArgumentException( "End date time must not be earlier than the start date time.", nameof( end ) );
            }

            Start = start;
            End = end;
        }

        /// <summary>
        /// Constructs a single-point date time interval that starts and ends at the given <see cref="LocalDateTime"/>.
        /// </summary>
        /// <param name="point">The single date time included in the interval.</param>
        /// <returns>A single-point date time interval only including the given date time.</returns>
        public DateTimeInterval( LocalDateTime point )
        {
            Start = End = point;
        }

        /// <summary>
        /// Constructs a date time interval from a start date time and a period.
        /// </summary>
        /// <param name="start">Start date time of the interval.</param>
        /// <param name="period">The duration of the interval. Must be non-negative.</param>
        /// <exception cref="ArgumentException"><paramref name="period"/> is negative.</exception>
        /// <returns>A date time interval starting at the specified time and with the given duration.</returns>
        public DateTimeInterval( LocalDateTime start, Period period )
        {
            Start = start;
            End = start + period;

            if (End < Start)
            {
                throw new ArgumentException( "Period must be non-negative.", nameof( period ) );
            }
        }

        /// <summary>
        /// Returns the hash code for this interval, consistent with <see cref="Equals(DateTimeInterval)"/>.
        /// </summary>
        /// <returns>The hash code for this interval.</returns>
        public override int GetHashCode() => ( 17 * 37 + Start.GetHashCode() ) * 37 + End.GetHashCode();

        /// <summary>
        /// Compares two <see cref="DateTimeInterval" /> values for equality.
        /// </summary>
        /// <remarks>
        /// Date time intervals are equal if they have the same start and end date times.
        /// </remarks>
        /// <param name="lhs">The first value to compare.</param>
        /// <param name="rhs">The second value to compare.</param>
        /// <returns><c>true</c> if the two date time intervals have the same properties; <c>false</c> otherwise.</returns>
        public static bool operator ==( DateTimeInterval lhs, DateTimeInterval rhs )
        {
            if ( ReferenceEquals( lhs, rhs ) )
            {
                return true;
            }
            if ( lhs is null || rhs is null)
            {
                return false;
            }
            return lhs.Start == rhs.Start && lhs.End == rhs.End;
        }

        /// <summary>
        /// Compares two <see cref="DateTimeInterval" /> values for inequality.
        /// </summary>
        /// <remarks>
        /// Date time intervals are equal if they have the same start and end date times.
        /// </remarks>
        /// <param name="lhs">The first value to compare.</param>
        /// <param name="rhs">The second value to compare.</param>
        /// <returns><c>false</c> if the two date time intervals have the same properties; <c>true</c> otherwise.</returns>
        public static bool operator !=( DateTimeInterval lhs, DateTimeInterval rhs ) => !( lhs == rhs );

        /// <summary>
        /// Compares the given date time interval for equality with this one.
        /// </summary>
        /// <remarks>
        /// Date time intervals are equal if they have the same start and end date times.
        /// </remarks>
        /// <param name="other">The date time interval to compare this one with.</param>
        /// <returns><c>true</c> if this date time interval has the same properties as the one specified; <c>false</c> otherwise.</returns>
        public bool Equals( DateTimeInterval other ) => this == other;

        /// <summary>
        /// Compares the given object for equality with this one, as per <see cref="Equals(DateTimeInterval)"/>.
        /// </summary>
        /// <param name="obj">The value to compare this one with.</param>
        /// <returns><c>true</c> if the other object is a date time interval equal to this one, consistent with <see cref="Equals(DateTimeInterval)"/>; <c>false</c> otherwise.</returns>
        public override bool Equals( object obj ) => this == obj as DateTimeInterval;

        /// <summary>
        /// Checks whether the given date time is within this date time interval. This requires that
        /// the date time is not earlier than the start date time, and not later than the end date time.
        /// </summary>
        /// <param name="dateTime">The date time to check for containment within this interval.</param>
        /// <exception cref="ArgumentException"><paramref name="dateTime"/> is not in the same
        /// calendar as the start and end date of this interval.</exception>
        /// <returns><c>true</c> if <paramref name="dateTime"/> is within this interval; <c>false</c> otherwise.</returns>
        public bool Contains( LocalDateTime dateTime )
        {
            if ( !dateTime.Calendar.Equals( Start.Calendar ) )
            {
                throw new ArgumentException( "The date time to check must be in the same calendar as the start and end date times", nameof( dateTime ) );
            }
            return Start <= dateTime && dateTime < End;
        }

        /// <summary>
        /// Determines whether the date time interval overlaps this interval, i.e. both intervals contain at least one common date time.
        /// </summary>
        /// <param name="interval">The interval to check for overlap with this interval.</param>
        /// <exception cref="ArgumentException"><paramref name="interval"/> is not in the same calendar as this interval.</exception>
        /// <returns><c>true</c> if <paramref name="interval"/> overlaps this interval; <c>false</c> otherwise.</returns>
        public bool Overlaps( DateTimeInterval interval )
        {
            if ( interval == null )
            {
                throw new ArgumentNullException( nameof( interval ) );
            }

            if ( !Start.Calendar.Equals( interval.Start.Calendar ) )
            {
                throw new ArgumentException( "The given interval must be in the same calendar as this interval.", nameof( interval ) );
            }

            return ( interval.Start == interval.End
                    ? Start <= interval.End
                    : Start < interval.End ) &&
                ( Start == End
                    ? interval.Start <= End
                    : interval.Start < End );
        }

        /// <summary>
        /// Gets the period of this date time interval.
        /// </summary>
        /// <value>The period of this date time interval.</value>
        public Period Period => Period.Between( Start, End );

        /// <summary>
        /// Returns a string representation of this interval.
        /// </summary>
        /// <returns>
        /// A string representation of this interval, as <c>start/end</c>,
        /// where "start" and "end" are the date times formatted using an ISO-8601 compatible pattern.
        /// </returns>
        public override string ToString()
        {
            var pattern = LocalDateTimePattern.ExtendedIso;
            return pattern.Format( Start ) + "/" + pattern.Format( End );
        }
    }
}
using System;

namespace NodaTime.Extensions
{
    [System.Obsolete("Ported")]
    public static class DateTimeAdjusters
    {
        /// <summary>
        /// A date time adjuster to move to the next specified time, but return the original date time if the time is already correct.
        /// </summary>
        /// <param name="time">The time to adjust date times to.</param>
        /// <returns>An adjuster which advances a date time to the next occurrence of the
        /// specified time, or the original date time if the time is already correct.</returns>
        public static Func<LocalDateTime, LocalDateTime> NextOrSame(LocalTime time) => dateTime => dateTime.TimeOfDay == time ? dateTime : NextTime(dateTime, time);

        /// <summary>
        /// A date time adjuster to move to the next specified time, adding a day if the time is already correct.
        /// </summary>
        /// <param name="time">The time to adjust date times to.</param>
        /// <returns>An adjuster which advances a date time to the next occurrence of the specified time.</returns>
        public static Func<LocalDateTime, LocalDateTime> Next(LocalTime time) => dateTime => NextTime(dateTime, time);

        private static LocalDateTime NextTime(LocalDateTime dateTime, LocalTime time)
        {
            var date = dateTime.TimeOfDay < time
                ? dateTime.Date
                : dateTime.Date.NextDay();

            return date.At(time);
        }

        /// <summary>
        /// A date time adjuster to move to the previous specified time, but return the original date time if the time is already correct.
        /// </summary>
        /// <param name="time">The time to adjust date times to.</param>
        /// <returns>An adjuster which advances a date time to the previous occurrence of the
        /// specified time, or the original date time if the time is already correct.</returns>
        public static Func<LocalDateTime, LocalDateTime> PreviousOrSame(LocalTime time) => dateTime => dateTime.TimeOfDay == time ? dateTime : PreviousTime(dateTime, time);

        /// <summary>
        /// A date time adjuster to move to the previous specified time, subtracting a day if the time is already correct.
        /// </summary>
        /// <param name="time">The time to adjust date times to.</param>
        /// <returns>An adjuster which advances a date time to the previous occurrence of the specified time.</returns>
        public static Func<LocalDateTime, LocalDateTime> Previous(LocalTime time) => dateTime => PreviousTime(dateTime, time);

        private static LocalDateTime PreviousTime(LocalDateTime dateTime, LocalTime time)
        {
            var date = time < dateTime.TimeOfDay
                ? dateTime.Date
                : dateTime.Date.PreviousDay();

            return date.At(time);
        }
    }
}
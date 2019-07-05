namespace NodaTime.Extensions
{
    public static class ClockExtensions
    {
        /// <summary>
        /// Returns a zoned clock associated with the given time zone.
        /// </summary>
        /// <param name="clock">A clock.</param>
        /// <param name="timeZone">A time zone.</param>
        /// <returns>A zoned clock associated with the given time zone.</returns>
        public static ZonedClock InZone(this IClock clock, DateTimeZone timeZone) => new ZonedClock(clock, timeZone, CalendarSystem.Iso);
    }
}
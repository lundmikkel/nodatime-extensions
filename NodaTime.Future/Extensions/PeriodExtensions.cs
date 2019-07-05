namespace NodaTime.Extensions
{
    public static class PeriodExtensions
    {
        /// <summary>
        /// Returns a new period with only the specified units.
        /// </summary>
        /// <param name="period">The period whose values should be used.</param>
        /// <param name="units">The units that should be taken from the given period.</param>
        /// <returns>A new period with only the specified units.</returns>
        public static Period OnlyWith( this Period period, PeriodUnits units = PeriodUnits.AllUnits )
        {
            var builder = new PeriodBuilder();

            if ( units.HasFlag( PeriodUnits.Years ) )
            {
                builder.Years = period.Years;
            }
            if ( units.HasFlag( PeriodUnits.Months ) )
            {
                builder.Months = period.Months;
            }
            if ( units.HasFlag( PeriodUnits.Weeks ) )
            {
                builder.Weeks = period.Weeks;
            }
            if ( units.HasFlag( PeriodUnits.Days ) )
            {
                builder.Days = period.Days;
            }
            if ( units.HasFlag( PeriodUnits.Hours ) )
            {
                builder.Hours = period.Hours;
            }
            if ( units.HasFlag( PeriodUnits.Minutes ) )
            {
                builder.Minutes = period.Minutes;
            }
            if ( units.HasFlag( PeriodUnits.Seconds ) )
            {
                builder.Seconds = period.Seconds;
            }
            if ( units.HasFlag( PeriodUnits.Milliseconds ) )
            {
                builder.Milliseconds = period.Milliseconds;
            }
            if ( units.HasFlag( PeriodUnits.Ticks ) )
            {
                builder.Ticks = period.Ticks;
            }

            return builder.Build();
        }

        /// <summary>
        /// Serializes the <see cref="Instant"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="period">A period.</param>
        /// <returns>A machine-readable string representation of <paramref name="period"/>.</returns>
        public static string ToStandardString( this Period period ) => period.ToString();
    }
}
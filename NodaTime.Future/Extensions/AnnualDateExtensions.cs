namespace NodaTime.Extensions
{
    public static class AnnualDateExtensions
    {
        /// <summary>
        /// Returns this annual date in the same year as <paramref name="fromDate"/>, unless that date is before <paramref name="fromDate"/>,
        /// then in the next year.
        /// </summary>
        /// <param name="annualDate">An annual date.</param>
        /// <param name="fromDate">The reference date from which the next occurrence should be found.</param>
        /// <returns>The next occurrence of the annual date.</returns>
        /// <remarks>
        /// <para>
        /// If the annual date is the same day as <paramref name="fromDate"/>, that date is returned.
        /// </para>
        /// <para>
        /// As with <see cref="AnnualDate.IsValidYear"/>, if the annual date represents February 29th, and the next occurrence is not in a leap year,
        /// the returned value will be February 28th of that year.
        /// </para>
        /// </remarks>
        public static LocalDate NextOccurrence( this AnnualDate annualDate, LocalDate fromDate )
        {
            var date = annualDate.InYear( fromDate.Year );

            // If the date already passed, go to the next year
            if ( date < fromDate )
            {
                date = annualDate.InYear( fromDate.Year + 1 );
            }

            return date;
        }
    }
}
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="DateInterval"/> with dates between year 1900 and 2100.
    /// </summary>
    public class DateIntervalGenerator : TypedSpecimenBuilder<DateInterval>
    {
        private readonly LocalDateGenerator generator = new LocalDateGenerator();

        public override DateInterval Create( ISpecimenContext context )
        {
            var date1 = generator.Create( context );
            var date2 = generator.Create( context );

            return date1 <= date2
                ? new DateInterval( date1, date2 )
                : new DateInterval( date2, date1 );
        }
    }
}
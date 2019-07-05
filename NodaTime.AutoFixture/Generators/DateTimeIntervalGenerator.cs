using AutoFixture.Kernel;
using NodaTime;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="DateTimeInterval"/> with date times between year 1900 and 2100.
    /// </summary>
    public class DateTimeIntervalGenerator : TypedSpecimenBuilder<DateTimeInterval>
    {
        private readonly LocalDateTimeGenerator generator = new LocalDateTimeGenerator();

        public override DateTimeInterval Create( ISpecimenContext context )
        {
            var dateTime1 = generator.Create( context );
            var dateTime2 = generator.Create( context );

            return dateTime1 <= dateTime2
                ? new DateTimeInterval( dateTime1, dateTime2 )
                : new DateTimeInterval( dateTime2, dateTime1 );
        }
    }
}
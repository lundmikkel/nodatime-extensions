using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="Period"/> less than a week.
    /// </summary>
    public class PeriodGenerator : TypedSpecimenBuilder<Period>
    {
        private static readonly Random Random = new Random();
        private readonly LocalTimeGenerator LocalTimeGenerator = new LocalTimeGenerator();

        public override Period Create( ISpecimenContext context )
        {
            var days = Random.Next( 7 );
            var hoursAndMinutes = LocalTimeGenerator.Create( context );

            // Avoid zero periods
            if ( days == 0 && hoursAndMinutes == LocalTime.Midnight )
            {
                return Create( context );
            }

            var periodBuilder = new PeriodBuilder
            {
                Days = days,
                Hours = hoursAndMinutes.Hour,
                Minutes = hoursAndMinutes.Minute
            };

            return periodBuilder.Build();
        }
    }
}
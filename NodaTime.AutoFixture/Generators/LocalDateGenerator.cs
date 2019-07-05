using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="LocalDate"/> between year 1900 and 2100.
    /// </summary>
    public class LocalDateGenerator : TypedSpecimenBuilder<LocalDate>
    {
        private static readonly Random Random = new Random();
        private static readonly LocalDate Min = new LocalDate( 1900, 1, 1 );
        private static readonly LocalDate Max = Min + Period.FromYears( 200 );
        private static readonly int NumberOfDays = new DateInterval( Min, Max ).Length;

        public override LocalDate Create( ISpecimenContext context )
        {
            var days = Random.Next( NumberOfDays );
            return Min.PlusDays( days );
        }
    }
}
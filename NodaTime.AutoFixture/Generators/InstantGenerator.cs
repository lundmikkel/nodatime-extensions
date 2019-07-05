using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="Instant"/> between year 1900 and 2100.
    /// </summary>
    public class InstantGenerator : TypedSpecimenBuilder<Instant>
    {
        private readonly Random Random = new Random();
        private static readonly Instant Min = Instant.FromUtc( 1900, 01, 01, 00, 00 );
        private static readonly Instant Max = Instant.FromUtc( 2100, 01, 01, 00, 00 );
        private static readonly double NumberOfTicks = ( Max - Min ).TotalTicks;

        public override Instant Create( ISpecimenContext context )
        {
            var ticks = Random.NextDouble() * NumberOfTicks;
            return Min + Duration.FromTicks( ticks );
        }
    }
}
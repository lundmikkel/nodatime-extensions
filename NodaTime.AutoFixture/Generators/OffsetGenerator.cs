using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="Offset"/> in fifteen minute increments.
    /// </summary>
    public class OffsetGenerator : TypedSpecimenBuilder<Offset>
    {
        private static readonly Random Random = new Random();
        private static readonly int MinValueInMinutes = Offset.MinValue.Seconds / NodaConstants.SecondsPerMinute;
        private static readonly int MaxValueInMinutes = Offset.MaxValue.Seconds / NodaConstants.SecondsPerMinute;

        public override Offset Create( ISpecimenContext context )
        {
            var offsetInMinutes = Random.Next( MinValueInMinutes, MaxValueInMinutes );

            // Truncate to 15 minutes
            offsetInMinutes -= offsetInMinutes % 15;

            return Offset.FromTicks( offsetInMinutes * NodaConstants.TicksPerMinute );
        }
    }
}
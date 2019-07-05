using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="LocalTime"/> in ten minute increments.
    /// </summary>
    public class LocalTimeGenerator : TypedSpecimenBuilder<LocalTime>
    {
        private static readonly Random Random = new Random();
        private const int DecaMinutesPerStandardDay = NodaConstants.MinutesPerDay / 10;

        public override LocalTime Create( ISpecimenContext context )
        {
            var minutesSinceMidnight = Random.Next( DecaMinutesPerStandardDay ) * 10;
            var secondsSinceMidnight = minutesSinceMidnight * NodaConstants.SecondsPerMinute;
            return LocalTime.FromSecondsSinceMidnight( secondsSinceMidnight );
        }
    }
}
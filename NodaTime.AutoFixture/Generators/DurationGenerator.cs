using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random, non-negative <see cref="Duration"/>.
    /// </summary>
    public class DurationGenerator : TypedSpecimenBuilder<Duration>
    {
        private static readonly Random Random = new Random();
        private readonly Duration Max;

        /// <summary>
        /// Creates a <see cref="DurationGenerator"/> using the given max duration.
        /// </summary>
        /// <param name="max">A positive duration. If <c>null</c>, a max duration of two standard days is used.</param>
        /// <exception cref="ArgumentException"><paramref name="max"/> is negative.</exception>
        public DurationGenerator( Duration? max = null )
        {
            if ( max < Duration.Zero )
            {
                throw new ArgumentException( "The max duration must be non-negative.", nameof( max ) );
            }

            Max = max ?? Duration.FromDays( 2 );
        }

        public override Duration Create( ISpecimenContext context )
        {
            var minutes = Random.NextDouble() * Max.BclCompatibleTicks / NodaConstants.TicksPerMinute;
            var duration = Duration.FromMinutes( (long)minutes );
            return duration == Duration.Zero ? Create( context ) : duration;
        }
    }
}
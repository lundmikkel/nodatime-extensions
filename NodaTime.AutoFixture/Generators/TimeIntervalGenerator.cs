using AutoFixture.Kernel;
using NodaTime;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="TimeInterval"/>.
    /// </summary>
    public class TimeIntervalGenerator : TypedSpecimenBuilder<TimeInterval>
    {
        private readonly LocalTimeGenerator _generator = new LocalTimeGenerator();

        public override TimeInterval Create( ISpecimenContext context )
        {
            var start = _generator.Create( context );
            var end = _generator.Create( context );

            return new TimeInterval(start, end);
        }
    }
}
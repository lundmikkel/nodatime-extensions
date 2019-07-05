using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="Interval"/> that is contained in the period from year 1900 to 2100.
    /// </summary>
    public class IntervalGenerator : TypedSpecimenBuilder<Interval>
    {
        private readonly InstantGenerator generator = new InstantGenerator();

        public override Interval Create( ISpecimenContext context )
        {
            var instant1 = generator.Create( context );
            var instant2 = generator.Create( context );

            return instant1 <= instant2
                ? new Interval( instant1, instant2 )
                : new Interval( instant2, instant1 );
        }
    }
}
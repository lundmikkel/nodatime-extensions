using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    public class IsoDayOfWeekGenerator : TypedSpecimenBuilder<IsoDayOfWeek>
    {
        private static readonly Random Random = new Random();

        public override IsoDayOfWeek Create( ISpecimenContext context ) => (IsoDayOfWeek)( Random.Next( 7 ) + 1 );
    }
}
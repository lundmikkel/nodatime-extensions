using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="AnnualDate"/>.
    /// </summary>
    public class AnnualDateGenerator : TypedSpecimenBuilder<AnnualDate>
    {
        private static readonly Random Random = new Random();

        protected static readonly int[] MaxDaysPerMonth = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        public override AnnualDate Create( ISpecimenContext context )
        {
            var month = Random.Next( 12 );
            var day = Random.Next( MaxDaysPerMonth[ month ] );

            return new AnnualDate( month + 1, day + 1 );
        }
    }
}
using System;
using AutoFixture.Kernel;
using NodaTime.Tzdb;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="DateTimeZone"/>.
    /// </summary>
    public class DateTimeZoneGenerator : TypedSpecimenBuilder<DateTimeZone>
    {
        private static readonly Random Random = new Random();
        private readonly IDateTimeZoneProvider TimeZoneProvider;

        /// <summary>
        /// Creates a <see cref="DateTimeZoneGenerator"/> using the given <see cref="IDateTimeZoneProvider"/>.
        /// </summary>
        /// <param name="timeZoneProvider">A time zone provider. If <c>null</c>, it defaults to the time zone provider from <see cref="DateTimeZoneProviderFactory.Create"/>.</param>
        public DateTimeZoneGenerator( IDateTimeZoneProvider timeZoneProvider = null )
        {
            TimeZoneProvider = timeZoneProvider ?? DateTimeZoneProviderFactory.Create();
        }

        public override DateTimeZone Create( ISpecimenContext context )
        {
            var index = Random.Next( TimeZoneProvider.Ids.Count );
            var id = TimeZoneProvider.Ids[ index ];
            return TimeZoneProvider[ id ];
        }
    }
}
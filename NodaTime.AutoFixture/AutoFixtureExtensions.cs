using AutoFixture;
using NodaTime.AutoFixture.Generators;

namespace NodaTime.AutoFixture
{
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Customizes an <see cref="IFixture"/> so it can generate sensible <see cref="NodaTime"/> types.
        /// </summary>
        /// <typeparam name="T">The type of the given <see cref="IFixture"/>.</typeparam>
        /// <param name="fixture">An <see cref="IFixture"/>.</param>
        /// <returns>The given fixture.</returns>
        public static T CustomizeForNodaTime<T>( this T fixture ) where T : IFixture
        {
            // Add generators
            fixture.Customizations.Insert( 0, new IsoDayOfWeekGenerator() );
            fixture.Customizations.Add( new AnnualDateGenerator() );
            fixture.Customizations.Add( new DateIntervalGenerator() );
            fixture.Customizations.Add( new DateTimeIntervalGenerator() );
            fixture.Customizations.Add( new DateTimeZoneGenerator() );
            fixture.Customizations.Add( new DurationGenerator() );
            fixture.Customizations.Add( new InstantGenerator() );
            fixture.Customizations.Add( new IntervalGenerator() );
            fixture.Customizations.Add( new LocalDateGenerator() );
            fixture.Customizations.Add( new LocalDateTimeGenerator() );
            fixture.Customizations.Add( new LocalTimeGenerator() );
            fixture.Customizations.Add( new OffsetGenerator() );
            fixture.Customizations.Add( new PeriodGenerator() );
            fixture.Customizations.Add( new TimeIntervalGenerator() );

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize( fixture );

            return fixture;
        }
    }
}
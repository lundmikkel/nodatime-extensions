using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.WebApi.ModelBinding
{
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Configures <seealso cref="HttpConfiguration"/> with everything required for working with NodaTime.
        /// </summary>
        /// <param name="config">An http configuration.</param>
        /// <param name="dateTimeZoneProvider">A date time zone provider.</param>
        public static void ConfigureForNodaTime( this HttpConfiguration config, IDateTimeZoneProvider dateTimeZoneProvider )
        {
            // Configure serialization for NodaTime
            config.Formatters.JsonFormatter.SerializerSettings.ConfigureForNodaTime( dateTimeZoneProvider );

            // Adds action value binder for NodaTime model binders
            config.Services.Replace( typeof( IActionValueBinder ), new ActionValueBinder( dateTimeZoneProvider ) );
        }

        public static JsonSerializerSettings ConfigureForNodaTime( this JsonSerializerSettings settings, IDateTimeZoneProvider dateTimeZoneProvider )
        {
            if ( settings == null )
            {
                throw new ArgumentNullException( nameof( settings ) );
            }

            // Add Planday-specific NodaTime converters
            settings.Converters.Add( new NodaPatternConverter<LocalTime>( Patterns.LocalTimePattern ) );
            settings.Converters.Add( new NodaPatternConverter<LocalDateTime>( Patterns.LocalDateTimePattern ) );
            settings.Converters.Add( new NodaPatternConverter<Duration>( Patterns.DurationPattern ) );
            settings.Converters.Add( new NodaPatternConverter<Instant>( Patterns.InstantPattern ) );
            settings.Converters.Add( NodaConverters.IsoIntervalConverter );
            settings.Converters.Add( new NodaPatternConverter<LocalDate>( Patterns.LocalDatePattern ) );
            settings.Converters.Add( new NodaPatternConverter<Offset>( Patterns.OffsetPattern ) );
            settings.Converters.Add( NodaConverters.CreateDateTimeZoneConverter( dateTimeZoneProvider ) );
            settings.Converters.Add( new NodaPatternConverter<Period>( Patterns.PeriodPattern ) );
            settings.Converters.Add( new NodaPatternConverter<OffsetDateTime>( Patterns.OffsetDateTimePattern ) );
            settings.Converters.Add( new NodaPatternConverter<ZonedDateTime>( Patterns.ZonedDateTimePattern ) );

            // Disable automatic conversion of anything that looks like a date and time to BCL types.
            settings.DateParseHandling = DateParseHandling.None;

            // return to allow fluent chaining if desired
            return settings;
        }
    }
}
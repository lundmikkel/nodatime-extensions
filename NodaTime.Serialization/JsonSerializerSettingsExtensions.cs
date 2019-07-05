using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization
{
    // TODO: Add an analyzer that ensures user's don't use NodaTime.Serialization.JsonNet
    public static class JsonSerializerSettingsExtensions
    {
        /// <summary>
        /// Sets up <see cref="Newtonsoft.Json"/> to work with NodaTime types.
        /// </summary>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> that the converters should be added to.</param>
        /// <param name="dateTimeZoneProvider">A date time zone provider to be used for binding date time zones.
        /// It is recommended to use <see cref="NodaTime.Tzdb.DateTimeZoneProviderFactory.Create"/>.</param>
        /// <returns>The <see cref="JsonSerializerSettings"/> provided in <paramref name="settings"/>.</returns>
        public static JsonSerializerSettings ConfigureForNodaTime(this JsonSerializerSettings settings, IDateTimeZoneProvider dateTimeZoneProvider)
        {
            if (settings == null)
            {
                throw new System.ArgumentNullException(nameof(settings));
            }
            
            // Add Planday-specific NodaTime converters
            settings.Converters.Add(new NodaPatternConverter<LocalTime>(Patterns.LocalTimePattern));
            settings.Converters.Add(new NodaPatternConverter<LocalDateTime>(Patterns.LocalDateTimePattern));
            settings.Converters.Add(new NodaPatternConverter<Duration>(Patterns.DurationPattern));
            settings.Converters.Add(new NodaPatternConverter<Instant>(Patterns.InstantPattern));
            settings.Converters.Add(NodaConverters.IsoIntervalConverter);
            settings.Converters.Add(new NodaPatternConverter<DateInterval>(Patterns.DateIntervalPattern));
            settings.Converters.Add(new NodaPatternConverter<DateTimeInterval>(Patterns.DateTimeIntervalPattern));
            settings.Converters.Add(new NodaPatternConverter<TimeInterval>(Patterns.TimeIntervalPattern));
            settings.Converters.Add(new NodaPatternConverter<LocalDate>(Patterns.LocalDatePattern));
            settings.Converters.Add(new NodaPatternConverter<Offset>(Patterns.OffsetPattern));
            settings.Converters.Add(new NodaPatternConverter<DateTimeZone>(Patterns.GetDateTimeZonePattern(dateTimeZoneProvider)));
            settings.Converters.Add(new NodaPatternConverter<Period>(Patterns.PeriodPattern));
            settings.Converters.Add(new NodaPatternConverter<OffsetDateTime>(Patterns.OffsetDateTimePattern));
            settings.Converters.Add(new NodaPatternConverter<ZonedDateTime>(Patterns.ZonedDateTimePattern));

            // Disable automatic conversion of anything that looks like a date and time to BCL types.
            settings.DateParseHandling = DateParseHandling.None;

            // return to allow fluent chaining if desired
            return settings;
        }
    }
}
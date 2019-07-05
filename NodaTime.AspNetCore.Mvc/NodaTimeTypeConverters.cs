using System.ComponentModel;
using NodaTime.AspNetCore.Mvc.TypeConverters;
using NodaTime;
using NodaTime.Tzdb;

namespace NodaTime.AspNetCore.Mvc
{
    public static class NodaTimeTypeConverters
    {
        /// <summary>
        /// Adds type converters for NodaTime types.
        /// </summary>
        /// <param name="dateTimeZoneProvider">A date time zone provider to be used for binding date time zones.
        /// It is recommended to use <see cref="DateTimeZoneProviderFactory.Create"/>.</param>
        /// <seealso cref="MvcOptionsExtensions.ConfigureForNodaTime"/>
        public static void Setup(IDateTimeZoneProvider dateTimeZoneProvider)
        {
            TypeDescriptor.AddAttributes(typeof(LocalDateTime), new TypeConverterAttribute(typeof(LocalDateTimeConverter)));
            TypeDescriptor.AddAttributes(typeof(LocalDate), new TypeConverterAttribute(typeof(LocalDateConverter)));
            TypeDescriptor.AddAttributes(typeof(LocalTime), new TypeConverterAttribute(typeof(LocalTimeConverter)));
            TypeDescriptor.AddAttributes(typeof(Instant), new TypeConverterAttribute(typeof(InstantConverter)));
            TypeDescriptor.AddAttributes(typeof(OffsetDateTime), new TypeConverterAttribute(typeof(OffsetDateTimeConverter)));
            TypeDescriptor.AddAttributes(typeof(ZonedDateTime), new TypeConverterAttribute(typeof(ZonedDateTimeConverter)));
            TypeDescriptor.AddAttributes(typeof(Offset), new TypeConverterAttribute(typeof(OffsetConverter)));
            TypeDescriptor.AddAttributes(typeof(Period), new TypeConverterAttribute(typeof(PeriodConverter)));
            TypeDescriptor.AddAttributes(typeof(Duration), new TypeConverterAttribute(typeof(DurationConverter)));
            TypeDescriptor.AddAttributes(typeof(Interval), new TypeConverterAttribute(typeof(IntervalConverter)));
            TypeDescriptor.AddAttributes(typeof(DateInterval), new TypeConverterAttribute(typeof(DateIntervalConverter)));
            TypeDescriptor.AddAttributes(typeof(DateTimeInterval), new TypeConverterAttribute(typeof(DateTimeIntervalConverter)));
            TypeDescriptor.AddAttributes(typeof(TimeInterval), new TypeConverterAttribute(typeof(TimeIntervalConverter)));
            TypeDescriptor.AddAttributes(typeof(DateTimeZone), new TypeConverterAttribute(typeof(DateTimeZoneConverter)));
        }
    }
}
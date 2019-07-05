using Microsoft.AspNetCore.Mvc;
using NodaTime.Tzdb;

namespace NodaTime.AspNetCore.Mvc
{
    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// Configures MVC model binding and validation for NodaTime types.
        /// </summary>
        /// <param name="options">MVC options.</param>
        /// <param name="dateTimeZoneProvider">A date time zone provider to be used for binding date time zones.
        /// It is recommended to use <see cref="DateTimeZoneProviderFactory.Create"/>.</param>
        /// <returns>The <see cref="MvcOptions"/> provided in <paramref name="options"/>.</returns>
        public static MvcOptions ConfigureForNodaTime(this MvcOptions options, IDateTimeZoneProvider dateTimeZoneProvider)
        {
            NodaTimeTypeConverters.Setup(dateTimeZoneProvider);

            options.Filters.Add(new CheckModelStateAttribute());

            return options;
        }
    }
}
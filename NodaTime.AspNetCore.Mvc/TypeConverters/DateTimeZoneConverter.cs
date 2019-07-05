using NodaTime.Tzdb;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class DateTimeZoneConverter : NodaTimeConverter<DateTimeZone>
    {
        private readonly IDateTimeZoneProvider _dateTimeZoneProvider = DateTimeZoneProviderFactory.Create();
        
        public override ParseResult<DateTimeZone> Parse(string value) => ParseResult<DateTimeZone>.ForValue(_dateTimeZoneProvider.GetZoneOrNull(value));
    }
}
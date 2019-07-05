using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class ZonedDateTimeConverter : NodaTimeConverter<ZonedDateTime>
    {
        public override ParseResult<ZonedDateTime> Parse(string value) => Patterns.ZonedDateTimePattern.Parse(value);
    }
}
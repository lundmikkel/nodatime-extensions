using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class LocalDateTimeConverter : NodaTimeConverter<LocalDateTime>
    {
        public override ParseResult<LocalDateTime> Parse(string value) => Patterns.LocalDateTimePattern.Parse(value);
    }
}
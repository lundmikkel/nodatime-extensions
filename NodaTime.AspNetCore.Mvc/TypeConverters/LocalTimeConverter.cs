using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class LocalTimeConverter : NodaTimeConverter<LocalTime>
    {
        public override ParseResult<LocalTime> Parse(string value) => Patterns.LocalTimePattern.Parse(value);
    }
}
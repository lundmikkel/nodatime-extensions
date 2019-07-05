using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class LocalDateConverter : NodaTimeConverter<LocalDate>
    {
        public override ParseResult<LocalDate> Parse(string value) => Patterns.LocalDatePattern.Parse(value);
    }
}
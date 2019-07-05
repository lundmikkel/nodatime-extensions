using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class OffsetDateTimeConverter : NodaTimeConverter<OffsetDateTime>
    {
        public override ParseResult<OffsetDateTime> Parse(string value) => Patterns.OffsetDateTimePattern.Parse(value);
    }
}
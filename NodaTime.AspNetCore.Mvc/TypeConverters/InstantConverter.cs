using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class InstantConverter : NodaTimeConverter<Instant>
    {
        public override ParseResult<Instant> Parse(string value) => Patterns.InstantPattern.Parse(value);
    }
}
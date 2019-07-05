using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class DurationConverter : NodaTimeConverter<Duration>
    {
        public override ParseResult<Duration> Parse(string value) => Patterns.DurationPattern.Parse(value);
    }
}
using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class TimeIntervalConverter : NodaTimeConverter<TimeInterval>
    {
        public override ParseResult<TimeInterval> Parse(string value) => Patterns.TimeIntervalPattern.Parse(value);
    }
}
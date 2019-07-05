using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class IntervalConverter : NodaTimeConverter<Interval>
    {
        public override ParseResult<Interval> Parse(string value) => Patterns.IntervalPattern.Parse(value);
    }
}
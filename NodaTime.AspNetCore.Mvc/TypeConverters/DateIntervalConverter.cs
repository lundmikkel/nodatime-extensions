using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class DateIntervalConverter : NodaTimeConverter<DateInterval>
    {
        public override ParseResult<DateInterval> Parse(string value) => Patterns.DateIntervalPattern.Parse(value);
    }
}
using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class DateTimeIntervalConverter : NodaTimeConverter<DateTimeInterval>
    {
        public override ParseResult<DateTimeInterval> Parse(string value) => Patterns.DateTimeIntervalPattern.Parse(value);
    }
}
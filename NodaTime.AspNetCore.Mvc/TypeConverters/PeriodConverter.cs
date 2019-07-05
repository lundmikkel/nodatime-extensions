using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class PeriodConverter : NodaTimeConverter<Period>
    {
        public override ParseResult<Period> Parse(string value) => Patterns.PeriodPattern.Parse(value);
    }
}
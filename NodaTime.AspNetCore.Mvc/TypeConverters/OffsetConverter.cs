using NodaTime;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal class OffsetConverter : NodaTimeConverter<Offset>
    {
        public override ParseResult<Offset> Parse(string value) => Patterns.OffsetPattern.Parse(value);
    }
}
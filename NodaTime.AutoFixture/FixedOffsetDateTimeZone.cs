using NodaTime.TimeZones;

namespace NodaTime.AutoFixture
{
    public class FixedOffsetDateTimeZone : DateTimeZone
    {
        public FixedOffsetDateTimeZone(Offset offset) : base(offset.ToString(), true, offset, offset)
        { }

        public FixedOffsetDateTimeZone(string id, Offset offset) : base(id, true, offset, offset)
        { }

        public override ZoneInterval GetZoneInterval(Instant instant) => new ZoneInterval(Offset.ToString(), null, null, Offset, Offset);

        public Offset Offset => MinOffset; // Same as MaxOffset
    }
}
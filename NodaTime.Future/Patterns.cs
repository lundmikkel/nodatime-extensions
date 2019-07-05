using System;
using System.Text;
using NodaTime.Tzdb;
using NodaTime.Utility;

namespace NodaTime
{
    using NodaTimeText = global::NodaTime.Text; // Alias because current namespace is also NodaTime

    public static class Patterns
    {
        private static NodaTimeText.IPattern<Duration> DurationMinimalExtendedPattern { get; } = new NodaTimeText.CompositePatternBuilder<Duration>
            {
                { NodaTimeText.DurationPattern.Roundtrip, _ => true },
                { NodaTimeText.DurationPattern.CreateWithInvariantCulture("-D':'hh':'mm"), d => d.Seconds == 0 && d.SubsecondTicks == 0 } // Historically the default Planday pattern
            }
            .Build();

        private static NodaTimeText.IPattern<LocalTime> LocalTimeMinimalExtendedPattern { get; } = new NodaTimeText.CompositePatternBuilder<LocalTime>
            {
                { NodaTimeText.LocalTimePattern.ExtendedIso, _ => true }, // Extended ISO format
                { NodaTimeText.LocalTimePattern.CreateWithInvariantCulture(@"HH"), _ => false }, // Fallback parsing format - never used for formatting
                { NodaTimeText.LocalTimePattern.CreateWithInvariantCulture(@"HH':'mm"), t => t.Second == 0 && t.TickOfSecond == decimal.Zero } // Default Planday pattern
            }
            .Build();

        private static NodaTimeText.IPattern<LocalDateTime> LocalDateTimeMinimalExtendedPattern { get; } = new NodaTimeText.CompositePatternBuilder<LocalDateTime>
            {
                { NodaTimeText.LocalDateTimePattern.ExtendedIso, _ => true },
                { NodaTimeText.LocalDateTimePattern.CreateWithInvariantCulture(@"uuuu'-'MM'-'dd'T'HH"), _ => false }, // Fallback parsing format - never used for formatting
                { NodaTimeText.LocalDateTimePattern.CreateWithInvariantCulture(@"uuuu'-'MM'-'dd'T'HH':'mm"), t => t.Second == 0 && t.TickOfSecond == decimal.Zero } // Default Planday pattern
            }
            .Build();

        private static NodaTimeText.IPattern<OffsetDateTime> OffsetDateTimeMinimalExtendedPattern { get; } = new NodaTimeText.CompositePatternBuilder<OffsetDateTime>
            {
                { NodaTimeText.OffsetDateTimePattern.ExtendedIso, _ => true },
                { NodaTimeText.OffsetDateTimePattern.CreateWithInvariantCulture(@"uuuu'-'MM'-'dd'T'HH':'mmo<G>"), t => t.LocalDateTime.TimeOfDay.Second == 0 &&
                                                                                                                       t.LocalDateTime.TimeOfDay.TickOfSecond == decimal.Zero }
            }
            .Build();

        private static NodaTimeText.IPattern<ZonedDateTime> ZonedDateTimeMinimalExtendedPattern { get; } = new NodaTimeText.CompositePatternBuilder<ZonedDateTime>
            {
                { CreateZonedDateTimePattern(NodaTimeText.ZonedDateTimePattern.ExtendedFormatOnlyIso.PatternText), _ => true },
                { CreateZonedDateTimePattern(@"uuuu'-'MM'-'dd'T'HH':'mm z '('o<g>')'"), t => t.LocalDateTime.TimeOfDay.Second == 0 &&
                                                                                             t.LocalDateTime.TimeOfDay.TickOfSecond == decimal.Zero }
            }
            .Build();

        private static NodaTimeText.ZonedDateTimePattern CreateZonedDateTimePattern(string patternText) =>
            NodaTimeText.ZonedDateTimePattern.CreateWithInvariantCulture(
                patternText,
                DateTimeZoneProviderFactory.Create()
            );

        /// <summary>
        /// Pattern for <see cref="Duration"/>
        /// </summary>
        public static NodaTimeText.IPattern<Duration> DurationPattern { get; } = DurationMinimalExtendedPattern;

        /// <summary>
        /// Pattern for <see cref="Offset"/>
        /// </summary>
        public static NodaTimeText.IPattern<Offset> OffsetPattern { get; } = NodaTimeText.OffsetPattern.GeneralInvariant;

        /// <summary>
        /// Pattern for <see cref="Instant"/>
        /// </summary>
        public static NodaTimeText.IPattern<Instant> InstantPattern { get; } = NodaTimeText.InstantPattern.ExtendedIso;

        /// <summary>
        /// Pattern for <see cref="Interval"/>
        /// </summary>
        public static NodaTimeText.IPattern<Interval> IntervalPattern { get; } = new IsoIntervalPattern();

        private class IsoIntervalPattern : NodaTimeText.IPattern<Interval>
        {
            public NodaTimeText.ParseResult<Interval> Parse(string text)
            {
                var slash = text.IndexOf('/');
                if (slash == -1)
                {
                    throw new InvalidNodaDataException("Expected ISO-8601-formatted interval; slash was missing.");
                }

                var startText = text.Substring(0, slash);
                var endText = text.Substring(slash + 1);

                var startInstant = default(Instant?);
                var endInstant = default(Instant?);

                if (!string.IsNullOrEmpty(startText))
                {
                    var result = Patterns.InstantPattern.Parse(startText);

                    if (!result.Success)
                    {
                        return NodaTimeText.ParseResult<Interval>.ForException(() => result.Exception);
                    }

                    startInstant = result.Value;
                }

                if (!string.IsNullOrEmpty(endText))
                {
                    var result = InstantPattern.Parse(endText);

                    if (!result.Success)
                    {
                        return NodaTimeText.ParseResult<Interval>.ForException(() => result.Exception);
                    }

                    endInstant = result.Value;
                }

                return NodaTimeText.ParseResult<Interval>.ForValue(new Interval(startInstant, endInstant));
            }

            public string Format(Interval value)
            {
                var start = value.HasStart ? InstantPattern.Format(value.Start) : string.Empty;
                var end = value.HasEnd ? InstantPattern.Format(value.End) : string.Empty;
                return $"{start}/{end}";
            }

            public StringBuilder AppendFormat(Interval value, StringBuilder builder) => builder.Append(Format(value));
        }

        /// <summary>
        /// Pattern for <see cref="LocalDate"/>
        /// </summary>
        public static NodaTimeText.IPattern<LocalDate> LocalDatePattern { get; } = NodaTimeText.LocalDatePattern.Iso;

        /// <summary>
        /// Pattern for <see cref="DateInterval"/>
        /// </summary>
        public static NodaTimeText.IPattern<DateInterval> DateIntervalPattern { get; } = new GenericIsoIntervalPattern<DateInterval, LocalDate>(
            LocalDatePattern,
            i => i.Start,
            i => i.End,
            (start, end) => new DateInterval(start, end)
        );

        /// <summary>
        /// Pattern for <see cref="LocalTime"/>
        /// </summary>
        public static NodaTimeText.IPattern<LocalTime> LocalTimePattern { get; } = LocalTimeMinimalExtendedPattern;

        /// <summary>
        /// Pattern for <see cref="TimeInterval"/>
        /// </summary>
        public static NodaTimeText.IPattern<TimeInterval> TimeIntervalPattern { get; } = new GenericIsoIntervalPattern<TimeInterval, LocalTime>(
            LocalTimePattern,
            i => i.Start,
            i => i.End,
            (start, end) => new TimeInterval(start, end)
        );

        /// <summary>
        /// Pattern for <see cref="LocalDateTime"/>
        /// </summary>
        public static NodaTimeText.IPattern<LocalDateTime> LocalDateTimePattern { get; } = LocalDateTimeMinimalExtendedPattern;

        /// <summary>
        /// Pattern for <see cref="DateTimeInterval"/>
        /// </summary>
        public static NodaTimeText.IPattern<DateTimeInterval> DateTimeIntervalPattern { get; } = new GenericIsoIntervalPattern<DateTimeInterval, LocalDateTime>(
            LocalDateTimePattern,
            i => i.Start,
            i => i.End,
            (start, end) => new DateTimeInterval(start, end)
        );

        /// <summary>
        /// Pattern for <see cref="OffsetDateTime"/>
        /// </summary>
        public static NodaTimeText.IPattern<OffsetDateTime> OffsetDateTimePattern { get; } = OffsetDateTimeMinimalExtendedPattern;

        /// <summary>
        /// Pattern for <see cref="ZonedDateTime"/>
        /// </summary>
        public static NodaTimeText.IPattern<ZonedDateTime> ZonedDateTimePattern { get; } = ZonedDateTimeMinimalExtendedPattern;

        /// <summary>
        /// Pattern for <see cref="Period"/>
        /// </summary>
        public static NodaTimeText.IPattern<Period> PeriodPattern { get; } = NodaTimeText.PeriodPattern.Roundtrip;

        /// <summary>
        /// Pattern for <see cref="Period"/>
        /// </summary>
        public static NodaTimeText.IPattern<DateTimeZone> GetDateTimeZonePattern(IDateTimeZoneProvider dateTimeZoneProvider) => new TimeZonePattern(dateTimeZoneProvider);

        private class TimeZonePattern : NodaTimeText.IPattern<DateTimeZone>
        {
            private readonly IDateTimeZoneProvider _dateTimeZoneProvider;

            public TimeZonePattern(IDateTimeZoneProvider dateTimeZoneProvider)
            {
                _dateTimeZoneProvider = dateTimeZoneProvider;
            }

            public NodaTimeText.ParseResult<DateTimeZone> Parse(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return NodaTimeText.ParseResult<DateTimeZone>.ForException(() => new ArgumentException("Time zone identifier is empty or null"));
                }

                var timeZone = _dateTimeZoneProvider.GetZoneOrNull(text);

                if (timeZone == null)
                {
                    return NodaTimeText.ParseResult<DateTimeZone>.ForException(() => new ArgumentException("Time zone identifier unknown"));
                }

                return NodaTimeText.ParseResult<DateTimeZone>.ForValue(timeZone);
            }

            public string Format(DateTimeZone value) => value?.Id ?? throw new ArgumentNullException(nameof(value)); // TODO: Is this the right way to handle null values?

            public StringBuilder AppendFormat(DateTimeZone value, StringBuilder builder) => builder.Append(Format(value));
        }

        private class GenericIsoIntervalPattern<TInterval, TEndpoint> : NodaTimeText.IPattern<TInterval>
        {
            private readonly NodaTimeText.IPattern<TEndpoint> _pattern;
            private readonly Func<TInterval, TEndpoint> _getStart;
            private readonly Func<TInterval, TEndpoint> _getEnd;
            private readonly Func<TEndpoint, TEndpoint, TInterval> _createInterval;

            public GenericIsoIntervalPattern(
                NodaTimeText.IPattern<TEndpoint> pattern,
                Func<TInterval, TEndpoint> getStart,
                Func<TInterval, TEndpoint> getEnd,
                Func<TEndpoint, TEndpoint, TInterval> createInterval
            )
            {
                _pattern = pattern;
                _getStart = getStart;
                _getEnd = getEnd;
                _createInterval = createInterval;
            }

            public NodaTimeText.ParseResult<TInterval> Parse(string text)
            {
                var slash = text.IndexOf('/');
                if (slash == -1)
                {
                    return NodaTimeText.ParseResult<TInterval>.ForException(() => new InvalidNodaDataException("Expected ISO-8601-formatted interval; slash was missing."));
                }

                var startText = text.Substring(0, slash);
                var startParsingResult = _pattern.Parse(startText);

                var endText = text.Substring(slash + 1);
                var endParsingResult = _pattern.Parse(endText);

                if (!startParsingResult.Success)
                {
                    return NodaTimeText.ParseResult<TInterval>.ForException(() => startParsingResult.Exception);
                }
                if (!endParsingResult.Success)
                {
                    return NodaTimeText.ParseResult<TInterval>.ForException(() => endParsingResult.Exception);
                }

                var start = startParsingResult.Value;
                var end = endParsingResult.Value;

                return NodaTimeText.ParseResult<TInterval>.ForValue(_createInterval(start, end));
            }

            public string Format(TInterval value) => $"{_pattern.Format(_getStart(value))}/{_pattern.Format(_getEnd(value))}";

            public StringBuilder AppendFormat(TInterval value, StringBuilder builder) => builder.Append(Format(value));
        }
    }
}
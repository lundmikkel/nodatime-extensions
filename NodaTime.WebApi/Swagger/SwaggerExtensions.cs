using System;
using Newtonsoft.Json;
using NodaTime.Tzdb;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

namespace NodaTime.WebApi.Swagger
{
    public static class SwaggerExtensions
    {
        public static SwaggerDocsConfig ConfigureForNodaTime(this SwaggerDocsConfig config, IDateTimeZoneProvider timeZoneProvider)
        {
            var timeZone = timeZoneProvider[America.NewYork];
            var instant = SystemClock.Instance.GetCurrentInstant();
            var zonedDateTime = instant.InZone(timeZone);
            var localDate = zonedDateTime.Date;
            var localTime = zonedDateTime.TimeOfDay;
            var localDateTime = zonedDateTime.LocalDateTime;
            var offsetDateTime = zonedDateTime.ToOffsetDateTime();
            var duration = Duration.FromMilliseconds(182713784L);
            var interval = new Interval(instant, instant + duration);
            var offset = timeZone.GetUtcOffset(instant);
            var period = Period.Between(localDateTime, localDateTime.PlusTicks(duration.BclCompatibleTicks));

            config.MapStruct(instant);
            config.MapStruct(localDate);
            config.MapStruct(localTime);
            config.MapStruct(localDateTime);
            config.MapStruct(offsetDateTime);
            config.MapStruct(zonedDateTime);
            config.MapStruct(interval);
            config.MapStruct(offset);
            config.MapClass(period);
            config.MapStruct(duration);
            config.MapClass(timeZone);

            return config;
        }

        private static void MapStruct<T>(this SwaggerDocsConfig config, T example) where T : struct
        {
            var factory = GetSchema(example);

            config.MapType<T>(factory);
            config.MapType<T?>(factory);
        }

        private static void MapClass<T>(this SwaggerDocsConfig config, T example) where T : class => config.MapType<T>(GetSchema(example));

        private static Func<Schema> GetSchema<T>(T example) => () => new Schema
        {
            type = "string",
            example = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(example))
        };
    }
}
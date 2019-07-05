using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Tzdb;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NodaTime.AspNetCore.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection ConfigureSwaggerForNodaTime(this IServiceCollection services, IDateTimeZoneProvider dateTimeZoneProvider)
        {
            services.AddSwaggerGen(c => c.ConfigureForNodaTime(dateTimeZoneProvider));
            return services;
        }

        public static void ConfigureForNodaTime(this SwaggerGenOptions config, IDateTimeZoneProvider dateTimeZoneProvider)
        {
            var dateTimeZone = dateTimeZoneProvider[America.NewYork];
            var instant = SystemClock.Instance.GetCurrentInstant();
            var interval = new Interval(instant, instant + Duration.FromMilliseconds(182713784L));
            var dateTimeInterval = interval.InZoneStrictly(dateTimeZone);

            var zonedDateTime = dateTimeZone.AtLeniently(instant.InZone(dateTimeZone).LocalDateTime.With(TimeAdjusters.TruncateToMinute));

            var localDateTime = zonedDateTime.LocalDateTime;
            var localDate = localDateTime.Date;
            var dateInterval = localDateTime.Date.GetContainingWeekInterval(IsoDayOfWeek.Monday);
            var localTime = localDateTime.TimeOfDay;
            var timeInterval = new TimeInterval(localTime, localTime + Period.FromHours(3) + Period.FromMinutes(45));
            var offsetDateTime = zonedDateTime.ToOffsetDateTime();
            var duration = Duration.FromDays(2) + Duration.FromHours(3) + Duration.FromMinutes(45);
            var offset = dateTimeZone.GetUtcOffset(instant);
            var period = Period.FromTicks(duration.BclCompatibleTicks);
            
            config.MapStruct(instant);
            config.MapStruct(localDate);
            config.MapStruct(localTime);
            config.MapStruct(localDateTime);
            config.MapStruct(offsetDateTime);
            config.MapStruct(zonedDateTime);
            config.MapStruct(interval);
            config.MapClass(dateInterval);
            config.MapClass(dateTimeInterval);
            config.MapClass(timeInterval);
            config.MapStruct(offset);
            config.MapClass(period);
            config.MapStruct(duration);
            config.MapClass(dateTimeZone);
        }

        private static void MapStruct<T>(this SwaggerGenOptions config, T example) where T : struct
        {
            var factory = GetSchema(example);

            config.MapType<T>(factory);
            config.MapType<T?>(factory);
        }

        private static void MapClass<T>(this SwaggerGenOptions config, T example) where T : class => config.MapType<T>(GetSchema(example));

        private static Func<Schema> GetSchema<T>(T example) => () => new Schema
        {
            Type = "string",
            Example = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(example))
        };
    }
}
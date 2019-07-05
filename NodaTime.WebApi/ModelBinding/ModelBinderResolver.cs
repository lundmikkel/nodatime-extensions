using System;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;
using NodaTime;

namespace NodaTime.WebApi.ModelBinding
{
    internal class ModelBinderResolver
    {
        private readonly Dictionary<Type, IModelBinder> Dictionary;

        public ModelBinderResolver( IDateTimeZoneProvider dateTimeZoneProvider )
        {
            Dictionary = new Dictionary<Type, IModelBinder>
            {
                { typeof( DateTimeZone ), new DateTimeZoneModelBinder( dateTimeZoneProvider ) },
                { typeof( Duration ), new ModelBinder<Duration>(Patterns.DurationPattern )},
                { typeof( Instant ), new ModelBinder<Instant>(Patterns.InstantPattern )},
                { typeof( Interval ), new IsoIntervalModelBinder( Patterns.InstantPattern ) },
                { typeof( LocalDate ), new ModelBinder<LocalDate>(Patterns.LocalDatePattern )},
                { typeof( LocalDateTime ), new ModelBinder<LocalDateTime>(Patterns.LocalDateTimePattern )},
                { typeof( LocalTime ), new ModelBinder<LocalTime>(Patterns.LocalTimePattern )},
                { typeof( Offset ), new ModelBinder<Offset>(Patterns.OffsetPattern )},
                { typeof( OffsetDateTime ), new ModelBinder<OffsetDateTime>(Patterns.OffsetDateTimePattern )},
                { typeof( Period ), new ModelBinder<Period>(Patterns.PeriodPattern )},
                { typeof( ZonedDateTime ), new ModelBinder<ZonedDateTime>(Patterns.ZonedDateTimePattern)}
            };
        }

        public IModelBinder GetModelBinder( Type type )
        {
            Dictionary.TryGetValue( type, out IModelBinder value );
            return value;
        }
    }
}
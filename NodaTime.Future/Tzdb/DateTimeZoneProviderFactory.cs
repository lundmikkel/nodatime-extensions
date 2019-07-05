using System;

namespace NodaTime.Tzdb
{
    public static class DateTimeZoneProviderFactory
    {
        private static readonly Lazy<IDateTimeZoneProvider> Lazy = new Lazy<IDateTimeZoneProvider>( GetDateTimeZoneProvider );

        public static IDateTimeZoneProvider Create() => Lazy.Value;

        private static IDateTimeZoneProvider GetDateTimeZoneProvider()
        {
            return DateTimeZoneProviders.Tzdb;

            // TODO: Figure out a good way to do this. The rest of the application should handle this as well.
            //try
            //{
            //    using ( var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( @"NodaTime.Tzdb.NodaZoneData.tzdb2016j.nzd" ) )
            //    {
            //        return new DateTimeZoneCache( TzdbDateTimeZoneSource.FromStream( stream ) );
            //    }
            //}
            //catch
            //{
            //    return DateTimeZoneProviders.Tzdb;
            //}
        }
    }
}
using System.IO;
using System.Net;

namespace NodaTime.Tzdb
{
    public class OnlineTzdbStreamProvider : ITzdbStreamProvider
    {
        private const string TzdbLatestUrl = @"http://nodatime.org/tzdb/latest.txt";

        public Stream GetStream()
        {
            // TODO: Catch various exceptions!

            // Get latest version from NodaTime
            string tzdbLatestVersionUrl;
            using ( var client = new WebClient() )
            {
                tzdbLatestVersionUrl = client.DownloadString( TzdbLatestUrl ).Trim();
            }

            // Check if the file already exists locally, otherwise download it
            var folderPath = Path.Combine( Path.GetTempPath(), "tzdb" );
            var nzdPath = Path.Combine( folderPath, Path.GetFileName( tzdbLatestVersionUrl ) );

            if ( !File.Exists( nzdPath ) )
            {
                if ( !Directory.Exists( folderPath ) )
                {
                    Directory.CreateDirectory( folderPath );
                }

                using ( var client = new WebClient() )
                {
                    client.DownloadFile( tzdbLatestVersionUrl, nzdPath );
                }
            }

            return new FileStream( nzdPath, FileMode.Open );
        }
    }
}
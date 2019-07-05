using System.IO;

namespace NodaTime.Tzdb
{
    public interface ITzdbStreamProvider
    {
        Stream GetStream();
    }
}
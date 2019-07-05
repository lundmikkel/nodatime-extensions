namespace NodaTime.Extensions
{
    public static class OffsetExtensions
    {
        /// <summary>
        /// Serializes the <see cref="Offset"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="offset">An offset.</param>
        /// <returns>A machine-readable string representation of <paramref name="offset"/>.</returns>
        public static string ToStandardString( this Offset offset ) => Patterns.OffsetPattern.Format(offset);
    }
}
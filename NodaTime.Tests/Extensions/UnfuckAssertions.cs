using FluentAssertions;
using FluentAssertions.Primitives;

namespace NodaTime.Tests.Extensions
{
    public static class UnfuckAssertions
    {
        public static ObjectAssertions Should(this DateInterval dateInterval) => ((object)dateInterval).Should();
    }
}
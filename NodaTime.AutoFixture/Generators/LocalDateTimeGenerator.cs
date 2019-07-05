using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// Generates a random <see cref="LocalDateTime"/> from a random <see cref="LocalDate"/> and <see cref="LocalTime"/>.
    /// </summary>
    public class LocalDateTimeGenerator : TypedSpecimenBuilder<LocalDateTime>
    {
        private static readonly LocalTimeGenerator LocalTimeGenerator = new LocalTimeGenerator();
        private static readonly LocalDateGenerator LocalDateGenerator = new LocalDateGenerator();

        public override LocalDateTime Create( ISpecimenContext context ) => LocalDateGenerator.Create( context ).At( LocalTimeGenerator.Create( context ) );
    }
}
using System;
using AutoFixture.Kernel;

namespace NodaTime.AutoFixture.Generators
{
    /// <summary>
    /// An typed abstract implementation of <see cref="T:Ploeh.AutoFixture.Kernel.ISpecimenBuilder" /> that handles common preconditions for <see cref="M:Ploeh.AutoFixture.Kernel.ISpecimenBuilder.Create(System.Object,Ploeh.AutoFixture.Kernel.ISpecimenContext)" />.
    /// </summary>
    /// <typeparam name="T">The type of the object that the specimen builder should be able to create.</typeparam>
    public abstract class TypedSpecimenBuilder<T> : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var type = typeof(T);

            if (!type.Equals(request))
            {
                return new NoSpecimen();
            }

            return Create(context);
        }

        /// <summary>
        /// Creates an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="context">A context that can be used to create other specimens.</param>
        /// <returns>An instance of type <typeparamref name="T"/>.</returns>
        public abstract T Create(ISpecimenContext context);
    }
}
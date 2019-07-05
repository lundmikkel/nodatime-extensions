using System;
using System.Collections.Generic;
using AutoFixture;

namespace NodaTime.Tests.AutoFixture
{
    public static class AutoFixtureExtensions
    {
        private static readonly HashSet<Func<IFixture, IFixture>> Registrations = new HashSet<Func<IFixture, IFixture>>();

        public static void AddCustomization(Func<IFixture, IFixture> registration) => Registrations.Add(registration);

        public static T Create<T>(this IFixture fixture, Func<T, bool> condition)
        {
            T value;

            do
            {
                value = fixture.Create<T>();
            } while (!condition(value));

            return value;
        }

        public static (TFirst, TSecond) Create<TFirst, TSecond>(this IFixture fixture, Func<TFirst, TSecond, bool> condition)
        {
            TFirst value1;
            TSecond value2;

            do
            {
                value1 = fixture.Create<TFirst>();
                value2 = fixture.Create<TSecond>();
            } while (!condition(value1, value2));

            return (value1, value2);
        }

        public static (T, T) Create<T>(this IFixture fixture, Func<T, T, bool> condition) => fixture.Create<T, T>(condition);
    }
}
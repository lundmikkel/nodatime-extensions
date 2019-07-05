using System;
using System.Collections.Generic;

namespace NodaTime.Extensions
{
    public static class DurationExtensions
    {
        /// <summary>
        /// Serializes the <see cref="Duration"/> using standard machine-to-machine format usable in JSON, URIs, and more.
        /// </summary>
        /// <param name="duration">A duration.</param>
        /// <returns>A machine-readable string representation of <paramref name="duration"/>.</returns>
        public static string ToStandardString( this Duration duration ) => Patterns.DurationPattern.Format(duration);

        /// <summary>
        /// Computes the sum of a sequence of <see cref="Duration"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Duration"/> values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        /// <remarks>This method returns <see cref="Duration.Zero"/> if source contains no elements.</remarks>
        public static Duration Sum( this IEnumerable<Duration> source )
        {
            if ( source == null )
            {
                throw new ArgumentNullException( nameof( source ) );
            }

            var total = Duration.Zero;

            foreach (var duration in source)
            {
                total += duration;
            }

            return total;
        }
        
        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="Duration"/> values.
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="Duration"/> values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        /// <remarks>This method returns <see cref="Duration.Zero"/> if source contains no elements.</remarks>
        public static Duration? Sum( this IEnumerable<Duration?> source )
        {
            if ( source == null )
            {
                throw new ArgumentNullException( nameof( source ) );
            }

            var total = Duration.Zero;

            foreach ( var duration in source )
            {
                total += duration.GetValueOrDefault();
            }

            return total;
        }

        /// <summary>
        /// Computes the sum of the sequence of <see cref="Duration"/> values that are obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Duration"/> values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is <c>null</c>.</exception>
        /// <remarks>
        /// <para>This method returns <see cref="Duration.Zero"/> if source contains no elements.</para>
        /// <para>You can apply this method to a sequence of arbitrary values if you provide a function, selector, that projects the members of source into <see cref="Duration"/>s.</para>
        /// </remarks>
        public static Duration Sum<T>( this IEnumerable<T> source, Func<T, Duration> selector)
        {
            if ( source == null )
            {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( selector == null )
            {
                throw new ArgumentNullException( nameof( source ) );
            }

            var total = Duration.Zero;

            foreach ( var element in source )
            {
                total += selector(element);
            }

            return total;
        }

        /// <summary>
        /// Returns the local duration between two local times.
        /// </summary>
        /// <param name="start">The start time.</param>
        /// <param name="end">The end time.</param>
        /// <returns></returns>
        public static Duration LocalDuration(LocalTime start, LocalTime end)
        {
            throw new NotImplementedException();
        }
    }
}
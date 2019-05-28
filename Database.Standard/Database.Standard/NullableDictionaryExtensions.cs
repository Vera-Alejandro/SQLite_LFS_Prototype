using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Interstates.Control.Database
{
    /// <summary>
    /// Extension methods for the NullableDictionary.
    /// </summary>
    static class NullableDictionaryExtensions
    {
        /// <summary>
        /// Convert to a NullableDictionary.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static NullableDictionary<TKey, TSource> ToNullableDictionary<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.ToNullableDictionary<TSource, TKey, TSource>(keySelector, s => s);
        }

        /// <summary>
        /// Convert to a NullableDictionary.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        public static NullableDictionary<TKey, TElement> ToNullableDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return source.ToNullableDictionary<TSource, TKey, TElement>(keySelector, elementSelector, null);
        }

        /// <summary>
        /// Convert to a NullableDictionary.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static NullableDictionary<TKey, TElement> ToNullableDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            var dictionary = new NullableDictionary<TKey, TElement>(comparer);

            foreach (var local in source) dictionary.Add(keySelector(local), elementSelector(local));

            return dictionary;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

// Kudos to this guy (http://chriscavanagh.wordpress.com/2010/07/16/nullabledictionary/) for getting a NullableDictionary implementation for us.

namespace Interstates.Control.Database
{
    /// <summary>
    /// A class that represents a null value for a dictionary key.
    /// </summary>
    public static class NullableKey
    {
        /// <summary>
        /// Creates a new NullableKey for a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NullableKey<T> Create<T>(T value) { return new NullableKey<T>(value); }
    }

    /// <summary>
    /// A class that represents a null value for a dictionary key.
    /// The Nullable&lt;T&gt; value can be implicitly converted to the underlying type.
    /// </summary>
    /// <typeparam name="T">The type of value to store in the NullableKey</typeparam>
    public class NullableKey<T>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Create a new instance of the NullableKey class for a type.
        /// </summary>
        /// <param name="value"></param>
        public NullableKey(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Returns the value as a string.
        /// </summary>
        /// <returns>A string value.</returns>
        public override string ToString() { return (Value != null) ? Value.ToString() : null; }

        /// <summary>
        /// Converts a value of type T to a NullableKey&lt;T&gt;.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator NullableKey<T>(T value) { return new NullableKey<T>(value); }

        /// <summary>
        /// Converts a NullableKey&lt;T&gt; to a value of type T.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator T(NullableKey<T> value) { return value.Value; }
    }

    /// <summary>
    /// A Dictionary implementation that allows null values for the key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class NullableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        private Dictionary<NullableKey<TKey>, TValue> dict;

        public NullableDictionary()
            : this(null)
        {
        }

        public NullableDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }

        public NullableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            dict = new Dictionary<NullableKey<TKey>, TValue>(capacity, new EqualityComparer(comparer));
        }

        public void Add(TKey key, TValue value) { dict.Add(NullableKey.Create(key), value); }
        public bool ContainsKey(TKey key) { return dict.ContainsKey(NullableKey.Create(key)); }
        public ICollection<TKey> Keys { get { List<TKey> result = new List<TKey>(); foreach (NullableKey<TKey> key in Keys) result.Add(key.Value); return result; } }
        public bool Remove(TKey key) { return dict.Remove(NullableKey.Create(key)); }
        public bool TryGetValue(TKey key, out TValue value) { return dict.TryGetValue(NullableKey.Create(key), out value); }
        public ICollection<TValue> Values { get { return dict.Values; } }
        public TValue this[TKey key] { get { return dict[NullableKey.Create(key)]; } set { dict[NullableKey.Create(key)] = value; } }
        public int Count { get { return dict.Count; } }
        public void Clear() { dict.Clear(); }
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { throw new NotImplementedException(); }

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) { Add(item.Key, item.Value); }
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) { return ContainsKey(item.Key); }
        int ICollection<KeyValuePair<TKey, TValue>>.Count { get { return dict.Count; } }
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return false; } }
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) { return Remove(item.Key); }

        #endregion

        #region IDictionary Members

        void IDictionary.Add(object key, object value) { Add((TKey)key, (TValue)value); }
        bool IDictionary.Contains(object key) { return ContainsKey((TKey)key); }
        bool IDictionary.IsFixedSize { get { return false; } }
        bool IDictionary.IsReadOnly { get { return false; } }
        void IDictionary.Remove(object key) { Remove((TKey)key); }
        object IDictionary.this[object key] { get { return this[(TKey)key]; } set { this[(TKey)key] = (TValue)value; } }
        ICollection IDictionary.Keys { get { return new List<TKey>(Keys); } }
        ICollection IDictionary.Values { get { return new List<TValue>(Values); } }
        IDictionaryEnumerator IDictionary.GetEnumerator() { return dict.GetEnumerator(); }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index) { throw new NotImplementedException(); }
        object ICollection.SyncRoot { get { return ((ICollection)dict).SyncRoot; } }
        bool ICollection.IsSynchronized { get { return false; } }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>();
            foreach (KeyValuePair<NullableKey<TKey>, TValue> i in dict)
                result.Add(new KeyValuePair<TKey, TValue>(i.Key.Value, i.Value));
            return result.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion

        private class EqualityComparer : IEqualityComparer<NullableKey<TKey>>
        {
            private IEqualityComparer<TKey> comparer;

            public EqualityComparer()
            {
            }

            public EqualityComparer(IEqualityComparer<TKey> comparer)
            {
                this.comparer = comparer;
            }

            #region IEqualityComparer<NullableKey<TKey>> Members

            public bool Equals(NullableKey<TKey> x, NullableKey<TKey> y)
            {
                return (comparer != null)
                    ? comparer.Equals(x.Value, y.Value)
                    : object.Equals(x.Value, y.Value);
            }

            public int GetHashCode(NullableKey<TKey> obj)
            {
                return (comparer != null)
                    ? comparer.GetHashCode(obj.Value)
                    : (obj != null && obj.Value != null) ? obj.Value.GetHashCode() + 1 : 0;
            }

            #endregion
        }
    }
}

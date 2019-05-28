using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Interstates.Control.Database
{
    /// <summary>
    /// A collection of objects without a key.
    /// </summary>
    /// <typeparam name="T">The type of Value</typeparam>
    public abstract class EntityCollection<T> : Collection<T>
    {
        /// <summary>
        /// Add a collection of items to the collection.
        /// </summary>
        /// <param name="itemCollection"></param>
        public void AddRange(ICollection<T> itemCollection)
        {
            foreach (T item in itemCollection)
                this.Add(item);
        }

        /// <summary>
        /// Converts the elements in the current Collection to
        /// another type, and returns a list containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <param name="converter">A System.Converter&lt;TInput,TOutput&gt; delegate that converts each element from
        /// one type to another type.</param>
        /// <returns>A Collection of the target type containing the converted
        /// elements from the current Collection.</returns>
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null) throw new ArgumentNullException("converter");
            List<TOutput> list = new List<TOutput>(Count);
            for (int i = 0; i < Count; i++)
            {
                list.Add(converter(this[i]));
            }
            return list;
        }

        /// <summary>
        /// Determines whether the Collection contains elements
        /// that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns> true if the Collection contains one or more elements
        /// that match the conditions defined by the specified predicate; otherwise, false.</returns>
        public bool Exists(Predicate<T> match)
        {
            return (this.FindIndex(match) != -1);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the first occurrence within the entire Collection.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The first element that matches the conditions defined by the specified predicate,
        /// if found; otherwise, the default value for type T.</returns>
        public T Find(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException("match");
            for (int i = 0; i < Count; i++)
            {
                if (match(base.Items[i]))
                {
                    return base.Items[i];
                }
            }
            return default(T);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>A Collection containing all the elements that match
        /// the conditions defined by the specified predicate, if found; otherwise, 
        /// an empty Collection.</returns>
        public List<T> FindAll(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException("match");
            List<T> list = new List<T>();
            for (int i = 0; i < Count; i++)
            {
                if (match(base.Items[i]))
                {
                    list.Add(base.Items[i]);
                }
            }
            return list;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the first occurrence within
        /// the entire Collection.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1.</returns>
        public int FindIndex(Predicate<T> match)
        {
            return this.FindIndex(0, Count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the first occurrence within
        /// the range of elements in the System.Collections.Generic.Listt&lt;T&gt; that extends
        /// from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.</returns>
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return this.FindIndex(startIndex, Count - startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the first occurrence within
        /// the range of elements in the Collection that starts
        /// at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.</returns>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if (startIndex > Count) throw new ArgumentOutOfRangeException("index");
            if ((count < 0) || (startIndex > (Count - count))) throw new ArgumentOutOfRangeException("count");
            if (match == null) throw new ArgumentNullException("match");
            int num = startIndex + count;
            for (int i = startIndex; i < num; i++)
            {
                if (match(base.Items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, 
        /// and returns the last occurrence within the entire Collection.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The last element that matches the conditions defined by the specified predicate,
        /// if found; otherwise, the default value for type T.</returns>
        public T FindLast(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException("match");
            for (int i = Count - 1; i >= 0; i--)
            {
                if (match(base.Items[i]))
                {
                    return base.Items[i];
                }
            }
            return default(T);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the last occurrence within
        /// the entire Collection.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.</returns>
        public int FindLastIndex(Predicate<T> match)
        {
            return this.FindLastIndex(Count - 1, Count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the last occurrence within
        /// the range of elements in the Collection that extends
        /// from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.</returns>
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return this.FindLastIndex(startIndex, startIndex + 1, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the last occurrence within
        /// the range of elements in the Collection that contains
        /// the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.</returns>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException("match");
            if (Count == 0)
            {
                if (startIndex != -1)
                {
                    throw new ArgumentOutOfRangeException("startIndex");
                }
            }
            else if (startIndex >= Count) 
                throw new ArgumentOutOfRangeException("startIndex");
            if ((count < 0) || (((startIndex - count) + 1) < 0)) 
                throw new ArgumentOutOfRangeException("count");
            int num = startIndex - count;
            for (int i = startIndex; i > num; i--)
            {
                if (match(base.Items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Performs the specified action on each element of the collection.
        /// </summary>
        /// <param name="action">The System.Action&lt;T&gt; delegate to perform on each element.</param>
        public void ForEach(Action<T> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            foreach (T cur in base.Items)
            {
                action(cur);
            }
        }

        /// <summary>
        /// Sorts the elements in the entire Collection using the default comparer.
        /// </summary>
        public void Sort()
        {
            this.Sort(0, this.Count, null);
        }

        /// <summary>
        /// Sorts the elements in the entire Collection using the specified System.Comparison&lt;T&lt;.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer)
        {
            this.Sort(0, this.Count, comparer);
        }

        /// <summary>
        /// Sorts the elements in the entire Collection using the specified comparer.
        /// </summary>
        /// <param name="comparison">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing
        /// elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException("comparison");
            if (Count > 0)
            {
                T[] items = new T[Count];
                base.CopyTo(items, 0);
                IComparer<T> comparer = new FunctorComparer<T>(comparison);
                Array.Sort<T>(items, 0, Count, comparer);
                for (int i = 0; i < Count; i++)
                    base.Items[i] = items[i];
            }
        }

        /// <summary>
        /// Sorts the elements in a range of elements in collection using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing
        /// elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "The value must not be negative");
            }
            if ((Count - index) < count)
            {
                throw new ArgumentOutOfRangeException("Invalid offset length");
            }
            T[] items = new T[Count];
            base.CopyTo(items, index);
            Array.Sort<T>(items, index, count, comparer);
            for (int i = index; i < count; i++)
                base.Items[i] = items[i];
        }

        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException("match");
            for (int i = 0; i < Count; i++)
            {
                if (!match(base.Items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the value at the specified index.
        /// </summary>
        /// <param name="index">The 0 based index to get.</param>
        /// <returns>Returns the value at the index.</returns>
        public T ValueAt(int index)
        {
            return base[index];
        }

        /// <summary>
        /// Sets the value at the specified index.
        /// </summary>
        /// <param name="index">The 0 based index to set.</param>
        /// <param name="value">The value to set the index position to.</param>
        /// <returns></returns>
        public void SetValueAt(int index, T value)
        {
            base[index] = value;
        }

    }

    #region EntityCollection<K, T>
    /// <summary>
    /// A collection of objects keyed by a single value.
    /// </summary>
    /// <typeparam name="K">The type of Key</typeparam>
    /// <typeparam name="T">The type of Value</typeparam>
    [Serializable, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(EntityCollectionDebugView<,>)), ComVisible(false)]
    public abstract class EntityCollection<K, T> : EntityCollection<T>
    {
        // Fields
        private IEqualityComparer<K> _comparer;
        private const int _defaultThreshold = 0;
        private Dictionary<K, T> _dict;
        private int _keyCount;
        private int _threshold;

        /// <summary>
        /// Initializes a new EnityCollection object.
        /// </summary>
        protected EntityCollection()
            : this(null, 0)
        {
        }

        /// <summary>
        /// Initializes a new EnityCollection object.
        /// </summary>
        /// <param name="comparer">The comparer to use to check for object equality.</param>
        protected EntityCollection(IEqualityComparer<K> comparer)
            : this(comparer, 0)
        {
        }

        /// <summary>
        /// Initializes a new EnityCollection object.
        /// </summary>
        /// <param name="comparer">The comparer to use to check for object equality.</param>
        /// <param name="dictionaryCreationThreshold">The theshold to reach before creating the underlying dictionary object.</param>
        protected EntityCollection(IEqualityComparer<K> comparer, int dictionaryCreationThreshold)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<K>.Default;
            }
            if (dictionaryCreationThreshold == -1)
            {
                dictionaryCreationThreshold = int.MaxValue;
            }
            if (dictionaryCreationThreshold < -1)
            {
                throw new ArgumentOutOfRangeException("dictionaryCreationThreshold", "Invalid threshold specified.");
            }
            _comparer = comparer;
            _threshold = dictionaryCreationThreshold;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="item">When this method returns true, contains the value associated with the specified
        /// key, if the key is found; otherwise, the default value for the type of the
        /// value parameter.</param>
        /// <returns></returns>
        public bool TryGetValue(K key, out T item)
        {
            if (_dict == null)
            {
                item = default(T);
                return false;
            }

            return _dict.TryGetValue(key, out item);
        }

        /// <summary>
        /// Determines whether the Dictionary&lt;K,T&gt;
        /// contains the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(K key)
        {
            if (_dict == null)
                return false;

            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// Gets a collection containing the keys in the dictionary.
        /// </summary>
        public ICollection<K> Keys
        {
            get { return _dict.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the values in the dictionary.
        /// </summary>
        public ICollection<T> Values
        {
            get { return _dict.Values; }
        }

        private void AddKey(K key, T item)
        {
            if (_dict != null)
            {
                _dict.Add(key, item);
            }
            else if (_keyCount == _threshold)
            {
                this.CreateDictionary();
                _dict.Add(key, item);
            }
            else
            {
                if (this.Contains(key))
                {
                    throw new ArgumentException("Adding duplicate key for item");
                }
                _keyCount++;
            }
        }

        /// <summary>
        /// Changes the key for an item in the collection.
        /// </summary>
        /// <param name="item">The item to change.</param>
        /// <param name="newKey">The new key for the item.</param>
        protected void ChangeItemKey(T item, K newKey)
        {
            if (!this.Contains(item))
            {
                throw new ArgumentException("Item does not exist for key");
            }
            K keyForItem = this.GetKeyForItem(item);
            if (!_comparer.Equals(keyForItem, newKey))
            {
                if (newKey != null)
                {
                    this.AddKey(newKey, item);
                }
                if (keyForItem != null)
                {
                    this.RemoveKey(keyForItem);
                }
            }
        }

        /// <summary>
        /// Clears all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            if (_dict != null)
            {
                _dict.Clear();
            }
            _keyCount = 0;
        }

        /// <summary>
        /// Returns true if the key is contained in the collection.
        /// </summary>
        /// <param name="key">The key to check for existence in the collection.</param>
        /// <returns></returns>
        public bool Contains(K key)
        {
            if (key == null)
            {
                throw new NullReferenceException("key");
            }
            if (_dict != null)
            {
                return _dict.ContainsKey(key);
            }
            foreach (T local in base.Items)
            {
                if (_comparer.Equals(this.GetKeyForItem(local), key))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the item is found in the collection.
        /// </summary>
        /// <param name="item">The item to check for existence in the collection.</param>
        /// <returns></returns>
        public new bool Contains(T item)
        {
            if (item == null)
            {
                throw new NullReferenceException("item");
            }
            K key = this.GetKeyForItem(item);
            if (_dict != null)
            {
                return _dict.ContainsKey(key);
            }
            foreach (T local in base.Items)
            {
                if (_comparer.Equals(this.GetKeyForItem(local), key))
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateDictionary()
        {
            _dict = new Dictionary<K, T>(_comparer);
            foreach (T local in base.Items)
            {
                K keyForItem = this.GetKeyForItem(local);
                if (keyForItem != null)
                {
                    _dict.Add(keyForItem, local);
                }
            }
        }

        /// <summary>
        /// Gets the key that uniquely identifies the item in the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract K GetKeyForItem(T item);

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The index into the collection.</param>
        /// <param name="item">The item to store in the collection</param>
        protected override void InsertItem(int index, T item)
        {
            K keyForItem = this.GetKeyForItem(item);
            if (keyForItem != null)
            {
                this.AddKey(keyForItem, item);
            }
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item associated with the specified key from the collection.
        /// </summary>
        /// <param name="key">The key of the item to remove from the collection.</param>
        /// <returns></returns>
        public bool Remove(K key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (_dict != null)
            {
                return (_dict.ContainsKey(key) && base.Remove(_dict[key]));
            }
            for (int i = 0; i < base.Items.Count; i++)
            {
                if (_comparer.Equals(this.GetKeyForItem(base.Items[i]), key))
                {
                    this.RemoveItem(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the item at the specified index position from the collection
        /// </summary>
        /// <param name="index">The 0 based index to remove.</param>
        protected override void RemoveItem(int index)
        {
            K keyForItem = this.GetKeyForItem(base.Items[index]);
            if (keyForItem != null)
            {
                this.RemoveKey(keyForItem);
            }
            base.RemoveItem(index);
        }

        private void RemoveKey(K key)
        {
            if (_dict != null)
            {
                _dict.Remove(key);
            }
            else
            {
                _keyCount--;
            }
        }

        /// <summary>
        /// Sets the specified index position to the item.
        /// </summary>
        /// <param name="index">The 0 based index to set.</param>
        /// <param name="item">The item to add to the collection.</param>
        protected override void SetItem(int index, T item)
        {
            K keyForItem = this.GetKeyForItem(item);
            K x = this.GetKeyForItem(base.Items[index]);
            if (_comparer.Equals(x, keyForItem))
            {
                if ((keyForItem != null) && (_dict != null))
                {
                    _dict[keyForItem] = item;
                }
            }
            else
            {
                if (keyForItem != null)
                {
                    this.AddKey(keyForItem, item);
                }
                if (x != null)
                {
                    this.RemoveKey(x);
                }
            }
            base.SetItem(index, item);
        }

        /// <summary>
        /// The equality comparer to use for equality checks.
        /// </summary>
        public IEqualityComparer<K> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        /// <summary>
        /// The internal dictionary used to key the collection.
        /// </summary>
        protected IDictionary<K, T> Dictionary
        {
            get
            {
                return _dict;
            }
        }

        /// <summary>
        /// Gets or sets an item associated with the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[K key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                if (_dict != null)
                {
                    return _dict[key];
                }
                foreach (T local in base.Items)
                {
                    if (_comparer.Equals(this.GetKeyForItem(local), key))
                    {
                        return local;
                    }
                }
                throw new KeyNotFoundException();
            }
        }
    }

    internal sealed class EntityCollectionDebugView<K, T>
    {
        // Fields
        private EntityCollection<K, T> kc;

        // Methods
        public EntityCollectionDebugView(EntityCollection<K, T> keyedCollection)
        {
            if (keyedCollection == null)
            {
                throw new ArgumentNullException("keyedCollection");
            }
            this.kc = keyedCollection;
        }

        // Properties
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.kc.Count];
                this.kc.CopyTo(array, 0);
                return array;
            }
        }
    }
    #endregion

    #region EntityCollection<K1, K2, T>
    /// <summary>
    /// A collection of objects keyed by a composite key of two values.
    /// </summary>	
    /// <typeparam name="K1">The type for Key1</typeparam>
    /// <typeparam name="K2">The type for Key2</typeparam>
    /// <typeparam name="T">The type of Value</typeparam>
    [Serializable, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(EntityCollectionDebugView<,,>)), ComVisible(false)]
    public abstract class EntityCollection<K1, K2, T> : EntityCollection<T>
    {
        // Fields
        private const int _defaultThreshold = 0;
        private int _keyCount;
        private int _threshold;
        private IEqualityComparer<K1> _comparer1;
        private IEqualityComparer<K2> _comparer2;

        protected Dictionary<K1, Dictionary<K2, T>> _dict;

        protected EntityCollection()
            : this(null, null, 0)
        {
        }

        protected EntityCollection(IEqualityComparer<K1> comparer1, IEqualityComparer<K2> comparer2)
            : this(comparer1, comparer2, 0)
        {
        }

        protected EntityCollection(IEqualityComparer<K1> comparer1, IEqualityComparer<K2> comparer2, int dictionaryCreationThreshold)
        {
            if (comparer1 == null)
            {
                comparer1 = EqualityComparer<K1>.Default;
            }
            if (comparer2 == null)
            {
                comparer2 = EqualityComparer<K2>.Default;
            }
            if (dictionaryCreationThreshold == -1)
            {
                dictionaryCreationThreshold = Int32.MaxValue;
            }
            if (dictionaryCreationThreshold < -1)
            {
                throw new ArgumentOutOfRangeException("dictionaryCreationThreshold", "The value for the dictionary creation theshold cannot be less than -1.");
            }
            _comparer1 = comparer1;
            _comparer2 = comparer2;
            _threshold = dictionaryCreationThreshold;
        }

        /// <summary>
        /// Comparer for key 1.
        /// </summary>
        public IEqualityComparer<K1> Comparer1 { get { return _comparer1; } }
        /// <summary>
        /// Comparer for key 2.
        /// </summary>
        public IEqualityComparer<K2> Comparer2 { get { return _comparer2; } }

        public bool TryGetValue(K1 key1, K2 key2, out T value)
        {
            value = default(T);

            Dictionary<K2, T> dict2;

            if (_dict == null)
                return false;

            if (_dict.TryGetValue(key1, out dict2))
            {
                return dict2.TryGetValue(key2, out value);
            }

            return false;
        }

        public bool ContainsKey(K1 key1, K2 key2)
        {
            T unused;

            return TryGetValue(key1, key2, out unused);
        }

        public ICollection<object[]> Keys
        {
            get
            {
                List<object[]> keyList = new List<object[]>();

                foreach (K1 key1 in _dict.Keys)
                    foreach (K2 key2 in _dict[key1].Keys)
                        keyList.Add(new object[] { key1, key2 });

                return keyList;
            }
        }

        public ICollection<T> Values
        {
            get
            {
                List<T> valueList = new List<T>();

                foreach (Dictionary<K2, T> dict2 in _dict.Values)
                    valueList.AddRange(dict2.Values);

                return valueList;
            }
        }

        private void AddKey(K1 key1, K2 key2, T item)
        {
            if (_dict != null)
            {
                if (!(_dict.ContainsKey(key1)))
                {
                    _dict[key1] = new Dictionary<K2, T>(_comparer2);
                }
                _dict[key1].Add(key2, item);
            }
            else if (_keyCount == _threshold)
            {
                this.CreateDictionary();
                _dict.Add(key1, new Dictionary<K2, T>());
                _dict[key1].Add(key2, item);
            }
            else
            {
                if (this.Contains(key1, key2))
                {
                    throw new ArgumentException("Adding duplicate key for item");
                }
                _keyCount++;
            }
        }

        protected override void InsertItem(int index, T item)
        {
            K1 key1;
            K2 key2;
            if (GetKeysForItem(item, out key1, out key2))
            {
                this.AddKey(key1, key2, item);
            }
            base.InsertItem(index, item);
        }

        protected void ChangeItemKey(T item, K1 newKey1, K2 newKey2)
        {
            if (!this.Contains(item))
            {
                throw new ArgumentException("Item does not exist for key");
            }
            K1 key1;
            K2 key2;
            bool validKey = GetKeysForItem(item, out key1, out key2);
            if (!_comparer1.Equals(key1, newKey1)
                &&
                !_comparer2.Equals(key2, newKey2))
            {
                if (newKey1 != null && newKey2 != null)
                {
                    this.AddKey(newKey1, newKey2, item);
                }
                if (validKey)
                {
                    this.RemoveKey(key1, key2);
                }
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            if (_dict != null)
            {
                _dict.Clear();
            }
            _keyCount = 0;
        }

        public bool Contains(K1 key1, K2 key2)
        {
            if (key1 == null)
            {
                throw new NullReferenceException("key1");
            }
            if (key2 == null)
            {
                throw new NullReferenceException("key2");
            }
            if (_dict != null)
            {
                return ContainsKey(key1, key2);
            }
            if (key1 != null && key2 != null)
            {
                foreach (T local in base.Items)
                {
                    K1 localKey1;
                    K2 localKey2;
                    this.GetKeysForItem(local, out localKey1, out localKey2);
                    if (_comparer1.Equals(localKey1, key1)
                        &&
                        _comparer2.Equals(localKey2, key2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public new bool Contains(T item)
        {
            if (item == null)
            {
                throw new NullReferenceException("item");
            }
            K1 key1;
            K2 key2;
            GetKeysForItem(item, out key1, out key2);
            if (key1 == null)
            {
                throw new NullReferenceException("key1");
            }
            if (key2 == null)
            {
                throw new NullReferenceException("key2");
            }
            if (_dict != null)
            {
                return ContainsKey(key1, key2);
            }
            foreach (T local in base.Items)
            {
                K1 localKey1;
                K2 localKey2;
                this.GetKeysForItem(local, out localKey1, out localKey2);
                if (_comparer1.Equals(localKey1, key1)
                    &&
                    _comparer2.Equals(localKey2, key2))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddItem(K1 key1, K2 key2, T item)
        {
            if (!(_dict.ContainsKey(key1)))
            {
                _dict[key1] = new Dictionary<K2, T>(_comparer2);
            }
            _dict[key1].Add(key2, item);
        }

        private void CreateDictionary()
        {
            _dict = new Dictionary<K1, Dictionary<K2, T>>(_comparer1);
            foreach (T local in base.Items)
            {
                K1 key1;
                K2 key2;
                if (GetKeysForItem(local, out key1, out key2))
                {
                    AddItem(key1, key2, local);
                }
            }
        }

        protected abstract object[] GetKeyForItem(T item);

        /// <summary>
        /// Returns true if the GetKeyForItem is not null.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        private bool GetKeysForItem(T item, out K1 key1, out K2 key2)
        {
            object[] keyForItem = this.GetKeyForItem(item);
            if (keyForItem == null)
            {
                key1 = default(K1);
                key2 = default(K2);
                return false;
            }
            key1 = (K1)keyForItem[0];
            key2 = (K2)keyForItem[1];
            return true;
        }

        public bool Remove(K1 key1, K2 key2)
        {
            if (key1 == null)
            {
                throw new ArgumentNullException("key1");
            }
            if (key2 == null)
            {
                throw new ArgumentNullException("key2");
            }
            if (_dict != null)
            {
                return (ContainsKey(key1, key2) && base.Remove(this[key1, key2]));
            }
            for (int i = 0; i < base.Items.Count; i++)
            {
                K1 k1;
                K2 k2;
                GetKeysForItem(base.Items[i], out k1, out k2);
                if (_comparer1.Equals(key1, k1) &&
                    _comparer2.Equals(key2, k2))
                {
                    this.RemoveItem(i);
                    return true;
                }
            }
            return false;
        }

        protected override void RemoveItem(int index)
        {
            K1 key1;
            K2 key2;
            GetKeysForItem(base.Items[index], out key1, out key2);
            if (key1 != null && key2 != null)
            {
                this.RemoveKey(key1, key2);
            }
            base.RemoveItem(index);
        }

        private void RemoveKey(K1 key1, K2 key2)
        {
            if (_dict != null)
            {
                if (_dict.ContainsKey(key1))
                {
                    if (_dict[key1].ContainsKey(key2))
                    {
                        _dict[key1].Remove(key2);
                    }
                }
            }
            else
            {
                _keyCount--;
            }
        }

        protected override void SetItem(int index, T item)
        {
            K1 key1;
            K2 key2;
            K1 x1;
            K2 x2;
            GetKeysForItem(item, out key1, out key2);
            GetKeysForItem(base.Items[index], out x1, out x2);
            if (_comparer1.Equals(x1, key1) &&
                _comparer2.Equals(x2, key2))
            {
                if ((key1 != null) && (key2 != null) && (_dict != null))
                    AddItem(key1, key2, item);
            }
            else
            {
                if ((key1 != null) && (key2 != null))
                    AddItem(key1, key2, item);
                if (x1 != null && x2 != null)
                    this.RemoveKey(x1, x2);
            }
            base.SetItem(index, item);
        }

        protected Dictionary<K1, Dictionary<K2, T>> Dictionary
        {
            get
            {
                return _dict;
            }
        }

        public T this[K1 key1, K2 key2]
        {
            get
            {
                if (key1 == null)
                {
                    throw new ArgumentNullException("key1");
                }
                if (key2 == null)
                {
                    throw new ArgumentNullException("key2");
                }
                T item;
                if (_dict != null)
                {
                    if (TryGetValue(key1, key2, out item))
                        return item;
                }
                foreach (T local in base.Items)
                {
                    K1 k1;
                    K2 k2;
                    GetKeysForItem(local, out k1, out k2);
                    if (_comparer1.Equals(k1, key1) &&
                        _comparer2.Equals(k2, key2))
                    {
                        return local;
                    }
                }

                throw new KeyNotFoundException(string.Format("Key not found for {0}, {1}", key1, key2));
            }
        }
    }

    internal sealed class EntityCollectionDebugView<K1, K2, T>
    {
        // Fields
        private EntityCollection<K1, K2, T> kc;

        // Methods
        public EntityCollectionDebugView(EntityCollection<K1, K2, T> keyedCollection)
        {
            if (keyedCollection == null)
            {
                throw new ArgumentNullException("keyedCollection");
            }
            this.kc = keyedCollection;
        }

        // Properties
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.kc.Count];
                this.kc.CopyTo(array, 0);
                return array;
            }
        }
    }
    #endregion

    #region EntityCollection<K1, K2, K3, T>
    /// <summary>
    /// A collection of objects keyed by a composite key of three values.
    /// </summary>	
    /// <typeparam name="K1">The type for Key1</typeparam>
    /// <typeparam name="K2">The type for Key2</typeparam>
    /// <typeparam name="K3">The type for Key3</typeparam>
    /// <typeparam name="T">The type of Value</typeparam>
    [Serializable, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(EntityCollectionDebugView<,,,,>)), ComVisible(false)]
    public abstract class EntityCollection<K1, K2, K3, T> : EntityCollection<T>
    {
        // Fields
        private const int _defaultThreshold = 0;
        private int _keyCount;
        private int _threshold;
        private IEqualityComparer<K1> _comparer1;
        private IEqualityComparer<K2> _comparer2;
        private IEqualityComparer<K3> _comparer3;

        protected Dictionary<K1, Dictionary<K2, Dictionary<K3, T>>> _dict;

        protected EntityCollection()
            : this(null, null, null, 0)
        {
        }

        protected EntityCollection(IEqualityComparer<K1> comparer1, IEqualityComparer<K2> comparer2, IEqualityComparer<K3> comparer3)
            : this(comparer1, comparer2, comparer3, 0)
        {
        }

        protected EntityCollection(IEqualityComparer<K1> comparer1, IEqualityComparer<K2> comparer2, IEqualityComparer<K3> comparer3, int dictionaryCreationThreshold)
        {
            if (comparer1 == null)
            {
                comparer1 = EqualityComparer<K1>.Default;
            }
            if (comparer2 == null)
            {
                comparer2 = EqualityComparer<K2>.Default;
            }
            if (comparer3 == null)
            {
                comparer3 = EqualityComparer<K3>.Default;
            }
            if (dictionaryCreationThreshold == -1)
            {
                dictionaryCreationThreshold = Int32.MaxValue;
            }
            if (dictionaryCreationThreshold < -1)
            {
                throw new ArgumentOutOfRangeException("dictionaryCreationThreshold", "The value for the dictionary creation theshold cannot be less than -1.");
            }
            _comparer1 = comparer1;
            _comparer2 = comparer2;
            _comparer3 = comparer3;
            _threshold = dictionaryCreationThreshold;
        }

        /// <summary>
        /// Comparer for key 1.
        /// </summary>
        public IEqualityComparer<K1> Comparer1 { get { return _comparer1; } }
        /// <summary>
        /// Comparer for key 2.
        /// </summary>
        public IEqualityComparer<K2> Comparer2 { get { return _comparer2; } }
        /// <summary>
        /// Comparer for key 3.
        /// </summary>
        public IEqualityComparer<K3> Comparer3 { get { return _comparer3; } }

        public bool TryGetValue(K1 key1, K2 key2, K3 key3, out T value)
        {
            value = default(T);

            Dictionary<K2, Dictionary<K3, T>> dict2;
            Dictionary<K3, T> dict3;

            if (_dict == null)
                return false;

            if (_dict.TryGetValue(key1, out dict2))
            {
                if (dict2.TryGetValue(key2, out dict3))
                    return dict3.TryGetValue(key3, out value);
            }

            return false;
        }

        public bool ContainsKey(K1 key1, K2 key2, K3 key3)
        {
            T unused;

            return TryGetValue(key1, key2, key3, out unused);
        }

        public ICollection<object[]> Keys
        {
            get
            {
                List<object[]> keyList = new List<object[]>();

                foreach (K1 key1 in _dict.Keys)
                    foreach (K2 key2 in _dict[key1].Keys)
                        foreach (K3 key3 in _dict[key1][key2].Keys)
                            keyList.Add(new object[] { key1, key2, key3 });

                return keyList;
            }
        }

        public ICollection<T> Values
        {
            get
            {
                List<T> valueList = new List<T>();

                foreach (Dictionary<K2, Dictionary<K3, T>> dict2 in _dict.Values)
                    foreach (Dictionary<K3, T> dict3 in dict2.Values)
                        valueList.AddRange(dict3.Values);

                return valueList;
            }
        }

        private void AddKey(K1 key1, K2 key2, K3 key3, T item)
        {
            if (_dict != null)
            {
                if (!(_dict.ContainsKey(key1)))
                {
                    _dict[key1] = new Dictionary<K2, Dictionary<K3, T>>(_comparer2);
                }
                if (!(_dict[key1].ContainsKey(key2)))
                {
                    _dict[key1][key2] = new Dictionary<K3, T>(_comparer3);
                }
                _dict[key1][key2].Add(key3, item);
            }
            else if (_keyCount == _threshold)
            {
                this.CreateDictionary();
                _dict.Add(key1, new Dictionary<K2, Dictionary<K3, T>>(_comparer2));
                _dict[key1].Add(key2, new Dictionary<K3, T>(_comparer3));
                _dict[key1][key2].Add(key3, item);
            }
            else
            {
                if (this.Contains(key1, key2, key3))
                {
                    throw new ArgumentException("Adding duplicate key for item");
                }
                _keyCount++;
            }
        }

        protected override void InsertItem(int index, T item)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            if (GetKeysForItem(item, out key1, out key2, out key3))
            {
                this.AddKey(key1, key2, key3, item);
            }
            base.InsertItem(index, item);
        }

        protected void ChangeItemKey(T item, K1 newKey1, K2 newKey2, K3 newKey3)
        {
            if (!this.Contains(item))
            {
                throw new ArgumentException("Item does not exist for key");
            }
            K1 key1;
            K2 key2;
            K3 key3;
            bool validKey = GetKeysForItem(item, out key1, out key2, out key3);
            if (!_comparer1.Equals(key1, newKey1)
                &&
                !_comparer2.Equals(key2, newKey2)
                &&
                !_comparer3.Equals(key3, newKey3))
            {
                if (newKey1 != null && newKey2 != null && newKey3 != null)
                {
                    this.AddKey(newKey1, newKey2, newKey3, item);
                }
                if (validKey)
                {
                    this.RemoveKey(key1, key2, key3);
                }
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            if (_dict != null)
            {
                _dict.Clear();
            }
            _keyCount = 0;
        }

        public bool Contains(K1 key1, K2 key2, K3 key3)
        {
            if (key1 == null)
            {
                throw new NullReferenceException("key1");
            }
            if (key2 == null)
            {
                throw new NullReferenceException("key2");
            }
            if (key3 == null)
            {
                throw new NullReferenceException("key3");
            }
            if (_dict != null)
            {
                return ContainsKey(key1, key2, key3);
            }
            if (key1 != null && key2 != null)
            {
                foreach (T local in base.Items)
                {
                    K1 localKey1;
                    K2 localKey2;
                    K3 localKey3;
                    this.GetKeysForItem(local, out localKey1, out localKey2, out localKey3);
                    if (_comparer1.Equals(localKey1, key1)
                        &&
                        _comparer2.Equals(localKey2, key2)
                        &&
                        _comparer3.Equals(localKey3, key3))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public new bool Contains(T item)
        {
            if (item == null)
            {
                throw new NullReferenceException("item");
            }
            K1 key1;
            K2 key2;
            K3 key3;
            GetKeysForItem(item, out key1, out key2, out key3);
            if (key1 == null)
            {
                throw new NullReferenceException("key1");
            }
            if (key2 == null)
            {
                throw new NullReferenceException("key2");
            }
            if (key3 == null)
            {
                throw new NullReferenceException("key3");
            }
            if (_dict != null)
            {
                return ContainsKey(key1, key2, key3);
            }
            foreach (T local in base.Items)
            {
                K1 localKey1;
                K2 localKey2;
                K3 localKey3;
                this.GetKeysForItem(local, out localKey1, out localKey2, out localKey3);
                if (_comparer1.Equals(localKey1, key1)
                    &&
                    _comparer2.Equals(localKey2, key2)
                    &&
                    _comparer3.Equals(localKey3, key3))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddItem(K1 key1, K2 key2, K3 key3, T item)
        {
            if (!(_dict.ContainsKey(key1)))
            {
                _dict[key1] = new Dictionary<K2, Dictionary<K3, T>>(_comparer2);
            }
            if (!(_dict[key1].ContainsKey(key2)))
            {
                _dict[key1][key2] = new Dictionary<K3, T>(_comparer3);
            }
            _dict[key1][key2].Add(key3, item);
        }

        private void CreateDictionary()
        {
            _dict = new Dictionary<K1, Dictionary<K2, Dictionary<K3, T>>>(_comparer1);
            foreach (T local in base.Items)
            {
                K1 key1;
                K2 key2;
                K3 key3;
                if (GetKeysForItem(local, out key1, out key2, out key3))
                {
                    AddItem(key1, key2, key3, local);
                }
            }
        }

        protected abstract object[] GetKeyForItem(T item);

        /// <summary>
        /// Returns true if the GetKeyForItem is not null.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="key3"></param>
        /// <returns></returns>
        private bool GetKeysForItem(T item, out K1 key1, out K2 key2, out K3 key3)
        {
            object[] keyForItem = this.GetKeyForItem(item);
            if (keyForItem == null)
            {
                key1 = default(K1);
                key2 = default(K2);
                key3 = default(K3);
                return false;
            }
            key1 = (K1)keyForItem[0];
            key2 = (K2)keyForItem[1];
            key3 = (K3)keyForItem[2];
            return true;
        }

        public bool Remove(K1 key1, K2 key2, K3 key3)
        {
            if (key1 == null)
            {
                throw new ArgumentNullException("key1");
            }
            if (key2 == null)
            {
                throw new ArgumentNullException("key2");
            }
            if (key3 == null)
            {
                throw new ArgumentNullException("key3");
            }
            if (_dict != null)
            {
                return (ContainsKey(key1, key2, key3) && base.Remove(this[key1, key2, key3]));
            }
            for (int i = 0; i < base.Items.Count; i++)
            {
                K1 k1;
                K2 k2;
                K3 k3;
                GetKeysForItem(base.Items[i], out k1, out k2, out k3);
                if (_comparer1.Equals(key1, k1)
                    &&
                    _comparer2.Equals(key2, k2)
                    &&
                    _comparer3.Equals(key3, k3))
                {
                    this.RemoveItem(i);
                    return true;
                }
            }
            return false;
        }

        protected override void RemoveItem(int index)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            GetKeysForItem(base.Items[index], out key1, out key2, out key3);
            if (key1 != null && key2 != null && key3 != null)
            {
                this.RemoveKey(key1, key2, key3);
            }
            base.RemoveItem(index);
        }

        private void RemoveKey(K1 key1, K2 key2, K3 key3)
        {
            if (_dict != null)
            {
                if (_dict.ContainsKey(key1)
                    &&
                    _dict[key1].ContainsKey(key2)
                    &&
                    _dict[key1][key2].ContainsKey(key3))
                {
                    _dict[key1][key2].Remove(key3);
                }
            }
            else
            {
                _keyCount--;
            }
        }

        protected override void SetItem(int index, T item)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            K1 x1;
            K2 x2;
            K3 x3;
            GetKeysForItem(item, out key1, out key2, out key3);
            GetKeysForItem(base.Items[index], out x1, out x2, out x3);
            if (_comparer1.Equals(x1, key1)
                &&
                _comparer2.Equals(x2, key2)
                &&
                _comparer3.Equals(x3, key3))
            {
                if ((key1 != null) && (key2 != null) && (key3 != null) && (_dict != null))
                    AddItem(key1, key2, key3, item);
            }
            else
            {
                if (key1 != null && key2 != null && key3 != null)
                    AddItem(key1, key2, key3, item);
                if (x1 != null && x2 != null && x3 != null)
                    this.RemoveKey(x1, x2, x3);
            }
            base.SetItem(index, item);
        }

        protected Dictionary<K1, Dictionary<K2, Dictionary<K3, T>>> Dictionary
        {
            get
            {
                return _dict;
            }
        }

        public T this[K1 key1, K2 key2, K3 key3]
        {
            get
            {
                if (key1 == null)
                {
                    throw new ArgumentNullException("key1");
                }
                if (key2 == null)
                {
                    throw new ArgumentNullException("key2");
                }
                if (key3 == null)
                {
                    throw new ArgumentNullException("key3");
                }
                T item;
                if (_dict != null)
                {
                    if (TryGetValue(key1, key2, key3, out item))
                        return item;
                }
                foreach (T local in base.Items)
                {
                    K1 k1;
                    K2 k2;
                    K3 k3;
                    GetKeysForItem(local, out k1, out k2, out k3);
                    if (_comparer1.Equals(k1, key1)
                        &&
                        _comparer2.Equals(k2, key2)
                        &&
                        _comparer3.Equals(k3, key3))
                    {
                        return local;
                    }
                }

                throw new KeyNotFoundException(string.Format("Key not found for {0}, {1}, {2}", key1, key2, key3));
            }
        }
    }

    internal sealed class EntityCollectionDebugView<K1, K2, K3, T>
    {
        // Fields
        private EntityCollection<K1, K2, K3, T> kc;

        // Methods
        public EntityCollectionDebugView(EntityCollection<K1, K2, K3, T> keyedCollection)
        {
            if (keyedCollection == null)
            {
                throw new ArgumentNullException("keyedCollection");
            }
            this.kc = keyedCollection;
        }

        // Properties
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.kc.Count];
                this.kc.CopyTo(array, 0);
                return array;
            }
        }
    }
    #endregion

    #region EntityCollection<K1, K2, K3, K4, T>
    /// <summary>
    /// A collection of objects keyed by a composite key of four values.
    /// </summary>	
    /// <typeparam name="K1">The type for Key1</typeparam>
    /// <typeparam name="K2">The type for Key2</typeparam>
    /// <typeparam name="K3">The type for Key3</typeparam>
    /// <typeparam name="K4">The type for Key4</typeparam>
    /// <typeparam name="T">The type of Value</typeparam>
    [Serializable, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(EntityCollectionDebugView<,,,>)), ComVisible(false)]
    public abstract class EntityCollection<K1, K2, K3, K4, T> : EntityCollection<T>
    {
        // Fields
        private const int _defaultThreshold = 0;
        private int _keyCount;
        private int _threshold;
        private IEqualityComparer<K1> _comparer1;
        private IEqualityComparer<K2> _comparer2;
        private IEqualityComparer<K3> _comparer3;
        private IEqualityComparer<K4> _comparer4;
        
        protected Dictionary<K1, Dictionary<K2, Dictionary<K3, Dictionary<K4, T>>>> _dict;

        protected EntityCollection()
            : this(null, null, null, null, 0)
        {
        }

        protected EntityCollection(IEqualityComparer<K1> comparer1, IEqualityComparer<K2> comparer2, IEqualityComparer<K3> comparer3, IEqualityComparer<K4> comparer4)
            : this(comparer1, comparer2, comparer3, comparer4, 0)
        {
        }

        protected EntityCollection(IEqualityComparer<K1> comparer1, IEqualityComparer<K2> comparer2, IEqualityComparer<K3> comparer3, IEqualityComparer<K4> comparer4, int dictionaryCreationThreshold)
        {
            if (comparer1 == null)
            {
                comparer1 = EqualityComparer<K1>.Default;
            }
            if (comparer2 == null)
            {
                comparer2 = EqualityComparer<K2>.Default;
            }
            if (comparer3 == null)
            {
                comparer3 = EqualityComparer<K3>.Default;
            }
            if (comparer4 == null)
            {
                comparer4 = EqualityComparer<K4>.Default;
            }
            if (dictionaryCreationThreshold == -1)
            {
                dictionaryCreationThreshold = Int32.MaxValue;
            }
            if (dictionaryCreationThreshold < -1)
            {
                throw new ArgumentOutOfRangeException("dictionaryCreationThreshold", "The value for the dictionary creation theshold cannot be less than -1.");
            }
            _comparer1 = comparer1;
            _comparer2 = comparer2;
            _comparer3 = comparer3;
            _comparer4 = comparer4;
            _threshold = dictionaryCreationThreshold;
        }

        /// <summary>
        /// Comparer for key 1.
        /// </summary>
        public IEqualityComparer<K1> Comparer1 { get { return _comparer1; } }
        /// <summary>
        /// Comparer for key 2.
        /// </summary>
        public IEqualityComparer<K2> Comparer2 { get { return _comparer2; } }
        /// <summary>
        /// Comparer for key 3.
        /// </summary>
        public IEqualityComparer<K3> Comparer3 { get { return _comparer3; } }
        /// <summary>
        /// Comparer for key 4.
        /// </summary>
        public IEqualityComparer<K4> Comparer4 { get { return _comparer4; } }

        public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, out T value)
        {
            value = default(T);

            Dictionary<K2, Dictionary<K3, Dictionary<K4, T>>> dict2;
            Dictionary<K3, Dictionary<K4, T>> dict3;
            Dictionary<K4, T> dict4;

            if (_dict == null)
                return false;

            if (_dict.TryGetValue(key1, out dict2))
            {
                if (dict2.TryGetValue(key2, out dict3))
                    if (dict3.TryGetValue(key3, out dict4))
                        return dict4.TryGetValue(key4, out value);
            }

            return false;
        }

        public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4)
        {
            T unused;

            return TryGetValue(key1, key2, key3, key4, out unused);
        }

        public ICollection<object[]> Keys
        {
            get
            {
                List<object[]> keyList = new List<object[]>();

                foreach (K1 key1 in _dict.Keys)
                    foreach (K2 key2 in _dict[key1].Keys)
                        foreach (K3 key3 in _dict[key1][key2].Keys)
                            foreach (K4 key4 in _dict[key1][key2][key3].Keys)
                                keyList.Add(new object[] { key1, key2, key3, key4 });

                return keyList;
            }
        }

        public ICollection<T> Values
        {
            get
            {
                List<T> valueList = new List<T>();

                foreach(Dictionary<K2, Dictionary<K3, Dictionary<K4, T>>> dict2 in _dict.Values)
                    foreach(Dictionary<K3, Dictionary<K4, T>> dict3 in dict2.Values)
                        foreach(Dictionary<K4, T> dict4 in dict3.Values)
                            valueList.AddRange(dict4.Values);

                return valueList;
            }
        }

        private void AddKey(K1 key1, K2 key2, K3 key3, K4 key4, T item)
        {
            if (_dict != null)
            {
                if (!(_dict.ContainsKey(key1)))
                {
                    _dict[key1] = new Dictionary<K2,Dictionary<K3,Dictionary<K4,T>>>(_comparer2);
                }
                if (!(_dict[key1].ContainsKey(key2)))
                {
                    _dict[key1][key2] = new Dictionary<K3,Dictionary<K4,T>>(_comparer3);
                }
                if (!(_dict[key1][key2].ContainsKey(key3)))
                {
                    _dict[key1][key2][key3] = new Dictionary<K4,T>(_comparer4);
                }
                _dict[key1][key2][key3].Add(key4, item);
            }
            else if (_keyCount == _threshold)
            {
                this.CreateDictionary();
                _dict.Add(key1, new Dictionary<K2, Dictionary<K3, Dictionary<K4, T>>>(_comparer2));
                _dict[key1].Add(key2, new Dictionary<K3, Dictionary<K4, T>>(_comparer3));
                _dict[key1][key2].Add(key3, new Dictionary<K4, T>(_comparer4));
                _dict[key1][key2][key3].Add(key4, item);
            }
            else
            {
                if (this.Contains(key1, key2, key3, key4))
                {
                    throw new ArgumentException("Adding duplicate key for item");
                }
                _keyCount++;
            }
        }

        protected override void InsertItem(int index, T item)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            if (GetKeysForItem(item, out key1, out key2, out key3, out key4))
            {
                this.AddKey(key1, key2, key3, key4, item);
            }
            base.InsertItem(index, item);
        }

        protected void ChangeItemKey(T item, K1 newKey1, K2 newKey2, K3 newKey3, K4 newKey4)
        {
            if (!this.Contains(item))
            {
                throw new ArgumentException("Item does not exist for key");
            }
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            bool validKey = GetKeysForItem(item, out key1, out key2, out key3, out key4);
            if (!_comparer1.Equals(key1, newKey1)
                &&
                !_comparer2.Equals(key2, newKey2)
                &&
                !_comparer3.Equals(key3, newKey3)
                &&
                !_comparer4.Equals(key4, newKey4))
            {
                if (newKey1 != null && newKey2 != null && newKey3 != null && newKey4 != null)
                {
                    this.AddKey(newKey1, newKey2, newKey3, newKey4, item);
                }
                if (validKey)
                {
                    this.RemoveKey(key1, key2, key3, key4);
                }
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            if (_dict != null)
            {
                _dict.Clear();
            }
            _keyCount = 0;
        }

        public bool Contains(K1 key1, K2 key2, K3 key3, K4 key4)
        {
            if (key1 == null)
            {
                throw new NullReferenceException("key1");
            }
            if (key2 == null)
            {
                throw new NullReferenceException("key2");
            }
            if (key3 == null)
            {
                throw new NullReferenceException("key3");
            }
            if (key4 == null)
            {
                throw new NullReferenceException("key4");
            }
            if (_dict != null)
            {
                return ContainsKey(key1, key2, key3, key4);
            }
            if (key1 != null && key2 != null)
            {
                foreach (T local in base.Items)
                {
                    K1 localKey1;
                    K2 localKey2;
                    K3 localKey3;
                    K4 localKey4;
                    this.GetKeysForItem(local, out localKey1, out localKey2, out localKey3, out localKey4);
                    if (_comparer1.Equals(localKey1, key1)
                        &&
                        _comparer2.Equals(localKey2, key2)
                        &&
                        _comparer3.Equals(localKey3, key3)
                        &&
                        _comparer4.Equals(localKey4, key4))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public new bool Contains(T item)
        {
            if (item == null)
            {
                throw new NullReferenceException("item");
            }
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            GetKeysForItem(item, out key1, out key2, out key3, out key4);
            if (key1 == null)
            {
                throw new NullReferenceException("key1");
            }
            if (key2 == null)
            {
                throw new NullReferenceException("key2");
            }
            if (key3 == null)
            {
                throw new NullReferenceException("key3");
            }
            if (key4 == null)
            {
                throw new NullReferenceException("key4");
            }
            if (_dict != null)
            {
                return ContainsKey(key1, key2, key3, key4);
            }
            foreach (T local in base.Items)
            {
                K1 localKey1;
                K2 localKey2;
                K3 localKey3;
                K4 localKey4;
                this.GetKeysForItem(local, out localKey1, out localKey2, out localKey3, out localKey4);
                if (_comparer1.Equals(localKey1, key1)
                    &&
                    _comparer2.Equals(localKey2, key2)
                    &&
                    _comparer3.Equals(localKey3, key3)
                    &&
                    _comparer4.Equals(localKey4, key4))
                {
                    return true;
                }
            }
            return false; 
        }

        private void AddItem(K1 key1, K2 key2, K3 key3, K4 key4, T item)
        {
            if (!(_dict.ContainsKey(key1)))
            {
                _dict[key1] = new Dictionary<K2,Dictionary<K3,Dictionary<K4,T>>>(_comparer2);
            }
            if (!(_dict[key1].ContainsKey(key2)))
            {
                _dict[key1][key2] = new Dictionary<K3,Dictionary<K4,T>>(_comparer3);
            }
            if (!(_dict[key1][key2].ContainsKey(key3)))
            {
                _dict[key1][key2][key3] = new Dictionary<K4,T>(_comparer4);
            }
            _dict[key1][key2][key3].Add(key4, item);
        }

        private void CreateDictionary()
        {
            _dict = new Dictionary<K1,Dictionary<K2,Dictionary<K3,Dictionary<K4,T>>>>(_comparer1);
            foreach (T local in base.Items)
            {
                K1 key1;
                K2 key2;
                K3 key3;
                K4 key4;
                if (GetKeysForItem(local, out key1, out key2, out key3, out key4))
                {
                    AddItem(key1, key2, key3, key4, local);
                }
            }
        }

        protected abstract object[] GetKeyForItem(T item);

        /// <summary>
        /// Returns true if the GetKeyForItem is not null.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="key3"></param>
        /// <param name="key4"></param>
        /// <returns></returns>
        private bool GetKeysForItem(T item, out K1 key1, out K2 key2, out K3 key3, out K4 key4)
        {
            object[] keyForItem = this.GetKeyForItem(item);
            if (keyForItem == null)
            {
                key1 = default(K1);
                key2 = default(K2);
                key3 = default(K3);
                key4 = default(K4);
                return false;
            }
            key1 = (K1)keyForItem[0];
            key2 = (K2)keyForItem[1];
            key3 = (K3)keyForItem[2];
            key4 = (K4)keyForItem[3];
            return true;
        }

        public bool Remove(K1 key1, K2 key2, K3 key3, K4 key4)
        {
            if (key1 == null)
            {
                throw new ArgumentNullException("key1");
            }
            if (key2 == null)
            {
                throw new ArgumentNullException("key2");
            }
            if (key3 == null)
            {
                throw new ArgumentNullException("key3");
            }
            if (key4 == null)
            {
                throw new ArgumentNullException("key4");
            }
            if (_dict != null)
            {
                return (ContainsKey(key1, key2, key3, key4) && base.Remove(this[key1, key2, key3, key4]));
            }
            for (int i = 0; i < base.Items.Count; i++)
            {
                K1 k1;
                K2 k2;
                K3 k3;
                K4 k4;
                GetKeysForItem(base.Items[i], out k1, out k2, out k3, out k4);
                if (_comparer1.Equals(key1, k1)
                    &&
                    _comparer2.Equals(key2, k2)
                    &&
                    _comparer3.Equals(key3, k3)
                    &&
                    _comparer4.Equals(key4, k4))
                {
                    this.RemoveItem(i);
                    return true;
                }
            }
            return false;
        }

        protected override void RemoveItem(int index)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            GetKeysForItem(base.Items[index], out key1, out key2, out key3, out key4);
            if (key1 != null && key2 != null && key3 != null && key4 != null)
            {
                this.RemoveKey(key1, key2, key3, key4);
            }
            base.RemoveItem(index);
        }

        private void RemoveKey(K1 key1, K2 key2, K3 key3, K4 key4)
        {
            if (_dict != null)
            {
                if (_dict.ContainsKey(key1)
                    &&
                    _dict[key1].ContainsKey(key2)
                    &&
                    _dict[key1][key2].ContainsKey(key3)
                    &&
                    _dict[key1][key2][key3].ContainsKey(key4))
                {
                    _dict[key1][key2][key3].Remove(key4);
                }
            }
            else
            {
                _keyCount--;
            }
        }

        protected override void SetItem(int index, T item)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            K1 x1;
            K2 x2;
            K3 x3;
            K4 x4;
            GetKeysForItem(item, out key1, out key2, out key3, out key4);
            GetKeysForItem(base.Items[index], out x1, out x2, out x3, out x4);
            if (_comparer1.Equals(x1, key1) 
                &&
                _comparer2.Equals(x2, key2)
                &&
                _comparer3.Equals(x3, key3)
                &&
                _comparer4.Equals(x4, key4))
            {
                if ((key1 != null) && (key2 != null) && (key3 != null) && (key4 != null) && (_dict != null))
                    AddItem(key1, key2, key3, key4, item);
            }
            else
            {
                if (key1 != null && key2 != null && key3 != null && key4 != null)
                    AddItem(key1, key2, key3, key4, item);
                if (x1 != null && x2 != null && x3 != null && x4 != null)
                    this.RemoveKey(x1, x2, x3, x4);
            }
            base.SetItem(index, item);
        }

        protected Dictionary<K1, Dictionary<K2, Dictionary<K3, Dictionary<K4, T>>>> Dictionary
        {
            get
            {
                return _dict;
            }
        }

        public T this[K1 key1, K2 key2, K3 key3, K4 key4]
        {
            get
            {
                if (key1 == null)
                {
                    throw new ArgumentNullException("key1");
                }
                if (key2 == null)
                {
                    throw new ArgumentNullException("key2");
                }
                if (key3 == null)
                {
                    throw new ArgumentNullException("key3");
                }
                if (key4 == null)
                {
                    throw new ArgumentNullException("key4");
                }
                T item;
                if (_dict != null)
                {
                    if (TryGetValue(key1, key2, key3, key4, out item))
                        return item;
                }
                foreach (T local in base.Items)
                {
                    K1 k1;
                    K2 k2;
                    K3 k3;
                    K4 k4;
                    GetKeysForItem(local, out k1, out k2, out k3, out k4);
                    if (_comparer1.Equals(k1, key1) 
                        &&
                        _comparer2.Equals(k2, key2)
                        &&
                        _comparer3.Equals(k3, key3)
                        &&
                        _comparer4.Equals(k4, key4))
                    {
                        return local;
                    }
                }

                throw new KeyNotFoundException(string.Format("Key not found for {0}, {1}, {2}, {3}", key1, key2, key3, key4));
            }
        }
    }

    internal sealed class EntityCollectionDebugView<K1, K2, K3, K4, T>
    {
        // Fields
        private EntityCollection<K1, K2, K3, K4, T> kc;

        // Methods
        public EntityCollectionDebugView(EntityCollection<K1, K2, K3, K4, T> keyedCollection)
        {
            if (keyedCollection == null)
            {
                throw new ArgumentNullException("keyedCollection");
            }
            this.kc = keyedCollection;
        }

        // Properties
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.kc.Count];
                this.kc.CopyTo(array, 0);
                return array;
            }
        }
    }
    #endregion

    #region EntityCollection<K1, K2, K3, K4, K5, T>
    /// <summary>
    /// A collection of objects keyed by a composite key of five values.
    /// </summary>	
    /// <typeparam name="K1">The type for Key1</typeparam>
    /// <typeparam name="K2">The type for Key2</typeparam>
    /// <typeparam name="K3">The type for Key3</typeparam>
    /// <typeparam name="K4">The type for Key4</typeparam>
    /// <typeparam name="K5">The type for Key5</typeparam>
    /// <typeparam name="T">The type of Value</typeparam>
    [Serializable, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(EntityCollectionDebugView<,,,,>)), ComVisible(false)]
    public abstract class EntityCollection<K1, K2, K3, K4, K5, T> : EntityCollection<T>
    {
        // Fields
        private const int _defaultThreshold = 0;
        private int _keyCount;
        private int _threshold;
        private IEqualityComparer<K1> _comparer1;
        private IEqualityComparer<K2> _comparer2;
        private IEqualityComparer<K3> _comparer3;
        private IEqualityComparer<K4> _comparer4;
        private IEqualityComparer<K5> _comparer5;

        protected Dictionary<K1, Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>>> _dict;

        protected EntityCollection()
            : this(null, null, null, null, null, 0)
        {
        }

        protected EntityCollection(IEqualityComparer<K1> comparer1, IEqualityComparer<K2> comparer2, IEqualityComparer<K3> comparer3, IEqualityComparer<K4> comparer4, IEqualityComparer<K5> comparer5)
            : this(comparer1, comparer2, comparer3, comparer4, comparer5, 0)
        {
        }

        protected EntityCollection(IEqualityComparer<K1> comparer1, IEqualityComparer<K2> comparer2, IEqualityComparer<K3> comparer3, IEqualityComparer<K4> comparer4, IEqualityComparer<K5> comparer5, int dictionaryCreationThreshold)
        {
            if (comparer1 == null)
            {
                comparer1 = EqualityComparer<K1>.Default;
            }
            if (comparer2 == null)
            {
                comparer2 = EqualityComparer<K2>.Default;
            }
            if (comparer3 == null)
            {
                comparer3 = EqualityComparer<K3>.Default;
            }
            if (comparer4 == null)
            {
                comparer4 = EqualityComparer<K4>.Default;
            }
            if (comparer5 == null)
            {
                comparer5 = EqualityComparer<K5>.Default;
            }
            if (dictionaryCreationThreshold == -1)
            {
                dictionaryCreationThreshold = Int32.MaxValue;
            }
            if (dictionaryCreationThreshold < -1)
            {
                throw new ArgumentOutOfRangeException("dictionaryCreationThreshold", "The value for the dictionary creation theshold cannot be less than -1.");
            }
            _comparer1 = comparer1;
            _comparer2 = comparer2;
            _comparer3 = comparer3;
            _comparer4 = comparer4;
            _comparer5 = comparer5;
            _threshold = dictionaryCreationThreshold;
        }

        /// <summary>
        /// Comparer for key 1.
        /// </summary>
        public IEqualityComparer<K1> Comparer1 { get { return _comparer1; } }
        /// <summary>
        /// Comparer for key 2.
        /// </summary>
        public IEqualityComparer<K2> Comparer2 { get { return _comparer2; } }
        /// <summary>
        /// Comparer for key 3.
        /// </summary>
        public IEqualityComparer<K3> Comparer3 { get { return _comparer3; } }
        /// <summary>
        /// Comparer for key 4.
        /// </summary>
        public IEqualityComparer<K4> Comparer4 { get { return _comparer4; } }
        /// <summary>
        /// Comparer for key 5.
        /// </summary>
        public IEqualityComparer<K5> Comparer5 { get { return _comparer5; } }

        public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, out T value)
        {
            value = default(T);

            Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>> dict2;
            Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>> dict3;
            Dictionary<K4, Dictionary<K5, T>> dict4;
            Dictionary<K5, T> dict5;

            if (_dict == null)
                return false;

            if (_dict.TryGetValue(key1, out dict2))
            {
                if (dict2.TryGetValue(key2, out dict3))
                    if (dict3.TryGetValue(key3, out dict4))
                        if(dict4.TryGetValue(key4, out dict5))
                            return dict5.TryGetValue(key5, out value);
            }

            return false;
        }

        public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5)
        {
            T unused;

            return TryGetValue(key1, key2, key3, key4, key5, out unused);
        }

        public ICollection<object[]> Keys
        {
            get
            {
                List<object[]> keyList = new List<object[]>();

                foreach (K1 key1 in _dict.Keys)
                    foreach (K2 key2 in _dict[key1].Keys)
                        foreach (K3 key3 in _dict[key1][key2].Keys)
                            foreach (K4 key4 in _dict[key1][key2][key3].Keys)
                                foreach (K5 key5 in _dict[key1][key2][key3][key4].Keys)
                                    keyList.Add(new object[] { key1, key2, key3, key4, key5 });

                return keyList;
            }
        }

        public ICollection<T> Values
        {
            get
            {
                List<T> valueList = new List<T>();

                foreach (Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>> dict2 in _dict.Values)
                    foreach (Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>> dict3 in dict2.Values)
                        foreach (Dictionary<K4, Dictionary<K5, T>> dict4 in dict3.Values)
                            foreach (Dictionary<K5, T> dict5 in dict4.Values)
                                valueList.AddRange(dict5.Values);

                return valueList;
            }
        }

        private void AddKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, T item)
        {
            if (_dict != null)
            {
                if (!(_dict.ContainsKey(key1)))
                {
                    _dict[key1] = new Dictionary<K2,Dictionary<K3,Dictionary<K4,Dictionary<K5,T>>>>(_comparer2);
                }
                if (!(_dict[key1].ContainsKey(key2)))
                {
                    _dict[key1][key2] = new Dictionary<K3,Dictionary<K4,Dictionary<K5,T>>>(_comparer3);
                }
                if (!(_dict[key1][key2].ContainsKey(key3)))
                {
                    _dict[key1][key2][key3] = new Dictionary<K4,Dictionary<K5,T>>(_comparer4);
                }
                if (!(_dict[key1][key2][key3].ContainsKey(key4)))
                {
                    _dict[key1][key2][key3][key4] = new Dictionary<K5,T>(_comparer5);
                }
                _dict[key1][key2][key3][key4].Add(key5, item);
            }
            else if (_keyCount == _threshold)
            {
                this.CreateDictionary();
                _dict.Add(key1, new Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>>(_comparer2));
                _dict[key1].Add(key2, new Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>(_comparer3));
                _dict[key1][key2].Add(key3, new Dictionary<K4, Dictionary<K5, T>>(_comparer4));
                _dict[key1][key2][key3].Add(key4, new Dictionary<K5, T>(_comparer5));
                _dict[key1][key2][key3][key4].Add(key5, item);
            }
            else
            {
                if (this.Contains(key1, key2, key3, key4, key5))
                {
                    throw new ArgumentException("Adding duplicate key for item");
                }
                _keyCount++;
            }
        }

        protected override void InsertItem(int index, T item)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            K5 key5;
            if (GetKeysForItem(item, out key1, out key2, out key3, out key4, out key5))
            {
                this.AddKey(key1, key2, key3, key4, key5, item);
            }
            base.InsertItem(index, item);
        }

        protected void ChangeItemKey(T item, K1 newKey1, K2 newKey2, K3 newKey3, K4 newKey4, K5 newKey5)
        {
            if (!this.Contains(item))
            {
                throw new ArgumentException("Item does not exist for key");
            }
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            K5 key5;
            bool validKey = GetKeysForItem(item, out key1, out key2, out key3, out key4, out key5);
            if (!_comparer1.Equals(key1, newKey1)
                &&
                !_comparer2.Equals(key2, newKey2)
                &&
                !_comparer3.Equals(key3, newKey3)
                &&
                !_comparer4.Equals(key4, newKey4)
                &&
                !_comparer5.Equals(key5, newKey5))
            {
                if (newKey1 != null && newKey2 != null && newKey3 != null && newKey4 != null && newKey5 != null)
                {
                    this.AddKey(newKey1, newKey2, newKey3, newKey4, newKey5, item);
                }
                if (validKey)
                {
                    this.RemoveKey(key1, key2, key3, key4, key5);
                }
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            if (_dict != null)
            {
                _dict.Clear();
            }
            _keyCount = 0;
        }

        public bool Contains(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5)
        {
            if (key1 == null)
            {
                throw new NullReferenceException("key1");
            }
            if (key2 == null)
            {
                throw new NullReferenceException("key2");
            }
            if (key3 == null)
            {
                throw new NullReferenceException("key3");
            }
            if (key4 == null)
            {
                throw new NullReferenceException("key4");
            }
            if (key5 == null)
            {
                throw new NullReferenceException("key5");
            }
            if (_dict != null)
            {
                return ContainsKey(key1, key2, key3, key4, key5);
            }
            if (key1 != null && key2 != null)
            {
                foreach (T local in base.Items)
                {
                    K1 localKey1;
                    K2 localKey2;
                    K3 localKey3;
                    K4 localKey4;
                    K5 localKey5;
                    this.GetKeysForItem(local, out localKey1, out localKey2, out localKey3, out localKey4, out localKey5);
                    if (_comparer1.Equals(localKey1, key1)
                        &&
                        _comparer2.Equals(localKey2, key2)
                        &&
                        _comparer3.Equals(localKey3, key3)
                        &&
                        _comparer4.Equals(localKey4, key4)
                        &&
                        _comparer5.Equals(localKey5, key5))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public new bool Contains(T item)
        {
            if (item == null)
            {
                throw new NullReferenceException("item");
            }
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            K5 key5;
            GetKeysForItem(item, out key1, out key2, out key3, out key4, out key5);
            if (key1 == null)
            {
                throw new NullReferenceException("key1");
            }
            if (key2 == null)
            {
                throw new NullReferenceException("key2");
            }
            if (key3 == null)
            {
                throw new NullReferenceException("key3");
            }
            if (key4 == null)
            {
                throw new NullReferenceException("key4");
            }
            if (key5 == null)
            {
                throw new NullReferenceException("key5");
            } 
            if (_dict != null)
            {
                return ContainsKey(key1, key2, key3, key4, key5);
            }
            foreach (T local in base.Items)
            {
                K1 localKey1;
                K2 localKey2;
                K3 localKey3;
                K4 localKey4;
                K5 localKey5;
                this.GetKeysForItem(local, out localKey1, out localKey2, out localKey3, out localKey4, out localKey5);
                if (_comparer1.Equals(localKey1, key1)
                    &&
                    _comparer2.Equals(localKey2, key2)
                    &&
                    _comparer3.Equals(localKey3, key3)
                    &&
                    _comparer4.Equals(localKey4, key4)
                    &&
                    _comparer5.Equals(localKey5, key5))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddItem(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, T item)
        {
            if (!(_dict.ContainsKey(key1)))
            {
                _dict[key1] = new Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>>(_comparer2);
            }
            if (!(_dict[key1].ContainsKey(key2)))
            {
                _dict[key1][key2] = new Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>(_comparer3);
            }
            if (!(_dict[key1][key2].ContainsKey(key3)))
            {
                _dict[key1][key2][key3] = new Dictionary<K4, Dictionary<K5, T>>(_comparer4);
            }
            if (!(_dict[key1][key2][key3].ContainsKey(key4)))
            {
                _dict[key1][key2][key3][key4] = new Dictionary<K5, T>(_comparer5);
            }
            _dict[key1][key2][key3][key4].Add(key5, item);
        }

        private void CreateDictionary()
        {
            _dict = new Dictionary<K1, Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>>>(_comparer1);
            foreach (T local in base.Items)
            {
                K1 key1;
                K2 key2;
                K3 key3;
                K4 key4;
                K5 key5;
                if (GetKeysForItem(local, out key1, out key2, out key3, out key4, out key5))
                {
                    AddItem(key1, key2, key3, key4, key5, local);
                }
            }
        }

        protected abstract object[] GetKeyForItem(T item);

        /// <summary>
        /// Returns true if the GetKeyForItem is not null.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="key3"></param>
        /// <param name="key4"></param>
        /// <param name="key5"></param>
        /// <returns></returns>
        private bool GetKeysForItem(T item, out K1 key1, out K2 key2, out K3 key3, out K4 key4, out K5 key5)
        {
            object[] keyForItem = this.GetKeyForItem(item);
            if (keyForItem == null)
            {
                key1 = default(K1);
                key2 = default(K2);
                key3 = default(K3);
                key4 = default(K4);
                key5 = default(K5);
                return false;
            }
            key1 = (K1)keyForItem[0];
            key2 = (K2)keyForItem[1];
            key3 = (K3)keyForItem[2];
            key4 = (K4)keyForItem[3];
            key5 = (K5)keyForItem[4];
            return true;
        }

        public bool Remove(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5)
        {
            if (key1 == null)
            {
                throw new ArgumentNullException("key1");
            }
            if (key2 == null)
            {
                throw new ArgumentNullException("key2");
            }
            if (key3 == null)
            {
                throw new ArgumentNullException("key3");
            }
            if (key4 == null)
            {
                throw new ArgumentNullException("key4");
            }
            if (key5 == null)
            {
                throw new ArgumentNullException("key5");
            }
            if (_dict != null)
            {
                return (ContainsKey(key1, key2, key3, key4, key5) && base.Remove(this[key1, key2, key3, key4, key5]));
            }
            for (int i = 0; i < base.Items.Count; i++)
            {
                K1 k1;
                K2 k2;
                K3 k3;
                K4 k4;
                K5 k5;
                GetKeysForItem(base.Items[i], out k1, out k2, out k3, out k4, out k5);
                if (_comparer1.Equals(key1, k1)
                    &&
                    _comparer2.Equals(key2, k2)
                    &&
                    _comparer3.Equals(key3, k3)
                    &&
                    _comparer4.Equals(key4, k4)
                    &&
                    _comparer5.Equals(key5, k5))
                {
                    this.RemoveItem(i);
                    return true;
                }
            }
            return false;
        }

        protected override void RemoveItem(int index)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            K5 key5;
            GetKeysForItem(base.Items[index], out key1, out key2, out key3, out key4, out key5);
            if (key1 != null && key2 != null && key3 != null && key4 != null && key5 != null)
            {
                this.RemoveKey(key1, key2, key3, key4, key5);
            }
            base.RemoveItem(index);
        }

        private void RemoveKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5)
        {
            if (_dict != null)
            {
                if (_dict.ContainsKey(key1)
                    &&
                    _dict[key1].ContainsKey(key2)
                    &&
                    _dict[key1][key2].ContainsKey(key3)
                    &&
                    _dict[key1][key2][key3].ContainsKey(key4)
                    &&
                    _dict[key1][key2][key3][key4].ContainsKey(key5))
                {
                    _dict[key1][key2][key3][key4].Remove(key5);
                }
            }
            else
            {
                _keyCount--;
            }
        }

        protected override void SetItem(int index, T item)
        {
            K1 key1;
            K2 key2;
            K3 key3;
            K4 key4;
            K5 key5;
            K1 x1;
            K2 x2;
            K3 x3;
            K4 x4;
            K5 x5;
            GetKeysForItem(item, out key1, out key2, out key3, out key4, out key5);
            GetKeysForItem(base.Items[index], out x1, out x2, out x3, out x4, out x5);
            if (_comparer1.Equals(x1, key1)
                &&
                _comparer2.Equals(x2, key2)
                &&
                _comparer3.Equals(x3, key3)
                &&
                _comparer4.Equals(x4, key4)
                &&
                _comparer5.Equals(x5, key5))
            {
                if ((key1 != null) && (key2 != null) && (key3 != null) && (key4 != null) && (key5 != null) && (_dict != null))
                    AddItem(key1, key2, key3, key4, key5, item);
            }
            else
            {
                if (key1 != null && key2 != null && key3 != null && key4 != null && key5 != null)
                    AddItem(key1, key2, key3, key4, key5, item);
                if (x1 != null && x2 != null && x3 != null && x4 != null && x3 != null)
                    this.RemoveKey(x1, x2, x3, x4, x5);
            }
            base.SetItem(index, item);
        }

        protected Dictionary<K1, Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, T>>>>> Dictionary
        {
            get
            {
                return _dict;
            }
        }

        public T this[K1 key1, K2 key2, K3 key3, K4 key4, K5 key5]
        {
            get
            {
                if (key1 == null)
                {
                    throw new ArgumentNullException("key1");
                }
                if (key2 == null)
                {
                    throw new ArgumentNullException("key2");
                }
                if (key3 == null)
                {
                    throw new ArgumentNullException("key3");
                }
                if (key4 == null)
                {
                    throw new ArgumentNullException("key4");
                }
                if (key5 == null)
                {
                    throw new ArgumentNullException("key5");
                }
                T item;
                if (_dict != null)
                {
                    if (TryGetValue(key1, key2, key3, key4, key5, out item))
                        return item;
                }
                foreach (T local in base.Items)
                {
                    K1 k1;
                    K2 k2;
                    K3 k3;
                    K4 k4;
                    K5 k5;
                    GetKeysForItem(local, out k1, out k2, out k3, out k4, out k5);
                    if (_comparer1.Equals(k1, key1)
                        &&
                        _comparer2.Equals(k2, key2)
                        &&
                        _comparer3.Equals(k3, key3)
                        &&
                        _comparer4.Equals(k4, key4)
                        &&
                        _comparer5.Equals(k5, key5))
                    {
                        return local;
                    }
                }

                throw new KeyNotFoundException(string.Format("Key not found for {0}, {1}, {2}, {3}", key1, key2, key3, key4));
            }
        }
    }

    internal sealed class EntityCollectionDebugView<K1, K2, K3, K4, K5, T>
    {
        // Fields
        private EntityCollection<K1, K2, K3, K4, K5, T> kc;

        // Methods
        public EntityCollectionDebugView(EntityCollection<K1, K2, K3, K4, K5, T> keyedCollection)
        {
            if (keyedCollection == null)
            {
                throw new ArgumentNullException("keyedCollection");
            }
            this.kc = keyedCollection;
        }

        // Properties
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.kc.Count];
                this.kc.CopyTo(array, 0);
                return array;
            }
        }
    }
    #endregion

    internal sealed class FunctorComparer<T> : IComparer<T>
    {
        // Fields
        private Comparer<T> c;
        private Comparison<T> comparison;

        // Methods
        public FunctorComparer(Comparison<T> comparison)
        {
            this.c = Comparer<T>.Default;
            this.comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return this.comparison(x, y);
        }
    }
}

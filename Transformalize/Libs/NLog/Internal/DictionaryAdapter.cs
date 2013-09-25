#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Provides untyped IDictionary interface on top of generic IDictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    internal class DictionaryAdapter<TKey, TValue> : IDictionary
    {
        private readonly IDictionary<TKey, TValue> implementation;

        /// <summary>
        ///     Initializes a new instance of the DictionaryAdapter class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        public DictionaryAdapter(IDictionary<TKey, TValue> implementation)
        {
            this.implementation = implementation;
        }

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.ICollection" /> object containing the values in the
        ///     <see
        ///         cref="T:System.Collections.IDictionary" />
        ///     object.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     An <see cref="T:System.Collections.ICollection" /> object containing the values in the
        ///     <see
        ///         cref="T:System.Collections.IDictionary" />
        ///     object.
        /// </returns>
        public ICollection Values
        {
            get { return new List<TValue>(implementation.Values); }
        }

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     The number of elements contained in the <see cref="T:System.Collections.ICollection" />.
        /// </returns>
        public int Count
        {
            get { return implementation.Count; }
        }

        /// <summary>
        ///     Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.
        /// </returns>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </returns>
        public object SyncRoot
        {
            get { return implementation; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.IDictionary" /> object has a fixed size; otherwise, false.
        /// </returns>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.IDictionary" /> object is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return implementation.IsReadOnly; }
        }

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.ICollection" /> object containing the keys of the
        ///     <see
        ///         cref="T:System.Collections.IDictionary" />
        ///     object.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     An <see cref="T:System.Collections.ICollection" /> object containing the keys of the
        ///     <see
        ///         cref="T:System.Collections.IDictionary" />
        ///     object.
        /// </returns>
        public ICollection Keys
        {
            get { return new List<TKey>(implementation.Keys); }
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.Object" /> with the specified key.
        /// </summary>
        /// <param name="key">Dictionary key.</param>
        /// <returns>Value corresponding to key or null if not found</returns>
        public object this[object key]
        {
            get
            {
                TValue value;

                if (implementation.TryGetValue((TKey) key, out value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }

            set { implementation[(TKey) key] = (TValue) value; }
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        /// <param name="key">
        ///     The <see cref="T:System.Object" /> to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        ///     The <see cref="T:System.Object" /> to use as the value of the element to add.
        /// </param>
        public void Add(object key, object value)
        {
            implementation.Add((TKey) key, (TValue) value);
        }

        /// <summary>
        ///     Removes all elements from the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        public void Clear()
        {
            implementation.Clear();
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.IDictionary" /> object contains an element with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to locate in the <see cref="T:System.Collections.IDictionary" /> object.
        /// </param>
        /// <returns>
        ///     True if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, false.
        /// </returns>
        public bool Contains(object key)
        {
            return implementation.ContainsKey((TKey) key);
        }

        /// <summary>
        ///     Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the
        ///     <see
        ///         cref="T:System.Collections.IDictionary" />
        ///     object.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the
        ///     <see
        ///         cref="T:System.Collections.IDictionary" />
        ///     object.
        /// </returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return new MyEnumerator(implementation.GetEnumerator());
        }

        /// <summary>
        ///     Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public void Remove(object key)
        {
            implementation.Remove((TKey) key);
        }

        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular
        ///     <see
        ///         cref="T:System.Array" />
        ///     index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from
        ///     <see
        ///         cref="T:System.Collections.ICollection" />
        ///     . The <see cref="T:System.Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">
        ///     The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Wrapper IDictionaryEnumerator.
        /// </summary>
        private class MyEnumerator : IDictionaryEnumerator
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> wrapped;

            /// <summary>
            ///     Initializes a new instance of the <see cref="MyEnumerator" /> class.
            /// </summary>
            /// <param name="wrapped">The wrapped.</param>
            public MyEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> wrapped)
            {
                this.wrapped = wrapped;
            }

            /// <summary>
            ///     Gets both the key and the value of the current dictionary entry.
            /// </summary>
            /// <value></value>
            /// <returns>
            ///     A <see cref="T:System.Collections.DictionaryEntry" /> containing both the key and the value of the current dictionary entry.
            /// </returns>
            public DictionaryEntry Entry
            {
                get { return new DictionaryEntry(wrapped.Current.Key, wrapped.Current.Value); }
            }

            /// <summary>
            ///     Gets the key of the current dictionary entry.
            /// </summary>
            /// <value></value>
            /// <returns>
            ///     The key of the current element of the enumeration.
            /// </returns>
            public object Key
            {
                get { return wrapped.Current.Key; }
            }

            /// <summary>
            ///     Gets the value of the current dictionary entry.
            /// </summary>
            /// <value></value>
            /// <returns>
            ///     The value of the current element of the enumeration.
            /// </returns>
            public object Value
            {
                get { return wrapped.Current.Value; }
            }

            /// <summary>
            ///     Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>
            ///     The current element in the collection.
            /// </returns>
            public object Current
            {
                get { return Entry; }
            }

            /// <summary>
            ///     Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            ///     True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            public bool MoveNext()
            {
                return wrapped.MoveNext();
            }

            /// <summary>
            ///     Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                wrapped.Reset();
            }
        }
    }
}
//-----------------------------------------------------------------------------
// ObservableOrderedDictionary.cs
// Author: Jay Carlson
//
// This is a mishmash of System.ServiceModels.Internal.OrderedDictionary<TKey, TValue>
// and ObservableCollection.cs
//
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
    /// <summary>
    /// Implementation of a dynamic data collection based on generic dictionary implementing INotifyCollectionChanged to 
    /// notify listeners when items get added, removed, or the whole dictionary is refreshed.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys</typeparam>
    /// <typeparam name="TValue">The type of the values</typeparam>
    /// <remarks>
    /// <para>
    /// ObservableOrderedDictionary provides an ObservableCollection-style interface with a Dictionary backend. 
    /// Unlike the regular Dictionary&lt;TKey, TValue&gt;, ObservableDictionary preserves element order.
    /// </para>
    /// <example>
    /// <para>
    /// This class is especially useful for storing arrays of data and ViewModels in a WPF data binding scenario
    /// when the application still needs immediate access to specific references.
    /// </para>
    /// <para>
    /// Consider the Geomagic Touch plug-in, which has several signal sources that all need a view model. With the ObservableCollection
    /// approach, the app would need specific references for each of those signal sources.
    /// </para>
    /// <code>
    /// SignalSourceViewModel X = new SignalSourceViewModel("X Position");
    /// SignalSourceViewModel Y = new SignalSourceViewModel("Y Position");
    /// SignalSourceViewModel Z = new SignalSourceViewModel("Z Position");
    /// ...
    /// 
    /// ObservableCollection&lt;SignalSourceViewModel&gt; SignalSources = new ObservableCollection&lt;SignalSourceViewModel&gt;();
    /// 
    /// ...
    /// X.SendValue(device.X);
    /// ...
    /// </code>
    /// <para>
    /// With ObservableDictionary, no variables need to be created to store local references:
    /// </para>
    /// <code>
    /// SignalSources = new ObservableDictionary&lt;string, SignalSourceViewModel&gt;();
    /// 
    /// SignalSources.Add("X", new SignalSourceViewModel("X Position"));
    /// SignalSources.Add("Y", new SignalSourceViewModel("Y Position"));
    /// SignalSources.Add("Z", new SignalSourceViewModel("Z Position"));
    /// ...
    /// 
    /// SignalSources["X"].SendValue(device.X);
    /// </code>
    /// </example>
    /// </remarks>
    [DataContract]
    [Serializable]
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
    {
        [DataMember]
        OrderedDictionary privateDictionary;

        public ObservableDictionary()
        {
            this.privateDictionary = new OrderedDictionary();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary != null)
            {
                this.privateDictionary = new OrderedDictionary();

                foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                {
                    this.privateDictionary.Add(pair.Key, pair.Value);
                }
            }
        }

        public int Count
        {
            get
            {
                return this.privateDictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                if (this.privateDictionary.Contains(key))
                {
                    return (TValue)this.privateDictionary[(object)key];
                }
                else
                {
                    throw new Exception("Key not found in dictionary");
                }
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                TValue originalItem = this[key];
                int i;
                IDictionaryEnumerator enumerator = privateDictionary.GetEnumerator();
                enumerator.MoveNext();
                for(i=0;i<privateDictionary.Count; i++)
                {
                    if(enumerator.Key == (object)key)
                    {
                        break;
                    }
                    enumerator.MoveNext();
                }
                this.privateDictionary[(object)key] = value;

                OnPropertyChanged(IndexerName);
                //OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, (object)value, i);
                OnCollectionReset();

            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                List<TKey> keys = new List<TKey>(this.privateDictionary.Count);

                foreach (TKey key in this.privateDictionary.Keys)
                {
                    keys.Add(key);
                }

                // Keys should be put in a ReadOnlyCollection,
                // but since this is an internal class, for performance reasons,
                // we choose to avoid creating yet another collection.

                return keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> values = new List<TValue>(this.privateDictionary.Count);

                foreach (TValue value in this.privateDictionary.Values)
                {
                    values.Add(value);
                }

                // Values should be put in a ReadOnlyCollection,
                // but since this is an internal class, for performance reasons,
                // we choose to avoid creating yet another collection.

                return values;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            this.privateDictionary.Add(key, value);
           

            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, (object)value, -1);
        }

        public void Clear()
        {
            this.privateDictionary.Clear();
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
            OnCollectionReset();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null || !this.privateDictionary.Contains(item.Key))
            {
                return false;
            }
            else
            {
                return this.privateDictionary[(object)item.Key].Equals(item.Value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return this.privateDictionary.Contains(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (array.Rank > 1 || arrayIndex >= array.Length || array.Length - arrayIndex < this.privateDictionary.Count)
            {
                throw new Exception("This operation is invalid for the current dictionary");
            }

            int index = arrayIndex;
            foreach (DictionaryEntry entry in this.privateDictionary)
            {
                array[index] = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
                index++;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (DictionaryEntry entry in this.privateDictionary)
            {
                yield return new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                this.privateDictionary.Remove(item.Key);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                //throw Fx.Exception.ArgumentNull("key");
            }

            if (this.privateDictionary.Contains(key))
            {
                TValue removedItem = this[key];

                this.privateDictionary.Remove(key);

                OnPropertyChanged(CountString);
                OnPropertyChanged(IndexerName);
                OnPropertyChanged("Keys");
                OnPropertyChanged("Values");
                //OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, -1);
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                //throw Fx.Exception.ArgumentNull("key");
            }

            bool keyExists = this.privateDictionary.Contains(key);
            value = keyExists ? (TValue)this.privateDictionary[(object)key] : default(TValue);

            return keyExists;
        }

        void IDictionary.Add(object key, object value)
        {
            this.privateDictionary.Add(key, value);
        }

        void IDictionary.Clear()
        {
            this.privateDictionary.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return this.privateDictionary.Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return this.privateDictionary.GetEnumerator();
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return ((IDictionary)this.privateDictionary).IsFixedSize;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return this.privateDictionary.IsReadOnly;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return this.privateDictionary.Keys;
            }
        }

        void IDictionary.Remove(object key)
        {
            this.privateDictionary.Remove(key);
        }

        ICollection IDictionary.Values
        {
            get
            {
                return this.privateDictionary.Values;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this.privateDictionary[key];
            }
            set
            {
                this.privateDictionary[key] = value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.privateDictionary.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get
            {
                return this.privateDictionary.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)this.privateDictionary).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)this.privateDictionary).SyncRoot;
            }
        }

        [field: NonSerializedAttribute()] 
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        [field: NonSerializedAttribute()] 
        public event PropertyChangedEventHandler PropertyChanged;

        private const string CountString = "Count";

        private const string IndexerName = "Item[]";

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, args);
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if(CollectionChanged != null)
                CollectionChanged(this, args);
        }

        /// <summary>
        /// Helper to raise a PropertyChanged event  />).
        /// </summary>
        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Helper to raise CollectionChanged event with action == Reset to any listeners
        /// </summary>
        private void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        #endregion Private Methods
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace RobotControl
{
    public class Robot : INotifyPropertyChanged
    {
        byte[] DataToSend;
        byte[] ResponseBuffer = new byte[32];

        bool waitingForResponse = false;

        AutoResetEvent autoEvent = new AutoResetEvent(false);
        object commLock = new object();

        bool IsAssociating { get; set; }
        int AssocNumJoints = 0;
        public ObservableConcurrentDictionary<uint, Controller> Controllers { get; set; }
        public ObservableConcurrentDictionary<uint, Controller> SerialControllers { get; set; }
        //public ObservableConcurrentDictionary<uint, Motor> Motors { get; set; }

        public ObservableCollection<Motor> Motors;
        public ObservableCollection<Motor> SerialMotors;

        public System.Timers.Timer setpointTimer;

        [IgnoreDataMember()]
        private IPacketTransport com;
        [IgnoreDataMember()]
        public IPacketTransport Com
        {
            get { return com; }
            set {
                if(com != value)
                {
                    if(com != null)
                        com.DataReceived -= Com_DataReceived;

                    com = value; 
                    com.DataReceived += Com_DataReceived;
                    com.UpdateSetpoints += Com_UpdateSetpoints;
                    Com_UpdateSetpoints();
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Com"));
                }
            }
        }

        public Robot()
        {
            Controllers = new ObservableConcurrentDictionary<uint, Controller>();
            Motors = new ObservableCollection<Motor>();
            SerialMotors = new ObservableCollection<Motor>();
            SerialControllers = new ObservableConcurrentDictionary<uint, Controller>();
            setpointTimer = new System.Timers.Timer(19);
            setpointTimer.Elapsed += setpointTimer_Elapsed;
            //setpointTimer.Start();
            //StartUpdates();
        }

        public void StartUpdates()
        {
            if(Com != null)
                Com.RequestData = true;
        }

        public void StopUpdates()
        {
            if(Com != null)
                Com.RequestData = false;
        }

        void setpointTimer_Elapsed(object sender, EventArgs e)
        {
            if(Com != null)
            {
                if (this.Com.SendData && (Controllers != null))
                {
                    //SerialMotors = Motors;
                    SerialControllers = Controllers;
                    this.Com.SendData = false;
                }
            }
        }

        void Com_DataReceived(byte[] data)
        {
            // This is a hack to deal with the discover messages
            uint address = BitConverter.ToUInt32(data, 1);
            JointCommands response = (JointCommands)data[5];
            switch(response)
            {
                case JointCommands.Associate:
                    if (!Controllers.ContainsKey(address))
                    {
                        Controllers.Add(address, new Controller() { Robot = this, Id = address });
                        if (Controllers.Count() >= AssocNumJoints)
                            IsAssociating = false;
                        Com_UpdateSetpoints();
                        //Com.AddAddress(address);
                    }
                    else
                        return;
                    break;
                case JointCommands.GetStatus:
                    //Debug.WriteLine("Received response: " + new SoapHexBinary(ResponseBuffer).ToString());
                    foreach (var joint in Controllers)
                    {
                        if (joint.Value.Id == address)
                        {
                            joint.Value.SetStatus(data);
                            return;
                        }
                    }
                    break;
                case JointCommands.GetHallPos:
                    foreach (var joint in Controllers)
                    {
                        if (joint.Value.Id == address)
                        {
                            joint.Value.SetHallPos(data);
                            return;
                        }
                    }
                    break;
                case JointCommands.GetPots:
                    foreach (var joint in Controllers)
                    {
                        if (joint.Value.Id == address)
                        {
                            joint.Value.SetPots(data);
                            return;
                        }
                    }
                    break;
                case JointCommands.GetCurrent:
                    foreach (var joint in Controllers)
                    {
                        if (joint.Value.Id == address)
                        {
                            joint.Value.SetCurrent(data);
                            return;
                        }
                    }
                    break;
            }
        }

        public void Com_UpdateSetpoints()
        {
            if (Com != null)
            {
                if (this.Com.SendData && (Controllers != null))
                {
                    //SerialMotors = Motors;
                    SerialControllers = Controllers;
                    this.Com.SendData = false;
                }
            }
        }

        public void SendCommand(JointCommands command, Controller joint = null, byte[] payload = null)
        {
            //lock(commLock)
            //{
                if (Com == null)
                    return;
                int numBytesToSend = 6; // number of bytes (1) + joint address (4) + command (1)
                if (payload != null)
                    numBytesToSend += payload.Length;

                byte[] buffer = new byte[numBytesToSend];
                uint address;
                if (joint == null)
                {
                    address = 0; // broadcast mode
                }
                else
                {
                    address = joint.Id;
                }
                // Number of bytes in payload
                buffer[0] = (byte)numBytesToSend;
                // Address
                byte[] addressBytes = BitConverter.GetBytes(address);
                // We don't really care if our int addresses are "correct" or not, since they're just bits.
                //if(BitConverter.IsLittleEndian)
                //   addressBytes.Reverse();
                Array.Copy(addressBytes, 0, buffer, 1, 4);

                // Command
                buffer[5] = (byte)command;

                if (payload != null)
                {
                    Array.Copy(payload, 0, buffer, 6, payload.Length);
                }
                //Debug.WriteLine("Sending: " + new SoapHexBinary(buffer).ToString());
                Com.Send(buffer);
            //}
        }
        /// <summary>
        /// This command discovers all the joints
        /// </summary>
        /// <param name="numJoints">The number of joints to wait for before quitting</param>
        public void DiscoverControllers()
        {
            SendCommand(JointCommands.Associate);
        }

        internal void Home(uint p, int index)
        {
            
        }
        
        [field: NonSerializedAttribute()] 
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SynchronizedObservableCollection<T> : IDisposable, IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Constructor
        public SynchronizedObservableCollection()
        {
            _context = SynchronizationContext.Current;
            _items = new List<T>();
        }
        #endregion

        #region Private Fields
        private readonly SynchronizationContext _context;
        private readonly IList<T> _items;
        private readonly ReaderWriterLockSlim _itemsLock = new ReaderWriterLockSlim();
        private readonly object _lock = new object();
        [NonSerialized]
        private Object _syncRoot;

        private readonly SimpleMonitor _monitor = new SimpleMonitor();
        #endregion

        #region Events
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Private Properties
        bool IList.IsFixedSize
        {
            get
            {
                //TODO: Do I need a lock here?
                // There is no IList<T>.IsFixedSize, so we must assume that only
                // readonly collections are fixed size, if our internal item 
                // collection does not implement IList.  Note that Array implements
                // IList, and therefore T[] and U[] will be fixed-size.
                var list = _items as IList;
                if (list != null)
                {
                    return list.IsFixedSize;
                }

                return _items.IsReadOnly;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        bool IList.IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        //TODO: Does this mean what I think it does?
        bool ICollection.IsSynchronized
        {
            get { return true; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    _itemsLock.EnterReadLock();

                    try
                    {
                        var c = _items as ICollection;
                        if (c != null)
                        {
                            _syncRoot = c.SyncRoot;
                        }
                        else
                        {
                            Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                        }
                    }
                    finally
                    {
                        _itemsLock.ExitReadLock();
                    }
                }

                return _syncRoot;
            }
        }
        #endregion

        #region Public Properties
        public int Count
        {
            get
            {
                _itemsLock.EnterReadLock();

                try
                {
                    return _items.Count;
                }
                finally
                {
                    _itemsLock.ExitReadLock();
                }
            }
        }

        public T this[int index]
        {
            get
            {
                _itemsLock.EnterReadLock();

                try
                {
                    CheckIndex(index);

                    return _items[index];
                }
                finally
                {
                    _itemsLock.ExitReadLock();
                }
            }
            set
            {
                T oldValue;

                _itemsLock.EnterWriteLock();

                try
                {
                    CheckIsReadOnly();
                    CheckIndex(index);
                    CheckReentrancy();

                    oldValue = this[index];

                    _items[index] = value;

                }
                finally
                {
                    _itemsLock.ExitWriteLock();
                }

                OnPropertyChanged("Item[]");
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldValue, value, index);
            }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("'value' is the wrong type");
                }
            }
        }

        #endregion

        #region Private Methods
        private IDisposable BlockReentrancy()
        {
            _monitor.Enter();

            return _monitor;
        }

        // ReSharper disable once UnusedParameter.Local
        private void CheckIndex(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckIsReadOnly()
        {
            if (_items.IsReadOnly)
            {
                throw new NotSupportedException("Collection is readonly");
            }
        }

        private void CheckReentrancy()
        {
            if (_monitor.Busy && CollectionChanged != null && CollectionChanged.GetInvocationList().Length > 1)
            {
                throw new InvalidOperationException("SynchronizedObservableCollection reentrancy not allowed");
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
            return ((value is T) || (value == null && default(T) == null));
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var collectionChanged = CollectionChanged;
            if (collectionChanged == null)
            {
                return;
            }

            using (BlockReentrancy())
            {
                _context.Send(state => collectionChanged(this, e), null);
            }
        }

        private void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }

            _context.Send(state => propertyChanged(this, e), null);
        }
        #endregion

        #region Public Methods
        public void Add(T item)
        {
            _itemsLock.EnterWriteLock();

            var index = _items.Count;

            try
            {
                CheckIsReadOnly();
                CheckReentrancy();

                _items.Insert(index, item);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        int IList.Add(object value)
        {
            _itemsLock.EnterWriteLock();

            var index = _items.Count;
            T item;

            try
            {
                CheckIsReadOnly();
                CheckReentrancy();

                item = (T)value;

                _items.Insert(index, item);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("'value' is the wrong type");
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);

            return index;
        }

        public void Clear()
        {
            _itemsLock.EnterWriteLock();

            try
            {
                CheckIsReadOnly();
                CheckReentrancy();

                _items.Clear();
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionReset();
        }

        public void CopyTo(T[] array, int index)
        {
            _itemsLock.EnterReadLock();

            try
            {
                _items.CopyTo(array, index);
            }
            finally
            {
                _itemsLock.ExitReadLock();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            _itemsLock.EnterReadLock();

            try
            {
                if (array == null)
                {
                    throw new ArgumentNullException("array", "'array' cannot be null");
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Multidimension arrays are not supported", "array");
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException("Non-zero lower bound arrays are not supported", "array");
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("index", "'index' is out of range");
                }

                if (array.Length - index < _items.Count)
                {
                    throw new ArgumentException("Array is too small");
                }

                var tArray = array as T[];
                if (tArray != null)
                {
                    _items.CopyTo(tArray, index);
                }
                else
                {
                    //
                    // Catch the obvious case assignment will fail.
                    // We can found all possible problems by doing the check though.
                    // For example, if the element type of the Array is derived from T,
                    // we can't figure out if we can successfully copy the element beforehand.
                    //
                    var targetType = array.GetType().GetElementType();
                    var sourceType = typeof(T);
                    if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                    {
                        throw new ArrayTypeMismatchException("Invalid array type");
                    }

                    //
                    // We can't cast array of value type to object[], so we don't support 
                    // widening of primitive types here.
                    //
                    var objects = array as object[];
                    if (objects == null)
                    {
                        throw new ArrayTypeMismatchException("Invalid array type");
                    }

                    var count = _items.Count;
                    try
                    {
                        for (var i = 0; i < count; i++)
                        {
                            objects[index++] = _items[i];
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArrayTypeMismatchException("Invalid array type");
                    }
                }
            }
            finally
            {
                _itemsLock.ExitReadLock();
            }
        }

        public bool Contains(T item)
        {
            _itemsLock.EnterReadLock();

            try
            {
                return _items.Contains(item);
            }
            finally
            {
                _itemsLock.ExitReadLock();
            }
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                _itemsLock.EnterReadLock();

                try
                {
                    return _items.Contains((T)value);
                }
                finally
                {
                    _itemsLock.ExitReadLock();
                }
            }

            return false;
        }

        public void Dispose()
        {
            _itemsLock.Dispose();
        }

        public IEnumerator<T> GetEnumerator()
        {
            _itemsLock.EnterReadLock();

            try
            {
                return _items.ToList().GetEnumerator();
            }
            finally
            {
                _itemsLock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            _itemsLock.EnterReadLock();

            try
            {
                return ((IEnumerable)_items.ToList()).GetEnumerator();
            }
            finally
            {
                _itemsLock.ExitReadLock();
            }
        }

        public int IndexOf(T item)
        {
            _itemsLock.EnterReadLock();

            try
            {
                return _items.IndexOf(item);
            }
            finally
            {
                _itemsLock.ExitReadLock();
            }
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                _itemsLock.EnterReadLock();

                try
                {
                    return _items.IndexOf((T)value);
                }
                finally
                {
                    _itemsLock.ExitReadLock();
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            _itemsLock.EnterWriteLock();

            try
            {
                CheckIsReadOnly();
                CheckIndex(index);
                CheckReentrancy();

                _items.Insert(index, item);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        void IList.Insert(int index, object value)
        {
            try
            {
                Insert(index, (T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("'value' is the wrong type");
            }
        }

        public bool Remove(T item)
        {
            int index;
            T value;

            _itemsLock.EnterUpgradeableReadLock();

            try
            {
                CheckIsReadOnly();

                index = _items.IndexOf(item);
                if (index < 0)
                {
                    return false;
                }

                _itemsLock.EnterWriteLock();

                try
                {
                    CheckReentrancy();

                    value = _items[index];

                    _items.RemoveAt(index);
                }
                finally
                {
                    _itemsLock.ExitWriteLock();
                }
            }
            finally
            {
                _itemsLock.ExitUpgradeableReadLock();
            }

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, value, index);

            return true;
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        public void RemoveAt(int index)
        {
            T value;

            _itemsLock.EnterWriteLock();

            try
            {
                CheckIsReadOnly();
                CheckIndex(index);
                CheckReentrancy();

                value = _items[index];

                _items.RemoveAt(index);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, value, index);
        }
        #endregion

        #region SimpleMonitor Class
        private class SimpleMonitor : IDisposable
        {
            private int _busyCount;

            public bool Busy
            {
                get { return _busyCount > 0; }
            }

            public void Enter()
            {
                ++_busyCount;
            }

            public void Dispose()
            {
                --_busyCount;
            }
        }
        #endregion
    }
}

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
namespace RobotApp.ViewModel
{

    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    [DataContract]
    [Serializable]
    public class InputSignalViewModel : INotifyPropertyChanged
    {
        
        //Action<double> ValueReceived;
        public string UniqueID;

        /// <summary>
        /// Initializes a new instance of the SignalSink class.
        /// </summary>
        public InputSignalViewModel(string SinkName, string ParentInstanceName)
        {
            Debug.WriteLine("Creating new input signal view model " + SinkName + " for " + ParentInstanceName);
            if (UniqueID != null)
            {
                Debug.WriteLine("Unique ID already set to " + this.UniqueID);
            }
            else
            {
                UniqueID = Guid.NewGuid().ToString();
                Debug.WriteLine("Creating new Unique ID: " + UniqueID);
            }
            
            this.SinkName = SinkName;
            this.ParentInstanceName = ParentInstanceName;

            PostLoadSetup();
            
        }

        public void PostLoadSetup()
        {
            if (!MainViewModel.Instance.InputSignalRegistry.ContainsKey(UniqueID))
            {
                MainViewModel.Instance.InputSignalRegistry.Add(UniqueID, this.DisplayName);
            }

            this.PropertyChanged += InputSignalViewModel_PropertyChanged;
        }

        void InputSignalViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "DisplayName")
            {
                if (MainViewModel.Instance.InputSignalRegistry.ContainsKey(UniqueID))
                {
                    MainViewModel.Instance.InputSignalRegistry[UniqueID] = this.DisplayName;
                }
            }
        }

        /// <summary>
        /// Provides a descriptive single-line text string for use in the UI
        /// </summary>
        public string DisplayName
        {
            get
            {
                return parentInstanceName + ":" + sinkName;
            }
        }

        /// <summary>
        /// The <see cref="SinkName" /> property's name.
        /// </summary>
        public const string SinkNamePropertyName = "SinkName";
        [DataMember]
        private string sinkName = "";

        /// <summary>
        /// Sets and gets the SinkName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SinkName
        {
            get
            {
                return sinkName;
            }

            set
            {
                if (sinkName == value)
                {
                    return;
                }

                sinkName = value;

                if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(SinkNamePropertyName));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }

        /// <summary>
        /// The <see cref="ParentInstanceName" /> property's name.
        /// </summary>
        public const string ParentInstanceNamePropertyName = "ParentInstanceName";
        
        [DataMember]
        private string parentInstanceName = "";

        /// <summary>
        /// Sets and gets the UniqueName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ParentInstanceName
        {
            get
            {
                return parentInstanceName;
            }

            set
            {
                if (parentInstanceName == value)
                {
                    return;
                }

                parentInstanceName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ParentInstanceNamePropertyName));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }


        /// <summary>
        /// The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";
        
        private double _value = 0;

        /// <summary>
        /// Sets and gets the Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ValuePropertyName));
                Messenger.Default.Send<Messages.Signal>(new Messages.Signal() { Value = _value }, DisplayName);
                //if(ValueReceived != null) ValueReceived(_value);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
        
        [field: NonSerializedAttribute()] 
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
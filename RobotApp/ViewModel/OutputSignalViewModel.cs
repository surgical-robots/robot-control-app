using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class OutputSignalViewModel : INotifyPropertyChanged
    {
        [DataMember]
        public ObservableCollection<SignalSinkSelectorViewModel> SelectedSinks { get; set; }

        /// <summary>
        /// Initializes a new instance of the SignalSource class.
        /// </summary>
        public OutputSignalViewModel(string sourceName)
        {
            this.SourceName = sourceName;
            SelectedSinks = new ObservableCollection<SignalSinkSelectorViewModel>();
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
                foreach (SignalSinkSelectorViewModel sink in SelectedSinks)
                {
                    Messenger.Default.Send<Messages.Signal>(new Messages.Signal() { Value = _value }, sink.SelectedSink.Key);
                }
                if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ValuePropertyName));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DisplayText"));
            }
        }
        [NonSerialized]
        private RelayCommand removeSignalSinkCommand;

        /// <summary>
        /// Gets the RemoveSignalSinkCommand.
        /// </summary>
        public RelayCommand RemoveSignalSinkCommand
        {
            get
            {
                return removeSignalSinkCommand
                    ?? (removeSignalSinkCommand = new RelayCommand(
                   () =>
                       {
                           SelectedSinks.RemoveAt(SelectedSinks.Count-1);
                       }));
            }
        }
        [NonSerialized]
        private RelayCommand addSignalSinkCommand;

        /// <summary>
        /// Gets the MyCommand.
        /// </summary>
        public RelayCommand AddSignalSinkCommand
        {
            get
            {
                return addSignalSinkCommand
                    ?? (addSignalSinkCommand = new RelayCommand(
                    () =>
                    {
                        SelectedSinks.Add(new SignalSinkSelectorViewModel());
                    }));
            }
        }

        /// <summary>
        /// The <see cref="SourceName" /> property's name.
        /// </summary>
        public const string SourceNamePropertyName = "SourceName";
        [DataMember]
        private string sourceName = "";

        /// <summary>
        /// Sets and gets the SourceName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SourceName
        {
            get
            {
                return sourceName;
            }

            set
            {
                if (sourceName == value)
                {
                    return;
                }

                sourceName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(SourceNamePropertyName));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DisplayText"));
            }
        }

        /// <summary>
        /// Sets and gets the DisplayText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DisplayText
        {
            get
            {
                return SourceName + " (" + Math.Round(Value, 3) + ")";
            }
        }
        [field: NonSerializedAttribute()] 
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
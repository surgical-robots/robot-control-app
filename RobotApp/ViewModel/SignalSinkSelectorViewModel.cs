using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
    public class SignalSinkSelectorViewModel
    {
        [DataMember]
        public ObservableDictionary<string, string> SignalSinks { get; set; }
        [DataMember]
        public KeyValuePair<string, string> SelectedSink { get; set; }
        /// <summary>
        /// Initializes a new instance of the SignalSinkSelectorViewModel class.
        /// </summary>
        public SignalSinkSelectorViewModel()
        {
            SignalSinks = MainViewModel.Instance.InputSignalRegistry;
        }
    }
}
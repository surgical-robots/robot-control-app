using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
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
    [CollectionDataContractAttribute]
    [Serializable]
    public class SignalSinkRegistryViewModel : ObservableCollection<InputSignalViewModel>
    {
            
        /// <summary>
        /// Initializes a new instance of the SignalSinkRegistry class.
        /// </summary>
        public SignalSinkRegistryViewModel() : base()
        {

            Messenger.Default.Register<Messages.RegisterSignalSink>(this, 
                (Messages.RegisterSignalSink message) => 
                {
                    this.Add(message.Sink);
                    message.Sink.PropertyChanged += Sink_PropertyChanged;
                    Debug.WriteLine("Adding " + message.Sink);
                });

            Messenger.Default.Register<Messages.UnregisterSignalSink>(this,
                            (Messages.UnregisterSignalSink message) =>
                            {
                                message.Sink.PropertyChanged -= Sink_PropertyChanged;
                                this.Remove(message.Sink);
                                Debug.WriteLine("Removing " + message.Sink);
                            });
        }

        void Sink_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "DisplayName")
                Refresh();
        }

        public void Refresh()
        {
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset, null));
        }
    }
}
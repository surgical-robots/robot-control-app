using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;

namespace RobotApp.ViewModel.Plugins
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class EnableDisablePluginViewModel : PluginViewModelBase
    {
        public ObservableCollection<InputSignalViewModel> Inputs {get; set;}
        public InputSignalViewModel EnableSignal { get; set; }
        public ObservableCollection<OutputSignalViewModel> Outputs { get; set; }

        /// <summary>
        /// Initializes a new instance of the EnableDisableViewModel class.
        /// </summary>
        public EnableDisablePluginViewModel()
        {

            Outputs = new ObservableCollection<OutputSignalViewModel>();
            TypeName = "Enable/Disable Signal";
            EnableSignal = new InputSignalViewModel("Enable", InstanceName);

            Messenger.Default.Register<Messages.Signal>(this, (msg) =>
                {
                    isEnabled = msg.Value > 0.5 ? true : false;
                });

            Inputs = new ObservableCollection<InputSignalViewModel>();

        }


        bool isEnabled;

        /// <summary>
        /// The <see cref="InstanceName" /> property's name.
        /// </summary>
        public const string InstanceNamePropertyName = "InstanceName";

        private string instanceName = "";

        /// <summary>
        /// Sets and gets the InstanceName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string InstanceName
        {
            get
            {
                return instanceName;
            }

            set
            {
                if (instanceName == value)
                {
                    return;
                }

                instanceName = value;
                RaisePropertyChanged(InstanceNamePropertyName);
                //InputSignal.ParentInstanceName = value;
                EnableSignal.ParentInstanceName = value;
            }
        }

       
    }
}
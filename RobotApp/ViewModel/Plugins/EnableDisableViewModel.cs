using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace RobotApp.ViewModel.Plugins
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class EnableDisableViewModel : SignalPluginViewModelBase
    {
        public ObservableCollection<SignalSinkViewModel> Inputs {get; set;}
        public SignalSinkViewModel EnableSignal { get; set; }
        public ObservableCollection<SignalSourceViewModel> Outputs { get; set; }

        /// <summary>
        /// Initializes a new instance of the EnableDisableViewModel class.
        /// </summary>
        public EnableDisableViewModel()
        {

            Outputs = new ObservableCollection<SignalSourceViewModel>();
            TypeName = "Enable/Disable Signal";
            EnableSignal = new SignalSinkViewModel("Enable", InstanceName,
                (inputValue) =>
                {
                    isEnabled = inputValue > 0.5 ? true : false;
                }
            );

            Inputs = new ObservableCollection<SignalSinkViewModel>();

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
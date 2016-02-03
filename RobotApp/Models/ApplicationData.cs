using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RobotApp;
using RobotApp.ViewModel;
using System.Collections.Concurrent;

namespace RobotApp.Models
{
    [DataContract]
    [Serializable]
    public class ApplicationData
    {
        [DataMember]
        public ObservableCollection<PluginDataItem> Plugins;

        [DataMember]
        public ObservableCollection<ViewModel.ControllerViewModel> Controllers;

        [DataMember]
        public ObservableDictionary<string, string> InputSignalRegistry;

        public ApplicationData()
        {
            Plugins = new ObservableCollection<PluginDataItem>();
            Controllers = new ObservableCollection<ViewModel.ControllerViewModel>();
        }

    }
}

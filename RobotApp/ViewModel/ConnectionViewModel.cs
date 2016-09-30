using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
namespace RobotApp.ViewModel
{
    public class ConnectionViewModel : ViewModelBase
    {
        public ObservableCollection<TransportViewModelBase> ConnectionTypes { get; set; }
        
        public ConnectionViewModel()
        {
            ConnectionTypes = new ObservableCollection<TransportViewModelBase>();
            ConnectionTypes.Add(new SerialTransportViewModel());
            ConnectionTypes.Add(new SocketTransportViewModel());
        }
    }
}

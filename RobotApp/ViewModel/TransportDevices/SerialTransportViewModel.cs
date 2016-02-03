using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Collections.ObjectModel;
using RobotControl;
namespace RobotApp.ViewModel
{
    public class SerialTransportViewModel : TransportViewModelBase
    {

        public ObservableCollection<SerialPort> Ports { get; set; }
        public SerialTransportViewModel()
        {
            this.Name = "Serial Transport";
            Ports = new ObservableCollection<SerialPort>();
            foreach(string portName in SerialPort.GetPortNames())
            {
                Ports.Add(new SerialPort(portName));
            }
        }
        private SerialPort selectedPort;

        public SerialPort SelectedPort
        {
            get { return selectedPort; }
            set { 
                selectedPort = value;
                MainViewModel.Instance.Robot.Com = new SerialPortPacketTransport(selectedPort.PortName, MainViewModel.Instance.Robot);
            }
        }

    }
}

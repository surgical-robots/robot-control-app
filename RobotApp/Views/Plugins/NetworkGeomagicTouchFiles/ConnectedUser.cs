using RobotApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.Views.Plugins.NetworkGeomagicTouchFiles
{
    public class ConnectedUser : PluginViewModelBase
    {
        private string _name;
        private string _ip;
        private int _myNumber;
        private bool _hasControl;
        public ConnectedUser()
        {
            this._name = "";
            this._ip = "";
            this._myNumber = 0;
            this._hasControl = false;
        }

        public bool HasControl
        {
            get
            {
                return this._hasControl;
            }
            set
            {
                this._hasControl = value;
                this.RaisePropertyChanged("HasControl");
                this.RaisePropertyChanged("ButtonString");
                this.RaisePropertyChanged("IsUserEnabled");
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
                this.RaisePropertyChanged("Name");
            }
        }
        public string Ip
        {
            get
            {
                return this._ip;
            }
            set
            {
                this._ip = value;
                this.RaisePropertyChanged("Ip");
            }
        }
        public int MyNumber
        {
            get
            {
                return this._myNumber;
            }
            set
            {
                this._myNumber = value;
                this.RaisePropertyChanged("MyNumber");
            }
        }

        public string ConfiguredString
        {
            get
            {
                return "User" + this._myNumber + ": Connected from " + this._ip;
            }
        }
        public string ButtonString
        {
            get
            {
                if(this._hasControl)
                {
                    return this._name + " has control";
                }
                else
                {
                    return "Give Control to " + this._name;
                }
            }
        }
        public bool IsUserEnabled
        {
            get
            {
                return !this._hasControl;
            }
        }
    }
}

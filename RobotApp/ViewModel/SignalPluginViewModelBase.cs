using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.ViewModel
{
    public class SignalPluginViewModelBase : ViewModelBase, IDisposable
    {
        /// <summary>
        /// The <see cref="TypeName" /> property's name.
        /// </summary>
        public const string TypeNamePropertyName = "TypeName";

        private string typeName = "";

        /// <summary>
        /// Sets and gets the TypeName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string TypeName
        {
            get
            {
                return typeName;
            }

            set
            {
                if (typeName == value)
                {
                    return;
                }

                typeName = value;
                RaisePropertyChanged(TypeNamePropertyName);
            }
        }

        private string instanceName;

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
                RaisePropertyChanged("InstanceName");
            }
        }


        public virtual void Dispose()
        {
            
        }
    }
}

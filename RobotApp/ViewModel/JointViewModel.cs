using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.ViewModel
{
    /// <summary>
    /// This view model describes a physical joint on the robot
    /// </summary>
    class JointViewModel : ViewModelBase
    {
        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string name = "";

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }
        /// <summary>
        /// The <see cref="Angle" /> property's name.
        /// </summary>
        public const string AnglePropertyName = "Angle";

        private double angle = 0;

        /// <summary>
        /// Sets and gets the Angle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Angle
        {
            get
            {
                return angle;
            }

            set
            {
                if (angle == value)
                {
                    return;
                }

                angle = value;
                RaisePropertyChanged(AnglePropertyName);
            }
        }
    }
}

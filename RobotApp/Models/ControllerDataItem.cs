using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RobotApp.ViewModel;

namespace RobotApp.Models
{
    [DataContract]
    public class ControllerDataItem
    {


        public ControllerViewModel GetControllerViewModel()
        {
            ControllerViewModel controller = new ControllerViewModel();
            
            
            
            return controller;
        }
    }


}

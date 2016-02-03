using RobotApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.Messages
{
    public class UnregisterSignalSink
    {
        public InputSignalViewModel Sink { get; set; }
    }
}

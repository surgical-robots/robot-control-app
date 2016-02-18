using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelSurge.DataModels;

namespace TelSurge
{
    class Surgery
    {
        public OmniPosition InControlPosition { get; set; }
        public User Master { get; set; }
        public List<User> ConnectedClients { get; set; }
        public User UserInControl { get; set; }
    }
}

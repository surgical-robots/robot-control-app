using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelSurge.DataModels;

namespace TelSurge
{
    public class Surgery
    {
        public OmniPosition InControlPosition { get; set; }
        public User Master { get; set; }
        public List<User> ConnectedClients { get; set; }
        public User UserInControl { get; set; }

        public Surgery()
        {
            this.ConnectedClients = new List<User>();
        }
    }
}

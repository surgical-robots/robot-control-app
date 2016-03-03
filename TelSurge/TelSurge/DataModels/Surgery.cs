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
            this.Master = new User();
        }
        public void Merge(Surgery Surgery, bool IsInControl, bool IsMaster)
        {
            if (!IsInControl)
            {
                this.InControlPosition = Surgery.InControlPosition;
                this.UserInControl = Surgery.UserInControl;
            }
            if (!IsMaster)
            {
                this.Master = Surgery.Master;
                this.ConnectedClients = Surgery.ConnectedClients;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelSurge.DataModels
{
    public class IPCamera
    {
        public int Identifier { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PTZAddress { get; set; }

        public IPCamera()
        {

        }
        public IPCamera(int ID, string Name, string Address)
        {
            this.Identifier = ID;
            this.Name = Name;
            this.Address = Address;
        }
        public IPCamera(int ID, string Name, string Address, string PTZAddress)
        {
            this.Identifier = ID;
            this.Name = Name;
            this.Address = Address;
            this.PTZAddress = PTZAddress;
        }
    }
}

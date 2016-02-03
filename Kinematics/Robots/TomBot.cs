using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics.Robots
{
    class TomBot : TwoArmCoupledShoulder
    {
        public TomBot()
        {
            this.ShoulderOffset = 17.78;
            this.LengthUpperArm = 68.58;
            this.LengthForearm = 96.393;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics.Robots
{
    class TomShortArm : TwoArmCoupledShoulderAndElbow
    {
        public TomShortArm()
        {
            this.ShoulderOffset = 17.78;
            this.LengthUpperArm = 54;
            this.LengthForearm = 63;
        }
    }
}

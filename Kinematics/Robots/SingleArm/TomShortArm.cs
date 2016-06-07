using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics.Robots
{
    class TomShortArm : CoupledShoulderAndElbow3DOF
    {
        public TomShortArm()
        {
            this.ShoulderOffset = 17.78;
            this.LengthUpperArm = 54;
            this.LengthForearm = 63;
            this.Theta1Max = 40;
            this.Theta1Min = -90;
            this.Theta2Max = 90;
            this.Theta2Min = -20;
            this.Theta3Max = 155;
            this.Theta3Min = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics.Robots
{
    class Frankie : FrankenBot
    {
        public Frankie()
        {
            this.ShoulderOffset = 17.78;
            this.LengthUpperArm = 53;
            this.LengthForearm = 80;
            this.Theta1Max = 30;
            this.Theta1Min = -120;
            this.Theta2Max = 30;
            this.Theta2Min = -90;
            this.Theta3Max = 140;
            this.Theta3Min = 0;
        }
    }
}

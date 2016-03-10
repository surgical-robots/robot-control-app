using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics.Robots
{
    class FourDOFsolver : IKSolver
    {
        public FourDOFsolver()
        {
            this.LinkTable = new double[4, 3] { { 0, 0, 0 }, { 90, 0, 0 }, { -90, 68.58, 0 }, { 0, 96.393, 0 } };
            this.N = 4;
            this.MinMax = new System.Windows.Point[N - 1];
            this.MinMax[0] = new System.Windows.Point(-90, 90);
            this.MinMax[1] = new System.Windows.Point(-45, 90);
            this.MinMax[2] = new System.Windows.Point(0, 105);
            this.OutputStrings = new string[3] { "Theta1", "Theta2", "Theta3" };
        }
    }
}

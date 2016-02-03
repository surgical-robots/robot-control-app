using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics
{
    public class TwoArmCoupledShoulder : Kinematic
    {
        double[] oldAngle = new double[3];

        /// <summary>
        /// Measurement in mm of shoulder offset from center plane
        /// </summary>
        public double ShoulderOffset { get; set; }

        /// <summary>
        /// Measurement in mm of the upper arm length
        /// </summary>
        public double LengthUpperArm { get; set; }

        /// <summary>
        /// Measurement in mm of the forearm from elbow to functional tip
        /// </summary>
        public double LengthForearm { get; set; }

        /// <summary>
        /// This returns an array of joint angles from an input XYZ position
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        protected override double[] getJointAngles(Point3D Position)
        {

            double px = Position.X;
            double py = Position.Y;
            double pz = Position.Z;

            double[] angles = new double[3];
            double[] kineAngle = new double[3];
            double[] radianAngle = new double[3];

            double Lmax = LengthUpperArm + LengthForearm;
            double L12 = Math.Sqrt(Math.Pow(px, 2) + Math.Pow(py, 2) + Math.Pow(pz, 2));
            double Lratio = Lmax / L12;

            double argument1 = (Math.Pow(LengthUpperArm, 2) + Math.Pow(LengthForearm, 2) - Math.Pow(L12, 2)) / (2 * LengthUpperArm * LengthForearm);
            kineAngle[2] = Math.PI - Math.Acos(argument1);

            double argument2 = py / (LengthUpperArm + LengthForearm * Math.Cos(kineAngle[2]));
            kineAngle[1] = Math.Atan2(argument2, Math.Sqrt(1 - Math.Pow(argument2, 2)));

            double argument3 = LengthUpperArm * Math.Cos(kineAngle[1]) + LengthForearm * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]);
            double test = Math.Atan2(px, pz);

            kineAngle[0] = -Math.Atan2(pz, px) + Math.Atan2(argument3, Math.Sqrt(Math.Pow(px, 2) + Math.Pow(pz, 2) - Math.Pow(argument3, 2)));
            if(px < 0 && pz < 0)
            {
                kineAngle[0] = kineAngle[0] - 2 * Math.PI;
            }
            
            radianAngle[0] = kineAngle[0] + kineAngle[1];
            radianAngle[1] = kineAngle[0] - kineAngle[1];
            radianAngle[2] = kineAngle[2];

            angles[0] = radianAngle[0] * 180 / Math.PI;
            angles[1] = radianAngle[1] * 180 / Math.PI;
            angles[2] = radianAngle[2] * 180 / Math.PI;

            if (double.IsNaN(angles[0]) || double.IsNaN(angles[1]) || double.IsNaN(angles[2]))
            {
                angles[0] = oldAngle[0];
                angles[1] = oldAngle[1];
                angles[2] = oldAngle[2];
                Debug.WriteLine("Out of workspace!");
            }

            oldAngle[0] = angles[0];
            oldAngle[1] = angles[1];
            oldAngle[2] = angles[2];

            return angles;
        }

        public override string[] OutputNames
        {
            get {
                return new string[] { "Shoulder Upper Bevel", "Shoulder Lower Bevel", "Elbow" };
            }
        }
    }
}

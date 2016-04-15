using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics
{
    public class TwoArmCoupledShoulder3DOF : Kinematic
    {
        double[] oldAngle = new double[12];

        Point3D oldPoint;

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
        /// Measurement in degs of the minimum shoulder yaw
        /// </summary>
        public double Theta1Min { get; set; }

        /// <summary>
        /// Measurement in degs of the maximum shoulder yaw
        /// </summary>
        public double Theta1Max { get; set; }

        /// <summary>
        /// Measurement in degs of the minimum shoulder pitch
        /// </summary>
        public double Theta2Min { get; set; }

        /// <summary>
        /// Measurement in degs of the maximum shoulder pitch
        /// </summary>
        public double Theta2Max { get; set; }

        /// <summary>
        /// Measurement in degs of the minimum elbow angle
        /// </summary>
        public double Theta3Min { get; set; }

        /// <summary>
        /// Measurement in degs of the maximum elbow angle
        /// </summary>
        public double Theta3Max { get; set; }

        /// <summary>
        /// This returns an array of joint angles from an input XYZ position
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        protected override double[] getJointAngles(Point3D PositionL, Point3D PositionR)
        {

            double pxL = PositionL.X;
            double pyL = PositionL.Y;
            double pzL = PositionL.Z;

            double pxR = PositionR.X;
            double pyR = PositionR.Y;
            double pzR = PositionR.Z;

            double[] angles = new double[12];
            double[] kineAngle = new double[3];
            double[] radianAngle = new double[3];
            double[] minAngle = new double[3];
            double[] maxAngle = new double[3];
            double[] dummyAngle = { 0, 0, 0 };
            bool angleLimited = false;

            minAngle[0] = Theta1Min * Math.PI / 180;
            minAngle[1] = Theta2Min * Math.PI / 180;
            minAngle[2] = Theta3Min * Math.PI / 180;
            maxAngle[0] = Theta1Max * Math.PI / 180;
            maxAngle[1] = Theta2Max * Math.PI / 180;
            maxAngle[2] = Theta3Max * Math.PI / 180;

            double Lmax = LengthUpperArm + LengthForearm;

            double barrierSpring = 0.5;

            // Left Arm Kinematics
            double L12 = Math.Sqrt(Math.Pow(pxL, 2) + Math.Pow(pyL, 2) + Math.Pow(pzL, 2));
            double Lratio = Lmax / L12;

            if(L12 > Lmax)
            {
                pxL = pxL * Lratio;
                pyL = pyL * Lratio;
                pzL = pzL * Lratio;
                L12 = Lmax;
                kineAngle[2] = 0;
            }
            else
            {
                double argument1 = (Math.Pow(LengthUpperArm, 2) + Math.Pow(LengthForearm, 2) - Math.Pow(L12, 2)) / (2 * LengthUpperArm * LengthForearm);
                kineAngle[2] = Math.PI - Math.Acos(argument1);
            }

            double argument2 = pyL / (LengthUpperArm + LengthForearm * Math.Cos(kineAngle[2]));
            if (argument2 > 1)
            {
                argument2 = 1;
                angleLimited = true;
            }
            kineAngle[1] = Math.Atan2(argument2, Math.Sqrt(1 - Math.Pow(argument2, 2)));

            double argument3 = LengthUpperArm * Math.Cos(kineAngle[1]) + LengthForearm * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]);
            kineAngle[0] = -Math.Atan2(pzL, pxL) + Math.Atan2(argument3, Math.Sqrt(Math.Pow(pxL, 2) + Math.Pow(pzL, 2) - Math.Pow(argument3, 2)));
            if(pxL < 0 && pzL < 0)
            {
                kineAngle[0] = kineAngle[0] - 2 * Math.PI;
            }

            for (int i = 0; i < 3; i++ )
            {
                if (kineAngle[i] < minAngle[i])
                {
                    kineAngle[i] = minAngle[i];
                    angleLimited = true;
                }
                else if (kineAngle[i] > maxAngle[i])
                {
                    kineAngle[i] = maxAngle[i];
                    angleLimited = true;
                }
            }

            radianAngle[0] = kineAngle[0] + kineAngle[1];
            radianAngle[1] = kineAngle[0] - kineAngle[1];
            radianAngle[2] = kineAngle[2];

            angles[0] = radianAngle[0] * 180 / Math.PI;
            angles[1] = radianAngle[1] * 180 / Math.PI;
            angles[2] = radianAngle[2] * 180 / Math.PI;

            // calculate inverse kinematics and haptic forces
            double kineZ = LengthUpperArm * Math.Cos(kineAngle[0]) * Math.Cos(kineAngle[1]) - LengthForearm * (Math.Sin(kineAngle[0]) * Math.Sin(kineAngle[2]) - Math.Cos(kineAngle[0]) * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]));
            double kineY = LengthUpperArm * Math.Sin(kineAngle[1]) + LengthForearm * Math.Sin(kineAngle[1]) * Math.Cos(kineAngle[2]);
            double kineX = LengthUpperArm * Math.Sin(kineAngle[0]) * Math.Cos(kineAngle[1]) + LengthForearm * (Math.Cos(kineAngle[0]) * Math.Sin(kineAngle[2]) + Math.Sin(kineAngle[0]) * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]));

            if(!angleLimited)
            {
                oldPoint.X = kineX;
                oldPoint.Y = kineY;
                oldPoint.Z = kineZ;
            }

            angles[3] = (oldPoint.X - PositionL.X) * barrierSpring;
            angles[4] = (oldPoint.Y - PositionL.Y) * -barrierSpring;
            angles[5] = (oldPoint.Z - PositionL.Z) * -barrierSpring;

            for (int i = 3; i < 6; i++)
            {
                if (angles[i] > 4)
                    angles[i] = 4;
                else if (angles[i] < -4)
                    angles[i] = -4;
            }

            // Right arm kinematics
            L12 = Math.Sqrt(Math.Pow(pxR, 2) + Math.Pow(pyR, 2) + Math.Pow(pzR, 2));
            Lratio = Lmax / L12;

            if (L12 > Lmax)
            {
                pxR = pxR * Lratio;
                pyR = pyR * Lratio;
                pzR = pzR * Lratio;
                L12 = Lmax;
                kineAngle[2] = 0;
            }
            else
            {
                double argument1 = (Math.Pow(LengthUpperArm, 2) + Math.Pow(LengthForearm, 2) - Math.Pow(L12, 2)) / (2 * LengthUpperArm * LengthForearm);
                kineAngle[2] = Math.PI - Math.Acos(argument1);
            }

            argument2 = pyR / (LengthUpperArm + LengthForearm * Math.Cos(kineAngle[2]));
            if (argument2 > 1)
            {
                argument2 = 1;
                angleLimited = true;
            }
            kineAngle[1] = Math.Atan2(argument2, Math.Sqrt(1 - Math.Pow(argument2, 2)));

            argument3 = LengthUpperArm * Math.Cos(kineAngle[1]) + LengthForearm * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]);
            kineAngle[0] = -Math.Atan2(pzR, pxR) + Math.Atan2(argument3, Math.Sqrt(Math.Pow(pxR, 2) + Math.Pow(pzR, 2) - Math.Pow(argument3, 2)));
            if (pxL < 0 && pzL < 0)
            {
                kineAngle[0] = kineAngle[0] - 2 * Math.PI;
            }

            for (int i = 0; i < 3; i++)
            {
                if (kineAngle[i] < minAngle[i])
                {
                    kineAngle[i] = minAngle[i];
                    angleLimited = true;
                }
                else if (kineAngle[i] > maxAngle[i])
                {
                    kineAngle[i] = maxAngle[i];
                    angleLimited = true;
                }
            }

            radianAngle[0] = kineAngle[0] + kineAngle[1];
            radianAngle[1] = kineAngle[0] - kineAngle[1];
            radianAngle[2] = kineAngle[2];

            angles[6] = radianAngle[0] * 180 / Math.PI;
            angles[7] = radianAngle[1] * 180 / Math.PI;
            angles[8] = radianAngle[2] * 180 / Math.PI;

            // calculate inverse kinematics and haptic forces
            kineZ = LengthUpperArm * Math.Cos(kineAngle[0]) * Math.Cos(kineAngle[1]) - LengthForearm * (Math.Sin(kineAngle[0]) * Math.Sin(kineAngle[2]) - Math.Cos(kineAngle[0]) * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]));
            kineY = LengthUpperArm * Math.Sin(kineAngle[1]) + LengthForearm * Math.Sin(kineAngle[1]) * Math.Cos(kineAngle[2]);
            kineX = LengthUpperArm * Math.Sin(kineAngle[0]) * Math.Cos(kineAngle[1]) + LengthForearm * (Math.Cos(kineAngle[0]) * Math.Sin(kineAngle[2]) + Math.Sin(kineAngle[0]) * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]));

            if (!angleLimited)
            {
                oldPoint.X = kineX;
                oldPoint.Y = kineY;
                oldPoint.Z = kineZ;
            }

            angles[9] = (oldPoint.X - PositionR.X) * -barrierSpring;
            angles[10] = (oldPoint.Y - PositionR.Y) * -barrierSpring;
            angles[11] = (oldPoint.Z - PositionR.Z) * -barrierSpring;

            for (int i = 3; i < 6; i++)
            {
                if (angles[i] > 4)
                    angles[i] = 4;
                else if (angles[i] < -4)
                    angles[i] = -4;
            }

            for (int i = 0; i < 6; i++)
            {
                if (double.IsNaN(angles[i]))
                {
                    angles[i] = oldAngle[i];
                }
                oldAngle[i] = angles[i];
            }

            return angles;
        }

        public override string[] OutputNames
        {
            get {
                return new string[] { "Left Upper Bevel", "Left Lower Bevel", "Left Elbow", "Force LX", "Force LY", "Force LZ",
                                    "Right Upper Bevel", "Right Lower Bevel", "Right Elbow", "Force RX", "Force RY", "Force RZ" };
            }
        }
    }
}

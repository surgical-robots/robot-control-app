using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics
{
    public class CombinedBot : Kinematic
    {
        double[] oldAngle = new double[15];

        Point3D oldPointL, oldPointR;

        double YawDir=0;
        double TiltDir = 0;

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

            Point3D midPoint;
            midPoint.X = (pzL + pzR) / 2;
            midPoint.Y = (pxL - pxR) / 2;
            midPoint.Z = (pyL + pyR) / 2;

            double r = Math.Sqrt(Math.Pow(midPoint.X, 2) + Math.Pow(midPoint.Y, 2) + Math.Pow(midPoint.Z, 2));
            double Phi = Math.Acos(midPoint.Z / r);
            double Theta = Math.Atan(midPoint.Y / midPoint.X);

            if (Phi > (115 * Math.PI / 180))
                TiltDir = 1;
            else if (Phi < (65 * Math.PI / 180))
                TiltDir = -1;
            else
                TiltDir = 0;

            if (Theta > (20 * Math.PI / 180))
                YawDir = 1;
            else if (Theta < -(20 * Math.PI / 180))
                YawDir = -1;
            else
                YawDir = 0;

            double[] angles = new double[15];
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
            double Lmin = Math.Sqrt(Math.Pow(LengthUpperArm, 2) + Math.Pow(LengthForearm, 2) - 2 * LengthUpperArm * LengthForearm * Math.Cos(Math.PI - maxAngle[2]));

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
                //angleLimited = true;
                kineAngle[2] = 0;
            }
            else if (L12 < Lmin)
            {
                Lratio = Lmin / L12;
                pxL = pxL * Lratio;
                pyL = pyL * Lratio;
                pzL = pzL * Lratio;
                L12 = Lmin;
                //angleLimited = true;
                kineAngle[2] = maxAngle[2];
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
            //if (kineAngle[1] < minAngle[1])
            //{
            //    double calcTheta = Math.Atan(pxL / pzL);
            //    double Lxz = L12 * Math.Cos(minAngle[1]);
            //    pxL = Lxz * Math.Sin(calcTheta);
            //    pyL = L12 * Math.Sin(minAngle[1]);
            //    pzL = Lxz * Math.Cos(calcTheta);
            //    kineAngle[1] = minAngle[1];
            //}
            //else if (kineAngle[1] > maxAngle[1])
            //{
            //    kineAngle[1] = maxAngle[1];
            //    double calcTheta = Math.Atan(pxL / pzL);
            //    double Lxz = L12 * Math.Cos(maxAngle[1]);
            //    pxL = Lxz * Math.Sin(calcTheta);
            //    pyL = L12 * Math.Sin(maxAngle[1]);
            //    pzL = Lxz * Math.Cos(calcTheta);
            //}

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
                oldPointL.X = kineX;
                oldPointL.Y = kineY;
                oldPointL.Z = kineZ;
            }

            angles[3] = (oldPointL.X - PositionL.X) * barrierSpring;
            angles[4] = (oldPointL.Y - PositionL.Y) * -barrierSpring;
            angles[5] = (oldPointL.Z - PositionL.Z) * -barrierSpring;

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
            else if (L12 < Lmin)
            {
                Lratio = Lmin / L12;
                pxR = pxR * Lratio;
                pyR = pyR * Lratio;
                pzR = pzR * Lratio;
                L12 = Lmin;
                kineAngle[2] = maxAngle[2];
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
                oldPointR.X = kineX;
                oldPointR.Y = kineY;
                oldPointR.Z = kineZ;
            }

            angles[9] = (oldPointR.X - PositionR.X) * -barrierSpring;
            angles[10] = (oldPointR.Y - PositionR.Y) * -barrierSpring;
            angles[11] = (oldPointR.Z - PositionR.Z) * -barrierSpring;

            for (int i = 9; i < 12; i++)
            {
                if (angles[i] > 4)
                    angles[i] = 4;
                else if (angles[i] < -4)
                    angles[i] = -4;
            }

            angles[12] = YawDir;
            angles[13] = TiltDir;
            angles[14] = Phi * 180 / Math.PI;

            for (int i = 0; i < 15; i++)
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
                                    "Right Upper Bevel", "Right Lower Bevel", "Right Elbow", "Force RX", "Force RY", "Force RZ", "YawDir", "TiltDir", "Phi" };
            }
        }
    }
}

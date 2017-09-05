using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Kinematics
{
    public class Kinematic
    {
        public enum CouplingType
        {
            None,
            ShoulderTwoDOF,
            ShoulderThreeDOF,
            FrankenBot,
            LouShoulder
        }

        public enum JointType
        {
            Rotation,
            Translation
        }

        /// <summary>
        /// Gets a list of joint angles from a three-dimensional position
        /// </summary>
        /// <param name="Position">The position to find the joint angles from</param>
        /// <returns></returns>
        public double[] GetJointAngles(Point3D Position)
        {
            return (getJointAngles(Position));
        }

        public double[] GetJointAngles(Point3D PositionL, Point3D PositionR)
        {
            return (getJointAngles(PositionL, PositionR));
        }

        public double[] GetJointAngles(Vector3 Position, Vector3 Orientation, float[,] RotM)
        {
            return (getJointAngles(Position, Orientation, RotM));
        }

        public double[] GetJointAngles(Vector3D Position, Vector3D Orientation, double[,] RotM)
        {
            return (getJointAngles(Position, Orientation, RotM));
        }

        public virtual string[] OutputNames { get { return new string[0]; } }

        /// <summary>
        /// Implement this function, which is specific to the robot.
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        protected virtual double[] getJointAngles(Point3D Position)
        {
            return new double[0];
        }

        protected virtual double[] getJointAngles(Point3D PositionL, Point3D PositionR)
        {
            return new double[0];
        }

        protected virtual double[] getJointAngles(Vector3 Position, Vector3 Orientation, float[,] RotM)
        {
            return new double[0];
        }

        protected virtual double[] getJointAngles(Vector3D Position, Vector3D Orientation, double[,] RotM)
        {
            return new double[0];
        }
    }
}

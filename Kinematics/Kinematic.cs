using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics
{
    public class Kinematic
    {
        /// <summary>
        /// Gets a list of joint angles from a three-dimensional position
        /// </summary>
        /// <param name="Position">The position to find the joint angles from</param>
        /// <returns></returns>
        public double[] GetJointAngles(Point3D Position)
        {
            return (getJointAngles(Position));
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
    }
}

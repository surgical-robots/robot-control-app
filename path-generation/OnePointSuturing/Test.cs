using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace path_generation.OnePointSuturing
{
    public class Test
    {
        Test()
        {
        }
        public static void specified_pos(Needle needle)
        {
            /*
            Needle needle = new Needle(); // for sake of have a valid head position and initial joint values for the following optimizer
            needle.kinematics.joint.UpperBevel = 60;
            needle.kinematics.joint.LowerBevel = 60;
            needle.kinematics.joint.Elbow = 150; // no need for twist, twist will be given in loop
            needle.kinematics.joint.twist=0;
            needle.update_needle();
            */

            Optimizer optimizer = new Optimizer();
            optimizer.point_target.X = NeedleKinematics.get_translation(needle.head).X;
            optimizer.point_target.Y = NeedleKinematics.get_translation(needle.head).Y;
            optimizer.point_target.Z = NeedleKinematics.get_translation(needle.head).Z;

            optimizer.x = new double[3] { needle.kinematics.joint.UpperBevel, needle.kinematics.joint.LowerBevel, needle.kinematics.joint.Elbow };
            int n = 20;
            for (int i = 0; i < n; i++)
            {
                optimizer.point_target.W = i * 360 / n;
                Joints optimized_joints = optimizer.minimize_postionNtwist();
                needle.kinematics.joint=optimized_joints;
                needle.update_needle();
                Print.PrintJointOnFile(optimized_joints);
                Print.PrintMatrixOnFile(needle.center);
                optimizer.x = new double[3] { optimized_joints.UpperBevel, optimized_joints.LowerBevel, optimized_joints.Elbow };
            }
        }
    }
}

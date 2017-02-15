using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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
        public static void optimization_test() // given a matrix find the possible one
        {
            Optimizer optimizaer = new Optimizer();
            Matrix3D T_target= new Matrix3D( 0.0630,    0.0303 ,   0.9976 , 129.7036,
                                           -0.2570,    0.9663,   -0.0131,  -43.1047,
                                           -0.9643,   -0.2556,    0.0686,   13.1218,
                                                 0,         0,         0,    1.0000);
            optimizaer.T_taget = T_target;
            optimizaer.x = new double[4] { -50.0282200688546, -75.3754525301847, 57.2766149563357, 162 };
            Joints optimized_joints;
            optimized_joints = optimizaer.minimize_error();
            Needle needle = new Needle();
            needle.kinematics.joint = optimized_joints;
            needle.update_needle();
        }
        public static void score_accuracy(Needle needle_old, Needle needle_new)
        {
            if (needle_old != null && needle_new != null)
            {
                Vector3D[] points_old = needle_old.real_half;
                Vector3D[] points_new = needle_new.real_half;
                int inx = find_closest(points_old, points_new[0]);
                int n = needle_old.real_half.Length;
                double score = 0;
                for (int i = inx; i < n; i++)
                {
                    score = score + Math.Pow((points_old[i] - points_new[i - inx]).Length, 2);
                }
                score = Math.Sqrt(score);
            }
        }
        private static int find_closest(Vector3D[] points, Vector3D p)
        {
            int n = points.Length;
            double[] len=new double[n];
            int i = 0;
            for (i = 0; i < n; i++)
            {
                len[i] = (points[i] - p).Length;
            }
            double min = len.Min();
            for (i = 0; i < n; i++)
            {
                if (len[i] == min)
                    break;
            }
            return i;
        }
    }
}

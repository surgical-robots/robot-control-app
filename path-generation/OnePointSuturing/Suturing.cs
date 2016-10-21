using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Forms;

namespace path_generation.OnePointSuturing
{
    public class Suturing
    {
        //public Joints joints;
        public Needle needle; // the trajectory will initialize the needle, then will get updated based on new needle tip.
        double t = 0;
        public int state;
        public Suturing()
        {
            state = 0;
        }
        public void START_SUTURING()
        {
            Console.Write("\nSuturing satrted.\n");
            needle = new Needle();
            t = 0;
            state = 1; // state initialization: state 1 indicates entry, state 2 exit and state 3 the suturing
        }
        public void SELECT_ENTRY(Joints joints)
        {
            
            needle.kinematics.joint.UpperBevel = joints.UpperBevel;
            needle.kinematics.joint.LowerBevel = joints.LowerBevel;
            needle.kinematics.joint.Elbow = joints.Elbow;
            needle.kinematics.joint.twist = joints.twist;
            needle.update_needle();


            Optimizer optimizer = new Optimizer();
            optimizer.T_taget = needle.kinematics.transformation_matrix(55);
            optimizer.x = new double[4] { joints.UpperBevel, joints.LowerBevel, joints.Elbow, joints.twist };
            Joints optimized_joints = optimizer.minimize_error();
            Needle n = new Needle();
            n.kinematics.joint.UpperBevel = optimized_joints.UpperBevel;
            n.kinematics.joint.LowerBevel = optimized_joints.LowerBevel;
            n.kinematics.joint.Elbow = optimized_joints.Elbow;
            n.kinematics.joint.twist = optimized_joints.twist;
            n.kinematics.transformation_matrix(55);
            Matrix3D ti = needle.kinematics.transformation_matrix(55);
            Matrix3D te = n.kinematics.transformation_matrix(55);
            state++;
             
            state++;
        }
        public void SELECT_ENTRY(double x, double y, double z)
        {
            Console.Write("\nState 1: ENTRY POINT selected\n");
            Vector3D entry_point = new Vector3D(x, y, z);
            //Vector3D entry_point = new Vector3D(-30, 0, 130);
            Print.print_vector(entry_point);
            state++;
        }
        public void SELECT_EXIT(double x, double y, double z)
        {
            Console.Write("\nState 2 EXIT POINT selected\n");
            Vector3D exit_point = new Vector3D(x, y, z);
            //Vector3D exit_point = new Vector3D(-25, 0, 130);
            Print.print_vector(exit_point);
            state++;
        }
        public bool INITIALIZE_SUTURING()
        {//-60,-60,80,0
            /*
            needle.kinematics.joint.UpperBevel = 40;
            needle.kinematics.joint.LowerBevel = 30;
            needle.kinematics.joint.Elbow = -70;
            needle.kinematics.joint.twist = 0;
            needle.update_needle();
            */
            state++;
            return true;
        }
        public void PRE_SUTURING()
        {
        }
        public bool DO_SUTURING()
        {
            needle.update_needle(needle.moved_head);
            t = t + Math.PI / (needle.n - 1);
            if (t >= 1 * Math.PI)
            {
                END_SUTURING();
                Console.Write("\nAutomatically ended\n");
                return true;
            }
            return false;
        }

        public void END_SUTURING()
        {
            Console.Write("\nSuturing ended.\n");
            t = 0;
            state = 0;
        }
        private double twist_correction(double t)
        {
            return (-t * 180 / Math.PI);
        }
    }
}

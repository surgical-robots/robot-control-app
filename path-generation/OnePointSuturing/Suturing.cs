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
        public Trajectory trajectory;
        double t = 0;
        public int state;
        //public enum mode { one_point_suturing, two_point_suturing };
        int mode = 1;
        public Suturing()
        {
            state = 0;
        }
        public void START_SUTURING()
        {
            Console.Write("\nSuturing satrted.\n");
            needle = new Needle();
            trajectory = new Trajectory(); // mode 2
            t = 0;
            state = 1; // state initialization: state 1 indicates entry, state 2 exit and state 3 the suturing
        }
        public void SELECT_ENTRY(Joints joints)
        {
            
            needle.kinematics.joint.UpperBevel = joints.UpperBevel;
            needle.kinematics.joint.LowerBevel = joints.LowerBevel;
            needle.kinematics.joint.Elbow = joints.Elbow;
            needle.kinematics.joint.twist = joints.twist;
            needle.update_needle(); // one-point suturing

            switch (mode)
            {
                case 1: // mode.one_point_suturing
                    state++;
                    state++;
                    break;
                case 2: //mode.two_point_suturing
                    trajectory.entry_point = NeedleKinematics.get_translation(needle.head);
                    //trajectory.set_entry_needle(joints);
                    state++;
                    break;
            }
        }
        public void SELECT_EXIT(Joints joints)
        {

            needle.kinematics.joint.UpperBevel = joints.UpperBevel;
            needle.kinematics.joint.LowerBevel = joints.LowerBevel;
            needle.kinematics.joint.Elbow = joints.Elbow;
            needle.kinematics.joint.twist = joints.twist;
            needle.update_needle();


            trajectory.exit_point = NeedleKinematics.get_translation(needle.head);//mode.two_point_suturing
            trajectory.set_entry_needle(joints);
            trajectory.set_exit_needle(joints);
            state++;

            // delete late
            /*
            Joints traj_joint = trajectory.update_trajectory(joints);
            needle.kinematics.joint.UpperBevel = traj_joint.UpperBevel;
            needle.kinematics.joint.LowerBevel = traj_joint.LowerBevel;
            needle.kinematics.joint.Elbow = traj_joint.Elbow;
            needle.kinematics.joint.twist = traj_joint.twist;
            needle.update_needle();
             * */
            //
        }
        public void SELECT_ENTRY(double x, double y, double z)// delete later
        {
            Console.Write("\nState 1: ENTRY POINT selected\n");
            Vector3D entry_point = new Vector3D(x, y, z);
            //Vector3D entry_point = new Vector3D(-30, 0, 130);
            Print.print_vector(entry_point);
            state++;
        }
        public void SELECT_EXIT(double x, double y, double z)// delete later
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
            switch(mode)
            {
                case 1: // mode.one_point_suturing
                    needle.update_needle(needle.moved_head);
                    Print.PrintMatrixOnFile(needle.center);
                    break;
                case 2: //mode.two_point_suturing
                    //needle = trajectory.needle_entry; //for test
                    needle = trajectory.update_trajectory();
                    Print.PrintMatrixOnFile(needle.center);
                    break;
            }

            t = t + Math.PI / (needle.n - 1);

            // for test and can be deleted
            //Test.specified_pos(needle);
            //
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Forms;

namespace path_generation
{
    public class Suturing0
    {      
        System.Windows.Forms.Timer interpolationTimer = new System.Windows.Forms.Timer();

        public double leftUpperBevel, leftLowerBevel, leftElbow; // for calculating orientation of forearm
        public Trajectory0 trajectory; // ideal trajectory; gives the needle tip at every moment
        public Needle0 needle; // the trajectory will initialize the needle, then will get updated based on new needle tip.
        double t = 0;
        public int state;
        public Suturing0()
        {
            state = 0;
        }
        public void START_SUTURING()
        {
            Console.Write("\nSuturing satrted.\n");
            trajectory = new Trajectory0();
            needle = new Needle0();
            t = 0;
            state = 1; // state initialization: state 1 indicates entry, state 2 exit and state 3 the suturing
        }
        public void SELECT_ENTRY(double x, double y, double z)
        {
            Console.Write("\nState 1: ENTRY POINT selected\n");
            Vector3D entry_point = new Vector3D(x, y, z);
            //Vector3D entry_point = new Vector3D(-30, 0, 130);
            trajectory.entry_point = entry_point;
            //trajectory.entry_point = find_tip_location(entry_point);
            Print0.print_vector(entry_point);
            state++;
        }
        public void SELECT_EXIT(double x, double y, double z)
        {
            Console.Write("\nState 2 EXIT POINT selected\n");
            Vector3D exit_point = new Vector3D(x, y, z);
            //Vector3D exit_point = new Vector3D(-25, 0, 130);
            trajectory.exit_point = exit_point;
            //trajectory.exit_point = find_tip_location(exit_point);
            Print0.print_vector(exit_point);
            state++;
        }
        public bool INITIALIZE_SUTURING()
        {
            if ((trajectory.exit_point - trajectory.entry_point).Length > 2 * trajectory.needle_radius)
            {
                END_SUTURING();
                MessageBox.Show("Entery and exit points are not valid!\nPick the entry and exit points again.");
                return false;
            }
            else
            {
                trajectory.create();
                needle.local_coordinate = trajectory.local_coordinate;
                needle.needle_tip_position = trajectory.needle_tip_position;
                needle.needle_tip_twist = trajectory.needle_tip_twist;
                state++;
                return true;
            }
        }
        public void PRE_SUTURING()
        {
            /*kinematics.update_kinematics(leftUpperBevel, leftLowerBevel, leftElbow, twist);
            Vector3D start_position = new Vector3D(Outputs["X"].Value, Outputs["Y"].Value, Outputs["Z"].Value);
            Vector3D target_position = needle.get_needle_holder_position(kinematics.transformation_matrix(46));
            double start_twist = Outputs["Twist"].Value;
            double target_twist = twist_correction(needle.get_needle_holder_twist());

            if (vector_interpolation(start_position, target_position) & digit_interpolation(start_twist, target_twist))
                state++;
            System.Threading.Thread.Sleep(100);*/
        }
        public bool DO_SUTURING()
        {
            t = t + trajectory.incr;
            Console.Write("\n****************t: {0}\n", t);
            needle.needle_tip_position = trajectory.needle_tip_position;
            needle.needle_tip_twist = trajectory.needle_tip_twist;
            
            /* test
            Matrix3D T4 = needle.kinematics.transformation_matrix(4);
            Matrix3D T5 = needle.kinematics.transformation_matrix(5);
            Matrix3D T6 = needle.kinematics.transformation_matrix(6);
            Vector3D v = new Vector3D(-28, 0, 0);
            Vector3D v0 = new Vector3D(0, 0, 0);
            Vector3D v5, v6, v66;
            
            v5 = new Vector3D(T5.M14, T5.M24, T5.M34);
            v6 = NeedleKinematics.transform(T6, v0);
            v66 = NeedleKinematics.transform(T5, v);


            v6 = NeedleKinematics.correction(v6);
            v5 = NeedleKinematics.correction(v5);
             * */
            needle.update_needle_holder_position(needle.kinematics.transformation_matrix(46));
            //needle.update_needle_holder_position();
            needle.update_needle_holder_twist();

            // Update trajectory for next around
            trajectory.update_needle_tip_position();
            trajectory.update_needle_tip_twist();

            if (t >= 2 * Math.PI)
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

            // set up interpolation timer
            //interpolationTimer.Interval = 50;
            //interpolationTimer.Tick += interpolationTimer_Tick;
            //interpolationTimer.Start();
        }
        private double twist_correction(double t)
        {
            return (-t * 180 / Math.PI);
        }
        /*private bool vector_interpolation(Vector3D start, Vector3D target)
        {
            Vector3D mid = new Vector3D();
            Vector3D line = target - start;
            double length = line.Length;

            if (length > 5)
            {
                mid = start + 5 * line / length;
                update_output(mid);
                return false;
            }
            else
            {
                mid = target;
                autoSuturing.
                update_output(mid);
                return true;
            }
        }
        private bool digit_interpolation(double start, double target)
        {
            double mid;
            double difference = target - start;
            if (Math.Abs(difference) > 5)
            {
                mid = start + 5 * Math.Sign(difference);
                update_output(mid);
                return false;
            }
            else
            {
                mid = target;
                update_output(mid);
                return true;
            }
        }

        private void interpolationTimer_Tick(object sender, EventArgs e)
        {
            while (!digit_interpolation(Outputs["Twist"].Value, 0)) ;
            interpolationTimer.Stop();
        }*/
    }
}

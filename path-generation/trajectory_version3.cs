using System;
using System.Windows.Media.Media3D;

namespace path_generation
{
    public class trajectory_version3
    {
        private Vector3D circle_center; // position of needle in the grasper;
        private static Vector3D needle_holder_position;
        private static Vector3D needle_tip_position;

        static double needle_holder_twist;
        double delta_theta;
        Vector3D e_u, e_v, e_x, e_y, circle_normal;
        private double r = 20; // radius of the needle
        //double LengthUpperArm = 68.58;
        //double LengthForearm = 96.393;
        private double t_incr = Math.PI / 10;
        static Vector3D ideal_needle_orientation = new Vector3D();
        static Vector3D optimal_needle_orientation = new Vector3D();

        public trajectory_version3()
        {
            //xc = 0; // center of the needle
            //yc = 0;
            //zc = 130;
        }
        public trajectory_version3(Vector3D entry_point, Vector3D exit_point)
        {
            e_u = new Vector3D(); //e: unit vector
            e_u = exit_point - entry_point;
            circle_normal = new Vector3D();
            e_x = new Vector3D(e_u.X, 0, e_u.Z);
            e_x = e_x / e_x.Length;
            e_y = new Vector3D(0, 1, 0);
            circle_normal = Vector3D.CrossProduct(e_y, e_u);
            circle_normal = circle_normal / circle_normal.Length;
            e_v = new Vector3D();
            e_v = Vector3D.CrossProduct(circle_normal, e_u);
            Vector3D offset = new Vector3D();
            offset = Math.Sqrt(r*r-.25 * e_u.Length*e_u.Length) * e_v/e_v.Length;
            circle_center = new Vector3D(0.5 * (entry_point.X + exit_point.X), 0.5 * (entry_point.Y + exit_point.Y), 0.5 * (entry_point.Z + exit_point.Z));
            circle_center = circle_center - offset;
            //needle_holder_position = 2 * circle_center - entry_point;
            needle_tip_position = entry_point;


            /*
            Vector3D temp = new Vector3D();
            temp = needle_holder_position - circle_center;
            needle_holder_twist = Math.Acos(Vector3D.DotProduct(temp, e_x) / (temp.Length * e_x.Length));
             * */
            print_vector(entry_point);
            print_vector(exit_point);
            print_vector(circle_center);
            print_vector(needle_holder_position);
        }

        private void update_needle_tip_position()
        {
            Vector3D temp = new Vector3D();
            temp = needle_tip_position - circle_center;
            Quaternion q_needle_tip_position = new Quaternion(temp.X, temp.Y, temp.Z, 0);
            //print_quaternion(q_needle_holder_position);

            Quaternion q_circle_normal = new Quaternion(Math.Sin(t_incr / 2) * circle_normal.X, Math.Sin(t_incr / 2) * circle_normal.Y, Math.Sin(t_incr / 2) * circle_normal.Z, Math.Cos(t_incr / 2));
            Quaternion q_circle_normal_conjugate = new Quaternion();
            q_circle_normal_conjugate = q_circle_normal;
            q_circle_normal_conjugate.Conjugate();
            q_needle_tip_position = q_circle_normal * q_needle_tip_position * q_circle_normal_conjugate;
            needle_tip_position.X = q_needle_tip_position.X + circle_center.X;
            needle_tip_position.Y = q_needle_tip_position.Y + circle_center.Y;
            needle_tip_position.Z = q_needle_tip_position.Z + circle_center.Z;
            //print_quaternion(q_circle_normal);
            //print_quaternion(q_needle_holder_position);
        }
        private void update_ideal_needle_orientation()
        {
            Vector3D centric = new Vector3D();
            centric = circle_center - needle_tip_position;
            ideal_needle_orientation = Vector3D.CrossProduct(centric, circle_normal);
            ideal_needle_orientation = ideal_needle_orientation / ideal_needle_orientation.Length;
        }
        private void update_optimal_needle_orientation(Vector3D forearm_orientation)
        {
            Vector3D temp = new Vector3D();
            temp = Vector3D.CrossProduct(ideal_needle_orientation, forearm_orientation);
            optimal_needle_orientation = Vector3D.CrossProduct(forearm_orientation, temp);
        }
        private void update_needle_holder_position(Vector3D forearm_orientation)
        {
            Vector3D e_r = new Vector3D();
            e_r = Vector3D.CrossProduct(optimal_needle_orientation, forearm_orientation);
            e_r = e_r / e_r.Length;
            needle_holder_position = needle_tip_position + r * e_r;
            /*Vector3D axis = new Vector3D();
            axis = Vector3D.CrossProduct(circle_normal, forearm_orientation);
            axis = axis / axis.Length;
            double angle;
            angle = Math.Acos(Vector3D.DotProduct(circle_normal,forearm_orientation)/(circle_normal.Length*forearm_orientation.Length));
            //needle_holder_position = 2 * circle_center - needle_tip_position;
            Vector3D temp = new Vector3D();
            temp = 2 * (circle_center - needle_tip_position);
            Quaternion q_needle_holder_position = new Quaternion(temp.X, temp.Y, temp.Z, 0);

            Quaternion q_axis = new Quaternion(Math.Sin(angle / 2) * axis.X, Math.Sin(angle / 2) * axis.Y, Math.Sin(angle / 2) * axis.Z, Math.Cos(angle / 2));
            Quaternion q_axis_conjugate = new Quaternion();
            q_axis_conjugate = q_axis;
            q_axis_conjugate.Conjugate();
            q_needle_holder_position = q_axis * q_needle_holder_position * q_axis_conjugate;
            needle_holder_position.X = q_needle_holder_position.X + needle_tip_position.X;
            needle_holder_position.Y = q_needle_holder_position.Y + needle_tip_position.Y;
            needle_holder_position.Z = q_needle_holder_position.Z + needle_tip_position.Z;
             * */

        }

        public dof4 end_effector(Vector3D forearm_orientation, double delta_theta) //trajectory() in MATLAB; calculation position of end effector and ideal orientation of the needle
        {
            needle_holder_twist = needle_holder_twist  + t_incr;
            update_needle_tip_position();
            update_ideal_needle_orientation();
            update_optimal_needle_orientation(forearm_orientation);
            update_needle_holder_position(forearm_orientation);
            dof4 needle_holder;
            needle_holder.pos = needle_holder_position;
            needle_holder.twist = needle_holder_twist;

            print_vector(needle_holder_position);
            print_double(needle_holder_twist);
            /*
            set_circle_center(); // calculating center of needle
            // calculating position
            DOF.pos = new Vector3D(xc + r * Math.Sin(t), yc + r * Math.Cos(t), zc);


            // calculation orientation
            Vector3D centric1 = new Vector3D(r * Math.Sin(t), r * Math.Cos(t), 0);
            Vector3D centric2 = new Vector3D(r * Math.Sin(t + t_incr), r * Math.Cos(t + t_incr), 0);
            Vector3D normal = new Vector3D();
            //Vector3D ideal_needle_orientation = new Vector3D();
            normal = Vector3D.CrossProduct(centric1, centric2); // normal of circle plane
            ideal_needle_orientation = Vector3D.CrossProduct(centric1, normal); // which is the tangent of the path
            Vector3D optimal_needle_orientation = new Vector3D();
            optimal_needle_orientation = minimizer(forearm_orientation);
            DOF.ori = optimal_needle_orientation;
            */
            return needle_holder;
        }
        private Vector3D minimizer(Vector3D forearm_orientation)
        {
            // This calculates the optimal needle orientaion such that its angle with path tangential is minimized
            Vector3D optimal_needle_orientation = new Vector3D();

            double[] x = new double[] { 1, 1, 1 };
            double[,] c = new double[,] { { forearm_orientation.X, forearm_orientation.Y, forearm_orientation.Z, 0 } };
            int[] ct = new int[] { 0 };
            alglib.minbleicstate state;
            alglib.minbleicreport rep;
            double epsg = 0.000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;
            alglib.minbleiccreate(x, out state);
            alglib.minbleicsetlc(state, c, ct);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minbleicoptimize(state, minimizer_fun, null, null);
            alglib.minbleicresults(state, out x, out rep);

            return optimal_needle_orientation;
        }
        private static void minimizer_fun(double[] x, ref double func, double[] grad, object obj)
        {
            func = -(x[0] * ideal_needle_orientation.X + x[1] * ideal_needle_orientation.Y + x[2] * ideal_needle_orientation.Z);
        }
        public void print_quaternion(Quaternion q)
        {
            Console.Write("\n [{0}, {1}, {2}]\t w: {3}\n", q.X, q.Y, q.Z, q.W);
        }
        public void print_vector(Vector3D v)
        {
            Console.Write("\n [{0}, {1}, {2}]\n", v.X, v.Y, v.Z);
        }
        public void print_double(double d)
        {
            Console.Write("\n {0}\n", d);
        }
        /*public point end_effector(Vector3D forearm_orientation, double t) //trajectory() in MATLAB; calculation position of end effector and ideal orientation of the needle
        {
            point DOF;
            set_circle_center(); // calculating center of needle
            // calculating position
            DOF.pos = new Vector3D(xc + r * Math.Sin(t), yc + r * Math.Cos(t), zc);


            // calculation orientation
            Vector3D centric1 = new Vector3D(r * Math.Sin(t), r * Math.Cos(t), 0);
            Vector3D centric2 = new Vector3D(r * Math.Sin(t + t_incr), r * Math.Cos(t + t_incr), 0);
            Vector3D normal = new Vector3D();
            //Vector3D ideal_needle_orientation = new Vector3D();
            normal = Vector3D.CrossProduct(centric1, centric2); // normal of circle plane
            ideal_needle_orientation = Vector3D.CrossProduct(centric1, normal); // which is the tangent of the path
            Vector3D optimal_needle_orientation = new Vector3D();
            optimal_needle_orientation = minimizer(forearm_orientation);
            DOF.ori = optimal_needle_orientation;

            return DOF;
        }
         */
        // MATLAB minimizer
        /*
        private Vector minimizer(Vector ori2, Vector tangential)
        {
            double ori2_x = ori2.x;
            double ori2_y = ori2.y;
            double ori2_z = ori2.z;
            double tangential_x = tangential.x;
            double tangential_y = tangential.y;
            double tangential_z = tangential.z;
            minimizerClass obj = new minimizerClass();
            MWArray[] output = new MWArray[4];
            output = obj.minimizerFunction(4,(MWArray)ori2_x, (MWArray)ori2_y, (MWArray)ori2_z, (MWArray)tangential_x, (MWArray)tangential_y, (MWArray)tangential_z);
            //Console.Write("The minimizer optimizes the path as below: {0}", output);
            //double[,] arr = (double[,])((MWArray)output).ToArray();
            //MWNumericArray x = (MWNumericArray)output[1];
            double[,] x = (double[,])((MWNumericArray)output[0]).ToArray(MWArrayComponent.Real);
            double[,] y = (double[,])((MWNumericArray)output[1]).ToArray(MWArrayComponent.Real);
            double[,] z = (double[,])((MWNumericArray)output[2]).ToArray(MWArrayComponent.Real);
            double[,] error = (double[,])((MWNumericArray)output[3]).ToArray(MWArrayComponent.Real);

            
            //MWNumericArray output2 = new MWNumericArray();
            //output2 = (MWNumericArray)obj.minimizerFunction((MWArray)ori2_x, (MWArray)ori2_y, (MWArray)ori2_z, (MWArray)tangential_x, (MWArray)tangential_y, (MWArray)tangential_z);
            //double[,] nativeOutput = (double[,])((MWNumericArray)output2).ToArray();
             
            Vector needle = new Vector();
            needle.x = x[0,0];
            needle.y = y[0, 0];
            needle.z = z[0, 0];
            return needle;
        }
         */

    }
}

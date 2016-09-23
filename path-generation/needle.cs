using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using minimizerNamespace;
using System.Windows.Media.Media3D;

namespace path_generation
{
    public class Needle
    {
        public Coordinate local_coordinate;
        public Coordinate twisted_local_coordinate;   

        private static Vector3D needle_holder_position;
        private Vector3D needle_tip_position;
        private double needle_holder_twist = 0;
        private double needle_tip_twist;

        public Needle()
        {
        }
        public Needle(Vector3D needle_tip_position)
        {
            Vector3D circle_center = new Vector3D();
            circle_center = local_coordinate.origin;
            this.needle_tip_position = needle_tip_position;
            needle_holder_position = 2 * circle_center - needle_tip_position;

            print_vector(needle_holder_position);
        }


        public void set_needle_tip_position(Vector3D needle_tip_position)
        {
            this.needle_tip_position = needle_tip_position;
        }
        public void set_needle_tip_twist(double needle_tip_twist)
        {
            this.needle_tip_twist = needle_tip_twist;
        }
        public Vector3D get_needle_holder_position()
        {
            Vector3D circle_center = new Vector3D();
            circle_center = local_coordinate.origin;
            needle_holder_position = 2 * circle_center - needle_tip_position;
            return needle_holder_position;
        }
        public double get_needle_holder_twist()
        {
            return needle_tip_twist;

        }

        

        /*public dof4 end_effector(Vector3D forearm_orientation, double delta_theta) //trajectory() in MATLAB; calculation position of end effector and ideal orientation of the needle
        {
            needle_holder_twist = needle_holder_twist  + t_incr;
            update_needle_holder_position();
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
            /
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
        }*/
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

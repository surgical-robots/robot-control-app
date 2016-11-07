using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace path_generation.OnePointSuturing
{
    // minlm_d_vb example: http://www.alglib.net/translator/man/manual.csharp.html#example_minlm_d_v
    class Optimizer
    {
        public NeedleKinematics kinematics;
        public Matrix3D T_taget;
        private Matrix3D T_FK;
        public Point4D point_target;
        public double[] x;
        public Optimizer()
        {
            kinematics = new NeedleKinematics();
            //x = new double[] { 0, 0, 0, 0 };
        }
        public Joints minimize_error()
        {
            //double[] x = new double[] { 0, 0, 0, 0 };

            double[] bndl = new double[] { -90, -90, 0, -180 };
            double[] bndu = new double[] { 90, 90, 180, 180 };
            double epsg = 0.0000000000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;
            alglib.minlmstate state;
            alglib.minlmreport rep;

            alglib.minlmcreatev(16, x, 0.0000000001, out state);
            alglib.minlmsetbc(state, bndl, bndu);
            alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlmoptimize(state, function_fvec, null, null);
            alglib.minlmresults(state, out x, out rep);

            Joints joints;
            joints.UpperBevel = x[0];
            joints.LowerBevel = x[1];
            joints.Elbow = x[2];
            joints.twist = x[3];
            return joints;
        }

        private void function_fvec(double[] x, double[] fi, object obj) // to reach a point with closest position and orientation
        {
            // updating joints
            kinematics.joint.UpperBevel = x[0];
            kinematics.joint.LowerBevel = x[1];
            kinematics.joint.Elbow = x[2];
            kinematics.joint.twist = x[3];
            // calculating the forward kinematics for the given joints value
            T_FK = kinematics.transformation_matrix(55);
            // calculating errors fi s
            fi[0] = Math.Pow(T_taget.M11 - T_FK.M11, 2);
            fi[1] = Math.Pow(T_taget.M12 - T_FK.M12, 2);
            fi[2] = Math.Pow(T_taget.M13 - T_FK.M13, 2);
            fi[3] = Math.Pow(T_taget.M14 - T_FK.M14, 2);
            fi[4] = Math.Pow(T_taget.M21 - T_FK.M21, 2);
            fi[5] = Math.Pow(T_taget.M22 - T_FK.M22, 2);
            fi[6] = Math.Pow(T_taget.M23 - T_FK.M23, 2);
            fi[7] = Math.Pow(T_taget.M24 - T_FK.M24, 2);
            fi[8] = Math.Pow(T_taget.M31 - T_FK.M31, 2);
            fi[9] = Math.Pow(T_taget.M32 - T_FK.M32, 2);
            fi[10] = Math.Pow(T_taget.M33 - T_FK.M33, 2);
            fi[11] = Math.Pow(T_taget.M34 - T_FK.M34, 2);
        }
        public Joints minimize_angle()
        {
            //double[] x = new double[] { 0, 0, 0, 0 };

            double[] bndl = new double[] { -90, -90, 0, -180 };
            double[] bndu = new double[] { 90, 90, 180, 180 };
            double epsg = 0.0000000000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;
            alglib.minlmstate state;
            alglib.minlmreport rep;

            alglib.minlmcreatev(4, x, 0.0000000001, out state);
            alglib.minlmsetbc(state, bndl, bndu);
            alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlmoptimize(state, function_fvec2, null, null);
            alglib.minlmresults(state, out x, out rep);

            Joints joints;
            joints.UpperBevel = x[0];
            joints.LowerBevel = x[1];
            joints.Elbow = x[2];
            joints.twist = x[3];
            return joints;
        }
        private void function_fvec2(double[] x, double[] fi, object obj) // to reach a point with closest position and a specified angle to surface
        {
            // updating joints
            kinematics.joint.UpperBevel = x[0];
            kinematics.joint.LowerBevel = x[1];
            kinematics.joint.Elbow = x[2];
            kinematics.joint.twist = x[3];
            // calculating the forward kinematics for the given joints value
            T_FK = kinematics.transformation_matrix(55);
            // calculating errors fi s
            fi[0] = Math.Pow(point_target.X - T_FK.M14, 2);
            fi[1] = Math.Pow(point_target.Y - T_FK.M24, 2);
            fi[2] = Math.Pow(point_target.Z - T_FK.M34, 2);
            Vector3D v1 = new Vector3D(0, 0, 1); // z of kinematic frame
            Vector3D v2 = new Vector3D(T_FK.M12, T_FK.M22, T_FK.M32);
            double angle = Vector3D.AngleBetween(v1, v2); ;
            fi[3] = Math.Pow(point_target.W - angle, 2);
            //Console.Write("\noptimizing angle: {0}", angle);
            //Console.Write("\noptimizing position: {0}, {1}, {2}", T_FK.M14, T_FK.M24, T_FK.M34);
        }
        public Joints minimize_center() // find a needle with same orientation that maches the given center
        {
            //double[] x = new double[] { 0, 0, 0, 0 };

            double[] bndl = new double[] { -90, -90, 0, -180 };
            double[] bndu = new double[] { 90, 90, 180, 180 };
            double epsg = 0.0000000000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;
            alglib.minlmstate state;
            alglib.minlmreport rep;

            alglib.minlmcreatev(16, x, 0.0000000001, out state);
            alglib.minlmsetbc(state, bndl, bndu);
            alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlmoptimize(state, function_fvec3, null, null);
            alglib.minlmresults(state, out x, out rep);

            Joints joints;
            joints.UpperBevel = x[0];
            joints.LowerBevel = x[1];
            joints.Elbow = x[2];
            joints.twist = x[3];
            return joints;
        }

        private void function_fvec3(double[] x, double[] fi, object obj) // to reach a point with closest position and orientation
        {
            // updating joints
            kinematics.joint.UpperBevel = x[0];
            kinematics.joint.LowerBevel = x[1];
            kinematics.joint.Elbow = x[2];
            kinematics.joint.twist = x[3];
            // calculating the forward kinematics for the given joints value
            T_FK = kinematics.transformation_matrix(5); //this gives center frame
            // calculating errors fi s
            fi[0] = Math.Pow(T_taget.M11 - T_FK.M11, 2);
            fi[1] = Math.Pow(T_taget.M12 - T_FK.M12, 2);
            fi[2] = Math.Pow(T_taget.M13 - T_FK.M13, 2);
            fi[3] = Math.Pow(T_taget.M14 - T_FK.M14, 2);
            fi[4] = Math.Pow(T_taget.M21 - T_FK.M21, 2);
            fi[5] = Math.Pow(T_taget.M22 - T_FK.M22, 2);
            fi[6] = Math.Pow(T_taget.M23 - T_FK.M23, 2);
            fi[7] = Math.Pow(T_taget.M24 - T_FK.M24, 2);
            fi[8] = Math.Pow(T_taget.M31 - T_FK.M31, 2);
            fi[9] = Math.Pow(T_taget.M32 - T_FK.M32, 2);
            fi[10] = Math.Pow(T_taget.M33 - T_FK.M33, 2);
            fi[11] = Math.Pow(T_taget.M34 - T_FK.M34, 2);
        }
        public Joints minimize_postionNtwist() // find a needle with given position and twist
        {
            //double[] x = new double[] { 0, 0, 0, 0 };

            double[] bndl = new double[] { -90, -90, 0, -180 };
            double[] bndu = new double[] { 90, 90, 180, 180 };
            double epsg = 0.0000000000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;
            alglib.minlmstate state;
            alglib.minlmreport rep;

            alglib.minlmcreatev(3, x, 0.0000000001, out state);
            alglib.minlmsetbc(state, bndl, bndu);
            alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlmoptimize(state, function_fvec4, null, null);
            alglib.minlmresults(state, out x, out rep);

            Joints joints;
            joints.UpperBevel = x[0];
            joints.LowerBevel = x[1];
            joints.Elbow = x[2];
            joints.twist = point_target.W;
            return joints;
        }

        private void function_fvec4(double[] x, double[] fi, object obj)
        {
            // updating joints
            kinematics.joint.UpperBevel = x[0];
            kinematics.joint.LowerBevel = x[1];
            kinematics.joint.Elbow = x[2];
            kinematics.joint.twist = point_target.W;
            // calculating the forward kinematics for the given joints value
            T_FK = kinematics.transformation_matrix(55);
            // calculating errors fi s
            fi[0] = Math.Pow(point_target.X - T_FK.M14, 2);
            fi[1] = Math.Pow(point_target.Y - T_FK.M24, 2);
            fi[2] = Math.Pow(point_target.Z - T_FK.M34, 2);
        }
    }
}

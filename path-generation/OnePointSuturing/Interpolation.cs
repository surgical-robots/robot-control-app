using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abacus.DoublePrecision;
//using System.Windows.Media.Media3D; //conflicting with Abacus
namespace path_generation.OnePointSuturing
{
    class Interpolation
    {
        //public System.Windows.Media.Media3D.Matrix3D M_initial, M_target;
        private Quaternion q1, q2;
        private Matrix44 m1, m2;
        private static double t;

        BezierInterpolation bezierInterpolation;
        public Interpolation()
        {
            t = 0;
            bezierInterpolation = new BezierInterpolation();
        }
        public void initialize(System.Windows.Media.Media3D.Matrix3D M_initial, System.Windows.Media.Media3D.Matrix3D M_target)
        {
            m1 = matrix3Dto44(M_initial);
            q1 = Quaternion.CreateFromRotationMatrix(m1);
            m2 = matrix3Dto44(M_target);
            q2 = Quaternion.CreateFromRotationMatrix(m2);
            bezierInterpolation.initialize(M_initial, M_target);
        }
        public System.Windows.Media.Media3D.Vector3D interpolate_center(Needle needle_entry, Needle needle_exit)
        {
            Vector3 v1 = new Vector3(needle_entry.center.M14, needle_entry.center.M24, needle_entry.center.M34);
            Vector3 v2 = new Vector3(needle_exit.center.M14, needle_exit.center.M24, needle_exit.center.M34);

            Vector3 v_mid = Vector3.Lerp(v1, v2, t);
            t = t + .1;
            if (t > 1)
                t = 1;
            System.Windows.Media.Media3D.Vector3D output = new System.Windows.Media.Media3D.Vector3D(v_mid.X, v_mid.Y, v_mid.Z);
            return output;
        }
        public System.Windows.Media.Media3D.Matrix3D update(System.Windows.Media.Media3D.Matrix3D current_needle_mid_move_head)
        {
            /*
            Quaternion q;
            q = Quaternion.Slerp(q1, q2, t);
            t = t + .1; // need a point to stop
            Matrix44 m = Matrix44.CreateFromQuaternion(q);
            Vector3 trans=Vector3.Lerp()
            return matrix44to3D(m);
             * */
            //Matrix44 m = Matrix44.Lerp(m1, m2, t);
            
            Matrix44 m = matrix3Dto44(current_needle_mid_move_head);
            System.Windows.Media.Media3D.Vector3D v = bezierInterpolation.bezier(t);
            m.R0C3 = v.X;
            m.R1C3 = v.Y;
            m.R2C3 = v.Z;
            t = t + 0.1; // need a point to stop
            if (t > 1)
                t = 1;
            return matrix44to3D(m);
        }
        private Matrix44 matrix3Dto44(System.Windows.Media.Media3D.Matrix3D m3D)
        {
            Matrix44 m44 = new Matrix44(m3D.M11, m3D.M12, m3D.M13, m3D.M14,
                                        m3D.M21, m3D.M22, m3D.M23, m3D.M24,
                                        m3D.M31, m3D.M32, m3D.M33, m3D.M34,
                                        m3D.OffsetX, m3D.OffsetY, m3D.OffsetZ, m3D.M44);
            return m44;
        }
        private System.Windows.Media.Media3D.Matrix3D matrix44to3D(Matrix44 m44)
        {
            System.Windows.Media.Media3D.Matrix3D m3D = new System.Windows.Media.Media3D.Matrix3D(m44.R0C0, m44.R0C1, m44.R0C2, m44.R0C3,
                                                                                                    m44.R1C0, m44.R1C1, m44.R1C2, m44.R1C3,
                                                                                                    m44.R2C0, m44.R2C1, m44.R2C2, m44.R2C3,
                                                                                                    m44.R3C0, m44.R3C1, m44.R3C2, m44.R3C3);
            return m3D;
        }
    }
}

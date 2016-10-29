using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
namespace path_generation.OnePointSuturing
{
    class BezierInterpolation
    {
        Matrix3D M_initial, M_target;
        Vector3D p0, p1, p2;
        public void initialize(Matrix3D M_initial, Matrix3D M_target)
        {
            this.M_initial = M_initial;
            this.M_target = M_target;
            get_p0();
            get_p2();
            get_p1();
        }
        void get_p0()
        {
            this.p0 = NeedleKinematics.get_translation(M_initial);
        }
        void get_p2()
        {
            this.p2 = NeedleKinematics.get_translation(M_target);
        }
        void get_p1()
        {
            Vector3D u0 = NeedleKinematics.get_ez(M_initial); //u0 orientation of needle
            Vector3D u2 = NeedleKinematics.get_ez(M_target); // might be inverse
            Vector3D p1 = intersect(u0, p0, u2, p2);
            this.p1 = p1;
        }
        static Vector3D intersect(Vector3D u1, Vector3D p1, Vector3D u2, Vector3D p2)
        {
            //Vector3D vector = new Vector3D((u2.X * p1.X - u1.X * p2.X) / (u2.X - u1.X), (u2.Y * p1.Y - u1.Y * p2.Y) / (u2.Y - u1.Y), (u2.Z * p1.Z - u1.Z * p2.Z) / (u2.Z - u1.Z));
            double x, y, z;
            double x1 = p1.X, y1 = p1.Y, z1 = p1.Z, x2 = p2.X, y2 = p2.Y, z2 = p2.Z;
            double a1 = u1.X, b1 = u1.Y, c1 = u1.Z, a2 = u2.X, b2 = u2.Y, c2 = u2.Z;
            y = ((x2 - x1) + (a1 / b1 * y1 - a2 / b2 * y2)) / (a1 / b1 - a2 / b2);
            x = x1 + a1 / b1 * (y - y1);
            double t1, t2;
            t1 = (x - x1) / a1;
            t2 = (x - x2) / a2;
            z = .5 * (z1 + c1 * t1) + .5 * (z2 + c2 * t2);
            Vector3D vector = new Vector3D(x, y, z);
            return vector;
        }
        public Vector3D bezier(double t)
        {
            return Math.Pow(1 - t, 2) * p0 + 2 * t * (1 - t) * p1 + Math.Pow(t, 2) * p2;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace path_generation
{
    public class Circle
    {
        public const double radius = 14;
        /*public Coordinate local_coordinate;
        public Circle(double radius, Vector3D one_point)
        {
            double twist = 0; // assumed
            Vector3D normal = new Vector3D(0, 0, 1);// assumed
            this.radius = radius;

            // caclulating axises
            this.local_coordinate.e_x = new Vector3D(1, 0, 0);
            this.local_coordinate.e_y = new Vector3D(0, 1, 0);
            this.local_coordinate.e_z = new Vector3D(normal.X, normal.Y, normal.Z);
            // calculating center
            Vector3D center = new Vector3D();
            center = one_point;
            center.X = one_point.X + radius;
            this.local_coordinate.origin = new Vector3D(center.X, center.Y, center.Z);
        }
        public Circle(double radius, double twist, Vector3D normal, Vector3D one_point)
        {
            this.radius = radius;
            this.local_coordinate.e_z = normal;
            double d = -(normal.X * one_point.X + normal.Y * one_point.Y + normal.Z * one_point.Z);
            Vector3D e_x = new Vector3D(1, 0, 0);
            Vector3D e_y = new Vector3D(0, 1, 0);
            Vector3D e_z = new Vector3D(0, 0, 1);
            Vector3D mutual = Vector3D.CrossProduct(e_z, normal);
        }
        public Vector3D rotaion(Vector3D axis, Vector3D point, double angle)
        {
            Quaternion q_point = new Quaternion(point.X, point.Y, point.Z, 0);
            Quaternion q_axis = new Quaternion(Math.Sin(-angle / 2) * axis.X, Math.Sin(-angle / 2) * axis.Y, Math.Sin(-angle / 2) * axis.Z, Math.Cos(-angle / 2));
            Quaternion q_axis_conjugate = new Quaternion();
            q_axis_conjugate = q_axis;
            q_axis_conjugate.Conjugate();
            q_point = q_axis * q_point * q_axis_conjugate;
            Vector3D output = new Vector3D(q_point.X, q_point.Y, q_point.Z);
            return output;
        }*/
    }
}

using System;
using System.Windows.Media.Media3D;

namespace path_generation.TwoPointSuturing
{
    public class Trajectory
    {
        public double needle_radius = 14; // radius of the needle
        public Vector3D entry_point;
        public Vector3D exit_point;
        public Vector3D center;
        public Vector3D normal;
        public int number_of_points = 20;
        public Coordinate local_coordinate;
        public Coordinate twisted_local_coordinate;
        public double incr { get; private set; }

        public Vector3D needle_tip_position;
        public double needle_tip_twist { get; private set; }

        public Trajectory()
        {
        }
        public Trajectory(Vector3D entry_point, Vector3D exit_point)
        {
            this.entry_point = entry_point;
            this.exit_point = exit_point;
            create();

        }
        public void create() // enrty and exit point must be given beforehand
        {
            incr = Math.PI / number_of_points;
            local_coordinate.origin = new Vector3D();
            local_coordinate.e_x = new Vector3D();
            local_coordinate.e_y = new Vector3D();
            local_coordinate.e_z = new Vector3D();
            twisted_local_coordinate.origin = new Vector3D();
            twisted_local_coordinate.e_x = new Vector3D();
            twisted_local_coordinate.e_y = new Vector3D();
            twisted_local_coordinate.e_z = new Vector3D();
            twisted_local_coordinate.e_x = new Vector3D(); //e: unit vector
            twisted_local_coordinate.e_x = exit_point - entry_point;
            local_coordinate.e_x = new Vector3D(twisted_local_coordinate.e_x.X, 0, twisted_local_coordinate.e_x.Z);
            local_coordinate.e_x = local_coordinate.e_x / local_coordinate.e_x.Length;
            local_coordinate.e_y = new Vector3D(0, 1, 0);
            local_coordinate.e_z = Vector3D.CrossProduct(local_coordinate.e_y, twisted_local_coordinate.e_x);
            local_coordinate.e_z = local_coordinate.e_z / local_coordinate.e_z.Length;
            twisted_local_coordinate.e_y = Vector3D.CrossProduct(local_coordinate.e_z, twisted_local_coordinate.e_x);
            Vector3D offset = new Vector3D();
            offset = Math.Sqrt(needle_radius * needle_radius - .25 * twisted_local_coordinate.e_x.Length * twisted_local_coordinate.e_x.Length) * twisted_local_coordinate.e_y / twisted_local_coordinate.e_y.Length;
            local_coordinate.origin = new Vector3D(0.5 * (entry_point.X + exit_point.X), 0.5 * (entry_point.Y + exit_point.Y), 0.5 * (entry_point.Z + exit_point.Z));
            local_coordinate.origin = local_coordinate.origin - offset;
            twisted_local_coordinate.origin = local_coordinate.origin;
            twisted_local_coordinate.e_z = local_coordinate.e_z;

            needle_tip_position = entry_point;
            needle_tip_twist = Math.Acos(Vector3D.DotProduct((needle_tip_position - local_coordinate.origin), local_coordinate.e_x) / ((needle_tip_position - local_coordinate.origin).Length * local_coordinate.e_x.Length));
            needle_tip_twist = Math.PI - needle_tip_twist;

            center = new Vector3D();
            center = local_coordinate.origin;
            normal = new Vector3D();
            normal = local_coordinate.e_z;
            Console.Write("\nreal entry\n");
            Print.print_vector(entry_point);
            Console.Write("\nreal exit\n");
            Print.print_vector(exit_point);
            Console.Write("\ncenter\n");
            Print.print_vector(center);

        }
        public Vector3D update_needle_tip_position()
        {
            Vector3D circle_normal = new Vector3D();
            circle_normal = local_coordinate.e_z;
            Vector3D circle_center = new Vector3D();
            circle_center = local_coordinate.origin;
            Vector3D temp = new Vector3D();
            temp = needle_tip_position - circle_center;
            Quaternion q_needle_tip_position = new Quaternion(temp.X, temp.Y, temp.Z, 0);
            Quaternion q_circle_normal = new Quaternion(Math.Sin(-incr / 2) * circle_normal.X, Math.Sin(-incr / 2) * circle_normal.Y, Math.Sin(-incr / 2) * circle_normal.Z, Math.Cos(-incr / 2));
            Quaternion q_circle_normal_conjugate = new Quaternion();
            q_circle_normal_conjugate = q_circle_normal;
            q_circle_normal_conjugate.Conjugate();
            q_needle_tip_position = q_circle_normal * q_needle_tip_position * q_circle_normal_conjugate;
            needle_tip_position.X = q_needle_tip_position.X + circle_center.X;
            needle_tip_position.Y = q_needle_tip_position.Y + circle_center.Y;
            needle_tip_position.Z = q_needle_tip_position.Z + circle_center.Z;
            /*
            needle_tip_position = rotateAboutAxis(normal, needle_tip_position - center, incr);
            needle_tip_position = needle_tip_position + center;
            */
            return needle_tip_position;
        }
        private Vector3D rotateAboutAxis(Vector3D axis, Vector3D point, double angle)
        {
            Vector3D rotated = new Vector3D();
            Matrix3D matrix = Matrix3D.Identity;
            Quaternion q_axis = new Quaternion(Math.Sin(angle / 2) * axis.X, Math.Sin(angle / 2) * axis.Y, Math.Sin(angle / 2) * axis.Z, Math.Cos(angle / 2));
            matrix.Rotate(q_axis);
            rotated = matrix.Transform(point);

            return rotated;
        }
        public double update_needle_tip_twist()
        {
            needle_tip_twist = needle_tip_twist + incr;
            return needle_tip_twist;
        }




    }
}

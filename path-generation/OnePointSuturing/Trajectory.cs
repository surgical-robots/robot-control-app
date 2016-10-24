using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace path_generation.OnePointSuturing
{
    public class Trajectory
    {
        // needle constants
        double radius = 14;
        int n = 20;

        // trajectory variables
        //Matrix3D center;
        public Matrix3D head;
        public Vector3D entry_point;
        public Vector3D exit_point;
        Matrix3D[] points;

        // defines needle instants
        public Needle needle_entry, needle_exit;

        public Trajectory()
        {
            head = new Matrix3D();
            needle_entry = new Needle();
            needle_exit = new Needle();
        }
        public void set_exit_needle(Joints joint)
        {
            double central_angle = 180 - get_central_angle();
            Optimizer optimizer = new Optimizer();
            Point4D target = new Point4D(exit_point.X, exit_point.Y, exit_point.Z, central_angle);
            Console.Write("\noTarget exit: {0}, {1}, {2}, {3}", exit_point.X, exit_point.Y, exit_point.Z, central_angle);
            optimizer.point_target = target;
            optimizer.x = new double[] { joint.UpperBevel, joint.LowerBevel, joint.Elbow, joint.twist };
            Joints optimized = optimizer.minimize_angle();
            needle_exit.kinematics.joint.UpperBevel = optimized.UpperBevel;
            needle_exit.kinematics.joint.LowerBevel = optimized.LowerBevel;
            needle_exit.kinematics.joint.Elbow = optimized.Elbow;
            needle_exit.kinematics.joint.twist = optimized.twist;
            needle_exit.update_needle();
        }
        public void set_entry_needle(Joints joint)
        {
            double central_angle = get_central_angle();
            Optimizer optimizer = new Optimizer();
            Point4D target = new Point4D(entry_point.X, entry_point.Y, entry_point.Z, central_angle);
            Console.Write("\noTarget entry: {0}, {1}, {2}, {3}", entry_point.X, entry_point.Y, entry_point.Z, central_angle);
            optimizer.point_target = target;
            optimizer.x = new double[] { joint.UpperBevel, joint.LowerBevel, joint.Elbow, joint.twist };
            Joints optimized = optimizer.minimize_angle();
            needle_entry.kinematics.joint.UpperBevel = optimized.UpperBevel;
            needle_entry.kinematics.joint.LowerBevel = optimized.LowerBevel;
            needle_entry.kinematics.joint.Elbow = optimized.Elbow;
            needle_entry.kinematics.joint.twist = optimized.twist;
            needle_entry.update_needle();
        }
        public Joints update_trajectory(Joints joint) // needle center for the needle frame and a point
        {
            double central_angle = get_central_angle();
            Optimizer optimizer = new Optimizer();
            Point4D target = new Point4D(entry_point.X, entry_point.Y, entry_point.Z, central_angle);
            optimizer.point_target = target;
            optimizer.x = new double[] { joint.UpperBevel, joint.LowerBevel, joint.Elbow, joint.twist };
            Joints optimized = optimizer.minimize_angle();
            return optimized;
            //Vector3D projected_exit_point = project_point(exit_point, needle_normal, needle_point);
            //Matrix3D head = calculate_needle_tip(entry_point, projected_exit_point, needle_normal);
        }
        public Matrix3D calculate_needle_tip(Vector3D entry_point, Vector3D exit_point, Vector3D normal) // check if entry_point and exit_point are on the same plane
        {
            Vector3D center=calculate_center(entry_point, exit_point);

            Vector3D ux, uy, uz;
            ux = center - entry_point;
            uz = normal;
            uy = Vector3D.CrossProduct(uz, ux);

            Matrix3D head = new Matrix3D(ux.X, uy.X, uz.X, entry_point.X,
                                         ux.Y, uy.Y, uz.Y, entry_point.Y,
                                         ux.Z, uy.Z, uz.Z, entry_point.Z,
                                         0, 0, 0, 1);
            return head;
        }
        private Vector3D calculate_center(Vector3D entry_point, Vector3D exit_point)
        {
            Coordinate local_coordinate;
            Coordinate twisted_local_coordinate;
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
            offset = Math.Sqrt(radius * radius - .25 * twisted_local_coordinate.e_x.Length * twisted_local_coordinate.e_x.Length) * twisted_local_coordinate.e_y / twisted_local_coordinate.e_y.Length;
            local_coordinate.origin = new Vector3D(0.5 * (entry_point.X + exit_point.X), 0.5 * (entry_point.Y + exit_point.Y), 0.5 * (entry_point.Z + exit_point.Z));
            local_coordinate.origin = local_coordinate.origin - offset;
            twisted_local_coordinate.origin = local_coordinate.origin;
            twisted_local_coordinate.e_z = local_coordinate.e_z;
            return local_coordinate.origin;
        }
        public static Vector3D project_point(Vector3D point, Vector3D normal, Vector3D plane_point)
        {
            double d = normal.X * (point.X - plane_point.X) + normal.Y * (point.Y - plane_point.Y) + normal.Z * (point.Z - plane_point.Z);
            return point - d * normal;
        }
        private double get_central_angle() // half of central angle
        {
            double chord = (exit_point - entry_point).Length;
            double angle = Math.Atan(chord / radius / 2);
            return angle * 180 / Math.PI;
        }
    }
}

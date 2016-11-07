using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace path_generation.OnePointSuturing
{
    public class Trajectory // Description: entry and exit needles are calculated, mid needels are interpolated 
    {
        // needle constants
        double radius = 14;
        int n = 20;

        // trajectory variables
        //Matrix3D center;
        public Matrix3D head; // not used
        public Vector3D entry_point;
        public Vector3D exit_point;
        Matrix3D[] points;

        // defines needle instants
        private Needle needle_entry, needle_exit, needle_mid;

        // define interpolation tool
        private Interpolation interpolation;

        // define the trajectory vaiable
        static double t;

        // define optimizer for center
        Optimizer optimizer;
        public Trajectory()
        {
            t = 0;
            head = new Matrix3D();
            needle_entry = new Needle();
            needle_exit = new Needle();
            needle_mid = new Needle();
            interpolation = new Interpolation(); // might be deleted
            optimizer = new Optimizer();

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

            // setup needle_mid
            needle_mid.kinematics.joint.UpperBevel = needle_entry.kinematics.joint.UpperBevel;
            needle_mid.kinematics.joint.LowerBevel = needle_entry.kinematics.joint.LowerBevel;
            needle_mid.kinematics.joint.Elbow = needle_entry.kinematics.joint.Elbow;
            needle_mid.kinematics.joint.twist = needle_entry.kinematics.joint.twist;
            needle_mid.update_needle();


        }
        public Needle update_trajectory()
        {
            /*
            //interpolation.M_initial = needle_entry.head;
            //interpolation.M_target = needle_exit.head;
            interpolation.initialize(needle_entry.head, needle_exit.head);// might not needed
            Matrix3D head = interpolation.update(needle_mid.moved_head); //might not needed// head of next needle (mid needle)
            needle_mid.update_needle(head);
            */

            Vector3D center_mid = interpolation.interpolate_center(needle_entry, needle_exit);
            
            Matrix3D M_target = needle_mid.moved_head;
            M_target.M14 = center_mid.X;
            M_target.M24 = center_mid.Y;
            M_target.M34 = center_mid.Z;
            optimizer.T_taget = M_target;
            optimizer.x = new double[4] { needle_mid.kinematics.joint.UpperBevel, needle_mid.kinematics.joint.LowerBevel, needle_mid.kinematics.joint.Elbow, needle_mid.kinematics.joint.twist };
            Joints optimized=optimizer.minimize_center();
            needle_mid.kinematics.joint = optimized;
            needle_mid.update_needle();
            //t = t + .1; // needed?


            return needle_mid;
        }
        public Joints update_trajectory(Joints joint) // needle center for the needle frame and a point
        {// not used
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
        {// not used
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
        private Vector3D calculate_center(Vector3D entry_point, Vector3D exit_point) // not used
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
        public static Vector3D project_point(Vector3D point, Vector3D normal, Vector3D plane_point) // not used
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

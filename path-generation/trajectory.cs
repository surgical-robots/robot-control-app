using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace path_generation
{
    public class trajectory
    {
        double xc; // center of the needle
        double yc;
        double zc;
        double r = 14; // radius of the needle
        double LengthUpperArm = 68.58;
        double LengthForearm = 96.393;
        double t_incr = Math.PI / 10;
        public trajectory()
        {
            xc = 0; // center of the needle
            yc = 0;
            zc = 130;

        }
        public trajectory(double x, double y, double z)
        {
            xc = x;
            yc = y;
            zc = z;
        }
        public Vector minimizer(Vector ebow, Vector tangential)
        {
            Vector needle = new Vector();
            return needle;
        }
        public point end_effector(double t) //trajectory() in MATLAB; calculation position of end effector and ideal orientation of the needle
        {
            point DOF;

            // calculating position
            DOF.pos.x = xc + r * Math.Sin(t);
            DOF.pos.y = yc + r * Math.Cos(t);
            DOF.pos.z = zc;

            //double[] vector1 = new double[3] { r * Math.Sin(t), r * Math.Cos(t), 0};
            //double[] vector2 = new double[3] { r * Math.Sin(t + t_incr), r * Math.Cos(t + t_incr), 0};
            //double[] normal = new double[3];

            // calculation orientation
            Vector centric1 = new Vector(r * Math.Sin(t), r * Math.Cos(t), 0);
            Vector centric2 = new Vector(r * Math.Sin(t + t_incr), r * Math.Cos(t + t_incr), 0);
            Vector normal = new Vector();
            Vector tangential = new Vector();
            normal = normal.cross(centric1, centric2);
            tangential = tangential.cross(centric1, normal);

            //Console.WriteLine("normal ? : {0}, {1}, {2}", normal.x, normal.y, normal.z);
            //Console.WriteLine("normal ? : {0}, {1}, {2}", normal.x / normal.norm(), normal.y / normal.norm(), normal.z / normal.norm());

            DOF.ori.x = tangential.x / tangential.norm();
            DOF.ori.y = tangential.y / tangential.norm();
            DOF.ori.z = tangential.z / tangential.norm();
            return DOF;
        }
    }
}

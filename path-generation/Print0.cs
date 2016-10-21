using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace path_generation
{
    public class Print0
    {
        public static void print_quaternion(Quaternion q)
        {
            Console.Write("\n [{0}, {1}, {2}]\t w: {3}\n", q.X, q.Y, q.Z, q.W);
        }
        public static void print_vector(Vector3D v)
        {
            Console.Write("\n [{0}, {1}, {2}]\n", v.X, v.Y, v.Z);
        }
        public static void print_double(double d)
        {
            Console.Write("\n {0}\n", d);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace path_generation
{
    public struct position
    {
        public double x;
        public double y;
        public double z;
    };
    public struct orientation
    {
        public double x;
        public double y;
        public double z;
    };
    public struct point
    {
        public position pos;
        public orientation ori;
    };
    public class Vector
    {
        public double x, y, z;
        //public double[] values = new double[3];
        public Vector()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }
        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        //public double[] cross(double[] a, double[] b)
        //{
        //    double[] product = new double[3] {a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[3] - a[1] * b[0]};
        //    return product;
        //}
        public Vector cross(Vector a, Vector b)
        {
            Vector product = new Vector(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
            return product;
        }
        public double norm()
        {
            return Math.Sqrt(x*x + y*y + z*z);
        }
    }


}

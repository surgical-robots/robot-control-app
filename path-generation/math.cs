using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
namespace path_generation
{

    public struct point
    {
        public Vector3D pos;
        public Vector3D ori;
    };

    public struct dof4
    {
        public Vector3D pos;
        public double twist;
    }

    public struct Coordinate
    {
        public Vector3D origin;
        public Vector3D e_x;
        public Vector3D e_y;
        public Vector3D e_z;
    }
    public class math
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
//using Microsoft.DirectX;

namespace path_generation.OnePointSuturing
{
    public class Needle
    {
        // needle constants
        public double radius = 14;
        const int n = 20;

        // needle variables
        public Matrix3D center;
        public Matrix3D head, head0;
        public Matrix3D tail, tail0;
        public Matrix3D moved_head, moved_head0;
        public Vector3D[] points;

        // needle kinematics
        public NeedleKinematics kinematics;

        public Needle()
        {
            kinematics = new NeedleKinematics();

            // initializing the center, head and tail
            center = new Matrix3D();
            center.SetIdentity();
            head0 = new Matrix3D();
            head0.SetIdentity();
            head0.M14 = -radius;
            tail0 = new Matrix3D();
            tail0.SetIdentity();
            tail0.M14 = radius;
            points = new Vector3D[n];

            // initializing points
            for (int i = 0; i < n; i++)
            {
                points[i].X = radius * Math.Cos((double)i / (n-1) * Math.PI);
                points[i].Y = -radius * Math.Sin((double)i / (n - 1) * Math.PI);
                points[i].Z = 0;
            }

            // initializing the moved head
            double angle = -Math.PI / (n - 1);
            Matrix3D rotZ = new Matrix3D(Math.Cos(angle), -Math.Sin(angle), 0, 0,
                                        Math.Sin(angle), Math.Cos(angle), 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
            moved_head0 = Matrix3D.Multiply(rotZ, head0);
        }
        public void update_needle()
        {
            // update the center, head, tail and moved head
            this.center = kinematics.transformation_matrix(5);
            this.head = Matrix3D.Multiply(center, this.head0);
            this.moved_head = Matrix3D.Multiply(center, this.moved_head0);
            this.tail = Matrix3D.Multiply(center, this.tail0);
            // update the points
        
        }
        public void update_points()
        {
            for (int i = 0; i < n; i++)
            {
                points[i] = NeedleKinematics.transform(center, points[i]);
            }
        }
    }
}

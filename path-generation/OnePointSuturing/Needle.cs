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
        public int n = 20;

        // needle variables
        public Matrix3D center;
        public Matrix3D head, head0;
        public Matrix3D tail, tail0;
        public Matrix3D moved_head, moved_head0;
        public Vector3D[] real_half; // points that composes the physical part of needle
        public Vector3D[] imag_half; // points that are not physically on the needle but are along the are of the needle

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
            real_half = new Vector3D[n];
            imag_half = new Vector3D[n];

            // initializing points
            for (int i = 0; i < n; i++)
            {
                real_half[i].X = radius * Math.Cos((double)i / (n - 1) * Math.PI);
                real_half[i].Y = -radius * Math.Sin((double)i / (n - 1) * Math.PI);
                real_half[i].Z = 0;

                imag_half[i].X = -radius * Math.Cos((double)i / (n - 1) * Math.PI);
                imag_half[i].Y = radius * Math.Sin((double)i / (n - 1) * Math.PI);
                imag_half[i].Z = 0;
            }

            // initializing the moved head
            double angle = -Math.PI / (n - 1);
            Matrix3D rotZ = new Matrix3D(Math.Cos(angle), -Math.Sin(angle), 0, 0,
                                        Math.Sin(angle), Math.Cos(angle), 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
            moved_head0 = Matrix3D.Multiply(rotZ, head0);
        }
        public void update_needle() // joints of kinematic must be upadated beforehand
        {
            // update the center, head, tail and moved head
            this.center = kinematics.transformation_matrix(5);
            this.head = Matrix3D.Multiply(center, this.head0);
            this.moved_head = Matrix3D.Multiply(center, this.moved_head0);
            this.tail = Matrix3D.Multiply(center, this.tail0);
            
            // update the points
            for (int i = 0; i < n; i++)
            {
                real_half[i] = NeedleKinematics.transform(center, this.real_half[i]);
                imag_half[i] = NeedleKinematics.transform(center, this.imag_half[i]);
            }
        }
        public void update_needle(Matrix3D desired) //based on head frame
        {
            Optimizer optimizer = new Optimizer();
            optimizer.T_taget = desired;
            optimizer.x = new double[4] { kinematics.joint.UpperBevel, kinematics.joint.LowerBevel, kinematics.joint.Elbow, kinematics.joint.twist };
            Joints optimized_joints = optimizer.minimize_error();
            kinematics.joint.UpperBevel = optimized_joints.UpperBevel;
            kinematics.joint.LowerBevel = optimized_joints.LowerBevel;
            kinematics.joint.Elbow = optimized_joints.Elbow;
            kinematics.joint.twist = optimized_joints.twist;
            update_needle();
        }
        public Vector3D get_normal()
        {
            Vector3D normal = new Vector3D(center.M13, center.M23, center.M33);
            return normal;
        }
    }
}

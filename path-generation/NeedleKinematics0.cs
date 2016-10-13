using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace path_generation
{
    public class NeedleKinematics0
    {
        public double leftUpperBevel;
        public double leftLowerBevel;
        public double leftElbow;
        public double twist;

        public NeedleKinematics0()
        {
        }
        /*
        public Matrix3D transformation_matrix(int a) //4 DOF Lue's calculation
        {
            double LengthUpperArm = 68.58;
            double LengthForearm = 96.393;
            double r = 14;
            // calculate forward kinematics and haptic forces, assuming kineAngle[0] is leftUpperBevel and kineAngle[1] is leftLowerBevel
            double theta1 = ((leftUpperBevel + leftLowerBevel) / 2) * Math.PI / 180; //theta1 = 0;
            double theta2 = ((leftUpperBevel - leftLowerBevel) / 2) * Math.PI / 180; //theta2 = 0;
            double theta3 = leftElbow * Math.PI / 180; //theta3 = 0;

            double c1 = Math.Cos(theta1);
            double s1 = Math.Sin(theta1);
            double c2 = Math.Cos(theta2);
            double s2 = Math.Sin(theta2);
            double c3 = Math.Cos(theta3);
            double s3 = Math.Sin(theta3); //twist = 90;
            double theta4 = twist * Math.PI / 180;
            double c4 = Math.Cos(theta4);
            double s4 = Math.Sin(theta4);
            Matrix3D T1 = new Matrix3D(c1, -s1, 0, 0,
                                       s1, c1, 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
            Matrix3D T2 = new Matrix3D(c2, -s2, 0, 0,
                                       0, 0, -1, 0,
                                       s2, c2, 0, 0,
                                        0, 0, 0, 1);
            Matrix3D T3 = new Matrix3D(c3, -s3, 0, LengthUpperArm,
                                       0, 0, 1, 0,
                                       -s3, -c3, 0, 0,
                                       0, 0, 0, 1);
            Matrix3D T4 = new Matrix3D(1, 0, 0, LengthForearm,
                                       0, c4, -s4, 0,
                                        0, s4, c4, 0,
                                        0, 0, 0, 1);
            Matrix3D T5 = new Matrix3D(1, 0, 0, 0,
                                       0, 1, 0, -2*r,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
            Matrix3D T = new Matrix3D();
            switch(a)
            {
                case 4:
                    T = Matrix3D.Multiply(T1, Matrix3D.Multiply(T2, Matrix3D.Multiply(T3, T4)));
                    break;
                case 5:
                    T = Matrix3D.Multiply(T1,Matrix3D.Multiply(T2, Matrix3D.Multiply(T3, Matrix3D.Multiply(T4, T5))));
                    break;
                case 45:
                    T = T5;
                    break;
                default:
                    break;

            }
            return T;
        }*/
        /*public Matrix3D transformation_matrix(int a) //5 DOF my calculation
        {
            double LengthUpperArm = 68.58;
            double LengthForearm = 96.393;
            double r = 14;
            // calculate forward kinematics and haptic forces, assuming kineAngle[0] is leftUpperBevel and kineAngle[1] is leftLowerBevel
            double theta1 = ((leftUpperBevel + leftLowerBevel) / 2) * Math.PI / 180;// theta1 = 0;
            double theta2 = ((leftUpperBevel - leftLowerBevel) / 2) * Math.PI / 180;// theta2 = 0;
            double theta3 = leftElbow * Math.PI / 180 + Math.PI / 2; //theta3 = 0 + Math.PI / 2;

            double c1 = Math.Cos(theta1);
            double s1 = Math.Sin(theta1);
            double c2 = Math.Cos(theta2);
            double s2 = Math.Sin(theta2);
            double c3 = Math.Cos(theta3);
            double s3 = Math.Sin(theta3); //twist = 0;
            double theta4 = twist * Math.PI / 180;
            double c4 = Math.Cos(theta4);
            double s4 = Math.Sin(theta4);
            Matrix3D T1 = new Matrix3D(c1, -s1, 0, 0,
                                       s1, c1, 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
            Matrix3D T2 = new Matrix3D(c2, -s2, 0, 0,
                                       0, 0, -1, 0,
                                       s2, c2, 0, 0,
                                        0, 0, 0, 1);
            Matrix3D T3 = new Matrix3D(c3, -s3, 0, LengthUpperArm,
                                       0, 0, 1, 0,
                                       -s3, -c3, 0, 0,
                                       0, 0, 0, 1);
            Matrix3D T4 = new Matrix3D(c4, -s4, 0, 0,
                                       0, 0, -1, -LengthForearm,
                                        s4, c4, 0, 0,
                                        0, 0, 0, 1);
            Matrix3D T5 = new Matrix3D(1, 0, 0, -2 * r,
                                       0, 1, 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
            Matrix3D T = new Matrix3D();
            switch (a)
            {
                case 4:
                    T = Matrix3D.Multiply(T1, Matrix3D.Multiply(T2, Matrix3D.Multiply(T3, T4)));
                    break;
                case 5:
                    T = Matrix3D.Multiply(T1, Matrix3D.Multiply(T2, Matrix3D.Multiply(T3, Matrix3D.Multiply(T4, T5))));
                    break;
                case 35:
                    T = Matrix3D.Multiply(T4, T5);
                    break;
                case 45:
                    T = T5;
                    break;
                default:
                    break;

            }
            return T;
        }*/
        public Matrix3D transformation_matrix(int a)
        {
            double LengthUpperArm = 68.58;
            double LengthForearm = 96.393;
            double r = 14;
            // calculate forward kinematics and haptic forces, assuming kineAngle[0] is leftUpperBevel and kineAngle[1] is leftLowerBevel
            double theta1 = ((leftUpperBevel + leftLowerBevel) / 2) * Math.PI / 180; //theta1 = 0;
            double theta2 = ((leftUpperBevel - leftLowerBevel) / 2) * Math.PI / 180; //theta2 = 0;
            double theta3 = leftElbow * Math.PI / 180 + Math.PI / 2; //theta3 = 0 + Math.PI / 2;

            double c1 = Math.Cos(theta1);
            double s1 = Math.Sin(theta1);
            double c2 = Math.Cos(theta2);
            double s2 = Math.Sin(theta2);
            double c3 = Math.Cos(theta3);
            double s3 = Math.Sin(theta3); //twist = 0;
            double theta4 = twist;
            double c5 = Math.Cos(theta4);
            double s5 = Math.Sin(theta4);
            Matrix3D T1 = new Matrix3D(c1, -s1, 0, 0,
                                       s1, c1, 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
            Matrix3D T2 = new Matrix3D(c2, -s2, 0, 0,
                                       0, 0, -1, 0,
                                       s2, c2, 0, 0,
                                        0, 0, 0, 1);
            Matrix3D T3 = new Matrix3D(c3, -s3, 0, LengthUpperArm,
                                       0, 0, 1, 0,
                                       -s3, -c3, 0, 0,
                                       0, 0, 0, 1);
            Matrix3D T4 = new Matrix3D(1, 0, 0, 0,
                                       0, 0, -1, -LengthForearm,
                                        0, 1, 0, 0,
                                        0, 0, 0, 1);
            Matrix3D T5 = new Matrix3D(c5, -s5, 0, 0,
                                       s5, c5, 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
            Matrix3D T6 = new Matrix3D(1, 0, 0, -2 * r,
                                       0, 1, 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);// r is negative to place the needle on the right. It's appear on the left since X is inverted
            Matrix3D T = new Matrix3D();
            switch (a)
            {
                case 4:
                    T = Matrix3D.Multiply(T1, Matrix3D.Multiply(T2, Matrix3D.Multiply(T3, T4)));
                    break;
                case 5:
                    T = Matrix3D.Multiply(T1, Matrix3D.Multiply(T2, Matrix3D.Multiply(T3, Matrix3D.Multiply(T4, T5))));
                    break;
                case 6:
                    T = Matrix3D.Multiply(T1, Matrix3D.Multiply(T2, Matrix3D.Multiply(T3, Matrix3D.Multiply(T4, Matrix3D.Multiply(T5, T6)))));
                    break;
                case 46:
                    T = Matrix3D.Multiply(T5, T6);
                    break;
                case 56:
                    T = T6;
                    break;
                default:
                    break;

            }
            return T;
        }
        public static Vector3D correction(Vector3D input)
        {
            Vector3D output = new Vector3D(input.Y, -input.Z, input.X);
            return output;
        }
        public static Vector3D correctionBack(Vector3D input)
        {
            Vector3D output = new Vector3D(input.Z, input.X, -input.Y);
            return output;
        }
        public static Vector3D transform(Matrix3D matrix, Vector3D vector)
        {
            Vector3D output = new Vector3D();
            output.X = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14;
            output.Y = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24;
            output.Z = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34;
            return output;
        }
    }
}

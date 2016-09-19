using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using minimizerNamespace;
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
        /*public Vector minimizer(Vector ebow, Vector tangential)
        {
            Vector needle = new Vector();
            return needle;
        }*/
        public point end_effector(Vector ori2, double t) //trajectory() in MATLAB; calculation position of end effector and ideal orientation of the needle
        {
            point DOF;

            // calculating position
            DOF.pos = new Vector();
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
            Vector needle = new Vector();
            
            //needle = minimizer(ori2, tangential);
            /*DOF.ori.x = tangential.x / tangential.norm();
            DOF.ori.y = tangential.y / tangential.norm();
            DOF.ori.z = tangential.z / tangential.norm();*/
            DOF.ori = new Vector();
            DOF.ori.x = 0;
            DOF.ori.y = 0;
            DOF.ori.z = 0;
            
            return DOF;
        }

        private Vector minimizer(Vector ori2, Vector tangential)
        {
            double ori2_x = ori2.x;
            double ori2_y = ori2.y;
            double ori2_z = ori2.z;
            double tangential_x = tangential.x;
            double tangential_y = tangential.y;
            double tangential_z = tangential.z;
            minimizerClass obj = new minimizerClass();
            MWArray[] output = new MWArray[4];
            output = obj.minimizerFunction(4,(MWArray)ori2_x, (MWArray)ori2_y, (MWArray)ori2_z, (MWArray)tangential_x, (MWArray)tangential_y, (MWArray)tangential_z);
            //Console.Write("The minimizer optimizes the path as below: {0}", output);
            //double[,] arr = (double[,])((MWArray)output).ToArray();
            //MWNumericArray x = (MWNumericArray)output[1];
            double[,] x = (double[,])((MWNumericArray)output[0]).ToArray(MWArrayComponent.Real);
            double[,] y = (double[,])((MWNumericArray)output[1]).ToArray(MWArrayComponent.Real);
            double[,] z = (double[,])((MWNumericArray)output[2]).ToArray(MWArrayComponent.Real);
            double[,] error = (double[,])((MWNumericArray)output[3]).ToArray(MWArrayComponent.Real);

            /*
            MWNumericArray output2 = new MWNumericArray();
            output2 = (MWNumericArray)obj.minimizerFunction((MWArray)ori2_x, (MWArray)ori2_y, (MWArray)ori2_z, (MWArray)tangential_x, (MWArray)tangential_y, (MWArray)tangential_z);
            double[,] nativeOutput = (double[,])((MWNumericArray)output2).ToArray();
             */
            Vector needle = new Vector();
            needle.x = x[0,0];
            needle.y = y[0, 0];
            needle.z = z[0, 0];
            return needle;
        }
    }
}

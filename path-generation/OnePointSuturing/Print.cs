using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.IO;

namespace path_generation.OnePointSuturing
{
    public class Print
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
        public static void PrintMatrixOnFile(Matrix3D T)
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            string OutputFileName = "dataM.txt";
            string fullPath = path + OutputFileName;

            if (!File.Exists(fullPath))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(fullPath))
                {
                    sw.Write(OutputFileName);
                    sw.Write("\t");
                    DateTime millisTime = DateTime.Now;
                    sw.WriteLine(millisTime);
                }
            }
            using (StreamWriter sw = File.AppendText(fullPath))
            {
                sw.Write("\t");
                sw.Write(T.M11.ToString());
                sw.Write("\t");
                sw.Write(T.M12.ToString());
                sw.Write("\t");
                sw.Write(T.M13.ToString());
                sw.Write("\t");
                sw.Write(T.M14.ToString());
                sw.Write("\t");
                sw.Write(T.M21.ToString());
                sw.Write("\t");
                sw.Write(T.M22.ToString());
                sw.Write("\t");
                sw.Write(T.M23.ToString());
                sw.Write("\t");
                sw.Write(T.M24.ToString());
                sw.Write("\t");
                sw.Write(T.M31.ToString());
                sw.Write("\t");
                sw.Write(T.M32.ToString());
                sw.Write("\t");
                sw.Write(T.M33.ToString());
                sw.Write("\t");
                sw.Write(T.M34.ToString());
                sw.Write("\t");
                sw.Write(T.OffsetX.ToString());
                sw.Write("\t");
                sw.Write(T.OffsetY.ToString());
                sw.Write("\t");
                sw.Write(T.OffsetZ.ToString());
                sw.Write("\t");
                sw.WriteLine(T.M44.ToString());
            }
        }
        public static void PrintJointOnFile(Joints J)
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            string OutputFileName = "dataJ.txt";
            string fullPath = path + OutputFileName;

            if (!File.Exists(fullPath))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(fullPath))
                {
                    sw.Write(OutputFileName);
                    sw.Write("\t");
                    DateTime millisTime = DateTime.Now;
                    sw.WriteLine(millisTime);
                }
            }
            using (StreamWriter sw = File.AppendText(fullPath))
            {
                sw.Write("\t");
                sw.Write(J.UpperBevel.ToString());
                sw.Write("\t");
                sw.Write(J.LowerBevel.ToString());
                sw.Write("\t");
                sw.Write(J.Elbow.ToString());
                sw.Write("\t");
                sw.WriteLine(J.twist.ToString());
            }
        }
    }
}

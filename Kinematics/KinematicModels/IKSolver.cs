using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Text;
using System.Threading.Tasks;

namespace Kinematics
{
    public class IKSolver : Kinematic
    {
        public double[] radAngle;
        public bool Initialized = false;

        const double IK_POS_THRESH = 1;
        const int IK_MAX_TRIES = 10000;

        /// <summary>
        /// index = 0        1          2
        /// alpha(i-1)     a(i-i)     d(i)
        /// </summary>
        public double[,] LinkTable { get; set; }

        /// <summary>
        /// This returns the number of links in the manipulator
        /// </summary>
        public int N { get; set; }

        /// <summary>
        /// This returns the minimum and maximum angles for each joint in degrees
        /// </summary>
        public Point[] MinMax { get; set; }

        /// <summary>
        /// This returns the joint coupling of the manipulator
        /// </summary>
        public CouplingType Coupling { get; set; }

        /// <summary>
        /// This returns whether or not to output workspace forces
        /// </summary>
        public bool OutputWorkspace { get; set; }

        /// <summary>
        /// This returns whether or not to invert workspace forces
        /// </summary>
        public bool[] InvertForces { get; set; }

        /// <summary>
        /// This returns the names of the angle outputs
        /// </summary>
        public string[] OutputStrings { get; set; }

        /// <summary>
        /// This returns an array of joint angles from an input XYZ position
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        protected override double[] getJointAngles(Point3D Position)
        {
            if(!Initialized)
            {
                radAngle = new double[N];
                radAngle.Initialize();
                Initialized = true;
            }
            double posError;
            Vector3D Ph;
            // create desired position vector
            Vector3D Pd = new Vector3D(Position.X, Position.Y, Position.Z);
            // declare 3D array for each joint frame axis (xi, yi, zi, Pi)
            Vector3D[,] frame = new Vector3D[N + 1, 4];
            frame.Initialize();
            // set base frame
            frame[0, 0].X = 1;
            frame[0, 1].Y = 1;
            frame[0, 2].Z = 1;

            int link = N - 1;
            int tries = 0;
            // begin Cyclic Coordinate Descent loop
            do
            {
                for (int i = 1; i < N+1; i++)
                {
                    // x(i)
                    frame[i, 0] = Vector3D.Add((Vector3D.Multiply(Math.Cos(radAngle[i - 1]), frame[(i - 1), 0])), (Vector3D.Multiply(Math.Sin(radAngle[i - 1]), frame[(i - 1), 1])));
                    // z(i)
                    frame[i, 2] = Vector3D.Add((Vector3D.Multiply(Math.Cos(LinkTable[i-1, 0]), frame[(i-1), 2])), Vector3D.Multiply(Math.Sin(LinkTable[(i-1),0]), Vector3D.CrossProduct(frame[i, 0], frame[(i-1), 2])));
                    // y(i)
                    frame[i, 1] = Vector3D.CrossProduct(frame[i, 2], frame[i, 0]);
                    //P(i)
                    frame[i, 3] = Vector3D.Add(Vector3D.Add(Vector3D.Multiply(LinkTable[i - 1, 2], frame[i - 1, 2]), Vector3D.Multiply(LinkTable[i - 1, 1], frame[i, 0])), frame[i-1, 3]);
                }
                // compute relative positions
                Vector3D[] Pih = new Vector3D[N+1];
                for(int i = N - 1; i >= 0; i--)
                {
                    Pih[i] = Vector3D.Subtract(frame[N, 3], frame[i, 3]);
                }
                // set position of end effector
                Ph = frame[N, 3];
                posError = Math.Sqrt(Math.Pow(Ph.X - Pd.X, 2) + Math.Pow(Ph.Y - Pd.Y, 2) + Math.Pow(Ph.Z - Pd.Z, 2));
                if (posError > IK_POS_THRESH)
                {
                    // create target effector position vector
                    Vector3D Target = Vector3D.Subtract(Pd, frame[link, 3]);
                    Target.Normalize();
                    Pih[link].Normalize();
                    // use the dot product to calculate the cos of the desired angle
                    double cosAngle = Vector3D.DotProduct(Target,Pih[link]);
                    // check if we need to rotate the link
                    if(cosAngle < 0.99999)
                    {
                        // use cross product to check rotation direction
                        Vector3D crossResult = Vector3D.CrossProduct(Pih[link], Target);
                        crossResult.Normalize();
                        double turnAngle = Vector3D.AngleBetween(Target, Pih[link]) * Math.PI / 180;
                        double sign = Vector3D.DotProduct(frame[link, 2], crossResult);
                        if (sign < 0)
                            turnAngle = -turnAngle;
                        radAngle[link] += turnAngle;
                        if (radAngle[link] < (MinMax[link - 1].X * Math.PI / 180))
                            radAngle[link] = MinMax[link - 1].X * Math.PI / 180;
                        else if (radAngle[link] > (MinMax[link - 1].Y * Math.PI / 180))
                            radAngle[link] = MinMax[link - 1].Y * Math.PI / 180;
                    }
                }
                if (link-- < 2) link = N - 1;
            }
            while (tries++ < IK_MAX_TRIES && posError > IK_POS_THRESH);
            double[] angles;
            // check if we are outputting workspace forces
            if(OutputWorkspace)
            {
                double forceGain = 0.5;
                angles = new double[N + 2];
                // calculate workspace forces if our position error is greater than the threshold
                if (posError > IK_POS_THRESH)
                {
                    Vector3D forces = Vector3D.Multiply(forceGain, Vector3D.Subtract(Pd, Ph));
                    // invert forces if desired
                    angles[N - 1] = InvertForces[0] ? -forces.X : forces.X;
                    angles[N] = InvertForces[1] ? -forces.Y : forces.Y;
                    angles[N + 1] = InvertForces[2] ? -forces.Z : forces.Z;
                }
                else
                {
                    // no workspace force if we can reach desired point
                    angles[N - 1] = 0;
                    angles[N] = 0;
                    angles[N + 1] = 0;
                }
            }
            else
                angles = new double[N - 1];
            // change output angles based on joint coupling
            switch(Coupling)
            {
                case CouplingType.None:
                    //convert angles to degrees
                    for (int i = 0; i < N - 1; i++)
                    {
                        angles[i] = radAngle[i + 1] * 180 / Math.PI;
                    }
                    break;
                case CouplingType.ShoulderTwoDOF:
                    angles[0] = (radAngle[1] + radAngle[2]) * 180 / Math.PI ;
                    angles[1] = (radAngle[1] - radAngle[2]) * 180 / Math.PI;
                    for (int i = 2; i < N - 1; i++)
                    {
                        angles[i] = radAngle[i + 1] * 180 / Math.PI;
                    }
                    break;
            }
            switch(N)
            {
                case 4:
                    Func<double[], double> f = (q) => 
                    Vector3D.DotProduct(Vector3D.Subtract(Pd, Ph), Vector3D.Subtract(Pd, Ph));
                    Func<double[], double[]> g = (q) =>  ;

                    break;
            }
            return angles;
        }

        public override string[] OutputNames
        {
            get
            {
                return OutputStrings;
            }
        }
    }
}

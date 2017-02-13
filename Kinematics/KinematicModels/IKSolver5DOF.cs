using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Kinematics
{
    public class IKSolver5DOF : Kinematic
    {
        private double[] radAngle;      // array of joint angles in radians
        private double[] minErrAngle;
        private double[] thetaOffset;   // array of theta offsets from DH parameters
        private Vector3D[,] frame;      // array of joint frame vectors
        private Vector3D Pd;            // desired position vector
        private Vector3D Ph;            // position of end effector
        private Vector3D[] Rd;          // desired orientation of end effector
        private Vector3D[] Rh;          // orientation of end effector
        private Vector3D Pih;           // array of relative position of end effector with respect to each frame
        Vector3D Pid;
        private double deltaP;          // position error
        private double deltaO;          // orientation error
        private double Ec;              // current error
        private double Ep = 10;         // previous error
        private double Epp = 10;        // interation before last error
        private double minError = 100;  // tracks min error
        private bool Initialized = false;
        private double maxForce = 4;

        const int IK_MAX_TRIES = 500;      // max number of CCD iterations
        private double wp = 1;              // position weight
        private double wo = 3;            // orientation weight
        private double hiErrThresh = 0.01;  // High error threshold, use factored rptCheck for errors beyond this magnitude
        private double hiLoFac = 10000;     // factor between high and low rptCheck
        private int rptCt = 0;              // counter for repeated error
        private double rptCheck;            // minimum change for repeat counter
        private int maxRpt = 5;             // maximum number of repeat errors
        private int nudgeCt = 0;            // counts nudges
        private double maxNudge = 5;        // maximum number of possible nudges


        /// <summary>
        /// index = 0        1          2          3
        /// alpha(i-1)     a(i-i)     d(i)      theta(i)
        /// </summary>
        public double[,] DHparameters { get; set; }

        /// <summary>
        /// This returns the number of links in the manipulator
        /// </summary>
        public int N { get; set; }

        /// <summary>
        /// This returns the weights (0 or 1) of each end effector orientations
        /// </summary>
        public bool[] Sigma { get; set; }

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
        /// Stop criteria for CCD
        /// </summary>
        public double eps { get; set; }

        /// <summary>
        /// Criteria to begin BGFS optimizer
        /// </summary>
        public double BETA { get; set; }

        protected override double[] getJointAngles(Vector3D Position, Vector3D Orientation, double[,] RotM)
        {
            if (!Initialized)
            {
                radAngle = new double[N + 1];
                minErrAngle = new double[N - 2];
                radAngle.Initialize();

                thetaOffset = new double[N + 1];
                thetaOffset[0] = 0;
                for (int i = 1; i <= N; i++)
                {
                    thetaOffset[i] = DHparameters[i - 1, 3] * Math.PI / 180;
                }

                Initialized = true;
            }
            hiLoFac = eps / 10;

            // create desired position vector
            Pd = new Vector3D(Position.X, Position.Y, Position.Z);

            double LengthUpperArm = 50;
            double LengthForearm = 83.3;
            double pad = 1.5;
            double Lmax = LengthUpperArm + LengthForearm - pad;
            double Lmin = Math.Sqrt(Math.Pow(LengthUpperArm, 2) + Math.Pow(LengthForearm, 2) - 2 * LengthUpperArm * LengthForearm * Math.Cos(Math.PI - MinMax[3].Y / 180 * Math.PI)) + pad;
            double L12 = Math.Sqrt(Math.Pow(Position.X, 2) + Math.Pow(Position.Y, 2) + Math.Pow(Position.Z, 2));
            double Lratio = Lmax / L12;

            // bound desired position by min and max workspace spheres
            if (L12 > Lmax)
            {
                Pd.X = Position.X * Lratio;
                Pd.Y = Position.Y * Lratio;
                Pd.Z = Position.Z * Lratio;
            }
            else if (L12 < Lmin)
            {
                Lratio = Lmin / L12;
                Pd.X = Position.X * Lratio;
                Pd.Y = Position.Y * Lratio;
                Pd.Z = Position.Z * Lratio;
            }

            // create desired orientation vector from roll, pitch, yaw
            Rd = new Vector3D[3];

            //Rd[0].X = Math.Cos(Orientation.Y) * Math.Cos(Orientation.Z);
            //Rd[0].Y = Math.Sin(Orientation.Z) * Math.Cos(Orientation.Y);
            //Rd[0].Z = -Math.Sin(Orientation.Y);
            //Rd[1].X = Math.Cos(Orientation.Z) * Math.Sin(Orientation.Y) * Math.Sin(Orientation.X) - Math.Sin(Orientation.Z) * Math.Cos(Orientation.X);
            //Rd[1].Y = Math.Sin(Orientation.X) * Math.Sin(Orientation.Y) * Math.Sin(Orientation.Z) + Math.Cos(Orientation.X) * Math.Cos(Orientation.Z);
            //Rd[1].Z = Math.Cos(Orientation.Y) * Math.Sin(Orientation.X);
            //Rd[2].X = Math.Cos(Orientation.X) * Math.Sin(Orientation.Y) * Math.Cos(Orientation.Z) + Math.Sin(Orientation.X) * Math.Sin(Orientation.Z);
            //Rd[2].Y = Math.Sin(Orientation.Z) * Math.Sin(Orientation.Y) * Math.Cos(Orientation.X) - Math.Cos(Orientation.Z) * Math.Sin(Orientation.X);
            //Rd[2].Z = Math.Cos(Orientation.Y) * Math.Cos(Orientation.X);

            Rd[0].X = RotM[0, 0];
            Rd[0].Y = RotM[0, 1];
            Rd[0].Z = RotM[0, 2];
            Rd[1].X = RotM[1, 0];
            Rd[1].Y = RotM[1, 1];
            Rd[1].Z = RotM[1, 2];
            Rd[2].X = RotM[2, 0];
            Rd[2].Y = RotM[2, 1];
            Rd[2].Z = RotM[2, 2];

            Rh = new Vector3D[3];
            // declare 3D array for each joint frame axis (xi, yi, zi, Pi)
            frame = new Vector3D[N + 1, 4];
            frame.Initialize();
            // set base frame
            frame[0, 0].X = 1;
            frame[0, 1].Y = 1;
            frame[0, 2].Z = 1;

            int tries = 0;
            minError = 10;

            // begin Cyclic Coordinate Descent loop
            do
            {
                ForwardKine();
                // calculate position error
                deltaP = Vector3D.DotProduct(Vector3D.Subtract(Pd, Ph), Vector3D.Subtract(Pd, Ph));
                // calculate orientation error
                deltaO = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (Sigma[i])
                    {
                        deltaO += Math.Pow((Vector3D.DotProduct(Rd[i], Rh[i]) - 1), 2);
                    }
                }
                // calculate total error
                Ec = wo * deltaO + wp * deltaP;

                if(Ec < minError)
                {
                    minError = Ec;
                    for(int i = 0; i < N - 2; i++)
                    {
                        minErrAngle[i] = radAngle[i + 1];
                    }
                }

                // repeat check 1 - normal repeating, high error
                if ((Math.Abs(Ec - Ep) < rptCheck * hiLoFac) && (Ec > hiErrThresh))
                {
                    wo = wo * wo;
                    rptCt++;
                }
                // repeat check 2 - normal repaeting, low error
                else if (Math.Abs(Ec - Ep) < rptCheck)
                {
                    wo = wo * wo;
                    rptCt++;
                }
                // repeat check 3 - oscillating repeating
                else if ((Math.Abs(Ec - Epp) < rptCheck * hiLoFac) && !(Math.Abs(Ec - Ep) < rptCheck * hiLoFac))
                {
                    rptCt++;
                }
                else
                    rptCt = 0;

                if(Ec < minError)
                {
                    minError = Ec;
                    for(int i = 0; i < 4; i++)
                    {
                        minErrAngle[i] = radAngle[i + 1];
                    }
                }

                if (Ec > eps) // begin Cyclic Coordinate Descent loop
                {
                    Epp = Ep;
                    Ep = Ec;

                    // backward recurssion through joints for CCD
                    for (int link = N - 2; link > 0; link-- )
                    {
                        ForwardKine();
                        // create target effector position vector
                        Pih = Vector3D.Subtract(Ph, frame[link, 3]);
                        Pid = Vector3D.Subtract(Pd, frame[link, 3]);

                        // calculate values for adjustment angle
                        double k1 = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (Sigma[i])
                                k1 += wo * Vector3D.DotProduct(Rd[i], frame[link, 2]) * Vector3D.DotProduct(Rh[i], frame[link, 2]);
                        }
                        k1 += wp * Vector3D.DotProduct(Pid, frame[link, 2]) * Vector3D.DotProduct(Pih, frame[link, 2]);

                        double k2 = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (Sigma[i])
                                k2 += wo * Vector3D.DotProduct(Rd[i], Rh[i]);
                        }
                        k2 += wp * Vector3D.DotProduct(Pid, Pih);

                        double k3 = 0;
                        Vector3D ko3 = new Vector3D();
                        for (int i = 0; i < 3; i++)
                        {
                            if (Sigma[i])
                                ko3 = Vector3D.Add(ko3, wo * Vector3D.CrossProduct(Rh[i], Rd[i]));
                                //ko3 = Vector3D.Add(ko3, wo * Vector3D.CrossProduct(Rd[i], Rh[i]));
                        }
                        k3 = Vector3D.DotProduct(frame[link, 2], Vector3D.Add(wp * Vector3D.CrossProduct(Pih, Pid), ko3));
                        //k3 = Vector3D.DotProduct(frame[link, 2], Vector3D.Add(wp * Vector3D.CrossProduct(Pid, Pih), ko3));
                        double phi;
                        // minimize position and orientation error
                        if ((k1 - k2) != 0)
                            phi = Math.Atan(-k3 / (k1 - k2));
                        else
                            phi = 0;

                        double d2g = (k1 - k2) * Math.Cos(phi) - k3 * Math.Sin(phi);
                        if (d2g > 0)
                        {
                            //shift phi by + or - pi
                            if (phi < 0)
                                phi = phi + Math.PI;
                            else
                                phi = phi - Math.PI;
                        }
                        
                        double phiMax = (MinMax[link - 1].Y * Math.PI / 180) - radAngle[link];
                        double phiMin = (MinMax[link - 1].X * Math.PI /180) - radAngle[link];
                        bool useBounds = false;

                        if (phi > phiMax)
                        {
                            phi -= 2 * Math.PI;
                            if (phi > phiMax || phi < phiMin)
                                useBounds = true;
                        }
                        else if (phi < phiMin)
                        {
                            phi += 2 * Math.PI;
                            if (phi > phiMax || phi < phiMin)
                                useBounds = true;
                        }

                        if (useBounds)
                        {
                            double gPhiMax = k1 * (1 - Math.Cos(phiMax)) + k2 * Math.Cos(phiMax) + k3 * Math.Sin(phiMax);
                            double gPhiMin = k1 * (1 - Math.Cos(phiMin)) + k2 * Math.Cos(phiMin) + k3 * Math.Sin(phiMin);

                            if (gPhiMax > gPhiMin)
                                phi = phiMax;
                            else
                                phi = phiMin;
                        }

                        radAngle[link] += phi;
                        //// adjust angle based on joint limits
                        //if (radAngle[link] < (MinMax[link - 1].X * Math.PI / 180))
                        //    radAngle[link] = MinMax[link - 1].X * Math.PI / 180;
                        //else if (radAngle[link] > (MinMax[link - 1].Y * Math.PI / 180))
                        //    radAngle[link] = MinMax[link - 1].Y * Math.PI / 180;
                        //if (double.IsNaN(radAngle[link]))
                        //    radAngle[link] = 0;
                    }
                }
                //else
                //{
                //    Ec = minError;
                //    for(int i = 0; i< N-2; i++)
                //    {
                //        radAngle[i + 1] = minErrAngle[i];
                //    }
                //}
                // set previous error value for next loop
            }
            while (tries++ < IK_MAX_TRIES && Ec > eps);

            if(Ec > minError)
            {
                for(int i = 0; i < 4; i++)
                {
                    radAngle[i + 1] = minErrAngle[i];
                }
            }

            int outputNum = OutputStrings.Length;
            double[] angles = new double[outputNum];

            // change output angles based on joint coupling
            switch (Coupling)
            {
                case CouplingType.None:
                    //convert angles to degrees
                    for (int i = 0; i < (N - 2); i++)
                    {
                        angles[i] = radAngle[i + 1] * 180 / Math.PI;
                    }
                    break;
                case CouplingType.ShoulderTwoDOF:
                    angles[0] = (radAngle[1] + radAngle[2]) * 180 / Math.PI;
                    angles[1] = (radAngle[1] - radAngle[2]) * 180 / Math.PI;
                    for (int i = 2; i < N - 1; i++)
                    {
                        angles[i] = radAngle[i + 1] * 180 / Math.PI;
                    }
                    break;
                case CouplingType.FrankenBot:
                    for (int i = 0; i < N; i++)
                    {
                        angles[i] = radAngle[i + 1] * 180 / Math.PI;
                    }
                    angles[1] = angles[1] + (angles[0] / 2.868);
                    break;
            }

            // check if we are outputting workspace forces
            if (OutputWorkspace)
            {
                double forceGain = 0.5;
                // calculate workspace forces if our position error is greater than the threshold
                Vector3D forces = Vector3D.Multiply(forceGain, Vector3D.Subtract(Pd, Position));
                // invert forces if desired
                angles[outputNum - 3] = InvertForces[0] ? -forces.X : forces.X;
                angles[outputNum - 2] = InvertForces[1] ? -forces.Y : forces.Y;
                angles[outputNum - 1] = InvertForces[2] ? -forces.Z : forces.Z;
                for (int i = (outputNum - 3); i < outputNum; i++)
                {
                    if (angles[i] > maxForce) angles[i] = maxForce;
                    else if (angles[i] < -maxForce) angles[i] = -maxForce;
                }
            }

            return angles;
        }

        // uses forward recurrsion formulas and DH paramenters to calculate positions and orientations for each kinematic frame
        void ForwardKine()
        {
            Vector3D[] Pstar = new Vector3D[N];
            Pstar.Initialize();

            // initialize frame positions
            for (int i = 0; i < N + 1; i++)
            {
                frame[i, 3].X = 0;
                frame[i, 3].Y = 0;
                frame[i, 3].Z = 0;
            }
            // forward recurrsion formulas for frame position and orientation
            for (int i = 1; i <= N; i++)
            {
                // x(i) orientation vector
                frame[i, 0] = Vector3D.Add((Vector3D.Multiply((Math.Cos(radAngle[i - 1] + thetaOffset[i])), frame[(i - 1), 0])), (Vector3D.Multiply((Math.Sin(radAngle[i - 1] + thetaOffset[i])), frame[(i - 1), 1])));
                // z(i) orientation vector
                frame[i, 2] = Vector3D.Add((Vector3D.Multiply(Math.Cos(DHparameters[i - 1, 0] * Math.PI / 180), frame[(i - 1), 2])), Vector3D.Multiply(Math.Sin(DHparameters[(i - 1), 0] * Math.PI / 180), Vector3D.CrossProduct(frame[i, 0], frame[(i - 1), 2])));
                // y(i) orientation vector
                frame[i, 1] = Vector3D.CrossProduct(frame[i, 2], frame[i, 0]);
            }
            // P* --> relative positions of next frame wrt present frame
            for (int i = 0; i < N; i++)
            {
                Pstar[i] = Vector3D.Add(Vector3D.Multiply(DHparameters[i, 2], frame[i, 2]), Vector3D.Multiply(DHparameters[i, 1], frame[i + 1, 0]));
            }
            //P(i) --> frame positions wrt base frame
            for (int i = 1; i < N + 1; i++)
            {
                frame[i, 3] = frame[i - 1, 3] + Pstar[i - 1];
            }
            // set position of end effector
            Ph = frame[N, 3];

            // set end effector orientation
            Rh[0] = frame[N, 0];
            Rh[1] = frame[N, 1];
            Rh[2] = frame[N, 2];
        }

        double DegToRad(double deg)
        {
            return deg / 180 * Math.PI;
        }
        
        double RadToDeg(double rad)
        {
            return rad / Math.PI * 180;
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

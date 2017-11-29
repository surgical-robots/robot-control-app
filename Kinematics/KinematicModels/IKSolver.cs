using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Kinematics
{
    public class IKSolver : Kinematic
    {
        private double[] radAngle;          // array of joint angles in radians
        private double[] offset;            // array of joint offsets in whatever length units used in DH parameters, for translational joints
        private double[] minErrAngle;
        private double[] thetaOffset;       // array of theta offsets from DH parameters
        private Vector3D[,] frame;          // array of joint frame vectors
        private Vector3D Pd;                // desired position vector
        private Vector3D Ph;                // position of end effector
        private Vector3D[] Rd;              // desired orientation of end effector
        private Vector3D[] Rh;              // orientation of end effector
        private Vector3D Pih;               // array of relative position of end effector with respect to each frame
        Vector3D Pid;
        private double deltaP;              // position error
        private double deltaO;              // orientation error
        private double Ec;                  // current error
        private double Ep = 10;             // previous error
        private double Epp = 10;            // interation before last error
        private double minError = 100;      // tracks min error
        private bool Initialized = false;
        private double maxForce = 4;

        const int IK_MAX_TRIES = 500;       // max number of CCD iterations
        private double wp = 1;              // position weight
        private double wo = 3;              // orientation weight
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
        /// This returns the frame index of the end effector
        /// </summary>
        public int EndEffector { get; set; }

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

        /// <summary>
        /// Maximum reach of robotic arm
        /// </summary>
        public double Lmax { get; set; }

        /// <summary>
        /// Minimum reach of robotic arm
        /// </summary>
        public double Lmin { get; set; }

        protected override double[] getJointAngles(Vector3D Position, Vector3D Orientation, double[,] RotM)
        {
            if (!Initialized)
            {
                radAngle = new double[EndEffector + 1];
                offset = new double[EndEffector + 1];
                minErrAngle = new double[N];
                radAngle.Initialize();
                offset.Initialize();

                thetaOffset = new double[EndEffector + 1];
                thetaOffset[0] = 0;
                for (int i = 1; i <= EndEffector; i++)
                {
                    thetaOffset[i] = DHparameters[i - 1, 3] * Math.PI / 180;
                }

                Initialized = true;
            }
            hiLoFac = eps / 10;

            // create desired position vector
            Pd = new Vector3D(Position.X, Position.Y, Position.Z);

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
            frame = new Vector3D[EndEffector + 1, 4];
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

                if (Ec < minError)
                {
                    minError = Ec;
                    for (int i = 0; i < N; i++)
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
                // repeat check 2 - normal repeating, low error
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

                if (Ec < minError)
                {
                    minError = Ec;
                    for (int i = 0; i < 4; i++)
                    {
                        minErrAngle[i] = radAngle[i + 1];
                    }
                }

                if (Ec > eps) // begin Cyclic Coordinate Descent loop
                {
                    Epp = Ep;
                    Ep = Ec;

                    // backward recurssion through joints for CCD
                    for (int link = N; link > 0; link--)
                    {
                        ForwardKine();
                        // create target effector position vector
                        Pih = Vector3D.Subtract(Ph, frame[link, 3]);
                        Pid = Vector3D.Subtract(Pd, frame[link, 3]);

                        switch ((JointType)DHparameters[link - 1, 4])
                        {
                            case JointType.Rotation:
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
                                }
                                k3 = Vector3D.DotProduct(frame[link, 2], Vector3D.Add(wp * Vector3D.CrossProduct(Pih, Pid), ko3));
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
                                double phiMin = (MinMax[link - 1].X * Math.PI / 180) - radAngle[link];
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
                                break;
                            case JointType.Translation:
                                double lambda = Vector3D.DotProduct(Pid - Pih, frame[link, 2]);
                                offset[link] += lambda;
                                // appply joint limits
                                if (offset[link] > MinMax[link - 1].Y)
                                    offset[link] = MinMax[link - 1].Y;
                                else if (offset[link] < MinMax[link - 1].X)
                                    offset[link] = MinMax[link - 1].X;
                                break;
                        }
                    }
                }
                // set previous error value for next loop
            }
            while (tries++ < IK_MAX_TRIES && Ec > eps);

            if (Ec > minError)
            {
                for (int i = 0; i < N; i++)
                {
                    radAngle[i + 1] = minErrAngle[i];
                }
            }

            int outputNum = OutputStrings.Length;
            double[] IKOutput = new double[outputNum];
            double[] degAngle = new double[radAngle.Length];

            // convert angles to degrees
            for (int i = 0; i < (N); i++)
            {
                degAngle[i + 1] = RadToDeg(radAngle[i + 1]);
            }
            // set output based on joint type
            for (int i = 0; i < (N); i++)
            {
                if ((JointType)DHparameters[i, 4] == JointType.Rotation)
                    IKOutput[i] = degAngle[i + 1];
                else if ((JointType)DHparameters[i, 4] == JointType.Translation)
                    IKOutput[i] = offset[i + 1];
            }

            // change output angles based on joint coupling
            switch (Coupling)
            {
                case CouplingType.None:
                    //convert angles to degrees
                    for (int i = 0; i < (N); i++)
                    {
                        IKOutput[i] = degAngle[i + 1];
                    }
                    break;
                case CouplingType.ShoulderTwoDOF:
                    IKOutput[0] = degAngle[1] + degAngle[2];
                    IKOutput[1] = degAngle[1] - degAngle[2];
                    break;
                case CouplingType.FrankenBot:
                    IKOutput[1] = degAngle[2] + (degAngle[1] / 2.868);
                    break;
                case CouplingType.LouShoulder:
                    IKOutput[0] = degAngle[1] - degAngle[2];
                    IKOutput[1] = degAngle[1] + degAngle[2];
                    IKOutput[2] = degAngle[3] - degAngle[1] + degAngle[2];
                    break;
            }

            // check if we are outputting workspace forces
            if (OutputWorkspace)
            {
                double forceGain = 0.5;
                // calculate workspace forces if our position error is greater than the threshold
                Vector3D forces = Vector3D.Multiply(forceGain, Vector3D.Subtract(Ph, Position));
                // invert forces if desired
                IKOutput[outputNum - 3] = InvertForces[0] ? -forces.X : forces.X;
                IKOutput[outputNum - 2] = InvertForces[1] ? -forces.Y : forces.Y;
                IKOutput[outputNum - 1] = InvertForces[2] ? -forces.Z : forces.Z;
                for (int i = (outputNum - 3); i < outputNum; i++)
                {
                    if (IKOutput[i] > maxForce) IKOutput[i] = maxForce;
                    else if (IKOutput[i] < -maxForce) IKOutput[i] = -maxForce;
                }
            }

            return IKOutput;
        }

        // uses forward recurrsion formulas and DH paramenters to calculate positions and orientations for each kinematic frame
        void ForwardKine()
        {
            Vector3D[] Pstar = new Vector3D[EndEffector];
            Pstar.Initialize();

            // initialize frame positions
            for (int i = 0; i < EndEffector + 1; i++)
            {
                frame[i, 3].X = 0;
                frame[i, 3].Y = 0;
                frame[i, 3].Z = 0;
            }
            // forward recurrsion formulas for frame position and orientation
            for (int i = 1; i <= EndEffector; i++)
            {
                // x(i) orientation vector
                frame[i, 0] = Vector3D.Add((Vector3D.Multiply((Math.Cos(radAngle[i - 1] + thetaOffset[i])), frame[(i - 1), 0])), (Vector3D.Multiply((Math.Sin(radAngle[i - 1] + thetaOffset[i])), frame[(i - 1), 1])));
                // z(i) orientation vector
                frame[i, 2] = Vector3D.Add((Vector3D.Multiply(Math.Cos(DHparameters[i - 1, 0] * Math.PI / 180), frame[(i - 1), 2])), Vector3D.Multiply(Math.Sin(DHparameters[(i - 1), 0] * Math.PI / 180), Vector3D.CrossProduct(frame[i, 0], frame[(i - 1), 2])));
                // y(i) orientation vector
                frame[i, 1] = Vector3D.CrossProduct(frame[i, 2], frame[i, 0]);
            }
            // P* --> relative positions of next frame wrt present frame
            for (int i = 0; i < EndEffector; i++)
            {
                Pstar[i] = Vector3D.Add(Vector3D.Multiply(DHparameters[i, 2] + offset[i + 1], frame[i, 2]), Vector3D.Multiply(DHparameters[i, 1], frame[i + 1, 0]));
            }
            // P(i) --> frame positions wrt base frame
            for (int i = 1; i < EndEffector + 1; i++)
            {
                frame[i, 3] = frame[i - 1, 3] + Pstar[i - 1];
            }
            // set position of end effector
            Ph = frame[EndEffector, 3];

            // set end effector orientation
            Rh[0] = frame[EndEffector, 0];
            Rh[1] = frame[EndEffector, 1];
            Rh[2] = frame[EndEffector, 2];
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

        public override double[,] JointParms
        {
            get
            {
                return DHparameters;
            }
        }
    }
}

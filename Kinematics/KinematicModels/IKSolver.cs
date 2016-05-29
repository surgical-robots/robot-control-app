using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Kinematics
{
    public class IKSolver : Kinematic
    {
        private double[] radAngle;  // array of joint angles in radians
        private Vector3D[,] frame;  // array of joint frame vectors
        private Vector3D Pd;        // desired position vector
        private Vector3D Ph;        // position of end effector
        private Vector3D[] Rd;      // desired orientation of end effector
        private Vector3D[] Rh;      // orientation of end effector
        private Vector3D[] Pih;     // array of relative position of end effector with respect to each frame
        private double Eo;          // orientation error
        private double Ec;          // current error
        private double Ep;          // previous error
        private bool Initialized = false;
        private double maxForce = 4;

        const int IK_MAX_TRIES = 3000;

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
        public double IK_POS_THRESH { get; set; }

        /// <summary>
        /// Criteria to begin BGFS optimizer
        /// </summary>
        public double BETA { get; set; }

        /// <summary>
        /// This returns an array of joint angles from an input XYZ position
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        protected override double[] getJointAngles(Point3D Position)
        {
            if(!Initialized)
            {
                radAngle = new double[N + 1];
                radAngle.Initialize();
                Initialized = true;
            }
            // create desired position vector
            Pd = new Vector3D(Position.X, Position.Y, Position.Z);
            // create desired orientation vector
            Rd = new Vector3D[3];
            Rh = new Vector3D[3];
            // declare 3D array for each joint frame axis (xi, yi, zi, Pi)
            frame = new Vector3D[N + 1, 4];
            frame.Initialize();
            // set base frame
            frame[0, 0].X = 1;
            frame[0, 1].Y = 1;
            frame[0, 2].Z = 1;

            int link = N;
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
                for (int i = 0; i < N; i++)
                {
                    frame[i, 0] = frame[i + 1, 0];
                    frame[i, 1] = frame[i + 1, 1];
                    frame[i, 2] = frame[i + 1, 2];
                }
                // x(i)
                frame[N, 0] = Vector3D.Add((Vector3D.Multiply(Math.Cos(radAngle[N - 1]), frame[(N - 1), 0])), (Vector3D.Multiply(Math.Sin(radAngle[N - 1]), frame[(N - 1), 1])));
                // z(i)
                frame[N, 2] = Vector3D.Add((Vector3D.Multiply(Math.Cos(LinkTable[N - 1, 0]), frame[(N - 1), 2])), Vector3D.Multiply(Math.Sin(LinkTable[(N - 1), 0]), Vector3D.CrossProduct(frame[N, 0], frame[(N - 1), 2])));
                // y(i)
                frame[N, 1] = Vector3D.CrossProduct(frame[N, 2], frame[N, 0]);

                // compute relative positions
                Pih = new Vector3D[N + 1];
                for(int i = N - 1; i >= 0; i--)
                {
                    Pih[i] = Vector3D.Subtract(frame[N, 3], frame[i, 3]);
                }
                // set position of end effector
                Ph = frame[N, 3];
                // set end effector orientation
                Rh[0] = frame[N, 0];
                Rh[1] = frame[N, 1];
                Rh[2] = frame[N, 2];
                // calculate orientation error
                Eo = 0;
                for (int i = 0; i < 3; i++)
                {
                    if(Sigma[i])
                    {
                        Eo += Math.Pow((Vector3D.DotProduct(Rd[i], Rh[i]) - 1), 2);
                    }
                }
                // calculate current position error
                Ec = Eo + Math.Pow(Vector3D.DotProduct(Vector3D.Subtract(Pd, Ph), Vector3D.Subtract(Pd, Ph)), 2);

                if ((Ec > IK_POS_THRESH) && (Ec < BETA) && (Ec > Math.Pow(Ep, 2))) // begin BFGS optimization
                {
                    double epsg = 0.0000000001;
                    double epsf = 0;
                    double epsx = 0;
                    int maxits = 0;             // maximum number of iterations, for unlimited = 0
                    double[] optiAngle = new double[N];
                    for(int i = 0; i < N; i++)
                    {
                        optiAngle[i] = radAngle[i + 1];
                    }
                    alglib.minlbfgsstate state;
                    alglib.minlbfgsreport rep;

                    alglib.minlbfgscreate(N, 4, optiAngle, out state);         // create optimizer with current joint angles for initial values
                    alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);        // set optimizer options
                    alglib.minlbfgsoptimize(state, function1_grad, null, null);     // optimize
                    alglib.minlbfgsresults(state, out optiAngle, out rep);           // get results
                    for(int i = 0; i < N; i++)
                    {
                        radAngle[i + 1] = optiAngle[i];
                    }
                }
                else if (Ec > IK_POS_THRESH) // begin Cyclic Coordinate Descent loop
                {
                    
                    // create target effector position vector
                    Vector3D Pid = Vector3D.Subtract(Pd, frame[link, 3]);
                    //Pid.Normalize();
                    //Pih[link].Normalize();
                    double wp = 1;      // position weight
                    double wo = 1;      // orientation weight

                    double k1 = 0;
                    for(int i = 0; i < 3; i++)
                    {
                        if(Sigma[i])
                            k1 += wo * Vector3D.DotProduct(Rd[i], frame[link, 2]) * Vector3D.DotProduct(Rh[i], frame[link, 2]);
                    }
                    k1 += wp * Vector3D.DotProduct(Pid, frame[link-1, 2]) * Vector3D.DotProduct(Pih[link], frame[link-1, 2]);

                    double k2 = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if(Sigma[i])
                            k2 += wo * Vector3D.DotProduct(Rd[i], Rh[i]);
                    }
                    k2 += wp * Vector3D.DotProduct(Pid, Pih[link]);

                    double k3 = 0;
                    Vector3D ko3 = new Vector3D();
                    for (int i = 0; i < 3; i++)
                    {
                        if (Sigma[i])
                            ko3 = Vector3D.Add(ko3, wo * Vector3D.CrossProduct(Rh[i], Rd[i]));
                    }
                    k3 = Vector3D.DotProduct(frame[link-1, 2], Vector3D.Add(wp * Vector3D.CrossProduct(Pih[link], Pid), ko3));
                    // minimize position and orientation error
                    double turnAngle = Math.Atan(-k3 / (k1 - k2));
                    radAngle[link] += turnAngle;
                    /*
                    // use the dot product to calculate the cos of the desired angle
                    double cosAngle = Vector3D.DotProduct(Pid,Pih[link]);
                    // check if we need to rotate the link
                    if(cosAngle < 0.99999)
                    {
                        // use cross product to check rotation direction
                        Vector3D crossResult = Vector3D.CrossProduct(Pih[link], Pid);
                        crossResult.Normalize();
                        double turnAngle = Vector3D.AngleBetween(Pid, Pih[link]) * Math.PI / 180;
                        double sign = Vector3D.DotProduct(frame[link, 2], crossResult);
                        if (sign < 0)
                            turnAngle = -turnAngle;
                        radAngle[link] += turnAngle;
                     */
                    // adjust angle based on joint limits
                    if (radAngle[link] < (MinMax[link - 1].X * Math.PI / 180))
                        radAngle[link] = MinMax[link - 1].X * Math.PI / 180;
                    else if (radAngle[link] > (MinMax[link - 1].Y * Math.PI / 180))
                        radAngle[link] = MinMax[link - 1].Y * Math.PI / 180;
                    if (double.IsNaN(radAngle[link]))
                        radAngle[link] = 0;
                    //}
                        
                    // backward recurssion through joints for CCD
                    if (link-- < 2) link = N;
                }
                // set previous error value for next loop
                Ep = Ec;
            }
            while (tries++ < IK_MAX_TRIES && Ec > IK_POS_THRESH);

            double[] angles;
            // check if we are outputting workspace forces
            if(OutputWorkspace)
            {
                double forceGain = 0.5;
                angles = new double[N + 2];
                // calculate workspace forces if our position error is greater than the threshold
                if (Ec > IK_POS_THRESH)
                {
                    Vector3D forces = Vector3D.Multiply(forceGain, Vector3D.Subtract(Pd, Ph));
                    // invert forces if desired
                    angles[N - 1] = InvertForces[0] ? -forces.X : forces.X;
                    angles[N] = InvertForces[1] ? -forces.Y : forces.Y;
                    angles[N + 1] = InvertForces[2] ? -forces.Z : forces.Z;
                    for(int i = N-1; i < N+2; i++)
                    {
                        if (angles[i] > maxForce) angles[i] = maxForce;
                        else if (angles[i] < -maxForce) angles[i] = -maxForce;
                    }
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
            return angles;
        }

        protected override double[] getJointAngles(Point3D Position, Point3D Orientation)
        {
            if (!Initialized)
            {
                radAngle = new double[N + 1];
                radAngle.Initialize();
                Initialized = true;
            }
            // create desired position vector
            Pd = new Vector3D(Position.X, Position.Y, Position.Z);
            // create desired orientation vector from roll, pitch, yaw
            Rd = new Vector3D[3];
            // convert to radians
            Orientation.X = Orientation.X * Math.PI / 180;
            Orientation.Y = Orientation.Y * Math.PI / 180;
            Orientation.Z = Orientation.Z * Math.PI / 180;
            // convert roll/pitch/yaw to rotation matrix
            //      Z-Y-Z Euler Angles
            /*
            Rd[0].X = Math.Cos(Orientation.X) * Math.Cos(Orientation.Y) * Math.Cos(Orientation.Z) - Math.Sin(Orientation.X) * Math.Sin(Orientation.Z);
            Rd[0].Y = Math.Sin(Orientation.X) * Math.Cos(Orientation.Y) * Math.Cos(Orientation.Z) + Math.Cos(Orientation.X) * Math.Sin(Orientation.Z);
            Rd[0].Z = -Math.Sin(Orientation.Y) * Math.Cos(Orientation.Z);
            Rd[1].X = -Math.Cos(Orientation.X) * Math.Cos(Orientation.Y) * Math.Sin(Orientation.Z) - Math.Sin(Orientation.X) * Math.Cos(Orientation.Z);
            Rd[1].Y = -Math.Sin(Orientation.X) * Math.Cos(Orientation.Y) * Math.Sin(Orientation.Z) + Math.Cos(Orientation.X) * Math.Cos(Orientation.Z);
            Rd[1].Z = Math.Sin(Orientation.Y) * Math.Sin(Orientation.Z);
            Rd[2].X = Math.Cos(Orientation.X) * Math.Sin(Orientation.Y);
            Rd[2].Y = Math.Sin(Orientation.X) * Math.Sin(Orientation.Y);
            Rd[2].Z = Math.Cos(Orientation.Y);
            */
            //      X-Y-Z Fixed Angles
            //Rd[0].X = Math.Cos(Orientation.X) * Math.Cos(Orientation.Y);
            //Rd[0].Y = Math.Sin(Orientation.X) * Math.Cos(Orientation.Y);
            //Rd[0].Z = -Math.Sin(Orientation.Y);
            //Rd[1].X = Math.Cos(Orientation.X) * Math.Sin(Orientation.Y) * Math.Sin(Orientation.Z) - Math.Sin(Orientation.X) * Math.Cos(Orientation.Z);
            //Rd[1].Y = Math.Sin(Orientation.X) * Math.Sin(Orientation.Y) * Math.Sin(Orientation.Z) + Math.Cos(Orientation.X) * Math.Cos(Orientation.Z);
            //Rd[1].Z = Math.Cos(Orientation.Y) * Math.Sin(Orientation.Z);
            //Rd[2].X = Math.Cos(Orientation.X) * Math.Sin(Orientation.Y) * Math.Cos(Orientation.Z) + Math.Sin(Orientation.X) * Math.Sin(Orientation.Z);
            //Rd[2].Y = Math.Sin(Orientation.X) * Math.Sin(Orientation.Y) * Math.Cos(Orientation.Z) - Math.Cos(Orientation.X) * Math.Sin(Orientation.Z);
            //Rd[2].Z = Math.Cos(Orientation.Y) * Math.Cos(Orientation.Z);

            Rd[0].X = Math.Cos(Orientation.Y) * Math.Cos(Orientation.Z);
            Rd[0].Y = Math.Sin(Orientation.Z) * Math.Cos(Orientation.Y);
            Rd[0].Z = -Math.Sin(Orientation.Y);
            Rd[1].X = Math.Cos(Orientation.Z) * Math.Sin(Orientation.Y) * Math.Sin(Orientation.X) - Math.Sin(Orientation.Z) * Math.Cos(Orientation.X);
            Rd[1].Y = Math.Sin(Orientation.X) * Math.Sin(Orientation.Y) * Math.Sin(Orientation.Z) + Math.Cos(Orientation.X) * Math.Cos(Orientation.Z);
            Rd[1].Z = Math.Cos(Orientation.Y) * Math.Sin(Orientation.X);
            Rd[2].X = Math.Cos(Orientation.X) * Math.Sin(Orientation.Y) * Math.Cos(Orientation.Z) + Math.Sin(Orientation.X) * Math.Sin(Orientation.Z);
            Rd[2].Y = Math.Sin(Orientation.Z) * Math.Sin(Orientation.Y) * Math.Cos(Orientation.X) - Math.Cos(Orientation.Z) * Math.Sin(Orientation.X);
            Rd[2].Z = Math.Cos(Orientation.Y) * Math.Cos(Orientation.X);

            Rh = new Vector3D[3];
            // declare 3D array for each joint frame axis (xi, yi, zi, Pi)
            frame = new Vector3D[N + 1, 4];
            frame.Initialize();
            // set base frame
            frame[0, 0].X = 1;
            frame[0, 1].Y = 1;
            frame[0, 2].Z = 1;

            int link = N;
            int tries = 0;
            // begin Cyclic Coordinate Descent loop
            do
            {
                for (int i = 1; i < N + 1; i++)
                {
                    // x(i)
                    frame[i, 0] = Vector3D.Add((Vector3D.Multiply(Math.Cos(radAngle[i - 1]), frame[(i - 1), 0])), (Vector3D.Multiply(Math.Sin(radAngle[i - 1]), frame[(i - 1), 1])));
                    // z(i)
                    frame[i, 2] = Vector3D.Add((Vector3D.Multiply(Math.Cos(LinkTable[i - 1, 0]), frame[(i - 1), 2])), Vector3D.Multiply(Math.Sin(LinkTable[(i - 1), 0]), Vector3D.CrossProduct(frame[i, 0], frame[(i - 1), 2])));
                    // y(i)
                    frame[i, 1] = Vector3D.CrossProduct(frame[i, 2], frame[i, 0]);
                    //P(i)
                    frame[i, 3] = Vector3D.Add(Vector3D.Add(Vector3D.Multiply(LinkTable[i - 1, 2], frame[i - 1, 2]), Vector3D.Multiply(LinkTable[i - 1, 1], frame[i, 0])), frame[i - 1, 3]);
                }
                //for (int i = 0; i < N; i++)
                //{
                //    frame[i, 0] = frame[i + 1, 0];
                //    frame[i, 1] = frame[i + 1, 1];
                //    frame[i, 2] = frame[i + 1, 2];
                //}
                //// x(i)
                //frame[N, 0] = Vector3D.Add((Vector3D.Multiply(Math.Cos(radAngle[N - 1]), frame[(N - 1), 0])), (Vector3D.Multiply(Math.Sin(radAngle[N - 1]), frame[(N - 1), 1])));
                //// z(i)
                //frame[N, 2] = Vector3D.Add((Vector3D.Multiply(Math.Cos(LinkTable[N - 1, 0]), frame[(N - 1), 2])), Vector3D.Multiply(Math.Sin(LinkTable[(N - 1), 0]), Vector3D.CrossProduct(frame[N, 0], frame[(N - 1), 2])));
                //// y(i)
                //frame[N, 1] = Vector3D.CrossProduct(frame[N, 2], frame[N, 0]);

                // compute relative positions
                Pih = new Vector3D[N + 1];
                for (int i = N - 1; i >= 0; i--)
                {
                    Pih[i] = Vector3D.Subtract(frame[N, 3], frame[i, 3]);
                }
                // set position of end effector
                Ph = frame[N, 3];
                // set end effector orientation
                Rh[0] = frame[N, 0];
                Rh[1] = frame[N, 1];
                Rh[2] = frame[N, 2];
                // calculate orientation error
                Eo = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (Sigma[i])
                    {
                        Eo += Math.Pow((Vector3D.DotProduct(Rd[i], Rh[i]) - 1), 2);
                    }
                }
                // calculate current position error
                Ec = Eo + Math.Pow(Vector3D.DotProduct(Vector3D.Subtract(Pd, Ph), Vector3D.Subtract(Pd, Ph)), 2);

                if ((Ec > IK_POS_THRESH) && (Ec < BETA) && (Ec > Math.Pow(Ep, 2))) // begin BFGS optimization
                {
                    double epsg = 0.0000000001;
                    double epsf = 0;
                    double epsx = 0;
                    int maxits = 0;             // maximum number of iterations, for unlimited = 0
                    double[] optiAngle = new double[N];
                    for (int i = 0; i < N; i++)
                    {
                        optiAngle[i] = radAngle[i + 1];
                    }
                    alglib.minlbfgsstate state;
                    alglib.minlbfgsreport rep;

                    alglib.minlbfgscreate(N, 3, optiAngle, out state);         // create optimizer with current joint angles for initial values
                    alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);        // set optimizer options
                    alglib.minlbfgsoptimize(state, function1_grad, null, null);     // optimize
                    alglib.minlbfgsresults(state, out optiAngle, out rep);           // get results
                    for (int i = 0; i < N; i++)
                    {
                        radAngle[i + 1] = optiAngle[i];
                        // adjust angle based on joint limits
                        if (radAngle[i + 1] < (MinMax[i].X * Math.PI / 180))
                            radAngle[i + 1] = MinMax[i].X * Math.PI / 180;
                        else if (radAngle[i + 1] > (MinMax[i].Y * Math.PI / 180))
                            radAngle[i + 1] = MinMax[i].Y * Math.PI / 180;
                    }
                }
                else if (Ec > IK_POS_THRESH) // begin Cyclic Coordinate Descent loop
                {

                    // create target effector position vector
                    Vector3D Pid = Vector3D.Subtract(Pd, frame[link, 3]);
                    double wp = 1;      // position weight
                    double wo = 1;      // orientation weight

                    double k1 = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (Sigma[i])
                            k1 += wo * Vector3D.DotProduct(Rd[i], frame[link, 2]) * Vector3D.DotProduct(Rh[i], frame[link, 2]);
                    }
                    k1 += wp * Vector3D.DotProduct(Pid, frame[link, 2]) * Vector3D.DotProduct(Pih[link], frame[link, 2]);

                    double k2 = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (Sigma[i])
                            k2 += wo * Vector3D.DotProduct(Rd[i], Rh[i]);
                    }
                    k2 += wp * Vector3D.DotProduct(Pid, Pih[link]);

                    double k3 = 0;
                    Vector3D ko3 = new Vector3D();
                    for (int i = 0; i < 3; i++)
                    {
                        if (Sigma[i])
                            ko3 = Vector3D.Add(ko3, wo * Vector3D.CrossProduct(Rh[i], Rd[i]));
                    }
                    k3 = Vector3D.DotProduct(frame[link, 2], Vector3D.Add(wp * Vector3D.CrossProduct(Pih[link], Pid), ko3));
                    // minimize position and orientation error
                    double turnAngle = Math.Atan(-k3 / (k1 - k2));
                    radAngle[link] += turnAngle;
                    // adjust angle based on joint limits
                    if (radAngle[link] < (MinMax[link - 1].X * Math.PI / 180))
                        radAngle[link] = MinMax[link - 1].X * Math.PI / 180;
                    else if (radAngle[link] > (MinMax[link - 1].Y * Math.PI / 180))
                        radAngle[link] = MinMax[link - 1].Y * Math.PI / 180;
                    if (double.IsNaN(radAngle[link]))
                        radAngle[link] = 0;
                    //}

                    // backward recurssion through joints for CCD
                    if (link-- < 2) link = N;
                }
                // set previous error value for next loop
                Ep = Ec;
            }
            while (tries++ < IK_MAX_TRIES && Ec > IK_POS_THRESH);

            double[] angles;
            // check if we are outputting workspace forces
            if (OutputWorkspace)
            {
                double forceGain = 0.5;
                angles = new double[N + 2];
                // calculate workspace forces if our position error is greater than the threshold
                if (Ec > IK_POS_THRESH)
                {
                    Vector3D forces = Vector3D.Multiply(forceGain, Vector3D.Subtract(Pd, Ph));
                    // invert forces if desired
                    angles[N - 1] = InvertForces[0] ? -forces.X : forces.X;
                    angles[N] = InvertForces[1] ? -forces.Y : forces.Y;
                    angles[N + 1] = InvertForces[2] ? -forces.Z : forces.Z;
                    for (int i = N - 1; i < N + 2; i++)
                    {
                        if (angles[i] > maxForce) angles[i] = maxForce;
                        else if (angles[i] < -maxForce) angles[i] = -maxForce;
                    }
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
                angles = new double[N];
            // change output angles based on joint coupling
            switch (Coupling)
            {
                case CouplingType.None:
                    //convert angles to degrees
                    for (int i = 0; i < N; i++)
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
            }
            return angles;
        }

        public void function1_grad(double[] q, ref double func, double[] grad, object obj)
        {
            // calculate frame positions
            for (int i = 1; i < N + 1; i++)
            {
                // x(i)
                frame[i, 0] = Vector3D.Add((Vector3D.Multiply(Math.Cos(q[i - 1]), frame[(i - 1), 0])), (Vector3D.Multiply(Math.Sin(q[i - 1]), frame[(i - 1), 1])));
                // z(i)
                frame[i, 2] = Vector3D.Add((Vector3D.Multiply(Math.Cos(LinkTable[i - 1, 0]), frame[(i - 1), 2])), Vector3D.Multiply(Math.Sin(LinkTable[(i - 1), 0]), Vector3D.CrossProduct(frame[i, 0], frame[(i - 1), 2])));
                // y(i)
                frame[i, 1] = Vector3D.CrossProduct(frame[i, 2], frame[i, 0]);
                //P(i)
                frame[i, 3] = Vector3D.Add(Vector3D.Add(Vector3D.Multiply(LinkTable[i - 1, 2], frame[i - 1, 2]), Vector3D.Multiply(LinkTable[i - 1, 1], frame[i, 0])), frame[i - 1, 3]);
            }
            //for (int i = 0; i < N; i++)
            //{
            //    frame[i, 0] = frame[i + 1, 0];
            //    frame[i, 1] = frame[i + 1, 1];
            //    frame[i, 2] = frame[i + 1, 2];
            //}
            //// x(i)
            //frame[N, 0] = Vector3D.Add((Vector3D.Multiply(Math.Cos(radAngle[N - 1]), frame[(N - 1), 0])), (Vector3D.Multiply(Math.Sin(radAngle[N - 1]), frame[(N - 1), 1])));
            //// z(i)
            //frame[N, 2] = Vector3D.Add((Vector3D.Multiply(Math.Cos(LinkTable[N - 1, 0]), frame[(N - 1), 2])), Vector3D.Multiply(Math.Sin(LinkTable[(N - 1), 0]), Vector3D.CrossProduct(frame[N, 0], frame[(N - 1), 2])));
            //// y(i)
            //frame[N, 1] = Vector3D.CrossProduct(frame[N, 2], frame[N, 0]);

            // compute relative positions
            Pih = new Vector3D[N + 1];
            for (int i = N - 1; i >= 0; i--)
            {
                Pih[i] = Vector3D.Subtract(frame[N, 3], frame[i, 3]);
            }
            // set position of end effector
            Ph = frame[N, 3];
            // set end effector orientation
            Rh[0] = frame[N, 0];
            Rh[1] = frame[N, 1];
            Rh[2] = frame[N, 2];
            // calculate orientation error
            Eo = 0;
            for (int i = 0; i < 3; i++)
            {
                if (Sigma[i])
                {
                    Eo += Math.Pow((Vector3D.DotProduct(Rd[i], Rh[i]) - 1), 2);
                }
            }
            // function to be minimized
            func = Eo + Math.Pow(Vector3D.DotProduct(Vector3D.Subtract(Pd, Ph), Vector3D.Subtract(Pd, Ph)), 2);
            // declare gradient vector elements for each joint
            for (int i = 0; i < N-1; i++)
            {
                Vector3D gradO = new Vector3D();
                for (int j = 0; j < 3; j++ )
                {
                    if (Sigma[j])
                        gradO = Vector3D.Add(gradO, (Vector3D.DotProduct(Rd[j], Rh[j]) - 1) * Vector3D.CrossProduct(Rh[j], Rd[j]));
                }
                grad[i] = Vector3D.DotProduct(Vector3D.Multiply(2, frame[i, 2]), Vector3D.Add((Vector3D.CrossProduct(Vector3D.Subtract(Pd, Ph), Pih[i])), gradO));
            }
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

namespace Kinematics.Robots
{
    class JoeBizzle : IKSolver
    {
        public JoeBizzle()
        {
            // alpha(i-1)     a(i-i)     d(i)      theta(i)     joint-type
            DHparameters = new double[7, 5] { {   0,      0,      0,   0, (double)JointType.Rotation },
                                              {  90,      0,      0,   0, (double)JointType.Rotation },
                                              {  90,      0,      0,  90, (double)JointType.Rotation },
                                              {  90,      0,   66.1,  90, (double)JointType.Rotation },
                                              {  90,      0,      0, 180, (double)JointType.Rotation },
                                              {  90,      0,     70, 180, (double)JointType.Rotation },
                                              {   0,      0,      0,   0, (double)JointType.Rotation } };
            N = 6;
            EndEffector = 7;
            Sigma = new bool[3] { false, true, false };
            MinMax = new System.Windows.Point[N];
            MinMax[0] = new System.Windows.Point(-180, 180);
            MinMax[1] = new System.Windows.Point(-180, 45);
            MinMax[2] = new System.Windows.Point(-180, 180);
            MinMax[3] = new System.Windows.Point(0, 155);

            MinMax[4] = new System.Windows.Point(-180, 180);
            MinMax[5] = new System.Windows.Point(-120, 120);

            Coupling = CouplingType.None;
            OutputWorkspace = true;
            InvertForces = new bool[3] { true, true, false };
            OutputStrings = new string[9] { "Joint1", "Joint2", "Joint3", "Joint4", "Joint5", "Joint6", "FX", "FY", "FZ" };
            //OutputStrings = new string[7] { "Joint1", "Joint2", "Joint3", "Joint4", "FX", "FY", "FZ" };
            eps = 0.0001;
            BETA = 10;
            Lmin = 75;
            Lmax = 165;
        }
    }
}

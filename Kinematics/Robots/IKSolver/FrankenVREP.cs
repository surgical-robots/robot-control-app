namespace Kinematics.Robots
{
    class FrankenVREP : IKSolver
    {
        public FrankenVREP()
        {
            // alpha(i-1)     a(i-i)     d(i)      theta(i)
            DHparameters = new double[6, 5] { {   0,      0,      0,   0, (double)JointType.Rotation },
                                              {  90,      0,      0,   0, (double)JointType.Rotation },
                                              {  90,      0,      0,  90, (double)JointType.Rotation }, 
                                              {  90,      0,   53.3,  90, (double)JointType.Rotation },
                                              {  90,      0,      0, 180, (double)JointType.Rotation },
                                              {  90,      0,     80, 180, (double)JointType.Rotation } };
            N = 4;
            EndEffector = 6;
            Sigma = new bool[3] { false, true, false };
            MinMax = new System.Windows.Point[N];
            MinMax[0] = new System.Windows.Point(-180, 45);
            MinMax[1] = new System.Windows.Point(-90, 20);
            MinMax[2] = new System.Windows.Point(-90, 90);
            MinMax[3] = new System.Windows.Point(0, 140);

            Coupling = CouplingType.None;
            OutputWorkspace = false;
            InvertForces = new bool[3] { false, false, false };
            OutputStrings = new string[4] { "Joint1", "Joint2", "Joint3", "Joint4" };
            eps = 0.0001;
            BETA = 10;
            Lmin = 50;
            Lmax = 130;
        }
    }
}

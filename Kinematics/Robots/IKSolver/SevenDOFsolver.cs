namespace Kinematics.Robots
{
    class SevenDOFsolver : IKSolver
    {
        public SevenDOFsolver()
        {
            this.DHparameters = new double[7, 4] { {   0,      0,      0, 0 },
                                                   { -90,      0,      0, 0 },
                                                   {  90, 291.26,      0, 0 }, 
                                                   { -90,      0,      0, 0 },
                                                   {  90, 323.63,      0, 0 },
                                                   { -90,      0,      0, 0 },
                                                   {  90,     75,      0, 0 } };
            this.N = 7;
            this.Sigma = new bool[3] { true, true, true };
            this.MinMax = new System.Windows.Point[N];
            this.MinMax[0] = new System.Windows.Point(-180, 180);
            this.MinMax[1] = new System.Windows.Point(-180, 180);
            this.MinMax[2] = new System.Windows.Point(-180, 180);
            this.MinMax[3] = new System.Windows.Point(-180, 180);
            this.MinMax[4] = new System.Windows.Point(-180, 180);
            this.MinMax[5] = new System.Windows.Point(-180, 180);
            this.MinMax[6] = new System.Windows.Point(-180, 180);

            this.Coupling = CouplingType.None;
            this.OutputWorkspace = false;
            this.InvertForces = new bool[3] { false, false, false };
            this.OutputStrings = new string[7] { "Joint1", "Joint2", "Joint3", "Joint4", "Joint5", "Joint6", "Joint7" };
            this.IK_POS_THRESH = 1;
            this.BETA = 10;
        }
    }
}

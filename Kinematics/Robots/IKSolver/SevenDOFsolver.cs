namespace Kinematics.Robots
{
    class SevenDOFsolver : IKSolver
    {
        public SevenDOFsolver()
        {
            this.LinkTable = new double[7, 3] { {   0,      0,      0 }, 
                                                {  90,      0,  139.2 },
                                                {  90,      0,      0 },
                                                {  90,      0,  291.3 }, 
                                                {  90,      0,      0 },
                                                {  90,      0,  323.6 },
                                                {  90,      0,      0 } };
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
            this.OutputWorkspace = true;
            this.InvertForces = new bool[3] { false, false, true };
            this.OutputStrings = new string[9] { "Joint1", "Joint2", "Joint3", "Joint4", "Joint5", "Joint6", "ForceX", "ForceY", "ForceZ" };
            this.IK_POS_THRESH = 1;
            this.BETA = 10;
        }
    }
}

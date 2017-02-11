namespace Kinematics.Robots
{
    class FiveDOFcheater : IKSolver5DOF
    {
        public FiveDOFcheater()
        {
            /// alpha(i-1)     a(i-i)     d(i)      theta(i)
            this.DHparameters = new double[6, 4] { {   0,      0,      0,   0 },
                                                   {  90,      0,      0,   0 },
                                                   {  90,      0,      0,  90 }, 
                                                   {  90,      0,   53.3,  90 },
                                                   {  90,      0,      0, 180 },
                                                   {  90,      0,     80, 180 } };
            this.N = 6;
            this.Sigma = new bool[3] { false, false, false };
            this.MinMax = new System.Windows.Point[N];
            this.MinMax[0] = new System.Windows.Point(-90, 90);
            this.MinMax[1] = new System.Windows.Point(-90, 30);
            this.MinMax[2] = new System.Windows.Point(-90, 90);
            this.MinMax[3] = new System.Windows.Point(0, 140);
            this.MinMax[4] = new System.Windows.Point(-180, 180);
            this.MinMax[5] = new System.Windows.Point(-180, 180);

            this.Coupling = CouplingType.FrankenBot;
            this.OutputWorkspace = false;
            this.InvertForces = new bool[3] { false, false, false };
            this.OutputStrings = new string[6] { "Joint1", "Joint2", "Joint3", "Joint4", "Joint5", "Joint6" };
            this.eps = 0.1;
            this.BETA = 10;
        }
    }
}

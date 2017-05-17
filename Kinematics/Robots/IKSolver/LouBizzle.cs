namespace Kinematics.Robots
{
    class LouBizzle : IKSolver5DOF
    {
        public LouBizzle()
        {
            /// alpha(i-1)     a(i-i)     d(i)      theta(i)
            this.DHparameters = new double[6, 4] { {   0,      0,      0,   0 },
                                                   {  90,      0,      0,   0 },
                                                   {  90,      0,      0,  90 }, 
                                                   {  90,      0,  87.57,  90 },
                                                   {  90,      0,      0, 180 },
                                                   {  90,      0,     92, 180 } };
            this.N = 6;
            this.Sigma = new bool[3] { false, true, false };
            this.MinMax = new System.Windows.Point[N];
            this.MinMax[0] = new System.Windows.Point(-90, 10);
            this.MinMax[1] = new System.Windows.Point(-90, 30);
            this.MinMax[2] = new System.Windows.Point(-180, 180);
            this.MinMax[3] = new System.Windows.Point(0, 155);
            this.MinMax[4] = new System.Windows.Point(-180, 180);
            this.MinMax[5] = new System.Windows.Point(-180, 180);

            this.Coupling = CouplingType.LouShoulder;
            this.OutputWorkspace = false;
            this.InvertForces = new bool[3] { true, true, false };
            this.OutputStrings = new string[4] { "Joint1", "Joint2", "Joint3", "Joint4" };
            this.eps = 0.0001;
            this.BETA = 10;
            this.Lmin = 75;
            this.Lmax = 179.57;
        }
    }
}

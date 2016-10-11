namespace Kinematics.Robots
{
    class FourDOF_needle : IKSolver
    {
        double L1 = 20;
        double L2 = 30;
        double r = 0;
        public FourDOF_needle()
        {
            this.DHparameters = new double[3, 4] { {   0,      0,      0, 0 },
                                                   {  90,      0,      0, 0 },
                                                   {   0,     L1,      0, 0 }, };
            /*
            this.DHparameters = new double[5, 4] { {   0,      0,      0, 0 },
                                                   {  90,      0,      0, 0 },
                                                   { -90,     L1,      0, 0 }, 
                                                   {  90,      0,     L2, 0 },
                                                   {   0,   -2*r,      0, 0 } };
             * */
            this.N = 3;
            this.Sigma = new bool[3] { false, false, false };
            this.MinMax = new System.Windows.Point[N];
            this.MinMax[0] = new System.Windows.Point(-180, 180);
            this.MinMax[1] = new System.Windows.Point(-180, 180);
            this.MinMax[2] = new System.Windows.Point(-180, 180);
            //this.MinMax[3] = new System.Windows.Point(-180, 180);
            //this.MinMax[4] = new System.Windows.Point(-180, 180);

            this.Coupling = CouplingType.None;
            this.OutputWorkspace = false;
            this.InvertForces = new bool[3] { false, false, false };
            this.OutputStrings = new string[3] { "Joint1", "Joint2", "Joint3"};
            this.IK_POS_THRESH = 1;
            this.BETA = 10;
        }
    }
}

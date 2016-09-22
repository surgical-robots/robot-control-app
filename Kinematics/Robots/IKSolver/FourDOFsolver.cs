namespace Kinematics.Robots
{
    class FourDOFsolver : IKSolver
    {
        public FourDOFsolver()
        {
            this.DHparameters = new double[4, 4] { {   0,      0, 0, 0 }, 
                                                   {  90,      0, 0, 0 }, 
                                                   { -90,  68.58, 0, 0 }, 
                                                   {  90, 96.393, 0, 0 } };
            this.N = 4;
            this.Sigma = new bool[3] { false, false, false };
            this.MinMax = new System.Windows.Point[N];
            this.MinMax[0] = new System.Windows.Point(-90, 90);
            this.MinMax[1] = new System.Windows.Point(-90, 45);
            this.MinMax[2] = new System.Windows.Point(0, 105);
            this.MinMax[3] = new System.Windows.Point(-180, 180);
            this.Coupling = CouplingType.ShoulderTwoDOF;
            this.OutputWorkspace = true;
            this.InvertForces = new bool[3] { false, false, true };
            this.OutputStrings = new string[6] { "Upper Bevel", "Lower Bevel", "Elbow", "Workspace FX", "Workspace FY", "Workspace FZ" };
            this.IK_POS_THRESH = 0.01;
            this.BETA = 1;
        }
    }
}

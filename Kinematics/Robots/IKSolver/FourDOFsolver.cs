namespace Kinematics.Robots
{
    class FourDOFsolver : IKSolver
    {
        public FourDOFsolver()
        {
            this.DHparameters = new double[4, 5] { {   0,      0, 0, 0, (double)JointType.Rotation  }, 
                                                   {  90,      0, 0, 0, (double)JointType.Rotation  }, 
                                                   { -90,  68.58, 0, 0, (double)JointType.Rotation  }, 
                                                   {  90, 96.393, 0, 0, (double)JointType.Rotation  } };
            this.N = 4;                                               
            this.Sigma = new bool[3] { false, false, false };         
            this.MinMax = new System.Windows.Point[N];
            this.MinMax[0] = new System.Windows.Point(-90, 90);
            this.MinMax[1] = new System.Windows.Point(-90, 45);
            this.MinMax[2] = new System.Windows.Point(0, 105);
            this.MinMax[3] = new System.Windows.Point(-180, 180);
            this.Coupling = CouplingType.None;
            this.OutputWorkspace = false;
            this.InvertForces = new bool[3] { false, false, true };
            this.OutputStrings = new string[3] { "Upper Bevel", "Lower Bevel", "Elbow" };
            this.eps = 0.01;
            this.BETA = 1;
        }
    }
}

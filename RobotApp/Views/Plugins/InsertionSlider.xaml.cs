namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for InsertionSlider.xaml
    /// </summary>
    public partial class InsertionSlider : PluginBase
    {
        public InsertionSlider()
        {
            this.TypeName = "Insertion Slider";
            this.InstanceName = "Insertion";
            InitializeComponent();

            Outputs.Add("LeftUpperBevel", new ViewModel.OutputSignalViewModel("Left Upper Bevel"));
            Outputs.Add("LeftLowerBevel", new ViewModel.OutputSignalViewModel("Left Lower Bevel"));
            Outputs.Add("LeftElbow", new ViewModel.OutputSignalViewModel("Left Elbow"));
            Outputs.Add("RightUpperBevel", new ViewModel.OutputSignalViewModel("Right Upper Bevel"));
            Outputs.Add("RightLowerBevel", new ViewModel.OutputSignalViewModel("Right Lower Bevel"));
            Outputs.Add("RightElbow", new ViewModel.OutputSignalViewModel("Right Elbow"));
        }

        public void SetAngles()
        {
            double elbowAngle;
            double shoulderAngle1;
            double shoulderAngle2;

            if (sliderValue <= 50)
            {
                elbowAngle = 90 * (sliderValue / 50);
                Outputs["LeftElbow"].Value = elbowAngle;
                Outputs["LeftUpperBevel"].Value = 0;
                Outputs["LeftLowerBevel"].Value = -180;
                Outputs["RightElbow"].Value = elbowAngle;
                Outputs["RightUpperBevel"].Value = 0;
                Outputs["RightLowerBevel"].Value = -180;
            }
            else if (sliderValue < 75)
            {
                shoulderAngle1 = 105 * ((sliderValue - 50) / 25);
                shoulderAngle2 = 75 * ((sliderValue - 50) / 25);
                elbowAngle = 90;
                Outputs["LeftElbow"].Value = elbowAngle;
                Outputs["LeftUpperBevel"].Value = -shoulderAngle2;
                Outputs["LeftLowerBevel"].Value = -180 + shoulderAngle1;
                Outputs["RightElbow"].Value = elbowAngle;
                Outputs["RightUpperBevel"].Value = -shoulderAngle2;
                Outputs["RightLowerBevel"].Value = -180 + shoulderAngle1;
            }
            else
            {
                shoulderAngle1 = 75 * ((100 - sliderValue) / 25);
                elbowAngle = 90 * ((100 - sliderValue) / 25);
                Outputs["LeftElbow"].Value = elbowAngle;
                Outputs["LeftUpperBevel"].Value = -shoulderAngle1;
                Outputs["LeftLowerBevel"].Value = -shoulderAngle1;
                Outputs["RightElbow"].Value = elbowAngle;
                Outputs["RightUpperBevel"].Value = -shoulderAngle1;
                Outputs["RightLowerBevel"].Value = -shoulderAngle1;
            }
        }

        /// <summary>
        /// The <see cref="SliderValue" /> property's name.
        /// </summary>
        public const string SliderValuePropertyName = "SliderValue";

        private double sliderValue = 0;

        /// <summary>
        /// Sets and gets the SliderValue property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double SliderValue
        {
            get
            {
                return sliderValue;
            }

            set
            {
                if (sliderValue == value)
                {
                    return;
                }

                sliderValue = value;
                SetAngles();
                RaisePropertyChanged(SliderValuePropertyName);
            }
        }
    }
}

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for DummyController.xaml
    /// </summary>
    public partial class DummyController : PluginBase
    {
        public DummyController()
        {
            this.TypeName = "Dummy Controller";
            InitializeComponent();

            Outputs.Add("Output1", new ViewModel.OutputSignalViewModel("Output1"));
            Outputs.Add("Output2", new ViewModel.OutputSignalViewModel("Output2"));
            Outputs.Add("Output3", new ViewModel.OutputSignalViewModel("Output3"));
            Outputs.Add("Output4", new ViewModel.OutputSignalViewModel("Output4"));
            Outputs.Add("Output5", new ViewModel.OutputSignalViewModel("Output5"));
            Outputs.Add("Output6", new ViewModel.OutputSignalViewModel("Output6"));
        }

        /// <summary>
        /// The <see cref="InputOne" /> property's name.
        /// </summary>
        public const string InputOnePropertyName = "InputOne";

        private double inputOne = 0;

        /// <summary>
        /// Sets and gets the InputOne property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double InputOne
        {
            get
            {
                return inputOne;
            }

            set
            {
                if (inputOne == value)
                {
                    return;
                }

                inputOne = value;
                Outputs["Output1"].Value = inputOne;
                RaisePropertyChanged(InputOnePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="InputTwo" /> property's name.
        /// </summary>
        public const string InputTwoPropertyName = "InputTwo";

        private double inputTwo = 0;

        /// <summary>
        /// Sets and gets the InputTwo property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double InputTwo
        {
            get
            {
                return inputTwo;
            }

            set
            {
                if (inputTwo == value)
                {
                    return;
                }

                inputTwo = value;
                Outputs["Output2"].Value = inputTwo;
                RaisePropertyChanged(InputTwoPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="InputThree" /> property's name.
        /// </summary>
        public const string InputThreePropertyName = "InputThree";

        private double inputThree = 0;

        /// <summary>
        /// Sets and gets the InputThree property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double InputThree
        {
            get
            {
                return inputThree;
            }

            set
            {
                if (inputThree == value)
                {
                    return;
                }

                inputThree = value;
                Outputs["Output3"].Value = inputThree;
                RaisePropertyChanged(InputThreePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="InputFour" /> property's name.
        /// </summary>
        public const string InputFourPropertyName = "InputFour";

        private double inputFour = 0;

        /// <summary>
        /// Sets and gets the InputFour property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double InputFour
        {
            get
            {
                return inputFour;
            }

            set
            {
                if (inputFour == value)
                {
                    return;
                }

                inputFour = value;
                Outputs["Output4"].Value = inputFour;
                RaisePropertyChanged(InputFourPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="InputFive" /> property's name.
        /// </summary>
        public const string InputFivePropertyName = "InputFive";

        private double inputFive = 0;

        /// <summary>
        /// Sets and gets the InputFive property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double InputFive
        {
            get
            {
                return inputFive;
            }

            set
            {
                if (inputFive == value)
                {
                    return;
                }

                inputFive = value;
                Outputs["Output5"].Value = inputFive;
                RaisePropertyChanged(InputFivePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="InputSix" /> property's name.
        /// </summary>
        public const string InputSixPropertyName = "InputSix";

        private double inputSix = 0;

        /// <summary>
        /// Sets and gets the InputSix property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double InputSix
        {
            get
            {
                return inputSix;
            }

            set
            {
                if (inputSix == value)
                {
                    return;
                }

                inputSix = value;
                Outputs["Output6"].Value = inputSix;
                RaisePropertyChanged(InputSixPropertyName);
            }
        }
    }
}

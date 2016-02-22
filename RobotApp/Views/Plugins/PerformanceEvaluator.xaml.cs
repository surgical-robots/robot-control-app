using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using System.Diagnostics;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Description for Scale.
    /// </summary>
    public partial class PerformanceEvaluator : PluginBase
    {
        Stopwatch stopwatch = new Stopwatch();
        public void SendData()
        {
            stopwatch.Start();
            for(int i = 1; i<=NumberOfMessages; i++)
            {
                Outputs["StimulusOutput"].Value = i;
            }
        }

        public int NumberOfMessages { get; set; }

        public long Ticks { get; set; }

        public double MeasuredFrequency { get; set; }

        /// <summary>
        /// Initializes a new instance of the Scale class.
        /// </summary>
        public PerformanceEvaluator()
        {
            TypeName = "Performance Evaluator (Message Timer)";
            InstanceName = "PerformanceEvaluator";

            Outputs.Add("StimulusOutput", new ViewModel.OutputSignalViewModel("StimulusOutput"));

            Inputs.Add("Input", new ViewModel.InputSignalViewModel("Input", this.InstanceName));

            InitializeComponent();

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input"].UniqueID, (message) =>
            {
                if((int)message.Value == NumberOfMessages)
                {
                    stopwatch.Stop();
                    Ticks = stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    MeasuredFrequency = NumberOfMessages / Ticks;
                    this.RaisePropertyChanged("Ticks");
                    this.RaisePropertyChanged("MeasuredFrequency");
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendData();
        }



    }
}
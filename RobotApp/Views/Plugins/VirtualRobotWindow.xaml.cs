using System.Windows;
using System.Windows.Media.Media3D;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for VirtualRobotWindow.xaml
    /// </summary>
    public partial class VirtualRobotWindow : Window
    {

        public VirtualRobotWindow()
        {
            InitializeComponent();
        }

        private ModelVisual3D fullModel = null;
        /// <summary>
        /// Sets and gets the ModelGroup property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ModelVisual3D FullModel
        {
            get
            {
                return fullModel;
            }
            set
            {
                if (fullModel == value)
                    return;
                fullModel = value;
                robotModel.Children.Clear();
                robotModel.Children.Add(fullModel);
            }
        }

        //public ModelVisual3D robotModel { get; set; }

        private ModelVisual3D dModel = null;
        /// <summary>
        /// Sets and gets the ModelGroup property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ModelVisual3D DModel
        {
            get
            {
                return dModel;
            }
            set
            {
                if (dModel == value)
                    return;
                dModel = value;
                topModel.Children.Clear();
                topModel.Children.Add(dModel);
            }
        }

        private ModelVisual3D sModel = null;
        /// <summary>
        /// Sets and gets the ModelGroup property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ModelVisual3D SModel
        {
            get
            {
                return sModel;
            }
            set
            {
                if (sModel == value)
                    return;
                sModel = value;
                sideModel.Children.Clear();
                sideModel.Children.Add(sModel);
            }
        }
    }
}

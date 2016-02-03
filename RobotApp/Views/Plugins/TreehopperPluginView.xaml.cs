using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Treehopper;
using Treehopper.WPF;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for TreehopperPluginView.xaml
    /// </summary>
    public partial class TreehopperPluginView : PluginBase
    {
        public TreehopperPluginView()
        {
            InitializeComponent();
            TypeName = "Treehopper";
            Messenger.Default.Register<Treehopper.WPF.Message.BoardConnectedMessage>(this,
                message =>
                {
                    ConnectedBoard = message.Board;
                });
            Outputs.Add("Pin1", new ViewModel.OutputSignalViewModel("Pin1"));
            Outputs.Add("Pin2", new ViewModel.OutputSignalViewModel("Pin1"));
            Outputs.Add("Pin3", new ViewModel.OutputSignalViewModel("Pin1"));
        }

        /// <summary>
        /// The <see cref="ConnectedBoard" /> property's name.
        /// </summary>
        public const string ConnectedBoardPropertyName = "ConnectedBoard";

        private TreehopperBoard connectedBoard = null;

        /// <summary>
        /// Sets and gets the ConnectedBoard property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public TreehopperBoard ConnectedBoard
        {
            get
            {
                return connectedBoard;
            }

            set
            {
                if (connectedBoard == value)
                {
                    return;
                }

                connectedBoard = value;
                connectedBoard.Pin1.MakeDigitalInput();
                connectedBoard.Pin2.MakeDigitalInput();
                connectedBoard.Pin3.MakeDigitalInput();
                connectedBoard.Pin1.ValueChanged += Pin1_ValueChanged;
                connectedBoard.Pin2.ValueChanged += Pin2_ValueChanged;
                connectedBoard.Pin3.ValueChanged += Pin3_ValueChanged;
            }
        }

        void Pin3_ValueChanged(Pin sender, bool value)
        {
            Outputs["Pin1"].Value = value ? 1.0 : 0.0;
        }

        void Pin2_ValueChanged(Pin sender, bool value)
        {
            Outputs["Pin2"].Value = value ? 1.0 : 0.0;
        }

        void Pin1_ValueChanged(Pin sender, bool value)
        {
            Outputs["Pin3"].Value = value ? 1.0 : 0.0;
        }
    }
}

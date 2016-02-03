using GalaSoft.MvvmLight.Messaging;
using RobotApp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace RobotApp.Views.Plugins
{
    public class PluginBase : UserControl, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The <see cref="PluginArea" /> property's name.
        /// </summary>
        public const string PluginAreaPropertyName = "PluginArea";

        public StackPanel OutputsStackPanel { get; set; }

        /// <summary>
        /// This is the collection of signal source view models that will appear in the plugin GUI.
        /// </summary>
        public ObservableDictionary<string, OutputSignalViewModel> Outputs { get; set; }

        public ObservableDictionary<string, InputSignalViewModel> Inputs { get; set; }

        public ObservableDictionary<string, object> PluginData { get; set; }

        public StackPanel MainArea { get; set; }

        private StackPanel pluginArea = null;

        public static DependencyProperty PluginContentProperty = DependencyProperty.Register("PluginContent", typeof(StackPanel), typeof(PluginBase));

        public StackPanel PluginContent
        {
            get { return (StackPanel)GetValue(PluginContentProperty);  }
            set { SetValue(PluginContentProperty, value);  }
        }
               

        public PluginBase()
        {
            // Any inputs the plugin may have
            Inputs = new ObservableDictionary<string, InputSignalViewModel>();

            // Create style resources with the correct 5px margin
            Style ButtonStyle = new Style(typeof(Button), (Style)FindResource(typeof(Button)));
            ButtonStyle.Setters.Add(new Setter(Button.MarginProperty, new Thickness(5)));
            // StackPanel that holds our signal sources
            OutputsStackPanel = new StackPanel();
                        
            // "Signal Source Mapping" title textblock
            TextBlock signalSourceViewTitle = new TextBlock();
            signalSourceViewTitle.Text = "Signal Output Mapping";
            signalSourceViewTitle.Style = (Style)Application.Current.Resources["Title"];
            OutputsStackPanel.Children.Add(signalSourceViewTitle);
            
            // ObservableDictionary to hold all of the signal sources that our plugin might have
            Outputs = new ObservableDictionary<string, OutputSignalViewModel>();

            // GUI element to display all of the items properly using a SignalSourceView
            ItemsControl SignalSourceControl = new ItemsControl();
            DataTemplate signalDataTemplate = CreateTemplate(typeof(OutputSignalViewModel), typeof(SignalSourceView));
            var key = signalDataTemplate.DataTemplateKey;
            if(Application.Current.Resources[key] == null)
                Application.Current.Resources.Add(key, signalDataTemplate);
            SignalSourceControl.SetBinding(ItemsControl.ItemsSourceProperty, "Outputs.Values");
            OutputsStackPanel.Children.Add(SignalSourceControl);

            // Set the plugin datacontext to itself -- we assume we're not doing separate view models.
            this.DataContext = this;
            MainArea = new StackPanel();
            SetCurrentValue(PluginContentProperty, new StackPanel());

            // [Plugin Name] Configuration title textblock
            TextBlock title = new TextBlock();
            title.SetBinding(TextBlock.TextProperty, "DisplayTitle");
            title.Style = (Style)Application.Current.Resources["Title"];

            // Instance name GUI stuff
            UniformGrid grid = new UniformGrid();
            TextBlock InstanceNameLabel = new TextBlock();
            InstanceNameLabel.Text = "Instance Name";
            TextBox InstanceNameBox = new TextBox();
            Binding binding = new Binding();
            binding.Path = new PropertyPath("InstanceName");
            InstanceNameBox.SetBinding(TextBox.TextProperty, binding);
            grid.Columns = 2;
            grid.Children.Add(InstanceNameLabel);
            grid.Children.Add(InstanceNameBox);
            MainArea.Children.Add(title);
            MainArea.Children.Add(grid);
            
            this.Initialized += PluginBase_Initialized;
        }

        static public void EnumVisual(Visual myVisual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);

                // Do processing of the child visual object.
                if (typeof(Control).IsAssignableFrom(childVisual.GetType()))
                {
                    ((Control)childVisual).Margin = new Thickness(5);
                }
                              
                // Enumerate children of the child visual object.
                EnumVisual(childVisual);
            }
        }

        // Once the actual plugin gets initialized, jump back to PluginBase and set its content appropriately.
        void PluginBase_Initialized(object sender, EventArgs e)
        {
            MainArea.Children.Add(PluginContent);
            MainArea.Children.Add(OutputsStackPanel);
            EnumVisual(MainArea);
            this.Content = MainArea;
        }

        /// <summary>
        /// The <see cref="TypeName" /> property's name.
        /// </summary>
        public const string TypeNamePropertyName = "TypeName";

        private string typeName = "";

        /// <summary>
        /// Sets and gets the TypeName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string TypeName
        {
            get
            {
                return typeName;
            }

            set
            {
                if (typeName == value)
                {
                    return;
                }

                typeName = value;
                RaisePropertyChanged(TypeNamePropertyName);
                RaisePropertyChanged(DisplayTitle);
            }
        }

        /// <summary>
        /// Returns "[TypeName] Configuration" -- which is used in the title block.
        /// </summary>
        public string DisplayTitle { get { return TypeName + " Configuration";  } }

        /// <summary>
        /// The <see cref="InstanceName" /> property's name.
        /// </summary>
        public const string InstanceNamePropertyName = "InstanceName";

        private string instanceName = "";

        /// <summary>
        /// Sets and gets the InstanceName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string InstanceName
        {
            get
            {
                return instanceName;
            }

            set
            {
                if (instanceName == value)
                {
                    return;
                }

                instanceName = value;
                RaisePropertyChanged(InstanceNamePropertyName);
                foreach(var vm in Inputs)
                {
                    vm.Value.ParentInstanceName = value;
                }
            }
        }
        
        public void RaisePropertyChanged(string propName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Dispose()
        {
            foreach(var input in Inputs)
            {
                MainViewModel.Instance.InputSignalRegistry.Remove(input.Value.UniqueID);
            }
        }

        public virtual void PostLoadSetup()
        {

        }

        DataTemplate CreateTemplate(Type viewModelType, Type viewType)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

            var context = new ParserContext();

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("vm", "vm");
            context.XmlnsDictionary.Add("v", "v");

            var template = (DataTemplate)XamlReader.Parse(xaml, context);
            return template;
        }
    }

    [DataContract]
    public class IDInformation : IExtensibleDataObject
    {
        private ExtensionDataObject ExtensionDataValue;
        public ExtensionDataObject ExtensionData
        {
            get { return ExtensionDataValue; }
            set { ExtensionDataValue = value; }
        }

        [DataMember]
        public string ID;
    }
}

using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotApp.ViewModel.Plugins;
using RobotApp.Views.Plugins;
namespace RobotApp.ViewModel
{
    /// <summary>
    /// This is the ViewModel associated with the PluginConfiguration page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To add a new Plugin Controller, you'll need to create a new view model in ViewModels\PluginControllers.
    /// Derive your class from <see cref="PluginControllerViewModelBase"/>
    /// </para>
    /// </remarks>
    public class PluginConfigurationViewModel
    {
        public ObservableCollection<Type> PluginTypes { get; set; }

        public MainViewModel MainViewModel { get { return MainViewModel.Instance;  } }

        public PluginConfigurationViewModel()
        {

            var ListOfPluginTypes = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from lType in lAssembly.GetTypes()
                                     where typeof(PluginBase).IsAssignableFrom(lType)
                                     select lType).ToArray();
   
            PluginTypes = new ObservableCollection<Type>(ListOfPluginTypes);
        }

        private RelayCommand<PluginBase> deletePluginCommand;

        /// <summary>
        /// Gets the DeletePluginCommand.
        /// </summary>
        public RelayCommand<PluginBase> DeletePluginCommand
        {
            get
            {
                return deletePluginCommand
                    ?? (deletePluginCommand = new RelayCommand<PluginBase>(
                    p =>
                    {
                        MainViewModel.Plugins.Remove(p);
                    }));
            }
        }

        private RelayCommand<Type> addPluginCommand;

        /// <summary>
        /// Gets the AddPlugin command.
        /// </summary>
        public RelayCommand<Type> AddPluginCommand
        {
            get
            {
                return addPluginCommand
                    ?? (addPluginCommand = new RelayCommand<Type>(
                    p =>
                    {
                        if(p != null)
                            MainViewModel.Plugins.Add((PluginBase)Activator.CreateInstance(p));
                    }));
            }
        }
    }
}

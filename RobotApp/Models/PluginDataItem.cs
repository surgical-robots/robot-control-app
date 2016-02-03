using RobotApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RobotApp; 

namespace RobotApp.Models
{
    [DataContract]
    [Serializable]
    public class PluginDataItem
    {
        public string UniqueID;

        [DataMember]
        public string TypeName;

        [DataMember]
        public string InstanceName;

        [DataMember]
        public ObservableDictionary<string, OutputSignalViewModel> Outputs;

        [DataMember]
        public ObservableDictionary<string, InputSignalViewModel> Inputs;

        [DataMember]
        public ObservableDictionary<string, object> PluginData;

        public PluginDataItem(Views.Plugins.PluginBase plugin)
        {
            TypeName = plugin.GetType().ToString();
            Outputs = plugin.Outputs;
            Inputs = plugin.Inputs;
            InstanceName = plugin.InstanceName;
        }

        public Views.Plugins.PluginBase GetPlugin()
        {
            var ListOfPluginTypes = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from lType in lAssembly.GetTypes()
                                     where typeof(Views.Plugins.PluginBase).IsAssignableFrom(lType)
                                     select lType).ToArray();

            Views.Plugins.PluginBase pluginToAdd = null;
            foreach (Type pluginType in ListOfPluginTypes)
            {
                if (pluginType.ToString() == TypeName)
                {
                    pluginToAdd = (Views.Plugins.PluginBase)Activator.CreateInstance(pluginType);
                }
            }
            if (pluginToAdd != null)
            {
                pluginToAdd.Inputs = Inputs;
                foreach(var vm in pluginToAdd.Inputs.Values)
                {
                    vm.PostLoadSetup();
                }
                pluginToAdd.Outputs = Outputs;
                pluginToAdd.PluginData = PluginData;
                pluginToAdd.InstanceName = InstanceName;
            }
            return pluginToAdd;
        }
    }
}

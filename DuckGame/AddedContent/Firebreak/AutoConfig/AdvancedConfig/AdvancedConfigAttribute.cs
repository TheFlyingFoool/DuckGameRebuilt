using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    /// <summary>
    /// Marks the class for Advanced Config.
    /// This creates a JSON file that stores the data in the class.
    /// The JSON file can then be modified to modify whatever settings you want,
    /// this way it will be easier to store wackier values.
    /// </summary>
    /// <remarks>
    /// The class has to inherit from <see cref="IAdvancedConfig"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AdvancedConfigAttribute : Attribute
    {
        private static IEnumerable<(TypeInfo Class, AdvancedConfigAttribute Attribute)> s_all;
        private static Dictionary<Type, IAdvancedConfig> s_configs = new();
        private static string SaveDirPath = AutoConfigHandler.SaveDirPath + "/Advanced";
        public string FileName;
        
        public static void OnResults(Dictionary<Type,List<(MemberInfo MemberInfo, Attribute Attribute)>> lookupTable)
        {
            s_all = lookupTable[typeof(AdvancedConfigAttribute)]
                .Select(x => ((TypeInfo)x.MemberInfo, (AdvancedConfigAttribute)x.Attribute));

            if (!Directory.Exists(SaveDirPath))
                Directory.CreateDirectory(SaveDirPath);
            
            string[] existingFiles = Directory.GetFileSystemEntries(SaveDirPath);

            foreach ((TypeInfo type, AdvancedConfigAttribute attribute) in s_all)
            {
                if (existingFiles.Contains(attribute.FileName))
                    continue;

                IAdvancedConfig configData = (IAdvancedConfig)Activator.CreateInstance(type.AsType());
                configData.RevertToDefaults();
                string jsonData = JsonConvert.SerializeObject(configData, Formatting.Indented);

                try
                {
                    File.WriteAllText($"{SaveDirPath}/{attribute.FileName}.json", jsonData);
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to write Advanced Config of class [{type.Name}]", e);
                }
            }

            if (!TryLoad())
            {
                DevConsole.Log("|240,164,65|ACFG|DGRED| FAILED TO LOAD ADVANCED CONFIG");
            }
            else
            {
                SaveToFile();
            }
        }

        private static bool TryLoad()
        {
            foreach ((TypeInfo type, AdvancedConfigAttribute attribute) in s_all)
            {
                try
                {
                    Type configType = type.AsType();
                    string jsonString = File.ReadAllText($"{SaveDirPath}/{attribute.FileName}.json");
                    IAdvancedConfig configData = (IAdvancedConfig) JsonConvert.DeserializeObject(jsonString, type.AsType());
                    
                    s_configs[configType] = configData;
                }
                catch
                {
                    DevConsole.Log($"|240,164,65|ACFG|DGRED| FAILED TO LOAD AdvancedConfig FILE [{attribute.FileName}] FOR CLASS [{type.Name}]");
                }
            }
            
            return true;
        }

        public static void SaveToFile()
        {
            foreach ((TypeInfo type, AdvancedConfigAttribute attribute) in s_all)
            {
                IAdvancedConfig configData = s_configs[type.AsType()];
                string jsonData = JsonConvert.SerializeObject(configData, Formatting.Indented);
                
                File.WriteAllText($"{SaveDirPath}/{attribute.FileName}.json", jsonData);
            }
            
            DevConsole.Log("|240,164,65|ACFG|DGGREEN| SAVED ALL AdvancedConfig SUCCESSFULLY!");
        }
        
        public static TConfigClass Get<TConfigClass>() where TConfigClass : IAdvancedConfig
        {
            return (TConfigClass) s_configs[typeof(TConfigClass)];
        }

        [DrawingContext]
        public static void AdvancedConfigReloadUpdate()
        {
            if (!Keyboard.Pressed(Keys.F5))
                return;

            DevConsole.Log(TryLoad() // calls tryload and logs a different message based on the result
                ? "|240,164,65|ACFG|DGGREEN| RELOADED AdvancedConfig SUCCESSFULLY!"
                : "|240,164,65|ACFG|DGRED| FAILED TO RELOAD AdvancedConfig");
        }
        
        public AdvancedConfigAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}
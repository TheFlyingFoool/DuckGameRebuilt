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
    public sealed class AdvancedConfig : Attribute
    {
        public static IEnumerable<(TypeInfo Class, AdvancedConfig Attribute)> All;
        private static Dictionary<Type, IAdvancedConfig> s_configs = new();
        public static string SaveDirPath = AutoConfigHandler.SaveDirPath + "/Advanced";
        public string FileName;
        
        public static void OnResults(Dictionary<Type,List<(MemberInfo MemberInfo, Attribute Attribute)>> lookupTable)
        {
            All = lookupTable[typeof(AdvancedConfig)]
                .Select(x => ((TypeInfo)x.MemberInfo, (AdvancedConfig)x.Attribute));

            if (!Directory.Exists(SaveDirPath))
                Directory.CreateDirectory(SaveDirPath);
            
            string[] existingFiles = Directory.GetFileSystemEntries(SaveDirPath);

            foreach ((TypeInfo type, AdvancedConfig attribute) in All)
            {
                if (existingFiles.Any(x => new FileInfo(x).Name == $"{attribute.FileName}.json"))
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
            try
            {
                bool loadResult = TryLoad();
                
                if (!loadResult)
                {
                    DevConsole.Log("|240,164,65|ACFG|DGRED| FAILED TO LOAD ADVANCED CONFIG");
                }
                else
                {
                    SaveToFile();
                }
            }
            catch
            { }
        }

        private static bool TryLoad()
        {
            foreach ((TypeInfo type, AdvancedConfig attribute) in All)
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
                    return false;
                }
            }
            
            return true;
        }

        public static void SaveToFile()
        {
            foreach ((TypeInfo type, AdvancedConfig attribute) in All)
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
        
        public static object Get(Type configClassType)
        {
            return s_configs[configClassType];
        }

        [DrawingContext]
        public static void AdvancedConfigReloadUpdate()
        {
            if (Keyboard.Pressed(Keys.F5))
            {
                DevConsole.Log(TryLoad()
                    ? "|240,164,65|ACFG|DGGREEN| RELOADED AdvancedConfig SUCCESSFULLY!"
                    : "|240,164,65|ACFG|DGRED| FAILED TO RELOAD AdvancedConfig");
            }
        }
        
        public AdvancedConfig(string fileName)
        {
            FileName = fileName;
        }
    }
}
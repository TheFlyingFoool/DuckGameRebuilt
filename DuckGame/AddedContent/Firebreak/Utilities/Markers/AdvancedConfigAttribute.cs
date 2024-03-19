using DuckGame;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AddedContent.Firebreak
{
    public static partial class Marker
    {
        /// <summary>
        /// Marks the class for Advanced Config.
        /// This creates a JSON file that stores the data in the class.
        /// The JSON file can then be modified to modify whatever settings you want,
        /// this way it will be easier to store wackier values.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public sealed class AdvancedConfigAttribute : MarkerAttribute
        {
            private static Dictionary<Type, object> s_configs = new();
            public static List<AdvancedConfigAttribute> All = new();
            
            public static string SaveDirPath = AutoConfigHandler.SaveDirPath + "/Advanced";

            public string FileName;

            protected override void Implement()
            {
                All.Add(this);
                
                if (!Directory.Exists(SaveDirPath))
                    Directory.CreateDirectory(SaveDirPath);

                string[] existingFiles = Directory.GetFileSystemEntries(SaveDirPath);

                if (existingFiles.All(x => new FileInfo(x).Name != $"{FileName}.json"))
                {
                    Type configType = ((TypeInfo) Member).AsType();
                    object configData = Activator.CreateInstance(configType);
                    string jsonData = JsonConvert.SerializeObject(configData, Formatting.Indented);
                    
                    try
                    {
                        File.WriteAllText($"{SaveDirPath}/{FileName}.json", jsonData);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to write Advanced Config of class [{configType.Name}]", e);
                    }
                }

                if (!TryLoad())
                {
                    DevConsole.Log("|240,164,65|ACFG|DGRED| FAILED TO LOAD ADVANCED CONFIG");
                }
                else
                {
                    try
                    {
                        SaveToFile();
                    }
                    catch(Exception e)
                    {
                        DevConsole.Log("Error when trying to Save AdvancedConfigAttribute", Color.Red);
                        DevConsole.Log(e.ToString());
                    }
                }
            }

            private static bool TryLoad()
            {
                bool success = true;
                
                foreach (AdvancedConfigAttribute marker in All)
                {
                    Type configType = ((TypeInfo) marker.Member).AsType();
                    
                    try
                    {
                        string jsonString = File.ReadAllText($"{SaveDirPath}/{marker.FileName}.json");
                        object configData = JsonConvert.DeserializeObject(jsonString, configType);

                        s_configs[configType] = configData;
                    }
                    catch
                    {
                        success = false;
                        DevConsole.Log($"|240,164,65|ACFG|DGRED| FAILED TO LOAD AdvancedConfig FILE [{marker.FileName}] FOR CLASS [{configType.Name}]");
                    }
                }

                return success;
            }

            public static void SaveToFile()
            {
                foreach (AdvancedConfigAttribute marker in All)
                {
                    Type configType = ((TypeInfo) marker.Member).AsType();
                    
                    object configData = s_configs[configType];
                    string jsonData = JsonConvert.SerializeObject(configData, Formatting.Indented);

                    File.WriteAllText($"{SaveDirPath}/{marker.FileName}.json", jsonData);
                }

                DevConsole.Log("|240,164,65|ACFG|DGGREEN| SAVED ALL AdvancedConfig SUCCESSFULLY!");
            }

            public static TConfigClass Get<TConfigClass>()
            {
                return (TConfigClass)s_configs[typeof(TConfigClass)];
            }

            [UpdateContext]
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
}
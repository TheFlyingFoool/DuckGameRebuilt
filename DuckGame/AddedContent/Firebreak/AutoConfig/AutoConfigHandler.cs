using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DuckGame
{
    public static class AutoConfigHandler
    {
        private const string SaveDirName = "Data/";
        private const string MainSaveFileName = "Config" + FileExtension;
        private const string FileExtension = ".quack";
        public static string SaveDirPath = DuckFile.newSaveLocation + "DuckGame/" + SaveDirName;
        public static string MainSaveFilePath => SaveDirPath + MainSaveFileName;

        public static void Initialize()
        {
            if (!Directory.Exists(SaveDirPath))
                Directory.CreateDirectory(SaveDirPath);

            if (!File.Exists(MainSaveFilePath))
                SaveAll(false);
            else CleanForgottenFields();
            
            LoadAll();

            MonoMain.OnGameExit += SaveAll;
        }

        // this doesn't clean external files but it doesn't really matter
        // since saving cleans the file anyway
        public static void CleanForgottenFields()
        {
            List<string> keepLines = new();
            string[] allLines = File.ReadAllLines(MainSaveFilePath);

            foreach (string line in allLines)
            {
                string[] spl = line.Split('=');
                if (AutoConfigFieldAttribute.All.Any(x => x.UsableName == spl[0]))
                    keepLines.Add(line);
            }

            File.WriteAllLines(MainSaveFilePath, keepLines);
        }

        public static void SaveAll(bool isDangerous)
        {
            Log(ACAction.TrySave);
            bool failed = false;
            
            StringBuilder stringBuilder = new();
            for (int i = 0; i < AutoConfigFieldAttribute.All.Count; i++)
            {
                AutoConfigFieldAttribute attribute = AutoConfigFieldAttribute.All[i];

                if (isDangerous && attribute.PotentiallyDangerous)
                    continue;

                if (!FireSerializer.IsSerializable(attribute.MemberType))
                    throw new Exception($"No FireSerializer for type {attribute.MemberType} available. Code one yourself.");

                if (!Extensions.Try(() => FireSerializer.Serialize(attribute.Value), out string writtenValue))
                {
                    failed = true;
                    Log(ACAction.SaveFail, attribute.UsableName);
                    continue;
                }
                
                if (attribute.External is not null)
                {
                    string fileName = attribute.External + FileExtension;
                    string fullPath = SaveDirPath + fileName;
                    File.AppendAllLines(fullPath, new [] {$"{attribute.UsableName}={writtenValue}"});

                    writtenValue = fileName;
                }

                string dataLine = $"{attribute.UsableName}={writtenValue}";
                
                stringBuilder.Append(dataLine);
                stringBuilder.Append("\n");
            }

            File.WriteAllText(MainSaveFilePath, stringBuilder.ToString());
            
            if (!failed)
                Log(ACAction.SaveSuccess);
        }
        
        public static bool LoadAll()
        {
            Log(ACAction.TryLoad);
            
            List<AutoConfigFieldAttribute> all = AutoConfigFieldAttribute.All;

            if (!Extensions.Try(() => File.ReadAllLines(MainSaveFilePath), out string[] lines))
                SaveAll(false);

            if (!LoadAllInternal(all, lines))
                return false;

            Log(ACAction.LoadSuccess);
            return true;
        }

        private static bool LoadAllInternal(List<AutoConfigFieldAttribute> all, string[] lines)
        {
            bool failed = false;
            Dictionary<string, AutoConfigFieldAttribute> dic = all.ToDictionary(x => x.UsableName, x => x);
            
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] lineSplit = line.Split('=');

                if (lineSplit.Length != 2)
                {
                    Log(ACAction.LoadFail, line);
                    failed = true;
                    continue;
                }
                
                string name = lineSplit[0];
                string serializedValue = lineSplit[1];

                if (!dic.TryGetValue(name, out AutoConfigFieldAttribute attribute))
                    continue;

                if (!Extensions.Try(() => DeserializeAndSet(attribute, serializedValue)))
                {
                    Log(ACAction.LoadFail, attribute.UsableName);
                    failed = true;
                    continue;
                }
            }

            return !failed;
        }

        private static void DeserializeAndSet(AutoConfigFieldAttribute attribute, string newValue)
        {
            string serializedValue;
            if (attribute.External is null)
                serializedValue = newValue;
            else
            {
                // fix this garbage sloppy code -firebreak
                string[] lines = File.ReadAllLines(SaveDirPath + newValue);

                foreach (string line in lines)
                {
                    string[] spl = line.Split('=');

                    if (spl[0] != attribute.UsableName)
                        continue;

                    serializedValue = spl[1];
                    goto Deserialize_And_Set;
                }

                if (lines.Select(x => x.Length > 0).Count() == 1)
                    serializedValue = lines.First(x => x.Length > 0);
                else throw new Exception("External AutoConfig field not present in external file");
            }

            Deserialize_And_Set:
            object deserializedValue = FireSerializer.Deserialize(attribute.MemberType, serializedValue);
            attribute.Value = deserializedValue;
        }

        private static void Log(ACAction action, string reason = "")
        {
            string color = action switch
            {
                ACAction.TryLoad or ACAction.TrySave => Color.Wheat.ToDGColorString(),
                ACAction.LoadFail or ACAction.SaveFail => Colors.DGRed.ToDGColorString(),
                ACAction.LoadSuccess or ACAction.SaveSuccess => Colors.DGGreen.ToDGColorString()
            };
            
            DevConsole.Log("|240,164,65|ACFG " + color + action switch
            {
                ACAction.TryLoad => "Attempting load",
                ACAction.TrySave => "Attempting save",
                ACAction.LoadFail => "Load failed",
                ACAction.SaveFail => "Save failed",
                ACAction.LoadSuccess => "Load success",
                ACAction.SaveSuccess => "Save success",
            } + (reason == string.Empty ? reason : $": {reason}"));
        }

        private enum ACAction
        {
            TryLoad,
            TrySave,
            LoadFail,
            SaveFail,
            LoadSuccess,
            SaveSuccess,
        }
    }
}
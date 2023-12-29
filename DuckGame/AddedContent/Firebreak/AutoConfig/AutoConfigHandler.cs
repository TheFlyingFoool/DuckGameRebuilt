using AddedContent.Firebreak;
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
        public static string DecidedPath = "";
        public static string SaveDirPath => DecidedPath + SaveDirName;
        public static string MainSaveFilePath => SaveDirPath + MainSaveFileName;

        public static void Initialize()
        {
            if (Marker.AutoConfigAttribute.All.Count == 0) return;

            //hi hello yes, DecidedPath must be the proper one, if we force it to use the new save
            //then for people who played pre 1.5 and have their save in "Documents" this will create
            //a new save in appdata and DuckFile will try and fail to transfer the savefile making
            //it so your savefile seemingly gets deleted so be careful with this ok? okay thanks -NiK0
            string dir = "DuckGame/";
            DecidedPath = DuckFile.newSaveLocation + dir;
            if (!DuckFile.DirectoryExists(DecidedPath) && !Program.alternateSaveLocation && DuckFile.DirectoryExists(DuckFile.oldSaveLocation)) DecidedPath = DuckFile.oldSaveLocation + dir;
            

            if (!Directory.Exists(SaveDirPath)) Directory.CreateDirectory(SaveDirPath);


            if (!File.Exists(MainSaveFilePath)) SaveAll(false);

            if (!LoadAll())
                SaveAll(false);

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
                string[] spl = line.Split(new[] {'='}, 2);
                if (Marker.AutoConfigAttribute.All.Any(x => x.UsableName == spl[0]))
                    keepLines.Add(line);
            }

            File.WriteAllLines(MainSaveFilePath, keepLines);
        }

        public static void SaveAll(bool isDangerous)
        {
            if (Program.IsLanTestUser) return;
            try
            {
                Log(ACAction.TrySave);
                bool failed = false;

                // quick fix to https://canary.discord.com/channels/1004557726082400378/1005054418636517537/1153024982390161448
                HashSet<string> visitedExternalPaths = new();

                StringBuilder stringBuilder = new();
                for (int i = 0; i < Marker.AutoConfigAttribute.All.Count; i++)
                {
                    Marker.AutoConfigAttribute attribute = Marker.AutoConfigAttribute.All[i];

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

                        if (visitedExternalPaths.Add(fullPath))
                            File.WriteAllText(fullPath, string.Empty);
                        File.AppendAllLines(fullPath, new[] { $"{attribute.UsableName}={writtenValue}" });

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
            catch (IOException ex)
            {
                //screw you firebreak -NiK0
            }
        }
        
        /// <returns>True if loading succeeded</returns>
        public static bool LoadAll()
        {
            Log(ACAction.TryLoad);
            
            List<Marker.AutoConfigAttribute> all = Marker.AutoConfigAttribute.All;

            if (!Extensions.Try(() => File.ReadAllLines(MainSaveFilePath), out string[] lines))
                SaveAll(false);

            if (!LoadAllInternal(all, lines))
                return false;

            Log(ACAction.LoadSuccess);
            return true;
        }

        private static bool LoadAllInternal(List<Marker.AutoConfigAttribute> all, string[] lines)
        {
            bool failed = false;
            Dictionary<string, Marker.AutoConfigAttribute> dic = all.ToDictionary(x => x.UsableName, x => x);
            
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] lineSplit = line.Split(new[] {'='}, 2);

                if (lineSplit.Length != 2)
                {
                    Log(ACAction.LoadFail, line);
                    failed = true;
                    continue;
                }
                
                string name = lineSplit[0];
                string serializedValue = lineSplit[1];

                if (!dic.TryGetValue(name, out Marker.AutoConfigAttribute attribute))
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

        private static void DeserializeAndSet(Marker.AutoConfigAttribute attribute, string newValue)
        {
            string serializedValue;
            if (attribute.External is null)
                serializedValue = newValue;
            else
            {
                // fix this garbage sloppy code -firebreak
                string[] lines = File.ReadAllLines(SaveDirPath + newValue);

                int i = 0;
                foreach (string line in lines)
                {
                    i++;
                    // ... `=`s in the actual value will cause problems
                    string[] spl = line.Split(new[] {'='}, 2);

                    if (spl[0] != attribute.UsableName)
                        continue;

                    serializedValue = spl[1];

                    if (lines.Skip(i).Any(x => x.Split(new[] {'='}, 2)[0] == spl[0]))
                    {
                        File.WriteAllText(SaveDirPath + newValue, string.Empty);
                        throw new Exception("Duplicate AutoConfig entries for the same field");
                    }
                            
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
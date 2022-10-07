using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DuckGame
{
    public static class AutoConfigHandler
    {
        private const string SaveDirName = "Data/";
        private const string MainSaveFileName = "Config" + FileExtension;
        private const string FileExtension = ".quack";
        public static string SaveDirPath => DuckFile.oldSaveLocation + "DuckGame/" + SaveDirName;
        public static string MainSaveFilePath => SaveDirPath + MainSaveFileName;

        public static void Initialize()
        {
            DevConsole.Log("|240,164,65|ACFG|WHITE| ATTEMPTING TO LOAD CONFIG FIELD DATA...");

            if (!Directory.Exists(SaveDirPath))
                Directory.CreateDirectory(SaveDirPath);

            if (!File.Exists(MainSaveFilePath))
                SaveAll(false);

            if (!LoadAll())
                DevConsole.Log("|240,164,65|ACFG|DGRED| FAILED TO LOAD CONFIG FIELDS");

            MonoMain.OnGameExit += SaveAllClosing;
        }

        public static void SaveAll(bool isDangerous)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int length = AutoConfigFieldAttribute.All.Count;
            for (int i = 0; i < length; i++)
            {
                AutoConfigFieldAttribute attribute = AutoConfigFieldAttribute.All[i];
                MemberInfo field = attribute.field;
                bool isfield = true;
                Type fieldType;
                PropertyInfo pi = null;
                FieldInfo fi = field as FieldInfo;
                if (fi == null)
                {
                    pi = field as PropertyInfo;
                    if (pi == null)
                    {
                        throw new Exception("Unsupported AutoConfig field type");
                    }
                    isfield = false;
                    fieldType = pi.PropertyType;
                }
                else
                {
                    fieldType = fi.FieldType;
                }

                if (isDangerous && attribute.PotentiallyDangerous)
                    continue;

                if (!FireSerializer.IsSerializable(fieldType))
                    continue;
                object fieldValue;
                if (isfield)
                {
                    fieldValue = fi.GetValue(null);
                }
                else
                {
                    fieldValue = pi.GetMethod?.Invoke(null, null);
                }
                string fullName = attribute.Id ?? field.GetFullName();

                string writtenValue = FireSerializer.Serialize(fieldValue);

                if (attribute.External is not null)
                {
                    string fileName = attribute.External + FileExtension;
                    string fullPath = SaveDirPath + fileName;
                    File.WriteAllText(fullPath, writtenValue);

                    writtenValue = fileName;
                }

                string dataLine = $"{fullName}={writtenValue}";
                stringBuilder.Append(dataLine);

                if (i != length)
                    stringBuilder.Append("\n");
            }
            File.WriteAllText(MainSaveFilePath, stringBuilder.ToString());
            DevConsole.Log("|240,164,65|ACFG|DGGREEN| SAVED ALL CUSTOM CONFIG SUCCESSFULLY!");
        }
        public static void SaveAllClosing(bool isDangerous)
        {
            int length = AutoConfigFieldAttribute.All.Count;
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                AutoConfigFieldAttribute attribute = AutoConfigFieldAttribute.All[i];
                MemberInfo field = attribute.field;

                bool isfield = true;
                PropertyInfo pi = null;
                FieldInfo fi = field as FieldInfo;
                Type fieldType;
                if (fi == null)
                {
                    pi = field as PropertyInfo;
                    if (pi == null)
                    {
                        throw new Exception("Unsupported AutoConfig field type");
                    }
                    isfield = false;
                    fieldType = pi.PropertyType;
                }
                else
                {
                    fieldType = fi.FieldType;
                }
                if (isDangerous && attribute.PotentiallyDangerous)
                    continue;

                if (!FireSerializer.IsSerializable(fieldType))
                    continue;

                object fieldValue;
                if (isfield)
                {
                    fieldValue = fi.GetValue(null);
                }
                else
                {
                    fieldValue = pi.GetMethod?.Invoke(null, null);
                }
                string fullName = attribute.Id ?? field.GetFullName();

                string writtenValue = FireSerializer.Serialize(fieldValue);

                if (attribute.External is not null)
                {
                    string fileName = attribute.External + FileExtension;
                    string fullPath = SaveDirPath + fileName;
                    File.WriteAllText(fullPath, writtenValue);

                    writtenValue = fileName;
                }

                string dataLine = $"{fullName}={writtenValue}";
                stringBuilder.Append(dataLine);

                if (i != length)
                    stringBuilder.Append("\n");
            }
            try
            {
                File.WriteAllText(MainSaveFilePath, stringBuilder.ToString());
            }
            catch(Exception ex)
            {
                DevConsole.Log("e");
            }
            //DevConsole.Log("|240,164,65|ACFG|DGGREEN| SAVED ALL CUSTOM CONFIG SUCCESSFULLY!"); did i make another one just to removed this log to stop and error related to some closing stuff on linux, the answer is yes //
        }
        public static bool LoadAll()
        {
            IReadOnlyList<AutoConfigFieldAttribute> all = AutoConfigFieldAttribute.All;
            Extensions.Try(() => File.ReadAllLines(MainSaveFilePath), out string[] lines);

            if (lines is null || lines.Length == 0)
                SaveAll(false);

            // tries to load all via indexing. if that fails, tries to
            // load all via searching. if that false, returns false, 
            // otherwise returns true
            if (LoadAllIndex(all, lines) || LoadAllSearch(all, lines))
            {
                DevConsole.Log("|240,164,65|ACFG|DGGREEN| LOADED ALL CUSTOM CONFIG SUCCESSFULLY!");
                return true;
            }

            return false;
        }

        private static bool LoadAllIndex(IReadOnlyList<AutoConfigFieldAttribute> all, string[] lines)
        {
            DevConsole.Log("|240,164,65|ACFG|WHITE| ATTEMPTING CONFIG INDEX LOADING...");
            lines = lines.Where(Enumerable.Any).ToArray();

            if (all.Count != lines.Length)
                goto Fail;

            try
            {
                for (int i = 0; i < all.Count; i++)
                {
                    AutoConfigFieldAttribute _ = all[i];
                    PropertyInfo pi = null;
                    FieldInfo fi = _.field as FieldInfo;
                    Type type;
                    if (fi == null)
                    {
                        pi = _.field as PropertyInfo;
                        if (pi == null)
                        {
                            throw new Exception("Unsupported AutoConfig field type");
                        }
                        type = pi.PropertyType;
                    }
                    else
                    {
                        type = fi.FieldType;
                    }
                    string[] sides = lines[i].Split('=');

                    if (sides[0] != type.GetFullName())
                        goto Fail;

                    SetFieldValue(all[i], sides[1]);
                }
            }
            catch
            {
                goto Fail;
            }

            return true;

        Fail:
            {
                DevConsole.Log("|240,164,65|ACFG|DGRED| FAILED TO LOAD CONFIG FIELDS VIA INDEX");
                return false;
            }
        }

        private static bool LoadAllSearch(IReadOnlyList<AutoConfigFieldAttribute> all, string[] lines)
        {
            DevConsole.Log("|240,164,65|ACFG|WHITE| ATTEMPTING CONFIG SEARCH LOADING...");
            try
            {
                for (int i = 0; i < all.Count; i++)
                {
                    AutoConfigFieldAttribute attribute = all[i];
                    string fullName = attribute.Id ?? attribute.field.GetFullName();

                    if (!lines.TryFirst(x => fullName == x.Split('=')[0], out string line))
                        continue;

                    string[] sides = line.Split('=');

                    SetFieldValue(all[i], sides[1]);
                }
            }
            catch
            {
                DevConsole.Log("|240,164,65|ACFG|DGRED| FAILED TO LOAD CONFIG FIELDS VIA SEARCH");
                return false;
            }

            return true;
        }

        private static void SetFieldValue(AutoConfigFieldAttribute pair, string newValue)
        {
            MemberInfo field = pair.field;
            Type type;
            PropertyInfo pi = null;
            FieldInfo fi = field as FieldInfo;
            bool isfield = true;
            if (fi == null)
            {
                pi = field as PropertyInfo;
                if (pi == null)
                {
                    throw new Exception("Unsupported AutoConfig field type");
                }
                isfield = false;
                type = pi.PropertyType;
            }
            else
            {
                type = fi.FieldType;
            }
            object val = FireSerializer.Deserialize(type, pair.External is not null ? File.ReadAllText(SaveDirPath + newValue) : newValue);
            if (isfield)
            {
                fi.SetValue(null, val);
            }
            else
            {
                pi.SetMethod?.Invoke(null, new[] { val });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DuckGame;

public static class AutoConfigHandler
{
    private const string SaveDirName = "Data/";
    private const string MainSaveFileName = "Config" + FileExtension;
    private const string FileExtension = ".quack";
    public static string SaveDirPath => DuckFile.userDirectory + SaveDirName;
    public static string MainSaveFilePath => SaveDirPath + MainSaveFileName;

    [PostInitialize]
    public static void Initialize()
    {
        DevConsole.Log("|240,164,65|ACFG|WHITE| ATTEMPTING TO LOAD CONFIG FIELD DATA...");
        
        if (!Directory.Exists(SaveDirPath))
            Directory.CreateDirectory(SaveDirPath);
        
        if (!File.Exists(MainSaveFilePath))
            SaveAll(false);

        if (!LoadAll())
            DevConsole.Log("|240,164,65|ACFG|DGRED| FAILED TO LOAD CONFIG FIELDS");

        MonoMain.OnGameExit += SaveAll;
    }

    public static void SaveAll(bool isDangerous)
    {
        var all = AutoConfigFieldAttribute.All;
        int length = all.Count;
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            (MemberInfo field, AutoConfigFieldAttribute attribute) = all[i];
            Type fieldType = field switch
            {
                FieldInfo fi => fi.FieldType,
                PropertyInfo pi => pi.PropertyType,
                _ => throw new Exception("Unsupported AutoConfig field type")
            };
            
            if (isDangerous && attribute.PotentiallyDangerous)
                continue;

            if (!FireSerializer.IsSerializable(fieldType))
                continue;

            object fieldValue = field switch
            {
                FieldInfo fi => fi.GetValue(null),
                PropertyInfo pi => pi.GetMethod?.Invoke(null, null),
                _ => throw new Exception("Unsupported AutoConfig field type")
            };
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
    
    public static bool LoadAll()
    {
        var all = AutoConfigFieldAttribute.All;
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

    private static bool LoadAllIndex(IReadOnlyList<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>> all, string[] lines)
    {
        DevConsole.Log("|240,164,65|ACFG|WHITE| ATTEMPTING CONFIG INDEX LOADING...");
        lines = lines.Where(Enumerable.Any).ToArray();

        if (all.Count != lines.Length)
            goto Fail;
        
        try
        {
            for (int i = 0; i < all.Count; i++)
            {
                (MemberInfo field, AutoConfigFieldAttribute _) = all[i];
                Type type = field switch
                {
                    FieldInfo fi => fi.FieldType,
                    PropertyInfo pi => pi.PropertyType,
                    _ => throw new Exception("Unsupported AutoConfig field type")
                };
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

    private static bool LoadAllSearch(IReadOnlyList<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>> all, string[] lines)
    {
        DevConsole.Log("|240,164,65|ACFG|WHITE| ATTEMPTING CONFIG SEARCH LOADING...");
        try
        {
            for (int i = 0; i < all.Count; i++)
            {
                (MemberInfo field, AutoConfigFieldAttribute attribute) = all[i];
                string fullName = attribute.Id ?? field.GetFullName();

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

    private static void SetFieldValue(MemberAttributePair<MemberInfo, AutoConfigFieldAttribute> pair, string newValue)
    {
        (MemberInfo field, AutoConfigFieldAttribute attribute) = pair;
        Type type = field switch
        {
            FieldInfo fi => fi.FieldType,
            PropertyInfo pi => pi.PropertyType,
            _ => throw new Exception("Unsupported AutoConfig field type")
        };

        object val = FireSerializer.Deserialize(type, attribute.External is not null
            ? File.ReadAllText(SaveDirPath + newValue)
            : newValue);

        switch (field)
        {
            case FieldInfo fi:
            {
                fi.SetValue(null, val);
                break;
            }
            case PropertyInfo pi:
            {
                pi.SetMethod?.Invoke(null, new[] {val});
                break;
            }
        }
    }
}
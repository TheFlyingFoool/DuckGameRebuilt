using System;
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
        int length = all.Length;
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            (FieldInfo field, AutoConfigFieldAttribute attribute) = all[i];
            Type fieldType = field.FieldType;
            
            if (isDangerous && attribute.PotentiallyDangerous)
                continue;

            if (!FireSerializer.IsSerializable(fieldType))
                continue;

            object fieldValue = field.GetValue(null);
            string fullName = attribute.Id ?? field.GetFullName();

            string dataLine = $"{fullName}={FireSerializer.Serialize(fieldValue)}";
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
        string[] lines = File.ReadAllLines(MainSaveFilePath);

        if (!lines.Any())
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

    private static bool LoadAllIndex(MemberAttributePair<FieldInfo, AutoConfigFieldAttribute>[] all, string[] lines)
    {
        DevConsole.Log("|240,164,65|ACFG|WHITE| ATTEMPTING CONFIG INDEX LOADING...");
        lines = lines.Where(Enumerable.Any).ToArray();

        if (all.Length != lines.Length)
            goto Fail;
        
        try
        {
            for (int i = 0; i < all.Length; i++)
            {
                (FieldInfo field, _) = all[i];
                Type type = field.FieldType;
                string[] sides = lines[i].Split('=');

                if (sides[0] != type.GetFullName())
                    goto Fail;

                field.SetValue(null, FireSerializer.Deserialize(type, sides[1]));
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

    private static bool LoadAllSearch(MemberAttributePair<FieldInfo, AutoConfigFieldAttribute>[] all, string[] lines)
    {
        DevConsole.Log("|240,164,65|ACFG|WHITE| ATTEMPTING CONFIG SEARCH LOADING...");
        try
        {
            for (int i = 0; i < all.Length; i++)
            {
                (FieldInfo field, AutoConfigFieldAttribute attribute) = all[i];
                Type type = field.FieldType;
                string fullName = attribute.Id ?? field.GetFullName();

                if (!lines.TryFirst(x => fullName == x.Split('=')[0], out string line))
                    continue;

                string[] sides = line.Split('=');

                field.SetValue(null, FireSerializer.Deserialize(type, sides[1]));
            }
        }
        catch
        {
            DevConsole.Log("|240,164,65|ACFG|DGRED| FAILED TO LOAD CONFIG FIELDS VIA SEARCH");
            return false;
        }

        return true;
    }
}
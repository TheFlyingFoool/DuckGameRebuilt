using System;
using System.IO;
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

    
    public static void Initialize()
    {
        DevConsole.Log("|240,164,65|ACFG Attempting to load config field data...");
        
        if (!Directory.Exists(SaveDirPath))
            Directory.CreateDirectory(SaveDirPath);
        
        if (!File.Exists(MainSaveFilePath))
            SaveAll(false);
        
        if (!LoadAll())
            DevConsole.Log("|240,164,65|ACFG Failed to load configuration fields");

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
            string fullName = field.GetFullName();

            string dataLine = $"{fullName}={FireSerializer.Serialize(fieldValue)}";
            stringBuilder.Append(dataLine);

            if (i != length)
                stringBuilder.Append("\n");
        }
        
        File.WriteAllText(MainSaveFilePath, stringBuilder.ToString());
    }
    
    public static bool LoadAll()
    {
        var all = AutoConfigFieldAttribute.All;
        string[] lines = File.ReadAllLines(MainSaveFilePath);
        
        // tries to load all via indexing. if that fails, tries to
        // load all via searching. if that false, returns false, 
        // otherwise returns true
        return LoadAllIndex(all, lines) || LoadAllSearch(all, lines);
    }

    private static bool LoadAllIndex(MemberAttributePair<FieldInfo, AutoConfigFieldAttribute>[] all, string[] lines)
    {
        for (int i = 0; i < all.Length; i++)
        {
            (FieldInfo field, _) = all[i];
            Type type = field.FieldType;
            string[] sides = lines[i].Split('=');
            
            if (sides[0] != type.GetFullName())
                return false;
            
            field.SetValue(null, FireSerializer.Deserialize(type, sides[1]));
        }
        
        return true;
    }

    private static bool LoadAllSearch(MemberAttributePair<FieldInfo, AutoConfigFieldAttribute>[] all, string[] lines)
    {
        for (int i = 0; i < all.Length; i++)
        {
            (FieldInfo field, _) = all[i];
            Type type = field.FieldType;
            string fullName = field.GetFullName();
            
            if (!lines.TryFirst(x => fullName == x.Split('=')[0], out string line))
                continue;
            
            string[] sides = line.Split('=');
            
            field.SetValue(null, FireSerializer.Deserialize(type, sides[1]));
        }

        return true;
    }
}
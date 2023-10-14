using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        public static string ScriptsDirPath = $"{DuckFile.newSaveLocation}DuckGame/Scripts/";
        public const string SCRIPT_FILE_EXTENSION = "dsh";
        
        [DSHCommand(Description = "Runs a script (by extensionless name) from your ~DuckGame/Scripts/ folder")]
        public static void Source(string scriptName)
        {
            if (!Directory.Exists(ScriptsDirPath))
                Directory.CreateDirectory(ScriptsDirPath);
            
            scriptName = Regex.Replace(scriptName, @"[^\w \/\\]", "");
            
            string fullScriptName = $"{scriptName}.{SCRIPT_FILE_EXTENSION}";
            string scriptPath = $"{ScriptsDirPath}{fullScriptName}";

            if (!File.Exists(scriptPath))
                throw new Exception($"Script not found: {fullScriptName}");

            string fileContent = File.ReadAllText(scriptPath);
            
            console.Run(fileContent, false);
        }
    }
}
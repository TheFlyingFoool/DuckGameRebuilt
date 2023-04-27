using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        public static string ScriptsDirPath = DuckFile.oldSaveLocation + "DuckGame/" + "mm_scripts/";
        public const string SCRIPT_FILE_EXTENSION = "dsh";
        
        [MMCommand(Description = "Runs a script (by extensionless name) from your mm_scripts folder")]
        public static void Source(string scriptName)
        {
            scriptName = Regex.Replace(scriptName, @"[^\w \/]", "");
            
            string fullScriptName = $"{scriptName}.{SCRIPT_FILE_EXTENSION}";
            string scriptPath = $"{ScriptsDirPath}{fullScriptName}";

            if (!File.Exists(scriptPath))
                throw new Exception($"Script not found: {fullScriptName}");

            string fileContent = File.ReadAllText(scriptPath).Trim('\n', '\r');
            
            console.ExecuteCommand(fileContent);
        }
    }
}
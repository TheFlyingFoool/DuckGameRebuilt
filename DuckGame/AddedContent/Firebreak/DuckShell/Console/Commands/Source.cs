using AddedContent.Firebreak;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        public static string ScriptsDirPath => $"{DuckFile.saveDirectory}Scripts/";
        public const string SCRIPT_FILE_EXTENSION = ".dsh";
        
        [Marker.DevConsoleCommand(Description = "Runs a script from your ~DuckGame/Scripts/ folder", To = ImplementTo.DuckShell)]
        public static void Source(
            [FilePathAutoCompl(
                "|saveDirectory|Scripts",
                SystemEntryType.File,
                SearchOption.TopDirectoryOnly, // todo: support recursive directory search
                FilePathAutoComplAttribute.Return.EntryNameNoExtension)] string scriptName)
        {
            if (!Directory.Exists(ScriptsDirPath))
                Directory.CreateDirectory(ScriptsDirPath);
            
            scriptName = Regex.Replace(scriptName, @"[^\w \/\\]", "");
            
            string fullScriptName = scriptName;
            if (!fullScriptName.EndsWith(SCRIPT_FILE_EXTENSION))
                fullScriptName += SCRIPT_FILE_EXTENSION;
            
            string scriptPath = $"{ScriptsDirPath}{fullScriptName}";

            if (!File.Exists(scriptPath))
                throw new Exception($"Script not found: {fullScriptName}");

            string fileContent = File.ReadAllText(scriptPath);
            
            console.Run(fileContent, false);
        }
    }
}
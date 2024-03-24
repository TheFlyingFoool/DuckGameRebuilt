using AddedContent.Firebreak;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class LevelInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(Level);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                CMD.Level levelArg = new("");
                object parseResult = levelArg.Parse(fromString);

                if (parseResult is null)
                    return new Exception(levelArg.GetParseFailedMessage());
                
                return parseResult;
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                string[] searchPaths =
                {
                    $"{Content.path}/levels/deathmatch/",
                    DuckFile.levelDirectory,
                    $"{Content.path}/levels/"
                };

                List<string> levels = new();

                // built-in & local custom
                foreach (string searchPath in searchPaths)
                {
                    levels.AddRange(DuckFile.GetFiles(searchPath, "*.lev", SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x).Name));
                }

                // modded / mappacks
                foreach (Mod mod in ModLoader.accessibleMods)
                {
                    if (mod.configuration.content is null)
                        continue;
                    
                    levels.AddRange(mod.configuration.content.levels.Select(x => new FileInfo(x).Name));
                }
                
                // special
                levels.AddRange(CMD.Level.SpecialLevelLookup.Keys);

                return levels;
            }
        }
    }
}
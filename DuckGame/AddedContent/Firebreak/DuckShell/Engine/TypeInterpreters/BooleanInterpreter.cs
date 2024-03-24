using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class BooleanInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(bool);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                if (string.IsNullOrEmpty(fromString))
                    goto Failed;

                switch (fromString.Trim().ToLower())
                {
                    case "1":
                    case "y":
                    case "yes":
                    case "true":
                        return true;
                    
                    case "0":
                    case "n":
                    case "no":
                    case "false":
                        return false;

                    default:
                        goto Failed;
                }

                Failed: return new Exception($"Unable to parse to bool: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return new [] {"true", "false"};
            }
        }
    }
}
using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class EnumInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(Enum);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                try
                {
                    return Enum.Parse(specificType, fromString, true);
                }
                catch (Exception e)
                {
                    return new Exception($"Unable to parse to {specificType.Name}: {fromString}", e);
                }
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return Enum.GetNames(specificType);
            }
        }
    }
}
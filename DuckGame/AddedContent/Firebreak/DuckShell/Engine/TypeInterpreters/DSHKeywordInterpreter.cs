using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class KeywordInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(DSHKeyword);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                string keyword = context.ParameterInfo.Name;
                
                return fromString == keyword
                    ? ValueOrException<object>.FromValue(null)
                    : ValueOrException<object>.FromError($"Invalid syntax. Expected [{keyword}] but got [{fromString}]");
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                // this technically never gets executed because
                // i manually handled the case for DSHKeyword
                return new[] {context.ParameterInfo.Name};
            }
        }
    }
}
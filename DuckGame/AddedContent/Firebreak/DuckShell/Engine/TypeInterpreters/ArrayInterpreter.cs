using AddedContent.Firebreak;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class ArrayInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(Array);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                string[] items;

                if (string.IsNullOrEmpty(fromString))
                    items = Array.Empty<string>();
                else
                {
                    var splits = new List<string>();
                    StringBuilder current = new();
                    for (var i = 0; i < fromString.Length; i++)
                    {
                        var c = fromString[i];

                        switch (c)
                        {
                            case '\\':
                                if (i + 1 >= fromString.Length)
                                    continue;

                                current.Append(fromString[++i]);
                                continue;

                            case ',':
                                if (current.Length == 0)
                                    goto default;

                                splits.Add(current.ToString());
                                current.Clear();
                                break;

                            default:
                                current.Append(c);
                                break;
                        }
                    }

                    if (current.Length > 0) splits.Add(current.ToString());

                    items = splits.ToArray();
                }

                var values = new object[items.Length];

                Type elementType = specificType.GetElementType()!;

                if (!engine.TypeInterpreterModules.TryFirst(x => x.ParsingType.IsAssignableFrom(elementType),
                        out ITypeInterpreter interpreter))
                    return new Exception($"No conversion module found: {elementType.Name}");

                for (var i = 0; i < items.Length; i++)
                {
                    var argString = items[i];

                    var parseResult = interpreter.ParseString(argString, elementType, engine);

                    if (parseResult.Failed)
                        return new Exception($"Parsing Error: {parseResult.Error.Message}");

                    values[i] = parseResult.Value;
                }

                Array convertedArray = Array.CreateInstance(elementType, values.Length);
                Array.Copy(values, convertedArray, values.Length);

                return convertedArray;
            }

            public string[] Options(string fromString, Type specificType, CommandRunner engine)
            {
                return Array.Empty<string>();
            }
        }
    }
}
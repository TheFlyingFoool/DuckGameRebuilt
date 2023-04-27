using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public class ArrayInterpreter : ITypeInterpreter
    {
        public Type ParsingType { get; } = typeof(Array);

        public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
        {
            string[] items = fromString is null ? Array.Empty<string>() : fromString.Split(new[] {"\xb1,"}, StringSplitOptions.None);
            object[] values = new object[items.Length];

            Type elementType = specificType.GetElementType()!;

            if (!engine.TypeInterpreterModules.TryGet(x => x.ParsingType.IsAssignableFrom(elementType), out ITypeInterpreter interpreter))
                return new Exception($"No conversion module found: {elementType.Name}");

            for (int i = 0; i < items.Length; i++)
            {
                string argString = items[i];
            
                ValueOrException<object> parseResult = interpreter.ParseString(argString, elementType, engine);

                if (parseResult.Failed)
                    return new Exception($"Parsing Error: {parseResult.Error.Message}");

                values[i] = parseResult.Value;
            }

            Array convertedArray = Array.CreateInstance(elementType, values.Length);
            Array.Copy(values, convertedArray, values.Length);
        
            return convertedArray;
        }
    }
}
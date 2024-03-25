using AddedContent.Firebreak;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class LayerInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(Layer);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return Layer.core._layers.TryFirst(x => x.name.ToLower() == fromString, out Layer layer) ? layer : new Exception($"No layer found with name: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                List<Layer> layers = Layer.core._layers;
                string[] options = new string[layers.Count];

                for (int i = 0; i < options.Length; i++)
                {
                    options[i] = layers[i].name;
                }
                
                return options;
            }
        }
    }
}
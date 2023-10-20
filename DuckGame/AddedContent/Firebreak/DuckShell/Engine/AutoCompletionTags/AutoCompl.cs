using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleEngine
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AutoCompl : Attribute
    {
        private AutocompletionType _category = AutocompletionType.None;
        private Type _paramType = null;
        private string[] _selection = null;
        private Func<string[]> _func = null;
        
        // if only this was rust
        public AutoCompl() { }
        
        public AutoCompl(Type paramType)
        {
            _category = AutocompletionType.FromType;
            _paramType = paramType;
        }
        
        public AutoCompl(params string[] selection)
        {
            _category = AutocompletionType.FromSelection;
            _selection = selection;
        }
        
        // i forgot these dont go in attribute constructors but uhhh umm uh -firebreak
        public AutoCompl(Func<string[]> autocompletionFunction)
        {
            _category = AutocompletionType.FromFunction;
            _func = autocompletionFunction;
        }
        
        public virtual string[] Get(string word)
        {
            return FilterAndSortToRelevant(word, _category switch
            {
                AutocompletionType.FromType => GetFromType(_paramType, word),
                AutocompletionType.FromSelection => _selection,
                AutocompletionType.FromFunction => _func(),
                _ => Array.Empty<string>()
            });
        }

        public static string[] FilterAndSortToRelevant(string word, IEnumerable<string> choices)
        {
            word = word.ToLower();
            
            return choices
                .Select(x => x.ToLower())
                .Where(x => x.Contains(word))
                .OrderByDescending(x => x.StartsWith(word))
                .ThenBy(x => x.Length)
                .ToArray();
        }

        public static IList<string> GetFromType(Type type, string word = "")
        {
            if (Commands.console.Shell.TypeInterpreterModules
                .TryFirst(x => type.InheritsFrom(x.ParsingType), out ITypeInterpreter module))
            {
                return module.Options(word, type, Commands.console.Shell);
            }

            return Array.Empty<string>();
        }
        
        private enum AutocompletionType
        {
            None,
            FromType,
            FromSelection,
            FromFunction,
        }
    }
}
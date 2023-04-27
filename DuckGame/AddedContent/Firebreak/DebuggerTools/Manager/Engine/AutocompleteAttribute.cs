using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleEngine
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AutocompleteAttribute : Attribute
    {
        /// <summary>
        /// 0 - type-based<br/>
        /// 1 - from static list<br/>
        /// 2 - from tag
        /// </summary>
        public readonly byte AutoCompleteType;

        public readonly string[] StaticCompletionList;
        public readonly AutocompleteTag CompletionTag = default;

        public AutocompleteAttribute(string staticCompletion)
        {
            AutoCompleteType = 1;
            StaticCompletionList = new[] {staticCompletion};
        }

        public AutocompleteAttribute(string[] staticCompletionList)
        {
            AutoCompleteType = 1;
            StaticCompletionList = staticCompletionList;
        }

        public AutocompleteAttribute() // type based
        {
            AutoCompleteType = 0;
        }

        public AutocompleteAttribute(AutocompleteTag tag)
        {
            AutoCompleteType = 2;
            CompletionTag = tag;
        }

        public IEnumerable<string> GetAutocomplete(Command.Parameter[] parameters, string[] prevArgs, int argIndex, string currentArg)
        {
            Command.Parameter currentParameter = parameters[argIndex];

            switch (AutoCompleteType)
            {
                case 0: // from type
                    Type parameterType = currentParameter.ParameterType;
                    return CleanSuggestions(TypeAutoComplete(parameterType, prevArgs, currentArg), currentArg);

                case 1: // from static list
                    return CleanSuggestions(StaticCompletionList, currentArg);

                case 2: // from tag
                    AutocompleteTag autocompleteTag = currentParameter.Autocompletion!.CompletionTag;
                    return CleanSuggestions(TagAutoComplete(autocompleteTag, prevArgs, currentArg), currentArg);
            }

            return Array.Empty<string>();
        }

        private static IEnumerable<string> CleanSuggestions(IReadOnlyList<string> suggestions, string currentArg)
        {
            return suggestions
                    .Where(x => x.StartsWith(currentArg) || x.Contains(currentArg))
                    .OrderBy(x => x.Length);
        }

        private static IReadOnlyList<string> TypeAutoComplete(Type t, string[] prevArgs, string currentArg)
        {
            if (t.InheritsFrom<Enum>())
                return Enum.GetNames(t);

            return Array.Empty<string>();
        }

        private static IReadOnlyList<string> TagAutoComplete(AutocompleteTag tag, string[] prevArgs, string currentArg)
        {
            switch (tag)
            {
                case AutocompleteTag.Command:
                    // idk some recursion bullshit i'll figure out later
                    break;
            }

            return Array.Empty<string>();
        }
    }

    public enum AutocompleteTag
    {
        Command
    }
}
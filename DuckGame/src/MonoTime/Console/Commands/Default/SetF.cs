using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
using System;
using System.Reflection;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Set a state of the duck to a new value", IsCheat = true)]
        public static void SetF(
            Duck duck,
            [ReflectionAutoCompl(typeof(Duck), MemberTypes.Property | MemberTypes.Field , ALL_INSTANCE)] string stateName,
            string newValue)
        {
            MemberInfo[] members = typeof(Duck).GetMembers(ALL_INSTANCE);

            foreach (MemberInfo member in members)
            {
                if (member.MemberType is not MemberTypes.Property and not MemberTypes.Field)
                    continue;
                
                if (!member.Name.CaselessEquals(stateName))
                    continue;

                // found
                
                FieldOrPropertyInfo state = new(member);
                
                Type t = state.FieldOrPropertyType;

                if (!Commands.console.Shell.TypeInterpreterModules.TryFirst(x => t.InheritsFrom(x.ParsingType), out ITypeInterpreter parser))
                    throw new Exception($"No parsing module found for type: {t.Name}");

                ValueOrException<object> parseResult = parser.ParseString(newValue, t, new(Commands.console.Shell, null));

                object oldValue = state.GetValue(duck);
                state.SetValue(duck, parseResult.Unpack());

                string oldSerialized = FireSerializer.Serialize(oldValue);
                string newSerialized = FireSerializer.Serialize(parseResult.Value);
                
                DevConsole.Log($"{state.Name}: [{oldSerialized}] -> [{newSerialized}]");

                return;
            }
            
            throw new Exception("State not found: " + stateName);
        }
    }
}
using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
using System;
using System.Reflection;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Get the current value of a state of the duck")]
        public static string GetF(
            Duck duck,
            [ReflectionAutoCompl(typeof(Duck), MemberTypes.Property | MemberTypes.Field , ALL_INSTANCE)] string stateName)
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
                object value = state.GetValue(duck);

                string serialized = FireSerializer.Serialize(value);
                return serialized;
            }

            throw new Exception("State not found: " + stateName);
        }
    }
}
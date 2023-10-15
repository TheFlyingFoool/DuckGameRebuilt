using AddedContent.Firebreak.DuckShell.Implementation;
using DuckGame.ConsoleEngine;
using System;
using System.Reflection;
using System.Text;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        internal class DSHCommandAttribute : MarkerAttribute
        {
            public Command Command = null!;
            public string Name { get; set; } = null!;
            public string? Description { get; set; } = null;
            public bool Hidden { get; set; } = false;

            public DSHCommandAttribute(string name) : this()
            {
                Name = name;
            }

            public DSHCommandAttribute()
            {
            
            }
            
            protected override void Implement()
            {
                DevConsoleDSHWrapper.AttributeCommandInfos.Add((MethodInfo) Member);
            }

            public StringBuilder GetFunctionSignature()
            {
                StringBuilder builder = new();
        
                builder.Append(Name);
                builder.Append('(');

                Command.Parameter[] parameterInfos = Command.Parameters;
                for (var i = 0; i < parameterInfos.Length; i++)
                {
                    Command.Parameter pInfo = parameterInfos[i];

                    Type pType = pInfo.ParameterType;
                    string pName = pInfo.Name;
                    bool isOptional = pInfo.IsOptional;

                    if (i > 0)
                        builder.Append(", ");

                    if (isOptional)
                        builder.Append('[');

                    builder.Append(pType.Name);
                    builder.Append(' ');
                    builder.Append(pName);

                    if (isOptional)
                        builder.Append(']');
                }

                builder.Append(')');

                return builder;
            }
        }
    }
}
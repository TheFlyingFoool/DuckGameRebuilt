using System;
using System.Text;

namespace DuckGame.ConsoleEngine
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DSHCommand : Attribute
    {
        public Command Command = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool Hidden { get; set; } = false;

        public DSHCommand(string name)
        {
            Name = name;
        }

        public DSHCommand() { }

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
using System;
using System.Reflection;

namespace DuckGame.ConsoleEngine
{
    public class ShellCommand
    {
        public Func<object[], object> Invoke;
        public string Name;
        public Parameter[] Parameters;

        public ShellCommand(string name, Parameter[] parameters, Func<object?[]?, object?> invoke)
        {
            Name = name;
            Parameters = parameters;
            Invoke = invoke;
        }

        /// <param name="methodInfo" />
        public static ShellCommand FromMethodInfo(MethodInfo methodInfo)
        {
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            Parameter[] parameters = new Parameter[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo pInfo = parameterInfos[i];
            
                parameters[i] = new Parameter()
                {
                    Name = pInfo.Name ?? "<NULL>",
                    ParameterType = pInfo.ParameterType,
                    IsOptional = pInfo.IsOptional,
                    DefaultValue = pInfo.DefaultValue,
                    IsParams = pInfo.IsDefined(typeof(ParamArrayAttribute), false),
                    Autocompletion = pInfo.GetCustomAttribute<AutoCompl>() ?? new AutoCompl(pInfo.ParameterType)
                };
            }

            return new ShellCommand(methodInfo.Name, parameters, args => methodInfo.Invoke(null, args));
        }

        public class Parameter
        {
            public string Name;
            public Type ParameterType;
            public bool IsOptional;
            public object? DefaultValue;
            public bool IsParams;
            public AutoCompl Autocompletion = new();
        }
    }
}
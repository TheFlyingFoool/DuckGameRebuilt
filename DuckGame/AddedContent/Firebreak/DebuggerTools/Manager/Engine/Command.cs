using System;
using System.Reflection;

namespace DuckGame.ConsoleEngine
{
    public class Command
    {
        public Func<object[], object> Invoke;
        public string Name;
        public Parameter[] Parameters;

        public Command(string name, Parameter[] parameters, Func<object?[]?, object?> invoke)
        {
            Name = name;
            Parameters = parameters;
            Invoke = invoke;
        }

        /// <param name="methodInfo" />
        /// <param name="fast">Sacrifice debug-ability for performance</param>
        public static Command FromMethodInfo(MethodInfo methodInfo, bool fast)
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
                    Autocompletion = pInfo.GetCustomAttribute<AutocompleteAttribute>()
                };
            }

            Func<object?[]?, object?> action = fast
                ? (Func<object?[]?, object?>) Delegate.CreateDelegate(typeof(Func<object?[]?, object?>), methodInfo) 
                : args => methodInfo.Invoke(null, args);
        
            return new Command(methodInfo.Name, parameters, action);
        }

        public class Parameter
        {
            public string Name;
            public Type ParameterType;
            public bool IsOptional = false;
            public object? DefaultValue = null;
            public bool IsParams = false;
            public AutocompleteAttribute? Autocompletion = null;
        }
    }
}
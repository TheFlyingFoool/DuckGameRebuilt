#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    /// <summary>
    ///     Marks the method using the attribute as a console command.
    ///     This will initialize it alongside the default commands, and can
    ///     then be used in the Dev Console.
    ///     <br /> <br />
    ///     Allowed argument types:<br />
    ///     <list type="bullet">
    ///         <item> <see cref="int"/> (int)</item>
    ///         <item> <see cref="float"/> (float)</item>
    ///         <item> <see cref="bool"/> (bool)</item>
    ///         <item> <see cref="string"/> (string)</item>
    ///         <item> <see cref="Thing"/></item>
    ///         <item> <see cref="Level"/></item>
    ///         <item> <see cref="Layer"/></item>
    ///     </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.ReturnValue, Inherited = false)]
    internal class DevConsoleCommandAttribute : Attribute
    {
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public bool IsCheat { get; set; }
        public bool CanCrash { get; set; }
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public bool HostOnly { get; set; }

        static DevConsoleCommandAttribute()
        {
        }
        private static string getRealName(MethodInfo methodInfo, DevConsoleCommandAttribute attribute)
        {
            return (attribute.Name ?? methodInfo.Name).ToLower().Replace(" ", "");
        }
        public static void OnResults(Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> all)
        {
            foreach ((MemberInfo memberInfo, Attribute attr) in all[typeof(DevConsoleCommandAttribute)])
            {
                DevConsoleCommandAttribute attribute = (DevConsoleCommandAttribute) attr;
                MethodInfo method = (MethodInfo) memberInfo;
                
                ParameterInfo[] parameters = method.GetParameters();
                
                string realName = getRealName(method, attribute);

                // if (!parameters.Any())
                // {
                //     DevConsole.AddCommand(new CMD(realName, delegate () { method.Invoke(null, null); }) { cancrash = attribute.CanCrash});
                //     continue;
                // }

                int parameterLength = parameters.Length;
                CMD.Argument[] arguments = new CMD.Argument[parameterLength];
                for (int i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = ParameterInfoToCmdArgument(parameters[i], i == arguments.Length - 1);
                }

                DevConsole.AddCommand(new CMD(realName, arguments, cmd =>
                {
                    if (attribute.HostOnly && !Network.isServer)
                    {
                        DevConsole.Log("You have to be the host!", Color.Red);
                        return;
                    }
                    
                    if (attribute.IsCheat && DevConsole.CheckCheats())
                    {
                        DevConsole.Log("You can't do that here!", Color.Red);
                        return;
                    }
                    
                    object[] objectParameters = new object[arguments.Length];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        object argVal = cmd.Arg<object>(parameters[i].Name);
                        object val = cmd.arguments[i].optional && argVal is null
                            ? parameters[i].DefaultValue
                            : argVal;
                        
                        objectParameters[i] = val;
                    }

                    try
                    {
                        // invokes the method. if it returns a value, logs it
                        if (method.Invoke(null, objectParameters) is { } result)
                        {
                            DevConsole.LogComplexMessage(result switch
                            {
                                IEnumerable ie and not string => ie.Cast<object>().ToReadableString(),
                                _ => result.ToString()
                            }, Color.White);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e.InnerException ?? e;
                    }
                })
                {
                    cancrash = attribute.CanCrash,
                    description = attribute.Description ?? "",
                    cheat = attribute.IsCheat,
                    aliases = attribute.Aliases.ToList()
                });
            }
        }

        private static CMD.Argument ParameterInfoToCmdArgument(ParameterInfo parameter, bool isLast)
        {
            CMD.Argument arg;
            Type type = parameter.ParameterType;
            string name = parameter.Name;
            bool optional = parameter.IsOptional;
            object? defaultValue = parameter.DefaultValue;

            arg = CMD.GetArgument(type, name, optional, isLast);

            if (optional)
                arg.value = defaultValue;

            return arg;
        }
    }
}
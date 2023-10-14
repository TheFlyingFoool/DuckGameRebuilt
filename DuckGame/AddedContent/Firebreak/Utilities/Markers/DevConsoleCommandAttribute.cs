using DuckGame;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
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
        internal class DevConsoleCommandAttribute : MarkerAttribute
        {
            public string? Name { get; set; } = null;
            public string? Description { get; set; } = null;
            public bool IsCheat { get; set; }
            public bool CanCrash { get; set; }
            public string[] Aliases { get; set; } = Array.Empty<string>();
            public bool HostOnly { get; set; }
            public bool DebugOnly { get; set; }

            protected override void Implement()
            {
                if (DebugOnly && !Program.IS_DEV_BUILD)
                    return;
                
                MethodInfo method = (MethodInfo)Member;

                ParameterInfo[] parameters = method.GetParameters();

                string realName = (Name ?? method.Name).ToLower().Replace(" ", "");

                int parameterLength = parameters.Length;
                CMD.Argument[] arguments = new CMD.Argument[parameterLength];
                for (int i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = ParameterInfoToCmdArgument(parameters[i], i == arguments.Length - 1);
                }

                DevConsole.AddCommand(new CMD(realName, arguments, cmd =>
                {
                    if (HostOnly && !Network.isServer)
                    {
                        DevConsole.Log("You have to be the host!", Color.Red);
                        return;
                    }

                    if (IsCheat && DevConsole.CheckCheats())
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
                    cancrash = CanCrash,
                    description = Description ?? "",
                    cheat = IsCheat,
                    aliases = Aliases.ToList()
                });
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
}
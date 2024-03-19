using AddedContent.Firebreak.DuckShell.Implementation;
using DuckGame;
using DuckGame.ConsoleEngine;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace AddedContent.Firebreak
{
    public static partial class Marker
    {
        /// <summary>
        ///     Marks the method using the attribute as a console command.
        ///     This will initialize it alongside the default commands, and can
        ///     then be used in the DevConsole or DuckShell.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.ReturnValue, Inherited = false)]
        public class DevConsoleCommandAttribute : MarkerAttribute
        {
            public string Name { get; set; } = null;
            public string? Description { get; set; } = null;
            public bool IsCheat { get; set; }
            public bool CanCrash { get; set; }
            public string[] Aliases { get; set; } = Array.Empty<string>();
            public bool HostOnly { get; set; }
            public bool DebugOnly { get; set; }
            public ImplementTo To { get; set; } = ImplementTo.Both;
            public ShellCommand Command { get; set; }

            protected override void Implement()
            {
                if (DebugOnly && !Program.IS_DEV_BUILD)
                    return;
                
                MethodInfo method = (MethodInfo)Member;
                    
                Command = ShellCommand.FromMethodInfo(method);
                Name ??= method.Name;

                ParameterInfo[] parameters = method.GetParameters();

                string idName = Name.ToLower().Replace(" ", "");
                
                if (To is ImplementTo.Both or ImplementTo.DuckShell)
                {
                    DevConsoleDSHWrapper.AttributeCommands.Add(this);
                }
                if (To is ImplementTo.Both or ImplementTo.DuckHack)
                {
                    int parameterLength = parameters.Length;
                    CMD.Argument[] arguments = new CMD.Argument[parameterLength];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        arguments[i] = ParameterInfoToCmdArgument(parameters[i], i == arguments.Length - 1);
                    }
                    
                    DevConsole.AddCommand(new CMD(idName, arguments, cmd =>
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
            }

            private static CMD.Argument ParameterInfoToCmdArgument(ParameterInfo parameter, bool isLast)
            {
                CMD.Argument arg;
                Type type = parameter.ParameterType;
                string name = parameter.Name;
                bool optional = parameter.IsOptional;
                object? defaultValue = parameter.DefaultValue;
                bool isParams = parameter.IsDefined(typeof(ParamArrayAttribute), false);

                arg = CMD.GetArgument(type, name, optional || isParams, isLast);

                if (optional)
                    arg.defaultValue = defaultValue;
                else if (isParams)
                    arg.defaultValue = Array.CreateInstance(type.GetElementType()!, 0);

                return arg;
            }
        }
    }
    
    public enum ImplementTo
    {
        Both,
        DuckShell,
        DuckHack,
    }
}
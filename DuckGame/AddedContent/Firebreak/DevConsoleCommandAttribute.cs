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
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DevConsoleCommandAttribute : Attribute
    {
        public MethodInfo method;
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public bool IsCheat { get; set; }
        public string[] Aliases { get; set; } = Array.Empty<string>();

        static DevConsoleCommandAttribute()
        {
        }
        private static string getRealName(MethodInfo methodInfo, DevConsoleCommandAttribute attribute)
        {
            return (attribute.Name ?? methodInfo.Name).ToLower().Replace(" ", "");
        }
        public static void OnResults(Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> all)
        {
            foreach ((MemberInfo MemberInfo, Attribute vAttribute) in all[typeof(DevConsoleCommandAttribute)])
            {
                DevConsoleCommandAttribute Module = vAttribute as DevConsoleCommandAttribute;
                Module.method = MemberInfo as MethodInfo;
                ParameterInfo[] parameters = Module.method.GetParameters();
                string realName = getRealName(Module.method, Module);

                if (!parameters.Any())
                {
                    DevConsole.AddCommand(new CMD(realName, delegate () { Module.method.Invoke(null, null); }));
                    continue;
                }

                CMD.Argument[] arguments = new CMD.Argument[parameters.Length];
                for (int i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = ParameterInfoToCmdArgument(parameters[i], i == arguments.Length - 1);
                }

                DevConsole.AddCommand(new CMD(realName, arguments, cmd =>
                {
                    // gets the parameters by fetching them through the command
                    // using the previously defined variable names, which are
                    // also the parameter names
                    object[] objectParameters = new object[arguments.Length];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        objectParameters[i] = cmd.Arg<object>(parameters[i].Name);
                    }

                    try
                    {
                        // invokes the method. if it returns a value, logs it
                        if (Module.method.Invoke(null, objectParameters) is { } result)
                            DevConsole.LogComplexMessage(result switch
                            {
                                IEnumerable ie and not string => ie.Cast<object>().ToReadableString(),
                                _ => result.ToString()
                            }, Color.White);
                    }
                    catch (Exception e)
                    {
                        // using this try catch i can get the inner exception
                        // and log that instead of logging an ambigious message
                        // telling me that the target of invocation threw an error
                        throw e.InnerException ?? e;
                    }
                })
                {
                    description = Module.Description ?? "",
                    cheat = Module.IsCheat,
                    aliases = Module.Aliases.ToList()
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

            if (type == typeof(int))
                arg = new CMD.Integer(name, optional);
            else if (type == typeof(float))
                arg = new CMD.Float(name, optional);
            else if (type == typeof(bool))
                arg = new CMD.Boolean(name, optional);
            else if (type == typeof(Vec2))
                arg = new CMD.Vec2(name, optional);
            else if (type == typeof(string))
                arg = new CMD.String(name, optional) { takesMultispaceString = isLast };
            else if (type == typeof(Duck))
                arg = new CMD.Duck(name, optional);
            else if (type == typeof(Profile))
                arg = new CMD.Profile(name, optional);
            else if (typeof(Thing).IsAssignableFrom(type))
                arg = new CMD.Thing<Thing>(name, optional);
            else if (typeof(Level).IsAssignableFrom(type))
                arg = new CMD.Level(name, optional);
            else if (typeof(Layer).IsAssignableFrom(type))
                arg = new CMD.Layer(name, optional);
            else if (typeof(Enum).IsAssignableFrom(type))
                arg = new CMD.Enum(name, type, optional);
            else
                throw new Exception($"Parameter type of [{type.FullName}] is not supported");

            if (optional)
                arg.value = defaultValue;

            return arg;
        }
    }
}
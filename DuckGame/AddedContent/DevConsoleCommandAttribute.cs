#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DuckGame;

namespace DuckGame;

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
    public string? Name { get; set; } = null;
    public string? Description { get; set; } = null;
    public bool IsCheat { get; set; }
    public string[] Aliases { get; set; } = Array.Empty<string>();

    public static void Initialize()
    {
        var methodsUsing = CollectAllMethodsUsingThisAttribute();

        foreach ((MethodInfo method, DevConsoleCommandAttribute attribute) in methodsUsing)
        {
            ParameterInfo[] parameters = method.GetParameters();
            string realName = getRealName(method, attribute);

            if (!parameters.Any())
            {
                DevConsole.AddCommand(new CMD(realName, (Action) Delegate.CreateDelegate(typeof(Action), method)));
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
                
                // invokes the method. if it returns a value, logs it
                if (method.Invoke(null, objectParameters) is { } result)
                    DevConsole.Log(result);
            })
            {
                description = attribute.Description ?? "",
                cheat = attribute.IsCheat
            });
        }

        string getRealName(MethodInfo methodInfo, DevConsoleCommandAttribute attribute)
        {
            return (attribute.Name ?? methodInfo.Name).ToLower().Replace(" ", "");
        }
    }

    private static CMD.Argument ParameterInfoToCmdArgument(ParameterInfo parameter, bool isLast)
    {
        CMD.Argument arg;
        var type = parameter.ParameterType;
        string name = parameter.Name;
        bool optional = parameter.IsOptional;
        object? defaultValue = parameter.DefaultValue;
                
        if (type == typeof(int))
            arg = new CMD.Integer(name, optional);
        else if (type == typeof(float))
            arg = new CMD.Float(name, optional);
        else if (type == typeof(bool))
            arg = new CMD.Boolean(name, optional);
        else if (type == typeof(string))
            arg = new CMD.String(name, optional) { takesMultispaceString = isLast };
        else if (typeof(Thing).IsAssignableFrom(type))
            arg = new CMD.Thing<Thing>(name, optional);
        else if (typeof(Level).IsAssignableFrom(type))
            arg = new CMD.Level(name, optional);
        else if (typeof(Layer).IsAssignableFrom(type))
            arg = new CMD.Layer(name, optional);
        else 
            throw new Exception($"Parameter type of [{type.FullName}] is not supported");

        if (optional)
            arg.value = defaultValue;

        return arg;
    }

    private static List<(MethodInfo Method, DevConsoleCommandAttribute Attribute)> CollectAllMethodsUsingThisAttribute()
    {
        return Helper.GetAllMembersWithAttribute<MethodInfo, DevConsoleCommandAttribute>()
            .Select(x => (x.Member, x.Attributes.First()))
            .ToList();
    }
}
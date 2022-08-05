#nullable enable
using System;
using System.Collections.Generic;
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
///         <item> <see cref="Level"/> </item>
///         <item> <see cref="float"/> </item>
///         <item> <see cref="int"/> </item>
///         <item> <see cref="string"/> </item>
///         <item> <see cref="bool"/> </item>
///         <item> <see cref="CMD.Layer"/> </item>
///         <item> <see cref="Thing"/> </item>
///         <item> <see cref="CMD.Font"/> </item>
///     </list>
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class DevConsoleCommandAttribute : Attribute
{
    public string? Name { get; set; } = null;
    public string[] Aliases { get; set; } = Array.Empty<string>();

    public static void Initialize()
    {
        var methodsUsing = CollectAllMethodsUsingThisAttribute();

        foreach (var (method, attribute) in methodsUsing)
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
                arguments[i] = null;
            }

            DevConsole.AddCommand(new CMD(realName, arguments, _ => { })); // TODO make this actually function
        }

        string getRealName(MethodInfo methodInfo, DevConsoleCommandAttribute attribute)
        {
            return (attribute.Name ?? methodInfo.Name).ToLower().Replace(" ", "");
        }
    }

    private static List<(MethodInfo Method, DevConsoleCommandAttribute Attribute)> CollectAllMethodsUsingThisAttribute()
    {
        return Helper.GetAllMembersWithAttribute<MethodInfo, DevConsoleCommandAttribute>()
            .Select(x => (x.Member, x.Attributes.First()))
            .ToList();
    }
}
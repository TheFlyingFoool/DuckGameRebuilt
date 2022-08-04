using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    /// <summary>
    /// Marks the method using the attribute as a console command.
    /// This will initialize it alongside the default commands, and can
    /// then be used in the Dev Console.
    /// <br /> <br />
    /// Allowed argument types:<br />
    /// <list type="bullet">
    /// <item><see cref="Level"/></item>
    /// <item><see cref="float"/></item>
    /// <item><see cref="int"/></item>
    /// <item><see cref="string"/></item>
    /// <item><see cref="bool"/></item>
    /// <item><see cref="CMD.Layer"/></item>
    /// <item><see cref="Thing"/></item>
    /// <item><see cref="CMD.Font"/></item>
    /// </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ConsoleCommandAttribute : Attribute
    {
        public ConsoleCommandAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public static void Initialize()
        {
            
        }
    }
    
    // // TODO move to it's own class/file
    // public static IEnumerable<(TInfoType Member, IEnumerable<TAttribute> Attributes)>
    //     GetAllMembersWithAttribute<TInfoType, TAttribute>() where TInfoType  : MemberInfo 
    //     where TAttribute : Attribute
    // {
    //     MemberTypes memberType = MemberTypes.All;
    //
    //     if (typeof(TInfoType).IsAssignableTo(typeof(FieldInfo)))
    //         memberType = MemberTypes.Field;
    //     else if (typeof(TInfoType).IsAssignableTo(typeof(MethodInfo)))
    //         memberType = MemberTypes.Method;
    //     else if (typeof(TInfoType).IsAssignableTo(typeof(PropertyInfo)))
    //         memberType = MemberTypes.Property;
    //     else if (typeof(TInfoType).IsAssignableTo(typeof(ConstructorInfo)))
    //         memberType = MemberTypes.Constructor;
    //     
    //     return GetAllMembersWithAttribute<TAttribute>(memberType)
    //         .Select<(MemberInfo Member, IEnumerable<TAttribute> Attributes), (TInfoType, IEnumerable<TAttribute>)>
    //             (x => ((TInfoType) x.Member, x.Attributes));
    // }
    //
    // public static IEnumerable<(MemberInfo Member, IEnumerable<TAttribute> Attributes)>
    //     GetAllMembersWithAttribute<TAttribute>(MemberTypes filter = MemberTypes.All, Type? inType = null) where TAttribute : Attribute
    // {
    //     if (inType is { })
    //         return inType.GetMembers(BindingFlags.Public | BindingFlags.Static)
    //             .Where(x => x.GetCustomAttributes<TAttribute>(false).Any()
    //                         && x.MemberType.HasFlag(filter))
    //             .Select(x => (x, x.GetCustomAttributes<TAttribute>(false)));
    //     
    //     return Assembly.GetExecutingAssembly()
    //         .GetTypes()
    //         .SelectMany(x => x.GetMembers(BindingFlags.Public | BindingFlags.Static))
    //         .Where(x => x.GetCustomAttributes<TAttribute>(false).Any()
    //                     && x.MemberType.HasFlag(filter))
    //         .Select(x => (x, x.GetCustomAttributes<TAttribute>(false)));
    // }
}
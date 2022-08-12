using System;
using System.Linq;
using System.Reflection;

namespace DuckGame;

[AttributeUsage(AttributeTargets.Class)]
public class FireSerializerModuleAttribute : Attribute
{
    public static readonly IFireSerializerModule[] Serializers
        = MemberAttributePair<TypeInfo, FireSerializerModuleAttribute>.GetAll()
            .Where(x =>
            {
                if (x.MemberInfo.AsType().GetInterfaces().Any(x => x.Name == $"{nameof(IFireSerializerModule<object>)}`1"))
                    return true;

                throw new Exception($"{x.MemberInfo.Name} is using the {nameof(FireSerializerModuleAttribute)} attribute" +
                                    $"without implementing the {nameof(IFireSerializerModule<object>)} interface");
            })
            .Select(x => (IFireSerializerModule) Activator.CreateInstance(x.MemberInfo.AsType()))
            .ToArray();
}
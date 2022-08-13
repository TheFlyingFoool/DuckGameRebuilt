using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame;

[AttributeUsage(AttributeTargets.Class)]
public class FireSerializerModuleAttribute : Attribute
{
    public static List<IFireSerializerModule> Serializers;

    static FireSerializerModuleAttribute()
    {
        MemberAttributePair<TypeInfo, FireSerializerModuleAttribute>.RequestSearch(all =>
        {
            List<IFireSerializerModule> serializerModules = new();

            foreach (var (memberInfo, _) in all)
            {
                Type type = memberInfo.AsType();

                if (type.GetInterfaces().All(x => x.Name != $"{nameof(IFireSerializerModule<object>)}`1"))
                    throw new Exception($"{memberInfo.Name} is using the {nameof(FireSerializerModuleAttribute)} attribute" +
                                        $"without implementing the {nameof(IFireSerializerModule<object>)} interface");
                
                serializerModules.Add((IFireSerializerModule) Activator.CreateInstance(type));
            }

            Serializers = serializerModules;
        });
    }
}
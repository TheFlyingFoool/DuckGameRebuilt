using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FireSerializerModuleAttribute : Attribute
    {
        public static List<IFireSerializerModule> Serializers;
        public TypeInfo memberInfo;
        static FireSerializerModuleAttribute()
        {

        }
        public static void OnResults(Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> all)
        {
            List<IFireSerializerModule> serializerModules = new();
            foreach ((MemberInfo MemberInfo, Attribute vAttribute) in all[typeof(FireSerializerModuleAttribute)])
            {
                FireSerializerModuleAttribute FireSerializerModule = vAttribute as FireSerializerModuleAttribute;
                FireSerializerModule.memberInfo = (TypeInfo)(object)MemberInfo;
                Type type = FireSerializerModule.memberInfo;

                if (type.GetInterfaces().All(x => x.Name != $"{nameof(IFireSerializerModule<object>)}`1"))
                    throw new Exception($"{FireSerializerModule.memberInfo.Name} is using the {nameof(FireSerializerModuleAttribute)} attribute" + $"without implementing the {nameof(IFireSerializerModule<object>)} interface");

                serializerModules.Add((IFireSerializerModule)Activator.CreateInstance(type));
            }
            Serializers = serializerModules;
        }
    }
}
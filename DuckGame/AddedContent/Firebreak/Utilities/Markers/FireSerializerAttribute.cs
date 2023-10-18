using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
    {
        [AttributeUsage(AttributeTargets.Class)]
        internal class FireSerializerAttribute : MarkerAttribute
        {
            public static List<IFireSerializerModule> Serializers = new();

            protected override void Implement()
            {
                Type type = (TypeInfo) Member;
                EnsureInterfaceImplementation(type);
                
                IFireSerializerModule instance = (IFireSerializerModule) Activator.CreateInstance(type);
                Serializers.Add(instance);
            }

            private static void EnsureInterfaceImplementation(Type type)
            {
                if (type.GetInterfaces().All(x => x.Name != $"{nameof(IFireSerializerModule<object>)}`1"))
                {
                    throw new Exception($"{type.Name} is using the {nameof(FireSerializerAttribute)} attribute" +
                                        $"without implementing the {nameof(IFireSerializerModule<object>)} interface");
                }
            }
        }
    }
}
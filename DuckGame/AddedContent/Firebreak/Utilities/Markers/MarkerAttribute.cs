using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AddedContent.Firebreak
{
    public abstract class MarkerAttribute : Attribute
    {
        public static Dictionary<Type, List<MarkerAttribute>> AllMarked = new();

        public MemberInfo Member;

        public static void Initialize()
        {
            Type[] registeredMarkers = typeof(Marker).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            List<MemberInfo> memberInfos = new();
            
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                MemberInfo[] typeMembers = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        
                memberInfos.Add(type);
                memberInfos.AddRange(typeMembers.Where(x => x is not TypeInfo));
            }

            for (int i = 0; i < memberInfos.Count; i++)
            {
                MemberInfo member = memberInfos[i];
                Attribute[] memberAttributes = GetCustomAttributes(member, false);

                for (int j = 0; j < memberAttributes.Length; j++)
                {
                    Attribute attribute = memberAttributes[j];
                    Type attributeType = attribute.GetType();
                    
                    if (attribute is not MarkerAttribute)
                        continue;

                    MarkerAttribute marker = (MarkerAttribute) attribute;
                        
                    for (int k = 0; k < registeredMarkers.Length; k++)
                    {
                        if (registeredMarkers[k] != attributeType)
                            continue;

                        marker.Member = member;
                        
                        AllMarked.TryAdd(attributeType, new List<MarkerAttribute>());
                        AllMarked[attributeType].Add(marker);
                        
                        marker.Implement();
                        break;
                    }
                }
            }
        }

        protected abstract void Implement();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DuckGame;

public static class MemberAttributePairHandler
{
    public static Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> LookupTable = new();
    
    /// <summary>This is a list of attribute types btw</summary>
    public static List<Type> AttributeLookupRequests = new();
    
    public static event Action<Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>>> GlobalOnSearchComplete = default;

    public static void Init()
    {
        List<Type> TargetCustomAttributes = new List<Type>()
        {
            typeof(AutoConfigFieldAttribute),
            typeof(FireSerializerModuleAttribute),
            typeof(DevConsoleCommandAttribute),
            typeof(DrawingContextAttribute),
            typeof(PostInitializeAttribute)
        };
        foreach(Type CustomAttribute in TargetCustomAttributes)
        {
            RuntimeHelpers.RunClassConstructor(CustomAttribute.TypeHandle);
        }
        var types = Assembly.GetExecutingAssembly().GetTypes();
        
        List<MemberInfo> memberInfos = new();
        for (int i = 0; i < types.Length; i++)
        {
            Type type = types[i];
            
            memberInfos.Add(type);
            memberInfos.AddRange(type.GetMembers());
        }
        foreach (var memberInfo2 in memberInfos)
        {
            // dont worry about the casting, it was casted to IEnumerable<Attribute>
            // from a Attribute[] in the first place, so im just utilizing that
           // var attributes = (Attribute[]) memberInfo.GetCustomAttributes();
            if (memberInfo2.CustomAttributes.Count() == 0)
                continue;
            //  if (attributes.Length == 0)
            //     continue;
            bool breakall = false;
            foreach(CustomAttributeData CustomAttribute in memberInfo2.CustomAttributes)
            {
                for (int i = 0; i < TargetCustomAttributes.Count; i++)
                {
                    Type TargetCustomAttribute = TargetCustomAttributes[i];
                    if (TargetCustomAttribute == CustomAttribute.AttributeType)
                    {
                        Attribute attribute = Attribute.GetCustomAttribute(memberInfo2, TargetCustomAttribute);
                        LookupTable.TryUse(TargetCustomAttribute, new List<(MemberInfo, Attribute)>(), relatedList =>
                        {
                            relatedList.Add((memberInfo2, attribute));
                        });
                        breakall = true;
                        break;
                    }
                }
                if (breakall)
                {
                    break;
                }
            }
            //foreach(Type CustomAttribute in CustomAttributes)
            //{
            //    if (memberInfo2.IsDefined(CustomAttribute))
            //    {
            //        Attribute attribute = Attribute.GetCustomAttribute(memberInfo2, CustomAttribute);
            //        LookupTable.TryUse(attribute.GetType(), new List<(MemberInfo, Attribute)>(), relatedList =>
            //        {
            //            relatedList.Add((memberInfo2, attribute));
            //        });
            //        //relatedList.Add((memberInfo, attribute));
            //        break;
            //    }
            //}
            // if this MemberInfo contains an attribute that is requested, use it
            //if (attributes.TryFirst(attr => AttributeLookupRequests.Contains(attr.GetType()), out var attribute))
            //{
            //    // if the lookup table contains the wanted attribute KeyValue pair, add a
            //    // new (MemberInfo, Attribute) tuple to it, otherwise add the key and do the same
            //    LookupTable.TryUse(attribute.GetType(), new List<(MemberInfo, Attribute)>(), relatedList =>
            //    {
            //        relatedList.Add((memberInfo, attribute));
            //    });
            //}
        }

        GlobalOnSearchComplete.Invoke(LookupTable);
    }
}
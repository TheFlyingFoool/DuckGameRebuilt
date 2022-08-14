using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame;

public static class MemberAttributePairHandler
{
    public static Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> LookupTable = new();
    
    /// <summary>This is a list of attribute types btw</summary>
    public static List<Type> AttributeLookupRequests = new();
    
    public static event Action<Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>>> GlobalOnSearchComplete = default;

    public static void Init()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        
        List<MemberInfo> memberInfos = new();
        for (int i = 0; i < types.Length; i++)
        {
            Type type = types[i];
            
            memberInfos.Add(type);
            memberInfos.AddRange(type.GetMembers());
        }
        
        foreach (var memberInfo in memberInfos)
        {
            // dont worry about the casting, it was casted to IEnumerable<Attribute>
            // from a Attribute[] in the first place, so im just utilizing that
            var attributes = (Attribute[]) memberInfo.GetCustomAttributes();

            if (attributes.Length == 0)
                continue;

            // if this MemberInfo contains an attribute that is requested, use it
            if (attributes.TryFirst(attr => AttributeLookupRequests.Contains(attr.GetType()), out var attribute))
            {
                // if the lookup table contains the wanted attribute KeyValue pair, add a
                // new (MemberInfo, Attribute) tuple to it, otherwise add the key and do the same
                LookupTable.TryUse(attribute.GetType(), new List<(MemberInfo, Attribute)>(), relatedList =>
                {
                    relatedList.Add((memberInfo, attribute));
                });
            }
        }

        GlobalOnSearchComplete.Invoke(LookupTable);
    }
}
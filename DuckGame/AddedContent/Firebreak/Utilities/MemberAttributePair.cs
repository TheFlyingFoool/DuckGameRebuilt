using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame;

public record MemberAttributePair<TMemberInfo, TAttribute>(TMemberInfo MemberInfo, TAttribute Attribute)
    where TMemberInfo : MemberInfo where TAttribute : Attribute
{
    public TMemberInfo MemberInfo = MemberInfo;
    public TAttribute Attribute = Attribute;

    public static void RequestSearch(Action<List<MemberAttributePair<TMemberInfo, TAttribute>>>? onSearchComplete = null)
    {
        var attributeType = typeof(TAttribute);
        if (!MemberAttributePairHandler.AttributeLookupRequests.Contains(attributeType))
            MemberAttributePairHandler.AttributeLookupRequests.Add(attributeType);

        MemberAttributePairHandler.OnSearchComplete += dic =>
        {
            List<MemberAttributePair<TMemberInfo, TAttribute>> pairsUsing = new();
            
            foreach (var (memberInfo, attribute) in dic[typeof(TAttribute)])
            {
                pairsUsing.Add(new ( (TMemberInfo) memberInfo, (TAttribute) attribute) );
            }
            
            OnSearchComplete.Invoke(pairsUsing);
        };

        if (onSearchComplete is not null)
            OnSearchComplete += onSearchComplete;
    }

    public static event Action<List<MemberAttributePair<TMemberInfo, TAttribute>>> OnSearchComplete = default;

    public void Deconstruct(out TMemberInfo memberInfo, out TAttribute attribute)
    {
        memberInfo = MemberInfo;
        attribute = Attribute;
    }
}
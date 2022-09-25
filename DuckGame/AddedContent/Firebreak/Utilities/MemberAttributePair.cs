using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuckGame
{
    public class MemberAttributePair<TMemberInfo , TAttribute>
    {
        public MemberAttributePair(TMemberInfo  pMemberInfo, TAttribute pAttribute) // public static void TryUse<TMemberInfo , TAttribute>(this Dictionary<TMemberInfo , TAttribute> dic, TMemberInfo  requestedKey, TAttribute defaultValue, Action<TAttribute> action)
        {
            MemberInfo = pMemberInfo;
            Attribute = pAttribute;
        }
        public TMemberInfo  MemberInfo;// = MemberInfo;
        public TAttribute Attribute;// = Attribute;
        public static void RequestSearch(Action<List<MemberAttributePair<TMemberInfo , TAttribute>>>? onSearchComplete = null)
        {
            Type attributeType = typeof(TAttribute);
            if (!MemberAttributePairHandler.AttributeLookupRequests.Contains(attributeType))
                MemberAttributePairHandler.AttributeLookupRequests.Add(attributeType);

            MemberAttributePairHandler.GlobalOnSearchComplete += dic =>
            {
                List<MemberAttributePair<TMemberInfo , TAttribute>> pairsUsing = new();

                foreach ((MemberInfo memberInfo, Attribute attribute) in dic[typeof(TAttribute)])
                {
                    pairsUsing.Add(new((TMemberInfo )((object)memberInfo), (TAttribute)((object)attribute)));
                }

                OnSearchComplete.Invoke(pairsUsing);
            };

            if (onSearchComplete is not null)
                OnSearchComplete += onSearchComplete;
        }

        public static event Action<List<MemberAttributePair<TMemberInfo , TAttribute>>> OnSearchComplete = default;

        public void Deconstruct(out TMemberInfo  memberInfo, out TAttribute attribute)
        {
            memberInfo = MemberInfo;
            attribute = Attribute;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public class MemberAttributePair<T1, T2>
    {
        public MemberAttributePair(T1 pMemberInfo, T2 pAttribute) // public static void TryUse<T1, T2>(this Dictionary<T1, T2> dic, T1 requestedKey, T2 defaultValue, Action<T2> action)
        {
            MemberInfo = pMemberInfo;
            Attribute = pAttribute;
        }
        public T1 MemberInfo;// = MemberInfo;
        public T2 Attribute;// = Attribute;
        public static void RequestSearch(Action<List<MemberAttributePair<T1, T2>>>? onSearchComplete = null)
        {
            Type attributeType = typeof(T2);
            if (!MemberAttributePairHandler.AttributeLookupRequests.Contains(attributeType))
                MemberAttributePairHandler.AttributeLookupRequests.Add(attributeType);

            MemberAttributePairHandler.GlobalOnSearchComplete += dic =>
            {
                List<MemberAttributePair<T1, T2>> pairsUsing = new();

                foreach ((MemberInfo memberInfo, Attribute attribute) in dic[typeof(T2)])
                {
                    pairsUsing.Add(new((T1)((object)memberInfo), (T2)((object)attribute)));
                }

                OnSearchComplete.Invoke(pairsUsing);
            };

            if (onSearchComplete is not null)
                OnSearchComplete += onSearchComplete;
        }

        public static event Action<List<MemberAttributePair<T1, T2>>> OnSearchComplete = default;

        public void Deconstruct(out T1 memberInfo, out T2 attribute)
        {
            memberInfo = MemberInfo;
            attribute = Attribute;
        }
    }
}
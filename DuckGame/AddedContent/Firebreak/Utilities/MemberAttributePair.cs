using System;
using System.Linq;
using System.Reflection;

namespace DuckGame;

public record MemberAttributePair<TMemberInfo, TAttribute>(TMemberInfo MemberInfo, TAttribute Attribute)
    where TMemberInfo : MemberInfo where TAttribute : Attribute
{
    public TMemberInfo MemberInfo = MemberInfo;
    public TAttribute Attribute = Attribute;

    public static MemberAttributePair<TMemberInfo, TAttribute>[] GetAll()
    {
        return Extensions.GetAllMembersWithAttribute<TMemberInfo, TAttribute>()
            .Select(x => new MemberAttributePair<TMemberInfo, TAttribute>(x.Member, x.Attributes.First()))
            .ToArray();
    }
    
    public static MemberAttributePair<TMemberInfo, TAttribute>[] GetAllFromClass<T>()
    {
        return Extensions.GetAllMembersWithAttribute<TMemberInfo, TAttribute>(typeof(T))
            .Select(x => new MemberAttributePair<TMemberInfo, TAttribute>(x.Member, x.Attributes.First()))
            .ToArray();
    }

    public void Deconstruct(out TMemberInfo memberInfo, out TAttribute attribute)
    {
        memberInfo = MemberInfo;
        attribute = Attribute;
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuckGame.ConsoleEngine
{
    public class ReflectionAutoComplAttribute : AutoCompl
    {
        private readonly Type _searchClass;
        private readonly MemberTypes _memberType;
        private readonly BindingFlags _bindingFlags;

        private List<string> _cached = null;
        
        private const BindingFlags DEFAULT_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        public ReflectionAutoComplAttribute(Type searchClass, MemberTypes memberType, BindingFlags bindingFlags = DEFAULT_FLAGS)
        {
            _searchClass = searchClass;
            _memberType = memberType;
            _bindingFlags = bindingFlags;
        }

        public override IList<string> Get(string word)
        {
            if (_cached is not null)
                return _cached;

            _cached = new List<string>();

            MemberInfo[] allMembers = _searchClass.GetMembers(_bindingFlags);
            for (int i = 0; i < allMembers.Length; i++)
            {
                MemberInfo member = allMembers[i];
                
                if (_memberType.HasFlag(member.MemberType))
                    _cached.Add(member.Name);
            }

            return _cached;
        }
    }
}
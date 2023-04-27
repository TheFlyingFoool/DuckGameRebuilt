using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DuckGame
{

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
                typeof(AdvancedConfig),
                typeof(FireSerializerModuleAttribute),
                typeof(DevConsoleCommandAttribute),
                typeof(DrawingContextAttribute),
                typeof(PostInitializeAttribute)
            };
            foreach (Type CustomAttribute in TargetCustomAttributes)
            {
                RuntimeHelpers.RunClassConstructor(CustomAttribute.TypeHandle);
            }
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            List<MemberInfo> memberInfos = new();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                try
                {
                    MemberInfo[] members = type.GetMembers();
                    memberInfos.Add(type);
                    memberInfos.AddRange(members);
                }
                catch { } //GetMembers crashed for someone one time with a SEHException
            }
            foreach (MemberInfo memberInfo2 in memberInfos)
            {
                // dont worry about the casting, it was casted to IEnumerable<Attribute>
                // from a Attribute[] in the first place, so im just utilizing that
                // var attributes = (Attribute[]) memberInfo.GetCustomAttributes();
                if (memberInfo2.CustomAttributes.Count() == 0)
                    continue;
                //  if (attributes.Length == 0)
                //     continue;
                bool breakall = false;
                foreach (CustomAttributeData CustomAttribute in memberInfo2.CustomAttributes)
                {
                    for (int i = 0; i < TargetCustomAttributes.Count; i++)
                    {
                        Type TargetCustomAttribute = TargetCustomAttributes[i];
                        if (TargetCustomAttribute == CustomAttribute.AttributeType)
                        {
                            Attribute attribute = Attribute.GetCustomAttribute(memberInfo2, TargetCustomAttribute);
                            if (!LookupTable.ContainsKey(TargetCustomAttribute))
                                LookupTable.Add(TargetCustomAttribute, new List<(MemberInfo, Attribute)>());
                            LookupTable[TargetCustomAttribute].Add((memberInfo2, attribute));
                            breakall = true;
                            break;
                        }
                    }
                    if (breakall)
                    {
                        break;
                    }
                }
            }
            //GlobalOnSearchComplete.Invoke(LookupTable);
            AutoConfigFieldAttribute.OnResults(LookupTable);
            AdvancedConfig.OnResults(LookupTable);
            FireSerializerModuleAttribute.OnResults(LookupTable);
            DevConsoleCommandAttribute.OnResults(LookupTable);
            DrawingContextAttribute.OnResults(LookupTable);
            PostInitializeAttribute.OnResults(LookupTable);
        }
    }
}
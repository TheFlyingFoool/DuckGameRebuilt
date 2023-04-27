using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuckGame
{

    /// <summary>
    /// Use this attribute on a field or property and it will be saved
    /// on a file when you exit, then loaded when you boot up the game
    /// </summary>
    /// <remarks>
    /// Using this on properties: <br />
    /// The getter will be invoked when the value is being saved <br />
    /// The setter will be invoked when the value is being loaded
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AutoConfigFieldAttribute : Attribute
    {
        public static List<AutoConfigFieldAttribute> All;

        public static void OnResults(Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> all)
        {
            All = new List<AutoConfigFieldAttribute>();
            var allAutoConfig = all[typeof(AutoConfigFieldAttribute)];
            foreach ((MemberInfo memberInfo, Attribute vAttribute) in allAutoConfig)
            {
                AutoConfigFieldAttribute attribute = (AutoConfigFieldAttribute) vAttribute;
                attribute.MemberInfo = memberInfo;
                All.Add(attribute);
            }
        }
        public string? ShortName { get; set; } = null;
        /// <summary>
        /// Whether or not this field will be saved in the event of a crash.
        /// This can be potentially dangerous if the reason for the crash is this
        /// specific config field. In which case the user will be stuck in an
        /// eternal crash loop.
        /// </summary>
        public bool PotentiallyDangerous { get; set; } = false;

        public string? Id { get; set; } = null;

        /// <summary>
        /// The name of the file (without the file extension) that
        /// the data will be saved to instead of on the main file
        /// </summary>
        /// <remarks>
        /// If no value is specified, then the value is saved on
        /// the main file as usual
        /// </remarks>
        public string? External { get; set; } = null;
        
        /// <summary>
        /// The <see cref="MemberInfo"/> the attribute is applied to
        /// </summary>
        public MemberInfo MemberInfo;

        public object GetMemberValue()
        {
            return MemberInfo switch
            {
                FieldInfo fi => fi.GetValue(null),
                PropertyInfo pi => pi.GetMethod?.Invoke(null, null),
                _ => throw new Exception("Unsupported AutoConfig field type")
            };
        }

        public Type GetMemberType()
        {
            return MemberInfo switch
            {
                FieldInfo fi => fi.FieldType,
                PropertyInfo pi => pi.PropertyType,
                _ => throw new Exception("Unsupported AutoConfig field type")
            };
        }
    }
}
using DuckGame;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
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
        internal sealed class AutoConfigAttribute : MarkerAttribute
        {
            public static List<AutoConfigAttribute> All = new();
            
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

            public Type MemberType => Member switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => throw s_notImplementedException
            };

            public object Value
            {
                get => Member switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(null),
                    PropertyInfo propertyInfo => propertyInfo.GetMethod?.Invoke(null, null),
                    _ => throw s_notImplementedException
                };
                set
                {
                    switch (Member)
                    {
                        case FieldInfo fieldInfo:
                            fieldInfo.SetValue(null, value);
                            break;

                        case PropertyInfo propertyInfo:
                            propertyInfo.SetMethod?.Invoke(null, new[] {value});
                            break;

                        default:
                            throw s_notImplementedException;
                    }
                }
            }

            public string UsableName => Id ?? Member.GetFullName();

            private static readonly NotImplementedException s_notImplementedException = new("AutoConfig only supports fields and properties");

            protected override void Implement()
            {
                All.Add(this);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static IReadOnlyList<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>> All;
        public static IReadOnlyList<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>> AllLate;
        public static IReadOnlyList<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>> AllEarly;

        static AutoConfigFieldAttribute()
        {
            MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>.RequestSearch(all =>
            {
                var allEarly = new List<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>>();
                var allLate = new List<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>>();
                var everything = new List<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>>();

                for (int i = 0; i < all.Count; i++)
                {
                    var item = all[i];
                    
                    if (item.Attribute.EarlyLoad)
                    {
                        allEarly.Add(item);
                    }
                    else
                    {
                        allLate.Add(item);
                    }
                    
                    everything.Add(item);
                }

                AllLate = allLate;
                AllEarly = allEarly;
                All = everything;
            });
        }

        public string? ShortName { get; set; } = null;
        /// <summary>
        /// Whether or not this field will be saved in the event of a crash.
        /// This can be potentially dangerous if the reason for the crash is this
        /// specific config field. In which case the user will be stuck in an
        /// eternal crash loop.
        /// </summary>
        public bool PotentiallyDangerous { get; set; } = true;

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
        /// Loads this configuration before literally everything else (Program.Main)
        /// </summary>
        public bool EarlyLoad { get; set; } = false;
    }
}
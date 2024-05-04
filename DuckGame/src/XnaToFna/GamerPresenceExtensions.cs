using MonoMod;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace XnaToFna.StubXDK.GamerServices
{
    public static class GamerPresenceExtensions
    {
        private static Type t_PropertyDictionary;
        private static ConstructorInfo ctor_PropertyDictionary;

        [MonoModHook("System.Void XnaToFna.StubXDK.GamerServices.GamerPresenceExtensions::SetPresenceModeString(Microsoft.Xna.Framework.GamerServices.GamerPresence,System.String)")]
        public static void SetPresenceModeString(object presence, string value)
        {
        }

        [MonoModHook("Microsoft.Xna.Framework.GamerServices.PropertyDictionary XnaToFna.StubXDK.GamerServices.GamerPresenceExtensions::GetProperties(Microsoft.Xna.Framework.GamerServices.GamerPresence)")]
        public static object GetProperties(object presence)
        {
            if (t_PropertyDictionary == null)
            {
                t_PropertyDictionary = StubXDKHelper.GamerServicesAsm.GetType("Microsoft.Xna.Framework.GamerServices.PropertyDictionary");
                ctor_PropertyDictionary = t_PropertyDictionary.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, new Type[1]
                {
          typeof (Dictionary<string, object>)
                }, null);
            }
            return ctor_PropertyDictionary.Invoke(new object[1]
            {
         new Dictionary<string, object>()
            });
        }
    }
}

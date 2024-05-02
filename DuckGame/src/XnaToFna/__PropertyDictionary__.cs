using MonoMod;

namespace XnaToFna.StubXDK.GamerServices
{
    public static class __PropertyDictionary__
    {
        [MonoModHook("System.Void Microsoft.Xna.Framework.GamerServices.PropertyDictionary::SetValue(System.String,System.String)")]
        public static void SetValue(object dictionary, string key, string value)
        {
        }

        [MonoModHook("System.Void Microsoft.Xna.Framework.GamerServices.PropertyDictionary::SetValue(System.String,System.Int32)")]
        public static void SetValue(object dictionary, string key, int value)
        {
        }
    }
}

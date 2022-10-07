// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.__PropertyDictionary__
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

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

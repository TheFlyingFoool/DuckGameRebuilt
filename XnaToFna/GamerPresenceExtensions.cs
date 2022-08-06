// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.GamerPresenceExtensions
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

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
      if (GamerPresenceExtensions.t_PropertyDictionary == (Type) null)
      {
        GamerPresenceExtensions.t_PropertyDictionary = StubXDKHelper.GamerServicesAsm.GetType("Microsoft.Xna.Framework.GamerServices.PropertyDictionary");
        GamerPresenceExtensions.ctor_PropertyDictionary = GamerPresenceExtensions.t_PropertyDictionary.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, (Binder) null, new Type[1]
        {
          typeof (Dictionary<string, object>)
        }, (ParameterModifier[]) null);
      }
      return GamerPresenceExtensions.ctor_PropertyDictionary.Invoke(new object[1]
      {
        (object) new Dictionary<string, object>()
      });
    }
  }
}

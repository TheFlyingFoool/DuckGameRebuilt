// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyForms.Application
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System.Reflection;
using System.Threading;

namespace XnaToFna.ProxyForms
{
  public sealed class Application
  {
    public static event ThreadExceptionEventHandler ThreadException;

    public static string ProductVersion
    {
      get
      {
        Assembly entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
          return null;
        Module manifestModule = entryAssembly.ManifestModule;
        if (manifestModule != null)
        {
          AssemblyInformationalVersionAttribute customAttribute1 = manifestModule.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
          if (customAttribute1 != null)
            return customAttribute1.InformationalVersion;
          AssemblyVersionAttribute customAttribute2 = manifestModule.GetCustomAttribute<AssemblyVersionAttribute>();
          if (customAttribute2 != null)
            return customAttribute2.Version;
        }
        return entryAssembly.GetName().Version.ToString();
      }
    }

    public static string ExecutablePath => Assembly.GetEntryAssembly().Location;

    public static void Run(Form mainForm)
    {
    }

    public static void EnableVisualStyles()
    {
    }
  }
}

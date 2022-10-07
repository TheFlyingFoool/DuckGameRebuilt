// Decompiled with JetBrains decompiler
// Type: XnaToFna.really
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System.Reflection;

namespace XnaToFna
{
    public class really
    {
        public AssemblyName name;

        internal bool ScanPath(XnaToFnaMapping mappings) => this.name.Name == mappings.Target;
    }
}

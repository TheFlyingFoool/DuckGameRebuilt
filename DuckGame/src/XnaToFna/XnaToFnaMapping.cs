// Decompiled with JetBrains decompiler
// Type: XnaToFna.XnaToFnaMapping
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Mono.Cecil;

namespace XnaToFna
{
    public class XnaToFnaMapping
    {
        public bool IsActive;
        public ModuleDefinition Module;
        public string Target;
        public string[] Sources;
        public SetupDelegate Setup;

        public XnaToFnaMapping(string target, string[] sources, SetupDelegate setup = null)
        {
            Target = target;
            Sources = sources;
            Setup = setup;
        }

        public delegate void SetupDelegate(XnaToFnaUtil xtf, XnaToFnaMapping mapping);
    }
}

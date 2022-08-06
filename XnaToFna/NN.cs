// Decompiled with JetBrains decompiler
// Type: XnaToFna.NN
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Mono.Cecil;

namespace XnaToFna
{
  public class NN
  {
    public ModuleDefinition mod;

    public bool Relinkthing(XnaToFnaMapping mappings) => this.mod.Assembly.Name.Name == mappings.Target;
  }
}

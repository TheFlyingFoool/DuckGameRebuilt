// Decompiled with JetBrains decompiler
// Type: XnaToFna.XnaToFnaModder
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using DuckGame;
using Mono.Cecil;
using MonoMod;
using MonoMod.Utils;
using src.XnaToFna;
using System;
using System.Collections.Generic;

namespace XnaToFna
{
    public class XnaToFnaModder : MonoModder
    {
        public Dictionary<string, TranspilerMapEntry> TranspilerMap = new Dictionary<string, TranspilerMapEntry>();

        public XnaToFnaUtil XTF;

        public XnaToFnaModder(XnaToFnaUtil xtf) => XTF = xtf;

        public override void Log(string text)
        {
            if (text.StartsWith("[MapDependency]"))
                return;
            XTF.Log("[MonoMod] " + text);
        }

        public override IMetadataTokenProvider Relinker(
          IMetadataTokenProvider mtp,
          IGenericParameterProvider context)
        {
            return PostRelinker(MainRelinker(mtp, context), context);
        }
        public override ModuleDefinition DefaultMissingDependencyResolver(MonoModder mod, ModuleDefinition main, string name, string fullName)
        {
            if (fullName.StartsWith("Steam,") || fullName.StartsWith("Steam.Debug,"))
            {
                string path = Program.GameDirectory + "DGSteam.dll";
                return MonoModExt.ReadModule(path, GenReaderParameters(false, path));
            }
            return base.DefaultMissingDependencyResolver(mod, main, name, fullName);
        }
        public override void PatchRefsInMethod(MethodDefinition method)
        {
            try
            {
                base.PatchRefsInMethod(method);
            }
            catch(Exception e)
            {
                DevConsole.Log("e");
            }
        }

    }
}

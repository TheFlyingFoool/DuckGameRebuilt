// Decompiled with JetBrains decompiler
// Type: DuckGame.ModDependencyNotFoundException
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Runtime.Serialization;

namespace DuckGame
{
    [Serializable]
    internal class ModDependencyNotFoundException : Exception
    {
        public ModDependencyNotFoundException()
        {
        }

        public ModDependencyNotFoundException(string message)
          : base(message)
        {
        }

        public ModDependencyNotFoundException(string message, Exception inner)
          : base(message, inner)
        {
        }

        protected ModDependencyNotFoundException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public ModDependencyNotFoundException(string mod, string missing)
          : base("Mod " + mod + " is missing hard dependency " + missing)
        {
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.DefaultContentManager
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    /// <summary>
    /// The quick and easy default implementation. Pulls all exported types
    /// that are subclassed by the requested Type.
    /// </summary>
    internal class DefaultContentManager : IManageContent
    {
        public IEnumerable<System.Type> Compile<T>(Mod mod) => mod.configuration.assembly.GetTypes().Where<System.Type>(type => type.IsSubclassOf(typeof(T)));
    }
}

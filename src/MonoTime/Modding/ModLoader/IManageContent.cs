// Decompiled with JetBrains decompiler
// Type: DuckGame.IManageContent
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// The interface with which a mod provides loadable types to
    /// the main assembly. If your mod does not provide a content manager,
    /// it will use the default content manager.
    /// </summary>
    public interface IManageContent
    {
        /// <summary>Provide a list of types that are subclasses of T.</summary>
        /// <typeparam name="T">The type the game requires subclasses of.</typeparam>
        /// <param name="mod">A reference to this mod's Mod object.</param>
        /// <returns>An enumerable Type list.</returns>
        IEnumerable<System.Type> Compile<T>(Mod mod);
    }
}

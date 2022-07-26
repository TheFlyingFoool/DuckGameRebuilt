// Decompiled with JetBrains decompiler
// Type: DuckGame.ICloneable`1
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    /// <summary>
    /// Represents an object that can be cloned into a specific type.
    /// </summary>
    /// <typeparam name="T">The type it can be cloned into.</typeparam>
    public interface ICloneable<T> : ICloneable
    {
        /// <summary>Clones this instance.</summary>
        /// <returns>The new instance.</returns>
        T Clone();
    }
}

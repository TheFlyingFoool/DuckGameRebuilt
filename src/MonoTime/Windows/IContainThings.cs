// Decompiled with JetBrains decompiler
// Type: DuckGame.IContainThings
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// Defines an object which contains, or may contain, these type of objects.
    /// </summary>
    public interface IContainThings
    {
        IEnumerable<System.Type> contains { get; }
    }
}

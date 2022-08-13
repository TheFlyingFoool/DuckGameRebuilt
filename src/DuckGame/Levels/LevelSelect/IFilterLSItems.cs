// Decompiled with JetBrains decompiler
// Type: DuckGame.IFilterLSItems
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    /// <summary>
    /// Represents an interface for filtering level select items from the list based on
    /// conditions.
    /// </summary>
    public interface IFilterLSItems
    {
        /// <summary>Filters the specified level.</summary>
        /// <param name="level">The level.</param>
        /// <param name="location">The location.</param>
        /// <returns>true to keep, false to remove</returns>
        bool Filter(string level, LevelLocation location = LevelLocation.Any);
    }
}

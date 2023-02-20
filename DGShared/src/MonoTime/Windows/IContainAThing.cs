// Decompiled with JetBrains decompiler
// Type: DuckGame.IContainAThing
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    /// <summary>
    /// Defines an object which contain, or may contain, an object of a specific type
    /// </summary>
    public interface IContainAThing
    {
        System.Type contains { get; }
    }
}

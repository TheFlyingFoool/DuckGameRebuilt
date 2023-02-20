// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsMaterial
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    /// <summary>
    /// Represents a material type given to physics objects, and changes
    /// how they interact with the world (metal objects become too hot to hold under heat,
    /// paper/wood burns, etc).
    /// </summary>
    public enum PhysicsMaterial
    {
        Default = 0,
        Metal = 1,
        Rubber = 2,
        Wood = 3,
        Paper = 4,
        Crust = 5,
        Plastic = 6,
        Duck = 7,
        Glass = 8,
        Reserved = 255, // 0x000000FF
    }
}

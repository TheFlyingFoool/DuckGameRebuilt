// Decompiled with JetBrains decompiler
// Type: DuckGame.ATRCShrapnel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATRCShrapnel : AmmoType
    {
        public ATRCShrapnel()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 0.4f;
            bulletSpeed = 18f;
            combustable = true;
        }

        public override void MakeNetEffect(Vec2 pos, bool fromNetwork = false)
        {
            for (int index = 0; index < 1; index = index + 1 + 1)
                Level.Add(new ExplosionPart(pos.x - 20f + Rando.Float(40f), pos.y - 20f + Rando.Float(40f)));
            SFX.Play("explode");
        }
    }
}

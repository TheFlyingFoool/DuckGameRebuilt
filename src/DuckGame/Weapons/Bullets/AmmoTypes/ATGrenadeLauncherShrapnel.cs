// Decompiled with JetBrains decompiler
// Type: DuckGame.ATGrenadeLauncherShrapnel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATGrenadeLauncherShrapnel : ATShrapnel
    {
        public override void MakeNetEffect(Vec2 pos, bool fromNetwork = false)
        {
            for (int index = 0; index < 1; ++index)
            {
                ExplosionPart explosionPart = new ExplosionPart(pos.x - 8f + Rando.Float(16f), pos.y - 8f + Rando.Float(16f));
                explosionPart.xscale *= 0.7f;
                explosionPart.yscale *= 0.7f;
                Level.Add(explosionPart);
            }
            SFX.Play("explode");
        }
    }
}

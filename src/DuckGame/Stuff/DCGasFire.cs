// Decompiled with JetBrains decompiler
// Type: DuckGame.DCGasFire
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DCGasFire : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            Level.Add((Thing)new ExplosionPart(c.x, c.y - 2f));
            if (server)
            {
                YellowBarrel yellowBarrel = new YellowBarrel(c.x, c.y);
                yellowBarrel.vSpeed = -3f;
                Level.Add((Thing)yellowBarrel);
                Grenade grenade1 = new Grenade(c.x, c.y);
                grenade1.PressAction();
                grenade1.hSpeed = -1f;
                grenade1.vSpeed = -2f;
                Level.Add((Thing)grenade1);
                Grenade grenade2 = new Grenade(c.x, c.y);
                grenade2.PressAction();
                grenade2.hSpeed = 1f;
                grenade2.vSpeed = -2f;
                Level.Add((Thing)grenade2);
                Level.Remove((Thing)c);
            }
            Level.Add((Thing)new MusketSmoke(c.x, c.y));
        }
    }
}

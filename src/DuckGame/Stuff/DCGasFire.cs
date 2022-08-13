// Decompiled with JetBrains decompiler
// Type: DuckGame.DCGasFire
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DCGasFire : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            Level.Add(new ExplosionPart(c.x, c.y - 2f));
            if (server)
            {
                YellowBarrel yellowBarrel = new YellowBarrel(c.x, c.y)
                {
                    vSpeed = -3f
                };
                Level.Add(yellowBarrel);
                Grenade grenade1 = new Grenade(c.x, c.y);
                grenade1.PressAction();
                grenade1.hSpeed = -1f;
                grenade1.vSpeed = -2f;
                Level.Add(grenade1);
                Grenade grenade2 = new Grenade(c.x, c.y);
                grenade2.PressAction();
                grenade2.hSpeed = 1f;
                grenade2.vSpeed = -2f;
                Level.Add(grenade2);
                Level.Remove(c);
            }
            Level.Add(new MusketSmoke(c.x, c.y));
        }
    }
}

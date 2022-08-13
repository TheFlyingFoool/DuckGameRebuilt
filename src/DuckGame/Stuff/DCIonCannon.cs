// Decompiled with JetBrains decompiler
// Type: DuckGame.DCIonCannon
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DCIonCannon : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            Level.Add(new ExplosionPart(c.x, c.y - 2f));
            Level.Add(new IonCannon(new Vec2(c.x, c.y + 3000f), new Vec2(c.x, c.y - 3000f))
            {
                serverVersion = server
            });
            Graphics.FlashScreen();
            SFX.Play("laserBlast");
            if (!server)
                return;
            Level.Remove(c);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.ATGrenade
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATGrenade : AmmoType
    {
        public ATGrenade()
        {
            accuracy = 1f;
            penetration = 0.35f;
            bulletSpeed = 9f;
            rangeVariation = 0f;
            speedVariation = 0f;
            range = 2000f;
            rebound = true;
            affectedByGravity = true;
            deadly = false;
            weight = 5f;
            ownerSafety = 4;
            bulletThickness = 2f;
            bulletColor = Color.White;
            bulletType = typeof(GrenadeBullet);
            immediatelyDeadly = true;
            sprite = new Sprite("launcherGrenade");
            sprite.CenterOrigin();
            flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(pistolShell);
        }
    }
}

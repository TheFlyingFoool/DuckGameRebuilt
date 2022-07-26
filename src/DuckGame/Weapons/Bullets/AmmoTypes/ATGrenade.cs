// Decompiled with JetBrains decompiler
// Type: DuckGame.ATGrenade
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATGrenade : AmmoType
    {
        public ATGrenade()
        {
            this.accuracy = 1f;
            this.penetration = 0.35f;
            this.bulletSpeed = 9f;
            this.rangeVariation = 0.0f;
            this.speedVariation = 0.0f;
            this.range = 2000f;
            this.rebound = true;
            this.affectedByGravity = true;
            this.deadly = false;
            this.weight = 5f;
            this.ownerSafety = 4;
            this.bulletThickness = 2f;
            this.bulletColor = Color.White;
            this.bulletType = typeof(GrenadeBullet);
            this.immediatelyDeadly = true;
            this.sprite = new Sprite("launcherGrenade");
            this.sprite.CenterOrigin();
            this.flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y);
            pistolShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)pistolShell);
        }
    }
}

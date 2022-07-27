// Decompiled with JetBrains decompiler
// Type: DuckGame.ATMissileShrapnel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATMissileShrapnel : AmmoType
    {
        public ATMissileShrapnel()
        {
            this.accuracy = 0.75f;
            this.range = 250f;
            this.penetration = 0.4f;
            this.bulletSpeed = 18f;
            this.combustable = true;
        }

        public override void MakeNetEffect(Vec2 pos, bool fromNetwork = false)
        {
            Level.Add(new ExplosionPart(pos.x + Rando.Float(-2f, 2f), pos.y + Rando.Float(-2f, 2f), false));
            for (int index = 0; index < 4; ++index)
                Level.Add(new ExplosionPart(pos.x + Rando.Float(-11f, 11f), pos.y + Rando.Float(-11f, 11f), false));
            if (fromNetwork)
            {
                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(pos, 70f))
                {
                    if (physicsObject.isServerForObject)
                    {
                        physicsObject.sleeping = false;
                        physicsObject.vSpeed = -2f;
                    }
                }
            }
            SFX.Play("explode");
        }
    }
}

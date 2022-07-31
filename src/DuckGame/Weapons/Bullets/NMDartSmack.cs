// Decompiled with JetBrains decompiler
// Type: DuckGame.NMDartSmack
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMDartSmack : NMEvent
    {
        public Vec2 vector;
        public PhysicsObject hit;

        public NMDartSmack(Vec2 pVector, PhysicsObject pSmack)
        {
            this.vector = pVector;
            this.hit = pSmack;
        }

        public NMDartSmack()
        {
        }

        public override void Activate()
        {
            if (Level.current == null || this.hit == null || !this.hit.isServerForObject)
                return;
            this.hit.hSpeed += this.vector.x;
            this.hit.vSpeed += this.vector.y;
            Duck duck = this.hit as Duck;
            if (!(duck != null) && this.hit is RagdollPart && (this.hit as RagdollPart).doll != null && (this.hit as RagdollPart).doll.captureDuck != null)
                duck = (this.hit as RagdollPart).doll.captureDuck;
            if (duck == null)
                return;
            if (duck.holdObject is Grenade)
                duck.forceFire = true;
            if (Rando.Float(1f) > 0.6f)
                duck.Swear();
            duck.Disarm(null);
        }
    }
}

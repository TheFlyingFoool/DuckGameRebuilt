// Decompiled with JetBrains decompiler
// Type: DuckGame.ControlWave
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ControlWave : Thing, ITeleport
    {
        private MindControlRay _owner;
        private float _fade = 1f;
        private bool _isNotControlRay;
        private bool _isLocalWave = true;

        public ControlWave(float xpos, float ypos, float dir, MindControlRay owner, bool local = true)
          : base(xpos, ypos)
        {
            this._owner = owner;
            this.graphic = new Sprite("controlWave")
            {
                flipH = this.offDir < 0
            };
            this.center = new Vec2(8f, 8f);
            this.xscale = this.yscale = 0.2f;
            this.angle = dir;
            this._isLocalWave = local;
        }

        public override void Update()
        {
            if (this.isServerForObject)
            {
                this.xscale = this.yscale = Maths.CountUp(this.yscale, 0.05f);
                this._fade -= 0.05f;
                if (_fade < 0.0)
                    Level.Remove(this);
                this.alpha = Maths.NormalizeSection(this._fade, 0.2f, 0.3f);
                Vec2 p2 = Vec2.Zero;
                if (this._owner.controlledDuck == null && !this._isNotControlRay)
                {
                    p2 = new Vec2((float)Math.Cos((double)this.angle), (float)-Math.Sin((double)this.angle));
                    if (this._isLocalWave)
                    {
                        foreach (IAmADuck amAduck in Level.CheckCircleAll<IAmADuck>(this.position, 3f))
                        {
                            Duck d = amAduck as Duck;
                            switch (amAduck)
                            {
                                case RagdollPart _ when (amAduck as RagdollPart).doll.captureDuck != null:
                                    d = (amAduck as RagdollPart).doll.captureDuck;
                                    break;
                                case TrappedDuck _ when (amAduck as TrappedDuck).captureDuck != null:
                                    d = (amAduck as TrappedDuck).captureDuck;
                                    break;
                            }
                            if (d != null && d.mindControl == null && !d.HasEquipment(typeof(TinfoilHat)) && !(d.holdObject is MindControlRay))
                                this._owner.ControlDuck(d);
                        }
                    }
                }
                else
                {
                    if (this._owner.controlledDuck != null)
                    {
                        p2 = this._owner.controlledDuck.cameraPosition - this.position;
                        p2.Normalize();
                        this.angleDegrees = -Maths.PointDirection(Vec2.Zero, p2);
                    }
                    this._isNotControlRay = true;
                }
                this.position += p2 * 2.6f;
            }
            else
            {
                this.xscale = this.yscale = 1f;
                this.position += new Vec2((float)Math.Cos((double)this.angle), (float)-Math.Sin((double)this.angle)) * 2.6f;
            }
        }
    }
}

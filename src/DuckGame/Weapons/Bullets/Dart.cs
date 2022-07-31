// Decompiled with JetBrains decompiler
// Type: DuckGame.Dart
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Dart : PhysicsObject, IPlatform
    {
        public StateBinding _stickTimeBinding = new StateBinding(nameof(_stickTime));
        public StateBinding _stuckBinding = new StateBinding(nameof(_stuck));
        private SpriteMap _sprite;
        public bool _stuck;
        public float _stickTime = 1f;
        private Duck _owner;
        public bool burning;

        public Dart(float xpos, float ypos, Duck owner, float fireAngle)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("dart", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-4f, -2f);
            this.collisionSize = new Vec2(9f, 4f);
            this.depth = -0.5f;
            this.thickness = 1f;
            this.weight = 3f;
            this._owner = owner;
            this.breakForce = 1f;
            this._stickTime = 2f + Rando.Float(0.8f);
            if ((double)Rando.Float(1f) > 0.949999988079071)
                this._stickTime += Rando.Float(15f);
            this.angle = fireAngle;
            if (owner == null)
                return;
            owner.clip.Add(this);
            this.clip.Add(owner);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this._stuck && _stickTime > 0.980000019073486)
                return false;
            if (type is DTFade)
            {
                DartShell dartShell = new DartShell(this.x, this.y, Rando.Float(0.1f) * -this._sprite.flipMultH, this._sprite.flipH)
                {
                    angle = this.angle
                };
                Level.Add(dartShell);
                dartShell.hSpeed = (float)((0.5 + (double)Rando.Float(0.3f)) * -(double)this._sprite.flipMultH);
                Level.Remove(this);
                return true;
            }
            if (this._stuck && _stickTime > 0.4f)
                this._stickTime = 0.4f;
            return false;
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._stuck || with is Gun || (with.weight < 5f && !(with is Dart) && !(with is RagdollPart)) || with is FeatherVolume || with is Teleporter || base.removeFromLevel || with is Spring || with is SpringUpLeft || with is SpringUpRight)
            {
                if (with is EnergyBlocker && with.solid)
                {
                    this.LightOnFire();
                }
                return;
            }
            if (!this.destroyed && !this._stuck)
            {
                if (with is PhysicsObject)
                {
                    Duck duck = with as Duck;
                    if (duck != null && base.isServerForObject)
                    {
                        if (duck.isServerForObject)
                        {
                            duck.hSpeed += this.hSpeed * 0.7f;
                            duck.vSpeed -= 1.5f;
                            Event.Log(new DartHitEvent(base.responsibleProfile, duck.profile));
                            if (duck.holdObject is Grenade)
                            {
                                duck.forceFire = true;
                            }
                            if (Rando.Float(1f) > 0.6f)
                            {
                                duck.Swear();
                            }
                            duck.Disarm(this);
                        }
                        else
                        {
                            Send.Message(new NMDartSmack(new Vec2(this.hSpeed * 0.7f, -1.5f), duck), duck.connection);
                        }
                    }
                    RagdollPart r = with as RagdollPart;
                    if (r != null && base.isServerForObject && r.doll != null && r.doll.captureDuck != null)
                    {
                        Duck d = r.doll.captureDuck;
                        if (r.isServerForObject)
                        {
                            r.hSpeed += this.hSpeed * 0.7f;
                            r.vSpeed -= 1.5f;
                            if (d.holdObject is Grenade)
                            {
                                d.forceFire = true;
                            }
                            if (Rando.Float(1f) > 0.6f)
                            {
                                d.Swear();
                            }
                            d.Disarm(this);
                        }
                        else
                        {
                            Send.Message(new NMDartSmack(new Vec2(this.hSpeed * 0.7f, -1.5f), r), r.connection);
                        }
                    }
                    if (with is IPlatform || duck != null || r != null)
                    {
                        DartShell dartShell = new DartShell(base.x, base.y, -this._sprite.flipMultH * Rando.Float(0.6f), this._sprite.flipH);
                        Level.Add(dartShell);
                        dartShell.hSpeed = -this.hSpeed / 3f * (0.3f + Rando.Float(0.8f));
                        dartShell.vSpeed = -2f + Rando.Float(4f);
                        Level.Remove(this);
                        if (this.burning)
                        {
                            with.Burn(this.position, this);
                        }
                        return;
                    }
                }
                float deg = -base.angleDegrees % 360f;
                if (deg < 0f)
                {
                    deg += 360f;
                }
                bool stick = false;
                if ((with is Block || with is Spikes || with is Saws) && from == ImpactedFrom.Right && (deg < 45f || deg > 315f))
                {
                    stick = true;
                    base.angleDegrees = 0f;
                }
                else if ((with is Block || with is Spikes || with is Saws) && from == ImpactedFrom.Top && deg > 45f && deg < 135f)
                {
                    stick = true;
                    base.angleDegrees = 270f;
                }
                else if ((with is Block || with is Spikes || with is Saws) && from == ImpactedFrom.Left && deg > 135f && deg < 225f)
                {
                    stick = true;
                    base.angleDegrees = 180f;
                }
                else if (from == ImpactedFrom.Bottom && deg > 225f && deg < 315f)
                {
                    stick = true;
                    base.angleDegrees = 90f;
                }
                if (stick)
                {
                    this._stuck = true;
                    SFX.Play("dartStick", 0.8f, -0.1f + Rando.Float(0.2f), 0f, false);
                    this.vSpeed = 0f;
                    this.gravMultiplier = 0f;
                    base.grounded = true;
                    this._sprite.frame = 1;
                    this._stickTime = 1f;
                }
            }
        }

        public void LightOnFire()
        {
            if (this.burning)
                return;
            this.burning = true;
            this.onFire = true;
            Level.Add(SmallFire.New(0.0f, 0.0f, 0.0f, 0.0f, stick: this, firedFrom: this));
            SFX.Play("ignite", Rando.Float(0.9f, 1f), Rando.Float(-0.2f, 0.2f));
        }

        public override void Update()
        {
            base.Update();
            if (!this.destroyed && !this._stuck)
            {
                if (!this.burning && Level.CheckCircle<SmallFire>(this.position, 8f) != null)
                    this.LightOnFire();
                this._sprite.frame = 0;
                this.angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(this.hSpeed, this.vSpeed));
            }
            if (this._stuck)
            {
                this.vSpeed = 0.0f;
                this.hSpeed = 0.0f;
                this.grounded = true;
                this._sprite.frame = 1;
                this._stickTime -= 0.005f;
                this.gravMultiplier = 0.0f;
            }
            if (_stickTime > 0.0 || this.destroyed)
                return;
            this.Destroy(new DTFade());
        }
    }
}

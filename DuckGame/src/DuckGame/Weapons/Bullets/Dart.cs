// Decompiled with JetBrains decompiler
// Type: DuckGame.Dart
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap("dart", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -2f);
            collisionSize = new Vec2(9f, 4f);
            depth = -0.5f;
            thickness = 1f;
            weight = 3f;
            _owner = owner;
            breakForce = 1f;
            _stickTime = 2f + Rando.Float(0.8f);
            if (Rando.Float(1f) > 0.95f)
                _stickTime += Rando.Float(15f);
            angle = fireAngle;
            if (owner == null)
                return;
            owner.clip.Add(this);
            clip.Add(owner);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (_stuck && _stickTime > 0.98f)
                return false;
            if (type is DTFade)
            {
                DartShell dartShell = new DartShell(x, y, Rando.Float(0.1f) * -_sprite.flipMultH, _sprite.flipH)
                {
                    angle = angle
                };
                Level.Add(dartShell);
                dartShell.hSpeed = (float)((0.5 + Rando.Float(0.3f)) * -_sprite.flipMultH);
                Level.Remove(this);
                return true;
            }
            if (_stuck && _stickTime > 0.4f)
                _stickTime = 0.4f;
            return false;
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (_stuck || with is Gun || (with.weight < 5f && !(with is Dart) && !(with is RagdollPart)) || with is FeatherVolume || with is Teleporter || removeFromLevel || with is Spring || with is SpringUpLeft || with is SpringUpRight)
            {
                if (with is EnergyBlocker && with.solid)
                {
                    LightOnFire();
                }
                return;
            }
            if (!destroyed && !_stuck)
            {
                if (with is PhysicsObject)
                {
                    Duck duck = with as Duck;
                    if (duck != null && isServerForObject)
                    {
                        if (duck.isServerForObject)
                        {
                            duck.hSpeed += hSpeed * 0.7f;
                            duck.vSpeed -= 1.5f;
                            Event.Log(new DartHitEvent(responsibleProfile, duck.profile));
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
                            Send.Message(new NMDartSmack(new Vec2(hSpeed * 0.7f, -1.5f), duck), duck.connection);
                        }
                    }
                    RagdollPart r = with as RagdollPart;
                    if (r != null && isServerForObject && r.doll != null && r.doll.captureDuck != null)
                    {
                        Duck d = r.doll.captureDuck;
                        if (r.isServerForObject)
                        {
                            r.hSpeed += hSpeed * 0.7f;
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
                            Send.Message(new NMDartSmack(new Vec2(hSpeed * 0.7f, -1.5f), r), r.connection);
                        }
                    }
                    if (with is IPlatform || duck != null || r != null)
                    {
                        DartShell dartShell = new DartShell(x, y, -_sprite.flipMultH * Rando.Float(0.6f), _sprite.flipH);
                        Level.Add(dartShell);
                        dartShell.hSpeed = -hSpeed / 3f * (0.3f + Rando.Float(0.8f));
                        dartShell.vSpeed = -2f + Rando.Float(4f);
                        Level.Remove(this);
                        if (burning)
                        {
                            with.Burn(position, this);
                        }
                        return;
                    }
                }
                float deg = -angleDegrees % 360f;
                if (deg < 0f)
                {
                    deg += 360f;
                }
                bool stick = false;
                if ((with is Block || with is Spikes || with is Saws) && from == ImpactedFrom.Right && (deg < 45f || deg > 315f))
                {
                    stick = true;
                    angleDegrees = 0f;
                }
                else if ((with is Block || with is Spikes || with is Saws) && from == ImpactedFrom.Top && deg > 45f && deg < 135f)
                {
                    stick = true;
                    angleDegrees = 270f;
                }
                else if ((with is Block || with is Spikes || with is Saws) && from == ImpactedFrom.Left && deg > 135f && deg < 225f)
                {
                    stick = true;
                    angleDegrees = 180f;
                }
                else if (from == ImpactedFrom.Bottom && deg > 225f && deg < 315f)
                {
                    stick = true;
                    angleDegrees = 90f;
                }
                if (stick)
                {
                    _stuck = true;
                    SFX.Play("dartStick", 0.8f, -0.1f + Rando.Float(0.2f), 0f, false);
                    vSpeed = 0f;
                    gravMultiplier = 0f;
                    grounded = true;
                    _sprite.frame = 1;
                    _stickTime = 1f;
                }
            }
        }

        public void LightOnFire()
        {
            if (burning)
                return;
            burning = true;
            onFire = true;
            Level.Add(SmallFire.New(0f, 0f, 0f, 0f, stick: this, firedFrom: this));
            SFX.Play("ignite", Rando.Float(0.9f, 1f), Rando.Float(-0.2f, 0.2f));
        }

        public override void Update()
        {
            base.Update();
            if (!destroyed && !_stuck)
            {
                if (!burning && Level.CheckCircle<SmallFire>(position, 8f) != null)
                    LightOnFire();
                _sprite.frame = 0;
                angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(hSpeed, vSpeed));
            }
            if (_stuck)
            {
                vSpeed = 0f;
                hSpeed = 0f;
                grounded = true;
                _sprite.frame = 1;
                _stickTime -= 0.005f;
                gravMultiplier = 0f;
            }
            if (_stickTime > 0 || destroyed)
                return;
            Destroy(new DTFade());
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.MagnetGun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    public class MagnetGun : Gun
    {
        public StateBinding _powerBinding = new StateBinding(nameof(_power));
        public StateBinding _grabbedBinding = new StateBinding(nameof(grabbed));
        public StateBinding _magnetActiveBinding = new StateBinding(nameof(_magnetActive));
        public StateBinding _keepRaisedBinding = new StateBinding("_keepRaised");
        public StateBinding _attachIndexBinding = new StateBinding(nameof(attachIndex));
        public NetIndex4 attachIndex = new NetIndex4(0);
        public NetIndex4 localAttachIndex = new NetIndex4(0);
        private Sprite _magnet;
        private SinWave _wave = (SinWave)0.8f;
        private float _waveMult;
        public Thing _grabbed;
        private Block _stuck;
        private Vec2 _stickPos = Vec2.Zero;
        private Vec2 _stickNormal = Vec2.Zero;
        private Sound _beamSound;
        public bool _magnetActive;
        private List<MagnaLine> _lines = new List<MagnaLine>();
        private Vec2 _rayHit;
        private bool _hasRay;
        private bool prevMagActive;
        public float _power = 1f;
        public Thing _prevGrabDuck;

        public Thing grabbed
        {
            get => _grabbed;
            set
            {
                if (_grabbed != null && _grabbed != value)
                    ReleaseGrab(_grabbed);
                _grabbed = value;
            }
        }

        public MagnetGun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            _ammoType = new ATLaser
            {
                range = 150f,
                accuracy = 0.8f,
                penetration = -1f
            };
            _type = "gun";
            graphic = new Sprite("magnetGun");
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(14f, 9f);
            _barrelOffsetTL = new Vec2(24f, 14f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _magnet = new Sprite("magnet");
            _magnet.CenterOrigin();
            _bio = "Nope.";
            _editorName = "Magnet Gun";
            editorTooltip = "Attracts metal objects. This seems like a bad idea.";
            _holdOffset = new Vec2(3f, 1f);
            _lowerOnFire = false;
        }

        public override void Initialize()
        {
            _beamSound = SFX.Get("magnetBeam", 0f, looped: true);
            int num = 10;
            for (int index = 0; index < num; ++index)
                _lines.Add(new MagnaLine(0f, 0f, this, _ammoType.range, index / (float)num));
            base.Initialize();
        }

        public override void CheckIfHoldObstructed()
        {
            if (_stuck != null)
                return;
            base.CheckIfHoldObstructed();
        }

        public override void Update()
        {
            _waveMult = Lerp.Float(_waveMult, 0f, 0.1f);
            if (isServerForObject && !Recorderator.Playing)
                _magnetActive = action && _power > 0.01f;
            if (_magnetActive)
                _waveMult = 1f;
            if (isServerForObject && _magnetActive && !prevMagActive)
                _power -= 0.01f;
            prevMagActive = _magnetActive;
            if (_beamSound.Volume > 0.01f && _beamSound.State != SoundState.Playing)
                _beamSound.Play();
            else if (_beamSound.Volume < 0.01f && _beamSound.State == SoundState.Playing)
                _beamSound.Stop();
            _beamSound.Volume = Maths.LerpTowards(_beamSound.Volume, _magnetActive ? 0.1f : 0f, 0.1f);
            if (_power > 1)
                _power = 1f;
            if (_power < 0f)
                _power = 0f;
            _beamSound.Pitch = _power >= 0.5f ? 0f : _power - 0.5f;
            if (isServerForObject && (duck == null && grounded || duck != null && duck.grounded || infinite.value))
                _power = 1f;
            Vec2 p1_1 = Offset(barrelOffset);
            bool flag1 = _grabbed is MagnetGun && (_grabbed as MagnetGun)._grabbed == this;
            if (_magnetActive && held && !flag1)
            {
                if (isServerForObject && duck != null && !duck.grounded)
                    _power -= 0.0015f;
                foreach (MagnaLine line in _lines)
                {
                    line.Update();
                    line.show = true;
                    float num = _ammoType.range;
                    if (_hasRay)
                        num = (barrelPosition - _rayHit).length;
                    line.dist = num;
                }
                if (_grabbed == null && _stuck == null)
                {
                    Holdable holdable1 = null;
                    float val1 = 0f;
                    Vec2 vec2_1 = barrelVector.Rotate(Maths.DegToRad(90f), Vec2.Zero);
                    Vec2 normalized1 = vec2_1.normalized;
                    for (int index = 0; index < 3; ++index)
                    {
                        Vec2 p1_2 = p1_1;
                        if (index == 0)
                            p1_2 += normalized1 * 8f;
                        else if (index == 2)
                            p1_2 -= normalized1 * 8f;
                        foreach (Holdable holdable2 in Level.CheckLineAll<Holdable>(p1_2, p1_2 + barrelVector * _ammoType.range))
                        {
                            if (holdable2 != this && holdable2 != owner && holdable2.owner != owner && holdable2.physicsMaterial == PhysicsMaterial.Metal && (holdable2.duck == null || !(holdable2.duck.holdObject is MagnetGun)) && (holdable2.duck == null || !(holdable2.duck.holdObject is TapedGun) || holdable2 == holdable2.duck.holdObject) && !(holdable2.owner is MagnetGun))
                            {
                                Holdable holdable3 = holdable2;
                                if (holdable2.tape != null)
                                    holdable3 = holdable2.tape;
                                vec2_1 = holdable3.position - p1_1;
                                float length = vec2_1.length;
                                if (holdable1 == null || length < val1)
                                {
                                    val1 = length;
                                    holdable1 = holdable3;
                                }
                            }
                        }
                    }
                    _hasRay = false;
                    if (holdable1 != null && Level.CheckLine<Block>(p1_1, holdable1.position) == null)
                    {
                        float num = ((1f - Math.Min(val1, _ammoType.range) / _ammoType.range) * 0.8f);
                        Duck duck = holdable1.owner as Duck;
                        if (duck != null && !(duck.holdObject is MagnetGun) && num > 0.3f)
                        {
                            if (!(holdable1 is Equipment) || holdable1.equippedDuck == null)
                            {
                                duck.ThrowItem(false);
                                duck = null;
                            }
                            else if (holdable1 is TinfoilHat)
                            {
                                duck.Unequip(holdable1 as Equipment);
                                duck = null;
                            }
                        }
                        vec2_1 = p1_1 - holdable1.position;
                        Vec2 normalized2 = vec2_1.normalized;
                        if (duck != null && holdable1 is Equipment)
                        {
                            if (duck.ragdoll != null)
                            {
                                duck.ragdoll.makeActive = true;
                                return;
                            }
                            if (!(holdable1.owner.realObject is Duck) && Network.isActive)
                                return;
                            holdable1.owner.realObject.hSpeed += normalized2.x * num;
                            holdable1.owner.realObject.vSpeed += (float)(normalized2.y * num * 4);
                            if ((holdable1.owner.realObject as PhysicsObject).grounded && holdable1.owner.realObject.vSpeed > 0)
                                holdable1.owner.realObject.vSpeed = 0f;
                        }
                        else
                        {
                            Fondle(holdable1);
                            holdable1.hSpeed += normalized2.x * num;
                            holdable1.vSpeed += (float)(normalized2.y * num * 4);
                            if (holdable1.grounded && holdable1.vSpeed > 0)
                                holdable1.vSpeed = 0f;
                        }
                        _hasRay = true;
                        _rayHit = holdable1.position;
                        if (isServerForObject && val1 < 20)
                        {
                            if (holdable1 is Equipment && holdable1.duck != null)
                            {
                                _grabbed = holdable1.owner.realObject;
                                RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.Short));
                                holdable1.duck.immobilized = true;
                                holdable1.duck.gripped = true;
                                holdable1.duck.ThrowItem();
                                if (_grabbed != null && holdable1.owner != null && !(_grabbed is Duck))
                                {
                                    _grabbed.owner = this;
                                    _grabbed.offDir = offDir;
                                    SuperFondle(_grabbed, DuckNetwork.localConnection);
                                }
                            }
                            else
                            {
                                _grabbed = holdable1;
                                RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.Short));
                                holdable1.owner = this;
                                holdable1.owner.offDir = offDir;
                                if (holdable1 is Grenade)
                                    (holdable1 as Grenade).OnPressAction();
                            }
                            attachIndex += 1;
                        }
                    }
                    else if (isServerForObject && _stuck == null && (Math.Abs(angle) < 0.05f || Math.Abs(angle) > 1.5f))
                    {
                        Vec2 position = owner.position;
                        if (duck.sliding)
                            position.y += 4f;
                        Vec2 hitPos;
                        Block block = Level.CheckRay<Block>(position, position + barrelVector * _ammoType.range, out hitPos);
                        _hasRay = true;
                        _rayHit = hitPos;
                        if (block != null && block.physicsMaterial == PhysicsMaterial.Metal)
                        {
                            vec2_1 = block.position - position;
                            float num = ((1f - Math.Min(vec2_1.length, _ammoType.range) / _ammoType.range) * 0.8f);
                            Vec2 vec2_2 = hitPos - duck.position;
                            double length = vec2_2.length;
                            vec2_2.Normalize();
                            owner.hSpeed += vec2_2.x * num;
                            owner.vSpeed += vec2_2.y * num;
                            if (length < 20f)
                            {
                                _stuck = block;
                                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.Short));
                                _stickPos = hitPos;
                                _stickNormal = -barrelVector;
                                attachIndex += 1;
                            }
                        }
                    }
                }
            }
            else
            {
                if (isServerForObject)
                {
                    if (_grabbed != null)
                    {
                        ReleaseGrab(_grabbed);
                        _grabbed = null;
                        _collisionSize = new Vec2(14f, _collisionSize.y);
                    }
                    if (_stuck != null)
                    {
                        _stuck = null;
                        if (owner != null && !_raised)
                            duck._groundValid = 6;
                    }
                }
                foreach (MagnaLine line in _lines)
                    line.show = false;
            }
            if (owner is MagnetGun && (owner as MagnetGun)._grabbed != this)
            {
                Fondle(this);
                ReleaseGrab(this);
            }
            if (Network.isActive)
            {
                if (_grabbed != null)
                {
                    if (_grabbed is TrappedDuck && _grabbed.connection != connection)
                    {
                        _grabbed = (_grabbed as TrappedDuck)._duckOwner;
                        if (_grabbed != null)
                        {
                            Duck grabbed = _grabbed as Duck;
                            grabbed.immobilized = true;
                            grabbed.gripped = true;
                            grabbed.ThrowItem();
                            grabbed._trapped = null;
                        }
                    }
                    if (_grabbed is Duck grabbed1)
                    {
                        grabbed1.isGrabbedByMagnet = true;
                        if (isServerForObject)
                        {
                            Fondle(grabbed1);
                            Fondle(grabbed1.holdObject);
                            foreach (Thing t in grabbed1._equipment)
                                Fondle(t);
                            Fondle(grabbed1._ragdollInstance);
                            Fondle(grabbed1._trappedInstance);
                            Fondle(grabbed1._cookedInstance);
                        }
                    }
                }
                if (_grabbed == null && _prevGrabDuck != null && _prevGrabDuck is Duck)
                    (_prevGrabDuck as Duck).isGrabbedByMagnet = false;
                _prevGrabDuck = _grabbed;
            }
            if (_grabbed != null && owner != null)
            {
                if (isServerForObject)
                    Fondle(_grabbed);
                _grabbed.hSpeed = owner.hSpeed;
                _grabbed.vSpeed = owner.vSpeed;
                _grabbed.angle = angle;
                _grabbed.offDir = offDir;
                _grabbed.enablePhysics = false;
                if (_grabbed is TapedGun)
                    (_grabbed as TapedGun).UpdatePositioning();
                _collisionSize = new Vec2(16f + _grabbed.width, _collisionSize.y);
                if (_grabbed is Duck grabbed)
                {
                    grabbed.grounded = true;
                    grabbed.sliding = false;
                    grabbed.crouch = false;
                }
            }
            if (localAttachIndex < attachIndex)
            {
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                    Level.Add(SmallSmoke.New(p1_1.x + Rando.Float(-1f, 1f), p1_1.y + Rando.Float(-1f, 1f)));
                SFX.Play("grappleHook");
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                    Level.Add(Spark.New(p1_1.x - barrelVector.x * 2f + Rando.Float(-1f, 1f), p1_1.y - barrelVector.y * 2f + Rando.Float(-1f, 1f), barrelVector + new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
                localAttachIndex = attachIndex;
            }
            if (isServerForObject)
            {
                if (_magnetActive && _raised && duck != null && !duck.grounded && _grabbed == null)
                    _keepRaised = true;
                else
                    _keepRaised = false;
                if (_stuck != null && duck != null)
                {
                    if (_stickPos.y < owner.position.y - 8)
                    {
                        owner.position = _stickPos + _stickNormal * 12f;
                        _raised = true;
                        _keepRaised = true;
                    }
                    else
                    {
                        owner.position = _stickPos + _stickNormal * 16f;
                        _raised = false;
                        _keepRaised = false;
                    }
                    owner.hSpeed = owner.vSpeed = 0f;
                    duck.moveLock = true;
                }
                else if (_stuck == null && duck != null)
                    duck.moveLock = false;
                if (owner == null && prevOwner != null)
                {
                    if (this.prevOwner is Duck prevOwner)
                        prevOwner.moveLock = false;
                    _prevOwner = null;
                }
            }
            bool flag2 = _grabbed is MagnetGun && (_grabbed as MagnetGun)._grabbed == this;
            if (_grabbed != null && !flag2)
            {
                if (_grabbed is Duck)
                {
                    _grabbed.position = Offset(barrelOffset + new Vec2(0f, -6f)) + barrelVector * _grabbed.halfWidth;
                    (_grabbed as Duck).UpdateSkeleton();
                    (_grabbed as Duck).gripped = true;
                }
                else _grabbed.position = Offset(barrelOffset) + barrelVector * _grabbed.halfWidth;
            }
            else
            {
                int num1 = flag2 ? 1 : 0;
            }
            base.Update();
        }

        private void ReleaseGrab(Thing pThing)
        {
            pThing.angle = 0f;
            if (pThing is Holdable t)
            {
                t.owner = null;
                t.ReturnToWorld();
                ReturnItemToWorld(t);
            }
            if (pThing is Duck duck)
            {
                duck.immobilized = false;
                duck.gripped = false;
                duck.crippleTimer = 1f;
            }
            pThing.enablePhysics = true;
            pThing.hSpeed = barrelVector.x * 5f;
            pThing.vSpeed = barrelVector.y * 5f;
            if (!(pThing is EnergyScimitar))
                return;
            (pThing as EnergyScimitar).StartFlying(offDir < 0 ? (float)(-angleDegrees - 180) : -angleDegrees, true);
        }

        public override void Draw()
        {
            base.Draw();
            Draw(ref _magnet, new Vec2(5f, (float)((float)_wave * _waveMult - 2)));
            foreach (Thing line in _lines)
                line.Draw();
        }

        public override void OnPressAction()
        {
            _waveMult = 1f;
            if (!raised)
                return;
            _keepRaised = true;
        }

        public override void OnHoldAction()
        {
        }

        public override void OnReleaseAction() => _keepRaised = false;

        public override void Fire()
        {
        }
    }
}

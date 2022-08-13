// Decompiled with JetBrains decompiler
// Type: DuckGame.Grapple
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("previewPriority", true)]
    public class Grapple : Equipment, ISwing
    {
        public StateBinding _ropeDataBinding = new DataBinding(nameof(ropeData));
        public BitBuffer ropeData = new BitBuffer();
        protected SpriteMap _sprite;
        public Harpoon _harpoon;
        public Rope _rope;
        protected Vec2 _barrelOffsetTL;
        private float _grappleLength = 200f;
        private Tex2D _laserTex;
        public Sprite _ropeSprite;
        protected Vec2 _wallPoint;
        private Vec2 _lastHit = Vec2.Zero;
        protected Vec2 _grappleTravel;
        protected Sprite _sightHit;
        private float _grappleDist;
        private bool _canGrab;
        private int _lagFrames;

        public Vec2 barrelPosition => Offset(barrelOffset);

        public Vec2 barrelOffset => _barrelOffsetTL - center;

        public bool hookInGun => _harpoon.inGun;

        public Grapple(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("grappleArm", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-5f, -4f);
            collisionSize = new Vec2(11f, 7f);
            _offset = new Vec2(0f, 7f);
            _equippedDepth = 12;
            _barrelOffsetTL = new Vec2(10f, 4f);
            _jumpMod = true;
            thickness = 0.1f;
            _laserTex = Content.Load<Tex2D>("pointerLaser");
            editorTooltip = "Allows you to swing from platforms like some kind of loon.";
        }

        public override void OnTeleport() => Degrapple();

        public override void OnPressAction()
        {
        }

        public override void OnReleaseAction()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            _harpoon = new Harpoon(this);
            Level.Add(_harpoon);
            _sightHit = new Sprite("laserSightHit");
            _sightHit.CenterOrigin();
            _ropeSprite = new Sprite("grappleWire")
            {
                center = new Vec2(8f, 0f)
            };
        }

        public Rope GetRopeParent(Thing child)
        {
            for (Rope ropeParent = _rope; ropeParent != null; ropeParent = ropeParent.attach2 as Rope)
            {
                if (ropeParent.attach2 == child)
                    return ropeParent;
            }
            return null;
        }

        public void SerializeRope(Rope r)
        {
            if (r != null)
            {
                ropeData.Write(true);
                ropeData.Write(CompressedVec2Binding.GetCompressedVec2(r.attach2Point));
                SerializeRope(r.attach2 as Rope);
            }
            else
                ropeData.Write(false);
        }

        public void DeserializeRope(Rope r)
        {
            if (ropeData.ReadBool())
            {
                if (r == null)
                {
                    _rope = new Rope(0f, 0f, r, null, tex: _ropeSprite, belongsTo: this);
                    r = _rope;
                }
                r.attach1 = r;
                r._thing = null;
                Level.Add(r);
                Vec2 uncompressedVec2 = CompressedVec2Binding.GetUncompressedVec2(ropeData.ReadInt());
                if (r == _rope)
                {
                    r.attach1 = r;
                    if (duck != null)
                        r.position = duck.position;
                    else
                        r.position = position;
                    r._thing = duck;
                }
                if (r.attach2 == null || !(r.attach2 is Rope) || r.attach2 == r)
                {
                    Rope rope = new Rope(uncompressedVec2.x, uncompressedVec2.y, r, null, tex: _ropeSprite, belongsTo: this);
                    r.attach2 = rope;
                }
                if (r.attach2 != null)
                {
                    r.attach2.position = uncompressedVec2;
                    (r.attach2 as Rope).attach1 = r;
                }
                DeserializeRope(r.attach2 as Rope);
            }
            else if (r == _rope)
            {
                Degrapple();
            }
            else
            {
                if (r == null)
                    return;
                Rope attach1 = r.attach1 as Rope;
                attach1.TerminateLaterRopes();
                _harpoon.Latch(r.position);
                attach1.attach2 = _harpoon;
            }
        }

        public void Degrapple()
        {
            _harpoon.Return();
            if (_rope != null)
                _rope.RemoveRope();
            if (_rope != null && duck != null)
            {
                duck.frictionMult = 1f;
                duck.gravMultiplier = 1f;
                duck._double = false;
                if (duck.vSpeed < 0.0 && duck.framesSinceJump > 3)
                    duck.vSpeed *= 1.75f;
                if (duck.vSpeed >= duck.jumpSpeed * 0.95f && Math.Abs(duck.vSpeed) + Math.Abs(duck.hSpeed) < 2.0f)
                {
                    SFX.Play("jump", 0.5f);
                    duck.vSpeed = duck.jumpSpeed;
                }
            }
            _rope = null;
            frictionMult = 1f;
            gravMultiplier = 1f;
        }

        public Vec2 wallPoint => _wallPoint;

        public Vec2 grappelTravel => _grappleTravel;

        public override void Update()
        {
            if (_harpoon == null)
                return;
            if (isServerForObject)
            {
                ropeData.Clear();
                SerializeRope(_rope);
            }
            else
            {
                ropeData.SeekToStart();
                DeserializeRope(_rope);
            }
            if (_rope != null)
                _rope.SetServer(isServerForObject);
            if (isServerForObject && _equippedDuck != null && duck != null)
            {
                if (duck._trapped != null)
                    Degrapple();
                ATTracer type = new ATTracer();
                float num = type.range = _grappleLength;
                type.penetration = 1f;
                float ang = 45f;
                if (offDir < 0)
                    ang = 135f;
                if (_harpoon.inGun)
                {
                    Vec2 p1 = Offset(barrelOffset);
                    if (_lagFrames > 0)
                    {
                        --_lagFrames;
                        if (_lagFrames == 0)
                            _canGrab = false;
                        else
                            ang = Maths.PointDirection(p1, _lastHit);
                    }
                    type.penetration = 9f;
                    Bullet bullet = new Bullet(p1.x, p1.y, type, ang, owner, tracer: true);
                    _wallPoint = bullet.end;
                    _grappleTravel = bullet.travelDirNormalized;
                    num = (p1 - _wallPoint).length;
                }
                if (num < _grappleLength - 2.0 && num <= _grappleDist + 16.0)
                {
                    _lastHit = _wallPoint;
                    _canGrab = true;
                }
                else if (_canGrab && _lagFrames == 0)
                {
                    _lagFrames = 6;
                    _wallPoint = _lastHit;
                }
                else
                    _canGrab = false;
                _grappleDist = num;
                if (duck.inputProfile.Pressed("JUMP") && duck._trapped == null)
                {
                    if (_harpoon.inGun)
                    {
                        if (!duck.grounded && duck.framesSinceJump > 6 && _canGrab && (!(duck.holdObject is TV) || (duck.holdObject as TV)._ruined || !(duck.holdObject as TV).channel || !duck._double || duck._groundValid <= 0))
                        {
                            RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.Short));
                            _harpoon.Fire(wallPoint, grappelTravel);
                            _rope = new Rope(barrelPosition.x, barrelPosition.y, null, _harpoon, duck, tex: _ropeSprite, belongsTo: this);
                            Level.Add(_rope);
                        }
                    }
                    else
                    {
                        Degrapple();
                        _lagFrames = 0;
                        _canGrab = false;
                    }
                }
            }
            base.Update();
            if (owner != null)
                offDir = owner.offDir;
            if (duck != null)
                duck.grappleMul = false;
            if (!isServerForObject || _rope == null)
                return;
            if (owner != null)
            {
                _rope.position = owner.position;
            }
            else
            {
                _rope.position = position;
                if (prevOwner != null)
                {
                    PhysicsObject prevOwner = this.prevOwner as PhysicsObject;
                    prevOwner.frictionMult = 1f;
                    prevOwner.gravMultiplier = 1f;
                    _prevOwner = null;
                    frictionMult = 1f;
                    gravMultiplier = 1f;
                    if (this.prevOwner is Duck)
                        (this.prevOwner as Duck).grappleMul = false;
                }
            }
            if (!_harpoon.stuck)
                return;
            if (duck != null)
            {
                if (!duck.grounded)
                {
                    duck.frictionMult = 0f;
                }
                else
                {
                    duck.frictionMult = 1f;
                    duck.gravMultiplier = 1f;
                }
                if (_rope.properLength > 0.0)
                {
                    if (duck.inputProfile.Down("UP") && _rope.properLength >= 16.0)
                        _rope.properLength -= 2f;
                    if (duck.inputProfile.Down("DOWN") && _rope.properLength <= 256.0)
                        _rope.properLength += 2f;
                    _rope.properLength = Maths.Clamp(_rope.properLength, 16f, 256f);
                }
            }
            else if (!grounded)
            {
                frictionMult = 0f;
            }
            else
            {
                frictionMult = 1f;
                gravMultiplier = 1f;
            }
            Vec2 vec2_1 = _rope.attach1.position - _rope.attach2.position;
            if (_rope.properLength < 0.0)
                _rope.properLength = _rope.startLength = vec2_1.length;
            if (vec2_1.length <= _rope.properLength)
                return;
            vec2_1 = vec2_1.normalized;
            if (duck != null)
            {
                this.duck.grappleMul = true;
                PhysicsObject duck = this.duck;
                if (this.duck.ragdoll != null)
                {
                    Degrapple();
                }
                else
                {
                    Vec2 position = duck.position;
                    duck.position = _rope.attach2.position + vec2_1 * _rope.properLength;
                    Vec2 vec2_2 = duck.position - duck.lastPosition;
                    duck.hSpeed = vec2_2.x;
                    duck.vSpeed = vec2_2.y;
                }
            }
            else
            {
                position = _rope.attach2.position + vec2_1 * _rope.properLength;
                Vec2 vec2_3 = position - lastPosition;
                hSpeed = vec2_3.x;
                vSpeed = vec2_3.y;
            }
        }

        public override void Draw()
        {
            if (_equippedDuck == null)
                base.Draw();
            else if (_autoOffset)
            {
                Vec2 offset = _offset;
                if (_equippedDuck.offDir < 0)
                    offset.x *= -1f;
                Vec2 vec2 = Vec2.Zero;
                if (_equippedDuck.holdObject != null)
                {
                    vec2 = _equippedDuck.holdObject.handOffset;
                    vec2.x *= _equippedDuck.offDir;
                }
                position = _equippedDuck.armPosition + vec2;
            }
            if (Options.Data.fireGlow)
                return;
            DrawGlow();
        }

        public override void DrawGlow()
        {
            if (_equippedDuck != null && _harpoon != null && _harpoon.inGun && _canGrab && _equippedDuck._trapped == null)
            {
                Graphics.DrawTexturedLine(_laserTex, Offset(barrelOffset), _wallPoint, Color.Red, 0.5f, depth - 1);
                if (_sightHit != null)
                {
                    _sightHit.color = Color.Red;
                    Graphics.Draw(_sightHit, _wallPoint.x, _wallPoint.y);
                }
            }
            base.DrawGlow();
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.Grapple
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        public StateBinding _ropeDataBinding = (StateBinding)new DataBinding(nameof(ropeData));
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

        public Vec2 barrelPosition => this.Offset(this.barrelOffset);

        public Vec2 barrelOffset => this._barrelOffsetTL - this.center;

        public bool hookInGun => this._harpoon.inGun;

        public Grapple(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("grappleArm", 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-5f, -4f);
            this.collisionSize = new Vec2(11f, 7f);
            this._offset = new Vec2(0.0f, 7f);
            this._equippedDepth = 12;
            this._barrelOffsetTL = new Vec2(10f, 4f);
            this._jumpMod = true;
            this.thickness = 0.1f;
            this._laserTex = Content.Load<Tex2D>("pointerLaser");
            this.editorTooltip = "Allows you to swing from platforms like some kind of loon.";
        }

        public override void OnTeleport() => this.Degrapple();

        public override void OnPressAction()
        {
        }

        public override void OnReleaseAction()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this._harpoon = new Harpoon((Thing)this);
            Level.Add((Thing)this._harpoon);
            this._sightHit = new Sprite("laserSightHit");
            this._sightHit.CenterOrigin();
            this._ropeSprite = new Sprite("grappleWire");
            this._ropeSprite.center = new Vec2(8f, 0.0f);
        }

        public Rope GetRopeParent(Thing child)
        {
            for (Rope ropeParent = this._rope; ropeParent != null; ropeParent = ropeParent.attach2 as Rope)
            {
                if (ropeParent.attach2 == child)
                    return ropeParent;
            }
            return (Rope)null;
        }

        public void SerializeRope(Rope r)
        {
            if (r != null)
            {
                this.ropeData.Write(true);
                this.ropeData.Write(CompressedVec2Binding.GetCompressedVec2(r.attach2Point));
                this.SerializeRope(r.attach2 as Rope);
            }
            else
                this.ropeData.Write(false);
        }

        public void DeserializeRope(Rope r)
        {
            if (this.ropeData.ReadBool())
            {
                if (r == null)
                {
                    this._rope = new Rope(0.0f, 0.0f, (Thing)r, (Thing)null, tex: this._ropeSprite, belongsTo: ((Thing)this));
                    r = this._rope;
                }
                r.attach1 = (Thing)r;
                r._thing = (Thing)null;
                Level.Add((Thing)r);
                Vec2 uncompressedVec2 = CompressedVec2Binding.GetUncompressedVec2(this.ropeData.ReadInt());
                if (r == this._rope)
                {
                    r.attach1 = (Thing)r;
                    if (this.duck != null)
                        r.position = this.duck.position;
                    else
                        r.position = this.position;
                    r._thing = (Thing)this.duck;
                }
                if (r.attach2 == null || !(r.attach2 is Rope) || r.attach2 == r)
                {
                    Rope rope = new Rope(uncompressedVec2.x, uncompressedVec2.y, (Thing)r, (Thing)null, tex: this._ropeSprite, belongsTo: ((Thing)this));
                    r.attach2 = (Thing)rope;
                }
                if (r.attach2 != null)
                {
                    r.attach2.position = uncompressedVec2;
                    (r.attach2 as Rope).attach1 = (Thing)r;
                }
                this.DeserializeRope(r.attach2 as Rope);
            }
            else if (r == this._rope)
            {
                this.Degrapple();
            }
            else
            {
                if (r == null)
                    return;
                Rope attach1 = r.attach1 as Rope;
                attach1.TerminateLaterRopes();
                this._harpoon.Latch(r.position);
                attach1.attach2 = (Thing)this._harpoon;
            }
        }

        public void Degrapple()
        {
            this._harpoon.Return();
            if (this._rope != null)
                this._rope.RemoveRope();
            if (this._rope != null && this.duck != null)
            {
                this.duck.frictionMult = 1f;
                this.duck.gravMultiplier = 1f;
                this.duck._double = false;
                if ((double)this.duck.vSpeed < 0.0 && this.duck.framesSinceJump > 3)
                    this.duck.vSpeed *= 1.75f;
                if ((double)this.duck.vSpeed >= (double)this.duck.jumpSpeed * 0.949999988079071 && (double)Math.Abs(this.duck.vSpeed) + (double)Math.Abs(this.duck.hSpeed) < 2.0)
                {
                    SFX.Play("jump", 0.5f);
                    this.duck.vSpeed = this.duck.jumpSpeed;
                }
            }
            this._rope = (Rope)null;
            this.frictionMult = 1f;
            this.gravMultiplier = 1f;
        }

        public Vec2 wallPoint => this._wallPoint;

        public Vec2 grappelTravel => this._grappleTravel;

        public override void Update()
        {
            if (this._harpoon == null)
                return;
            if (this.isServerForObject)
            {
                this.ropeData.Clear();
                this.SerializeRope(this._rope);
            }
            else
            {
                this.ropeData.SeekToStart();
                this.DeserializeRope(this._rope);
            }
            if (this._rope != null)
                this._rope.SetServer(this.isServerForObject);
            if (this.isServerForObject && this._equippedDuck != null && this.duck != null)
            {
                if (this.duck._trapped != null)
                    this.Degrapple();
                ATTracer type = new ATTracer();
                float num = type.range = this._grappleLength;
                type.penetration = 1f;
                float ang = 45f;
                if (this.offDir < (sbyte)0)
                    ang = 135f;
                if (this._harpoon.inGun)
                {
                    Vec2 p1 = this.Offset(this.barrelOffset);
                    if (this._lagFrames > 0)
                    {
                        --this._lagFrames;
                        if (this._lagFrames == 0)
                            this._canGrab = false;
                        else
                            ang = Maths.PointDirection(p1, this._lastHit);
                    }
                    type.penetration = 9f;
                    Bullet bullet = new Bullet(p1.x, p1.y, (AmmoType)type, ang, this.owner, tracer: true);
                    this._wallPoint = bullet.end;
                    this._grappleTravel = bullet.travelDirNormalized;
                    num = (p1 - this._wallPoint).length;
                }
                if ((double)num < (double)this._grappleLength - 2.0 && (double)num <= (double)this._grappleDist + 16.0)
                {
                    this._lastHit = this._wallPoint;
                    this._canGrab = true;
                }
                else if (this._canGrab && this._lagFrames == 0)
                {
                    this._lagFrames = 6;
                    this._wallPoint = this._lastHit;
                }
                else
                    this._canGrab = false;
                this._grappleDist = num;
                if (this.duck.inputProfile.Pressed("JUMP") && this.duck._trapped == null)
                {
                    if (this._harpoon.inGun)
                    {
                        if (!this.duck.grounded && this.duck.framesSinceJump > 6 && this._canGrab && (!(this.duck.holdObject is TV) || (this.duck.holdObject as TV)._ruined || !(this.duck.holdObject as TV).channel || !this.duck._double || this.duck._groundValid <= 0))
                        {
                            RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.Short));
                            this._harpoon.Fire(this.wallPoint, this.grappelTravel);
                            this._rope = new Rope(this.barrelPosition.x, this.barrelPosition.y, (Thing)null, (Thing)this._harpoon, (Thing)this.duck, tex: this._ropeSprite, belongsTo: ((Thing)this));
                            Level.Add((Thing)this._rope);
                        }
                    }
                    else
                    {
                        this.Degrapple();
                        this._lagFrames = 0;
                        this._canGrab = false;
                    }
                }
            }
            base.Update();
            if (this.owner != null)
                this.offDir = this.owner.offDir;
            if (this.duck != null)
                this.duck.grappleMul = false;
            if (!this.isServerForObject || this._rope == null)
                return;
            if (this.owner != null)
            {
                this._rope.position = this.owner.position;
            }
            else
            {
                this._rope.position = this.position;
                if (this.prevOwner != null)
                {
                    PhysicsObject prevOwner = this.prevOwner as PhysicsObject;
                    prevOwner.frictionMult = 1f;
                    prevOwner.gravMultiplier = 1f;
                    this._prevOwner = (Thing)null;
                    this.frictionMult = 1f;
                    this.gravMultiplier = 1f;
                    if (this.prevOwner is Duck)
                        (this.prevOwner as Duck).grappleMul = false;
                }
            }
            if (!this._harpoon.stuck)
                return;
            if (this.duck != null)
            {
                if (!this.duck.grounded)
                {
                    this.duck.frictionMult = 0.0f;
                }
                else
                {
                    this.duck.frictionMult = 1f;
                    this.duck.gravMultiplier = 1f;
                }
                if ((double)this._rope.properLength > 0.0)
                {
                    if (this.duck.inputProfile.Down("UP") && (double)this._rope.properLength >= 16.0)
                        this._rope.properLength -= 2f;
                    if (this.duck.inputProfile.Down("DOWN") && (double)this._rope.properLength <= 256.0)
                        this._rope.properLength += 2f;
                    this._rope.properLength = Maths.Clamp(this._rope.properLength, 16f, 256f);
                }
            }
            else if (!this.grounded)
            {
                this.frictionMult = 0.0f;
            }
            else
            {
                this.frictionMult = 1f;
                this.gravMultiplier = 1f;
            }
            Vec2 vec2_1 = this._rope.attach1.position - this._rope.attach2.position;
            if ((double)this._rope.properLength < 0.0)
                this._rope.properLength = this._rope.startLength = vec2_1.length;
            if ((double)vec2_1.length <= (double)this._rope.properLength)
                return;
            vec2_1 = vec2_1.normalized;
            if (this.duck != null)
            {
                this.duck.grappleMul = true;
                PhysicsObject duck = (PhysicsObject)this.duck;
                if (this.duck.ragdoll != null)
                {
                    this.Degrapple();
                }
                else
                {
                    Vec2 position = duck.position;
                    duck.position = this._rope.attach2.position + vec2_1 * this._rope.properLength;
                    Vec2 vec2_2 = duck.position - duck.lastPosition;
                    duck.hSpeed = vec2_2.x;
                    duck.vSpeed = vec2_2.y;
                }
            }
            else
            {
                Vec2 position = this.position;
                this.position = this._rope.attach2.position + vec2_1 * this._rope.properLength;
                Vec2 vec2_3 = this.position - this.lastPosition;
                this.hSpeed = vec2_3.x;
                this.vSpeed = vec2_3.y;
            }
        }

        public override void Draw()
        {
            if (this._equippedDuck == null)
                base.Draw();
            else if (this._autoOffset)
            {
                Vec2 offset = this._offset;
                if (this._equippedDuck.offDir < (sbyte)0)
                    offset.x *= -1f;
                Vec2 vec2 = Vec2.Zero;
                if (this._equippedDuck.holdObject != null)
                {
                    vec2 = this._equippedDuck.holdObject.handOffset;
                    vec2.x *= (float)this._equippedDuck.offDir;
                }
                this.position = this._equippedDuck.armPosition + vec2;
            }
            if (Options.Data.fireGlow)
                return;
            this.DrawGlow();
        }

        public override void DrawGlow()
        {
            if (this._equippedDuck != null && this._harpoon != null && this._harpoon.inGun && this._canGrab && this._equippedDuck._trapped == null)
            {
                Graphics.DrawTexturedLine(this._laserTex, this.Offset(this.barrelOffset), this._wallPoint, Color.Red, 0.5f, this.depth - 1);
                if (this._sightHit != null)
                {
                    this._sightHit.color = Color.Red;
                    Graphics.Draw(this._sightHit, this._wallPoint.x, this._wallPoint.y);
                }
            }
            base.DrawGlow();
        }
    }
}

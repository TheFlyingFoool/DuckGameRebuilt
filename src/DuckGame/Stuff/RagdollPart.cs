// Decompiled with JetBrains decompiler
// Type: DuckGame.RagdollPart
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isInDemo", true)]
    public class RagdollPart : Holdable, IAmADuck
    {
        public StateBinding _dollBinding = new StateBinding(nameof(_doll));
        public StateBinding _connectBinding = new StateBinding(nameof(connect));
        public StateBinding _jointBinding = new StateBinding(nameof(_joint));
        public StateBinding _partBinding = new StateBinding(nameof(netPart), 2);
        public StateBinding _framesSinceGroundedBinding = new StateBinding("framesSinceGrounded", 4);
        private bool _setting;
        public float extraGravMultiplier = 1f;
        public Vec2 _lastReasonablePosition;
        private SpriteMap _sprite;
        private SpriteMap _quackSprite;
        public RagdollPart _joint;
        public RagdollPart connect;
        public int clipFrames;
        private DuckPersona _prevPersona;
        private int _part;
        public float addWeight;
        private bool _zekeBear;
        private DuckPersona _rlPersona;
        public Ragdoll _doll;
        private SpriteMap _campDuck;
        private bool extinguishing;
        private bool _setSkipClip;
        private int _ownTime;
        private Vec2 _stickLerp;
        private Vec2 _stickSlowLerp;

        public override NetworkConnection connection
        {
            get => this._doll != null ? this._doll.connection : base.connection;
            set
            {
                if (this._setting)
                    return;
                this._setting = true;
                if (this._doll != null)
                    this._doll.connection = value;
                base.connection = value;
                this._setting = false;
            }
        }

        public override NetIndex8 authority
        {
            get => this._doll != null ? this._doll.authority : base.authority;
            set
            {
                if (this._setting)
                    return;
                this._setting = true;
                if (this._doll != null)
                    this._doll.authority = value;
                base.authority = value;
                this._setting = false;
            }
        }

        public override float currentGravity => PhysicsObject.gravity * this.gravMultiplier * this.floatMultiplier * this.extraGravMultiplier;

        public byte netPart
        {
            get => (byte)this._part;
            set => this.part = (int)value;
        }

        public RagdollPart joint
        {
            get => this._joint;
            set => this._joint = value;
        }

        public int part
        {
            get => this._part;
            set
            {
                int part = this._part;
                this._part = value;
                if (this._doll != null && this._doll._duck != null)
                    this._persona = this._doll._duck.persona;
                if (this.part == 0)
                    this.center = new Vec2(16f, 13f);
                else if (this.part == 1)
                    this.center = new Vec2(16f, 13f);
                else if (this.part == 3)
                    this.center = new Vec2(6f, 8f);
                else
                    this.center = new Vec2(8f, 8f);
                if (this.part == 0 || this.part == 1)
                {
                    if (this.part == 0)
                    {
                        this.collisionOffset = new Vec2(-4f, -5f);
                        this.collisionSize = new Vec2(8f, 10f);
                    }
                    else
                    {
                        this.collisionOffset = new Vec2(-4f, -5f);
                        this.collisionSize = new Vec2(8f, 10f);
                    }
                }
                else
                {
                    this.collisionOffset = new Vec2(-1f, -1f);
                    this.collisionSize = new Vec2(2f, 2f);
                }
                if (this._persona == null || this._prevPersona == this._persona && part == this._part)
                    return;
                this._quackSprite = this._persona.quackSprite.CloneMap();
                this._sprite = this._persona.sprite.CloneMap();
                this._quackSprite.frame = 18;
                this._sprite.frame = 18;
                if (part != this._part || this.graphic == null)
                    this.graphic = (Sprite)this._sprite;
                this._quackSprite.frame = this._part == 0 ? 18 : 19;
                this._sprite.frame = this._part == 0 ? 18 : 19;
                if (this.doll != null && this.doll.captureDuck != null && this.doll.captureDuck.eyesClosed)
                {
                    this._quackSprite.frame = this._part == 0 ? 20 : 19;
                    this._sprite.frame = this._part == 0 ? 20 : 19;
                }
                this._prevPersona = this._persona;
            }
        }

        public override float weight
        {
            get => this._weight + this.addWeight;
            set => this._weight = value;
        }

        public override void Zap(Thing zapper)
        {
            if (this._doll != null)
                this._doll.Zap(zapper);
            base.Zap(zapper);
        }

        public void MakeZekeBear()
        {
            this._quackSprite = new SpriteMap("teddy", 32, 32);
            this._sprite = this._quackSprite;
            this._quackSprite.frame = 0;
            this._sprite.frame = 0;
            this.graphic = (Sprite)this._sprite;
            this._quackSprite.frame = this._part == 0 ? 0 : 1;
            this._sprite.frame = this._part == 0 ? 0 : 1;
            if (this.part == 0)
                this.center = new Vec2(16f, 16f);
            else if (this.part == 1)
                this.center = new Vec2(16f, 13f);
            else if (this.part == 3)
                this.center = new Vec2(6f, 8f);
            else
                this.center = new Vec2(8f, 8f);
            this._zekeBear = true;
        }

        public DuckPersona _persona
        {
            get => this._rlPersona;
            set => this._rlPersona = value;
        }

        public Ragdoll doll
        {
            get => this._doll;
            set => this._doll = value;
        }

        public RagdollPart(
          float xpos,
          float ypos,
          int p,
          DuckPersona persona,
          int off,
          Ragdoll doll)
          : base(xpos, ypos)
        {
            if (persona == null)
                persona = Persona.Duck1;
            this._sprite = new SpriteMap("crate", 16, 16);
            this._campDuck = new SpriteMap("campduck", 32, 32);
            this.graphic = (Sprite)this._sprite;
            this._editorName = "Crate";
            this.thickness = 0.5f;
            this.weight = 0.05f;
            this.bouncy = 0.6f;
            this._holdOffset = new Vec2(2f, 0.0f);
            this.flammable = 0.3f;
            this.tapeable = false;
            this.SortOutDetails(xpos, ypos, p, persona, off, doll);
        }

        public override void Extinquish()
        {
            if (this.extinguishing)
                return;
            this.extinguishing = true;
            if (this.doll != null && this.doll.captureDuck != null)
                this.doll.captureDuck.Extinquish();
            base.Extinquish();
            this.extinguishing = false;
        }

        public void SortOutDetails(
          float xpos,
          float ypos,
          int p,
          DuckPersona persona,
          int off,
          Ragdoll doll)
        {
            this.x = xpos;
            this.y = ypos;
            this.hSpeed = 0.0f;
            this.vSpeed = 0.0f;
            this._part = this.part;
            this.offDir = (sbyte)off;
            this.airFrictionMult = 0.3f;
            this._persona = persona;
            this._doll = doll;
            this.part = p;
        }

        public override void OnTeleport()
        {
            this.position.x += (float)(Math.Sign(this.hSpeed) * 8);
            this.doll.part1.position = this.position;
            this.doll.part2.position = this.position;
            this.doll.part3.position = this.position;
            this.doll.part1.hSpeed = this.hSpeed;
            this.doll.part2.hSpeed = this.hSpeed;
            this.doll.part3.hSpeed = this.hSpeed;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this._doll == null)
                return false;
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle((Thing)this._doll, DuckNetwork.localConnection);
            if (bullet.isLocal && this._doll.captureDuck != null)
            {
                Duck captureDuck = this._doll.captureDuck;
                Equipment equipment1 = captureDuck.GetEquipment(typeof(ChestPlate));
                if (equipment1 != null && Collision.Point(hitPos, (Thing)equipment1))
                {
                    equipment1.UnEquip();
                    SFX.Play("ting2");
                    captureDuck.Unequip(equipment1);
                    equipment1.hSpeed = bullet.travelDirNormalized.x;
                    equipment1.vSpeed = -2f;
                    equipment1.Destroy((DestroyType)new DTShot(bullet));
                    equipment1.solid = false;
                    return true;
                }
                Equipment equipment2 = captureDuck.GetEquipment(typeof(Helmet));
                if (equipment2 != null && Collision.Point(hitPos, (Thing)equipment2))
                {
                    equipment2.UnEquip();
                    SFX.Play("ting2");
                    captureDuck.Unequip(equipment2);
                    equipment2.hSpeed = bullet.travelDirNormalized.x;
                    equipment2.vSpeed = -2f;
                    equipment2.Destroy((DestroyType)new DTShot(bullet));
                    equipment2.solid = false;
                    return true;
                }
            }
            Feather feather = Feather.New(0.0f, 0.0f, this._persona);
            feather.hSpeed = (float)(-(double)bullet.travelDirNormalized.x * (1.0 + (double)Rando.Float(1f)));
            feather.vSpeed = -Rando.Float(2f);
            feather.position = hitPos;
            Level.Add((Thing)feather);
            if (bullet.isLocal)
            {
                this.hSpeed += bullet.travelDirNormalized.x * bullet.ammo.impactPower;
                this.vSpeed += bullet.travelDirNormalized.y * bullet.ammo.impactPower;
                SFX.Play("thwip", pitch: Rando.Float(-0.1f, 0.1f));
                this._doll.Shot(bullet);
            }
            return base.Hit(bullet, hitPos);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this._doll == null)
                return false;
            if (type is DTIncinerate)
            {
                if (!this._doll.removeFromLevel && this._doll.captureDuck != null && this._doll.captureDuck.dead)
                {
                    CookedDuck t = new CookedDuck(this._doll.x, this._doll.y);
                    Level.Add((Thing)SmallSmoke.New(this._doll.x + Rando.Float(-4f, 4f), this._doll.y + Rando.Float(-4f, 4f)));
                    Level.Add((Thing)SmallSmoke.New(this._doll.x + Rando.Float(-4f, 4f), this._doll.y + Rando.Float(-4f, 4f)));
                    Level.Add((Thing)SmallSmoke.New(this._doll.x + Rando.Float(-4f, 4f), this._doll.y + Rando.Float(-4f, 4f)));
                    this.ReturnItemToWorld((Thing)t);
                    t.vSpeed = this.vSpeed - 2f;
                    t.hSpeed = this.hSpeed;
                    Level.Add((Thing)t);
                    SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                    Level.Remove((Thing)this._doll);
                    this._doll.captureDuck._cooked = t;
                }
                else
                {
                    if (this._doll.captureDuck == null)
                        return false;
                    this._doll.captureDuck.Kill(type);
                    return true;
                }
            }
            if (!this.destroyed)
                this._doll.Killed(type);
            return false;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._doll.captureDuck != null && !this._doll.captureDuck.dead && (this.part == 0 || this.part == 1) && (double)this.totalImpactPower > 5.0 && (double)this._doll.captureDuck.quack < 0.25)
            {
                this._doll.captureDuck.Swear();
                float intensityToSet = Math.Min(this.totalImpactPower * 0.01f, 1f);
                if ((double)intensityToSet > 0.0500000007450581)
                    RumbleManager.AddRumbleEvent(this._doll.captureDuck.profile, new RumbleEvent(intensityToSet, 0.05f, 0.6f));
            }
            base.OnSolidImpact(with, from);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._doll == null || !this.isServerForObject || !with.isServerForObject || with is RagdollPart || with is FeatherVolume || with == this.owner || with == this._doll.holdingOwner || with == this._doll.captureDuck)
                return;
            if (with is Duck)
            {
                Holdable lastHoldItem = (with as Duck)._lastHoldItem;
                if ((with as Duck)._timeSinceThrow < (byte)15 && (lastHoldItem == this._doll.part1 || lastHoldItem == this._doll.part2 || lastHoldItem == this._doll.part3))
                    return;
            }
            if (this._doll.captureDuck == null)
                return;
            Vec2 position = this._doll.captureDuck.position;
            this._doll.captureDuck.collisionOffset = this.collisionOffset;
            this._doll.captureDuck.collisionSize = this.collisionSize;
            this._doll.captureDuck.position = this.position;
            this._doll.captureDuck.OnSoftImpact(with, from);
            this._doll.captureDuck.position = position;
        }

        public void UpdateLastReasonablePosition(Vec2 pPosition)
        {
            if ((double)pPosition.y <= -7000.0 || (double)pPosition.y >= (double)Level.activeLevel.lowestPoint + 400.0)
                return;
            this._lastReasonablePosition = pPosition;
        }

        public override void Update()
        {
            if (this._doll == null || (double)this.y > (double)Level.activeLevel.lowestPoint + 1000.0 && this.isOffBottomOfLevel)
                return;
            this.UpdateLastReasonablePosition(this.position);
            if (this.clipFrames > 0)
                --this.clipFrames;
            if (this.owner != null && this._doll != null && !this.doll.inSleepingBag)
            {
                ++this._ownTime;
                if (this._ownTime > 20)
                {
                    this._doll.ShakeOutOfSleepingBag();
                    this._ownTime = 0;
                }
            }
            if (this._doll.captureDuck != null)
            {
                if (this._zekeBear)
                {
                    if (this._part == 0)
                        this.depth = this._doll.captureDuck.depth + 2;
                    else
                        this.depth = this._doll.captureDuck.depth;
                }
                else if (this._part == 0)
                {
                    this.depth = this._doll.captureDuck.depth - 10;
                    if (this._doll.part3 != null)
                        this.depth = this._doll.part3.depth - 10;
                }
                else
                    this.depth = this._doll.captureDuck.depth;
                this.canPickUp = true;
                if (this._doll.captureDuck.HasEquipment(typeof(ChokeCollar)) && this._part != 0)
                    this.canPickUp = false;
            }
            if (this._joint != null && this.connect != null)
            {
                if (this.owner == null && this.prevOwner != null)
                {
                    this.clip.Add((MaterialThing)(this.prevOwner as PhysicsObject));
                    this.connect.clip.Add((MaterialThing)(this.prevOwner as PhysicsObject));
                    this._joint.clipFrames = 12;
                    this._joint.clipThing = this._prevOwner;
                    this._prevOwner = (Thing)null;
                }
                if (this.owner != null)
                {
                    this._joint.clipFrames = 0;
                    this._joint.depth = this.depth;
                }
                if (this.owner != null || this._joint.owner != null)
                    this.weight = 0.1f;
                else
                    this.weight = 4f;
                if (this._zekeBear)
                {
                    if (this.owner != null || this._joint.owner != null)
                        this.weight = 0.1f;
                    else
                        this.weight = 0.2f;
                }
                if (this._joint.clipFrames > 0)
                {
                    this.skipClip = true;
                    this._setSkipClip = true;
                }
                else if (this._setSkipClip)
                {
                    this.skipClip = false;
                    this._setSkipClip = false;
                }
            }
            if (this._part > 1)
                this.canPickUp = false;
            base.Update();
            if (this._doll.captureDuck != null && this._doll.captureDuck.HasEquipment(typeof(FancyShoes)) && this._part == 0 && this._doll.captureDuck.holdObject != null)
            {
                this._doll.captureDuck.holdObject.position = this.Offset(new Vec2(3f, 5f) + this._doll.captureDuck.holdObject.holdOffset);
                this._doll.captureDuck.holdObject.angle = this.angle;
                if (this._doll.captureDuck.holdObject != null && this._doll.captureDuck.isServerForObject)
                {
                    this._doll.captureDuck.holdObject.isLocal = this.isLocal;
                    this._doll.captureDuck.holdObject.UpdateAction();
                }
            }
            if (this.doll != null && this.doll.part3 != null)
            {
                this.offDir = this.doll.part3.offDir;
                if (this.doll.captureDuck != null && this.enablePhysics)
                {
                    this.doll.captureDuck.offDir = this.offDir;
                    if (this.owner != null)
                        this.doll.captureDuck._prevOwner = this.owner;
                }
            }
            FluidPuddle fluidPuddle = Level.CheckPoint<FluidPuddle>(this.position + new Vec2(0.0f, 4f));
            if (fluidPuddle != null)
            {
                if ((double)this.y + 4.0 - (double)fluidPuddle.top > 8.0)
                {
                    this.gravMultiplier = -0.5f;
                    this.grounded = false;
                }
                else
                {
                    if ((double)this.y + 4.0 - (double)fluidPuddle.top < 3.0)
                    {
                        this.gravMultiplier = 0.2f;
                        this.grounded = true;
                    }
                    else if ((double)this.y + 4.0 - (double)fluidPuddle.top > 4.0)
                    {
                        this.gravMultiplier = -0.2f;
                        this.grounded = true;
                    }
                    this.grounded = true;
                }
            }
            else
                this.gravMultiplier = 1f;
            if (this._joint != null)
            {
                if (this._doll.captureDuck != null && this._doll.captureDuck.IsQuacking())
                    this.graphic = (Sprite)this._quackSprite;
                else
                    this.graphic = (Sprite)this._sprite;
                if (this.isServerForObject)
                {
                    if (this.offDir < (sbyte)0)
                        this.angleDegrees = (float)(-(double)Maths.PointDirection(this.position, this._joint.position) + 180.0 + 90.0);
                    else
                        this.angleDegrees = (float)(-(double)Maths.PointDirection(this.position, this._joint.position) - 90.0);
                }
            }
            if (this._part == 3 && this.connect != null)
            {
                this.angleDegrees = (float)(-(double)Maths.PointDirection(this.position, this.connect.position) + 180.0);
                this.depth = this.connect.depth + 2;
            }
            this.visible = this._part != 2;
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (!this._onFire)
            {
                SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                for (int index = 0; index < 2; ++index)
                    Level.Add((Thing)SmallFire.New(Rando.Float(6f) - 3f, Rando.Float(2f) - 2f, 0.0f, 0.0f, stick: ((MaterialThing)this)));
                this._onFire = true;
                this._doll.LitOnFire(litBy);
            }
            return true;
        }

        public override void Draw()
        {
            this.addWeight = 0.0f;
            this.extraGravMultiplier = 1f;
            if (this._part == 2 || this._joint == null)
                return;
            Vec2 position = this.position;
            Vec2 vec2_1 = this.position - this._joint.position;
            float num1 = vec2_1.length;
            if ((double)num1 > 8.0)
                num1 = 8f;
            this.position = this._joint.position + vec2_1.normalized * num1;
            if (this._part == 0 && this._doll != null && this._doll.captureDuck != null && (this._doll.captureDuck.quack > 0 || this.doll != null && this.doll.tongueStuck != Vec2.Zero))
            {
                Vec2 tounge = this._doll.captureDuck.tounge;
                this._stickLerp = Lerp.Vec2Smooth(this._stickLerp, tounge, 0.2f);
                this._stickSlowLerp = Lerp.Vec2Smooth(this._stickSlowLerp, tounge, 0.1f);
                Vec2 stickLerp = this._stickLerp;
                Vec2 vec = Maths.AngleToVec(this.angle);
                Vec2 vec2_2 = this.offDir >= (sbyte)0 ? stickLerp * Maths.Clamp(1f - (vec - stickLerp).length, 0.0f, 1f) : stickLerp * Maths.Clamp(1f - (vec - stickLerp * -1f).length, 0.0f, 1f);
                vec2_2.y *= -1f;
                Vec2 vec2_3 = this._stickSlowLerp;
                vec2_3.y *= -1f;
                float num2 = vec2_2.length;
                bool flag = false;
                if (this.doll != null && this.doll.tongueStuck != Vec2.Zero)
                {
                    flag = true;
                    num2 = 1f;
                }
                if ((double)num2 > 0.0500000007450581 | flag)
                {
                    Vec2 vec2_4 = this.position - (this.position - this._joint.position).normalized * 3f;
                    if (flag)
                    {
                        vec2_2 = (this.doll.tongueStuck - vec2_4) / 6f;
                        Vec2 vec2_5 = (this.doll.tongueStuck - vec2_4) / 6f / 2f;
                        vec2_3 = (this.Offset(new Vec2((this.doll.tongueStuck - vec2_4).length / 2f, 2f)) - vec2_4) / 6f;
                    }
                    List<Vec2> vec2List = Curve.Bezier(8, vec2_4, vec2_4 + vec2_3 * 6f, vec2_4 + vec2_2 * 6f);
                    Vec2 vec2_6 = Vec2.Zero;
                    float num3 = 1f;
                    foreach (Vec2 p2 in vec2List)
                    {
                        if (vec2_6 != Vec2.Zero)
                        {
                            Vec2 vec2_7 = vec2_6 - p2;
                            Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_6 + vec2_7.normalized * 0.4f, p2, new Color(223, 30, 30), 0.15f * num3, this.depth + 1);
                            Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_6 + vec2_7.normalized * 0.4f, p2 - vec2_7.normalized * 0.4f, Color.Black, 0.3f * num3, this.depth - 1);
                        }
                        num3 -= 0.1f;
                        vec2_6 = p2;
                        if (this.doll != null && this.doll.captureDuck != null)
                            this.doll.captureDuck.tongueCheck = p2;
                    }
                    if (this._graphic != null && this._graphic == this._quackSprite)
                    {
                        SpriteMap graphic = this.graphic as SpriteMap;
                        if (this.doll != null && this.doll.inSleepingBag)
                            this._graphic = (Sprite)this._campDuck;
                        if (this._offDir < (sbyte)0)
                            this._graphic.flipH = true;
                        else
                            this._graphic.flipH = false;
                        this._graphic.position = this.position;
                        this._graphic.alpha = this.alpha;
                        this._graphic.angle = this.angle;
                        this._graphic.depth = this.depth + 4;
                        this._graphic.scale = this.scale;
                        this._graphic.center = this.center;
                        if (this._graphic == this._campDuck)
                        {
                            (this._graphic as SpriteMap).frame = 4;
                            this._graphic.Draw();
                        }
                        else
                        {
                            (this._graphic as SpriteMap).frame += 36;
                            this._graphic.Draw();
                            (this._graphic as SpriteMap).frame -= 36;
                        }
                        this._graphic = (Sprite)graphic;
                    }
                }
                else if (this.doll != null && this.doll.captureDuck != null)
                    this.doll.captureDuck.tongueCheck = Vec2.Zero;
            }
            else if (this.doll != null && this.doll.captureDuck != null)
                this.doll.captureDuck.tongueCheck = Vec2.Zero;
            global::DuckGame.SpriteMap graphic1 = this.graphic as global::DuckGame.SpriteMap;
            if (graphic1 != null && this.doll != null && this.doll.inSleepingBag)
            {
                if (graphic1.frame == 18)
                    this._campDuck.frame = 0;
                else if (graphic1.frame == 19)
                    this._campDuck.frame = 1;
                else if (graphic1.frame == 20)
                    this._campDuck.frame = 2;
                if (graphic1 == this._quackSprite && graphic1.frame == 18)
                    this._campDuck.frame = 3;
                this.graphic = (Sprite)this._campDuck;
            }
            float angleDegrees = this.angleDegrees;
            if (this.offDir < (sbyte)0)
                this.angleDegrees = (float)(-(double)Maths.PointDirection(this.position, this._joint.position) + 180.0 + 90.0);
            else
                this.angleDegrees = (float)(-(double)Maths.PointDirection(this.position, this._joint.position) - 90.0);
            base.Draw();
            this.angleDegrees = angleDegrees;
            this.graphic = (Sprite)graphic1;
            this.position = position;
        }
    }
}

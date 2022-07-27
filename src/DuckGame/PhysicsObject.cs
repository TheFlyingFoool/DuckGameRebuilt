// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsObject
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public abstract class PhysicsObject : MaterialThing, ITeleport
    {
        public StateBinding _positionBinding = (StateBinding)new InterpolatedVec2Binding(nameof(netPosition), 10000);
        public StateBinding _velocityBinding = (StateBinding)new CompressedVec2Binding(GhostPriority.High, nameof(netVelocity), 20, true);
        public StateBinding _angleBinding = (StateBinding)new CompressedFloatBinding(GhostPriority.High, "_angle", 0.0f, isRot: true, doLerp: true);
        public StateBinding _offDirBinding = new StateBinding(GhostPriority.High, "_offDir");
        public StateBinding _ownerBinding = new StateBinding(GhostPriority.High, nameof(netOwner));
        public StateBinding _physicsStateBinding = (StateBinding)new PhysicsFlagBinding(GhostPriority.High);
        public StateBinding _burntBinding = (StateBinding)new CompressedFloatBinding("burnt", bits: 8);
        public StateBinding _collideSoundBinding = (StateBinding)new NetSoundBinding("_netCollideSound");
        public bool isSpawned;
        private const short positionMax = 8191;
        public Vec2 cameraPositionOverride = Vec2.Zero;
        public float vMax = 8f;
        public float hMax = 12f;
        protected Holdable _holdObject;
        public bool sliding;
        public bool crouch;
        public bool disableCrouch;
        public float friction = 0.1f;
        public float frictionMod;
        public float frictionMult = 1f;
        public float airFrictionMult = 1f;
        public float throwSpeedMultiplier = 1f;
        public static float gravity = 0.2f;
        protected bool _skipAutoPlatforms;
        protected bool _skipPlatforms;
        public bool wasHeldByPlayer;
        protected bool duck;
        public float gravMultiplier = 1f;
        public float floatMultiplier = 0.4f;
        private MaterialThing _collideLeft;
        private MaterialThing _collideRight;
        private MaterialThing _collideTop;
        private MaterialThing _collideBottom;
        private MaterialThing _wallCollideLeft;
        private MaterialThing _wallCollideRight;
        protected bool _inPhysicsLoop;
        protected Vec2 _lastPosition = Vec2.Zero;
        protected Vec2 _lastVelocity = Vec2.Zero;
        public bool inRewindLoop;
        public bool predict;
        private List<MaterialThing> _hitThings;
        private List<Duck> _hitDucks;
        public Vec2 velocityBeforeFriction = Vec2.Zero;
        private bool _initedNetSounds;
        public bool skipClip;
        private FluidData _curFluid;
        protected FluidPuddle _curPuddle;
        public bool removedFromFall;
        public DateTime lastGrounded;
        public byte framesSinceGrounded = 99;
        public bool _sleeping;
        public bool doFloat;
        private static Comparison<MaterialThing> XHspeedPositive = new Comparison<MaterialThing>(PhysicsObject.SortCollisionXHspeedPositive);
        private static Comparison<MaterialThing> XHspeedNegative = new Comparison<MaterialThing>(PhysicsObject.SortCollisionXHspeedNegative);
        private static Comparison<MaterialThing> YVspeedPositive = new Comparison<MaterialThing>(PhysicsObject.SortCollisionYVspeedPositive);
        private static Comparison<MaterialThing> YVspeedNegative = new Comparison<MaterialThing>(PhysicsObject.SortCollisionYVspeedNegative);
        public bool platformSkip;
        public float specialFrictionMod = 1f;
        private Predicate<MaterialThing> _collisionPred;
        //private bool firstCheck;
        private bool _awaken = true;
        private bool modifiedGravForFloat;
        public bool modFric;
        public bool updatePhysics = true;
        public bool didSpawn;
        public bool spawnAnimation;
        private MaterialGrid _gridMaterial;
        private Material _oldMaterial;
        private bool _oldMaterialSet;

        public short netVelocityX
        {
            get => (short)Math.Round((double)this.hSpeed * 1000.0);
            set => this.hSpeed = (float)value / 1000f;
        }

        public short netVelocityY
        {
            get => (short)Math.Round((double)this.vSpeed * 1000.0);
            set => this.vSpeed = (float)value / 1000f;
        }

        public byte netAngle
        {
            get
            {
                float num = this.angleDegrees;
                if ((double)num < 0.0)
                    num = Math.Abs(num) + 180f;
                return (byte)Math.Round((double)num % 360.0 / 2.0);
            }
            set => this.angleDegrees = (float)value * 2f;
        }

        public virtual Vec2 netVelocity
        {
            get => this.velocity;
            set => this.velocity = value;
        }

        public short netPositionX
        {
            get => (short)Maths.Clamp((int)Math.Round((double)this.position.x * 4.0), (int)short.MinValue, (int)short.MaxValue);
            set => this.position.x = (float)value / 4f;
        }

        public short netPositionY
        {
            get => (short)Maths.Clamp((int)Math.Round((double)this.position.y * 4.0), (int)short.MinValue, (int)short.MaxValue);
            set => this.position.y = (float)value / 4f;
        }

        public virtual Thing netOwner
        {
            get => this.owner;
            set => this.owner = value;
        }

        public override Vec2 netPosition
        {
            get
            {
                double x = (double)this.position.x;
                return this.position;
            }
            set
            {
                double x = (double)value.x;
                this.position = value;
            }
        }

        public override Vec2 cameraPosition => !(this.cameraPositionOverride != Vec2.Zero) ? base.cameraPosition : this.cameraPositionOverride;

        public Thing clipThing
        {
            get => this.clip.Count == 0 ? (Thing)null : (Thing)this.clip.ElementAt<MaterialThing>(0);
            set
            {
                if (value != null && value != this)
                {
                    this.clip.Clear();
                    this.clip.Add(value as MaterialThing);
                }
                else
                    this.clip.Clear();
            }
        }

        public Thing impactingThing
        {
            get => this.impacting.Count == 0 ? (Thing)null : (Thing)this.impacting.ElementAt<MaterialThing>(0);
            set
            {
                if (value != null && value != this)
                {
                    this.impacting.Clear();
                    this.impacting.Add(value as MaterialThing);
                }
                else
                    this.impacting.Clear();
            }
        }

        public virtual Holdable holdObject
        {
            get => this._holdObject;
            set => this._holdObject = value;
        }

        public Gun gun => this.holdObject as Gun;

        public float currentFriction => (this.friction + this.frictionMod) * this.frictionMult;

        public virtual float currentGravity => PhysicsObject.gravity * this.gravMultiplier * this.floatMultiplier;

        public MaterialThing collideLeft => this._collideLeft;

        public MaterialThing collideRight => this._collideRight;

        public MaterialThing collideTop => this._collideTop;

        public MaterialThing collideBottom => this._collideBottom;

        public MaterialThing wallCollideLeft => this._wallCollideLeft;

        public MaterialThing wallCollideRight => this._wallCollideRight;

        public override float impactPowerV => base.impactPowerV - ((double)this.vSpeed > 0.0 ? this.currentGravity * this.weightMultiplierInvTotal : 0.0f);

        public override float hSpeed
        {
            get => !this._inPhysicsLoop ? this._hSpeed : this.lastHSpeed;
            set => this._hSpeed = value;
        }

        public override float vSpeed
        {
            get => !this._inPhysicsLoop ? this._vSpeed : this.lastVSpeed;
            set => this._vSpeed = value;
        }

        public Vec2 lastPosition => this._lastPosition;

        public Vec2 lastVelocity => this._lastVelocity;

        public bool ownerIsLocalController => this.owner != null && this.owner.responsibleProfile != null && this.owner.responsibleProfile.localPlayer;

        public virtual float holdWeightMultiplier => this.holdObject != null ? this.holdObject.weightMultiplier : 1f;

        public virtual float holdWeightMultiplierSmall => this.holdObject != null ? this.holdObject.weightMultiplierSmall : 1f;

        public PhysicsObject()
          : this(0.0f, 0.0f)
        {
        }

        public PhysicsObject(float xval, float yval)
          : base(xval, yval)
        {
            this.syncPriority = GhostPriority.Normal;
            PhysicsObject.gravity = !TeamSelect2.Enabled("MOOGRAV") ? 0.2f : 0.1f;
            this._physicsIndex = Thing.GetGlobalIndex();
            this._ghostType = Editor.IDToType[this.GetType()];
            this._placementCost += 6;
            this._hitThings = new List<MaterialThing>();
            this._hitDucks = new List<Duck>();
        }

        public override void DoInitialize() => base.DoInitialize();

        public override void Initialize() => this._grounded = true;

        public override bool ShouldUpdate() => false;

        public bool sleeping
        {
            get => this._sleeping;
            set
            {
                if (this._sleeping && !value)
                {
                    this._sleeping = value;
                    foreach (PhysicsObject physicsObject in Level.CheckLineAll<PhysicsObject>(this.topLeft + new Vec2(0.0f, -4f), this.topRight + new Vec2(0.0f, -4f)))
                        physicsObject.sleeping = false;
                }
                this._sleeping = value;
            }
        }

        public static int SortCollisionXHspeedPositive(MaterialThing t1, MaterialThing t2)
        {
            float num1 = t1.x + (t1 is Block ? -10000f : 0.0f);
            float num2 = t2.x + (t2 is Block ? -10000f : 0.0f);
            if ((double)num1 > (double)num2)
                return 1;
            return (double)num1 < (double)num2 ? -1 : 0;
        }

        public static int SortCollisionXHspeedNegative(MaterialThing t1, MaterialThing t2)
        {
            float num1 = (float)(-(double)t1.x + (t1 is Block ? 10000.0 : 0.0));
            float num2 = (float)(-(double)t2.x + (t2 is Block ? 10000.0 : 0.0));
            if ((double)num1 > (double)num2)
                return 1;
            return (double)num1 < (double)num2 ? -1 : 0;
        }

        public static int SortCollisionYVspeedPositive(MaterialThing t1, MaterialThing t2)
        {
            float num1 = t1.y + (t1 is Block ? 10000f : 0.0f);
            float num2 = t2.y + (t2 is Block ? 10000f : 0.0f);
            if ((double)num1 > (double)num2)
                return 1;
            return (double)num1 < (double)num2 ? -1 : 0;
        }

        public static int SortCollisionYVspeedNegative(MaterialThing t1, MaterialThing t2)
        {
            float num1 = (float)(-(double)t1.y + (t1 is Block ? -10000.0 : 0.0));
            float num2 = (float)(-(double)t2.y + (t2 is Block ? -10000.0 : 0.0));
            if ((double)num1 > (double)num2)
                return 1;
            return (double)num1 < (double)num2 ? -1 : 0;
        }

        /// <summary>
        /// Called when the object is shot out of a pipe/cannon/etc
        /// </summary>
        /// <param name="pFrom">The thing this object was ejected from</param>
        public virtual void Ejected(Thing pFrom)
        {
        }

        public virtual void UpdatePhysics()
        {
            if (this.framesSinceGrounded > (byte)10)
                this.framesSinceGrounded = (byte)10;
            this._lastPosition = this.position;
            this._lastVelocity = this.velocity;
            base.Update();
            if (!this.solid || !this.enablePhysics || this.level != null && !this.level.simulatePhysics)
            {
                this.lastGrounded = DateTime.Now;
                if (this.solid)
                    return;
                this.solidImpacting.Clear();
                this.impacting.Clear();
            }
            else
            {
                if (this._collisionPred == null)
                    this._collisionPred = (Predicate<MaterialThing>)(thing => thing == null || !Collision.Rect(this.topLeft, this.bottomRight, (Thing)thing));
                this._collideLeft = (MaterialThing)null;
                this._collideRight = (MaterialThing)null;
                this._collideTop = (MaterialThing)null;
                this._collideBottom = (MaterialThing)null;
                this._wallCollideLeft = (MaterialThing)null;
                this._wallCollideRight = (MaterialThing)null;
                this._curPuddle = (FluidPuddle)null;
                if (!this.skipClip)
                {
                    this.clip.RemoveWhere(this._collisionPred);
                    this.impacting.RemoveWhere(this._collisionPred);
                }
                if (this._sleeping)
                {
                    if ((double)this.hSpeed == 0.0 && (double)this.vSpeed == 0.0 && (double)this.heat <= 0.0 && !this._awaken)
                        return;
                    this._sleeping = false;
                    this._awaken = false;
                }
                if (!this.skipClip)
                    this.solidImpacting.RemoveWhere(this._collisionPred);
                float currentFriction = this.currentFriction;
                if (this.sliding || this.crouch)
                    currentFriction *= 0.28f;
                float num1 = currentFriction * this.specialFrictionMod;
                if (this.owner is Duck)
                    this.gravMultiplier = 1f;
                if ((double)this.hSpeed > -(double)num1 && (double)this.hSpeed < (double)num1)
                    this.hSpeed = 0.0f;
                if (this.duck)
                {
                    if ((double)this.hSpeed > 0.0)
                        this.hSpeed -= num1;
                    if ((double)this.hSpeed < 0.0)
                        this.hSpeed += num1;
                }
                else if (this.grounded)
                {
                    if ((double)this.hSpeed > 0.0)
                        this.hSpeed -= num1;
                    if ((double)this.hSpeed < 0.0)
                        this.hSpeed += num1;
                }
                else
                {
                    if (this.isServerForObject && (double)this.y > (double)Level.current.lowestPoint + 500.0)
                    {
                        this.removedFromFall = true;
                        switch (this)
                        {
                            case Duck _:
                                return;
                            case RagdollPart _:
                                return;
                            case TrappedDuck _:
                                return;
                            default:
                                Level.Remove((Thing)this);
                                break;
                        }
                    }
                    if ((double)this.hSpeed > 0.0)
                        this.hSpeed -= num1 * 0.7f * this.airFrictionMult;
                    if ((double)this.hSpeed < 0.0)
                        this.hSpeed += num1 * 0.7f * this.airFrictionMult;
                }
                if ((double)this.hSpeed > (double)this.hMax)
                    this.hSpeed = this.hMax;
                if ((double)this.hSpeed < -(double)this.hMax)
                    this.hSpeed = -this.hMax;
                Vec2 p1_1 = this.topLeft + new Vec2(0.0f, 0.5f);
                Vec2 p2_1 = this.bottomRight + new Vec2(0.0f, -0.5f);
                this.lastHSpeed = this.hSpeed;
                float num2 = 0.0f;
                bool flag1 = false;
                if ((double)this.hSpeed != 0.0)
                {
                    int num3 = (int)Math.Ceiling((double)Math.Abs(this.hSpeed) / 4.0);
                    float hSpeed = this.hSpeed;
                    if ((double)this.hSpeed < 0.0)
                    {
                        p1_1.x += this.hSpeed;
                        p2_1.x -= 2f;
                    }
                    else
                    {
                        p2_1.x += this.hSpeed;
                        p1_1.x += 2f;
                    }
                    this._hitThings.Clear();
                    Level.CheckRectAll<MaterialThing>(p1_1, p2_1, this._hitThings);
                    if (Network.isActive && !this.isServerForObject && (double)Math.Abs(this.hSpeed) > 0.5)
                    {
                        this._hitDucks.Clear();
                        Level.CheckRectAll<Duck>(p1_1 + new Vec2(this.hSpeed * 2f, 0.0f), p2_1 + new Vec2(this.hSpeed * 2f, 0.0f), this._hitDucks);
                        foreach (Duck hitDuck in this._hitDucks)
                        {
                            if ((double)this.hSpeed > 0.0)
                                hitDuck.Impact((MaterialThing)this, ImpactedFrom.Left, true);
                            else if ((double)this.hSpeed < 0.0)
                                hitDuck.Impact((MaterialThing)this, ImpactedFrom.Right, true);
                        }
                    }
                    if ((double)this.hSpeed > 0.0)
                        DGList.Sort<MaterialThing>(this._hitThings, PhysicsObject.XHspeedPositive);
                    else
                        DGList.Sort<MaterialThing>(this._hitThings, PhysicsObject.XHspeedNegative);
                    for (int index = 0; index < num3; ++index)
                    {
                        float num4 = this.hSpeed / (float)num3;
                        if ((double)num4 != 0.0 && Math.Sign(num4) == Math.Sign(hSpeed))
                        {
                            this.x += num4;
                            this._inPhysicsLoop = true;
                            bool flag2 = false;
                            foreach (MaterialThing hitThing in this._hitThings)
                            {
                                if (hitThing != this && !this.clip.Contains(hitThing) && !hitThing.clip.Contains((MaterialThing)this) && hitThing.solid && (this.planeOfExistence == (byte)4 || (int)hitThing.planeOfExistence == (int)this.planeOfExistence) && (!flag2 || hitThing is Block))
                                {
                                    Vec2 position = this.position;
                                    bool flag3 = false;
                                    if ((double)hitThing.left <= (double)this.right && (double)hitThing.left > (double)this.left)
                                    {
                                        flag3 = true;
                                        if ((double)this.hSpeed > 0.0)
                                        {
                                            this._collideRight = hitThing;
                                            if (hitThing is Block)
                                            {
                                                this._wallCollideRight = hitThing;
                                                flag2 = true;
                                            }
                                            hitThing.Impact((MaterialThing)this, ImpactedFrom.Left, true);
                                            this.Impact(hitThing, ImpactedFrom.Right, true);
                                        }
                                    }
                                    if ((double)hitThing.right >= (double)this.left && (double)hitThing.right < (double)this.right)
                                    {
                                        flag3 = true;
                                        if ((double)this.hSpeed < 0.0)
                                        {
                                            this._collideLeft = hitThing;
                                            if (hitThing is Block)
                                            {
                                                this._wallCollideLeft = hitThing;
                                                flag2 = true;
                                            }
                                            hitThing.Impact((MaterialThing)this, ImpactedFrom.Right, true);
                                            this.Impact(hitThing, ImpactedFrom.Left, true);
                                        }
                                    }
                                    if (hitThing is IBigStupidWall && (double)(position - this.position).length > 64.0)
                                        this.position = position;
                                    if (flag3)
                                    {
                                        hitThing.Touch((MaterialThing)this);
                                        this.Touch(hitThing);
                                    }
                                }
                            }
                            this._inPhysicsLoop = false;
                        }
                        else
                            break;
                    }
                }
                if (flag1)
                    this.x = num2;
                if ((double)this.vSpeed > (double)this.vMax)
                    this.vSpeed = this.vMax;
                if ((double)this.vSpeed < -(double)this.vMax)
                    this.vSpeed = -this.vMax;
                this.vSpeed += this.currentGravity;
                if ((double)this.vSpeed < 0.0)
                    this.grounded = false;
                this.grounded = false;
                ++this.framesSinceGrounded;
                if ((double)this.vSpeed <= 0.0)
                    Math.Floor((double)this.vSpeed);
                else
                    Math.Ceiling((double)this.vSpeed);
                Vec2 p1_2 = this.topLeft + new Vec2(0.5f, 0.0f);
                Vec2 p2_2 = this.bottomRight + new Vec2(-0.5f, 0.0f);
                float num5 = -9999f;
                bool flag4 = false;
                float vSpeed = this.vSpeed;
                this.lastVSpeed = this.vSpeed;
                if ((double)this.vSpeed < 0.0)
                {
                    p1_2.y += this.vSpeed;
                    p2_2.y -= 2f;
                }
                else
                {
                    p2_2.y += this.vSpeed;
                    p1_2.y += 2f;
                }
                this._hitThings.Clear();
                Level.CheckRectAll<MaterialThing>(p1_2, p2_2, this._hitThings);
                if ((double)this.vSpeed > 0.0)
                    DGList.Sort<MaterialThing>(this._hitThings, PhysicsObject.YVspeedPositive);
                else
                    DGList.Sort<MaterialThing>(this._hitThings, PhysicsObject.YVspeedNegative);
                double top = (double)this.top;
                double bottom = (double)this.bottom;
                if (this is Duck duck)
                {
                    int num6 = !duck.inputProfile.Down("DOWN") ? 0 : (duck._jumpValid > 0 ? 1 : 0);
                }
                int num7 = (int)Math.Ceiling((double)Math.Abs(this.vSpeed) / 4.0);
                for (int index1 = 0; index1 < num7; ++index1)
                {
                    float num8 = this.vSpeed / (float)num7;
                    if ((double)num8 != 0.0 && Math.Sign(num8) == Math.Sign(vSpeed))
                    {
                        this.y += num8;
                        this._inPhysicsLoop = true;
                        for (int index2 = 0; index2 < this._hitThings.Count; ++index2)
                        {
                            MaterialThing hitThing = this._hitThings[index2];
                            if (hitThing is FluidPuddle)
                            {
                                flag4 = true;
                                this._curPuddle = hitThing as FluidPuddle;
                                if ((double)hitThing.top < (double)this.bottom - 2.0 && (double)hitThing.collisionSize.y > 2.0)
                                    num5 = hitThing.top;
                            }
                            if (hitThing != this && !this.clip.Contains(hitThing) && !hitThing.clip.Contains((MaterialThing)this) && hitThing.solid && (this.planeOfExistence == (byte)4 || (int)hitThing.planeOfExistence == (int)this.planeOfExistence))
                            {
                                Vec2 position = this.position;
                                bool flag5 = false;
                                if ((double)hitThing.bottom >= (double)this.top && (double)hitThing.top < (double)this.top)
                                {
                                    flag5 = true;
                                    if ((double)this.vSpeed < 0.0)
                                    {
                                        double y = (double)this.y;
                                        this._collideTop = hitThing;
                                        hitThing.Impact((MaterialThing)this, ImpactedFrom.Bottom, true);
                                        this.Impact(hitThing, ImpactedFrom.Top, true);
                                    }
                                }
                                if ((double)hitThing.top <= (double)this.bottom && (double)hitThing.bottom > (double)this.bottom)
                                {
                                    flag5 = true;
                                    if ((double)this.vSpeed > 0.0)
                                    {
                                        EnergyScimitar energyScimitar = this as EnergyScimitar;
                                        this._collideBottom = hitThing;
                                        hitThing.Impact((MaterialThing)this, ImpactedFrom.Top, true);
                                        this.Impact(hitThing, ImpactedFrom.Bottom, true);
                                    }
                                }
                                if (hitThing is IBigStupidWall && (double)(position - this.position).length > 64.0)
                                    this.position = position;
                                if (flag5)
                                {
                                    hitThing.Touch((MaterialThing)this);
                                    this.Touch(hitThing);
                                }
                            }
                        }
                        this._inPhysicsLoop = false;
                    }
                    else
                        break;
                }
                if (this.grounded)
                {
                    this.lastGrounded = DateTime.Now;
                    this.framesSinceGrounded = (byte)0;
                    if (!this.doFloat && (double)this.hSpeed == 0.0 && (double)this.vSpeed == 0.0 && !(this._collideBottom is PhysicsObject) && (this._collideBottom is Block || this._collideBottom is IPlatform) && (!(this._collideBottom is ItemBox) || (this._collideBottom as ItemBox).canBounce))
                        this._sleeping = true;
                }
                if ((double)num5 > -999.0)
                {
                    if (!this.doFloat && (double)this.vSpeed > 1.0)
                    {
                        Level.Add((Thing)new WaterSplash(this.x, num5 - 3f, this._curFluid));
                        SFX.Play("largeSplash", Rando.Float(0.6f, 0.7f), Rando.Float(-0.7f, -0.2f));
                    }
                    this.doFloat = true;
                }
                else
                    this.doFloat = false;
                if (this._curPuddle != null)
                {
                    this._curFluid = this._curPuddle.data;
                    if (this.onFire && (double)this._curFluid.flammable <= 0.5 && (double)this._curFluid.heat <= 0.5)
                        this.Extinquish();
                    else if ((double)this._curFluid.heat > 0.5)
                    {
                        if ((double)this.flammable > 0.0 && this.isServerForObject)
                        {
                            bool onFire = this.onFire;
                            this.Burn(this.position, (Thing)this);
                            if (this is Duck && (this as Duck).onFire && !onFire)
                                (this as Duck).ThrowItem();
                        }
                        this.DoHeatUp(0.015f, this.position);
                    }
                    else
                        this.DoHeatUp(-0.05f, this.position);
                }
                if (this.doFloat)
                {
                    if ((!(this is Duck) ? 0 : ((this as Duck).crouch ? 1 : 0)) != 0)
                    {
                        if ((double)this.floatMultiplier > 0.980000019073486)
                            this.vSpeed *= 0.8f;
                        this.floatMultiplier = 0.8f;
                    }
                    else
                    {
                        if ((double)this.floatMultiplier > 0.980000019073486)
                            this.vSpeed *= 0.4f;
                        this.vSpeed *= 0.95f;
                        this.floatMultiplier = 0.4f;
                    }
                }
                else
                {
                    if (flag4 && (double)vSpeed > 1.0 && (double)Math.Abs(this.vSpeed) < 0.00999999977648258)
                    {
                        Level.Add((Thing)new WaterSplash(this.x, this.bottom, this._curFluid));
                        SFX.Play("littleSplash", Rando.Float(0.8f, 0.9f), Rando.Float(-0.2f, 0.2f));
                    }
                    this.floatMultiplier = 1f;
                }
                Recorder.LogVelocity(Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed));
                if (this._sleeping)
                    return;
                if (this.modFric)
                    this.modFric = false;
                else
                    this.specialFrictionMod = 1f;
            }
        }

        public void DoFloat(bool lavaOnly = false)
        {
            this.onlyFloatInLava = lavaOnly;
            this.DoFloat();
        }

        public void DoFloat()
        {
            if ((double)this.buoyancy > 0.0)
            {
                FluidPuddle fluidPuddle = Level.CheckPoint<FluidPuddle>(this.position + new Vec2(0.0f, 4f));
                if (fluidPuddle != null)
                {
                    if (this.onlyFloatInLava && (double)fluidPuddle.data.heat < 0.5)
                        return;
                    if ((double)this.y + 4.0 - (double)fluidPuddle.top > 8.0)
                    {
                        this.modifiedGravForFloat = true;
                        this.gravMultiplier = -0.5f;
                        this.grounded = false;
                    }
                    else
                    {
                        if ((double)this.y + 4.0 - (double)fluidPuddle.top < 3.0)
                        {
                            this.modifiedGravForFloat = true;
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
            }
            else
            {
                if (!this.modifiedGravForFloat)
                    return;
                this.gravMultiplier = 1f;
            }
        }

        public override void Impact(MaterialThing with, ImpactedFrom from, bool solidImpact)
        {
            bool flag1 = true;
            bool flag2 = false;
            switch (with)
            {
                case Block _:
                    flag2 = true;
                    with.SolidImpact((MaterialThing)this, from);
                    if (with.destroyed)
                        return;
                    if (from == ImpactedFrom.Right)
                    {
                        this.x = with.left + (this.x - this.right);
                        this.SolidImpact(with, from);
                        if ((double)this.hSpeed > -(double)this.hSpeed * (double)this.bouncy)
                        {
                            this.hSpeed = -this.hSpeed * this.bouncy;
                            if ((double)Math.Abs(this.hSpeed) < 0.100000001490116)
                                this.hSpeed = 0.0f;
                        }
                    }
                    if (from == ImpactedFrom.Left)
                    {
                        this.x = with.right + (this.x - this.left);
                        this.SolidImpact(with, from);
                        if ((double)this.hSpeed < -(double)this.hSpeed * (double)this.bouncy)
                        {
                            this.hSpeed = -this.hSpeed * this.bouncy;
                            if ((double)Math.Abs(this.hSpeed) < 0.100000001490116)
                                this.hSpeed = 0.0f;
                        }
                    }
                    if (from == ImpactedFrom.Top)
                    {
                        this.y = (float)((double)with.bottom + ((double)this.y - (double)this.top) + 1.0);
                        this.SolidImpact(with, from);
                        if ((double)this.vSpeed < -(double)this.vSpeed * (double)this.bouncy)
                        {
                            this.vSpeed = -this.vSpeed * this.bouncy;
                            if ((double)Math.Abs(this.vSpeed) < 0.100000001490116)
                                this.vSpeed = 0.0f;
                        }
                    }
                    if (from == ImpactedFrom.Bottom)
                    {
                        this.y = with.top + (this.y - this.bottom);
                        this.SolidImpact(with, from);
                        if ((double)this.vSpeed > -(double)this.vSpeed * (double)this.bouncy)
                        {
                            this.vSpeed = -this.vSpeed * this.bouncy;
                            if ((double)Math.Abs(this.vSpeed) < 0.100000001490116)
                                this.vSpeed = 0.0f;
                        }
                        this.grounded = true;
                        break;
                    }
                    break;
                case IPlatform _:
                    flag2 = false;
                    if (from == ImpactedFrom.Bottom)
                    {
                        if (with is PhysicsObject && (!with.grounded || (double)Math.Abs(with.vSpeed) >= 0.300000011920929) || (double)with.top + ((double)this.vSpeed + 2.0) <= (double)this.bottom || this._skipPlatforms || this._skipAutoPlatforms && with is AutoPlatform)
                            return;
                        with.SolidImpact((MaterialThing)this, ImpactedFrom.Top);
                        if (with.destroyed)
                            return;
                        this.y = with.top + (this.y - this.bottom);
                        this.SolidImpact(with, from);
                        if ((double)this.vSpeed > -(double)this.vSpeed * (double)this.bouncy)
                        {
                            this.vSpeed = -this.vSpeed * this.bouncy;
                            if ((double)Math.Abs(this.vSpeed) < 0.100000001490116)
                                this.vSpeed = 0.0f;
                        }
                        this.grounded = true;
                        break;
                    }
                    break;
                default:
                    flag1 = false;
                    break;
            }
            if (flag1)
            {
                if (!flag2 && !this.impacting.Contains(with))
                {
                    base.Impact(with, from, solidImpact);
                    this.impacting.Add(with);
                }
                else
                {
                    if (!flag2 || this.solidImpacting.Contains(with))
                        return;
                    base.Impact(with, from, solidImpact);
                    this.solidImpacting.Add(with);
                }
            }
            else
                base.Impact(with, from, solidImpact);
        }

        public override void Update()
        {
            if (Network.isActive && !this._initedNetSounds)
            {
                this._initedNetSounds = true;
                List<string> list = this.collideSounds.GetList(ImpactedFrom.Bottom);
                if (list != null)
                {
                    this._netCollideSound = new NetSoundEffect(list.ToArray());
                    this._netCollideSound.volume = this._impactVolume;
                }
            }
            if (!this.updatePhysics)
                return;
            this.UpdatePhysics();
        }

        public override void DoDraw()
        {
            if (!Content.renderingToTarget && this.spawnAnimation)
            {
                if (this._gridMaterial == null)
                    this._gridMaterial = new MaterialGrid((Thing)this);
                if (!this._gridMaterial.finished)
                {
                    if (!this._oldMaterialSet)
                    {
                        this._oldMaterial = this.material;
                        this._oldMaterialSet = true;
                    }
                    this.material = (Material)this._gridMaterial;
                }
                else
                {
                    if (this._oldMaterialSet)
                        this.material = this._oldMaterial;
                    this.spawnAnimation = false;
                }
            }
            base.DoDraw();
        }

        public override void Draw()
        {
            if (this.graphic != null)
                this.graphic.flipH = this.offDir <= (sbyte)0;
            base.Draw();
        }

        public override void NetworkUpdate()
        {
        }

        protected void SyncNetworkAction(PhysicsObject.NetAction pAction)
        {
            if (!Network.isActive)
            {
                pAction();
            }
            else
            {
                if (!this.isServerForObject)
                    return;
                this.CheckForNetworkActionAttribute(pAction.Method);
                pAction();
                Send.Message((NetMessage)new NMRunNetworkAction(this, Editor.NetworkActionIndex(this.GetType(), pAction.Method)));
            }
        }

        protected void SyncNetworkAction<T>(Action<T> pMethod, T pParameter)
        {
            if (!Network.isActive)
            {
                pMethod(pParameter);
            }
            else
            {
                if (!this.isServerForObject)
                    return;
                this.CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter);
                Send.Message((NetMessage)new NMRunNetworkActionParameters(this, pMethod.Method, new object[1]
                {
          (object) pParameter
                }));
            }
        }

        protected void SyncNetworkAction<T, T2>(Action<T, T2> pMethod, T pParameter, T2 pParameter2)
        {
            if (!Network.isActive)
            {
                pMethod(pParameter, pParameter2);
            }
            else
            {
                if (!this.isServerForObject)
                    return;
                this.CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter, pParameter2);
                Send.Message((NetMessage)new NMRunNetworkActionParameters(this, pMethod.Method, new object[2]
                {
          (object) pParameter,
          (object) pParameter2
                }));
            }
        }

        protected void SyncNetworkAction<T, T2, T3>(
          Action<T, T2, T3> pMethod,
          T pParameter,
          T2 pParameter2,
          T3 pParameter3)
        {
            if (!Network.isActive)
            {
                pMethod(pParameter, pParameter2, pParameter3);
            }
            else
            {
                if (!this.isServerForObject)
                    return;
                this.CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter, pParameter2, pParameter3);
                Send.Message((NetMessage)new NMRunNetworkActionParameters(this, pMethod.Method, new object[3]
                {
          (object) pParameter,
          (object) pParameter2,
          (object) pParameter3
                }));
            }
        }

        protected void SyncNetworkAction<T, T2, T3, T4>(
          Action<T, T2, T3, T4> pMethod,
          T pParameter,
          T2 pParameter2,
          T3 pParameter3,
          T4 pParameter4)
        {
            if (!Network.isActive)
            {
                pMethod(pParameter, pParameter2, pParameter3, pParameter4);
            }
            else
            {
                if (!this.isServerForObject)
                    return;
                this.CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter, pParameter2, pParameter3, pParameter4);
                Send.Message((NetMessage)new NMRunNetworkActionParameters(this, pMethod.Method, new object[4]
                {
          (object) pParameter,
          (object) pParameter2,
          (object) pParameter3,
          (object) pParameter4
                }));
            }
        }

        protected void SyncNetworkAction<T, T2, T3, T4, T5>(
          Action<T, T2, T3, T4, T5> pMethod,
          T pParameter,
          T2 pParameter2,
          T3 pParameter3,
          T4 pParameter4,
          T5 pParameter5)
        {
            if (!Network.isActive)
            {
                pMethod(pParameter, pParameter2, pParameter3, pParameter4, pParameter5);
            }
            else
            {
                if (!this.isServerForObject)
                    return;
                this.CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter, pParameter2, pParameter3, pParameter4, pParameter5);
                Send.Message((NetMessage)new NMRunNetworkActionParameters(this, pMethod.Method, new object[5]
                {
          (object) pParameter,
          (object) pParameter2,
          (object) pParameter3,
          (object) pParameter4,
          (object) pParameter5
                }));
            }
        }

        private void CheckForNetworkActionAttribute(MethodInfo pMethod)
        {
            if (!((IEnumerable<object>)pMethod.GetCustomAttributes(typeof(NetworkAction), false)).Any<object>())
                throw new Exception("SyncNetworkAction can only be used for functions with the [NetworkAction] attribute defined.");
        }

        public delegate void NetAction();
    }
}

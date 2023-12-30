// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsObject
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public StateBinding _positionBinding = new InterpolatedVec2Binding(nameof(netPosition), 10000);
        public StateBinding _velocityBinding = new CompressedVec2Binding(GhostPriority.High, nameof(netVelocity), 20, true);
        public StateBinding _angleBinding = new CompressedFloatBinding(GhostPriority.High, "_angle", 0, 16, true, true);
        public StateBinding _offDirBinding = new StateBinding(GhostPriority.High, "_offDir");
        public StateBinding _ownerBinding = new StateBinding(GhostPriority.High, nameof(netOwner));
        public StateBinding _physicsStateBinding = new PhysicsFlagBinding(GhostPriority.High);
        public StateBinding _burntBinding = new CompressedFloatBinding("burnt", bits: 8);
        public StateBinding _collideSoundBinding = new NetSoundBinding("_netCollideSound");
        public bool isSpawned;
        //private const short positionMax = 8191;
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
        private MaterialThing _lastrealcollideBottom;
        private Vec2 _lastrealcollidepos;
        private Vec2 _lastrealcollidesize;
        private Vec2 _lastrealcollideoffset;
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
        private static Comparison<MaterialThing> XHspeedPositive = new Comparison<MaterialThing>(SortCollisionXHspeedPositive);
        private static Comparison<MaterialThing> XHspeedNegative = new Comparison<MaterialThing>(SortCollisionXHspeedNegative);
        private static Comparison<MaterialThing> YVspeedPositive = new Comparison<MaterialThing>(SortCollisionYVspeedPositive);
        private static Comparison<MaterialThing> YVspeedNegative = new Comparison<MaterialThing>(SortCollisionYVspeedNegative);
        public bool platformSkip;
        public float specialFrictionMod = 1f;
        private Predicate<MaterialThing> _collisionPred;
        //private bool firstCheck;
        private bool _awaken = true;
        public bool modifiedGravForFloat;
        public bool modFric;
        public bool updatePhysics = true;
        public bool didSpawn;
        public bool spawnAnimation;
        private MaterialGrid _gridMaterial;
        private Material _oldMaterial;
        private bool _oldMaterialSet;
        public bool initemspawner;
        public short netVelocityX
        {
            get => (short)Math.Round(hSpeed * 1000f);
            set => hSpeed = value / 1000f;
        }

        public short netVelocityY
        {
            get => (short)Math.Round(vSpeed * 1000f);
            set => vSpeed = value / 1000f;
        }

        public byte netAngle
        {
            get
            {
                float num = angleDegrees;
                if (num < 0) num = Math.Abs(num) + 180f;
                return (byte)Math.Round(num % 360 / 2);
            }
            set => angleDegrees = value * 2f;
        }

        public virtual Vec2 netVelocity
        {
            get => velocity;
            set => velocity = value;
        }

        public short netPositionX
        {
            get => (short)Maths.Clamp((int)Math.Round(position.x * 4f), short.MinValue, short.MaxValue);
            set => position.x = value / 4f;
        }

        public short netPositionY
        {
            get => (short)Maths.Clamp((int)Math.Round(position.y * 4f), short.MinValue, short.MaxValue);
            set => position.y = value / 4f;
        }

        public virtual Thing netOwner
        {
            get => owner;
            set => owner = value;
        }

        public override Vec2 netPosition
        {
            get
            {
                double x = position.x;
                return position;
            }
            set
            {
                double x = value.x;
                position = value;
            }
        }

        public override Vec2 cameraPosition => !(cameraPositionOverride != Vec2.Zero) ? base.cameraPosition : cameraPositionOverride;

        public Thing clipThing
        {
            get => clip.Count == 0 ? null : (Thing)clip.ElementAt(0);
            set
            {
                if (value != null && value != this)
                {
                    clip.Clear();
                    clip.Add(value as MaterialThing);
                }
                else
                    clip.Clear();
            }
        }

        public Thing impactingThing
        {
            get => impacting.Count == 0 ? null : (Thing)impacting.ElementAt(0);
            set
            {
                if (value != null && value != this)
                {
                    impacting.Clear();
                    impacting.Add(value as MaterialThing);
                }
                else
                    impacting.Clear();
            }
        }

        public virtual Holdable holdObject
        {
            get => _holdObject;
            set => _holdObject = value;
        }

        public Gun gun => holdObject as Gun;

        public float currentFriction => (friction + frictionMod) * frictionMult;

        public virtual float currentGravity => gravity * gravMultiplier * floatMultiplier;

        public MaterialThing collideLeft => _collideLeft;

        public MaterialThing collideRight => _collideRight;

        public MaterialThing collideTop => _collideTop;

        public MaterialThing collideBottom => _collideBottom;

        public MaterialThing wallCollideLeft => _wallCollideLeft;

        public MaterialThing wallCollideRight => _wallCollideRight;

        public override float impactPowerV => base.impactPowerV - (vSpeed > 0 ? currentGravity * weightMultiplierInvTotal : 0f);

        public override float hSpeed
        {
            get => !_inPhysicsLoop ? _hSpeed : lastHSpeed;
            set => _hSpeed = value;
        }

        public override float vSpeed
        {
            get => !_inPhysicsLoop ? _vSpeed : lastVSpeed;
            set => _vSpeed = value;
        }

        public Vec2 lastPosition => _lastPosition;

        public Vec2 lastVelocity => _lastVelocity;

        public bool ownerIsLocalController => owner != null && owner.responsibleProfile != null && owner.responsibleProfile.localPlayer;

        public virtual float holdWeightMultiplier => holdObject != null ? holdObject.weightMultiplier : 1f;

        public virtual float holdWeightMultiplierSmall => holdObject != null ? holdObject.weightMultiplierSmall : 1f;

        public PhysicsObject()
          : this(0f, 0f)
        {
        }

        public PhysicsObject(float xval, float yval)
          : base(xval, yval)
        {
            syncPriority = GhostPriority.Normal;
            gravity = !TeamSelect2.Enabled("MOOGRAV") ? 0.2f : 0.1f;
            _physicsIndex = GetGlobalIndex();
            _ghostType = Editor.IDToType[GetType()];
            _placementCost += 6;
            _hitThings = new List<MaterialThing>();
            _hitDucks = new List<Duck>();
        }

        public override void DoInitialize() => base.DoInitialize();

        public override void Initialize() => _grounded = true;

        public override bool ShouldUpdate() => false;

        public bool sleeping
        {
            get => _sleeping;
            set
            {
                if (_sleeping && !value)
                {
                    _sleeping = value;
                    foreach (PhysicsObject physicsObject in Level.CheckLineAll<PhysicsObject>(topLeft + new Vec2(0f, -4f), topRight + new Vec2(0f, -4f)))
                        physicsObject.sleeping = false;
                }
                _sleeping = value;
            }
        }

        public static int SortCollisionXHspeedPositive(MaterialThing t1, MaterialThing t2)
        {
            float num1 = t1.x + (t1.isBlock ? -10000f : 0f);
            float num2 = t2.x + (t2.isBlock ? -10000f : 0f);
            if (num1 > num2)
                return 1;
            return num1 < num2 ? -1 : 0;
        }

        public static int SortCollisionXHspeedNegative(MaterialThing t1, MaterialThing t2)
        {
            float num1 = -t1.x + (t1.isBlock ? 10000f : 0);
            float num2 = -t2.x + (t2.isBlock ? 10000f : 0);
            if (num1 > num2)
                return 1;
            return num1 < num2 ? -1 : 0;
        }

        public static int SortCollisionYVspeedPositive(MaterialThing t1, MaterialThing t2)
        {
            float num1 = t1.y + (t1.isBlock ? 10000f : 0f);
            float num2 = t2.y + (t2.isBlock ? 10000f : 0f);
            if (num1 > num2)
                return 1;
            return num1 < num2 ? -1 : 0;
        }

        public static int SortCollisionYVspeedNegative(MaterialThing t1, MaterialThing t2)
        {
            float num1 = -t1.y + (t1.isBlock ? -10000f : 0f);
            float num2 = -t2.y + (t2.isBlock ? -10000f : 0f);
            if (num1 > num2)
                return 1;
            return num1 < num2 ? -1 : 0;
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
            if (framesSinceGrounded > 10)
                framesSinceGrounded = 10;
            _lastPosition = position;
            _lastVelocity = velocity;
            base.Update();
            if (!solid || !enablePhysics || level != null && !level.simulatePhysics)
            {
                //lastGrounded = DateTime.Now;
                if (solid)
                    return;
                solidImpacting.Clear();
                impacting.Clear();
            }
            else
            {
                if (_collisionPred == null)
                    _collisionPred = thing => thing == null || !Collision.Rect(topLeft, bottomRight, thing);
                _collideLeft = null;
                _collideRight = null;
                _collideTop = null;
                _collideBottom = null;
                _wallCollideLeft = null;
                _wallCollideRight = null;
                _curPuddle = null;
                if (!skipClip)
                {
                    clip.RemoveWhere(_collisionPred);
                    impacting.RemoveWhere(_collisionPred);
                }
                if (_sleeping)
                {//(_collideBottom is PhysicsObject)
                    if (hSpeed == 0 && this.vSpeed == 0 && heat <= 0 && !_awaken && (!(_lastrealcollideBottom is PhysicsObject) || (!_lastrealcollideBottom.removeFromLevel && (_lastrealcollideBottom.position.x - _lastrealcollidepos.x) == 0 && (_lastrealcollideBottom.position.y - _lastrealcollidepos.y) == 0 && _lastrealcollidesize == _lastrealcollideBottom.collisionSize && _lastrealcollideoffset == _lastrealcollideBottom.collisionOffset
                        && _lastrealcollideBottom.grounded && (_lastrealcollideBottom as PhysicsObject).sleeping)))
                        return;
                    _sleeping = false;
                    _awaken = false;
                }
                if (!skipClip)
                    solidImpacting.RemoveWhere(_collisionPred);
                float currentFriction = this.currentFriction;
                if (sliding || crouch)
                    currentFriction *= 0.28f;
                float num1 = currentFriction * specialFrictionMod;
                if (owner is Duck)
                    gravMultiplier = 1f;
                if (hSpeed > -num1 && hSpeed < num1)
                    hSpeed = 0f;
                if (duck)
                {
                    if (hSpeed > 0) hSpeed -= num1;
                    if (hSpeed < 0) hSpeed += num1;
                }
                else if (grounded)
                {
                    if (hSpeed > 0) hSpeed -= num1;
                    if (hSpeed < 0) hSpeed += num1;
                }
                else
                {
                    if (isServerForObject && y > Level.current.lowestPoint + 500)
                    {
                        removedFromFall = true;
                        switch (this)
                        {
                            case Duck _:
                                return;
                            case RagdollPart _:
                                return;
                            case TrappedDuck _:
                                return;
                            default:
                                Level.Remove(this);
                                break;
                        }
                    }
                    if (hSpeed > 0) hSpeed -= num1 * 0.7f * airFrictionMult;
                    if (hSpeed < 0) hSpeed += num1 * 0.7f * airFrictionMult;
                }
                if (hSpeed > hMax) hSpeed = hMax;
                if (hSpeed < -hMax) hSpeed = -hMax;
                Vec2 p1_1 = topLeft + new Vec2(0f, 0.5f);
                Vec2 p2_1 = bottomRight + new Vec2(0f, -0.5f);
                lastHSpeed = hSpeed;
                //float num2 = 0f; THESE ARENT EVEN DOING ANYTHING?? -NiK0
                //bool flag1 = false;
                if (hSpeed != 0)
                {
                    int num3 = (int)Math.Ceiling(Math.Abs(this.hSpeed) / 4);
                    float hSpeed = this.hSpeed;
                    if (this.hSpeed < 0)
                    {
                        p1_1.x += this.hSpeed;
                        p2_1.x -= 2f;
                    }
                    else
                    {
                        p2_1.x += this.hSpeed;
                        p1_1.x += 2f;
                    }
                    _hitThings.Clear();
                    Level.CheckRectAll(p1_1, p2_1, _hitThings);
                    if (Network.isActive && !isServerForObject && Math.Abs(this.hSpeed) > 0.5f)
                    {
                        _hitDucks.Clear();
                        Level.CheckRectAll(p1_1 + new Vec2(this.hSpeed * 2f, 0f), p2_1 + new Vec2(this.hSpeed * 2f, 0f), _hitDucks);
                        foreach (Duck hitDuck in _hitDucks)
                        {
                            if (this.hSpeed > 0) hitDuck.Impact(this, ImpactedFrom.Left, true);
                            else if (this.hSpeed < 0) hitDuck.Impact(this, ImpactedFrom.Right, true);
                        }
                    }
                    if (this.hSpeed > 0) 
                        DGList.Sort(_hitThings, XHspeedPositive);
                    else 
                        DGList.Sort(_hitThings, XHspeedNegative);
                    for (int index = 0; index < num3; ++index)
                    {
                        float num4 = this.hSpeed / num3;
                        if (num4 != 0 && Math.Sign(num4) == Math.Sign(hSpeed))
                        {
                            x += num4;
                            _inPhysicsLoop = true;
                            bool flag2 = false;
                            foreach (MaterialThing hitThing in _hitThings)
                            {
                                if (hitThing != this && !clip.Contains(hitThing) && !hitThing.clip.Contains(this) && hitThing.solid && (planeOfExistence == 4 || hitThing.planeOfExistence == planeOfExistence) && (!flag2 || hitThing.isBlock))
                                {
                                    Vec2 position = this.position;
                                    bool flag3 = false;
                                    if (hitThing.left <= right && hitThing.left > left)
                                    {
                                        flag3 = true;
                                        if (this.hSpeed > 0)
                                        {
                                            _collideRight = hitThing;
                                            if (hitThing.isBlock)
                                            {
                                                _wallCollideRight = hitThing;
                                                flag2 = true;
                                            }
                                            hitThing.Impact(this, ImpactedFrom.Left, true);
                                            Impact(hitThing, ImpactedFrom.Right, true);
                                        }
                                    }
                                    if (hitThing.right >= left && hitThing.right < right)
                                    {
                                        flag3 = true;
                                        if (this.hSpeed < 0)
                                        {
                                            _collideLeft = hitThing;
                                            if (hitThing.isBlock)
                                            {
                                                _wallCollideLeft = hitThing;
                                                flag2 = true;
                                            }
                                            hitThing.Impact(this, ImpactedFrom.Right, true);
                                            Impact(hitThing, ImpactedFrom.Left, true);
                                        }
                                    }
                                    if (hitThing is IBigStupidWall && (position - this.position).length > 64f)
                                        this.position = position;
                                    if (flag3)
                                    {
                                        hitThing.Touch(this);
                                        Touch(hitThing);
                                    }
                                }
                            }
                            _inPhysicsLoop = false;
                        }
                        else break;
                    }
                }
                //if (flag1) x = num2;
                if (this.vSpeed > vMax) this.vSpeed = vMax;
                if (this.vSpeed < -vMax) this.vSpeed = -vMax;
                this.vSpeed += currentGravity;
                if (this.vSpeed < 0) grounded = false;
                grounded = false;
                framesSinceGrounded++;
                if (this.vSpeed <= 0) Math.Floor(this.vSpeed);
                else Math.Ceiling(this.vSpeed);
                Vec2 p1_2 = topLeft + new Vec2(0.5f, 0f);
                Vec2 p2_2 = bottomRight + new Vec2(-0.5f, 0f);
                float num5 = -9999f;
                bool flag4 = false;
                float vSpeed = this.vSpeed;
                lastVSpeed = this.vSpeed;
                if (this.vSpeed < 0)
                {
                    p1_2.y += this.vSpeed;
                    p2_2.y -= 2f;
                }
                else
                {
                    p2_2.y += this.vSpeed;
                    p1_2.y += 2f;
                }
                _hitThings.Clear();
                Level.CheckRectAll(p1_2, p2_2, _hitThings);
                if (this.vSpeed > 0) 
                    DGList.Sort(_hitThings, YVspeedPositive);
                else 
                    DGList.Sort(_hitThings, YVspeedNegative);

                //double top = this.top; WHATS THE POINT OF THIS?????????? -NiK0
                //double bottom = this.bottom;

                //Whats the point of this?? -NiK0
                //if (this is Duck duck)
                //{
                //    int num6 = !duck.inputProfile.Down(Triggers.Down) ? 0 : (duck._jumpValid > 0 ? 1 : 0);
                //}
                int num7 = (int)Math.Ceiling(Math.Abs(this.vSpeed) / 4f);
                for (int index1 = 0; index1 < num7; ++index1)
                {
                    float num8 = this.vSpeed / num7;
                    if (num8 != 0 && Math.Sign(num8) == Math.Sign(vSpeed))
                    {
                        y += num8;
                        _inPhysicsLoop = true;
                        for (int index2 = 0; index2 < _hitThings.Count; ++index2)
                        {
                            MaterialThing hitThing = _hitThings[index2];
                            if (hitThing is FluidPuddle)
                            {
                                flag4 = true;
                                _curPuddle = hitThing as FluidPuddle;
                                if (hitThing.top < bottom - 2f && hitThing.collisionSize.y > 2f) num5 = hitThing.top;
                            }
                            if (hitThing != this && !clip.Contains(hitThing) && !hitThing.clip.Contains(this) && hitThing.solid && (planeOfExistence == 4 || hitThing.planeOfExistence == planeOfExistence))
                            {
                                Vec2 position = this.position;
                                bool flag5 = false;
                                if (hitThing.bottom >= top && hitThing.top < top)
                                {
                                    flag5 = true;
                                    if (this.vSpeed < 0)
                                    {
                                        double y = this.y;
                                        _collideTop = hitThing;
                                        hitThing.Impact(this, ImpactedFrom.Bottom, true);
                                        Impact(hitThing, ImpactedFrom.Top, true);
                                    }
                                }
                                if (hitThing.top <= bottom && hitThing.bottom > bottom)
                                {
                                    flag5 = true;
                                    if (this.vSpeed > 0)
                                    {
                                        //EnergyScimitar energyScimitar = this as EnergyScimitar;
                                        if (!(hitThing is FluidPuddle) || buoyancy > 0f)
                                        {
                                            _collideBottom = hitThing;
                                            _lastrealcollidepos = hitThing.position;
                                            _lastrealcollidesize = hitThing.collisionSize;
                                            _lastrealcollideoffset = hitThing.collisionOffset;
                                            _lastrealcollideBottom = hitThing;
                                        }

                                        hitThing.Impact(this, ImpactedFrom.Top, true);
                                        Impact(hitThing, ImpactedFrom.Bottom, true);
                                    }
                                }
                                if (hitThing is IBigStupidWall && (position - this.position).length > 64f) this.position = position;
                                if (flag5)
                                {
                                    hitThing.Touch(this);
                                    Touch(hitThing);
                                }
                            }
                        }
                        _inPhysicsLoop = false;
                    }
                    else break;
                }
                if (grounded || initemspawner)
                {
                    //(!(_collideBottom is PhysicsObject) || (!(_collideBottom as PhysicsObject).modifiedGravForFloat && _collideBottom.grounded && (!((this._lastrealcollideBottom as PhysicsObject)._lastrealcollideBottom is PhysicsObject) || (!((this._lastrealcollideBottom as PhysicsObject)._lastrealcollideBottom as PhysicsObject).modifiedGravForFloat && ((this._lastrealcollideBottom as PhysicsObject)._lastrealcollideBottom as PhysicsObject).groun
                    //))))) 
                    //!(this._lastrealcollideBottom as PhysicsObject)._lastrealcollideBottom is PhysicsObject || !((this._lastrealcollideBottom as PhysicsObject)._lastrealcollideBottom as PhysicsObject).modifiedGravForFloat
                    // lastGrounded = DateTime.Now;
                    framesSinceGrounded = 0;// mmmm remove i shall !(_collideBottom is PhysicsObject) // !doFloat &&  !doFloat && 

                    if ((!doFloat || buoyancy <= 0f) && hSpeed == 0 && this.vSpeed == 0 && (((_collideBottom is Block || _collideBottom is IPlatform) && (!(_collideBottom is ItemBox) || (_collideBottom as ItemBox).canBounce)) || initemspawner) && (!(_collideBottom is PhysicsObject) || ((_collideBottom as PhysicsObject).sleeping))) // !(_collideBottom as PhysicsObject).modifiedGravForFloat && _collideBottom.grounded && 
                        // (this._lastrealcollideBottom as PhysicsObject)._lastrealcollideBottom is PhysicsObject
                        _sleeping = true;
                }
                if (num5 > -999f)
                {
                    if (!doFloat && this.vSpeed > 1f)
                    {
                        if (DGRSettings.ActualParticleMultiplier > 0) Level.Add(new WaterSplash(x, num5 - 3f, _curFluid));
                        SFX.Play("largeSplash", Rando.Float(0.6f, 0.7f), Rando.Float(-0.7f, -0.2f));
                    }
                    doFloat = true;
                }
                else doFloat = false;
                if (_curPuddle != null)
                {
                    _curFluid = _curPuddle.data;
                    if (onFire && _curFluid.flammable <= 0.5 && _curFluid.heat <= 0.5)
                        Extinquish();
                    else if (_curFluid.heat > 0.5)
                    {
                        if (flammable > 0 && isServerForObject)
                        {
                            bool onFire = this.onFire;
                            Burn(position, this);
                            if (this is Duck && (this as Duck).onFire && !onFire) (this as Duck).ThrowItem();
                        }
                        DoHeatUp(0.015f, position);
                    }
                    else
                        DoHeatUp(-0.05f, position);
                }
                if (doFloat)
                {
                    if ((!(this is Duck) ? 0 : ((this as Duck).crouch ? 1 : 0)) != 0)
                    {
                        if (floatMultiplier > 0.98f) this.vSpeed *= 0.8f;
                        floatMultiplier = 0.8f;
                    }
                    else
                    {
                        if (floatMultiplier > 0.98f) this.vSpeed *= 0.4f;
                        this.vSpeed *= 0.95f;
                        floatMultiplier = 0.4f;
                    }
                }
                else
                {
                    if (flag4 && vSpeed > 1 && Math.Abs(this.vSpeed) < 0.01f)
                    {
                        Level.Add(new WaterSplash(x, bottom, _curFluid));
                        SFX.Play("littleSplash", Rando.Float(0.8f, 0.9f), Rando.Float(-0.2f, 0.2f));
                    }
                    floatMultiplier = 1f;
                }
                Recorder.LogVelocity(Math.Abs(hSpeed) + Math.Abs(this.vSpeed));
                if (_sleeping)
                    return;
                if (modFric)
                    modFric = false;
                else
                    specialFrictionMod = 1f;
            }
        }

        public void DoFloat(bool lavaOnly = false)
        {
            onlyFloatInLava = lavaOnly;
            DoFloat();
        }

        public void DoFloat()
        {
            if (buoyancy > 0f)
            {
                FluidPuddle fluidPuddle = Level.CheckPoint<FluidPuddle>(position + new Vec2(0f, 4f));
                if (fluidPuddle != null)
                {
                    if (onlyFloatInLava && fluidPuddle.data.heat < 0.5f)
                        return;
                    if (y + 4f - fluidPuddle.top > 8f)
                    {
                        modifiedGravForFloat = true;
                        gravMultiplier = -0.5f;
                        grounded = false;
                    }
                    else
                    {
                        if (y + 4f - fluidPuddle.top < 3f)
                        {
                            modifiedGravForFloat = true;
                            gravMultiplier = 0.2f;
                            grounded = true;
                        }
                        else if (y + 4f - fluidPuddle.top > 4f)
                        {
                            gravMultiplier = -0.2f;
                            grounded = true;
                        }
                        grounded = true;
                    }
                }
                else
                    gravMultiplier = 1f;
            }
            else
            {
                if (!modifiedGravForFloat)
                    return;
                gravMultiplier = 1f;
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
                    with.SolidImpact(this, from);
                    if (with.destroyed)
                        return;
                    if (from == ImpactedFrom.Right)
                    {
                        x = with.left + (x - right);
                        SolidImpact(with, from);
                        if (hSpeed > -hSpeed * bouncy)
                        {
                            hSpeed = -hSpeed * bouncy;
                            if (Math.Abs(hSpeed) < 0.1f)
                                hSpeed = 0f;
                        }
                    }
                    if (from == ImpactedFrom.Left)
                    {
                        x = with.right + (x - left);
                        SolidImpact(with, from);
                        if (hSpeed < -hSpeed * bouncy)
                        {
                            hSpeed = -hSpeed * bouncy;
                            if (Math.Abs(hSpeed) < 0.1f)
                                hSpeed = 0f;
                        }
                    }
                    if (from == ImpactedFrom.Top)
                    {
                        y = (with.bottom + (y - top) + 1f);
                        SolidImpact(with, from);
                        if (vSpeed < -vSpeed * bouncy)
                        {
                            vSpeed = -vSpeed * bouncy;
                            if (Math.Abs(vSpeed) < 0.1f)
                                vSpeed = 0f;
                        }
                    }
                    if (from == ImpactedFrom.Bottom)
                    {
                        y = with.top + (y - bottom);
                        SolidImpact(with, from);
                        if (vSpeed > -vSpeed * bouncy)
                        {
                            vSpeed = -vSpeed * bouncy;
                            if (Math.Abs(vSpeed) < 0.1f || (bouncy != 0f && Math.Abs(_vSpeed) < 0.1f))
                            {
                                vSpeed = 0f;
                            }
                        }
                        grounded = true;
                        break;
                    }
                    break;
                case IPlatform _:
                    flag2 = false;
                    if (from == ImpactedFrom.Bottom)
                    {
                        if (with is PhysicsObject && (!with.grounded || Math.Abs(with.vSpeed) >= 0.3f) || with.top + (vSpeed + 2f) <= bottom || _skipPlatforms || _skipAutoPlatforms && with is AutoPlatform)
                            return;
                        with.SolidImpact(this, ImpactedFrom.Top);
                        if (with.destroyed)
                            return;
                        y = with.top + (y - bottom);
                        SolidImpact(with, from);
                        if (vSpeed > -vSpeed * bouncy)
                        {
                            vSpeed = -vSpeed * bouncy;
                            if (Math.Abs(vSpeed) < 0.1f || (bouncy != 0f && Math.Abs(_vSpeed) < 0.1f))
                                vSpeed = 0f;
                        }
                        grounded = true;
                        break;
                    }
                    break;
                default:
                    flag1 = false;
                    break;
            }
            if (flag1)
            {
                if (!flag2 && !impacting.Contains(with))
                {
                    base.Impact(with, from, solidImpact);
                    impacting.Add(with);
                }
                else
                {
                    if (!flag2 || solidImpacting.Contains(with))
                        return;
                    base.Impact(with, from, solidImpact);
                    solidImpacting.Add(with);
                }
            }
            else
                base.Impact(with, from, solidImpact);
        }

        public override void Update()
        {
            if (Network.isActive && !_initedNetSounds)
            {
                _initedNetSounds = true;
                List<string> list = collideSounds.GetList(ImpactedFrom.Bottom);
                if (list != null)
                {
                    _netCollideSound = new NetSoundEffect(list.ToArray())
                    {
                        volume = _impactVolume
                    };
                }
            }
            if (!updatePhysics)
                return;
            UpdatePhysics();
        }

        public override void DoDraw()
        {
            if (!Content.renderingToTarget && spawnAnimation)
            {
                if (_gridMaterial == null)
                    _gridMaterial = new MaterialGrid(this);
                if (!_gridMaterial.finished)
                {
                    if (!_oldMaterialSet)
                    {
                        _oldMaterial = material;
                        _oldMaterialSet = true;
                    }
                    material = _gridMaterial;
                }
                else
                {
                    if (_oldMaterialSet)
                        material = _oldMaterial;
                    spawnAnimation = false;
                }
            }
            base.DoDraw();
        }

        public override void Draw()
        {
            if (graphic != null)
                graphic.flipH = offDir <= 0;
            base.Draw();
        }

        public override void NetworkUpdate()
        {
        }

        protected void SyncNetworkAction(NetAction pAction)
        {
            if (!Network.isActive)
            {
                pAction();
            }
            else
            {
                if (!isServerForObject)
                    return;
                CheckForNetworkActionAttribute(pAction.Method);
                pAction();
                Send.Message(new NMRunNetworkAction(this, Editor.NetworkActionIndex(GetType(), pAction.Method)));
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
                if (!isServerForObject)
                    return;
                CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter);
                Send.Message(new NMRunNetworkActionParameters(this, pMethod.Method, new object[1]
                {
           pParameter
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
                if (!isServerForObject)
                    return;
                CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter, pParameter2);
                Send.Message(new NMRunNetworkActionParameters(this, pMethod.Method, new object[2]
                {
           pParameter,
           pParameter2
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
                if (!isServerForObject)
                    return;
                CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter, pParameter2, pParameter3);
                Send.Message(new NMRunNetworkActionParameters(this, pMethod.Method, new object[3]
                {
           pParameter,
           pParameter2,
           pParameter3
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
                if (!isServerForObject)
                    return;
                CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter, pParameter2, pParameter3, pParameter4);
                Send.Message(new NMRunNetworkActionParameters(this, pMethod.Method, new object[4]
                {
           pParameter,
           pParameter2,
           pParameter3,
           pParameter4
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
                if (!isServerForObject)
                    return;
                CheckForNetworkActionAttribute(pMethod.Method);
                pMethod(pParameter, pParameter2, pParameter3, pParameter4, pParameter5);
                Send.Message(new NMRunNetworkActionParameters(this, pMethod.Method, new object[5]
                {
           pParameter,
           pParameter2,
           pParameter3,
           pParameter4,
           pParameter5
                }));
            }
        }

        private void CheckForNetworkActionAttribute(MethodInfo pMethod)
        {
            if (!pMethod.GetCustomAttributes(typeof(NetworkAction), false).Any())
                throw new Exception("SyncNetworkAction can only be used for functions with the [NetworkAction] attribute defined.");
        }

        public delegate void NetAction();
    }
}

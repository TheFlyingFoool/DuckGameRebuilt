// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialThing
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public abstract class MaterialThing : Thing
    {
        public bool _ruined;
        protected NetSoundEffect _netCollideSound = new NetSoundEffect();
        public float buoyancy;
        public bool onlyFloatInLava;
        protected bool ignoreCollisions;
        public float thickness = -1f;
        public PhysicsMaterial physicsMaterial;
        private bool _destroyedReal;
        public bool cold;
        protected bool _translucent;
        protected float _maxHealth;
        public float _hitPoints;
        public bool destructive;
        public bool onlyCrush;
        protected bool _dontCrush;
        private HashSet<MaterialThing> _clip = new HashSet<MaterialThing>();
        private HashSet<MaterialThing> _impacting = new HashSet<MaterialThing>();
        private HashSet<MaterialThing> _solidImpacting = new HashSet<MaterialThing>();
        private byte _planeOfExistence = 4;
        public bool _didImpactSound;
        protected bool _grounded;
        protected float _bouncy;
        protected float _breakForce = 999f;
        protected float _impactThreshold = 0.5f;
        protected float _weight = 5f;
        public float spreadExtinguisherSmoke;
        private bool _islandDirty;
        private CollisionIsland _island;
        protected Thing _zapper;
        public bool willHeat;
        private Organizer<ImpactedFrom, string> _collideSounds = new Organizer<ImpactedFrom, string>();
        protected float _impactVolume = 0.5f;
        public float lastHSpeed;
        public float lastVSpeed;
        private Thing _lastBurnedBy;
        protected float _flameWait;
        public float burnSpeed = 1f / 500f;
        private int extWait;
        public float coolingFactor = 3f / 500f;
        protected bool _sendDestroyMessage;
        private bool _sentDestroyMessage;
        public float flammable;
        public float heat;
        public float burnt;
        protected bool _onFire;
        public ActionTimer _burnWaitTimer;
        public bool isBurnMessage;
        public bool superNonFlammable;

        /// <summary>Called when the object hits a spring.</summary>
        /// <param name="pSpringer">The spring what which sprung</param>
        /// <returns>Return 'true' to continue running regular spring logic. 'False' to ignore it.</returns>
        public virtual bool Sprung(Thing pSpringer) => true;

        public static Vec2 ImpactVector(ImpactedFrom from)
        {
            switch (from)
            {
                case ImpactedFrom.Left:
                    return new Vec2(-1f, 0f);
                case ImpactedFrom.Right:
                    return new Vec2(1f, 0f);
                case ImpactedFrom.Top:
                    return new Vec2(0f, -1f);
                default:
                    return new Vec2(0f, 1f);
            }
        }

        public bool _destroyed
        {
            get => _destroyedReal;
            set
            {
                int num = value ? 1 : 0;
                _destroyedReal = value;
            }
        }

        public bool translucent => _translucent;

        public virtual bool Hurt(float points)
        {
            if (_maxHealth == 0.0)
                return false;
            _hitPoints -= points;
            return true;
        }

        public bool dontCrush
        {
            get => _dontCrush;
            set => _dontCrush = value;
        }

        public HashSet<MaterialThing> clip
        {
            get => _clip;
            set => _clip = value;
        }

        public HashSet<MaterialThing> impacting
        {
            get => _impacting;
            set => _impacting = value;
        }

        public HashSet<MaterialThing> solidImpacting
        {
            get => _solidImpacting;
            set => _solidImpacting = value;
        }

        public byte planeOfExistence
        {
            get => _planeOfExistence;
            set => _planeOfExistence = value;
        }

        public bool grounded
        {
            get => _grounded;
            set => _grounded = value;
        }

        public float bouncy
        {
            get => _bouncy;
            set => _bouncy = value;
        }

        public float breakForce
        {
            get => _breakForce;
            set => _breakForce = value;
        }

        public virtual bool destroyed => _destroyed;

        public float impactThreshold
        {
            get => _impactThreshold;
            set => _impactThreshold = value;
        }

        public virtual float weight
        {
            get => _weight;
            set => _weight = value;
        }

        public float weightMultiplier
        {
            get
            {
                float num = weight;
                if (num < 5.0)
                    num = 5f;
                return 5f / num;
            }
        }

        public float weightMultiplierSmall
        {
            get
            {
                float num = weight * 0.75f;
                if (num < 5.0)
                    num = 5f;
                return 5f / num;
            }
        }

        public float weightMultiplierInv
        {
            get
            {
                float num = weight;
                if (num < 5.0)
                    num = 5f;
                return num / 5f;
            }
        }

        public float weightMultiplierInvTotal => weight / 5f;

        public bool islandDirty
        {
            get => _islandDirty;
            set => _islandDirty = value;
        }

        public CollisionIsland island
        {
            get => _island;
            set => _island = value;
        }

        public virtual void Zap(Thing zapper) => _zapper = zapper;

        public void CheckIsland()
        {
            if (island == null || island.owner == this || level == null || !level.simulatePhysics || (position - island.owner.position).lengthSq <= island.radiusSquared)
                return;
            island.RemoveThing(this);
            UpdateIsland();
        }

        public void UpdateIsland()
        {
            CollisionIsland island = Level.current.things.GetIsland(position);
            if (this.island != null && this.island != island)
                this.island.RemoveThing(this);
            if (island != null)
            {
                if (this.island != island)
                    island.AddThing(this);
            }
            else
                Level.current.things.AddIsland(this);
            islandDirty = false;
        }

        public override void DoInitialize() => base.DoInitialize();

        public override void DoTerminate()
        {
            if (island != null)
                island.RemoveThing(this);
            base.DoTerminate();
        }

        public virtual float impactPowerH => Math.Abs(hSpeed) * weightMultiplierInvTotal;

        public virtual float impactPowerV => Math.Abs(vSpeed) * weightMultiplierInvTotal;

        public float impactDirectionH => hSpeed * weightMultiplierInvTotal;

        public float impactDirectionV => vSpeed * weightMultiplierInvTotal;

        public float totalImpactPower => impactPowerH + impactPowerV;

        public Vec2 impactDirection => new Vec2(impactDirectionH, impactDirectionV);

        public Organizer<ImpactedFrom, string> collideSounds
        {
            get => _collideSounds;
            set => _collideSounds = value;
        }

        public float impactVolume
        {
            get => _impactVolume;
            set => _impactVolume = value;
        }

        public MaterialThing(float x, float y)
          : base(x, y)
        {
        }

        public void Bounce(ImpactedFrom direction)
        {
            if (direction == ImpactedFrom.Left || direction == ImpactedFrom.Right)
                BounceH();
            else
                BounceV();
        }

        public void BounceH() => hSpeed = -hSpeed * bouncy;

        public void BounceV() => vSpeed = -vSpeed * bouncy;

        public override void Update() => _didImpactSound = false;

        public Thing lastBurnedBy => _lastBurnedBy;

        public virtual void UpdateOnFire()
        {
            if (burnt < 1.0)
            {
                if (_flameWait > 1.0)
                {
                    AddFire();
                    _flameWait = 0f;
                }
                _flameWait += 0.03f;
                burnt += burnSpeed;
            }
            else
                Destroy(new DTIncinerate(_lastBurnedBy));
        }

        public override void DoUpdate()
        {
            if (spreadExtinguisherSmoke > 0.0)
            {
                spreadExtinguisherSmoke -= 0.15f;
                if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 2f)
                {
                    ++extWait;
                    if (extWait >= 3)
                    {
                        JetpackSmoke t = new JetpackSmoke(x + Rando.Float(-1f, 1f), bottom + Rando.Float(-4f, 1f))
                        {
                            depth = (Depth)0.9f
                        };
                        Level.current.AddThing(t);
                        t.hSpeed += hSpeed * Rando.Float(0.2f, 0.3f);
                        t.vSpeed = Rando.Float(-0.1f, 0f);
                        t.vSpeed -= Math.Abs(vSpeed) * Rando.Float(0.05f, 0.1f);
                        extWait = 0;
                    }
                }
            }
            if (heat != 0f)
            {
                if (heat > 0f)
                    heat -= coolingFactor;
                else if (heat < -0.01f)
                    heat += coolingFactor;
                else
                    heat = 0f;
            }
            //if (heat > 0f)
            //    heat -= coolingFactor;
            //else if (heat < -0.01f)
            //    heat += coolingFactor;
            //else
            //    heat = 0f;
            if (_onFire && isServerForObject)
                UpdateOnFire();
            base.DoUpdate();
        }

        public void NetworkDestroy() => OnDestroy(new DTImpact(this));

        public virtual bool Destroy(DestroyType type = null)
        {
            if (!_destroyed)
            {
                _destroyed = OnDestroy(type);
                if (isServerForObject && (_destroyed || _sendDestroyMessage && !_sentDestroyMessage) && isStateObject)
                {
                    Send.Message(new NMDestroyProp(this));
                    _sentDestroyMessage = true;
                }
            }
            return _destroyed;
        }

        protected virtual bool OnDestroy(DestroyType type = null) => false;

        public virtual void Regenerate() => _destroyed = false;

        public virtual bool DoHit(Bullet bullet, Vec2 hitPos) => Hit(bullet, hitPos);

        public virtual bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (physicsMaterial == PhysicsMaterial.Metal)
            {
                Level.Add(MetalRebound.New(hitPos.x, hitPos.y, bullet.travelDirNormalized.x > 0f ? 1 : -1));
                hitPos -= bullet.travelDirNormalized;
                for (int index = 0; index < 3; ++index)
                    Level.Add(Spark.New(hitPos.x, hitPos.y, bullet.travelDirNormalized));
            }
            else if (physicsMaterial == PhysicsMaterial.Wood)
            {
                hitPos -= bullet.travelDirNormalized;
                for (int index = 0; index < 3; ++index)
                {
                    WoodDebris woodDebris = WoodDebris.New(hitPos.x, hitPos.y);
                    woodDebris.hSpeed = -bullet.travelDirNormalized.x + Rando.Float(-1f, 1f);
                    woodDebris.vSpeed = -bullet.travelDirNormalized.y + Rando.Float(-1f, 1f);
                    Level.Add(woodDebris);
                }
            }
            return thickness > bullet.ammo.penetration;
        }

        public virtual void DoExitHit(Bullet bullet, Vec2 exitPos) => ExitHit(bullet, exitPos);

        public virtual void ExitHit(Bullet bullet, Vec2 exitPos)
        {
        }

        public bool onFire
        {
            get => _onFire;
            set => _onFire = value;
        }

        public virtual void Burn(Vec2 firePosition, Thing litBy)
        {
            if (Network.isActive && !isServerForObject && !isBurnMessage && !_onFire && this is Duck && (this as Duck).profile != null)
                Send.Message(new NMLightDuck(this as Duck));
            if (!isServerForObject && !isBurnMessage || _onFire || _burnWaitTimer != null && !(bool)_burnWaitTimer)
                return;
            int num = onFire ? 1 : 0;
            _onFire = OnBurn(firePosition, litBy);
            _lastBurnedBy = litBy;
        }

        public virtual void UpdateFirePosition(SmallFire f)
        {
        }

        public virtual void AddFire() => Level.Add(SmallFire.New(Rando.Float(((left - x) * 0.7f), ((right - x) * 0.7f)), Rando.Float(((top - y) * 0.7f), ((bottom - y) * 0.7f)), 0f, 0f, stick: this));

        protected virtual bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (flammable < 1f / 1000f)
                return false;
            if (!_onFire)
                SFX.Play("ignite", 0.7f, Rando.Float(0.3f) - 0.3f);
            AddFire();
            AddFire();
            return true;
        }

        public virtual void DoHeatUp(float val, Vec2 location)
        {
            bool flag = heat < 0f;
            if (!flag || val > 0f)
            {
                heat += val;
                if (heat > 1.5)
                    heat = 1.5f;
                if (!flag && heat < 0f)
                    heat = 0f;
            }
            if (val <= 0f)
                return;
            HeatUp(location);
        }

        public virtual void DoHeatUp(float val) => DoHeatUp(val, position);

        public virtual void HeatUp(Vec2 location)
        {
        }

        public virtual void DoFreeze(float val, Vec2 location)
        {
            if (val < 0f)
                val = -val;
            heat -= val;
            if (heat < -1.5)
                heat = -1.5f;
            Freeze(location);
        }

        public virtual void DoFreeze(float val) => DoFreeze(val, position);

        public virtual void Freeze(Vec2 location)
        {
        }

        public virtual void Extinquish()
        {
            foreach (SmallFire smallFire in Level.CheckCircleAll<SmallFire>(position, 20f))
            {
                if (smallFire.stick == this)
                    Level.Remove(smallFire);
            }
            _onFire = false;
            _burnWaitTimer = new ActionTimer(0.05f, reset: false);
        }

        protected float CalculateImpactPower(MaterialThing with, ImpactedFrom from) => from == ImpactedFrom.Left || from == ImpactedFrom.Right ? impactPowerH + with.impactPowerH : impactPowerV + with.impactPowerV;

        protected virtual float CalculatePersonalImpactPower(MaterialThing with, ImpactedFrom from) => Math.Abs(from == ImpactedFrom.Left || from == ImpactedFrom.Right ? impactPowerH : impactPowerV);

        public virtual void Impact(MaterialThing with, ImpactedFrom from, bool solidImpact)
        {
            if (with.ignoreCollisions || ignoreCollisions || CalculateImpactPower(with, from) <= _impactThreshold)
                return;
            if (!with.onlyCrush || from == ImpactedFrom.Top)
                OnSoftImpact(with, from);
            OnImpact(with, from);
        }

        public virtual void Touch(MaterialThing with)
        {
        }

        public virtual void SolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.ignoreCollisions || ignoreCollisions)
                return;
            double impactPower = CalculateImpactPower(with, from);
            if (impactPower > _breakForce)
                Destroy(new DTImpact(with));
            if (CalculatePersonalImpactPower(with, from) > _impactThreshold)
                _didImpactSound = PlayCollideSound(from);
            if (impactPower <= _impactThreshold)
                return;
            OnSolidImpact(with, from);
            OnImpact(with, from);
        }

        public virtual bool PlayCollideSound(ImpactedFrom from)
        {
            if (!collideSounds.HasGroup(from) || _didImpactSound)
                return false;
            if (Network.isActive)
            {
                if (isServerForObject)
                    _netCollideSound.Play();
            }
            else
                SFX.Play(collideSounds.GetRandom(from), _impactVolume);
            return true;
        }

        public virtual void OnImpact(MaterialThing with, ImpactedFrom from)
        {
        }

        public virtual void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
        }

        public virtual void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
        }

        public virtual void DrawGlow()
        {
        }
    }
}

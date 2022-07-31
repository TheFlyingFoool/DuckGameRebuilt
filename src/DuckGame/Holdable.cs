// Decompiled with JetBrains decompiler
// Type: DuckGame.Holdable
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public abstract class Holdable : PhysicsObject
    {
        public StateBinding _triggerHeldBinding = new StateBinding(nameof(_triggerHeld));
        public StateBinding _canPickUpBinding = new StateBinding(nameof(canPickUp));
        public float raiseSpeed = 0.2f;
        protected Vec2 _prevHoverPos;
        private ItemSpawner _hoverSpawner;
        protected Vec2 _lastReceivedPosition = Vec2.Zero;
        protected bool _immobilizeOwner;
        public bool _keepRaised;
        protected bool _canRaise = true;
        public bool hoverRaise = true;
        public bool ignoreHands;
        public bool hideRightWing;
        public bool hideLeftWing;
        public float angleMul = 1f;
        public Vec2 holsterOffset = Vec2.Zero;
        public float holsterAngle;
        public bool holsterable = true;
        protected bool _hasTrigger = true;
        public bool canStore = true;
        public bool canPickUp = true;
        public Vec2 handOffset;
        public Vec2 _holdOffset;
        public Vec2 _extraOffset = Vec2.Zero;
        public float handAngle;
        public bool handFlip;
        public bool triggerAction;
        public Duck _equippedDuck;
        protected bool _raised;
        public bool _triggerHeld;
        public bool tapeable = true;
        private Duck _disarmedFrom;
        private DateTime _disarmTime = DateTime.MinValue;
        public Depth _oldDepth;
        public bool _hasOldDepth;
        protected int _equippedDepth = 4;
        protected bool _fireActivated;
        public TapedGun tape;
        public int tapedIndexPreference = -1;
        public bool addVerticalTapeOffset = true;
        public float charThreshold = 100f;

        public override Thing netOwner
        {
            get => this.owner;
            set
            {
                this._prevOwner = this.owner;
                this._lastThrownBy = this._owner;
                this.owner = value;
            }
        }

        public ItemSpawner hoverSpawner
        {
            get => this._hoverSpawner;
            set
            {
                if (this._hoverSpawner != null && value == null)
                    this.gravMultiplier = 1f;
                else if (this._hoverSpawner == null && value != null)
                    this.gravMultiplier = 0f;
                if (value != null && this._hoverSpawner != value)
                    this._prevHoverPos = this.position;
                this._hoverSpawner = value;
            }
        }

        public override Vec2 netPosition
        {
            get
            {
                if (this.owner != null && !(this is DrumSet))
                    return new Vec2(-10000f, -8999f);
                return this.hoverSpawner == null ? this.position : this.hoverSpawner.position + new Vec2(0f, -8f);
            }
            set
            {
                double num = (double)Math.Abs(value.x);
                if (value.x <= -9000.0)
                    return;
                if (this.hoverSpawner == null || this._lastReceivedPosition != value || (double)(this._lastReceivedPosition - this.position).length > 25.0)
                    this.position = value;
                this._lastReceivedPosition = value;
            }
        }
        public new Duck duck
        {
            get
            {
                return this._owner as Duck;
            }
        }
        public virtual bool immobilizeOwner
        {
            get => this._immobilizeOwner;
            set => this._immobilizeOwner = value;
        }

        public virtual bool HolsterActivate(Holster pHolster) => false;

        public virtual void HolsterUpdate(Holster pHolster)
        {
        }

        public bool keepRaised => this._keepRaised;

        public bool canRaise => this._canRaise;

        public bool hasTrigger => this._hasTrigger;

        public Vec2 holdOffset => new Vec2(this._holdOffset.x * offDir, this._holdOffset.y);

        public override Vec2 center
        {
            get => this._center + this._extraOffset;
            set => this._center = value;
        }

        public override Vec2 OffsetLocal(Vec2 pos)
        {
            Vec2 vec2 = pos * this.scale - this._extraOffset;
            if (this.offDir < 0)
                vec2.x *= -1f;
            vec2 = vec2.Rotate(this.angle, new Vec2(0f, 0f));
            return vec2;
        }

        public override Vec2 ReverseOffsetLocal(Vec2 pos)
        {
            Vec2 vec2 = pos * this.scale - this._extraOffset;
            vec2 = vec2.Rotate(-this.angle, new Vec2(0f, 0f));
            return vec2;
        }

        public override bool action => (this._owner == null || this._owner.owner == this || this._owner is Duck && !(this._owner as Duck).Held(this, true) ? 0 : (this._owner.action ? 1 : 0)) != 0 || this.triggerAction;

        public Duck equippedDuck => this._equippedDuck;

        public bool raised
        {
            get => this._raised;
            set => this._raised = value;
        }

        public virtual bool CanTapeTo(Thing pThing) => true;

        public Duck disarmedFrom
        {
            get => this._disarmedFrom;
            set => this._disarmedFrom = value;
        }

        public DateTime disarmTime
        {
            get => this._disarmTime;
            set => this._disarmTime = value;
        }

        public int equippedDepth => this._equippedDepth;

        public Holdable()
        {
        }

        public Holdable(float xpos, float ypos)
          : base(xpos, ypos)
        {
        }

        public virtual void UpdateTaped(TapedGun pTaped)
        {
        }

        public virtual void PreUpdateTapedPositioning(TapedGun pTaped)
        {
        }

        public virtual void UpdateTapedPositioning(TapedGun pTaped)
        {
        }

        /// <summary>
        /// Override this to define a special taped object that two taped objects should turn into
        /// </summary>
        /// <param name="pTaped">The taped gun responsible. Make sure you check gun1 and gun2 to make sure it's the combination you're expecting (two swords = long sword for example)</param>
        /// <returns></returns>
        public virtual Holdable BecomeTapedMonster(TapedGun pTaped) => null;

        public virtual void Thrown() => this.angle = 0f;

        public virtual void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            if (this.offDir > 0)
            {
                Block block = Level.CheckLine<Block>(new Vec2(owner.x, this.y), new Vec2(this.right, this.y));
                if (block is Door && ((block as Door)._jam == 1.0 || (block as Door)._jam == -1.0))
                    block = null;
                owner.holdObstructed = block != null;
            }
            else
            {
                Block block = Level.CheckLine<Block>(new Vec2(this.left, this.y), new Vec2(owner.x, this.y));
                if (block is Door && ((block as Door)._jam == 1.0 || (block as Door)._jam == -1.0))
                    block = null;
                owner.holdObstructed = block != null;
            }
        }

        public virtual void ReturnToWorld()
        {
        }

        public virtual int PickupPriority() => !(this is CTFPresent) ? (!(this is Gun) ? (!(this is TeamHat) ? (this is Banana || this is BananaCluster ? 4 : (!(this is Hat) || this is TinfoilHat ? (!(this is Equipment) ? 3 : 2) : 3)) : 5) : 1) : 0;

        public virtual void UpdateAction()
        {
            if (!this.isServerForObject)
                return;
            bool flag1 = false;
            bool flag2 = false;
            if (this.action)
            {
                if (!this._triggerHeld)
                {
                    this.PressAction();
                    flag1 = true;
                }
                else
                    this.HoldAction();
            }
            else if (this._triggerHeld)
            {
                this.ReleaseAction();
                flag2 = true;
            }
            if (this is Gun && (flag1 || this._fireActivated))
            {
                ++(this as Gun).bulletFireIndex;
                (this as Gun).plugged = false;
            }
            if (Network.isActive && this.isServerForObject && this is Gun)
            {
                if (flag1 || (this as Gun).firedBullets.Count > 0 || this._fireActivated)
                    Send.Message(new NMFireGun(this as Gun, (this as Gun).firedBullets, (this as Gun).bulletFireIndex, !this._fireActivated && !flag1 && flag2, this.duck != null ? this.duck.netProfileIndex : (byte)4, !flag1 && !flag2), NetMessagePriority.Urgent);
                (this as Gun).firedBullets.Clear();
            }
            this._fireActivated = false;
        }

        public Thing tapedCompatriot
        {
            get
            {
                if (this.tape != null)
                {
                    if (this.tape.gun1 == this && this.tape.gun2 != this)
                        return tape.gun2;
                    if (this.tape.gun2 == this && this.tape.gun1 != this)
                        return tape.gun1;
                }
                return null;
            }
        }

        public virtual Vec2 tapedOffset => Vec2.Zero;

        public bool tapedIsGun1 => this.tape != null && this.tape.gun1 == this;

        public bool tapedIsGun2 => this.tape != null && this.tape.gun2 == this;

        public override Thing owner
        {
            get => this._owner;
            set
            {
                if (this._owner != value)
                {
                    if (this.owner is TapedGun)
                        this._prevOwner = this.owner.prevOwner;
                    else
                        this._prevOwner = this._owner;
                }
                this._lastThrownBy = this._prevOwner != null ? this._prevOwner : this._owner;
                this._owner = value;
                if (this._owner == null)
                {
                    this.solid = true;
                    this.enablePhysics = true;
                }
                else
                {
                    if (this._owner != null)
                        return;
                    this.solid = false;
                    this.enablePhysics = false;
                }
            }
        }

        public bool held => this.duck != null && this.duck.Held(this);

        public virtual void UpdateMaterial()
        {
            if (this.material == null && burnt >= (double)this.charThreshold)
            {
                this.material = new MaterialCharred();
                SFX.Play("flameExplode");
                for (int index = 0; index < 3; ++index)
                    Level.Add(SmallSmoke.New(this.x + Rando.Float(-2f, 2f), this.y + Rando.Float(-2f, 2f)));
            }
            else if (this.material == null && heat > 0.1f && this.physicsMaterial == PhysicsMaterial.Metal)
                this.material = new MaterialRedHot(this);
            else if (this.material == null && heat < -0.1f)
                this.material = new MaterialFrozen(this);
            if (this.material is MaterialRedHot)
            {
                if (heat < 0.1f)
                    this.material = null;
                else
                    (this.material as MaterialRedHot).intensity = Math.Min(this.heat - 0.1f, 1f);
            }
            else
            {
                if (!(this.material is MaterialFrozen))
                    return;
                if (heat > -0.1f)
                    this.material = null;
                else
                    (this.material as MaterialFrozen).intensity = Math.Min(Math.Abs(this.heat) - 0.1f, 1f);
            }
        }

        public override void Update()
        {
            this.UpdateMaterial();
            if (this.owner != null)
            {
                if (!this._hasOldDepth)
                {
                    this._oldDepth = this.depth;
                    this._hasOldDepth = true;
                }
                Thing thing = this.owner;
                if (this.owner is Duck && (this.owner as Duck)._trapped != null)
                    thing = (this.owner as Duck)._trapped;
                if (this.duck == null || this.duck.holdObject == this || this is Equipment)
                    this.depth = thing.depth + (this._equippedDuck != null ? this._equippedDepth : 9);
                if ((this.duck == null || this.duck.holdObject == this) && !(thing is TapedGun))
                    this.offDir = thing.offDir;
                if (this.owner is Duck owner)
                {
                    if (Network.isActive && owner.Held(this, true) && !(this is Vine) && (!(this is Equipment) || (this as Equipment).equippedDuck == null) && owner.holdObject != this && !(owner.holdObject is TapedGun) && this.isServerForObject)
                    {
                        owner.ObjectThrown(this);
                        return;
                    }
                    this._responsibleProfile = owner.profile;
                    owner.UpdateHoldPosition(false);
                }
                this._sleeping = false;
                if ((this.duck == null || this.duck.Held(this, true) || this is Equipment) && this.tape == null)
                    this.grounded = false;
                if (this.duck == null || this.duck.holdObject is TapedGun && !(this is Equipment))
                    this.UpdateAction();
                this.solidImpacting.Clear();
                this.impacting.Clear();
                this.triggerAction = false;
            }
            else
            {
                if (this._hasOldDepth)
                {
                    this.depth = this._oldDepth;
                    this._hasOldDepth = false;
                }
                if (this.owner == null || this.owner is TapedGun)
                    this.UpdateAction();
                base.Update();
                this.DoFloat();
                this.triggerAction = false;
            }
        }

        public void UpdateHoldPositioning()
        {
        }

        public virtual void PressAction()
        {
            this._triggerHeld = true;
            this.OnPressAction();
            this.HoldAction();
        }

        public virtual void OnPressAction()
        {
        }

        public void HoldAction()
        {
            this._triggerHeld = true;
            this.OnHoldAction();
        }

        public virtual void OnHoldAction()
        {
        }

        public void ReleaseAction()
        {
            this._triggerHeld = false;
            this.OnReleaseAction();
        }

        public virtual void OnReleaseAction()
        {
        }
    }
}

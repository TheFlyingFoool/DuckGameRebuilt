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
            get => owner;
            set
            {
                _prevOwner = owner;
                _lastThrownBy = _owner;
                owner = value;
            }
        }

        public ItemSpawner hoverSpawner
        {
            get => _hoverSpawner;
            set
            {
                if (_hoverSpawner != null && value == null)
                {
                    gravMultiplier = 1f;
                    initemspawner = false;
                }
                else if (_hoverSpawner == null && value != null)
                {
                    gravMultiplier = 0f;
                    initemspawner = true;
                }
                if (value != null && _hoverSpawner != value)
                {
                    _prevHoverPos = position;
                    initemspawner = true;
                }
                if (this is RagdollPart)
                {
                    initemspawner = false;
                }
                _hoverSpawner = value;
            }
        }

        public override Vec2 netPosition
        {
            get
            {
                if (owner != null && !(this is DrumSet))
                    return new Vec2(-10000f, -8999f);
                return hoverSpawner == null ? position : hoverSpawner.position + new Vec2(0f, -8f);
            }
            set
            {
                double num = Math.Abs(value.x);
                if (value.x <= -9000f)
                    return;
                if (hoverSpawner == null || _lastReceivedPosition != value || (_lastReceivedPosition - position).length > 25f)
                    position = value;
                _lastReceivedPosition = value;
            }
        }
        public new Duck duck
        {
            get
            {
                return _owner as Duck;
            }
        }
        public virtual bool immobilizeOwner
        {
            get => _immobilizeOwner;
            set => _immobilizeOwner = value;
        }

        public virtual bool HolsterActivate(Holster pHolster) => false;

        public virtual void HolsterUpdate(Holster pHolster)
        {
        }

        public bool keepRaised => _keepRaised;

        public bool canRaise => _canRaise;

        public bool hasTrigger => _hasTrigger;

        public Vec2 holdOffset => new Vec2(_holdOffset.x * offDir, _holdOffset.y);

        public override Vec2 center
        {
            get => _center + _extraOffset;
            set => _center = value;
        }

        public override Vec2 OffsetLocal(Vec2 pos)
        {
            Vec2 vec2 = pos * scale - _extraOffset;
            if (offDir < 0)
                vec2.x *= -1f;
            vec2 = vec2.Rotate(angle, new Vec2(0f, 0f));
            return vec2;
        }

        public override Vec2 ReverseOffsetLocal(Vec2 pos)
        {
            Vec2 vec2 = pos * scale - _extraOffset;
            vec2 = vec2.Rotate(-angle, new Vec2(0f, 0f));
            return vec2;
        }

        public bool forceAction;//this is for recorderator -NiK0
        public override bool action => forceAction || ((_owner == null || _owner.owner == this || _owner is Duck && !(_owner as Duck).Held(this, true) ? 0 : (_owner.action ? 1 : 0)) != 0 || triggerAction);

        public Duck equippedDuck => _equippedDuck;

        public bool raised
        {
            get => _raised;
            set => _raised = value;
        }

        public virtual bool CanTapeTo(Thing pThing) => true;

        public Duck disarmedFrom
        {
            get => _disarmedFrom;
            set => _disarmedFrom = value;
        }

        public DateTime disarmTime
        {
            get => _disarmTime;
            set => _disarmTime = value;
        }

        public int equippedDepth => _equippedDepth;

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

        public virtual void Thrown() => angle = 0f;

        public virtual void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            if (offDir > 0)
            {
                Block block = Level.CheckLine<Block>(new Vec2(owner.x, y), new Vec2(right, y));
                if (block is Door && ((block as Door)._jam == 1f || (block as Door)._jam == -1f))
                    block = null;
                owner.holdObstructed = block != null;
            }
            else
            {
                Block block = Level.CheckLine<Block>(new Vec2(left, y), new Vec2(owner.x, y));
                if (block is Door && ((block as Door)._jam == 1f || (block as Door)._jam == -1f))
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
            if (!isServerForObject)
                return;
            bool flag1 = false;
            bool flag2 = false;
            if (action)
            {
                if (!_triggerHeld)
                {
                    PressAction();
                    flag1 = true;
                }
                else
                    HoldAction();
            }
            else if (_triggerHeld)
            {
                ReleaseAction();
                flag2 = true;
            }
            if (this is Gun && (flag1 || _fireActivated))
            {
                ++(this as Gun).bulletFireIndex;
                (this as Gun).plugged = false;
            }
            if (Network.isActive && isServerForObject && this is Gun)
            {
                if (flag1 || (this as Gun).firedBullets.Count > 0 || _fireActivated)
                    Send.Message(new NMFireGun(this as Gun, (this as Gun).firedBullets, (this as Gun).bulletFireIndex, !_fireActivated && !flag1 && flag2, duck != null ? duck.netProfileIndex : (byte)4, !flag1 && !flag2), NetMessagePriority.Urgent);
                (this as Gun).firedBullets.Clear();
            }
            _fireActivated = false;
        }

        public Thing tapedCompatriot
        {
            get
            {
                if (tape != null)
                {
                    if (tape.gun1 == this && tape.gun2 != this)
                        return tape.gun2;
                    if (tape.gun2 == this && tape.gun1 != this)
                        return tape.gun1;
                }
                return null;
            }
        }

        public virtual Vec2 tapedOffset => Vec2.Zero;

        public bool tapedIsGun1 => tape != null && tape.gun1 == this;

        public bool tapedIsGun2 => tape != null && tape.gun2 == this;

        public override Thing owner
        {
            get => _owner;
            set
            {
                if (_owner != value)
                {
                    if (owner is TapedGun)
                        _prevOwner = owner.prevOwner;
                    else
                        _prevOwner = _owner;
                }
                _lastThrownBy = _prevOwner != null ? _prevOwner : _owner;
                _owner = value;
                if (_owner == null)
                {
                    solid = true;
                    enablePhysics = true;
                }
                else
                {
                    if (_owner != null)
                        return;
                    solid = false;
                    enablePhysics = false;
                }
            }
        }

        public bool held => duck != null && duck.Held(this);

        public virtual void UpdateMaterial()
        {
            if (material == null)
            {
                if (burnt >= charThreshold)
                {
                    material = new MaterialCharred();
                    SFX.Play("flameExplode");
                    for (int index = 0; index < 3; ++index)
                        Level.Add(SmallSmoke.New(x + Rando.Float(-2f, 2f), y + Rando.Float(-2f, 2f)));
                }
                else if (heat > 0.1f && physicsMaterial == PhysicsMaterial.Metal)
                    material = new MaterialRedHot(this);
                else if (heat < -0.1f)
                    material = new MaterialFrozen(this);
            }
            if (material is MaterialRedHot)
            {
                if (heat < 0.1f)
                    material = null;
                else
                    (material as MaterialRedHot).intensity = Math.Min(heat - 0.1f, 1f);
            }
            else if (material is MaterialFrozen)
            {
                if (heat > -0.1f)
                    material = null;
                else
                    (material as MaterialFrozen).intensity = Math.Min(Math.Abs(heat) - 0.1f, 1f);
            }
        }

        
        public override void Update()
        {
            UpdateMaterial();
            if (owner != null)
            {
                if (!_hasOldDepth)
                {
                    _oldDepth = depth;
                    _hasOldDepth = true;
                }
                Thing thing = this.owner;
                if (this.owner is Duck && (this.owner as Duck)._trapped != null)
                    thing = (this.owner as Duck)._trapped;
                if (duck == null || duck.holdObject == this || this is Equipment)
                    depth = thing.depth + (_equippedDuck != null ? _equippedDepth : 9);
                if ((duck == null || duck.holdObject == this) && !(thing is TapedGun))
                    offDir = thing.offDir;
                if (this.owner is Duck owner)
                {
                    if (Network.isActive && owner.Held(this, true) && !(this is Vine) && (!(this is Equipment) || (this as Equipment).equippedDuck == null) && owner.holdObject != this && !(owner.holdObject is TapedGun) && isServerForObject)
                    {
                        owner.ObjectThrown(this);
                        return;
                    }
                    _responsibleProfile = owner.profile;
                    owner.UpdateHoldPosition(false);
                }
                _sleeping = false;
                if ((duck == null || duck.Held(this, true) || this is Equipment) && tape == null)
                    grounded = false;
                if (duck == null || duck.holdObject is TapedGun && !(this is Equipment))
                    UpdateAction();
                solidImpacting.Clear();
                impacting.Clear();
                triggerAction = false;
            }
            else
            {
                if (_hasOldDepth)
                {
                    depth = _oldDepth;
                    _hasOldDepth = false;
                }
                if (owner == null || owner is TapedGun)
                    UpdateAction();
                base.Update();
                DoFloat();
                triggerAction = false;
            }
        }

        public void UpdateHoldPositioning()
        {
        }

        public virtual void PressAction()
        {
            _triggerHeld = true;
            OnPressAction();
            HoldAction();
        }

        public virtual void OnPressAction()
        {
        }

        public void HoldAction()
        {
            _triggerHeld = true;
            OnHoldAction();
        }

        public virtual void OnHoldAction()
        {
        }

        public void ReleaseAction()
        {
            _triggerHeld = false;
            OnReleaseAction();
        }

        public virtual void OnReleaseAction()
        {
        }
    }
}

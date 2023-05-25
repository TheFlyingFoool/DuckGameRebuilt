// Decompiled with JetBrains decompiler
// Type: DuckGame.Equipment
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public abstract class Equipment : Holdable
    {
        public StateBinding _equippedBinding = new StateBinding(nameof(netEquippedDuck));
        public StateBinding _equipIndexBinding = new StateBinding(nameof(equipIndex));
        public NetIndex4 equipIndex = new NetIndex4(0);
        public NetIndex4 localEquipIndex = new NetIndex4(0);
        protected bool _hasEquippedCollision;
        protected Vec2 _equippedCollisionOffset;
        protected Vec2 _equippedCollisionSize;
        protected bool _jumpMod;
        protected Vec2 _offset;
        protected bool _autoOffset = true;
        //private Vec2 _colSize = Vec2.Zero;
        private float _equipmentHealth = 1f;
        public float autoEquipTime;
        public bool _prevEquipped;
        protected Vec2 _wearOffset = Vec2.Zero;
        public bool wearable = true;
        protected bool _isArmor;
        protected float _equippedThickness = 0.1f;
        private bool _appliedEquippedCollision;
        private Vec2 _unequippedCollisionSize;
        //private Vec2 _unequippedCollisionOffset;

        public Duck netEquippedDuck
        {
            get => _equippedDuck;
            set
            {
                if (_equippedDuck != value && _equippedDuck != null)
                    _equippedDuck.Unequip(this, true);
                if (_equippedDuck != value && value != null)
                    value.Equip(this, false, true);
                _equippedDuck = value;
            }
        }

        public override Vec2 collisionOffset
        {
            get => !_hasEquippedCollision || _equippedDuck == null ? _collisionOffset : _equippedCollisionOffset;
            set => _collisionOffset = value;
        }

        public override Vec2 collisionSize
        {
            get => !_hasEquippedCollision || _equippedDuck == null ? _collisionSize : _equippedCollisionSize;
            set => _collisionSize = value;
        }

        public bool jumpMod => _jumpMod;

        public Vec2 wearOffset
        {
            get => _wearOffset;
            set => _wearOffset = value;
        }

        public bool isArmor => _isArmor;

        public Equipment(float xpos, float ypos)
          : base(xpos, ypos)
        {
            weight = 2f;
            thickness = 0.1f;
        }

        public override void Terminate()
        {
            if (_equippedDuck != null)
                _equippedDuck.Unequip(this, true);
            base.Terminate();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            switch (type)
            {
                case DTImpale _:
                case DTImpact _:
                    SFX.Play("smallDestroy", 0.8f, Rando.Float(-0.1f, 0.1f));
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 3; ++index)
                        Level.Add(SmallSmoke.New(x + Rando.Float(-2f, 2f), y + Rando.Float(-2f, 2f)));
                    break;
            }
            return true;
        }

        public void PositionOnOwner()
        {
            if (_equippedDuck == null)
                return;
            Duck duck = this.duck;
            if (duck == null || duck.skeleton == null)
                return;
            DuckBone duckBone = this.duck.skeleton.upperTorso;
            switch (this)
            {
                case Hat _:
                case ChokeCollar _:
                    duckBone = this.duck.skeleton.head;
                    break;
                case Boots _:
                    offDir = owner.offDir;
                    duckBone = this.duck.skeleton.lowerTorso;
                    break;
            }
            offDir = owner.offDir;
            position = duckBone.position;
            angle = offDir > 0 ? -duckBone.orientation : duckBone.orientation;
            Vec2 wearOffset = _wearOffset;
            if (this is TeamHat)
                wearOffset -= (this as TeamHat).hatOffset;
            position += new Vec2(wearOffset.x * offDir, wearOffset.y).Rotate(angle, Vec2.Zero);
        }

        public override void Update()
        {
            if (autoEquipTime > 0) autoEquipTime -= 0.016f;
            else autoEquipTime = 0f;
            if (isServerForObject)
            {
                if (_equipmentHealth <= 0 && _equippedDuck != null && duck != null)
                {
                    duck.KnockOffEquipment(this);
                    if (Network.isActive)
                        NetSoundEffect.Play("equipmentTing");
                }
                _equipmentHealth = Lerp.Float(_equipmentHealth, 1f, 0.003f);
            }
            UpdateEquippedCollision();
            if (destroyed)
            {
                alpha -= 0.1f;
                if (alpha < 0)
                    Level.Remove(this);
            }
            if (localEquipIndex < equipIndex)
            {
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                    Level.Add(SmallSmoke.New(x + Rando.Float(-2f, 2f), y + Rando.Float(-2f, 2f)));
                SFX.Play("equip", 0.8f);
                localEquipIndex = equipIndex;
            }
            _prevEquipped = _equippedDuck != null;
            thickness = _equippedDuck != null ? _equippedThickness : 0.1f;
            if (Network.isActive && _equippedDuck != null && duck != null && !duck.HasEquipment(this))
                duck.Equip(this, false);
            PositionOnOwner();
            base.Update();
        }

        public new void Hurt(float amount)
        {
            _equipmentHealth -= amount;
        }
        private void UpdateEquippedCollision()
        {
            if (_hasEquippedCollision && _equippedDuck != null)
            {
                if (!_appliedEquippedCollision)
                {
                    _unequippedCollisionSize = _collisionSize;
                    //this._unequippedCollisionOffset = this._collisionOffset;
                }
                collisionSize = _equippedCollisionSize;
                collisionOffset = _equippedCollisionOffset;
                _appliedEquippedCollision = true;
            }
            else
            {
                if (!_appliedEquippedCollision)
                    return;
                collisionSize = _unequippedCollisionSize;
                collisionOffset = _equippedCollisionOffset;
                _appliedEquippedCollision = false;
            }
        }

        public virtual void Equip(Duck d)
        {
            if (_equippedDuck == null)
            {
                owner = d;
                solid = false;
                _equippedDuck = d;
            }
            UpdateEquippedCollision();
        }

        public virtual void UnEquip()
        {
            if (_equippedDuck != null)
            {
                owner = null;
                solid = true;
                _equippedDuck = null;
            }
            UpdateEquippedCollision();
        }

        public override void PressAction()
        {
            if (_equippedDuck == null && held)
            {
                if (!(this.owner is Duck owner))
                    return;
                RumbleManager.AddRumbleEvent(owner.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
                owner.ThrowItem();
                owner.Equip(this);
            }
            else
                base.PressAction();
        }

        public override void Draw()
        {
            PositionOnOwner();
            base.Draw();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (_equippedDuck == null && autoEquipTime > 0f)
            {
                Duck d = with as Duck;
                if (d == null && with is FeatherVolume)
                {
                    d = (with as FeatherVolume).duckOwner;
                }
                if (d != null)
                {
                    d.Equip(this, true, false);
                    if (this is ChokeCollar)
                    {
                        (this as ChokeCollar).ball.hSpeed = 0f;
                        (this as ChokeCollar).ball.vSpeed = 0f;
                    }
                }
            }
            base.OnSoftImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_equippedDuck == null || bullet.owner == duck || !bullet.isLocal)
                return false;
            if (!_isArmor || duck == null)
                return base.Hit(bullet, hitPos);
            if (bullet.isLocal)
            {
                duck.KnockOffEquipment(this, b: bullet);
                Fondle(this, DuckNetwork.localConnection);
            }
            if (bullet.isLocal && Network.isActive)
                NetSoundEffect.Play("equipmentTing");
            bullet.hitArmor = true;
            Level.Add(MetalRebound.New(hitPos.x, hitPos.y, bullet.travelDirNormalized.x > 0 ? 1 : -1));
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                Level.Add(Spark.New(x, y, bullet.travelDirNormalized));
            return base.Hit(bullet, hitPos);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.Equipment
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private Vec2 _colSize = Vec2.Zero;
        private float _equipmentHealth = 1f;
        public float autoEquipTime;
        public bool _prevEquipped;
        protected Vec2 _wearOffset = Vec2.Zero;
        public bool wearable = true;
        protected bool _isArmor;
        protected float _equippedThickness = 0.1f;
        private bool _appliedEquippedCollision;
        private Vec2 _unequippedCollisionSize;
        private Vec2 _unequippedCollisionOffset;

        public Duck netEquippedDuck
        {
            get => this._equippedDuck;
            set
            {
                if (this._equippedDuck != value && this._equippedDuck != null)
                    this._equippedDuck.Unequip(this, true);
                if (this._equippedDuck != value && value != null)
                    value.Equip(this, false, true);
                this._equippedDuck = value;
            }
        }

        public override Vec2 collisionOffset
        {
            get => !this._hasEquippedCollision || this._equippedDuck == null ? this._collisionOffset : this._equippedCollisionOffset;
            set => this._collisionOffset = value;
        }

        public override Vec2 collisionSize
        {
            get => !this._hasEquippedCollision || this._equippedDuck == null ? this._collisionSize : this._equippedCollisionSize;
            set => this._collisionSize = value;
        }

        public bool jumpMod => this._jumpMod;

        public Vec2 wearOffset
        {
            get => this._wearOffset;
            set => this._wearOffset = value;
        }

        public bool isArmor => this._isArmor;

        public Equipment(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.weight = 2f;
            this.thickness = 0.1f;
        }

        public override void Terminate()
        {
            if (this._equippedDuck != null)
                this._equippedDuck.Unequip(this, true);
            base.Terminate();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            switch (type)
            {
                case DTImpale _:
                case DTImpact _:
                    SFX.Play("smallDestroy", 0.8f, Rando.Float(-0.1f, 0.1f));
                    for (int index = 0; index < 3; ++index)
                        Level.Add((Thing)SmallSmoke.New(this.x + Rando.Float(-2f, 2f), this.y + Rando.Float(-2f, 2f)));
                    break;
            }
            return true;
        }

        public void PositionOnOwner()
        {
            if (this._equippedDuck == null)
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
                    this.offDir = this.owner.offDir;
                    duckBone = this.duck.skeleton.lowerTorso;
                    break;
            }
            this.offDir = this.owner.offDir;
            this.position = duckBone.position;
            this.angle = this.offDir > (sbyte)0 ? -duckBone.orientation : duckBone.orientation;
            Vec2 wearOffset = this._wearOffset;
            if (this is TeamHat)
                wearOffset -= (this as TeamHat).hatOffset;
            this.position = this.position + new Vec2(wearOffset.x * (float)this.offDir, wearOffset.y).Rotate(this.angle, Vec2.Zero);
        }

        public override void Update()
        {
            if ((double)this.autoEquipTime > 0.0)
                this.autoEquipTime -= 0.016f;
            else
                this.autoEquipTime = 0.0f;
            if (this.isServerForObject)
            {
                if ((double)this._equipmentHealth <= 0.0 && this._equippedDuck != null && this.duck != null)
                {
                    this.duck.KnockOffEquipment(this);
                    if (Network.isActive)
                        NetSoundEffect.Play("equipmentTing");
                }
                this._equipmentHealth = Lerp.Float(this._equipmentHealth, 1f, 3f / 1000f);
            }
            this.UpdateEquippedCollision();
            if (this.destroyed)
            {
                this.alpha -= 0.1f;
                if ((double)this.alpha < 0.0)
                    Level.Remove((Thing)this);
            }
            if (this.localEquipIndex < this.equipIndex)
            {
                for (int index = 0; index < 2; ++index)
                    Level.Add((Thing)SmallSmoke.New(this.x + Rando.Float(-2f, 2f), this.y + Rando.Float(-2f, 2f)));
                SFX.Play("equip", 0.8f);
                this.localEquipIndex = this.equipIndex;
            }
            this._prevEquipped = this._equippedDuck != null;
            this.thickness = this._equippedDuck != null ? this._equippedThickness : 0.1f;
            if (Network.isActive && this._equippedDuck != null && this.duck != null && !this.duck.HasEquipment(this))
                this.duck.Equip(this, false);
            this.PositionOnOwner();
            base.Update();
        }

        public void Hurt(float amount) => this._equipmentHealth -= amount;

        private void UpdateEquippedCollision()
        {
            if (this._hasEquippedCollision && this._equippedDuck != null)
            {
                if (!this._appliedEquippedCollision)
                {
                    this._unequippedCollisionSize = this._collisionSize;
                    this._unequippedCollisionOffset = this._collisionOffset;
                }
                this.collisionSize = this._equippedCollisionSize;
                this.collisionOffset = this._equippedCollisionOffset;
                this._appliedEquippedCollision = true;
            }
            else
            {
                if (!this._appliedEquippedCollision)
                    return;
                this.collisionSize = this._unequippedCollisionSize;
                this.collisionOffset = this._equippedCollisionOffset;
                this._appliedEquippedCollision = false;
            }
        }

        public virtual void Equip(Duck d)
        {
            if (this._equippedDuck == null)
            {
                this.owner = (Thing)d;
                this.solid = false;
                this._equippedDuck = d;
            }
            this.UpdateEquippedCollision();
        }

        public virtual void UnEquip()
        {
            if (this._equippedDuck != null)
            {
                this.owner = (Thing)null;
                this.solid = true;
                this._equippedDuck = (Duck)null;
            }
            this.UpdateEquippedCollision();
        }

        public override void PressAction()
        {
            if (this._equippedDuck == null && this.held)
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
            this.PositionOnOwner();
            base.Draw();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._equippedDuck == null && this.autoEquipTime > 0f)
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
            if (this._equippedDuck == null || bullet.owner == this.duck || !bullet.isLocal)
                return false;
            if (!this._isArmor || this.duck == null)
                return base.Hit(bullet, hitPos);
            if (bullet.isLocal)
            {
                this.duck.KnockOffEquipment(this, b: bullet);
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            }
            if (bullet.isLocal && Network.isActive)
                NetSoundEffect.Play("equipmentTing");
            bullet.hitArmor = true;
            Level.Add((Thing)MetalRebound.New(hitPos.x, hitPos.y, (double)bullet.travelDirNormalized.x > 0.0 ? 1 : -1));
            for (int index = 0; index < 6; ++index)
                Level.Add((Thing)Spark.New(this.x, this.y, bullet.travelDirNormalized));
            return base.Hit(bullet, hitPos);
        }
    }
}

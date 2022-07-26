// Decompiled with JetBrains decompiler
// Type: DuckGame.NMFireGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NMFireGun : NMEvent
    {
        public Gun gun;
        public byte fireIndex;
        public List<Bullet> bullets;
        private List<NMFireBullet> _fireEvents = new List<NMFireBullet>();
        public byte ammoType;
        public Vec2 position;
        public byte owner;
        public bool release;
        public bool onlyFireAction;
        private AmmoType _ammoTypeInstance;
        private byte _levelIndex;

        public NMFireGun()
        {
        }

        public NMFireGun(
          Gun g,
          List<Bullet> varBullets,
          byte fIndex,
          bool rel,
          byte ownerIndex = 4,
          bool onlyFireActionVar = false)
        {
            this.gun = g;
            this.bullets = varBullets;
            this.fireIndex = fIndex;
            this.owner = ownerIndex;
            this.release = rel;
            this.onlyFireAction = onlyFireActionVar;
            if (varBullets == null)
                return;
            bool flag = true;
            foreach (Bullet varBullet in varBullets)
            {
                if (flag)
                {
                    this._ammoTypeInstance = varBullet.ammo;
                    this.ammoType = AmmoType.indexTypeMap[varBullet.ammo.GetType()];
                    this.position = new Vec2(varBullet.x, varBullet.y);
                    flag = false;
                }
                this._fireEvents.Add(new NMFireBullet(varBullet.range, varBullet.bulletSpeed, varBullet.angle));
            }
        }

        public override void Activate()
        {
            if ((int)this._levelIndex != (int)DuckNetwork.levelIndex)
                return;
            if (this._fireEvents.Count > 0 && this._fireEvents[0].typeInstance != null)
                this._fireEvents[0].typeInstance.MakeNetEffect(this.position, true);
            foreach (NMFireBullet fireEvent in this._fireEvents)
            {
                fireEvent.connection = this.connection;
                fireEvent.DoActivate(this.position, (int)this.owner < DuckNetwork.profiles.Count ? DuckNetwork.profiles[(int)this.owner] : (Profile)null);
            }
            if (this.gun == null)
                return;
            if (this._fireEvents.Count > 0)
                this.gun.OnNetworkBulletsFired(this.position);
            this.gun.receivingPress = true;
            this.gun.hasFireEvents = true;
            this.gun.onlyFireAction = this.onlyFireAction;
            float wait = this.gun._wait;
            this.gun._wait = 0.0f;
            if (!this.release)
            {
                bool loaded = this.gun.loaded;
                this.gun.loaded = this._fireEvents.Count > 0 || this.gun.ammo == 0;
                this.gun.PressAction();
                if (this.gun.fullAuto)
                    this.gun.HoldAction();
                this.gun.loaded = loaded;
            }
            else
                this.gun.ReleaseAction();
            this.gun._wait = wait;
            this.gun.receivingPress = false;
            this.gun.hasFireEvents = false;
            this.gun.onlyFireAction = false;
            this.gun.bulletFireIndex = this.fireIndex;
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            this._serializedData.Write(DuckNetwork.levelIndex);
            this._serializedData.Write((byte)this._fireEvents.Count);
            foreach (NMFireBullet fireEvent in this._fireEvents)
            {
                fireEvent.SerializePacketData();
                this._serializedData.Write(fireEvent.serializedData, true);
                if (this._fireEvents.Count > 0)
                    this._ammoTypeInstance.WriteAdditionalData(this._serializedData);
            }
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            this._levelIndex = d.ReadByte();
            byte num = d.ReadByte();
            for (int index = 0; index < (int)num; ++index)
            {
                NMFireBullet nmFireBullet = new NMFireBullet();
                BitBuffer msg = d.ReadBitBuffer();
                nmFireBullet.OnDeserialize(msg);
                AmmoType instance = Activator.CreateInstance(AmmoType.indexTypeMap[this.ammoType]) as AmmoType;
                instance.ReadAdditionalData(d);
                nmFireBullet.typeInstance = instance;
                this._fireEvents.Add(nmFireBullet);
            }
        }
    }
}

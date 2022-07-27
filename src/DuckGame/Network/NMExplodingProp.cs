// Decompiled with JetBrains decompiler
// Type: DuckGame.NMExplodingProp
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NMExplodingProp : NMEvent
    {
        private byte _levelIndex;
        public List<Bullet> bullets;
        private AmmoType _ammoTypeInstance;
        public byte ammoType;
        public Vec2 position;
        private List<NMFireBullet> _fireEvents = new List<NMFireBullet>();

        public NMExplodingProp()
        {
        }

        public NMExplodingProp(List<Bullet> varBullets)
        {
            this.bullets = varBullets;
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
            if (_levelIndex != DuckNetwork.levelIndex)
                return;
            if (this._fireEvents.Count > 0 && this._fireEvents[0].typeInstance != null)
                this._fireEvents[0].typeInstance.MakeNetEffect(this.position, true);
            foreach (NMFireBullet fireEvent in this._fireEvents)
                fireEvent.DoActivate(this.position, null);
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
            for (int index = 0; index < num; ++index)
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

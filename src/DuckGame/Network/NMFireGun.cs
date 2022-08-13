// Decompiled with JetBrains decompiler
// Type: DuckGame.NMFireGun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            gun = g;
            bullets = varBullets;
            fireIndex = fIndex;
            owner = ownerIndex;
            release = rel;
            onlyFireAction = onlyFireActionVar;
            if (varBullets == null)
                return;
            bool flag = true;
            foreach (Bullet varBullet in varBullets)
            {
                if (flag)
                {
                    _ammoTypeInstance = varBullet.ammo;
                    ammoType = AmmoType.indexTypeMap[varBullet.ammo.GetType()];
                    position = new Vec2(varBullet.x, varBullet.y);
                    flag = false;
                }
                _fireEvents.Add(new NMFireBullet(varBullet.range, varBullet.bulletSpeed, varBullet.angle));
            }
        }

        public override void Activate()
        {
            if (_levelIndex != DuckNetwork.levelIndex)
                return;
            if (_fireEvents.Count > 0 && _fireEvents[0].typeInstance != null)
                _fireEvents[0].typeInstance.MakeNetEffect(position, true);
            foreach (NMFireBullet fireEvent in _fireEvents)
            {
                fireEvent.connection = connection;
                fireEvent.DoActivate(position, owner < DuckNetwork.profiles.Count ? DuckNetwork.profiles[owner] : null);
            }
            if (gun == null)
                return;
            if (_fireEvents.Count > 0)
                gun.OnNetworkBulletsFired(position);
            gun.receivingPress = true;
            gun.hasFireEvents = true;
            gun.onlyFireAction = onlyFireAction;
            float wait = gun._wait;
            gun._wait = 0f;
            if (!release)
            {
                bool loaded = gun.loaded;
                gun.loaded = _fireEvents.Count > 0 || gun.ammo == 0;
                gun.PressAction();
                if (gun.fullAuto)
                    gun.HoldAction();
                gun.loaded = loaded;
            }
            else
                gun.ReleaseAction();
            gun._wait = wait;
            gun.receivingPress = false;
            gun.hasFireEvents = false;
            gun.onlyFireAction = false;
            gun.bulletFireIndex = fireIndex;
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            _serializedData.Write(DuckNetwork.levelIndex);
            _serializedData.Write((byte)_fireEvents.Count);
            foreach (NMFireBullet fireEvent in _fireEvents)
            {
                fireEvent.SerializePacketData();
                _serializedData.Write(fireEvent.serializedData, true);
                if (_fireEvents.Count > 0)
                    _ammoTypeInstance.WriteAdditionalData(_serializedData);
            }
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            _levelIndex = d.ReadByte();
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
            {
                NMFireBullet nmFireBullet = new NMFireBullet();
                BitBuffer msg = d.ReadBitBuffer();
                nmFireBullet.OnDeserialize(msg);
                AmmoType instance = Activator.CreateInstance(AmmoType.indexTypeMap[ammoType]) as AmmoType;
                instance.ReadAdditionalData(d);
                nmFireBullet.typeInstance = instance;
                _fireEvents.Add(nmFireBullet);
            }
        }
    }
}

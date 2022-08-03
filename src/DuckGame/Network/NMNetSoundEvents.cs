// Decompiled with JetBrains decompiler
// Type: DuckGame.NMNetSoundEvents
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NMNetSoundEvents : NMEvent
    {
        private List<NetSoundEffect> _sounds = new List<NetSoundEffect>();

        public NMNetSoundEvents()
        {
            manager = BelongsToManager.EventManager;
            priority = NetMessagePriority.UnreliableUnordered;
        }

        public NMNetSoundEvents(List<NetSoundEffect> pSounds)
        {
            manager = BelongsToManager.EventManager;
            priority = NetMessagePriority.UnreliableUnordered;
            foreach (NetSoundEffect pSound in pSounds)
                _sounds.Add(pSound);
        }

        protected override void OnSerialize()
        {
            byte val = (byte)Math.Min(_sounds.Count, 16);
            _serializedData.Write(val);
            for (int index = 0; index < val; ++index)
                _serializedData.Write(_sounds[index].sfxIndex);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            _sounds.Clear();
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
            {
                NetSoundEffect netSoundEffect = NetSoundEffect.Get(d.ReadUShort());
                if (netSoundEffect != null)
                    _sounds.Add(netSoundEffect);
            }
        }

        public override void Activate()
        {
            foreach (NetSoundEffect sound in _sounds)
                sound.Play();
        }
    }
}

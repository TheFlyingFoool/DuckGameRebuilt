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
            this.manager = BelongsToManager.EventManager;
            this.priority = NetMessagePriority.UnreliableUnordered;
        }

        public NMNetSoundEvents(List<NetSoundEffect> pSounds)
        {
            this.manager = BelongsToManager.EventManager;
            this.priority = NetMessagePriority.UnreliableUnordered;
            foreach (NetSoundEffect pSound in pSounds)
                this._sounds.Add(pSound);
        }

        protected override void OnSerialize()
        {
            byte val = (byte)Math.Min(this._sounds.Count, 16);
            this._serializedData.Write(val);
            for (int index = 0; index < (int)val; ++index)
                this._serializedData.Write(this._sounds[index].sfxIndex);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            this._sounds.Clear();
            byte num = d.ReadByte();
            for (int index = 0; index < (int)num; ++index)
            {
                NetSoundEffect netSoundEffect = NetSoundEffect.Get(d.ReadUShort());
                if (netSoundEffect != null)
                    this._sounds.Add(netSoundEffect);
            }
        }

        public override void Activate()
        {
            foreach (NetSoundEffect sound in this._sounds)
                sound.Play();
        }
    }
}

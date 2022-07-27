// Decompiled with JetBrains decompiler
// Type: DuckGame.NMDestroyBlocks
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class NMDestroyBlocks : NMEvent
    {
        public HashSet<ushort> blocks = new HashSet<ushort>();
        private byte _levelIndex;

        public NMDestroyBlocks(HashSet<ushort> varBlocks) => this.blocks = varBlocks;

        public NMDestroyBlocks()
        {
        }

        public override void Activate()
        {
            if (!(Level.current is GameLevel) || DuckNetwork.levelIndex != _levelIndex)
                return;
            foreach (BlockGroup blockGroup in Level.current.things[typeof(BlockGroup)])
            {
                bool flag = false;
                foreach (ushort block1 in this.blocks)
                {
                    ushort u = block1;
                    Block block2 = blockGroup.blocks.FirstOrDefault<Block>(x => x is AutoBlock && (x as AutoBlock).blockIndex == u);
                    if (block2 != null)
                    {
                        block2.shouldWreck = true;
                        flag = true;
                    }
                }
                if (flag)
                    blockGroup.Wreck();
            }
            foreach (AutoBlock autoBlock in Level.current.things[typeof(AutoBlock)])
            {
                if (this.blocks.Contains(autoBlock.blockIndex))
                {
                    this.blocks.Remove(autoBlock.blockIndex);
                    autoBlock.shouldWreck = true;
                    autoBlock.skipWreck = true;
                }
            }
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            this._serializedData.Write(DuckNetwork.levelIndex);
            this._serializedData.Write((byte)this.blocks.Count);
            foreach (ushort block in this.blocks)
                this._serializedData.Write(block);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            this._levelIndex = d.ReadByte();
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
                this.blocks.Add(d.ReadUShort());
        }
    }
}

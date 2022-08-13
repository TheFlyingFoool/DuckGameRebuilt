// Decompiled with JetBrains decompiler
// Type: DuckGame.NMDestroyBlocks
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class NMDestroyBlocks : NMEvent
    {
        public HashSet<ushort> blocks = new HashSet<ushort>();
        private byte _levelIndex;

        public NMDestroyBlocks(HashSet<ushort> varBlocks) => blocks = varBlocks;

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
                foreach (ushort block1 in blocks)
                {
                    ushort u = block1;
                    Block block2 = blockGroup.blocks.FirstOrDefault<Block>(x => x is AutoBlock && (x as AutoBlock).blockIndex == u);
                    if (block2 != null)
                    {
                        Level.current.AddUpdateOnce(block2);
                        block2.shouldWreck = true;
                        flag = true;
                    }
                }
                if (flag)
                    blockGroup.Wreck();
            }
            foreach (AutoBlock autoBlock in Level.current.things[typeof(AutoBlock)])
            {
                if (blocks.Contains(autoBlock.blockIndex))
                {
                    blocks.Remove(autoBlock.blockIndex);
                    autoBlock.shouldWreck = true;
                    autoBlock.skipWreck = true;
                    Level.current.AddUpdateOnce(autoBlock);
                }
            }
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            _serializedData.Write(DuckNetwork.levelIndex);
            _serializedData.Write((byte)blocks.Count);
            foreach (ushort block in blocks)
                _serializedData.Write(block);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            _levelIndex = d.ReadByte();
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
                blocks.Add(d.ReadUShort());
        }
    }
}

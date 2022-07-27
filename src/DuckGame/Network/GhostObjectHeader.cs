// Decompiled with JetBrains decompiler
// Type: DuckGame.GhostObjectHeader
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public struct GhostObjectHeader
    {
        public NetworkConnection connection;
        public NetIndex16 id;
        public ushort classID;
        public byte levelIndex;
        public NetIndex8 authority;
        public NetIndex16 tick;
        public bool delta;

        public GhostObjectHeader(bool pClean)
        {
            this.id = new NetIndex16();
            this.classID = 0;
            this.levelIndex = 0;
            this.authority = (NetIndex8)0;
            this.tick = (NetIndex16)2;
            this.delta = false;
            this.connection = null;
        }

        public NetMessagePriority priority => !this.delta ? NetMessagePriority.ReliableOrdered : NetMessagePriority.UnreliableUnordered;

        public static void Serialize(
          BitBuffer pBuffer,
          GhostObject pGhost,
          NetIndex16 pTick,
          bool pDelta,
          bool pMinimal)
        {
            pBuffer.Write((ushort)(int)pGhost.ghostObjectIndex);
            pBuffer.Write((object)pGhost.thing.authority);
            if (pDelta)
            {
                pBuffer.Write(true);
                pBuffer.Write((ushort)(int)pTick);
            }
            else
            {
                pBuffer.Write(false);
                if (pGhost.thing.connection.profile != null)
                    pBuffer.Write(pGhost.thing.connection.profile.networkIndex);
                else
                    pBuffer.Write(byte.MaxValue);
                if (pMinimal)
                    return;
                pBuffer.Write(Editor.IDToType[pGhost.thing.GetType()]);
                pBuffer.Write(DuckNetwork.levelIndex);
            }
        }

        public static GhostObjectHeader Deserialize(BitBuffer pBuffer, bool pMinimal)
        {
            GhostObjectHeader ghostObjectHeader = new GhostObjectHeader(true)
            {
                id = (NetIndex16)pBuffer.ReadUShort(),
                authority = (NetIndex8)pBuffer.ReadByte()
            };
            if (pBuffer.ReadBool())
            {
                ghostObjectHeader.tick = (NetIndex16)pBuffer.ReadUShort();
                ghostObjectHeader.delta = true;
            }
            else
            {
                byte index = pBuffer.ReadByte();
                if (index != byte.MaxValue)
                    ghostObjectHeader.connection = DuckNetwork.profiles[index].connection;
                if (!pMinimal)
                {
                    ghostObjectHeader.classID = pBuffer.ReadUShort();
                    ghostObjectHeader.levelIndex = pBuffer.ReadByte();
                }
            }
            return ghostObjectHeader;
        }
    }
}

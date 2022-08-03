// Decompiled with JetBrains decompiler
// Type: DuckGame.NMGhostState
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMGhostState : NetMessage
    {
        public GhostObjectHeader header;
        public BitBuffer data;
        public long mask;
        public GhostObject ghost;

        public NetIndex16 id => header.id;

        public byte levelIndex => header.levelIndex;

        public ushort classID => header.classID;

        public NetIndex8 authority => header.authority;

        public NetIndex16 tick
        {
            get => header.tick;
            set => header.tick = value;
        }

        public bool minimalState { get; set; }

        public NMGhostState(BitBuffer dat)
        {
            data = dat;
            manager = BelongsToManager.GhostManager;
            header = new GhostObjectHeader(true);
        }

        public NMGhostState()
        {
            manager = BelongsToManager.GhostManager;
            header = new GhostObjectHeader(true);
        }

        protected override void OnSerialize() => _serializedData.WriteBufferData(data);

        public override void OnDeserialize(BitBuffer d)
        {
            header = GhostObjectHeader.Deserialize(d, minimalState);
            data = d.ReadBitBuffer();
        }

        public override string ToString() => base.ToString();
    }
}

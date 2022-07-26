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

        public NetIndex16 id => this.header.id;

        public byte levelIndex => this.header.levelIndex;

        public ushort classID => this.header.classID;

        public NetIndex8 authority => this.header.authority;

        public NetIndex16 tick
        {
            get => this.header.tick;
            set => this.header.tick = value;
        }

        public bool minimalState { get; set; }

        public NMGhostState(BitBuffer dat)
        {
            this.data = dat;
            this.manager = BelongsToManager.GhostManager;
            this.header = new GhostObjectHeader(true);
        }

        public NMGhostState()
        {
            this.manager = BelongsToManager.GhostManager;
            this.header = new GhostObjectHeader(true);
        }

        protected override void OnSerialize() => this._serializedData.WriteBufferData(this.data);

        public override void OnDeserialize(BitBuffer d)
        {
            this.header = GhostObjectHeader.Deserialize(d, this.minimalState);
            this.data = d.ReadBitBuffer();
        }

        public override string ToString() => base.ToString();
    }
}

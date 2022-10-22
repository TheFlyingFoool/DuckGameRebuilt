// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsSnapshotObject
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PhysicsSnapshotObject
    {
        public PhysicsObject thing;
        public ushort networkID;
        public System.Type type;
        public SnapshotContainedData data;
        public Vec2 position;
        public Vec2 velocity;
        public float angle;
        public byte frame;
        public double serverTime;
        public double clientTime;
        public int inputState;
        public object classData;
        public PhysicsSnapshotDuckProperties duckProps;
    }
}

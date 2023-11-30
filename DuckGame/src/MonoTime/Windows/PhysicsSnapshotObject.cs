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

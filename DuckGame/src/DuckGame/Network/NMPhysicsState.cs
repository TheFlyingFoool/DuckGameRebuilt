namespace DuckGame
{
    public class NMPhysicsState : NMObjectMessage
    {
        public Vec2 position;
        public Vec2 velocity;
        public int clientFrame;

        public NMPhysicsState()
        {
        }

        public NMPhysicsState(Vec2 Position, Vec2 Velocity, ushort ObjectID, int ClientFrame)
          : base(ObjectID)
        {
            position = Position;
            velocity = Velocity;
            clientFrame = ClientFrame;
        }
    }
}

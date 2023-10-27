namespace DuckGame
{
    public class NMDuckPhysicsState : NMPhysicsState
    {
        public int inputState;
        public ushort holding;

        public NMDuckPhysicsState()
        {
        }

        public NMDuckPhysicsState(
          Vec2 Position,
          Vec2 Velocity,
          ushort ObjectID,
          int ClientFrame,
          int InputState,
          ushort Holding)
          : base(Position, Velocity, ObjectID, ClientFrame)
        {
            inputState = InputState;
            holding = Holding;
        }
    }
}

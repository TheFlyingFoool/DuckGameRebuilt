namespace DuckGame
{
    /// <summary>The state of a pad.</summary>
    public struct PadState
    {
        public PadButton buttons;
        public TriggerStates triggers;
        public StickStates sticks;

        public bool IsButtonDown(PadButton butt) => (buttons & butt) != 0;

        public bool IsButtonUp(PadButton butt) => (buttons & butt) == 0;

        public struct TriggerStates
        {
            public float left;
            public float right;
        }

        public struct StickStates
        {
            public Vec2 left;
            public Vec2 right;
        }
    }
}

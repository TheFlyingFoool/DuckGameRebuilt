namespace DuckGame
{
    public class RegisteredVote
    {
        public Profile who;
        public VoteType vote = VoteType.None;
        public float slide;
        public bool open = true;
        public bool doClose;
        public float wobble;
        public float wobbleInc;
        public Vec2 leftStick;
        public Vec2 rightStick;
    }
}

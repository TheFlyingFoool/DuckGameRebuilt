namespace DuckGame
{
    public class AIStateWait : AIState
    {
        private float _wait;

        public AIStateWait(float wait) => _wait = wait;

        public override AIState Update(Duck duck, DuckAI ai)
        {
            _wait -= 0.016f;
            return _wait <= 0 ? null : (AIState)this;
        }
    }
}

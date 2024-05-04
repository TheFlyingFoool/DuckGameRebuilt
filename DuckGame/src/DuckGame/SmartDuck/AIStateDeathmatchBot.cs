namespace DuckGame
{
    public class AIStateDeathmatchBot : AIState
    {
        public override AIState Update(Duck duck, DuckAI ai)
        {
            if (Network.inLobby && !duck.pickedHat)
            {
                duck.pickedHat = true;
                _state.Push(new AIStatePickHat());
                return this;
            }
            if (duck.holdObject == null || !(duck.holdObject is Gun))
            {
                _state.Push(new AIStateFindGun());
                return this;
            }
            _state.Push(new AIStateFindTarget());
            return this;
        }
    }
}

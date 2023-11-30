namespace DuckGame
{
    public class NMConditionalEvent : ConditionalMessage
    {
        public NMConditionalEvent() => manager = BelongsToManager.EventManager;

        public virtual void Activate()
        {
        }
    }
}

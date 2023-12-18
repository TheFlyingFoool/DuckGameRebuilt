namespace DuckGame
{
    public class NMSkipLevel : NMEvent
    {
        public override void Activate()
        {
            HUD.AddPlayerChangeDisplay("@SKIPICON@|DGRED|Level was Skipped!", 2f);
            base.Activate();
        }
    }
}

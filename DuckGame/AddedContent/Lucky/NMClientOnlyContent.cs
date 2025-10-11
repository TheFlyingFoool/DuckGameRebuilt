namespace DuckGame
{
    [ClientOnly]
    public class NMClientOnlyContent : NMEvent
    {
        public NMClientOnlyContent()
        {
        }

        public override void Activate()
        {
            if (connection.isHost)
            {
                HUD.AddPlayerChangeDisplay("|PINK|DGR ITEMS ENABLED", 2f);
                Editor.EnableClientOnlyContent();
            }
        }
    }
}

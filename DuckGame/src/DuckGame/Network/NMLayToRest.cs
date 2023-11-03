namespace DuckGame
{
    public class NMLayToRest : NMEvent
    {
        public Duck who;

        public NMLayToRest()
        {
        }

        public NMLayToRest(Duck pWho) => who = pWho;

        public override void Activate()
        {
            if (who != null)
            {
                who.isConversionMessage = true;
                who.LayToRest(null);
                who.isConversionMessage = false;
            }
            base.Activate();
        }
    }
}

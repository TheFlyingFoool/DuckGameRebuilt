namespace DuckGame
{
    public class NMPerformFuneral : NMEvent
    {
        public Duck duck;

        public NMPerformFuneral()
        {
        }

        public NMPerformFuneral(Duck d) => duck = d;

        public override void Activate()
        {
            if (duck == null)
                return;
            duck.DoFuneralStuff();
        }
    }
}

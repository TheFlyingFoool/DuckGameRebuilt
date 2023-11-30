namespace DuckGame
{
    public class NMRessurect : NMEvent
    {
        public Vec2 position;
        public Duck duck;
        public byte lifeIndex;

        public NMRessurect()
        {
        }

        public NMRessurect(Vec2 pPosition, Duck pDuck, byte pLifeChangeIndex)
        {
            position = pPosition;
            duck = pDuck;
            lifeIndex = pLifeChangeIndex;
        }

        public override void Activate()
        {
            if (duck != null && duck.profile != null && duck.WillAcceptLifeChange(lifeIndex))
            {
                Duck.ResurrectEffect(position);
                duck.ResetNonServerDeathState();
            }
            base.Activate();
        }
    }
}

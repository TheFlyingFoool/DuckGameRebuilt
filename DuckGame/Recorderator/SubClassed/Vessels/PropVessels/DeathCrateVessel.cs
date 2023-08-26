namespace DuckGame
{
    public class DeathCrateVessel : HoldableVessel
    {
        public DeathCrateVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(DeathCrate));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            DeathCrateVessel v = new DeathCrateVessel(new DeathCrate(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            DeathCrate d = (DeathCrate)t;
            int b = 0;
            int divide = 32;
            for (int i = 0; i < 6; i++)
            {
                if (bArray[i]) b += divide;
                divide /= 2;
            }
            ((SpriteMap)d.graphic).ClearAnimations();
            ((SpriteMap)d.graphic).imageIndex = b;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            //same shenaningans as the windowvessel one happened here
            DeathCrate d = (DeathCrate)t;
            int z = ((SpriteMap)d.graphic).imageIndex;
            bArray[0] = (z & 32) > 0;
            bArray[1] = (z & 16) > 0;
            bArray[2] = (z & 8) > 0;
            bArray[3] = (z & 4) > 0;
            bArray[4] = (z & 2) > 0;
            bArray[5] = (z & 1) > 0;
            bArray[6] = d.visible;
            base.RecordUpdate();
        }
    }
}

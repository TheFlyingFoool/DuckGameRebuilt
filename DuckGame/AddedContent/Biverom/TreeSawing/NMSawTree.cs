namespace DuckGame
{
    [ClientOnly]
    public class NMSawTree : NMEvent
    {
        public Vec2 position;
        public bool flipped;
        public bool exploded;

        public NMSawTree(Vec2 pos, bool flip, bool explode)
        {
            position = pos;
            flipped = flip;
            exploded = explode;
        }

        public NMSawTree()
        {
        }

        public override void Activate()
        {
            AutoPlatform platformStump = Level.CheckPoint<AutoPlatform>(position);
            if (platformStump == null)
                return;
            TreeStump stump = new TreeStump(platformStump.x, platformStump.y, platformStump);
            Level.Add(stump);
            stump.Saw(flipped, exploded);
        }
    }
}

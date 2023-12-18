namespace DuckGame
{
    public class NMMeltTile : NMEvent
    {
        public short x;
        public short y;

        public NMMeltTile()
        {
        }

        public NMMeltTile(Vec2 pPosition)
        {
            x = (short)pPosition.x;
            y = (short)pPosition.y;
        }

        public override void Activate()
        {
            Level.CheckPoint<SnowTileset>(new Vec2(x, y))?.Melt(false, true);
            base.Activate();
        }
    }
}

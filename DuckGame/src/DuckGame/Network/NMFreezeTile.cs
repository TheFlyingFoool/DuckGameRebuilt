namespace DuckGame
{
    public class NMFreezeTile : NMEvent
    {
        public short x;
        public short y;

        public NMFreezeTile()
        {
        }

        public NMFreezeTile(Vec2 pPosition)
        {
            x = (short)pPosition.x;
            y = (short)pPosition.y;
        }

        public override void Activate()
        {
            Level.CheckPoint<SnowTileset>(new Vec2(x, y))?.Freeze(false, true);
            base.Activate();
        }
    }
}

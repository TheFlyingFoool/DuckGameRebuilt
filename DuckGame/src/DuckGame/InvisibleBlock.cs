namespace DuckGame
{
    public class InvisibleBlock : Block, IPathNodeBlocker, IDontMove
    {
        public InvisibleBlock(float x, float y)
          : base(x, y)
        {
        }

        public InvisibleBlock(float x, float y, float wid, float hi, PhysicsMaterial mat = PhysicsMaterial.Default)
          : base(x, y, wid, hi, mat)
        {
        }
    }
}

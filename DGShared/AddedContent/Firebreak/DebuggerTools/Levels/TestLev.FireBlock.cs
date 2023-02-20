namespace DuckGame
{
    public partial class TestLev
    {
        [ClientOnly]
        public class FireBlock : Block
        {
            public FireBlock(float x, float y, float wid, float hi) : base(x, y, wid, hi, PhysicsMaterial.Default) { }

            public override void Draw()
            {
                Graphics.DrawRect(topLeft, bottomRight, Color.Red, depth, false);

                base.Draw();
            }
        }
    }
}
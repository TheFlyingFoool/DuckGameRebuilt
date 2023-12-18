namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class GreyBlock : Block
    {
        public GreyBlock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("greyBlock");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Grey Block";
            editorTooltip = "It's a featureless grey block.";
            thickness = 4f;
            physicsMaterial = PhysicsMaterial.Metal;
            shouldbeinupdateloop = false;
        }
    }
}

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Debug)]
    public class LightingTwoPointOH : Thing
    {
        public LightingTwoPointOH()
          : base()
        {
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            _visibleInGame = false;
            _editorName = "Lighting 2.0";
        }

        public override void Initialize()
        {
            Layer.lightingTwoPointOh = true;
            Layer.Lighting.depth = -20;
            base.Initialize();
        }
    }
}

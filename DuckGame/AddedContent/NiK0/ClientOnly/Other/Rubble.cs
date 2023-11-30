using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Details|Terrain")]
    [BaggedProperty("canSpawn", false)]
    public class Rubble : PhysicsObject
    {
        public SpriteMap sprite;
        public EditorProperty<int> style;
        public Rubble(float xpos, float ypos) : base(xpos, ypos) 
        {
            style = new EditorProperty<int>(-1, this, -1, 5, 1, "RANDOM");
            sprite = new SpriteMap("rubble", 8, 8);
            center = new Vec2(4);
            collisionSize = new Vec2(6);
            _collisionOffset = new Vec2(-3);
            graphic = sprite;
            hugWalls = WallHug.Floor;
            editorTooltip = "Just some rocks that fell from the infinitely tall rock wall.";
            thickness = -1000;
            weight = -1000;
        }
        public override Sprite GeneratePreview(
          int wide = 16,
          int high = 16,
          bool transparentBack = false,
          Effect effect = null,
          RenderTarget2D target = null)
        {
            frame = 5;
            return base.GeneratePreview(wide, high, transparentBack, effect, target);
        }
        public override void EditorPropertyChanged(object property)
        {
            if (style == -1) frame = Rando.Int(5);
            else frame = style;
            base.EditorPropertyChanged(property);
        }
        public override void Initialize()
        {
            if (style == -1) frame = Rando.Int(5);
            else frame = style;
            base.Initialize();
        }
    }
}

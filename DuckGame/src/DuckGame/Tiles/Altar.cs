namespace DuckGame
{
    [EditorGroup("Stuff|Pyramid", EditorItemType.Pyramid)]
    public class Altar : Platform
    {
        public EditorProperty<int> wide;
        public bool kill;
        public Platform leftPlat;
        public Platform rightPlat;

        public override void EditorPropertyChanged(object property)
        {
            (graphic as SpriteMap).frame = wide.value - 1;
            UpdateSize();
        }

        public void UpdateSize()
        {
            if ((graphic as SpriteMap).frame == 0)
            {
                center = new Vec2(8f, 8f);
                collisionSize = new Vec2(12f, 13f);
                collisionOffset = new Vec2(-6f, -5f);
            }
            else if ((graphic as SpriteMap).frame == 1)
            {
                center = new Vec2(16f, 8f);
                collisionSize = new Vec2(28f, 13f);
                collisionOffset = new Vec2(-14f, -5f);
            }
            else
            {
                if ((graphic as SpriteMap).frame != 2)
                    return;
                center = new Vec2(24f, 8f);
                collisionSize = new Vec2(44f, 13f);
                collisionOffset = new Vec2(-22f, -5f);
            }
        }

        public Altar(float xpos, float ypos, int dir)
          : base(xpos, ypos)
        {
            wide = new EditorProperty<int>(1, this, 1f, 3f, 1f);
            graphic = new SpriteMap("altar", 48, 16);
            hugWalls = WallHug.Floor;
            UpdateSize();
            thickness = 0f;
            depth = -0.05f;
            placementLayerOverride = Layer.Blocks;
        }

        public override void Draw()
        {
            flipHorizontal = false;
            base.Draw();
        }

        public override void Terminate()
        {
            if (leftPlat != null)
                Level.Remove(leftPlat);
            if (rightPlat != null)
                Level.Remove(rightPlat);
            base.Terminate();
        }
    }
}

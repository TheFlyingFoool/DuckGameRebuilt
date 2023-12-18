namespace DuckGame
{
    [EditorGroup("Arcade|Cameras", EditorItemType.ArcadeNew)]
    [BaggedProperty("previewPriority", true)]
    internal class CameraMover : Thing
    {
        public EditorProperty<float> SpeedX = new EditorProperty<float>(0f, min: -10f, max: 10f, increment: 0.25f);
        public EditorProperty<float> SpeedY = new EditorProperty<float>(0f, min: -10f, max: 10f, increment: 0.25f);
        public EditorProperty<float> MoveDelay = new EditorProperty<float>(0f, max: 120f, increment: 0.25f);

        public CameraMover(float xPos, float yPos)
          : base(xPos, yPos)
        {
            graphic = new Sprite("cameraMover");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -4f);
            editorCycleType = typeof(CameraZoomNew);
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
                alpha = 0f;
            base.Initialize();
        }
    }
}

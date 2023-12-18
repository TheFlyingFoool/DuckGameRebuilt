namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    public class CustomCamera : Thing
    {
        public EditorProperty<int> wide;

        public CustomCamera()
          : base()
        {
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            wide = new EditorProperty<int>(320, this, 60f, 1920f, 1f);
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
                alpha = 0f;
            base.Initialize();
        }

        public override void Draw()
        {
            base.Draw();
            if (Editor.editorDraw || !(Level.current is Editor))
                return;
            float num1 = wide.value;
            float num2 = num1 * (9f / 16f);
            Graphics.DrawRect(position + new Vec2((float)(-num1 / 2f), (float)(-num2 / 2f)), position + new Vec2(num1 / 2f, num2 / 2f), Color.Blue * 0.5f, (Depth)1f, false);
        }
    }
}

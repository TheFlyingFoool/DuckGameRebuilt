namespace DuckGame
{
    public abstract class TutorialSign : Thing
    {
        public TutorialSign(float xpos, float ypos, string image, string name)
          : base(xpos, ypos)
        {
            if (image == null)
                return;
            graphic = new Sprite(image);
            center = new Vec2(graphic.w / 2, graphic.h / 2);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = -0.5f;
            _editorName = name;
            layer = Layer.Background;
        }
    }
}

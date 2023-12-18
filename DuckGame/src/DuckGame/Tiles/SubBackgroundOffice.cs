namespace DuckGame
{
    public class SubBackgroundOffice : SubBackgroundTile
    {
        public SubBackgroundOffice(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("officeSubBackground", 32, 32, true);
            _opacityFromGraphic = true;
            center = new Vec2(24f, 16f);
            collisionSize = new Vec2(32f, 32f);
            collisionOffset = new Vec2(-16f, -16f);
        }
    }
}

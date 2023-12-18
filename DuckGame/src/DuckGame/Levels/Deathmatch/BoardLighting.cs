namespace DuckGame
{
    public class BoardLighting : Thing
    {
        private Sprite _lightRay;

        public BoardLighting(float xpos, float ypos)
          : base(xpos, ypos)
        {
            shouldbegraphicculled = false;
            _lightRay = new Sprite("rockThrow/lightRays");
            center = new Vec2(305f, 0f);
            graphic = _lightRay;
        }

        public override void Draw()
        {
            if (RockWeather.lightOpacity < 0.01f || Layer.blurry)
                return;
            base.Draw();
        }
    }
}

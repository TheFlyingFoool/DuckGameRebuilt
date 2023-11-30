namespace DuckGame
{
    public abstract class WeatherParticle
    {
        public Vec2 position;
        public float z;
        public Vec2 velocity;
        public float zSpeed;
        public float alpha = 1f;
        public bool die;

        public WeatherParticle(Vec2 pos) => position = pos;

        public abstract void Draw();

        public abstract void Update();
    }
}

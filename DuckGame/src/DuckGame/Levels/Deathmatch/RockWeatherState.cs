namespace DuckGame
{
    public class RockWeatherState
    {
        public Vec3 add;
        public Vec3 multiply;
        public Vec3 sky;
        public Vec2 sunPos;
        public float lightOpacity;
        public float sunGlow;
        public float sunOpacity = 1f;
        public float rainbowLight;
        public float rainbowLight2;

        public RockWeatherState Copy() => new RockWeatherState()
        {
            add = add,
            multiply = multiply,
            sky = sky,
            sunPos = sunPos,
            lightOpacity = lightOpacity,
            sunGlow = sunGlow,
            sunOpacity = sunOpacity,
            rainbowLight = rainbowLight,
            rainbowLight2 = rainbowLight2
        };
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.RockWeatherState
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            add = this.add,
            multiply = this.multiply,
            sky = this.sky,
            sunPos = this.sunPos,
            lightOpacity = this.lightOpacity,
            sunGlow = this.sunGlow,
            sunOpacity = this.sunOpacity,
            rainbowLight = this.rainbowLight,
            rainbowLight2 = this.rainbowLight2
        };
    }
}

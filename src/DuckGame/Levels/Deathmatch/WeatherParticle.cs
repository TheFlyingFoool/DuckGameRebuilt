// Decompiled with JetBrains decompiler
// Type: DuckGame.WeatherParticle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

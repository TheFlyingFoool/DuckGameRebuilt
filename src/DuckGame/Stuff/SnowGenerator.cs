// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowGenerator
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class SnowGenerator : Thing
    {
        private static bool initGen;
        private float snowWait = 1f;

        public SnowGenerator(float x, float y)
          : base(x, y)
        {
            _editorName = "Snow Machine";
            graphic = new Sprite("snowGenerator");
            center = new Vec2(8f, 8f);
            depth = (Depth)0.55f;
            _visibleInGame = false;
            snowWait = Rando.Float(4f);
            editorTooltip = "Let it snow!";
            solid = false;
            _collisionSize = new Vec2(0f, 0f);
            maxPlaceable = 32;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            snowWait -= Maths.IncFrameTimer();
            if (snowWait <= 0.0)
            {
                snowWait = Rando.Float(2f, 4f);
                Level.Add(new SnowFallParticle(x + Rando.Float(-8f, 8f), y + Rando.Float(-8f, 8f), new Vec2(0f, 0f)));
            }
            base.Update();
        }
    }
}

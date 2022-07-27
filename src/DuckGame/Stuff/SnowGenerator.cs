// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowGenerator
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Linq;

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
            this._editorName = "Snow Machine";
            this.graphic = new Sprite("snowGenerator");
            this.center = new Vec2(8f, 8f);
            this.depth = (Depth)0.55f;
            this._visibleInGame = false;
            this.snowWait = Rando.Float(4f);
            this.editorTooltip = "Let it snow!";
            this.solid = false;
            this._collisionSize = new Vec2(0.0f, 0.0f);
            this.maxPlaceable = 32;
        }

        public override void Initialize()
        {
            SnowGenerator.initGen = true;
            base.Initialize();
        }

        public override void Update()
        {
            if (SnowGenerator.initGen)
            {
                int num = 0;
                foreach (SnowGenerator t in Level.current.things[typeof(SnowGenerator)].ToList<Thing>())
                {
                    if (num < this.maxPlaceable)
                        ++num;
                    else
                        Level.current.RemoveThing(t);
                }
                SnowGenerator.initGen = false;
            }
            this.snowWait -= Maths.IncFrameTimer();
            if (snowWait <= 0.0)
            {
                this.snowWait = Rando.Float(2f, 4f);
                Level.Add(new SnowFallParticle(this.x + Rando.Float(-8f, 8f), this.y + Rando.Float(-8f, 8f), new Vec2(0.0f, 0.0f)));
            }
            base.Update();
        }
    }
}

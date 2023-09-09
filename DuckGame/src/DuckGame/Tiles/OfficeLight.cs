// Decompiled with JetBrains decompiler
// Type: DuckGame.OfficeLight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Lighting)]
    [BaggedProperty("isInDemo", true)]
    public class OfficeLight : Thing
    {
        private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public OfficeLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("officeLight");
            center = new Vec2(16f, 3f);
            _collisionSize = new Vec2(30f, 6f);
            _collisionOffset = new Vec2(-15f, -3f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Ceiling;
            layer = Layer.Game;
            editorCycleType = typeof(StreetLight);
            shouldbeinupdateloop = DGRSettings.AmbientParticles;
        }
        public override void Update()
        {
            if (tim > 0)
            {
                pl._range = Rando.Float(100f);
                if (tim <= 1)
                {
                    pl._range = 100;
                }
                pl.forceRefresh = true;
                tim--;
            }
            else
            {
                if (Rando.Int(240) == 0)
                {
                    tim = Rando.Int(10);
                }
            }
        }
        public int tim;
        public PointLight pl;
        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _occluders.Add(new LightOccluder(position + new Vec2(-15f, -3f), position + new Vec2(-15f, 4f), new Color(1f, 1f, 1f)));
            _occluders.Add(new LightOccluder(position + new Vec2(15f, -3f), position + new Vec2(15f, 4f), new Color(1f, 1f, 1f)));
            _occluders.Add(new LightOccluder(position + new Vec2(-15f, -2f), position + new Vec2(15f, -2f), new Color(1f, 1f, 1f)));
            pl = new PointLight(x, y - 1f, new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue), 100f, _occluders);
            Level.Add(pl);
            _shade = new SpriteThing(x, y, new Sprite("officeLight"))
            {
                center = center,
                layer = Layer.Foreground
            };
            Level.Add(_shade);
        }
    }
}

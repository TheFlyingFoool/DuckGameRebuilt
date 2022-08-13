// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidLightRoof
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Pyramid", EditorItemType.Pyramid)]
    [BaggedProperty("isInDemo", true)]
    public class PyramidLightRoof : Thing
    {
        private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();
        private Block myBlock;
        private PointLight light;
        private bool did;

        public PyramidLightRoof(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("pyramidRoofLight");
            center = new Vec2(7f, 5f);
            _collisionSize = new Vec2(14f, 6f);
            _collisionOffset = new Vec2(-7f, -3f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Ceiling;
            layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _occluders.Add(new LightOccluder(position + new Vec2(-15f, -3f), position + new Vec2(-15f, 4f), new Color(1f, 0.9f, 0.8f)));
            _occluders.Add(new LightOccluder(position + new Vec2(15f, -3f), position + new Vec2(15f, 4f), new Color(1f, 0.9f, 0.8f)));
            _occluders.Add(new LightOccluder(position + new Vec2(-15f, -2f), position + new Vec2(15f, -2f), new Color(1f, 0.9f, 0.8f)));
            light = new PointLight(x, y - 1f, PyramidWallLight.lightColor, 110f, _occluders, true);
            Level.Add(light);
            _shade = new SpriteThing(x, y, new Sprite("pyramidRoofLightShade"))
            {
                center = center,
                layer = Layer.Foreground
            };
            Level.Add(_shade);
        }

        public override void Update()
        {
            if (!did)
            {
                myBlock = Level.CheckPoint<Block>(new Vec2(x, y - 8f));
                did = true;
            }
            if (myBlock != null && myBlock.removeFromLevel)
            {
                Level.Remove(this);
                Level.Remove(light);
                Level.Remove(_shade);
            }
            base.Update();
        }
    }
}

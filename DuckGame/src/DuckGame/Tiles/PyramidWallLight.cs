// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidWallLight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Pyramid", EditorItemType.Pyramid)]
    [BaggedProperty("isInDemo", true)]
    public class PyramidWallLight : Thing
    {
        public static Color lightColor = new Color(byte.MaxValue, (byte)200, (byte)150);
        //private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();
        private SpriteMap _sprite;
        private Vec2 lightPos;

        public PyramidWallLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("pyramidWallLight", 14, 12);
            _sprite.AddAnimation("go", 0.2f, true, 0, 1, 2, 3, 4);
            _sprite.SetAnimation("go");
            graphic = _sprite;
            center = new Vec2(7f, 8f);
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-4f, -4f);
            depth = -0.9f;
            alpha = 0.7f;
            layer = Layer.Game;
            placementLayerOverride = Layer.Blocks;
            hugWalls = WallHug.Left | WallHug.Right;
            editorCycleType = typeof(PyramidBLight);
            shouldbeinupdateloop = DGRSettings.AmbientParticles;
        }
        public float timer;
        public override void Update()
        {
            timer += 0.02f * DGRSettings.ActualParticleMultiplier;
            if (timer >= 0.6f)
            {
                timer = Rando.Float(0.1f, 0.2f);
                Level.Add(new Ember(x + Rando.Float(-4, 4), y - Rando.Float(3.5f, 6)));
            }
        }
        public override void Draw()
        {
            graphic.flipH = flipHorizontal;
            if (DevConsole.showCollision)
            {
                Graphics.DrawCircle(lightPos, 2f, Color.Blue);
                foreach (LightOccluder occluder in _occluders)
                    Graphics.DrawLine(occluder.p1, occluder.p2, Color.Red, depth: ((Depth)1f));
            }
            base.Draw();
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _occluders.Add(new LightOccluder(position + new Vec2(-15f, 3f), position + new Vec2(-15f, -4f), new Color(0.95f, 0.9f, 0.85f)));
            _occluders.Add(new LightOccluder(position + new Vec2(15f, 3f), position + new Vec2(15f, -4f), new Color(0.95f, 0.9f, 0.85f)));
            _occluders.Add(new LightOccluder(position + new Vec2(-15f, 2f), position + new Vec2(15f, 2f), new Color(0.95f, 0.9f, 0.85f)));
            if (flipHorizontal)
            {
                lightPos = new Vec2(x, y);
                Level.Add(new PointLight(lightPos.x, lightPos.y, lightColor, 120f, _occluders, true));
            }
            else
            {
                lightPos = new Vec2(x, y);
                Level.Add(new PointLight(lightPos.x, lightPos.y, lightColor, 120f, _occluders, true));
            }
        }
    }
}

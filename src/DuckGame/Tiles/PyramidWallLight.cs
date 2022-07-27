// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidWallLight
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._sprite = new SpriteMap("pyramidWallLight", 14, 12);
            this._sprite.AddAnimation("go", 0.2f, true, 0, 1, 2, 3, 4);
            this._sprite.SetAnimation("go");
            this.graphic = _sprite;
            this.center = new Vec2(7f, 8f);
            this._collisionSize = new Vec2(8f, 8f);
            this._collisionOffset = new Vec2(-4f, -4f);
            this.depth = -0.9f;
            this.alpha = 0.7f;
            this.layer = Layer.Game;
            this.placementLayerOverride = Layer.Blocks;
            this.hugWalls = WallHug.Left | WallHug.Right;
        }

        public override void Draw()
        {
            this.graphic.flipH = this.flipHorizontal;
            if (DevConsole.showCollision)
            {
                Graphics.DrawCircle(this.lightPos, 2f, Color.Blue);
                foreach (LightOccluder occluder in this._occluders)
                    Graphics.DrawLine(occluder.p1, occluder.p2, Color.Red, depth: ((Depth)1f));
            }
            base.Draw();
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this._occluders.Add(new LightOccluder(this.position + new Vec2(-15f, 3f), this.position + new Vec2(-15f, -4f), new Color(0.95f, 0.9f, 0.85f)));
            this._occluders.Add(new LightOccluder(this.position + new Vec2(15f, 3f), this.position + new Vec2(15f, -4f), new Color(0.95f, 0.9f, 0.85f)));
            this._occluders.Add(new LightOccluder(this.position + new Vec2(-15f, 2f), this.position + new Vec2(15f, 2f), new Color(0.95f, 0.9f, 0.85f)));
            if (this.flipHorizontal)
            {
                this.lightPos = new Vec2(this.x, this.y);
                Level.Add(new PointLight(this.lightPos.x, this.lightPos.y, PyramidWallLight.lightColor, 120f, this._occluders, true));
            }
            else
            {
                this.lightPos = new Vec2(this.x, this.y);
                Level.Add(new PointLight(this.lightPos.x, this.lightPos.y, PyramidWallLight.lightColor, 120f, this._occluders, true));
            }
        }
    }
}

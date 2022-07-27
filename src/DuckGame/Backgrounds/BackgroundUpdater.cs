// Decompiled with JetBrains decompiler
// Type: DuckGame.BackgroundUpdater
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class BackgroundUpdater : Thing
    {
        protected ParallaxBackground _parallax;
        protected float _lastCameraX;
        protected bool _update = true;
        protected bool _yParallax = true;
        protected float _yOffset;
        public Rectangle scissor = new Rectangle(0.0f, 0.0f, 0.0f, 0.0f);
        public bool overrideBaseScissorCall;
        public float _extraYOffset;
        public Color backgroundColor;
        protected bool _skipMovement;

        public ParallaxBackground parallax => this._parallax;

        public bool update
        {
            get => this._update;
            set => this._update = value;
        }

        public void SetVisible(bool vis)
        {
            this._parallax.scissor = this.scissor;
            this._parallax.visible = vis;
            if (scissor.width == 0.0)
                return;
            this._parallax.layer.scissor = this.scissor;
        }

        public BackgroundUpdater(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._isStatic = true;
            this._opaque = true;
            this.editorTooltip = "Adds a parallaxing background visual to the level (limit 1 per level)";
        }

        public static Vec2 GetWallScissor()
        {
            Matrix matrix = Level.current.camera.getMatrix();
            int x = 0;
            int y = 0;
            float num = Graphics.width / Resolution.size.x;
            foreach (RockWall rockWall in Level.current.things[typeof(RockWall)])
            {
                if (y == 0)
                    y = (int)Resolution.size.x;
                Vec2 vec2 = Vec2.Transform(rockWall.position, matrix) * num;
                if (!rockWall.flipHorizontal && vec2.x > (double)x)
                    x = (int)vec2.x;
                else if (rockWall.flipHorizontal && vec2.x < (double)y)
                    y = (int)vec2.x;
            }
            if (y != 0)
                y -= x;
            if (y == 0)
                y = (int)Resolution.size.x;
            return new Vec2(x, y);
        }

        public override void Update()
        {
            if (!this.overrideBaseScissorCall)
            {
                Vec2 wallScissor = BackgroundUpdater.GetWallScissor();
                if (wallScissor != Vec2.Zero)
                    this.scissor = new Rectangle((int)wallScissor.x, 0.0f, (int)wallScissor.y, Resolution.current.y);
            }
            if (!this._update)
                return;
            if (!this._skipMovement)
            {
                float num = Level.current.camera.width * 4f / Graphics.width;
                if (this._yParallax)
                {
                    this._parallax.y = (float)(-((double)Level.current.camera.centerY / 12.0) - 5.0) + this._yOffset;
                }
                else
                {
                    Layer.Parallax.camera = Level.current.camera;
                    this._parallax.y = this._extraYOffset - 108f;
                }
                this._parallax.xmove = (this._lastCameraX - Level.current.camera.centerX) / num;
            }
            this._lastCameraX = Level.current.camera.centerX;
            if (scissor.width != 0.0)
                this._parallax.scissor = this.scissor;
            base.Update();
        }

        public override ContextMenu GetContextMenu() => null;
    }
}

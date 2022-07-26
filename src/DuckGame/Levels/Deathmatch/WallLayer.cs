// Decompiled with JetBrains decompiler
// Type: DuckGame.WallLayer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame
{
    public class WallLayer : Layer
    {
        private Effect _fx;
        private float _scroll;
        public float fieldHeight;
        public Matrix _view;
        public Matrix _proj;
        private float _ypos;
        private List<Sprite> _sprites = new List<Sprite>();
        private List<Sprite> _wallSprites = new List<Sprite>();

        public float scroll
        {
            get => this._scroll;
            set => this._scroll = value;
        }

        public new Matrix view => this._view;

        public new Matrix projection => this._proj;

        public float rise { get; set; }

        public float ypos
        {
            get => this._ypos;
            set => this._ypos = value;
        }

        public void AddSprite(Sprite s) => this._sprites.Add(s);

        public void AddWallSprite(Sprite s) => this._wallSprites.Add(s);

        public WallLayer(string nameval, int depthval = 0)
          : base(nameval, depthval)
        {
            this._fx = (Effect)Content.Load<MTEffect>("Shaders/fieldFadeAdd");
            this._view = Matrix.CreateLookAt(new Vec3(0.0f, 0.0f, -5f), new Vec3(0.0f, 0.0f, 0.0f), Vec3.Up);
            this._proj = Matrix.CreatePerspectiveFieldOfView(0.7853982f, 1.777778f, 0.01f, 100000f);
        }

        public override void Update()
        {
            float num1 = 53f + this.fieldHeight + this.rise;
            float num2 = -0.1f;
            float scroll = this.scroll;
            this._view = Matrix.CreateLookAt(new Vec3(scroll, 300f, -num1 + num2), new Vec3(scroll, 100f, -num1), Vec3.Down);
        }

        public override void Begin(bool transparent, bool isTargetDraw = false)
        {
            Vec3 vec3_1 = new Vec3((float)((double)DuckGame.Graphics.fade * (double)this._fade * (1.0 - (double)this._darken))) * this.colorMul;
            Vec3 vec3_2 = this._colorAdd + new Vec3(this._fadeAdd) + new Vec3(DuckGame.Graphics.flashAddRenderValue) + new Vec3(DuckGame.Graphics.fadeAddRenderValue) - new Vec3(this.darken);
            vec3_2 = new Vec3(Maths.Clamp(vec3_2.x, -1f, 1f), Maths.Clamp(vec3_2.y, -1f, 1f), Maths.Clamp(vec3_2.z, -1f, 1f));
            if ((double)this._darken > 0.0)
                this._darken -= 0.15f;
            else
                this._darken = 0.0f;
            if (this._fx != null)
            {
                this._fx.Parameters["fade"]?.SetValue((Vector3)vec3_1);
                this._fx.Parameters["add"]?.SetValue((Vector3)vec3_2);
            }
            DuckGame.Graphics.screen = this._batch;
            if (this._state.ScissorTestEnable)
                DuckGame.Graphics.SetScissorRectangle(this._scissor);
            this._batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, this._state, (MTEffect)this._fx, this.camera.getMatrix());
        }

        public override void Draw(bool transparent, bool isTargetDraw = false)
        {
            DuckGame.Graphics.currentLayer = (Layer)this;
            this._fx.Parameters["WVP"].SetValue((Microsoft.Xna.Framework.Matrix)(Matrix.CreateRotationY(-1.570796f) * Matrix.CreateTranslation(new Vec3(625f, 20f, 0.1f)) * this._view * this._proj));
            this.Begin(transparent, false);
            foreach (Sprite wallSprite in this._wallSprites)
            {
                float x = wallSprite.x;
                DuckGame.Graphics.Draw(wallSprite, wallSprite.x, wallSprite.y);
                wallSprite.x = x;
            }
            this._batch.End();
            this._fx.Parameters["WVP"].SetValue((Microsoft.Xna.Framework.Matrix)(Matrix.CreateRotationY(-1.570796f) * Matrix.CreateRotationZ(-1.570796f) * Matrix.CreateTranslation(new Vec3(625.5f, 160f, 0.1f)) * this._view * this._proj));
            this.Begin(transparent, false);
            foreach (Sprite wallSprite in this._wallSprites)
            {
                float x = wallSprite.x;
                DuckGame.Graphics.Draw(wallSprite, wallSprite.x, wallSprite.y);
                wallSprite.x = x;
            }
            this._batch.End();
            DuckGame.Graphics.screen = (MTSpriteBatch)null;
            DuckGame.Graphics.currentLayer = (Layer)null;
        }
    }
}

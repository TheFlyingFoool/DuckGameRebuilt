// Decompiled with JetBrains decompiler
// Type: DuckGame.GridBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class GridBackground : Layer
    {
        private VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[4];
        private BasicEffect _effect;
        private float _scroll;
        private Sprite _planet;

        public GridBackground(string nameval, int depthval = 0)
          : base(nameval, depthval)
        {
            float x = 400f;
            float y = 230f;
            this._effect = new BasicEffect(DuckGame.Graphics.device)
            {
                View = (Microsoft.Xna.Framework.Matrix)Matrix.CreateLookAt(new Vec3(0.0f, 0.0f, 500f), new Vec3(0.0f, 0.0f, 0.0f), Vec3.Up)
            };
            BasicEffect effect = this._effect;
            Viewport viewport = DuckGame.Graphics.viewport;
            double width = viewport.Width;
            viewport = DuckGame.Graphics.viewport;
            double height = viewport.Height;
            Microsoft.Xna.Framework.Matrix perspectiveFieldOfView = (Microsoft.Xna.Framework.Matrix)Matrix.CreatePerspectiveFieldOfView(0.7853982f, (float)(width / height), 0.01f, 100000f);
            effect.Projection = perspectiveFieldOfView;
            this._effect.Texture = (Texture2D)new Sprite("grid").texture;
            this._effect.TextureEnabled = true;
            this._effect.VertexColorEnabled = true;
            this._vertices[0].Position = (Vector3)new Vec3(0.0f, y, 0.0f);
            this._vertices[0].TextureCoordinate = (Vector2)new Vec2(0.0f, 0.0f);
            this._vertices[0].Color = (Microsoft.Xna.Framework.Color)new Color(1f, 1f, 1f, 1f);
            this._vertices[1].Position = (Vector3)new Vec3(0.0f, 0.0f, 0.0f);
            this._vertices[1].TextureCoordinate = (Vector2)new Vec2(0.0f, 11f);
            this._vertices[1].Color = (Microsoft.Xna.Framework.Color)new Color(0.5f, 0.5f, 0.5f, 1f);
            this._vertices[2].Position = (Vector3)new Vec3(x, y, 0.0f);
            this._vertices[2].TextureCoordinate = (Vector2)new Vec2(8.5f, 0.0f);
            this._vertices[2].Color = (Microsoft.Xna.Framework.Color)new Color(1f, 1f, 1f, 1f);
            this._vertices[3].Position = (Vector3)new Vec3(x, 0.0f, 0.0f);
            this._vertices[3].TextureCoordinate = (Vector2)new Vec2(8.5f, 11f);
            this._vertices[3].Color = (Microsoft.Xna.Framework.Color)new Color(0.5f, 0.5f, 0.5f, 1f);
            this._planet = new Sprite("background/planet");
        }

        public override void Update()
        {
            this._scroll += 0.4f;
            if (_scroll <= 32.0)
                return;
            this._scroll -= 32f;
        }

        public override void Begin(bool transparent, bool isTargetDraw = false)
        {
            this._effect.DiffuseColor = (Vector3)new Vec3(DuckGame.Graphics.fade, DuckGame.Graphics.fade, DuckGame.Graphics.fade);
            DuckGame.Graphics.screen = this._batch;
            this._batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, (MTEffect)_effect, this.camera.getMatrix());
        }

        public override void Draw(bool transparent, bool isTargetDraw = false)
        {
            DuckGame.Graphics.currentLayer = this;
            this._effect.World = (Microsoft.Xna.Framework.Matrix)Matrix.CreateTranslation(new Vec3(190f, -75f, 300f));
            this.Begin(transparent, false);
            this._planet.flipH = true;
            DuckGame.Graphics.Draw(this._planet, 0.0f, 0.0f);
            this._batch.End();
            this._effect.World = (Microsoft.Xna.Framework.Matrix)(Matrix.CreateRotationX(Maths.DegToRad(90f)) * Matrix.CreateTranslation(new Vec3(-400f, 52f, 0.0f)));
            this.Begin(transparent, false);
            float num1 = 32f;
            int num2 = 26;
            int num3 = 16;
            for (int index = 0; index < num3; ++index)
                DuckGame.Graphics.DrawLine(new Vec2(0.0f + this._scroll, index * num1), new Vec2(num1 * (num2 - 1), index * num1), Color.DarkGray, 3f);
            for (int index = 0; index < num2; ++index)
                DuckGame.Graphics.DrawLine(new Vec2(index * num1 + this._scroll, 0.0f), new Vec2(index * num1 + this._scroll, num1 * (num3 - 1)), Color.DarkGray, 3f);
            this._batch.End();
            this._effect.World = (Microsoft.Xna.Framework.Matrix)(Matrix.CreateRotationX(Maths.DegToRad(90f)) * Matrix.CreateTranslation(new Vec3(-400f, 62f, 0.0f)));
            this.Begin(transparent, false);
            for (int index = 0; index < num3; ++index)
                DuckGame.Graphics.DrawLine(new Vec2(0.0f + this._scroll, index * num1), new Vec2(num1 * (num2 - 1), index * num1), Color.DarkGray * 0.2f, 3f);
            for (int index = 0; index < num2; ++index)
                DuckGame.Graphics.DrawLine(new Vec2(index * num1 + this._scroll, 0.0f), new Vec2(index * num1 + this._scroll, num1 * (num3 - 1)), Color.DarkGray * 0.2f, 3f);
            this._batch.End();
            this._effect.World = (Microsoft.Xna.Framework.Matrix)(Matrix.CreateRotationX(Maths.DegToRad(90f)) * Matrix.CreateTranslation(new Vec3(-400f, -52f, 0.0f)));
            this.Begin(transparent, false);
            for (int index = 0; index < num3; ++index)
                DuckGame.Graphics.DrawLine(new Vec2(0.0f + this._scroll, index * num1), new Vec2(num1 * (num2 - 1), index * num1), Color.DarkGray, 3f);
            for (int index = 0; index < num2; ++index)
                DuckGame.Graphics.DrawLine(new Vec2(index * num1 + this._scroll, 0.0f), new Vec2(index * num1 + this._scroll, num1 * (num3 - 1)), Color.DarkGray, 3f);
            this._batch.End();
            DuckGame.Graphics.screen = null;
            DuckGame.Graphics.currentLayer = null;
        }
    }
}

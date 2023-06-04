// Decompiled with JetBrains decompiler
// Type: DuckGame.GridBackground
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
            _effect = new BasicEffect(Graphics.device)
            {
                View = (Microsoft.Xna.Framework.Matrix)Matrix.CreateLookAt(new Vec3(0f, 0f, 500f), new Vec3(0f, 0f, 0f), Vec3.Up),
                Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, (float)Graphics.viewport.Width / (float)Graphics.viewport.Height, 0.01f, 100000),

                Texture = (Texture2D)new Sprite("grid").texture,
                TextureEnabled = true,
                VertexColorEnabled = true
            };
            _vertices[0].Position = (Vector3)new Vec3(0f, y, 0f);
            _vertices[0].TextureCoordinate = (Vector2)new Vec2(0f, 0f);
            _vertices[0].Color = (Microsoft.Xna.Framework.Color)new Color(1f, 1f, 1f, 1f);
            _vertices[1].Position = (Vector3)new Vec3(0f, 0f, 0f);
            _vertices[1].TextureCoordinate = (Vector2)new Vec2(0f, 11f);
            _vertices[1].Color = (Microsoft.Xna.Framework.Color)new Color(0.5f, 0.5f, 0.5f, 1f);
            _vertices[2].Position = (Vector3)new Vec3(x, y, 0f);
            _vertices[2].TextureCoordinate = (Vector2)new Vec2(8.5f, 0f);
            _vertices[2].Color = (Microsoft.Xna.Framework.Color)new Color(1f, 1f, 1f, 1f);
            _vertices[3].Position = (Vector3)new Vec3(x, 0f, 0f);
            _vertices[3].TextureCoordinate = (Vector2)new Vec2(8.5f, 11f);
            _vertices[3].Color = (Microsoft.Xna.Framework.Color)new Color(0.5f, 0.5f, 0.5f, 1f);
            _planet = new Sprite("background/planet");
        }

        public override void Update()
        {
            _scroll += 0.4f;
            if (_scroll <= 32f)
                return;
            _scroll -= 32f;
        }

        public override void Begin(bool transparent, bool isTargetDraw = false)
        {
            _effect.DiffuseColor = (Vector3)new Vec3(Graphics.fade, Graphics.fade, Graphics.fade);
            Graphics.screen = _batch;
            _batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, (MTEffect)_effect, camera.getMatrix());
        }

        public override void Draw(bool transparent, bool isTargetDraw = false)
        {
            Graphics.currentLayer = this;
            _effect.World = (Microsoft.Xna.Framework.Matrix)Matrix.CreateTranslation(new Vec3(190f, -75f, 300f));
            Begin(transparent, false);
            _planet.flipH = true;
            Graphics.Draw(_planet, 0f, 0f);
            _batch.End();
            _effect.World = (Microsoft.Xna.Framework.Matrix)(Matrix.CreateRotationX(Maths.DegToRad(90f)) * Matrix.CreateTranslation(new Vec3(-400f, 52f, 0f)));
            Begin(transparent, false);
            float num1 = 32f;
            int num2 = 26;
            int num3 = 16;
            for (int index = 0; index < num3; ++index)
                Graphics.DrawLine(new Vec2(0f + _scroll, index * num1), new Vec2(num1 * (num2 - 1), index * num1), Color.DarkGray, 3f);
            for (int index = 0; index < num2; ++index)
                Graphics.DrawLine(new Vec2(index * num1 + _scroll, 0f), new Vec2(index * num1 + _scroll, num1 * (num3 - 1)), Color.DarkGray, 3f);
            _batch.End();
            _effect.World = (Microsoft.Xna.Framework.Matrix)(Matrix.CreateRotationX(Maths.DegToRad(90f)) * Matrix.CreateTranslation(new Vec3(-400f, 62f, 0f)));
            Begin(transparent, false);
            for (int index = 0; index < num3; ++index)
                Graphics.DrawLine(new Vec2(0f + _scroll, index * num1), new Vec2(num1 * (num2 - 1), index * num1), Color.DarkGray * 0.2f, 3f);
            for (int index = 0; index < num2; ++index)
                Graphics.DrawLine(new Vec2(index * num1 + _scroll, 0f), new Vec2(index * num1 + _scroll, num1 * (num3 - 1)), Color.DarkGray * 0.2f, 3f);
            _batch.End();
            _effect.World = (Microsoft.Xna.Framework.Matrix)(Matrix.CreateRotationX(Maths.DegToRad(90f)) * Matrix.CreateTranslation(new Vec3(-400f, -52f, 0f)));
            Begin(transparent, false);
            for (int index = 0; index < num3; ++index)
                Graphics.DrawLine(new Vec2(0f + _scroll, index * num1), new Vec2(num1 * (num2 - 1), index * num1), Color.DarkGray, 3f);
            for (int index = 0; index < num2; ++index)
                Graphics.DrawLine(new Vec2(index * num1 + _scroll, 0f), new Vec2(index * num1 + _scroll, num1 * (num3 - 1)), Color.DarkGray, 3f);
            _batch.End();
            Graphics.screen = null;
            Graphics.currentLayer = null;
        }
    }
}

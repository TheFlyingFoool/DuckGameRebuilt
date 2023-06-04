// Decompiled with JetBrains decompiler
// Type: DuckGame.FieldBackground
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class FieldBackground : Layer
    {
        private Effect _fx;
        private float _scroll;
        public float fieldHeight;
        public Matrix _view;
        public Matrix _proj;
        private float _ypos;
        private List<Sprite> _sprites = new List<Sprite>();

        public float scroll
        {
            get => _scroll;
            set => _scroll = value;
        }

        public new Matrix view => _view;

        public new Matrix projection => _proj;

        public float rise { get; set; }

        public float ypos
        {
            get => _ypos;
            set => _ypos = value;
        }

        public void AddSprite(Sprite s) => _sprites.Add(s);

        public FieldBackground(string nameval, int depthval = 0)
          : base(nameval, depthval)
        {
            _fx = (Effect)Content.Load<MTEffect>("Shaders/fieldFadeAdd");
            _view = Matrix.CreateLookAt(new Vec3(0f, 0f, -5f), new Vec3(0f, 0f, 0f), Vec3.Up);
            _proj = Matrix.CreatePerspectiveFieldOfView(((float)Math.PI / 4.0f), (float)320 / (float)180, 0.01f, 100000);
        }

        public override void Update()
        {
            float num1 = 53f + fieldHeight + rise;
            float num2 = -0.1f;
            float scroll = this.scroll;
            _view = Matrix.CreateLookAt(new Vec3(scroll, 300f, -num1 + num2), new Vec3(scroll, 100f, -num1), Vec3.Down);
        }

        public override void Begin(bool transparent, bool isTargetDraw = false)
        {
            Vec3 vec3_1 = new Vec3((float)(Graphics.fade * _fade * (1f - _darken))) * colorMul;
            Vec3 vec3_2 = _colorAdd + new Vec3(_fadeAdd) + new Vec3(Graphics.flashAddRenderValue) + new Vec3(Graphics.fadeAddRenderValue) - new Vec3(darken);
            vec3_2 = new Vec3(Maths.Clamp(vec3_2.x, -1f, 1f), Maths.Clamp(vec3_2.y, -1f, 1f), Maths.Clamp(vec3_2.z, -1f, 1f));
            if (!Options.Data.flashing)
                vec3_2 = new Vec3(0f, 0f, 0f);
            if (_darken > 0f)
                _darken -= 0.15f;
            else
                _darken = 0f;
            if (_fx != null)
            {
                _fx.Parameters["fade"]?.SetValue((Vector3)vec3_1);
                _fx.Parameters["add"]?.SetValue((Vector3)vec3_2);
            }
            Graphics.screen = _batch;
            if (_state.ScissorTestEnable)
                Graphics.SetScissorRectangle(_scissor);
            _batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, _state, (MTEffect)_fx, camera.getMatrix());
        }

        public override void Draw(bool transparent, bool isTargetDraw = false)
        {
            Graphics.currentLayer = this;
            _fx.Parameters["WVP"].SetValue((Microsoft.Xna.Framework.Matrix)(_view * _proj));
            Begin(transparent, false);
            foreach (Sprite sprite in _sprites)
                Graphics.Draw(sprite, sprite.x, sprite.y);
            _batch.End();
            Graphics.screen = null;
            Graphics.currentLayer = null;
        }
    }
}

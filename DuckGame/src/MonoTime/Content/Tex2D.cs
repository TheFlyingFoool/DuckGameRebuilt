// Decompiled with JetBrains decompiler
// Type: DuckGame.Tex2D
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace DuckGame
{
    public class Tex2D : Tex2DBase
    {
        protected Texture2D _base;
        private RenderTarget2D _effectTexture;

        public RenderTarget2D effectTexture
        {
            get
            {
                if (_effectTexture == null)
                    _effectTexture = new RenderTarget2D(width, height);
                return _effectTexture;
            }
        }
        public string Namebase
        {
            get
            {
                return _base.Name;
            }
            set
            {
                _base.Name = value;
            }
        }
        public Texture2D Texbase => _base;
        public override object nativeObject => _base;

        public override int width => _base == null ? -1 : _base.Width;

        public override int height => _base == null ? -1 : _base.Height;

        internal Tex2D(Texture2D tex, string texName, short curTexIndex = 0)
          : base(texName, curTexIndex)
        {
            _base = tex;
            _frameWidth = tex.Width;
            _frameHeight = tex.Height;
        }

        public Tex2D(int width, int height)
          : base("__internal", 0)
        {
            _base = new Texture2D(DuckGame.Graphics.device, width, height, false, SurfaceFormat.Color);
            _frameWidth = width;
            _frameHeight = height;
            Content.AssignTextureIndex(this);
        }
        public void SaveAsPng(Stream stream, int width, int height)
        {
            if (_base == null)
                return;
            _base.SaveAsPng(stream, width, height);
        }
        public override void GetData<T>(T[] data)
        {
            if (_base == null)
                return;
            _base.GetData<T>(data);
        }

        public override Color[] GetData()
        {
            if (_base == null)
                return null;
            Color[] data = new Color[_base.Width * _base.Height];
            _base.GetData<Color>(data);
            return data;
        }

        public override void SetData<T>(T[] colors)
        {
            if (_base == null)
                return;
            _base.SetData<T>(colors);
        }

        public override void SetData(Color[] colors)
        {
            if (_base == null)
                return;
            _base.SetData<Color>(colors);
        }

        protected override void DisposeNative()
        {
            if (_base == null)
                return;
            if (!DuckGame.Graphics.disposingObjects)
            {
                lock (DuckGame.Graphics.objectsToDispose)
                    DuckGame.Graphics.objectsToDispose.Add(_base);
            }
            _base = null;
        }

        public static implicit operator Texture2D(Tex2D tex) => tex._base;

        public static implicit operator Tex2D(Texture2D tex) => Content.GetTex2D(tex);
    }
}

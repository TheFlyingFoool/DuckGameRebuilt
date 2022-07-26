// Decompiled with JetBrains decompiler
// Type: DuckGame.Tex2D
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

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
                if (this._effectTexture == null)
                    this._effectTexture = new RenderTarget2D(this.width, this.height);
                return this._effectTexture;
            }
        }

        public override object nativeObject => (object)this._base;

        public override int width => this._base == null ? -1 : this._base.Width;

        public override int height => this._base == null ? -1 : this._base.Height;

        internal Tex2D(Texture2D tex, string texName, short curTexIndex = 0)
          : base(texName, curTexIndex)
        {
            this._base = tex;
            this._frameWidth = (float)tex.Width;
            this._frameHeight = (float)tex.Height;
        }

        public Tex2D(int width, int height)
          : base("__internal", (short)0)
        {
            this._base = new Texture2D(DuckGame.Graphics.device, width, height, false, SurfaceFormat.Color);
            this._frameWidth = (float)width;
            this._frameHeight = (float)height;
            Content.AssignTextureIndex(this);
        }

        public override void GetData<T>(T[] data)
        {
            if (this._base == null)
                return;
            this._base.GetData<T>(data);
        }

        public override Color[] GetData()
        {
            if (this._base == null)
                return (Color[])null;
            Color[] data = new Color[this._base.Width * this._base.Height];
            this._base.GetData<Color>(data);
            return data;
        }

        public override void SetData<T>(T[] colors)
        {
            if (this._base == null)
                return;
            this._base.SetData<T>(colors);
        }

        public override void SetData(Color[] colors)
        {
            if (this._base == null)
                return;
            this._base.SetData<Color>(colors);
        }

        protected override void DisposeNative()
        {
            if (this._base == null)
                return;
            if (!DuckGame.Graphics.disposingObjects)
            {
                lock (DuckGame.Graphics.objectsToDispose)
                    DuckGame.Graphics.objectsToDispose.Add((GraphicsResource)this._base);
            }
            this._base = (Texture2D)null;
        }

        public static implicit operator Texture2D(Tex2D tex) => tex._base;

        public static implicit operator Tex2D(Texture2D tex) => Content.GetTex2D(tex);
    }
}

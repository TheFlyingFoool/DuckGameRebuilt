// Decompiled with JetBrains decompiler
// Type: DuckGame.RenderTarget2D
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class RenderTarget2D : Tex2D
    {
        public bool depth;

        public RenderTarget2D(int width, int height, bool pdepth, RenderTargetUsage usage)
          : base((Texture2D)new Microsoft.Xna.Framework.Graphics.RenderTarget2D(DuckGame.Graphics.device, MonoMain.hidef ? Math.Min(width, 4096) : Math.Min(width, 2048), MonoMain.hidef ? Math.Min(height, 4096) : Math.Min(height, 2048), false, SurfaceFormat.Color, pdepth ? DepthFormat.Depth24Stencil8 : DepthFormat.None, 0, usage), "__renderTarget")
        {
            this.depth = pdepth;
        }

        public RenderTarget2D(int width, int height, bool pdepth = false)
          : base((Texture2D)new Microsoft.Xna.Framework.Graphics.RenderTarget2D(DuckGame.Graphics.device, MonoMain.hidef ? Math.Min(width, 4096) : Math.Min(width, 2048), MonoMain.hidef ? Math.Min(height, 4096) : Math.Min(height, 2048), false, SurfaceFormat.Color, pdepth ? DepthFormat.Depth24Stencil8 : DepthFormat.None, 0, RenderTargetUsage.DiscardContents), "__renderTarget")
        {
            this.depth = pdepth;
        }

        public Tex2D ToTex2D()
        {
            Tex2D tex2D = new Tex2D(this.width, this.height);
            tex2D.SetData(this.GetData());
            return tex2D;
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.RenderTarget2D
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public RenderTarget2D(int width, int height, bool depthBuffer, bool mipmap, int msc, RenderTargetUsage usage)
            : base(
                new Microsoft.Xna.Framework.Graphics.RenderTarget2D(DuckGame.Graphics.device,
                    MonoMain.hidef ? Math.Min(width, 4096) : Math.Min(width, 2048),
                    MonoMain.hidef ? Math.Min(height, 4096) : Math.Min(height, 2048), mipmap, SurfaceFormat.Color,
                    depthBuffer ? DepthFormat.Depth24Stencil8 : DepthFormat.None, msc, usage), "__renderTarget")
        {
            depth = depthBuffer;
        }

        public RenderTarget2D(int width, int height, bool pdepth, RenderTargetUsage usage)
          : base(new Microsoft.Xna.Framework.Graphics.RenderTarget2D(DuckGame.Graphics.device, MonoMain.hidef ? Math.Min(width, 4096) : Math.Min(width, 2048), MonoMain.hidef ? Math.Min(height, 4096) : Math.Min(height, 2048), false, SurfaceFormat.Color, pdepth ? DepthFormat.Depth24Stencil8 : DepthFormat.None, 0, usage), "__renderTarget")
        {
            depth = pdepth;
        }

        public RenderTarget2D(int width, int height, bool pdepth = false)
          : base(new Microsoft.Xna.Framework.Graphics.RenderTarget2D(DuckGame.Graphics.device, MonoMain.hidef ? Math.Min(width, 4096) : Math.Min(width, 2048), MonoMain.hidef ? Math.Min(height, 4096) : Math.Min(height, 2048), false, SurfaceFormat.Color, pdepth ? DepthFormat.Depth24Stencil8 : DepthFormat.None, 0, RenderTargetUsage.DiscardContents), "__renderTarget")
        {
            depth = pdepth;
        }

        public RenderTarget2D(Microsoft.Xna.Framework.Graphics.RenderTarget2D _target) : base(_target, "__renderTarget")
        {
            depth = true;
        }

        public Tex2D ToTex2D()
        {
            Tex2D tex2D = new Tex2D(width, height);
            tex2D.SetData(GetData());
            return tex2D;
        }
    }
}

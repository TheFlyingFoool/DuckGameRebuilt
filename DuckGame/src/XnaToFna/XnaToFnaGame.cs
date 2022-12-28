// Decompiled with JetBrains decompiler
// Type: XnaToFna.XnaToFnaGame
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework;
using XnaToFna.ProxyForms;

namespace XnaToFna
{
    public class XnaToFnaGame : Game
    {
        public XnaToFnaGame() => XnaToFnaHelper.Initialize(this);

        protected override void Initialize()
        {
            base.Initialize();
            XnaToFnaHelper.ApplyChanges((GraphicsDeviceManager)Services.GetService(typeof(IGraphicsDeviceManager)));
        }

        protected override void EndDraw()
        {
            base.EndDraw();
            GameForm.Instance?.ApplyChanges();
        }
    }
}

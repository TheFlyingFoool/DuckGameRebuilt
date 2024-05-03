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

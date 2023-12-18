using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Threading;

namespace DuckGame
{
    public class SynchronizedContentManager : ContentManager
    {
        private object syncRoot = new object();
        public static volatile int blockLoading;

        public SynchronizedContentManager(IServiceProvider serviceProvider)
          : base(serviceProvider)
        {
        }

        public override T Load<T>(string assetName)
        {
            while (blockLoading > 0 && Thread.CurrentThread != MonoMain.mainThread)
                Thread.Sleep(2);
            lock (Content._loadLock)
            {
                try
                {
                    return base.Load<T>(assetName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Texture2D FromStream(Stream stream)
        {
            lock (Content._loadLock)
            {
                try
                {
                    return Texture2D.FromStream(((IGraphicsDeviceService)ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice, stream);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}

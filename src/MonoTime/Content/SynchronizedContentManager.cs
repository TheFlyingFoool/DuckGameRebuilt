// Decompiled with JetBrains decompiler
// Type: DuckGame.SynchronizedContentManager
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            while (SynchronizedContentManager.blockLoading > 0 && Thread.CurrentThread != MonoMain.mainThread)
                Thread.Sleep(2);
            lock (DuckGame.Content._loadLock)
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
            lock (DuckGame.Content._loadLock)
            {
                try
                {
                    return Texture2D.FromStream(((IGraphicsDeviceService)this.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice, stream);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}

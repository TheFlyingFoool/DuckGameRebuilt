using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace XnaToFna
{
    public class ContentHelperGame : Game
    {
        public readonly GraphicsDeviceManager GraphicsDeviceManager;
        public readonly Queue<Action> ActionQueue = new Queue<Action>();

        public Thread GameThread { get; protected set; }

        public ContentHelperGame() => GraphicsDeviceManager = new GraphicsDeviceManager(this);

        protected override void Initialize()
        {
            Window.Title = "XnaToFna ContentHelper Game (ignore me!)";
            base.Initialize();
            while (ActionQueue.Count > 0)
                ActionQueue.Dequeue()();
            Exit();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                Console.WriteLine(GetType().FullName + " failed disposing: " + ex);
            }
        }
    }
}

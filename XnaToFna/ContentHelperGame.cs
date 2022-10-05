// Decompiled with JetBrains decompiler
// Type: XnaToFna.ContentHelperGame
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

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

    public ContentHelperGame() => this.GraphicsDeviceManager = new GraphicsDeviceManager(this);

    protected override void Initialize()
    {
      this.Window.Title = "XnaToFna ContentHelper Game (ignore me!)";
      base.Initialize();
      while (this.ActionQueue.Count > 0)
        this.ActionQueue.Dequeue()();
      this.Exit();
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        base.Dispose(disposing);
      }
      catch (Exception ex)
      {
        Console.WriteLine(this.GetType().FullName + " failed disposing: " + ex);
      }
    }
  }
}

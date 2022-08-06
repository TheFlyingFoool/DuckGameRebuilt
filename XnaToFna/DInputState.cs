// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyDInput.DInputState
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System.Collections.Generic;

namespace XnaToFna.ProxyDInput
{
  public class DInputState
  {
    public float leftX;
    public float leftY;
    public float leftZ;
    public float rightX;
    public float rightY;
    public float rightZ;
    public float slider1;
    public float slider2;
    public bool left;
    public bool right;
    public bool up;
    public bool down;
    public List<bool> buttons = new List<bool>();
    internal bool connected;
  }
}

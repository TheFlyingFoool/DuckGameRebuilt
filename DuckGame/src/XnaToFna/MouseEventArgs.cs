// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyForms.MouseEventArgs
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;

namespace XnaToFna.ProxyForms
{
    public class MouseEventArgs : EventArgs
    {
        public MouseButtons Button { get; private set; }

        public int Clicks { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Delta { get; private set; }

        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }
    }
}

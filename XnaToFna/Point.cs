// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyDrawing.Point
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;

namespace XnaToFna.ProxyDrawing
{
  [Serializable]
  public struct Point
  {
    public static readonly Point Empty;
    private int x;
    private int y;

    public bool IsEmpty => this == Point.Empty;

    public int X
    {
      get => this.x;
      set => this.x = value;
    }

    public int Y
    {
      get => this.y;
      set => this.y = value;
    }

    public Point(int dw)
      : this(dw & ushort.MaxValue, dw >> 16)
    {
    }

    public Point(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public void Offset(Point p) => this.Offset(p.X, p.Y);

    public void Offset(int x, int y)
    {
      this.x += x;
      this.y += y;
    }

    public static bool operator !=(Point left, Point right) => !(left == right);

    public static bool operator ==(Point left, Point right) => left.x == right.x && left.y == right.y;

    public override bool Equals(object obj) => obj is Point point && this == point;

    public override int GetHashCode() => this.x ^ this.y;

    public override string ToString() => string.Format("{{X={0},Y={1}}}", x, y);
  }
}

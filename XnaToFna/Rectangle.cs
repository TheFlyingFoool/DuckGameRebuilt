// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyDrawing.Rectangle
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;

namespace XnaToFna.ProxyDrawing
{
  [Serializable]
  public struct Rectangle
  {
    public static readonly Rectangle Empty;
    private int x;
    private int y;
    private int width;
    private int height;

    public bool IsEmpty => this == Rectangle.Empty;

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

    public int Width
    {
      get => this.width;
      set => this.width = value;
    }

    public int Height
    {
      get => this.height;
      set => this.height = value;
    }

    public int Left => this.X;

    public int Top => this.y;

    public int Right => this.X + this.Width;

    public int Bottom => this.y + this.height;

    public Rectangle(int x, int y, int width, int height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    public void Inflate(int width, int height)
    {
      this.x -= width;
      this.y -= height;
      this.width += width * 2;
      this.height += height * 2;
    }

    public void Intersect(Rectangle rect)
    {
      if (!this.IntersectsWithInclusive(rect))
      {
        this.x = this.y = this.width = this.height = 0;
      }
      else
      {
        this.x = Math.Max(this.Left, rect.Left);
        this.y = Math.Max(this.Top, rect.Top);
        this.width = Math.Min(this.Right, rect.Right) - this.x;
        this.height = Math.Min(this.Bottom, rect.Bottom) - this.y;
      }
    }

    public bool Contains(Point p) => this.Contains(p.X, p.Y);

    public bool Contains(int x, int y) => x >= this.Left && x < this.Right && y >= this.Top && y < this.Bottom;

    public bool Contains(Rectangle rect) => rect == Rectangle.Intersect(this, rect);

    public bool IntersectsWith(Rectangle rect) => this.Left < rect.Right && this.Right > rect.Left && this.Top < rect.Bottom && this.Bottom > rect.Top;

    private bool IntersectsWithInclusive(Rectangle rect) => this.Left <= rect.Right && this.Right >= rect.Left && this.Top <= rect.Bottom && this.Bottom >= rect.Top;

    public void Offset(Point p) => this.Offset(p.X, p.Y);

    public void Offset(int x, int y)
    {
      this.x += x;
      this.y += y;
    }

    public static Rectangle FromLTRB(int left, int top, int right, int bottom) => new Rectangle(left, top, right - left, bottom - top);

    public static Rectangle Inflate(Rectangle rect, int x, int y)
    {
      Rectangle rectangle = new Rectangle(rect.x, rect.y, rect.width, rect.height);
      rectangle.Inflate(x, y);
      return rectangle;
    }

    public static Rectangle Intersect(Rectangle a, Rectangle b)
    {
      a = new Rectangle(a.x, a.y, a.width, a.height);
      a.Intersect(b);
      return a;
    }

    public static Rectangle Union(Rectangle a, Rectangle b) => Rectangle.FromLTRB(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));

    public static bool operator !=(Rectangle left, Rectangle right) => !(left == right);

    public static bool operator ==(Rectangle left, Rectangle right) => left.x == right.x && left.y == right.y && left.width == right.width && left.height == right.height;

    public override bool Equals(object obj) => obj is Rectangle rectangle && this == rectangle;

    public override int GetHashCode() => this.height + this.width ^ this.x + this.y;

    public override string ToString() => string.Format("{{X={0},Y={1},Width={2},Height={3}}}", x, y, width, height);
  }
}

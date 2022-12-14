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

        public bool IsEmpty => this == Empty;

        public int X
        {
            get => x;
            set => x = value;
        }

        public int Y
        {
            get => y;
            set => y = value;
        }

        public int Width
        {
            get => width;
            set => width = value;
        }

        public int Height
        {
            get => height;
            set => height = value;
        }

        public int Left => X;

        public int Top => y;

        public int Right => X + Width;

        public int Bottom => y + height;

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void Inflate(int width, int height)
        {
            x -= width;
            y -= height;
            this.width += width * 2;
            this.height += height * 2;
        }

        public void Intersect(Rectangle rect)
        {
            if (!IntersectsWithInclusive(rect))
            {
                x = y = width = height = 0;
            }
            else
            {
                x = Math.Max(Left, rect.Left);
                y = Math.Max(Top, rect.Top);
                width = Math.Min(Right, rect.Right) - x;
                height = Math.Min(Bottom, rect.Bottom) - y;
            }
        }

        public bool Contains(Point p) => Contains(p.X, p.Y);

        public bool Contains(int x, int y) => x >= Left && x < Right && y >= Top && y < Bottom;

        public bool Contains(Rectangle rect) => rect == Intersect(this, rect);

        public bool IntersectsWith(Rectangle rect) => Left < rect.Right && Right > rect.Left && Top < rect.Bottom && Bottom > rect.Top;

        private bool IntersectsWithInclusive(Rectangle rect) => Left <= rect.Right && Right >= rect.Left && Top <= rect.Bottom && Bottom >= rect.Top;

        public void Offset(Point p) => Offset(p.X, p.Y);

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

        public static Rectangle Union(Rectangle a, Rectangle b) => FromLTRB(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));

        public static bool operator !=(Rectangle left, Rectangle right) => !(left == right);

        public static bool operator ==(Rectangle left, Rectangle right) => left.x == right.x && left.y == right.y && left.width == right.width && left.height == right.height;

        public override bool Equals(object obj) => obj is Rectangle rectangle && this == rectangle;

        public override int GetHashCode() => height + width ^ x + y;

        public override string ToString() => string.Format("{{X={0},Y={1},Width={2},Height={3}}}", x, y, width, height);
    }
}

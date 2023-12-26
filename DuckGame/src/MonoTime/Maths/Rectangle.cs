using System;

namespace DuckGame
{
    [Serializable]
    public struct Rectangle
    {
        public float height;
        public float width;
        public float x;
        public float y;

        public float Top
        {
            get => y;
            set => y = value;
        }

        public float Bottom
        {
            get => y + height;
            set => y = value - height;
        }

        public float Left
        {
            get => x;
            set => x = value;
        }

        public float Right
        {
            get => x + width;
            set => x = value - width;
        }

        public Vec2 tl => new Vec2(x, y);

        public Vec2 tr => new Vec2(x + width, y);

        public Vec2 bl => new Vec2(x, y + height);

        public Vec2 br => new Vec2(x + width, y + height);

        public Vec2 Center
        {
            get => new Vec2(x + width / 2f, y + height / 2f);
            set
            {
                x = value.x - width / 2f;
                y = value.y - height / 2f;
            }
        }

        public float aspect => width / height;

        public Rectangle(float x, float y, float width, float height)
        {
            if (width > 0)
            {
                this.x = x;
                this.width = width;
            }
            else
            {
                this.x = x + width;
                this.width = -width;
            }
            
            if (height > 0)
            {
                this.y = y;
                this.height = height;
            }
            else
            {
                this.y = y + height;
                this.height = -height;
            }
        }

        public Rectangle(Vec2 tl, Vec2 br)
        {
            if (tl.x > br.x)
            {
                float x = br.x;
                br.x = tl.x;
                tl.x = x;
            }
            if (tl.y > br.y)
            {
                float y = br.y;
                br.y = tl.y;
                tl.y = y;
            }
            x = tl.x;
            y = tl.y;
            width = br.x - tl.x;
            height = br.y - tl.y;
        }
        
        public static bool TryParse(string s, out Rectangle result)
        {
            if (s.Split(',') is { Length: 4 } values
                && float.TryParse(values[0], out float x)
                && float.TryParse(values[1], out float y)
                && float.TryParse(values[2], out float w)
                && float.TryParse(values[3], out float h))
            {
                result = new Rectangle(x, y, w, h);
                return true;
            }

            result = default;
            return false;
        }

        public static implicit operator Microsoft.Xna.Framework.Rectangle(Rectangle r) => new Microsoft.Xna.Framework.Rectangle((int)r.x, (int)r.y, (int)r.width, (int)r.height);

        public static implicit operator Rectangle(Microsoft.Xna.Framework.Rectangle r) => new Rectangle(r.X, r.Y, r.Width, r.Height);

        public bool Contains(Vec2 position) => position.x >= x && position.y >= y && position.x <= x + width && position.y <= y + height;

        public Rectangle GetQuadrant(int pQuadrantStartingWithTLClockwise)
        {
            switch (pQuadrantStartingWithTLClockwise)
            {
                case 0:
                    return new Rectangle(x, y, width / 2f, height / 2f);
                case 1:
                    return new Rectangle(x + width / 2f, y, width / 2f, height / 2f);
                case 2:
                    return new Rectangle(x + width / 2f, y + height / 2f, width / 2f, height / 2f);
                default:
                    return new Rectangle(x, y + height / 2f, width / 2f, height / 2f);
            }
        }

        public Rectangle Shrink(float fromAllSides)
        {
            return Shrink(fromAllSides, fromAllSides, fromAllSides, fromAllSides);
        }

        public Rectangle Shrink(float fromHorizontal, float fromVertical)
        {
            return Shrink(fromHorizontal, fromHorizontal, fromVertical, fromVertical);
        }

        public Rectangle Shrink(float fromLeft, float fromRight, float fromTop, float fromBottom)
        {
            return new Rectangle(x + fromLeft, y + fromTop, width - fromLeft - fromRight, height - fromTop - fromBottom);
        }
    }
}

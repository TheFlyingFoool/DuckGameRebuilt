// Decompiled with JetBrains decompiler
// Type: DuckGame.Rectangle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            get => this.y;
            set => this.y = value;
        }

        public float Bottom
        {
            get => this.y + this.height;
            set => this.y = value - this.height;
        }

        public float Left
        {
            get => this.x;
            set => this.x = value;
        }

        public float Right
        {
            get => this.x + this.width;
            set => this.x = value - this.width;
        }

        public Vec2 tl => new Vec2(this.x, this.y);

        public Vec2 tr => new Vec2(this.x + this.width, this.y);

        public Vec2 bl => new Vec2(this.x, this.y + this.height);

        public Vec2 br => new Vec2(this.x + this.width, this.y + this.height);

        public Vec2 Center
        {
            get => new Vec2(this.x + this.width / 2f, this.y + this.height / 2f);
            set
            {
                this.x = value.x - this.width / 2f;
                this.y = value.y - this.height / 2f;
            }
        }

        public float aspect => this.width / this.height;

        public Rectangle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
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
            this.x = tl.x;
            this.y = tl.y;
            this.width = br.x - tl.x;
            this.height = br.y - tl.y;
        }

        public static implicit operator Microsoft.Xna.Framework.Rectangle(Rectangle r) => new Microsoft.Xna.Framework.Rectangle((int)r.x, (int)r.y, (int)r.width, (int)r.height);

        public static implicit operator Rectangle(Microsoft.Xna.Framework.Rectangle r) => new Rectangle(r.X, r.Y, r.Width, r.Height);

        public bool Contains(Vec2 position) => position.x >= this.x && position.y >= this.y && position.x <= x + this.width && position.y <= y + this.height;

        public Rectangle GetQuadrant(int pQuadrantStartingWithTLClockwise)
        {
            switch (pQuadrantStartingWithTLClockwise)
            {
                case 0:
                    return new Rectangle(this.x, this.y, this.width / 2f, this.height / 2f);
                case 1:
                    return new Rectangle(this.x + this.width / 2f, this.y, this.width / 2f, this.height / 2f);
                case 2:
                    return new Rectangle(this.x + this.width / 2f, this.y + this.height / 2f, this.width / 2f, this.height / 2f);
                default:
                    return new Rectangle(this.x, this.y + this.height / 2f, this.width / 2f, this.height / 2f);
            }
        }
    }
}

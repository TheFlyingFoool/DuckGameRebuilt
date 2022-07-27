// Decompiled with JetBrains decompiler
// Type: DuckGame.GeometryItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class GeometryItem
    {
        public bool temporary;
        public float depth;
        public VertexPositionColor[] vertices;
        public int length;
        public int size = 512;
        public Material material;

        public GeometryItem() => this.vertices = new VertexPositionColor[this.size];

        public void Clear() => this.length = 0;

        public void AddTriangle(Vec2 p1, Vec2 p2, Vec2 p3, Color c)
        {
            if (this.length + 3 >= this.size)
            {
                VertexPositionColor[] vertexPositionColorArray = new VertexPositionColor[this.size * 2];
                this.vertices.CopyTo(vertexPositionColorArray, 0);
                this.vertices = vertexPositionColorArray;
                this.size *= 2;
            }
            this.vertices[this.length].Position.X = p1.x;
            this.vertices[this.length].Position.Y = p1.y;
            this.vertices[this.length].Position.Z = this.depth;
            this.vertices[this.length].Color = (Microsoft.Xna.Framework.Color)c;
            this.vertices[this.length + 1].Position.X = p2.x;
            this.vertices[this.length + 1].Position.Y = p2.y;
            this.vertices[this.length + 1].Position.Z = this.depth;
            this.vertices[this.length + 1].Color = (Microsoft.Xna.Framework.Color)c;
            this.vertices[this.length + 2].Position.X = p3.x;
            this.vertices[this.length + 2].Position.Y = p3.y;
            this.vertices[this.length + 2].Position.Z = this.depth;
            this.vertices[this.length + 2].Color = (Microsoft.Xna.Framework.Color)c;
            this.length += 3;
        }

        public void AddTriangle(Vec2 p1, Vec2 p2, Vec2 p3, Color c, Color c2, Color c3)
        {
            if (this.length + 3 >= this.size)
            {
                VertexPositionColor[] vertexPositionColorArray = new VertexPositionColor[this.size * 2];
                this.vertices.CopyTo(vertexPositionColorArray, 0);
                this.vertices = vertexPositionColorArray;
                this.size *= 2;
            }
            this.vertices[this.length].Position.X = p1.x;
            this.vertices[this.length].Position.Y = p1.y;
            this.vertices[this.length].Position.Z = this.depth;
            this.vertices[this.length].Color = (Microsoft.Xna.Framework.Color)c;
            this.vertices[this.length + 1].Position.X = p2.x;
            this.vertices[this.length + 1].Position.Y = p2.y;
            this.vertices[this.length + 1].Position.Z = this.depth;
            this.vertices[this.length + 1].Color = (Microsoft.Xna.Framework.Color)c2;
            this.vertices[this.length + 2].Position.X = p3.x;
            this.vertices[this.length + 2].Position.Y = p3.y;
            this.vertices[this.length + 2].Position.Z = this.depth;
            this.vertices[this.length + 2].Color = (Microsoft.Xna.Framework.Color)c3;
            this.length += 3;
        }
    }
}

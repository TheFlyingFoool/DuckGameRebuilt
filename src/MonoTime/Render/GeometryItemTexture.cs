// Decompiled with JetBrains decompiler
// Type: DuckGame.GeometryItemTexture
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class GeometryItemTexture
    {
        public bool temporary;
        public float depth;
        public VertexPositionColorTexture[] vertices;
        public Texture2D texture;
        public int length;
        public int size = 512;

        public GeometryItemTexture() => vertices = new VertexPositionColorTexture[size];

        public void Clear() => length = 0;

        public void AddTriangle(Vec2 p1, Vec2 p2, Vec2 p3, Vec2 tx1, Vec2 tx2, Vec2 tx3, Color c)
        {
            if (length + 3 >= size)
            {
                VertexPositionColorTexture[] positionColorTextureArray = new VertexPositionColorTexture[size * 2];
                vertices.CopyTo(positionColorTextureArray, 0);
                vertices = positionColorTextureArray;
                size *= 2;
            }
            vertices[length].Position.X = p1.x;
            vertices[length].Position.Y = p1.y;
            vertices[length].Position.Z = depth;
            vertices[length].TextureCoordinate.X = tx1.x;
            vertices[length].TextureCoordinate.Y = tx1.y;
            vertices[length].Color = (Microsoft.Xna.Framework.Color)c;
            vertices[length + 1].Position.X = p2.x;
            vertices[length + 1].Position.Y = p2.y;
            vertices[length + 1].Position.Z = depth;
            vertices[length + 1].TextureCoordinate.X = tx2.x;
            vertices[length + 1].TextureCoordinate.Y = tx2.y;
            vertices[length + 1].Color = (Microsoft.Xna.Framework.Color)c;
            vertices[length + 2].Position.X = p3.x;
            vertices[length + 2].Position.Y = p3.y;
            vertices[length + 2].Position.Z = depth;
            vertices[length + 2].TextureCoordinate.X = tx3.x;
            vertices[length + 2].TextureCoordinate.Y = tx3.y;
            vertices[length + 2].Color = (Microsoft.Xna.Framework.Color)c;
            length += 3;
        }

        public void AddTriangle(Vec2 p1, Vec2 p2, Vec2 p3, Color c, Color c2, Color c3)
        {
            if (length + 3 >= size)
            {
                VertexPositionColorTexture[] positionColorTextureArray = new VertexPositionColorTexture[size * 2];
                vertices.CopyTo(positionColorTextureArray, 0);
                vertices = positionColorTextureArray;
                size *= 2;
            }
            vertices[length].Position.X = p1.x;
            vertices[length].Position.Y = p1.y;
            vertices[length].Position.Z = depth;
            vertices[length].Color = (Microsoft.Xna.Framework.Color)c;
            vertices[length + 1].Position.X = p2.x;
            vertices[length + 1].Position.Y = p2.y;
            vertices[length + 1].Position.Z = depth;
            vertices[length + 1].Color = (Microsoft.Xna.Framework.Color)c2;
            vertices[length + 2].Position.X = p3.x;
            vertices[length + 2].Position.Y = p3.y;
            vertices[length + 2].Position.Z = depth;
            vertices[length + 2].Color = (Microsoft.Xna.Framework.Color)c3;
            length += 3;
        }
    }
}

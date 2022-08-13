// Decompiled with JetBrains decompiler
// Type: DuckGame.MTSpriteBatchItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MTSpriteBatchItem
    {
        public MTSpriteBatchItemMetaData MetaData;
        public bool inPool = false;
        public Texture2D Texture;
        public Material Material;
        public float Depth;
        public VertexPositionColorTexture vertexTL;
        public VertexPositionColorTexture vertexTR;
        public VertexPositionColorTexture vertexBL;
        public VertexPositionColorTexture vertexBR;

        public MTSpriteBatchItem()
        {
            vertexTL = new VertexPositionColorTexture();
            vertexTR = new VertexPositionColorTexture();
            vertexBL = new VertexPositionColorTexture();
            vertexBR = new VertexPositionColorTexture();
        }

        public void Set(
          float x,
          float y,
          float w,
          float h,
          Color color,
          Vec2 texCoordTL,
          Vec2 texCoordBR)
        {
            Microsoft.Xna.Framework.Color msc = (Microsoft.Xna.Framework.Color)color;
            vertexTL.Position.X = x;
            vertexTL.Position.Y = y;
            vertexTL.Position.Z = Depth;
            vertexTL.Color = msc;
            vertexTL.TextureCoordinate.X = texCoordTL.x;
            vertexTL.TextureCoordinate.Y = texCoordTL.y;
            vertexTR.Position.X = x + w;
            vertexTR.Position.Y = y;
            vertexTR.Position.Z = Depth;
            vertexTR.Color = msc;
            vertexTR.TextureCoordinate.X = texCoordBR.x;
            vertexTR.TextureCoordinate.Y = texCoordTL.y;
            vertexBL.Position.X = x;
            vertexBL.Position.Y = y + h;
            vertexBL.Position.Z = Depth;
            vertexBL.Color = msc;
            vertexBL.TextureCoordinate.X = texCoordTL.x;
            vertexBL.TextureCoordinate.Y = texCoordBR.y;
            vertexBR.Position.X = x + w;
            vertexBR.Position.Y = y + h;
            vertexBR.Position.Z = Depth;
            vertexBR.Color = msc;
            vertexBR.TextureCoordinate.X = texCoordBR.x;
            vertexBR.TextureCoordinate.Y = texCoordBR.y;
        }

        public void Set(
          Vec2 p1,
          Vec2 p2,
          Vec2 p3,
          Vec2 p4,
          Vec2 t1,
          Vec2 t2,
          Vec2 t3,
          Vec2 t4,
          Color c)
        {
            Microsoft.Xna.Framework.Color msc = (Microsoft.Xna.Framework.Color)c;
            vertexTL.Position.X = p1.x;
            vertexTL.Position.Y = p1.y;
            vertexTL.Position.Z = Depth;
            vertexTL.Color = msc;
            vertexTL.TextureCoordinate.X = t1.x;
            vertexTL.TextureCoordinate.Y = t1.y;
            vertexTR.Position.X = p2.x;
            vertexTR.Position.Y = p2.y;
            vertexTR.Position.Z = Depth;
            vertexTR.Color = msc;
            vertexTR.TextureCoordinate.X = t2.x;
            vertexTR.TextureCoordinate.Y = t2.y;
            vertexBL.Position.X = p3.x;
            vertexBL.Position.Y = p3.y;
            vertexBL.Position.Z = Depth;
            vertexBL.Color = msc;
            vertexBL.TextureCoordinate.X = t3.x;
            vertexBL.TextureCoordinate.Y = t3.y;
            vertexBR.Position.X = p4.x;
            vertexBR.Position.Y = p4.y;
            vertexBR.Position.Z = Depth;
            vertexBR.Color = msc;
            vertexBR.TextureCoordinate.X = t4.x;
            vertexBR.TextureCoordinate.Y = t4.y;
        }

        public void Set(
          float x,
          float y,
          float dx,
          float dy,
          float w,
          float h,
          float sin,
          float cos,
          Color color,
          Vec2 texCoordTL,
          Vec2 texCoordBR)
        {
            Microsoft.Xna.Framework.Color msc = (Microsoft.Xna.Framework.Color)color;
            vertexTL.Position.X = (float)(x + dx * cos - dy * sin);
            vertexTL.Position.Y = (float)(y + dx * sin + dy * cos);
            vertexTL.Position.Z = Depth;
            vertexTL.Color = msc;
            vertexTL.TextureCoordinate.X = texCoordTL.x;
            vertexTL.TextureCoordinate.Y = texCoordTL.y;
            vertexTR.Position.X = (float)(x + (dx + w) * cos - dy * sin);
            vertexTR.Position.Y = (float)(y + (dx + w) * sin + dy * cos);
            vertexTR.Position.Z = Depth;
            vertexTR.Color = msc;
            vertexTR.TextureCoordinate.X = texCoordBR.x;
            vertexTR.TextureCoordinate.Y = texCoordTL.y;
            vertexBL.Position.X = (float)(x + dx * cos - (dy + h) * sin);
            vertexBL.Position.Y = (float)(y + dx * sin + (dy + h) * cos);
            vertexBL.Position.Z = Depth;
            vertexBL.Color = msc;
            vertexBL.TextureCoordinate.X = texCoordTL.x;
            vertexBL.TextureCoordinate.Y = texCoordBR.y;
            vertexBR.Position.X = (float)(x + (dx + w) * cos - (dy + h) * sin);
            vertexBR.Position.Y = (float)(y + (dx + w) * sin + (dy + h) * cos);
            vertexBR.Position.Z = Depth;
            vertexBR.Color = msc;
            vertexBR.TextureCoordinate.X = texCoordBR.x;
            vertexBR.TextureCoordinate.Y = texCoordBR.y;
        }
    }
}

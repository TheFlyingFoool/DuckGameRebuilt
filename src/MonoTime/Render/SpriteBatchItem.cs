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
        public bool inPool = true;
        public Texture2D Texture;
        public Material Material;
        public float Depth;
        public VertexPositionColorTexture vertexTL;
        public VertexPositionColorTexture vertexTR;
        public VertexPositionColorTexture vertexBL;
        public VertexPositionColorTexture vertexBR;

        public MTSpriteBatchItem()
        {
            this.vertexTL = new VertexPositionColorTexture();
            this.vertexTR = new VertexPositionColorTexture();
            this.vertexBL = new VertexPositionColorTexture();
            this.vertexBR = new VertexPositionColorTexture();
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
            this.vertexTL.Position.X = x;
            this.vertexTL.Position.Y = y;
            this.vertexTL.Position.Z = this.Depth;
            this.vertexTL.Color = (Microsoft.Xna.Framework.Color)color;
            this.vertexTL.TextureCoordinate.X = texCoordTL.x;
            this.vertexTL.TextureCoordinate.Y = texCoordTL.y;
            this.vertexTR.Position.X = x + w;
            this.vertexTR.Position.Y = y;
            this.vertexTR.Position.Z = this.Depth;
            this.vertexTR.Color = (Microsoft.Xna.Framework.Color)color;
            this.vertexTR.TextureCoordinate.X = texCoordBR.x;
            this.vertexTR.TextureCoordinate.Y = texCoordTL.y;
            this.vertexBL.Position.X = x;
            this.vertexBL.Position.Y = y + h;
            this.vertexBL.Position.Z = this.Depth;
            this.vertexBL.Color = (Microsoft.Xna.Framework.Color)color;
            this.vertexBL.TextureCoordinate.X = texCoordTL.x;
            this.vertexBL.TextureCoordinate.Y = texCoordBR.y;
            this.vertexBR.Position.X = x + w;
            this.vertexBR.Position.Y = y + h;
            this.vertexBR.Position.Z = this.Depth;
            this.vertexBR.Color = (Microsoft.Xna.Framework.Color)color;
            this.vertexBR.TextureCoordinate.X = texCoordBR.x;
            this.vertexBR.TextureCoordinate.Y = texCoordBR.y;
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
            this.vertexTL.Position.X = p1.x;
            this.vertexTL.Position.Y = p1.y;
            this.vertexTL.Position.Z = this.Depth;
            this.vertexTL.Color = (Microsoft.Xna.Framework.Color)c;
            this.vertexTL.TextureCoordinate.X = t1.x;
            this.vertexTL.TextureCoordinate.Y = t1.y;
            this.vertexTR.Position.X = p2.x;
            this.vertexTR.Position.Y = p2.y;
            this.vertexTR.Position.Z = this.Depth;
            this.vertexTR.Color = (Microsoft.Xna.Framework.Color)c;
            this.vertexTR.TextureCoordinate.X = t2.x;
            this.vertexTR.TextureCoordinate.Y = t2.y;
            this.vertexBL.Position.X = p3.x;
            this.vertexBL.Position.Y = p3.y;
            this.vertexBL.Position.Z = this.Depth;
            this.vertexBL.Color = (Microsoft.Xna.Framework.Color)c;
            this.vertexBL.TextureCoordinate.X = t3.x;
            this.vertexBL.TextureCoordinate.Y = t3.y;
            this.vertexBR.Position.X = p4.x;
            this.vertexBR.Position.Y = p4.y;
            this.vertexBR.Position.Z = this.Depth;
            this.vertexBR.Color = (Microsoft.Xna.Framework.Color)c;
            this.vertexBR.TextureCoordinate.X = t4.x;
            this.vertexBR.TextureCoordinate.Y = t4.y;
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
            this.vertexTL.Position.X = (float)((double)x + (double)dx * (double)cos - (double)dy * (double)sin);
            this.vertexTL.Position.Y = (float)((double)y + (double)dx * (double)sin + (double)dy * (double)cos);
            this.vertexTL.Position.Z = this.Depth;
            this.vertexTL.Color = (Microsoft.Xna.Framework.Color)color;
            this.vertexTL.TextureCoordinate.X = texCoordTL.x;
            this.vertexTL.TextureCoordinate.Y = texCoordTL.y;
            this.vertexTR.Position.X = (float)((double)x + ((double)dx + (double)w) * (double)cos - (double)dy * (double)sin);
            this.vertexTR.Position.Y = (float)((double)y + ((double)dx + (double)w) * (double)sin + (double)dy * (double)cos);
            this.vertexTR.Position.Z = this.Depth;
            this.vertexTR.Color = (Microsoft.Xna.Framework.Color)color;
            this.vertexTR.TextureCoordinate.X = texCoordBR.x;
            this.vertexTR.TextureCoordinate.Y = texCoordTL.y;
            this.vertexBL.Position.X = (float)((double)x + (double)dx * (double)cos - ((double)dy + (double)h) * (double)sin);
            this.vertexBL.Position.Y = (float)((double)y + (double)dx * (double)sin + ((double)dy + (double)h) * (double)cos);
            this.vertexBL.Position.Z = this.Depth;
            this.vertexBL.Color = (Microsoft.Xna.Framework.Color)color;
            this.vertexBL.TextureCoordinate.X = texCoordTL.x;
            this.vertexBL.TextureCoordinate.Y = texCoordBR.y;
            this.vertexBR.Position.X = (float)((double)x + ((double)dx + (double)w) * (double)cos - ((double)dy + (double)h) * (double)sin);
            this.vertexBR.Position.Y = (float)((double)y + ((double)dx + (double)w) * (double)sin + ((double)dy + (double)h) * (double)cos);
            this.vertexBR.Position.Z = this.Depth;
            this.vertexBR.Color = (Microsoft.Xna.Framework.Color)color;
            this.vertexBR.TextureCoordinate.X = texCoordBR.x;
            this.vertexBR.TextureCoordinate.Y = texCoordBR.y;
        }
    }
}

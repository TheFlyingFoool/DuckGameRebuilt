// Decompiled with JetBrains decompiler
// Type: DuckGame.MTSpriteBatchItem
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class MTSpriteBatchItem : IComparable<MTSpriteBatchItem>
    {
        public bool usingspriteatlas;
        public Tex2D NormalTexture;
        public Vec2 Offsets = Vec2.Zero;
        public float DepthIndex;
        public MTSpriteBatchItemMetaData MetaData;
        public bool inPool = false;
        public Texture2D Texture;
        public Material Material;
        public float Depth;
        public VertexPositionColorTexture vertexTL;
        public VertexPositionColorTexture vertexTR;
        public VertexPositionColorTexture vertexBL;
        public VertexPositionColorTexture vertexBR;
        public float prevx;
        public float prevy;
        private float prevdx;
        private float prevdy;
        private float prevw;
        private float prevh;
        private float prevsin;
        private float prevcos;

        public MTSpriteBatchItem()
        {
            vertexTL = new VertexPositionColorTexture();
            vertexTR = new VertexPositionColorTexture();
            vertexBL = new VertexPositionColorTexture();
            vertexBR = new VertexPositionColorTexture();
        }
        public int CompareTo(MTSpriteBatchItem other)
        {
            return Depth.CompareTo(other.Depth);
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
            prevx = x;
            prevy = y;
            prevw = w;
            prevh = h;
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
        public void ChangePosition(float x = -1f,
          float y = -1f,
          float dx = -1f,
          float dy = -1f,
          float w = -1f,
          float h = -1f,
          float sin = -1f,
          float cos = -1f) // dan i might come back to this :))))))
        {
            if (x   == -1)   { x = prevx; };
            if (y   == -1)   { y = prevy; };
            if (dx  == -1)   { dx = prevdx; };
            if (dy  == -1)   { dy = prevdy; };
            if (w   == -1)   { w = prevw; };
            if (h   == -1)   { h = prevh; };
            if (sin == -1)   { sin = prevsin; };
            if (cos == -1)   { cos = prevcos; };
            vertexTL.Position.X = x + dx * cos - dy * sin;
            vertexTL.Position.Y = y + dx * sin + dy * cos;
            vertexTR.Position.X = x + (dx + w) * cos - dy * sin;
            vertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
            vertexBL.Position.X = x + dx * cos - (dy + h) * sin;
            vertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
            vertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
            vertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
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
            prevx = x;
            prevy = y;
            prevdx = dx;
            prevdy = dy;
            prevw = w;
            prevh = h;
            prevsin = sin;
            prevcos = cos;
            Microsoft.Xna.Framework.Color msc = (Microsoft.Xna.Framework.Color)color;
            vertexTL.Position.X = x + dx * cos - dy * sin;
            vertexTL.Position.Y = y + dx * sin + dy * cos;
            vertexTL.Position.Z = Depth;
            vertexTL.Color = msc;
            vertexTL.TextureCoordinate.X = texCoordTL.x;
            vertexTL.TextureCoordinate.Y = texCoordTL.y;
            vertexTR.Position.X = x + (dx + w) * cos - dy * sin;
            vertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
            vertexTR.Position.Z = Depth;
            vertexTR.Color = msc;
            vertexTR.TextureCoordinate.X = texCoordBR.x;
            vertexTR.TextureCoordinate.Y = texCoordTL.y;
            vertexBL.Position.X = x + dx * cos - (dy + h) * sin;
            vertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
            vertexBL.Position.Z = Depth;
            vertexBL.Color = msc;
            vertexBL.TextureCoordinate.X = texCoordTL.x;
            vertexBL.TextureCoordinate.Y = texCoordBR.y;
            vertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
            vertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
            vertexBR.Position.Z = Depth;
            vertexBR.Color = msc;
            vertexBR.TextureCoordinate.X = texCoordBR.x;
            vertexBR.TextureCoordinate.Y = texCoordBR.y;
        }
    }
}

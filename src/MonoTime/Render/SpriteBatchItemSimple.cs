// Decompiled with JetBrains decompiler
// Type: DuckGame.MTSimpleSpriteBatchItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    internal class MTSimpleSpriteBatchItem
    {
        public VertexPositionColor vertexTL;
        public VertexPositionColor vertexTR;
        public VertexPositionColor vertexBL;
        public VertexPositionColor vertexBR;

        public MTSimpleSpriteBatchItem()
        {
            vertexTL = new VertexPositionColor();
            vertexTR = new VertexPositionColor();
            vertexBL = new VertexPositionColor();
            vertexBR = new VertexPositionColor();
        }

        public void Set(float x, float y, float w, float h, Microsoft.Xna.Framework.Color color)
        {
            vertexTL.Position.X = x;
            vertexTL.Position.Y = y;
            vertexTL.Position.Z = 0f;
            vertexTL.Color = color;
            vertexTR.Position.X = x + w;
            vertexTR.Position.Y = y;
            vertexTR.Position.Z = 0f;
            vertexTR.Color = color;
            vertexBL.Position.X = x;
            vertexBL.Position.Y = y + h;
            vertexBL.Position.Z = 0f;
            vertexBL.Color = color;
            vertexBR.Position.X = x + w;
            vertexBR.Position.Y = y + h;
            vertexBR.Position.Z = 0f;
            vertexBR.Color = color;
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
          Microsoft.Xna.Framework.Color color,
          Vec2 texCoordTL,
          Vec2 texCoordBR)
        {
            vertexTL.Position.X = (float)(x + dx * cos - dy * sin);
            vertexTL.Position.Y = (float)(y + dx * sin + dy * cos);
            vertexTL.Position.Z = 0f;
            vertexTL.Color = color;
            vertexTR.Position.X = (float)(x + (dx + w) * cos - dy * sin);
            vertexTR.Position.Y = (float)(y + (dx + w) * sin + dy * cos);
            vertexTR.Position.Z = 0f;
            vertexTR.Color = color;
            vertexBL.Position.X = (float)(x + dx * cos - (dy + h) * sin);
            vertexBL.Position.Y = (float)(y + dx * sin + (dy + h) * cos);
            vertexBL.Position.Z = 0f;
            vertexBL.Color = color;
            vertexBR.Position.X = (float)(x + (dx + w) * cos - (dy + h) * sin);
            vertexBR.Position.Y = (float)(y + (dx + w) * sin + (dy + h) * cos);
            vertexBR.Position.Z = 0f;
            vertexBR.Color = color;
        }
    }
}

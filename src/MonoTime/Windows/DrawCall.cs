// Decompiled with JetBrains decompiler
// Type: DuckGame.DrawCall
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public struct DrawCall
    {
        public Tex2D texture;
        public Vec2 position;
        public Rectangle? sourceRect;
        public Color color;
        public float rotation;
        public Vec2 origin;
        public Vec2 scale;
        public SpriteEffects effects;
        public float depth;
        public Material material;
    }
}

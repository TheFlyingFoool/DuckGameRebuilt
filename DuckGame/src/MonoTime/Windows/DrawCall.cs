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

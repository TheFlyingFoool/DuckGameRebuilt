using DuckGame.AddedContent.Drake.PolyRender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame.AddedContent.Drake
{
    public static class DrakeTests
    {
        [DrawingContext(DrawingLayer.Game, DoDraw = false)]
        public static void PolyDrawTest()
        {
            Graphics.polyBatcher.Texture = (Texture2D)Profiles.active[0]?.duck?.graphic?.texture?.nativeObject;
            Graphics.polyBatcher.BlendState = BlendState.AlphaBlend;
            PolyRenderer.TexQuad(Vector2.One * 100, Vector2.One * 100 + Vector2.UnitX * 30, Vector2.One * 120,
                Vector2.One * 150 + Vector2.UnitX * 30);
        }
    }
}
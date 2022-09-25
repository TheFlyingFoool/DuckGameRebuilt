using System.Drawing;
using System.Drawing.Text;
using Microsoft.Xna.Framework;

namespace DuckGame.AddedContent.Drake.PolyRender
{
    public static class StaticFont
{

    public static AtlasedFont Font;
    
    static StaticFont()
    {
        PrivateFontCollection collection = new PrivateFontCollection();
        collection.AddFontFile("..\\MonoFont.ttf");
        FontFamily fontFamily = new FontFamily("Jetbrains Mono Medium", collection);
        Font = new AtlasedFont(fontFamily, 18, AtlasFontStyle.All);
    }

    public static void DrawString(string text, Vector2 pos, Color col, float scale = 1f, float margin = 0.5f) => Font?.DrawString(text, pos, col, scale, margin);
    public static void DrawRawString(string text, Vector2 pos, Color col, float scale = 1f, float margin = 0.5f) =>
        Font?.DrawRawString(text, pos, col, scale, margin);
    public static void DrawStringSized(string text, Vector2 pos, Color col, float cWidth, float margin = 0.5f) => Font?.DrawStringSized(text, pos, col, cWidth, margin);
}
}

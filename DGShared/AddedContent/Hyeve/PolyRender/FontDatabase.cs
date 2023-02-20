using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using AddedContent.Hyeve.Utils;
using Microsoft.Xna.Framework;
using Color = DuckGame.Color;

namespace AddedContent.Hyeve.PolyRender
{
    public static class FontDatabase
    {
        private static Dictionary<int, AtlasedFont> _fontSizes = new();

        private static FontFamily _font;
        
        static FontDatabase()
        {
            PrivateFontCollection collection = new();
            collection.AddFontFile("MonoFont.ttf");
            _font = new("Jetbrains Mono Medium", collection);
        }

        public static void GenerateFontSize(int size)
        {
            _fontSizes.Remove(size);
            _fontSizes.Add(size, new AtlasedFont(_font, size));
        }

        public static void DrawString(string text, Vector2 pos, Color col, int size, float margin = 0.6f)
        {
            if(!_fontSizes.ContainsKey(size)) GenerateFontSize(size);
            _fontSizes[size].DrawString(text, pos.Rounded(), col, 1f, margin);
        }

        public static void DrawRawString(string text, Vector2 pos, Color col, int size, float margin = 0.6f)
        {
            if(!_fontSizes.ContainsKey(size)) GenerateFontSize(size);
            _fontSizes[size].DrawRawString(text, pos.Rounded(), col,1f, margin);
        }

        public static AtlasedFont Get(int size)
        {
            if(!_fontSizes.ContainsKey(size)) GenerateFontSize(size);
            return _fontSizes[size];
        }
    }
}

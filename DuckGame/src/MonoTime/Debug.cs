using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace DuckGame
{
    public static class Debug
    {
        private static Texture2D _blank;

        [Conditional("DEBUG")]
        public static void Initialize()
        {
            _blank = new Texture2D(Graphics.device, 1, 1, false, SurfaceFormat.Color);
            _blank.SetData(new Color[1]
            {
        Color.White
            });
        }

        [Conditional("DEBUG")]
        public static void DrawLine(Vec2 p1, Vec2 p2, Color col, float width = 1f)
        {
            p1 = new Vec2((int)p1.x, (int)p1.y);
            p2 = new Vec2((int)p2.x, (int)p2.y);
            float rotation = (float)Math.Atan2(p2.y - p1.y, p2.x - p1.x);
            float length = (p1 - p2).length;
            Graphics.Draw((Tex2D)_blank, p1, new Rectangle?(), col, rotation, Vec2.Zero, new Vec2(length, width), SpriteEffects.None, (Depth)1f);
        }

        [Conditional("DEBUG")]
        public static void DrawRect(Vec2 p1, Vec2 p2, Color col) => Graphics.Draw((Tex2D)_blank, p1, new Rectangle?(), col, 0f, Vec2.Zero, new Vec2((float)-(p1.x - p2.x), (float)-(p1.y - p2.y)), SpriteEffects.None, (Depth)1f);

        [Conditional("DEBUG")]
        [Conditional("SWITCH")]
        public static void Assert(bool cond, string fmt, params object[] vals)
        {
            if (cond)
                return;
            DevConsole.Log(DCSection.General, Verbosity.Very, string.Format(fmt, vals));
            Debugger.Break();
        }
    }
}

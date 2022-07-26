// Decompiled with JetBrains decompiler
// Type: DuckGame.RhythmMode
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class RhythmMode
    {
        private static Sprite _bar;
        private static Sprite _ball;
        private static float _pos;
        private static float _soundPos;

        public static void Tick(float pos) => RhythmMode._pos = pos;

        public static void TickSound(float pos)
        {
            if ((double)pos < (double)RhythmMode._soundPos)
                SFX.Play("metronome");
            RhythmMode._soundPos = pos;
        }

        public static bool inTime => (double)RhythmMode._pos < 0.25 || (double)RhythmMode._pos > 0.75;

        public static void Draw()
        {
            if (RhythmMode._bar == null)
                RhythmMode._bar = new Sprite("rhythmBar");
            if (RhythmMode._ball == null)
            {
                RhythmMode._ball = new Sprite("rhythmBall");
                RhythmMode._ball.CenterOrigin();
            }
            Vec2 vec2 = new Vec2(Layer.HUD.camera.width / 2f - (float)(RhythmMode._bar.w / 2), 10f);
            Graphics.Draw(RhythmMode._bar, vec2.x, vec2.y);
            for (int index = 0; index < 5; ++index)
            {
                float x = (float)((double)vec2.x + 2.0 + (double)(index * (RhythmMode._bar.w / 4) + 1) + (double)RhythmMode._pos * ((double)RhythmMode._bar.w / 4.0));
                float num = Maths.Clamp((float)(((double)x - (double)vec2.x) / ((double)RhythmMode._bar.w - 2.0)), 0.0f, 1f);
                RhythmMode._ball.alpha = (float)((Math.Sin((double)num * (2.0 * Math.PI) - Math.PI / 2.0) + 1.0) / 2.0);
                if ((index == 1 && (double)RhythmMode._pos > 0.5 || index == 2 && (double)RhythmMode._pos <= 0.5) && RhythmMode.inTime)
                    RhythmMode._ball.scale = new Vec2(2f, 2f);
                else
                    RhythmMode._ball.scale = new Vec2(1f, 1f);
                Graphics.Draw(RhythmMode._ball, x, vec2.y + 4f);
            }
        }
    }
}

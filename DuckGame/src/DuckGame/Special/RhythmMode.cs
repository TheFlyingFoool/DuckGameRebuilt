// Decompiled with JetBrains decompiler
// Type: DuckGame.RhythmMode
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public static void Tick(float pos) => _pos = pos;

        public static void TickSound(float pos)
        {
            if (pos < _soundPos)
                SFX.Play("metronome");
            _soundPos = pos;
        }

        public static bool inTime => _pos < 0.25 || _pos > 0.75;

        public static void Draw()
        {
            if (_bar == null)
                _bar = new Sprite("rhythmBar");
            if (_ball == null)
            {
                _ball = new Sprite("rhythmBall");
                _ball.CenterOrigin();
            }
            Vec2 barPos = new Vec2(Layer.HUD.camera.width / 2f - _bar.w / 2, 10f);
            Graphics.Draw(ref _bar, barPos.x, barPos.y);
            for (int i = 0; i < 5; i++)
            {
                float xpos = ((barPos.x + 2 + ((i * (_bar.w / 4)) + 1))) + (_pos * (_bar.w / 4.0f));
                float distance = Maths.Clamp((xpos - barPos.x) / (_bar.w - 2.0f), 0.0f, 1.0f);
                _ball.alpha = ((float)Math.Sin((distance * (Math.PI * 2.0f)) - (Math.PI / 2.0f)) + 1.0f) / 2.0f;

                if (((i == 1 && _pos > 0.5f) || (i == 2 && _pos <= 0.5f)) && inTime)
                    _ball.scale = new Vec2(2, 2);
                else
                    _ball.scale = new Vec2(1, 1);

                Graphics.Draw(ref _ball, xpos, (barPos.y + 4));
            }
        }
    }
}

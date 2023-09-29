// Decompiled with JetBrains decompiler
// Type: DuckGame.HotnessAnimation
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class HotnessAnimation
    {
        private Sprite _redBar;
        private Sprite _blueBar;
        private SpriteMap _icon;
        private BitmapFont _font;
        private List<int> _sampleCool = new List<int>();
        private List<int> _cool = new List<int>();
        private List<int> _lastFrame = new List<int>();
        private List<float> _upScale = new List<float>();
        private List<string> _hotnessStrings = new List<string>()
    {
      "Absolute Zero",
      "Icy Moon",
      "Antarctica",
      "Ice Cube",
      "Ice Cream",
      "Coffee",
      "Fire",
      "A Volcanic Eruption",
      "The Sun"
    };
        private List<int> _tempMap = new List<int>()
    {
      -250,
      -200,
      -100,
      -40,
      -20,
      0,
      100,
      1200,
      4000,
      4500,
      5000
    };
        private bool _readyToTalk;
        private float _wait;

        public bool ready => _readyToTalk;

        public HotnessAnimation()
        {
            _redBar = new Sprite("newscast/redBar");
            _blueBar = new Sprite("newscast/blueBar");
            _icon = new SpriteMap("newscast/hotness", 18, 18);
            _icon.CenterOrigin();
            _font = new BitmapFont("biosFontDegree", 8);
            for (int index = 0; index < Profiles.active.Count; ++index)
            {
                if (Profiles.active.Count > index)
                {
                    int num = Profiles.active[index].endOfRoundStats.GetProfileScore();
                    if (num < 0)
                        num = 0;
                    _sampleCool.Add(num);
                }
                else
                    _sampleCool.Add(0);
                _cool.Add(-50);
                _lastFrame.Add(0);
                _upScale.Add(0f);
            }
        }

        public void Draw()
        {
            if (_wait > 1f)
            {
                bool flag = true;
                for (int index = 0; index < _cool.Count; ++index)
                {
                    if (_sampleCool[index] < _cool[index])
                    {
                        --_cool[index];
                        flag = false;
                    }
                    else if (_sampleCool[index] > _cool[index])
                    {
                        ++_cool[index];
                        flag = false;
                    }
                    if (_upScale[index] > 0f)
                        _upScale[index] -= 0.05f;
                }
                if (flag)
                {
                    _wait += 0.015f;
                    if (_wait > 2f) _readyToTalk = true;
                }
            }
            else _wait += 0.01f;
            _redBar.depth = (Depth)0.2f;
            Graphics.Draw(ref _redBar, 30f, 25f);
            _font.depth = (Depth)0.25f;
            if (DG.isHalloween)
                _font.Draw("SPOOKY  REPORT", 44f, 28f, Color.White, (Depth)0.25f);
            else
                _font.Draw("HOTNESS REPORT", 44f, 28f, Color.White, (Depth)0.25f);
            _blueBar.depth = (Depth)0.1f;
            Graphics.Draw(ref _blueBar, 30f, 18f);
            Graphics.DrawRect(new Vec2(20f, 135f), new Vec2(260f, 160f), new Color(12, 90, 182), (Depth)0.1f);
            Vec2 vec2_1 = new Vec2(60f, 50f);
            Vec2 vec2_2 = new Vec2(200f, 150f);
            Vec2 vec2_3 = new Vec2(vec2_2.x - vec2_1.x, vec2_2.y - vec2_1.y);
            List<Profile> active = Profiles.active;
            int index1 = 0;
            foreach (Profile profile in active)
            {
                float num1 = active.Count != 1 ? (active.Count != 2 ? index1 * (vec2_3.x / (active.Count - 1)) : (float)(vec2_3.x / 2f - vec2_3.x / 4f + index1 * (vec2_3.x / 2f))) : vec2_3.x / 2f;
                float num2 = (_cool[index1] + 50) / 250f;
                float num3 = 1f / (_tempMap.Count - 2);
                int index2 = (int)(num2 * (_tempMap.Count - 2));
                if (index2 < 0)
                    index2 = 0;
                int temp = _tempMap[index2];
                float num4 = Maths.NormalizeSection(num2, num3 * index2, num3 * (index2 + 1));
                int num5 = (int)(_tempMap[index2] + (_tempMap[index2 + 1] - _tempMap[index2]) * num4);
                float num6 = 50f;
                float num7 = num2 + 0.28f;
                float x = vec2_1.x + num1;
                float y = (float)(vec2_2.y - 32f - num7 * num6);
                profile.persona.sprite.depth = (Depth)0.3f;
                profile.persona.sprite.color = Color.White;
                Graphics.Draw(profile.persona.sprite, 0, x, y);
                Vec2 hatPoint = DuckRig.GetHatPoint(profile.persona.sprite.imageIndex);
                profile.team.hat.depth = (Depth)0.31f;
                profile.team.hat.center = new Vec2(16f, 16f) + profile.team.hatOffset;
                Graphics.Draw(profile.team.hat, profile.team.hat.frame, x + hatPoint.x, y + hatPoint.y);
                if (_cool.Count > 4)
                    Graphics.DrawRect(new Vec2(x - 9f, y + 16f), new Vec2(x + 9f, 160f), profile.persona.colorUsable, (Depth)0.05f);
                else
                    Graphics.DrawRect(new Vec2(x - 17f, y + 16f), new Vec2(x + 16f, 160f), profile.persona.colorUsable, (Depth)0.05f);
                string text = num5.ToString() + "=";
                _font.depth = (Depth)0.25f;
                if (_cool.Count > 4)
                    _font.scale = new Vec2(0.5f);
                _font.Draw(text, new Vec2((float)(x - _font.GetWidth(text) / 2f + 3f), 140f), Color.White, (Depth)0.25f);
                _font.scale = new Vec2(1f);
                _icon.depth = (Depth)0.3f;
                _icon.frame = (int)Math.Floor(num2 * 8.99f);
                if (_icon.frame != _lastFrame[index1])
                {
                    _lastFrame[index1] = _icon.frame;
                    _upScale[index1] = 0.5f;
                }
                _icon.scale = new Vec2(1f + _upScale[index1]);
                Graphics.Draw(ref _icon, x, y + 28f);
                ++index1;
            }
        }
    }
}

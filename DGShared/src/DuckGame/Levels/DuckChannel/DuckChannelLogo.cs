// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckChannelLogo
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class DuckChannelLogo : Thing
    {
        private Sprite _duck;
        private Sprite _channel;
        private Sprite _five;
        private float _duckLerp;
        private float _channelLerp;
        private float _fiveLerp;
        private List<float> _swipeLines = new List<float>();
        private List<float> _swipeSpeeds = new List<float>();
        private float _slideOutWait;
        private float _transitionWait;
        private bool _playSwipe;
        private bool _doTransition;

        public void PlaySwipe()
        {
            if (_playSwipe)
                return;
            _playSwipe = true;
            _duckLerp = 0f;
            _channelLerp = 0f;
            _slideOutWait = 0f;
            _fiveLerp = 0f;
            _doTransition = false;
            _transitionWait = 0f;
            for (int index = 0; index < 10; ++index)
            {
                _swipeLines[index] = Rando.Float(0.1f);
                _swipeSpeeds[index] = Rando.Float(0.01f, 0.012f);
            }
        }

        public bool doTransition => _doTransition;

        public DuckChannelLogo()
          : base()
        {
            _duck = new Sprite("newsTitleDuck");
            _channel = new Sprite("newsTitleChannel");
            _five = new Sprite("newsTitle5");
            layer = Layer.HUD;
            for (int index = 0; index < 10; ++index)
            {
                _swipeLines.Add(Rando.Float(0.1f));
                _swipeSpeeds.Add(Rando.Float(0.01f, 0.012f));
            }
        }

        public override void Update()
        {
            if (_playSwipe)
            {
                _transitionWait += 0.02f;
                if (_transitionWait > 1.0)
                    _doTransition = true;
                if (_slideOutWait < 1.0)
                {
                    _duckLerp = Lerp.FloatSmooth(_duckLerp, 1f, 0.1f, 1.1f);
                    _channelLerp = Lerp.FloatSmooth(_channelLerp, 1f, 0.1f, 1.1f);
                    _fiveLerp = Lerp.FloatSmooth(_fiveLerp, 1f, 0.1f, 1.1f);
                    _slideOutWait += 0.012f;
                }
                else
                {
                    _duckLerp = Lerp.FloatSmooth(_duckLerp, 0f, 0.1f, 1.1f);
                    _channelLerp = Lerp.FloatSmooth(_channelLerp, 0f, 0.1f, 1.1f);
                    _fiveLerp = Lerp.FloatSmooth(_fiveLerp, 0f, 0.1f, 1.1f);
                    if (_duckLerp < 0.01f)
                        _playSwipe = false;
                }
                for (int index = 0; index < _swipeLines.Count; ++index)
                    _swipeLines[index] = Lerp.Float(_swipeLines[index], 1f, _swipeSpeeds[index]);
            }
            else
                _doTransition = false;
        }

        public override void Draw()
        {
            Vec2 vec2_1 = new Vec2(10f, 12f);
            Vec2 vec2_2 = new Vec2((-200f * (1f - _duckLerp)), 0f);
            Vec2 vec2_3 = new Vec2((200f * (1f - _channelLerp)), 0f);
            Vec2 vec2_4 = new Vec2((300f * (1f - _channelLerp)), 0f);
            _duck.depth = (Depth)0.85f;
            Graphics.Draw(_duck, vec2_1.x + 80f + vec2_2.x, vec2_1.y + 60f + vec2_2.y);
            _channel.depth = (Depth)0.86f;
            Graphics.Draw(_channel, vec2_1.x + 64f + vec2_3.x, vec2_1.y + 74f + vec2_3.y);
            _five.depth = (Depth)0.85f;
            Graphics.Draw(_five, vec2_1.x + 144f + vec2_4.x, vec2_1.y + 64f + vec2_4.y);
            Vec2 vec2_5 = new Vec2(30f, 20f);
            float num1 = 500f;
            float num2 = 16f;
            float num3 = 600f;
            for (int index = 0; index < _swipeLines.Count; ++index)
            {
                float num4 = _swipeLines[index] * -1200f;
                Graphics.DrawRect(new Vec2(vec2_5.x + num3 + num4, vec2_5.y + index * num2), new Vec2(vec2_5.x + num1 + num3 + num4, vec2_5.y + index * num2 + num2), Color.Black, (Depth)0.83f);
            }
        }
    }
}

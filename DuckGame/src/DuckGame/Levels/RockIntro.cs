// Decompiled with JetBrains decompiler
// Type: DuckGame.RockIntro
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class RockIntro : Level, IHaveAVirtualTransition, IOnlyTransitionIn
    {
        private Sprite _bigDome;
        private Sprite _smallDome;
        private Sprite _smallPillar;
        private SpriteMap _domeBleachers;
        private VirtualBackground _virtualBackground;
        private Sprite _cornerWedge;
        private Sprite _intermissionText;
        private float _intermissionSlide;
        private Level _next;
        private Layer _subHUD;
        private float _panWait = 1f;
        private float _yScrollVel;
        private float _afterDownWait = 1f;
        private float rotter;
        private float _yScroll = 1f;

        public override string networkIdentifier => "@ROCKINTRO";

        public RockIntro(Level next) => _next = next;

        private bool ready => true;

        public override void Initialize()
        {
            _bigDome = new Sprite("dome");
            _bigDome.CenterOrigin();
            _smallDome = new Sprite("domeSmall");
            _smallDome.CenterOrigin();
            _smallPillar = new Sprite("domePillar");
            _smallPillar.center = new Vec2(_smallPillar.w / 2, 0f);
            _domeBleachers = new SpriteMap("domeBleachers", 25, 20)
            {
                center = new Vec2(13f, 13f)
            };
            _virtualBackground = new VirtualBackground(0f, 0f, null);
            Add(_virtualBackground);
            _cornerWedge = new Sprite("rockThrow/cornerWedge");
            _intermissionText = new Sprite("rockThrow/intermission");
            _subHUD = new Layer("SUBHUD", -85)
            {
                allowTallAspect = true
            };
            Layer.Add(_subHUD);
            Layer.Foreground.camera = new Camera(0f, 0f, 320f, 320f / Resolution.current.aspect);
        }

        public override void Update()
        {
            Music.volume = Lerp.Float(Music.volume, 0f, 0.008f);
            if (Music.volume <= 0.0)
                Music.Stop();
            _panWait -= 0.04f;
            if (_panWait >= 0.0)
                return;
            _yScrollVel += _yScroll < 0.4f ? -0.0001f : 0.0008f;
            if (_yScrollVel > 0.01f)
                _yScrollVel = 0.01f;
            if (_yScrollVel < 0.0)
                _yScrollVel = 0f;
            _yScroll -= _yScrollVel;
            _virtualBackground.layer.fade = Lerp.Float(_virtualBackground.layer.fade, 0.5f, 0.01f);
            if (_yScroll >= 0.04f)
                return;
            _afterDownWait -= 0.05f;
            if (_afterDownWait >= 0.0)
                return;
            _intermissionSlide = Lerp.FloatSmooth(_intermissionSlide, 1f, 0.1f, 1.1f);
            _subHUD.fade -= 0.02f;
            if (_subHUD.fade < 0.0)
                _subHUD.fade = 0f;
            _virtualBackground.layer.fade -= 0.02f;
            if (_virtualBackground.layer.fade < 0.0)
                _virtualBackground.layer.fade = 0f;
            if (!Network.isServer || _subHUD.fade > 0.0 || _intermissionSlide < 0.99f || !ready)
                return;
            Music.volume = 1f;
            current = new RockScoreboard(_next);
        }

        public override void PostDrawLayer(Layer l)
        {
            if (l == _subHUD)
            {
                float num1 = 160f;
                float num2 = _yScroll * num1;
                _virtualBackground.parallax.y = (float)(-num1 * (1.0 - num2 / num1));
                _bigDome.depth = (Depth)0.5f;
                Graphics.Draw(_bigDome, 160f, 130f + num2);
                float deg = 45f;
                float rad1 = Maths.DegToRad(deg);
                float rad2 = Maths.DegToRad(25f + rotter);
                rotter -= 0.3f;
                if (rotter <= -deg)
                    rotter += deg;
                for (int index = 0; index < 8; ++index)
                {
                    if (index == 0 || index > 4)
                        _smallDome.depth = (Depth)0.6f;
                    else
                        _smallDome.depth = (Depth)0.4f;
                    Vec2 vec2_1 = new Vec2((float)Math.Cos(rad2 + index * rad1), (float)(-Math.Sin(rad2 + index * rad1) * (0.4f * (1.0 - num2 / num1))));
                    Vec2 vec2_2 = new Vec2(160f, 130f + num2) + vec2_1 * 100f;
                    Graphics.Draw(_smallDome, vec2_2.x, vec2_2.y - 30f);
                    _smallPillar.depth = _smallDome.depth;
                    Graphics.Draw(_smallPillar, vec2_2.x, vec2_2.y - 11f);
                    _domeBleachers.depth = _smallDome.depth + 1;
                    _domeBleachers.frame = 7 - (index + 5) % 8;
                    Graphics.Draw(_domeBleachers, vec2_2.x, vec2_2.y - 30f);
                }
            }
            else if (l == Layer.HUD)
            {
                _cornerWedge.flipH = false;
                _cornerWedge.depth = (Depth)0.7f;
                if (_intermissionSlide > 0.01f)
                {
                    float x1 = (float)(_intermissionSlide * 320.0 - 320.0);
                    float y = 60f;
                    Graphics.DrawRect(new Vec2(x1, y), new Vec2(x1 + 320f, y + 30f), Color.Black, (Depth)0.9f);
                    float x2 = (float)(320.0 - _intermissionSlide * 320.0);
                    float num = 60f;
                    Graphics.DrawRect(new Vec2(x2, num + 30f), new Vec2(x2 + 320f, num + 60f), Color.Black, (Depth)0.9f);
                    Graphics.Draw(_intermissionText, (float)(_intermissionSlide * 336.0 - 320.0), num + 18f);
                    _intermissionText.depth = (Depth)0.91f;
                }
            }
            base.PostDrawLayer(l);
        }

        public override void OnMessage(NetMessage message)
        {
            if (!(message is NMScoresReceived))
                return;
            foreach (Profile profile in DuckNetwork.GetProfiles(message.connection))
                profile.ready = true;
        }
    }
}

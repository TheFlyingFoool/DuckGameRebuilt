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
            if (Music.volume <= 0f)
                Music.Stop();
            _panWait -= 0.04f;
            if (_panWait >= 0f)
                return;
            _yScrollVel += _yScroll < 0.4f ? -0.0001f : 0.0008f;
            if (_yScrollVel > 0.01f)
                _yScrollVel = 0.01f;
            if (_yScrollVel < 0f)
                _yScrollVel = 0f;
            _yScroll -= _yScrollVel;
            _virtualBackground.layer.fade = Lerp.Float(_virtualBackground.layer.fade, 0.5f, 0.01f);
            if (_yScroll >= 0.04f)
                return;
            _afterDownWait -= 0.05f;
            if (_afterDownWait >= 0.0f)
                return;
            _intermissionSlide = Lerp.FloatSmooth(_intermissionSlide, 1f, 0.1f, 1.1f);
            _subHUD.fade -= 0.02f;
            if (_subHUD.fade < 0f)
                _subHUD.fade = 0f;
            _virtualBackground.layer.fade -= 0.02f;
            if (_virtualBackground.layer.fade < 0f)
                _virtualBackground.layer.fade = 0f;
            if (!Network.isServer || _subHUD.fade > 0f || _intermissionSlide < 0.99f || !ready)
                return;
            Music.volume = 1f;
            current = new RockScoreboard(_next);
        }

        public override void PostDrawLayer(Layer l)
        {
            if (l == _subHUD)
            {
                float maxYPos = 160f;
                float ypos = _yScroll * maxYPos;
                _virtualBackground.parallax.y = ypos - maxYPos;
                _bigDome.depth = (Depth)0.5f;
                Graphics.Draw(_bigDome, 160f, 130f + ypos);
                float deg = 45f;
                float rad1 = Maths.DegToRad(deg);
                float rad2 = Maths.DegToRad(25f + rotter);
                if (MonoMain.UpdateLerpState)
                {
                    rotter -= 0.3f;
                    if (rotter <= -deg)
                        rotter += deg;
                }
                for (int index = 0; index < 8; ++index)
                {
                    if (index == 0 || index > 4)
                        _smallDome.depth = (Depth)0.6f;
                    else
                        _smallDome.depth = (Depth)0.4f;
                    Vec2 pos = new Vec2((float)Math.Cos(rad2 + index * rad1), (float)(-Math.Sin(rad2 + index * rad1) * (0.4f * (1f - ypos / maxYPos))));
                    Vec2 drawPos = new Vec2(160f, 130f + ypos) + pos * 100f;
                    Graphics.Draw(_smallDome, drawPos.x, drawPos.y - 30f);
                    _smallPillar.depth = _smallDome.depth;
                    Graphics.Draw(_smallPillar, drawPos.x, drawPos.y - 11f);
                    _domeBleachers.depth = _smallDome.depth + 1;
                    _domeBleachers.frame = 7 - (index + 5) % 8;
                    Graphics.Draw(_domeBleachers, drawPos.x, drawPos.y - 30f);
                }
            }
            else if (l == Layer.HUD)
            {
                _cornerWedge.flipH = false;
                _cornerWedge.depth = (Depth)0.7f;
                if (_intermissionSlide > 0.01f)
                {
                    float x = _intermissionSlide * 320f - 320f;
                    float y = 60f;
                    Graphics.DrawRect(new Vec2(x, y), new Vec2(x + 320f, y + 30f), Color.Black, (Depth)0.9f);
                    x = 320f - _intermissionSlide * 320f;
                    y = 60f;
                    Graphics.DrawRect(new Vec2(x, y + 30f), new Vec2(x + 320f, y + 60f), Color.Black, (Depth)0.9f);
                    Graphics.Draw(_intermissionText, _intermissionSlide * 336f - 320f, y + 18f);
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

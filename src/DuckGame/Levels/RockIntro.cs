// Decompiled with JetBrains decompiler
// Type: DuckGame.RockIntro
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public RockIntro(Level next) => this._next = next;

        private bool ready => true;

        public override void Initialize()
        {
            this._bigDome = new Sprite("dome");
            this._bigDome.CenterOrigin();
            this._smallDome = new Sprite("domeSmall");
            this._smallDome.CenterOrigin();
            this._smallPillar = new Sprite("domePillar");
            this._smallPillar.center = new Vec2(this._smallPillar.w / 2, 0.0f);
            this._domeBleachers = new SpriteMap("domeBleachers", 25, 20)
            {
                center = new Vec2(13f, 13f)
            };
            this._virtualBackground = new VirtualBackground(0.0f, 0.0f, null);
            Level.Add(_virtualBackground);
            this._cornerWedge = new Sprite("rockThrow/cornerWedge");
            this._intermissionText = new Sprite("rockThrow/intermission");
            this._subHUD = new Layer("SUBHUD", -85)
            {
                allowTallAspect = true
            };
            Layer.Add(this._subHUD);
            Layer.Foreground.camera = new Camera(0.0f, 0.0f, 320f, 320f / Resolution.current.aspect);
        }

        public override void Update()
        {
            Music.volume = Lerp.Float(Music.volume, 0.0f, 0.008f);
            if ((double)Music.volume <= 0.0)
                Music.Stop();
            this._panWait -= 0.04f;
            if (_panWait >= 0.0)
                return;
            this._yScrollVel += _yScroll < 0.400000005960464 ? -0.0001f : 0.0008f;
            if (_yScrollVel > 0.00999999977648258)
                this._yScrollVel = 0.01f;
            if (_yScrollVel < 0.0)
                this._yScrollVel = 0.0f;
            this._yScroll -= this._yScrollVel;
            this._virtualBackground.layer.fade = Lerp.Float(this._virtualBackground.layer.fade, 0.5f, 0.01f);
            if (_yScroll >= 0.400000005960464)
                return;
            this._afterDownWait -= 0.05f;
            if (_afterDownWait >= 0.0)
                return;
            this._intermissionSlide = Lerp.FloatSmooth(this._intermissionSlide, 1f, 0.1f, 1.1f);
            this._subHUD.fade -= 0.02f;
            if ((double)this._subHUD.fade < 0.0)
                this._subHUD.fade = 0.0f;
            this._virtualBackground.layer.fade -= 0.02f;
            if ((double)this._virtualBackground.layer.fade < 0.0)
                this._virtualBackground.layer.fade = 0.0f;
            if (!Network.isServer || (double)this._subHUD.fade > 0.0 || _intermissionSlide < 0.990000009536743 || !this.ready)
                return;
            Music.volume = 1f;
            Level.current = new RockScoreboard(this._next);
        }

        public override void PostDrawLayer(Layer l)
        {
            if (l == this._subHUD)
            {
                float num1 = 160f;
                float num2 = this._yScroll * num1;
                this._virtualBackground.parallax.y = (float)(-(double)num1 * (1.0 - (double)num2 / (double)num1));
                this._bigDome.depth = (Depth)0.5f;
                Graphics.Draw(this._bigDome, 160f, 130f + num2);
                float deg = 45f;
                float rad1 = Maths.DegToRad(deg);
                float rad2 = Maths.DegToRad(25f + this.rotter);
                this.rotter -= 0.3f;
                if (rotter <= -(double)deg)
                    this.rotter += deg;
                for (int index = 0; index < 8; ++index)
                {
                    if (index == 0 || index > 4)
                        this._smallDome.depth = (Depth)0.6f;
                    else
                        this._smallDome.depth = (Depth)0.4f;
                    Vec2 vec2_1 = new Vec2((float)Math.Cos((double)rad2 + index * (double)rad1), (float)(-Math.Sin((double)rad2 + index * (double)rad1) * (0.400000005960464 * (1.0 - (double)num2 / (double)num1))));
                    Vec2 vec2_2 = new Vec2(160f, 130f + num2) + vec2_1 * 100f;
                    Graphics.Draw(this._smallDome, vec2_2.x, vec2_2.y - 30f);
                    this._smallPillar.depth = this._smallDome.depth;
                    Graphics.Draw(this._smallPillar, vec2_2.x, vec2_2.y - 11f);
                    this._domeBleachers.depth = this._smallDome.depth + 1;
                    this._domeBleachers.frame = 7 - (index + 5) % 8;
                    Graphics.Draw(_domeBleachers, vec2_2.x, vec2_2.y - 30f);
                }
            }
            else if (l == Layer.HUD)
            {
                this._cornerWedge.flipH = false;
                this._cornerWedge.depth = (Depth)0.7f;
                if (_intermissionSlide > 0.00999999977648258)
                {
                    float x1 = (float)(_intermissionSlide * 320.0 - 320.0);
                    float y = 60f;
                    Graphics.DrawRect(new Vec2(x1, y), new Vec2(x1 + 320f, y + 30f), Color.Black, (Depth)0.9f);
                    float x2 = (float)(320.0 - _intermissionSlide * 320.0);
                    float num = 60f;
                    Graphics.DrawRect(new Vec2(x2, num + 30f), new Vec2(x2 + 320f, num + 60f), Color.Black, (Depth)0.9f);
                    Graphics.Draw(this._intermissionText, (float)(_intermissionSlide * 336.0 - 320.0), num + 18f);
                    this._intermissionText.depth = (Depth)0.91f;
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

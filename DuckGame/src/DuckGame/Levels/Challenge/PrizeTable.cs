// Decompiled with JetBrains decompiler
// Type: DuckGame.PrizeTable
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using static DuckGame.CMD;

namespace DuckGame
{
    [EditorGroup("Special|Arcade", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class PrizeTable : Thing
    {
        private SpriteMap _sprite;
        private Sprite _outline;
        private float _hoverFade;
        private SpriteMap _light;
        private Sprite _fixture;
        private Sprite _prizes;
        private Sprite _hoverSprite;
        public bool hoverChancyChallenge;
        private DustSparkleEffect _dust;
        public bool hover;
        public bool _unlocked = true;
        private ArcadeTableLight _lighting;
        private bool _hasEligibleChallenges;

        public override bool visible
        {
            get => base.visible;
            set
            {
                base.visible = value;
                _dust.visible = base.visible;
            }
        }

        public PrizeTable(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("arcade/prizeCounter", 69, 30);
            graphic = _sprite;
            depth = -0.5f;
            _outline = new Sprite("arcade/prizeCounterOutline")
            {
                depth = depth + 1
            };
            _outline.CenterOrigin();
            center = new Vec2(_sprite.width / 2, _sprite.h / 2);
            _collisionSize = new Vec2(16f, 15f);
            _collisionOffset = new Vec2(-8f, 0f);
            _light = new SpriteMap("arcade/prizeLights", 107, 55);
            _fixture = new Sprite("arcade/bigFixture");
            _prizes = new Sprite("arcade/prizes");
            _hoverSprite = new Sprite("arcade/chancyHover");
            hugWalls = WallHug.Floor;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _dust = new DustSparkleEffect(x - 54f, y - 40f, true, true);
            Level.Add(_dust);
            _dust.depth = depth - 2;
            _lighting = new ArcadeTableLight(x, y - 43f);
            Level.Add(_lighting);
        }

        public override void Update()
        {
            Profile activeprofile = null;
            foreach(Profile profile in Profiles.active)
            {
                activeprofile = profile;
                break;
            }
            if (activeprofile == null)
                return;
            _hasEligibleChallenges = Challenges.GetEligibleChancyChallenges(activeprofile).Count > 0;
            Duck duck1 = Level.Nearest<Duck>(x, y);
            if (duck1 != null)
            {
                if (duck1.grounded && (duck1.position - position).length < 20f)
                {
                    _hoverFade = Lerp.Float(_hoverFade, 1f, 0.1f);
                    hover = true;
                }
                else
                {
                    _hoverFade = Lerp.Float(_hoverFade, 0f, 0.1f);
                    hover = false;
                }
            }
            if (_hasEligibleChallenges)
            {
                Vec2 vec2 = new Vec2(40f, 0f);
                Duck duck2 = Level.Nearest<Duck>(x + vec2.x, y + vec2.y);
                if (duck2 != null) 
                    hoverChancyChallenge = duck2.grounded && (duck2.position - (position + vec2)).length < 20f;
            }
            if (_dust != null)
            {
                _dust.fade = 0.5f;
                _dust.visible = _unlocked && visible;
            }
        }

        public override void Draw()
        {
            _light.depth = depth - 9;
            _prizes.depth = depth - 7;
            Graphics.Draw(_prizes, x - 28f, y - 33f);
            if (_unlocked)
                graphic.color = Color.White;
            else
                graphic.color = Color.Black;
            Graphics.Draw(_light, x - 53f, y - 40f);
            if (Chancy.atCounter && !(Level.current is Editor))
            {
                Vec2 vec2 = new Vec2(32f, -15f);
                if (_hasEligibleChallenges)
                {
                    vec2 = new Vec2(42f, -10f);
                    Chancy.body.flipH = false;
                }
                else
                    Chancy.body.flipH = true;

                Chancy.body.depth = depth - 6;
                Graphics.Draw(Chancy.body, x + vec2.x, y + vec2.y);
                if (hoverChancyChallenge)
                    _hoverSprite.alpha = Lerp.Float(_hoverSprite.alpha, 1f, 0.05f);
                else
                    _hoverSprite.alpha = Lerp.Float(_hoverSprite.alpha, 0f, 0.05f);
                if (_hoverSprite.alpha > 0.01f)
                {
                    _hoverSprite.depth = (Depth)0f;
                    _hoverSprite.flipH = Chancy.body.flipH;
                    if (_hoverSprite.flipH)
                        Graphics.Draw(_hoverSprite, (x + vec2.x + 1f), (y + vec2.y - 1f));
                    else
                        Graphics.Draw(_hoverSprite, (x + vec2.x - 1f), (y + vec2.y - 1f));
                }
            }
            base.Draw();
            if (_hoverFade <= 0f)
                return;
            _outline.alpha = _hoverFade;
            Graphics.Draw(_outline, x + 1f, y);
        }
    }
}

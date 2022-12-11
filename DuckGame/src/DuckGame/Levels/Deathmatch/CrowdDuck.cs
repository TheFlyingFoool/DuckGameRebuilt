// Decompiled with JetBrains decompiler
// Type: DuckGame.CrowdDuck
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Linq;

namespace DuckGame
{
    public class CrowdDuck : Thing
    {
        private Mood _mood = Mood.Calm;
        private SpriteMap _sprite;
        private Sprite _letterSign;
        private Sprite _loveSign;
        private Sprite _sucksSign;
        private Sprite _suckSign;
        private bool _empty;
        private string _letter;
        private int _letterNumber;
        private float _letterSway = Rando.Float(2f);
        private BitmapFont _font;
        private Profile _lastLoyalty;
        private Profile _loyalty;
        public bool loyal;
        public bool newLoyal;
        private bool _busy;
        private bool _hate;
        private Profile _signProfile;
        private float _hatThrowTime = -1f;
        public int distVal;
        public int duckColor;
        private SpriteMap _originalSprite;

        public bool empty => _empty;

        public string letter
        {
            get => _letter;
            set => _letter = value;
        }

        public Profile lastLoyalty => _lastLoyalty;

        public Profile loyalty
        {
            get => _loyalty;
            set => _loyalty = value;
        }

        public bool busy
        {
            get => _busy;
            set => _busy = value;
        }

        public void ClearActions()
        {
            _busy = false;
            _hate = false;
            _letterNumber = 0;
            _letter = null;
            _lastLoyalty = _loyalty;
        }

        public void SetLetter(string l, int num, bool hate = false, Profile p = null)
        {
            _letter = l;
            _letterNumber = num;
            _busy = _letter != null;
            _hate = hate;
            _signProfile = p;
        }

        public void TryChangingAllegiance(Profile to, float awesomeness)
        {
            if (awesomeness > 0.1f && Rando.Float(1f) < awesomeness)
            {
                if (loyalty != null)
                {
                    if (loyalty != to)
                    {
                        if (!loyalty.stats.TryFanTransfer(to, awesomeness, loyal))
                            return;
                        if (loyal)
                            newLoyal = false;
                        else
                            loyalty = to;
                    }
                    else
                    {
                        if (awesomeness <= 0.15f || Rando.Float(1.1f) >= awesomeness)
                            return;
                        to.stats.MakeFanLoyal();
                        newLoyal = true;
                    }
                }
                else
                {
                    loyalty = to;
                    ++to.stats.unloyalFans;
                }
            }
            else
            {
                if (awesomeness >= -0.1f || Rando.Float(1f) >= Math.Abs(awesomeness) || loyalty != to || !loyalty.stats.FanConsidersLeaving(awesomeness, loyal))
                    return;
                if (loyal)
                    newLoyal = false;
                else
                    loyalty = null;
            }
        }

        public void ThrowHat(Profile p)
        {
            if (_lastLoyalty != _loyalty && (_lastLoyalty == p && _loyalty == null || _loyalty == p))
                _hatThrowTime = Rando.Float(0.2f, 1f);
            if (_loyalty != p)
                return;
            loyal = newLoyal;
        }

        public CrowdDuck(
          float xpos,
          float ypos,
          float zpos,
          int facing,
          int row,
          int dist,
          int empty = -1,
          Profile varLoyalty = null,
          Profile varLastLoyalty = null,
          bool varLoyal = false,
          int varColor = -1)
          : base(xpos, ypos)
        {
            shouldbegraphicculled = false;
            distVal = dist;
            z = zpos;
            int totalFans = Crowd.totalFans;
            int _max = 3;
            if (dist <= 20)
                _max += (int)(totalFans * 0.08f);
            if (dist > 20)
                _max = 2 + (int)(totalFans * 0.02f);
            if (dist > 30)
                _max = 1 + (int)(totalFans * 0.01f);
            if (Crowd.totalFans < 1)
                _max = 0;
            ++Crowd.fansUsed;
            Rando.Int(1);
            duckColor = varColor > -1 ? varColor : Rando.Int(3);
            _originalSprite = Persona.all.ElementAt<DuckPersona>(duckColor).crowdSprite;
            SpriteMap spriteMap = _originalSprite.CloneMap();
            if (empty == 0 || empty == -1 && Rando.Int(_max) < 1)
            {
                spriteMap.AddAnimation("idle", Rando.Float(0.05f, 0.1f), true, 9);
                _empty = true;
            }
            else
            {
                if (empty == -1)
                {
                    FanNum fan = Crowd.GetFan();
                    Profile profile = null;
                    if (fan != null)
                    {
                        profile = fan.profile;
                        if (fan.loyalFans > 0)
                            newLoyal = loyal = true;
                    }
                    _loyalty = _lastLoyalty = profile;
                }
                else
                {
                    _loyalty = varLoyalty;
                    _lastLoyalty = varLastLoyalty;
                    loyal = newLoyal = varLoyal;
                }
                switch (facing)
                {
                    case 0:
                        spriteMap.AddAnimation("idle", Rando.Float(0.05f, 0.1f), true, new int[1]);
                        spriteMap.AddAnimation("cheer", Rando.Float(0.05f, 0.1f), true, 0, 1);
                        spriteMap.AddAnimation("scream", Rando.Float(0.05f, 0.1f), true, 0, 2);
                        break;
                    case 1:
                        spriteMap.AddAnimation("idle", Rando.Float(0.05f, 0.1f), true, 3);
                        spriteMap.AddAnimation("cheer", Rando.Float(0.05f, 0.1f), true, 3, 4);
                        spriteMap.AddAnimation("scream", Rando.Float(0.05f, 0.1f), true, 3, 5);
                        break;
                    default:
                        spriteMap.AddAnimation("idle", Rando.Float(0.05f, 0.1f), true, 6);
                        spriteMap.AddAnimation("cheer", Rando.Float(0.05f, 0.1f), true, 6, 7);
                        spriteMap.AddAnimation("scream", Rando.Float(0.05f, 0.1f), true, 6, 8);
                        break;
                }
            }
            spriteMap.SetAnimation("idle");
            _sprite = spriteMap;
            graphic = _sprite;
            collisionSize = new Vec2(_sprite.width, _sprite.height);
            collisionOffset = new Vec2(-(_sprite.w / 2), -(_sprite.h / 2));
            center = new Vec2(0f, spriteMap.h);
            collisionOffset = new Vec2(collisionOffset.x, -_sprite.h);
            depth = (Depth)(0.3f - row * 0.05f);
            layer = Layer.Background;
            _letterSign = new Sprite("letterSign");
            _letterSign.CenterOrigin();
            _letterSign.depth = depth + 2;
            _font = new BitmapFont("biosFont", 8);
            _loveSign = new Sprite("loveSign");
            _loveSign.CenterOrigin();
            _loveSign.depth = (Depth)(0.32f - row * 0.05f);
            _sucksSign = new Sprite("sucksSign");
            _sucksSign.CenterOrigin();
            _sucksSign.depth = (Depth)(0.32f - row * 0.05f);
            _suckSign = new Sprite("suckSign");
            _suckSign.CenterOrigin();
            _suckSign.depth = (Depth)(0.32f - row * 0.05f);
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (_empty)
                return;
            if (_mood != Crowd.mood)
                _mood = Crowd.mood;
            if (_mood == Mood.Calm || _mood == Mood.Silent || _mood == Mood.Dead || _mood == Mood.Excited)
                _sprite.SetAnimation("cheer");
            else if (_mood == Mood.Extatic)
                _sprite.SetAnimation("scream");
            if (_hatThrowTime > 0f)
            {
                _hatThrowTime -= 0.01f;
            }
            else
            {
                if (_hatThrowTime <= -0.5f)
                    return;
                _lastLoyalty = _loyalty;
                if (_lastLoyalty == null)
                    SFX.Play("cutOffQuack2", 0.9f, Rando.Float(-0.1f, 0.1f));
                else
                    SFX.Play("cutOffQuack", 0.9f, Rando.Float(-0.1f, 0.1f));
                if (DGRSettings.S_ParticleMultiplier != 0) Level.Add(SmallSmoke.New(x + 6f, y - 35f));
                _hatThrowTime = -1f;
            }
        }

        public override void Draw()
        {
            if (_sprite == null || _originalSprite == null)
                return;
            _sprite.texture = _originalSprite.texture;
            if (!_empty && _letter != null)
            {
                float num = (float)(Math.Sin(_letterSway + _letterNumber * 0.1f) * 2f + 4f);
                _letterSway += 0.1f;
                if (_letter.Length == 1)
                {
                    if ((_signProfile == null || _signProfile == loyalty) && _letter != " ")
                    {
                        _letterSign.depth = depth + 5;
                        Graphics.Draw(_letterSign, x + 10f, y - 24f + num);
                        _font.Draw(_letter, x + 6f, y - 28f + num, Color.Gray, depth + 9);
                    }
                }
                else if (_hate)
                {
                    if (_letter[_letter.Length - 1] == 'S')
                    {
                        Graphics.Draw(_suckSign, x + 28f, y - 27f + num);
                        _font.Draw(_letter, (float)(x - _font.GetWidth(_letter) / 2.0 + 28.0), (float)(y - 26.0 - 8.0) + num, Color.Gray, _suckSign.depth + 3);
                    }
                    else
                    {
                        Graphics.Draw(_sucksSign, x + 28f, y - 27f + num);
                        _font.Draw(_letter, (float)(x - _font.GetWidth(_letter) / 2.0 + 28.0), (float)(y - 26.0 - 8.0) + num, Color.Gray, _sucksSign.depth + 3);
                    }
                }
                else
                {
                    Graphics.Draw(_loveSign, x + 28f, y - 27f + num);
                    _font.Draw(_letter, (float)(x - _font.GetWidth(_letter) / 2.0 + 29.0), y - 26f + num, Color.Gray, _loveSign.depth + 3);
                }
            }
            if (!_empty && _lastLoyalty != null && _lastLoyalty.persona != null && _lastLoyalty.team != null)
            {
                SpriteMap g = _lastLoyalty.persona.defaultHead;
                Vec2 vec2 = Vec2.Zero;
                if (_lastLoyalty.team.hasHat)
                {
                    vec2 = _lastLoyalty.team.hatOffset;
                    g = _lastLoyalty.team.GetHat(_lastLoyalty.persona);
                }
                if (g == null)
                    return;
                g.depth = depth + 2;
                g.angle = 0f;
                g.alpha = 1f;
                g.color = Color.White;
                bool flag = _sprite.imageIndex == 1 || _sprite.imageIndex == 2 || _sprite.imageIndex == 4 || _sprite.imageIndex == 5 || _sprite.imageIndex == 7 || _sprite.imageIndex == 8;
                float num = 0f;
                if (_sprite.imageIndex > 2)
                {
                    g.flipH = true;
                    num += 5f;
                    if (flag)
                        --num;
                }
                else
                {
                    g.flipH = false;
                    if (flag)
                        ++num;
                }
                if (g.flipH)
                    vec2.x = -vec2.x;
                if (loyal)
                    g.frame = flag ? 1 : 0;
                g.CenterOrigin();
                Graphics.Draw(g, (float)(x - vec2.x + 8.0) + num, (float)(y - vec2.y - 22.0 - (flag ? 1.0 : 0.0)));
                g.frame = 0;
                g.flipH = false;
            }
            base.Draw();
        }
    }
}

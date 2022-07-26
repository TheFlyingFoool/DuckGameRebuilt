// Decompiled with JetBrains decompiler
// Type: DuckGame.CrowdDuck
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public bool empty => this._empty;

        public string letter
        {
            get => this._letter;
            set => this._letter = value;
        }

        public Profile lastLoyalty => this._lastLoyalty;

        public Profile loyalty
        {
            get => this._loyalty;
            set => this._loyalty = value;
        }

        public bool busy
        {
            get => this._busy;
            set => this._busy = value;
        }

        public void ClearActions()
        {
            this._busy = false;
            this._hate = false;
            this._letterNumber = 0;
            this._letter = (string)null;
            this._lastLoyalty = this._loyalty;
        }

        public void SetLetter(string l, int num, bool hate = false, Profile p = null)
        {
            this._letter = l;
            this._letterNumber = num;
            this._busy = this._letter != null;
            this._hate = hate;
            this._signProfile = p;
        }

        public void TryChangingAllegiance(Profile to, float awesomeness)
        {
            if ((double)awesomeness > 0.100000001490116 && (double)Rando.Float(1f) < (double)awesomeness)
            {
                if (this.loyalty != null)
                {
                    if (this.loyalty != to)
                    {
                        if (!this.loyalty.stats.TryFanTransfer(to, awesomeness, this.loyal))
                            return;
                        if (this.loyal)
                            this.newLoyal = false;
                        else
                            this.loyalty = to;
                    }
                    else
                    {
                        if ((double)awesomeness <= 0.150000005960464 || (double)Rando.Float(1.1f) >= (double)awesomeness)
                            return;
                        to.stats.MakeFanLoyal();
                        this.newLoyal = true;
                    }
                }
                else
                {
                    this.loyalty = to;
                    ++to.stats.unloyalFans;
                }
            }
            else
            {
                if ((double)awesomeness >= -0.100000001490116 || (double)Rando.Float(1f) >= (double)Math.Abs(awesomeness) || this.loyalty != to || !this.loyalty.stats.FanConsidersLeaving(awesomeness, this.loyal))
                    return;
                if (this.loyal)
                    this.newLoyal = false;
                else
                    this.loyalty = (Profile)null;
            }
        }

        public void ThrowHat(Profile p)
        {
            if (this._lastLoyalty != this._loyalty && (this._lastLoyalty == p && this._loyalty == null || this._loyalty == p))
                this._hatThrowTime = Rando.Float(0.2f, 1f);
            if (this._loyalty != p)
                return;
            this.loyal = this.newLoyal;
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
            this.distVal = dist;
            this.z = zpos;
            int totalFans = Crowd.totalFans;
            int _max = 3;
            if (dist <= 20)
                _max += (int)((double)totalFans * 0.0799999982118607);
            if (dist > 20)
                _max = 2 + (int)((double)totalFans * 0.0199999995529652);
            if (dist > 30)
                _max = 1 + (int)((double)totalFans * 0.00999999977648258);
            if (Crowd.totalFans < 1)
                _max = 0;
            ++Crowd.fansUsed;
            Rando.Int(1);
            this.duckColor = varColor > -1 ? varColor : Rando.Int(3);
            this._originalSprite = Persona.all.ElementAt<DuckPersona>(this.duckColor).crowdSprite;
            SpriteMap spriteMap = this._originalSprite.CloneMap();
            if (empty == 0 || empty == -1 && Rando.Int(_max) < 1)
            {
                spriteMap.AddAnimation("idle", Rando.Float(0.05f, 0.1f), true, 9);
                this._empty = true;
            }
            else
            {
                if (empty == -1)
                {
                    FanNum fan = Crowd.GetFan();
                    Profile profile = (Profile)null;
                    if (fan != null)
                    {
                        profile = fan.profile;
                        if (fan.loyalFans > 0)
                            this.newLoyal = this.loyal = true;
                    }
                    this._loyalty = this._lastLoyalty = profile;
                }
                else
                {
                    this._loyalty = varLoyalty;
                    this._lastLoyalty = varLastLoyalty;
                    this.loyal = this.newLoyal = varLoyal;
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
            this._sprite = spriteMap;
            this.graphic = (Sprite)this._sprite;
            this.collisionSize = new Vec2((float)this._sprite.width, (float)this._sprite.height);
            this.collisionOffset = new Vec2((float)-(this._sprite.w / 2), (float)-(this._sprite.h / 2));
            this.center = new Vec2(0.0f, (float)spriteMap.h);
            this.collisionOffset = new Vec2(this.collisionOffset.x, (float)-this._sprite.h);
            this.depth = (Depth)(float)(0.300000011920929 - (double)row * 0.0500000007450581);
            this.layer = Layer.Background;
            this._letterSign = new Sprite("letterSign");
            this._letterSign.CenterOrigin();
            this._letterSign.depth = this.depth + 2;
            this._font = new BitmapFont("biosFont", 8);
            this._loveSign = new Sprite("loveSign");
            this._loveSign.CenterOrigin();
            this._loveSign.depth = (Depth)(float)(0.319999992847443 - (double)row * 0.0500000007450581);
            this._sucksSign = new Sprite("sucksSign");
            this._sucksSign.CenterOrigin();
            this._sucksSign.depth = (Depth)(float)(0.319999992847443 - (double)row * 0.0500000007450581);
            this._suckSign = new Sprite("suckSign");
            this._suckSign.CenterOrigin();
            this._suckSign.depth = (Depth)(float)(0.319999992847443 - (double)row * 0.0500000007450581);
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (this._empty)
                return;
            if (this._mood != Crowd.mood)
                this._mood = Crowd.mood;
            if (this._mood == Mood.Calm || this._mood == Mood.Silent || this._mood == Mood.Dead || this._mood == Mood.Excited)
                this._sprite.SetAnimation("cheer");
            else if (this._mood == Mood.Extatic)
                this._sprite.SetAnimation("scream");
            if ((double)this._hatThrowTime > 0.0)
            {
                this._hatThrowTime -= 0.01f;
            }
            else
            {
                if ((double)this._hatThrowTime <= -0.5)
                    return;
                this._lastLoyalty = this._loyalty;
                if (this._lastLoyalty == null)
                    SFX.Play("cutOffQuack2", 0.9f, Rando.Float(-0.1f, 0.1f));
                else
                    SFX.Play("cutOffQuack", 0.9f, Rando.Float(-0.1f, 0.1f));
                Level.Add((Thing)SmallSmoke.New(this.x + 6f, this.y - 35f));
                this._hatThrowTime = -1f;
            }
        }

        public override void Draw()
        {
            if (this._sprite == null || this._originalSprite == null)
                return;
            this._sprite.texture = this._originalSprite.texture;
            if (!this._empty && this._letter != null)
            {
                float num = (float)(Math.Sin((double)this._letterSway + (double)this._letterNumber * 0.100000001490116) * 2.0 + 4.0);
                this._letterSway += 0.1f;
                if (this._letter.Length == 1)
                {
                    if ((this._signProfile == null || this._signProfile == this.loyalty) && this._letter != " ")
                    {
                        this._letterSign.depth = this.depth + 5;
                        Graphics.Draw(this._letterSign, this.x + 10f, this.y - 24f + num);
                        this._font.Draw(this._letter, this.x + 6f, this.y - 28f + num, Color.Gray, this.depth + 9);
                    }
                }
                else if (this._hate)
                {
                    if (this._letter[this._letter.Length - 1] == 'S')
                    {
                        Graphics.Draw(this._suckSign, this.x + 28f, this.y - 27f + num);
                        this._font.Draw(this._letter, (float)((double)this.x - (double)this._font.GetWidth(this._letter) / 2.0 + 28.0), (float)((double)this.y - 26.0 - 8.0) + num, Color.Gray, this._suckSign.depth + 3);
                    }
                    else
                    {
                        Graphics.Draw(this._sucksSign, this.x + 28f, this.y - 27f + num);
                        this._font.Draw(this._letter, (float)((double)this.x - (double)this._font.GetWidth(this._letter) / 2.0 + 28.0), (float)((double)this.y - 26.0 - 8.0) + num, Color.Gray, this._sucksSign.depth + 3);
                    }
                }
                else
                {
                    Graphics.Draw(this._loveSign, this.x + 28f, this.y - 27f + num);
                    this._font.Draw(this._letter, (float)((double)this.x - (double)this._font.GetWidth(this._letter) / 2.0 + 29.0), this.y - 26f + num, Color.Gray, this._loveSign.depth + 3);
                }
            }
            if (!this._empty && this._lastLoyalty != null && this._lastLoyalty.persona != null && this._lastLoyalty.team != null)
            {
                SpriteMap g = this._lastLoyalty.persona.defaultHead;
                Vec2 vec2 = Vec2.Zero;
                if (this._lastLoyalty.team.hasHat)
                {
                    vec2 = this._lastLoyalty.team.hatOffset;
                    g = this._lastLoyalty.team.GetHat(this._lastLoyalty.persona);
                }
                if (g == null)
                    return;
                g.depth = this.depth + 2;
                g.angle = 0.0f;
                g.alpha = 1f;
                g.color = Color.White;
                bool flag = this._sprite.imageIndex == 1 || this._sprite.imageIndex == 2 || this._sprite.imageIndex == 4 || this._sprite.imageIndex == 5 || this._sprite.imageIndex == 7 || this._sprite.imageIndex == 8;
                float num = 0.0f;
                if (this._sprite.imageIndex > 2)
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
                if (this.loyal)
                    g.frame = flag ? 1 : 0;
                g.CenterOrigin();
                Graphics.Draw((Sprite)g, (float)((double)this.x - (double)vec2.x + 8.0) + num, (float)((double)this.y - (double)vec2.y - 22.0 - (flag ? 1.0 : 0.0)));
                g.frame = 0;
                g.flipH = false;
            }
            base.Draw();
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.ScoreRock
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Linq;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    public class ScoreRock : Holdable, IPlatform
    {
        public StateBinding _planeBinding = new StateBinding("planeOfExistence");
        public StateBinding _depthBinding = new StateBinding(nameof(netDepth));
        public StateBinding _zBinding = new StateBinding("z");
        public StateBinding _profileBinding = new StateBinding(nameof(netProfileIndex));
        private byte _netProfileIndex;
        private bool _customRock;
        private SpriteMap _sprite;
        private Sprite _dropShadow = new Sprite("dropShadow");
        private Vec2 _dropShadowPoint;
        private Vec2 _pos = Vec2.Zero;
        private Profile _profile;

        public float netDepth
        {
            get => this.depth.value;
            set => this.depth = (Depth)value;
        }

        public byte netProfileIndex
        {
            get => this._netProfileIndex;
            set
            {
                this._netProfileIndex = value;
                this._profile = Profiles.all.ElementAt<Profile>(_netProfileIndex);
                this.RefreshProfile(this._profile);
            }
        }

        public ScoreRock(float xpos, float ypos, Profile profile)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("scoreRock", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(16f, 13f);
            this.depth = -0.5f;
            this.thickness = 4f;
            this.weight = 7f;
            this.RefreshProfile(profile);
            this.flammable = 0.3f;
            this.collideSounds.Add("rockHitGround2");
            this._dropShadow.CenterOrigin();
            this._profile = profile;
            this.impactThreshold = 1f;
        }

        private void RefreshProfile(Profile profile)
        {
            if (profile == null)
                return;
            if (profile.team.hasHat)
                this._sprite.frame = 0;
            else if (profile.persona == Persona.Duck1)
                this._sprite.frame = 1;
            else if (profile.persona == Persona.Duck2)
                this._sprite.frame = 2;
            else if (profile.persona == Persona.Duck3)
                this._sprite.frame = 3;
            else if (profile.persona == Persona.Duck4)
                this._sprite.frame = 4;
            else if (profile.persona == Persona.Duck5)
                this._sprite.frame = 5;
            else if (profile.persona == Persona.Duck6)
                this._sprite.frame = 6;
            else if (profile.persona == Persona.Duck7)
                this._sprite.frame = 7;
            else if (profile.persona == Persona.Duck8)
                this._sprite.frame = 8;
            if (profile.team.rockTexture == null)
                return;
            this._sprite = new SpriteMap((Tex2D)profile.team.rockTexture, 24, 24);
            this.center = new Vec2(12f, 12f);
            this.graphic = _sprite;
            this._customRock = true;
            this.collisionOffset = new Vec2(-8f, -1f);
            this.collisionSize = new Vec2(16f, 13f);
        }

        public override void Update()
        {
            foreach (Block block in Level.CheckLineAll<Block>(this.position, this.position + new Vec2(0f, 100f)))
            {
                if (block.solid)
                {
                    this._dropShadowPoint.x = this.x;
                    this._dropShadowPoint.y = block.top;
                }
            }
            if (RockScoreboard.wallMode && this.x > 610.0)
            {
                this.x = 610f;
                this.hSpeed = -1f;
                SFX.Play("rockHitGround2", pitch: -0.4f);
            }
            if (RockScoreboard.wallMode && this.x > 610.0)
            {
                this.x = 610f;
                this.hSpeed = -1f;
                SFX.Play("rockHitGround2", pitch: -0.4f);
            }
            this._pos = this.position;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if (this._customRock || this._profile == null || this._profile.team == null || !this._profile.team.hasHat)
                return;
            SpriteMap hat = this._profile.team.GetHat(this._profile.persona);
            hat.depth = this.depth + 1;
            hat.center = new Vec2(16f, 16f);
            Vec2 vec2 = this.position - this._profile.team.hatOffset;
            Graphics.Draw(hat, vec2.x, vec2.y - 5f);
        }
    }
}

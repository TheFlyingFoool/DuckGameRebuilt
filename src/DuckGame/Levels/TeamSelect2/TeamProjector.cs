// Decompiled with JetBrains decompiler
// Type: DuckGame.TeamProjector
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class TeamProjector : Thing
    {
        private Sprite _selectPlatform;
        private Sprite _selectProjector;
        private SinWave _projectorSin = (SinWave)0.5f;
        private Profile _profile;
        private List<Profile> _profiles = new List<Profile>();
        private bool _swap;
        private float _swapFade = 1f;

        public TeamProjector(float xpos, float ypos, Profile profile)
          : base(xpos, ypos)
        {
            this._selectPlatform = new Sprite("selectPlatform");
            this._selectPlatform.CenterOrigin();
            this._selectProjector = new Sprite("selectProjector");
            this._selectProjector.CenterOrigin();
            this._profile = profile;
            this._profiles.Add(profile);
        }

        public void SetProfile(Profile newProfile) => this._profile = newProfile;

        public override void Update()
        {
            List<Profile> collection = new List<Profile>();
            collection.Add(this._profile);
            Team team = this._profile.team;
            if (team != null)
                collection = team.activeProfiles;
            bool flag = collection.Count == this._profiles.Count;
            foreach (Profile profile in collection)
            {
                if (!this._profiles.Contains(profile))
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
                this._swap = true;
            if (this._swap)
            {
                this._swapFade -= 0.1f;
            }
            else
            {
                this._swapFade += 0.1f;
                if (_swapFade > 1.0)
                    this._swapFade = 1f;
            }
            if (_swapFade > 0.0 || !this._swap)
                return;
            this._swap = false;
            this._swapFade = 0f;
            this._profiles.Clear();
            this._profiles.AddRange(collection);
        }

        public override void Draw()
        {
            float num1 = -0.53f;
            this._selectProjector.depth = (Depth)num1;
            this._selectProjector.alpha = (float)(0.300000011920929 + this._projectorSin.normalized * 0.200000002980232);
            this._selectPlatform.depth = (Depth)num1;
            int count = this._profiles.Count;
            int num2 = 0;
            foreach (Profile profile in this._profiles)
            {
                Color color = new Color(0.35f, 0.5f, 0.6f);
                profile.persona.sprite.alpha = Maths.Clamp(this._swapFade, 0f, 1f);
                profile.persona.sprite.color = color * (float)(0.699999988079071 + this._projectorSin.normalized * 0.100000001490116);
                profile.persona.sprite.color = new Color(profile.persona.sprite.color.r, profile.persona.sprite.color.g, profile.persona.sprite.color.b);
                profile.persona.sprite.flipH = false;
                profile.persona.armSprite.alpha = Maths.Clamp(this._swapFade, 0f, 1f);
                profile.persona.armSprite.color = color * (float)(0.699999988079071 + this._projectorSin.normalized * 0.100000001490116);
                profile.persona.armSprite.color = new Color(profile.persona.armSprite.color.r, profile.persona.armSprite.color.g, profile.persona.armSprite.color.b);
                profile.persona.armSprite.flipH = false;
                profile.persona.sprite.scale = new Vec2(1f, 1f);
                profile.persona.armSprite.scale = new Vec2(1f, 1f);
                float num3 = 12f;
                float num4 = (float)(this.x - (count - 1) * num3 / 2.0 + num2 * num3);
                profile.persona.sprite.depth = (Depth)(float)(num1 + 0.00999999977648258 + num2 * (1.0 / 1000.0));
                profile.persona.armSprite.depth = (Depth)(float)(num1 + 0.0199999995529652 + num2 * (1.0 / 1000.0));
                Graphics.Draw(profile.persona.sprite, num4 + 1f, this.y - 17f);
                Graphics.Draw(profile.persona.armSprite, (float)(num4 + 1.0 - 3.0), (float)(this.y - 17.0 + 6.0));
                Team team = profile.team;
                if (team != null)
                {
                    Vec2 hatPoint = DuckRig.GetHatPoint(profile.persona.sprite.imageIndex);
                    SpriteMap hat = profile.team.GetHat(profile.persona);
                    hat.depth = profile.persona.sprite.depth + 1;
                    hat.alpha = profile.persona.sprite.alpha;
                    hat.color = profile.persona.sprite.color;
                    hat.center = new Vec2(16f, 16f) + team.hatOffset;
                    hat.flipH = false;
                    Graphics.Draw(hat, (float)(num4 + hatPoint.x + 1.0), this.y - 17f + hatPoint.y);
                    hat.color = Color.White;
                }
                this._profile.persona.sprite.color = Color.White;
                this._profile.persona.armSprite.color = Color.White;
                ++num2;
            }
            Graphics.Draw(this._selectPlatform, this.x, this.y);
            Graphics.Draw(this._selectProjector, this.x, this.y - 6f);
        }
    }
}

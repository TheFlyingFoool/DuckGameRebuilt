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
            _selectPlatform = new Sprite("selectPlatform");
            _selectPlatform.CenterOrigin();
            _selectProjector = new Sprite("selectProjector");
            _selectProjector.CenterOrigin();
            _profile = profile;
            _profiles.Add(profile);
        }

        public void SetProfile(Profile newProfile) => _profile = newProfile;

        public override void Update()
        {
            List<Profile> collection = new List<Profile>();
            collection.Add(_profile);
            Team team = _profile.team;
            if (team != null)
                collection = team.activeProfiles;
            bool flag = collection.Count == _profiles.Count;
            foreach (Profile profile in collection)
            {
                if (!_profiles.Contains(profile))
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
                _swap = true;
            if (_swap)
            {
                _swapFade -= 0.1f;
            }
            else
            {
                _swapFade += 0.1f;
                if (_swapFade > 1f)
                    _swapFade = 1f;
            }
            if (_swapFade > 0f || !_swap)
                return;
            _swap = false;
            _swapFade = 0f;
            _profiles.Clear();
            _profiles.AddRange(collection);
        }

        public override void Draw()
        {
            float num1 = -0.53f;
            _selectProjector.depth = (Depth)num1;
            _selectProjector.alpha = (float)(0.3f + _projectorSin.normalized * 0.2f);
            _selectPlatform.depth = (Depth)num1;
            int count = _profiles.Count;
            int num2 = 0;
            foreach (Profile profile in _profiles)
            {
                Color color = new Color(0.35f, 0.5f, 0.6f);
                profile.persona.sprite.alpha = Maths.Clamp(_swapFade, 0f, 1f);
                profile.persona.sprite.color = color * (float)(0.7f + _projectorSin.normalized * 0.1f);
                profile.persona.sprite.color = new Color(profile.persona.sprite.color.r, profile.persona.sprite.color.g, profile.persona.sprite.color.b);
                profile.persona.sprite.flipH = false;
                profile.persona.armSprite.alpha = Maths.Clamp(_swapFade, 0f, 1f);
                profile.persona.armSprite.color = color * (float)(0.7f + _projectorSin.normalized * 0.1f);
                profile.persona.armSprite.color = new Color(profile.persona.armSprite.color.r, profile.persona.armSprite.color.g, profile.persona.armSprite.color.b);
                profile.persona.armSprite.flipH = false;
                profile.persona.sprite.scale = new Vec2(1f, 1f);
                profile.persona.armSprite.scale = new Vec2(1f, 1f);
                float num3 = 12f;
                float num4 = (float)(x - (count - 1) * num3 / 2f + num2 * num3);
                profile.persona.sprite.depth = (Depth)(float)(num1 + 0.01f + num2 * (1f / 1000f));
                profile.persona.armSprite.depth = (Depth)(float)(num1 + 0.02f + num2 * (1f / 1000f));
                Graphics.Draw(profile.persona.sprite, num4 + 1f, y - 17f);
                Graphics.Draw(profile.persona.armSprite, (float)(num4 + 1f - 3f), (float)(y - 17f + 6f));
                Team team = profile.team;
                if (team != null)
                {
                    Vec2 hatPoint = DuckRig.GetHatPoint(profile.persona.sprite.imageIndex);
                    SpriteMap hat = profile.team.GetHat(profile.persona);
                    hat.scale = new Vec2(1);
                    hat.depth = profile.persona.sprite.depth + 1;
                    hat.alpha = profile.persona.sprite.alpha;
                    hat.color = profile.persona.sprite.color;
                    hat.center = new Vec2(16f, 16f) + team.hatOffset;
                    hat.flipH = false;
                    Graphics.Draw(hat, (float)(num4 + hatPoint.x + 1f), y - 17f + hatPoint.y);
                    hat.color = Color.White;
                }
                _profile.persona.sprite.color = Color.White;
                _profile.persona.armSprite.color = Color.White;
                ++num2;
            }
            Graphics.Draw(_selectPlatform, x, y);
            Graphics.Draw(_selectProjector, x, y - 6f);
        }
    }
}

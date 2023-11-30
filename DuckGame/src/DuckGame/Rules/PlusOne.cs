using System;

namespace DuckGame
{
    public class PlusOne : Thing
    {
        public Duck _duck;
        private BitmapFont _font;
        private Profile _profile;
        private float pulse;
        private bool _temp;
        private float _wait = 1f;
        private bool _testMode;
        private int _num = 1;
        private Interp PlusOneLerp = new Interp(true);

        public PlusOne(float xpos, float ypos, Profile p, bool temp = false, bool testMode = false)
          : base(xpos, ypos)
        {
            _font = new BitmapFont("biosFont", 8);
            _profile = p;
            if (p != null && p.duck != null) p.duck.currentPlusOne = this;
            _temp = temp;
            layer = Layer.Blocks;
            depth = (Depth)0.9f;
            _testMode = testMode;
        }

        public override void Initialize()
        {
            if (!_testMode && _profile != null)
            {
                if (Teams.active.Count > 1 && Network.isActive && _profile.connection == DuckNetwork.localConnection)
                    DuckNetwork.GiveXP("Rounds Won", 1, 4, firstCap: 10, secondCap: 20);
                _profile.stats.lastWon = DateTime.Now;
                ++_profile.stats.matchesWon;
            }
            base.Initialize();
        }

        public void Pulse()
        {
            _wait = 1f;
            pulse = 1f;
            ++_num;
        }

        public override void Update()
        {
            pulse = Lerp.FloatSmooth(pulse, 0f, 0.1f);
            if (pulse < 0.03f)
                pulse = 0f;
            if (!_temp)
                _wait -= 0.01f;
            if (_wait >= 0f)
                return;
            Level.Remove(this);
            if (_duck == null)
                return;
            _duck.currentPlusOne = null;
        }

        public override void Draw()
        {
            if (_profile == null || _profile.persona == null || anchor == null)
                return;
            position = anchor.position;
            _font.scale = new Vec2((float)(1f + pulse * 0.5f));
            _num = 1;
            string text = "+" + _num.ToString();
            float xpos = x - _font.GetWidth(text) / 2f;

            PlusOneLerp.UpdateLerpState(new Vec2(xpos, y), MonoMain.IntraTick, MonoMain.UpdateLerpState);

            _font.Draw(text, PlusOneLerp.x - 1f, PlusOneLerp.y - 1f, Color.Black, (Depth)0.8f);
            _font.Draw(text, PlusOneLerp.x + 1f, PlusOneLerp.y - 1f, Color.Black, (Depth)0.8f);
            _font.Draw(text, PlusOneLerp.x - 1f, PlusOneLerp.y + 1f, Color.Black, (Depth)0.8f);
            _font.Draw(text, PlusOneLerp.x + 1f, PlusOneLerp.y + 1f, Color.Black, (Depth)0.8f);
            Color c = new Color((byte)_profile.persona.color.x, (byte)_profile.persona.color.y, (byte)_profile.persona.color.z);
            _font.Draw(text, PlusOneLerp.x, PlusOneLerp.y, c, (Depth)0.9f);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.CoolnessPlus
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CoolnessPlus : Thing
    {
        private BitmapFont _font;
        private Profile _profile;
        private float _wait = 1f;
        private int _change;

        public float wait
        {
            get => _wait;
            set => _wait = value;
        }

        public int change
        {
            get => _change;
            set
            {
                _change = value;
                _wait = 1f;
            }
        }

        public CoolnessPlus(float xpos, float ypos, Duck d, int c)
          : base(xpos, ypos)
        {
            _font = new BitmapFont("biosFont", 8);
            change = c;
            anchor = (Anchor)d;
            anchor.offset = new Vec2(-0f, -24f);
            _profile = d.profile;
        }

        public override void Initialize() => SFX.Play("scoreDing", 0.8f);

        public override void Update()
        {
            _wait -= 0.01f;
            if (_wait >= 0.0)
                return;
            Level.Remove(this);
        }

        public override void Draw()
        {
            position = anchor.position;
            string str = "";
            if (change > 0)
                str = "+";
            string text = str + change.ToString();
            float xpos = x - _font.GetWidth(text) / 2f;
            _font.Draw(text, xpos - 1f, y - 1f, Color.Black, (Depth)0.8f);
            _font.Draw(text, xpos + 1f, y - 1f, Color.Black, (Depth)0.8f);
            _font.Draw(text, xpos - 1f, y + 1f, Color.Black, (Depth)0.8f);
            _font.Draw(text, xpos + 1f, y + 1f, Color.Black, (Depth)0.8f);
            Color c = new Color((byte)_profile.persona.color.x, (byte)_profile.persona.color.y, (byte)_profile.persona.color.z);
            _font.Draw(text, xpos, y, c, (Depth)0.9f);
        }
    }
}

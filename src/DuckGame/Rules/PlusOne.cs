// Decompiled with JetBrains decompiler
// Type: DuckGame.PlusOne
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public PlusOne(float xpos, float ypos, Profile p, bool temp = false, bool testMode = false)
          : base(xpos, ypos)
        {
            this._font = new BitmapFont("biosFont", 8);
            this._profile = p;
            this._temp = temp;
            this.layer = Layer.Blocks;
            this.depth = (Depth)0.9f;
            this._testMode = testMode;
        }

        public override void Initialize()
        {
            if (!this._testMode && this._profile != null)
            {
                if (Teams.active.Count > 1 && Network.isActive && this._profile.connection == DuckNetwork.localConnection)
                    DuckNetwork.GiveXP("Rounds Won", 1, 4, firstCap: 10, secondCap: 20);
                this._profile.stats.lastWon = DateTime.Now;
                ++this._profile.stats.matchesWon;
            }
            base.Initialize();
        }

        public void Pulse()
        {
            this._wait = 1f;
            this.pulse = 1f;
            ++this._num;
        }

        public override void Update()
        {
            this.pulse = Lerp.FloatSmooth(this.pulse, 0f, 0.1f);
            if (pulse < 0.03f)
                this.pulse = 0f;
            if (!this._temp)
                this._wait -= 0.01f;
            if (_wait >= 0f)
                return;
            Level.Remove(this);
            if (this._duck == null)
                return;
            this._duck.currentPlusOne = null;
        }

        public override void Draw()
        {
            if (this._profile == null || this._profile.persona == null || this.anchor == null)
                return;
            this.position = this.anchor.position;
            this._font.scale = new Vec2((float)(1f + pulse * 0.5f));
            this._num = 1;
            string text = "+" + this._num.ToString();
            float xpos = this.x - this._font.GetWidth(text) / 2f;
            this._font.Draw(text, xpos - 1f, this.y - 1f, Color.Black, (Depth)0.8f);
            this._font.Draw(text, xpos + 1f, this.y - 1f, Color.Black, (Depth)0.8f);
            this._font.Draw(text, xpos - 1f, this.y + 1f, Color.Black, (Depth)0.8f);
            this._font.Draw(text, xpos + 1f, this.y + 1f, Color.Black, (Depth)0.8f);
            Color c = new Color((byte)this._profile.persona.color.x, (byte)this._profile.persona.color.y, (byte)this._profile.persona.color.z);
            this._font.Draw(text, xpos, this.y, c, (Depth)0.9f);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.Pedestal
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class Pedestal : Thing
    {
        private Team _team;
        private SpriteMap _sprite;
        private Sprite _scoreCard;
        private BitmapFont _font;
        private Sprite _trophy;
        private List<Duck> _ducks = new List<Duck>();

        public Pedestal(float xpos, float ypos, Team team, int place, bool smallMode)
          : base(xpos, ypos)
        {
            this._team = team;
            this._sprite = !smallMode ? new SpriteMap("rockThrow/placePedastals", 38, 45) : new SpriteMap("rockThrow/placePedastalsSmall", 27, 45);
            this._sprite.frame = place;
            this.center = new Vec2((float)(this._sprite.w / 2), (float)this._sprite.h);
            this.graphic = (Sprite)this._sprite;
            this.depth = (Depth)0.062f;
            this._scoreCard = new Sprite("rockThrow/scoreCard");
            this._font = new BitmapFont("biosFont", 8);
            this._scoreCard.CenterOrigin();
            this._trophy = new Sprite("trophy");
            this._trophy.CenterOrigin();
            if (Network.isServer)
            {
                int num1 = 0;
                foreach (Profile activeProfile in team.activeProfiles)
                {
                    float num2 = (float)((team.activeProfiles.Count - 1) * 10);
                    Duck duck = new Duck(xpos - num2 / 2f + (float)(num1 * 10), this.GetYOffset() - 15f, activeProfile);
                    duck.depth = (Depth)0.06f;
                    Level.Add((Thing)duck);
                    if (place == 0)
                    {
                        Trophy trophy = new Trophy(duck.x, duck.y);
                        Level.Add((Thing)trophy);
                        if (!Network.isActive)
                        {
                            duck.Fondle((Thing)trophy);
                            duck.GiveHoldable((Holdable)trophy);
                        }
                    }
                    ++num1;
                }
            }
            Level.Add((Thing)new Platform(xpos - 17f, this.GetYOffset(), 34f, 16f));
            Level.Add((Thing)new Block(-6f, this.GetYOffset() - 100f, 6f, 200f));
            Level.Add((Thing)new Block(320f, this.GetYOffset() - 100f, 6f, 200f));
            Level.Add((Thing)new Block(-20f, 155f, 600f, 100f));
        }

        public override void Update()
        {
        }

        public float GetYOffset()
        {
            float yoffset = this.y - 45f;
            if (this._sprite.frame == 1)
                yoffset = this.y - 28f;
            else if (this._sprite.frame == 2)
                yoffset = this.y - 19f;
            else if (this._sprite.frame == 3)
                yoffset = this.y - 12f;
            return yoffset;
        }

        public override void Draw()
        {
            this.depth = - 0.5f;
            base.Draw();
            int count = this._team.activeProfiles.Count;
            if (this._sprite.frame == 0)
            {
                this._trophy.depth = this.depth + 1;
                Graphics.Draw(this._trophy, this.x, this.y - 14f);
            }
            this._scoreCard.depth = (Depth)1f;
            Graphics.Draw(this._scoreCard, this.x, this.y + 2f);
            string text = Change.ToString((object)this._team.score);
            this._font.Draw(text, this.x - this._font.GetWidth(text) / 2f, this.y, Color.DarkSlateGray, this._scoreCard.depth + 1);
        }
    }
}

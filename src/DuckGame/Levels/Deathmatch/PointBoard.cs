// Decompiled with JetBrains decompiler
// Type: DuckGame.PointBoard
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PointBoard : Thing
    {
        private BitmapFont _font;
        private Sprite _scoreCard;
        private Team _team;
        private Thing _stick;

        public PointBoard(Thing rock, Team t)
          : base(rock.x + 24f, rock.y)
        {
            this._scoreCard = new Sprite("rockThrow/scoreCard");
            this._font = new BitmapFont("biosFont", 8);
            this._team = t;
            this._scoreCard.CenterOrigin();
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(16f, 13f);
            this.center = new Vec2((float)(this._scoreCard.w / 2), (float)(this._scoreCard.h / 2));
            this._stick = rock;
            this.depth = - 0.1f;
        }

        public override void Update()
        {
            this.x = this._stick.x + 24f;
            this.y = this._stick.y;
        }

        public override void Draw()
        {
            this._scoreCard.depth = this.depth;
            Graphics.Draw(this._scoreCard, this.x, this.y);
            if (this._team == null)
            {
                string text = "X";
                this._font.Draw(text, this.x - this._font.GetWidth(text) / 2f, this.y - 2f, Color.DarkSlateGray, this._scoreCard.depth + 1);
            }
            else
            {
                string text = Change.ToString((object)this._team.score);
                this._font.Draw(text, this.x - this._font.GetWidth(text) / 2f, this.y - 2f, Color.DarkSlateGray, this._scoreCard.depth + 1);
            }
        }
    }
}

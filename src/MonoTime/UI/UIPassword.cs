// Decompiled with JetBrains decompiler
// Type: DuckGame.UIStringEntry
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIStringEntry : UIText
    {
        private bool _directionalPassword;

        public UIStringEntry(
          bool directional,
          string textVal,
          Color c,
          UIAlign al = UIAlign.Center,
          float heightAdd = 0f,
          InputProfile controlProfile = null)
          : base(textVal, c, al, heightAdd, controlProfile)
        {
            this._directionalPassword = directional;
        }

        public override void Draw()
        {
            if (this._directionalPassword && this._text != "  NONE")
            {
                this._collisionSize.x = 48f;
                float num = this._text.Length * 8;
                Graphics.DrawPassword(this._text, new Vec2(this.x + (((this.align & UIAlign.Left) <= UIAlign.Center ? ((this.align & UIAlign.Right) <= UIAlign.Center ? (float)(-(double)num / 2.0) : this.width / 2f - num) : (float)-((double)this.width / 2.0)) - 8f), this.y + ((this.align & UIAlign.Top) <= UIAlign.Center ? ((this.align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-(double)this._font.height / 2.0) : this.height / 2f - this._font.height) : (float)-((double)this.height / 2.0))), this._color, this.depth);
            }
            else
            {
                if (this._text.Length > 10)
                    this._text = this._text.Substring(0, 8) + "..";
                this._collisionSize.x = 48f;
                float num = this._text.Length * 8;
                Graphics.DrawString(this._text, new Vec2(this.x + (((this.align & UIAlign.Left) <= UIAlign.Center ? ((this.align & UIAlign.Right) <= UIAlign.Center ? (float)(-(double)num / 2.0) : this.width / 2f - num) : (float)-((double)this.width / 2.0)) - 8f), this.y + ((this.align & UIAlign.Top) <= UIAlign.Center ? ((this.align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-(double)this._font.height / 2.0) : this.height / 2f - this._font.height) : (float)-((double)this.height / 2.0))), this._color, this.depth);
            }
        }
    }
}

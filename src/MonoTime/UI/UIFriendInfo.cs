// Decompiled with JetBrains decompiler
// Type: DuckGame.UIFriendInfo
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class UIFriendInfo : UIMenuItem
    {
        private UIMenu _rootMenu;
        private Sprite _avatar;

        public UIFriendInfo(User friend, UIMenu rootMenu)
          : base(" " + friend.name)
        {
            byte[] avatarSmall = friend.avatarSmall;
            if (avatarSmall != null)
            {
                Texture2D tex = new Texture2D(DuckGame.Graphics.device, 32, 32);
                tex.SetData<byte>(avatarSmall);
                this._avatar = new Sprite((Tex2D)tex);
                this._avatar.CenterOrigin();
            }
            this._rootMenu = rootMenu;
            this._collisionSize.y = 14f;
            this._textElement.SetFont(new BitmapFont("smallBiosFont", 7, 6));
            this._textElement.text = "  " + friend.name + "\n  |LIME|WANTS TO PLAY";
        }

        public override void Activate(string trigger)
        {
        }

        public override void Update() => base.Update();

        public override void Draw()
        {
            DuckGame.Graphics.DrawRect(this.leftSection.topLeft, this.rightSection.bottomRight, Colors.BlueGray, this.depth - 1);
            if (this._avatar != null)
            {
                this._avatar.depth = this.depth + 2;
                this._avatar.scale = new Vec2(0.25f);
                DuckGame.Graphics.Draw(this._avatar, (float)(this.leftSection.left + _avatar.width * this._avatar.scale.x / 2.0 + 6.0), this.y + 3f);
            }
            base.Draw();
        }
    }
}

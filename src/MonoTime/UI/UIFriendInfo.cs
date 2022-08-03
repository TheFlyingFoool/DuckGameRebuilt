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
                _avatar = new Sprite((Tex2D)tex);
                _avatar.CenterOrigin();
            }
            _rootMenu = rootMenu;
            _collisionSize.y = 14f;
            _textElement.SetFont(new BitmapFont("smallBiosFont", 7, 6));
            _textElement.text = "  " + friend.name + "\n  |LIME|WANTS TO PLAY";
        }

        public override void Activate(string trigger)
        {
        }

        public override void Update() => base.Update();

        public override void Draw()
        {
            DuckGame.Graphics.DrawRect(leftSection.topLeft, rightSection.bottomRight, Colors.BlueGray, depth - 1);
            if (_avatar != null)
            {
                _avatar.depth = depth + 2;
                _avatar.scale = new Vec2(0.25f);
                DuckGame.Graphics.Draw(_avatar, (float)(leftSection.left + _avatar.width * _avatar.scale.x / 2.0 + 6.0), y + 3f);
            }
            base.Draw();
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.UIInviteMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIInviteMenu : UIMenu
    {
        private static Dictionary<int, int> _sortDictionary = new Dictionary<int, int>()
    {
      {
        0,
        6
      },
      {
        1,
        1
      },
      {
        2,
        3
      },
      {
        3,
        4
      },
      {
        4,
        5
      },
      {
        5,
        2
      },
      {
        6,
        0
      }
    };
        public static Dictionary<ulong, Sprite> avatars = new Dictionary<ulong, Sprite>();
        private List<UIInviteUser> _users = new List<UIInviteUser>();
        private BitmapFont _littleFont;
        private UIBox _box;
        private new int _selection;
        private int _viewTop;
        private Sprite _moreArrow;
        private Sprite _noAvatar;
        private UIMenuAction _menuAction;
        private int _maxShow = 9;

        public static void Initialize()
        {
            if (!Steam.IsInitialized())
                return;
            foreach (User friend in Steam.friends)
                UIInviteMenu.avatars[friend.id] = UIInviteMenu.PrepareSprite(friend);
        }

        public static Sprite PrepareSprite(User u)
        {
            byte[] avatarMedium = u.avatarMedium;
            Sprite sprite = null;
            if (avatarMedium != null && avatarMedium.Length == 16384)
            {
                Texture2D tex = new Texture2D(DuckGame.Graphics.device, 64, 64);
                tex.SetData<byte>(avatarMedium);
                sprite = new Sprite((Tex2D)tex);
                sprite.CenterOrigin();
            }
            return sprite;
        }

        public static Sprite GetAvatar(User u)
        {
            Sprite avatar;
            if (!UIInviteMenu.avatars.TryGetValue(u.id, out avatar))
                avatar = UIInviteMenu.PrepareSprite(u);
            return avatar;
        }

        public void SetAction(UIMenuAction a) => this._menuAction = a;

        public UIInviteMenu(
          string title,
          UIMenuAction act,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          string conString = "",
          InputProfile conProfile = null,
          bool tiny = false)
          : base(title, xpos, ypos, wide, high)
        {
            if (Steam.IsInitialized())
            {
                int num = Steam.friends.OrderBy<User, int>(u => UIInviteMenu._sortDictionary[(int)u.state]).Count<User>();
                if (num > this._maxShow)
                    num = this._maxShow;
                this._littleFont = new BitmapFont("smallBiosFont", 7, 6);
                this._moreArrow = new Sprite("moreArrow");
                this._moreArrow.CenterOrigin();
                this._box = new UIBox(0.0f, 0.0f, 100f, 14 * num + 8, isVisible: false);
                this._noAvatar = new Sprite("noAvatar");
                this._noAvatar.CenterOrigin();
                this.Add(_box, true);
            }
            this._menuAction = act;
        }

        public override void Open()
        {
            HUD.CloseAllCorners();
            HUD.AddCornerControl(HUDCorner.BottomRight, "@MENU1@INVITE");
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@EXIT");
            this._users.Clear();
            if (Steam.IsInitialized())
            {
                IOrderedEnumerable<User> source1 = Steam.friends.OrderBy<User, int>(u => UIInviteMenu._sortDictionary[(int)u.state]);
                int num = source1.Count<User>();
                for (int index = 0; index < num; ++index)
                {
                    User u = source1.ElementAt<User>(index);
                    string source2 = u.name;
                    if (source2.Count<char>() > 17)
                        source2 = source2.Substring(0, 16) + ".";
                    UserInfo info = u.info;
                    if (info.relationship == FriendRelationship.Friend)
                        this._users.Add(new UIInviteUser()
                        {
                            user = u,
                            sprite = UIInviteMenu.GetAvatar(u),
                            state = info.state,
                            name = source2,
                            inGame = info.inGame,
                            inDuckGame = info.inCurrentGame,
                            inMyLobby = info.inLobby
                        });
                }
                this._users = this._users.OrderBy<UIInviteUser, UIInviteUser>(h => h, new CompareUsers()).ToList<UIInviteUser>();
            }
            base.Open();
        }

        public override void Close()
        {
            HUD.CloseAllCorners();
            base.Close();
        }

        public override void Update()
        {
            if (this.open)
            {
                if (Input.Pressed("MENUUP") && this._selection > 0)
                {
                    --this._selection;
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("MENUDOWN") && this._selection < this._users.Count - 1)
                {
                    ++this._selection;
                    SFX.Play("textLetter", 0.7f);
                }
                if (this._selection >= this._viewTop + this._maxShow)
                    this._viewTop = this._selection - (this._maxShow - 1);
                if (this._selection < this._viewTop)
                    this._viewTop = this._selection;
                if (Input.Pressed("CANCEL"))
                {
                    this._menuAction.Activate();
                    SFX.Play("resume", 0.6f);
                }
                if (this._users.Count > 0 && Input.Pressed("MENU1") && !this._users[this._selection].triedInvite)
                {
                    SFX.Play("rockHitGround", 0.8f);
                    this._users[this._selection].triedInvite = true;
                    TeamSelect2.InvitedFriend(this._users[this._selection].user);
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            int num1 = this._users.Count;
            if (num1 > this._maxShow)
                num1 = this._maxShow;
            float num2 = 14 * num1 - 12;
            float num3 = 0.0f;
            bool flag = false;
            for (int viewTop = this._viewTop; viewTop < this._viewTop + this._maxShow && viewTop < this._users.Count; ++viewTop)
            {
                UIInviteUser user = this._users[viewTop];
                float y = this.y - num2 / 2f + num3;
                float x = this.x - 68f;
                Sprite g = user.sprite ?? this._noAvatar;
                g.depth = this.depth + 4;
                g.scale = new Vec2(0.25f);
                g.alpha = this._selection == viewTop ? 1f : 0.3f;
                DuckGame.Graphics.Draw(g, x + 8f, y + 8f, new Rectangle(6f, 6f, 52f, 52f));
                this._littleFont.Draw(user.name, new Vec2(x + 15f, y), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                if (user.triedInvite)
                    this._littleFont.Draw("|LIME|@CHECK@INVITED", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                else if (user.inGame)
                {
                    if (user.inDuckGame)
                        this._littleFont.Draw("@ITEMBOX@|DGBLUE|IN DUCK GAME!", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                    else
                        this._littleFont.Draw("@USERONLINE@|YELLOW|IN SOME GAME", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                }
                else if (user.state == SteamUserState.Online)
                    this._littleFont.Draw("@USERONLINE@|DGGREEN|ONLINE", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                else if (user.state == SteamUserState.Away)
                    this._littleFont.Draw("@USERAWAY@|YELLOW|AWAY", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                else if (user.state == SteamUserState.Busy)
                    this._littleFont.Draw("@USERBUSY@|YELLOW|BUSY", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                else if (user.state == SteamUserState.Snooze)
                    this._littleFont.Draw("@USERBUSY@|YELLOW|SNOOZE", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                else if (user.state == SteamUserState.Offline)
                    this._littleFont.Draw("@USEROFFLINE@|LIGHTGRAY|OFFLINE", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                else if (user.state == SteamUserState.LookingToPlay)
                    this._littleFont.Draw("@USERONLINE@|DGGREEN|WANTS TO PLAY", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                else if (user.state == SteamUserState.LookingToTrade)
                    this._littleFont.Draw("@USERONLINE@|DGGREEN|WANTS TO TRADE", new Vec2(x + 15f, y + 6f), Color.White * (this._selection == viewTop ? 1f : 0.3f), this.depth + 4);
                DuckGame.Graphics.DrawRect(new Vec2(x, y), new Vec2(x + 135f, y + 13f), (flag ? Colors.BlueGray : Colors.BlueGray * 0.6f) * (this._selection == viewTop ? 1f : 0.3f), this.depth + 2);
                num3 += 14f;
                flag = !flag;
            }
            if (this._viewTop < this._users.Count - this._maxShow)
            {
                this._moreArrow.depth = this.depth + 2;
                this._moreArrow.flipV = false;
                DuckGame.Graphics.Draw(this._moreArrow, this.x, (float)((double)this.y + (double)num2 / 2.0 + 13.0));
            }
            if (this._viewTop > 0)
            {
                this._moreArrow.depth = this.depth + 2;
                this._moreArrow.flipV = true;
                DuckGame.Graphics.Draw(this._moreArrow, this.x, (float)((double)this.y - (double)num2 / 2.0 - 2.0));
            }
            base.Draw();
        }
    }
}

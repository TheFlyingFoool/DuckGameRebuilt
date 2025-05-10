using Microsoft.Xna.Framework.Graphics;
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
                avatars[friend.id] = PrepareSprite(friend);
        }

        public static Sprite PrepareSprite(User u)
        {
            byte[] avatarMedium = u.avatarMedium;
            Sprite sprite = null;
            if (avatarMedium != null && avatarMedium.Length == 16384)
            {
                Texture2D tex = new Texture2D(Graphics.device, 64, 64);
                tex.SetData(avatarMedium);
                sprite = new Sprite((Tex2D)tex);
                sprite.CenterOrigin();
            }
            return sprite;
        }

        public static Sprite GetAvatar(User u)
        {
            Sprite avatar;
            if (!avatars.TryGetValue(u.id, out avatar))
                avatar = PrepareSprite(u);
            return avatar;
        }

        public void SetAction(UIMenuAction a) => _menuAction = a;

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
                int num = Steam.friends.OrderBy(u => _sortDictionary[(int)u.state]).Count();
                if (num > _maxShow)
                    num = _maxShow;
                _littleFont = new BitmapFont("smallBiosFont", 7, 6);
                _moreArrow = new Sprite("moreArrow");
                _moreArrow.CenterOrigin();
                _box = new UIBox(0f, 0f, 100f, 14 * num + 8, isVisible: false);
                _noAvatar = new Sprite("noAvatar");
                _noAvatar.CenterOrigin();
                Add(_box, true);
            }
            _menuAction = act;
        }

        public override void Open()
        {
            HUD.CloseAllCorners();
            HUD.AddCornerControl(HUDCorner.BottomRight, "@MENU1@INVITE");
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@EXIT");
            _users.Clear();
            if (Steam.IsInitialized())
            {
                IOrderedEnumerable<User> source1 = Steam.friends.OrderBy(u => _sortDictionary[(int)u.state])
                                                                .ThenBy(u => u.name);
                int num = source1.Count();
                for (int index = 0; index < num; ++index)
                {
                    User u = source1.ElementAt(index);
                    string source2 = u.name;
                    if (source2.Length > 17)
                        source2 = source2.Substring(0, 16) + ".";
                    UserInfo info = u.info;
                    if (info.relationship == FriendRelationship.Friend)
                        _users.Add(new UIInviteUser()
                        {
                            user = u,
                            sprite = GetAvatar(u),
                            state = info.state,
                            name = source2,
                            inGame = info.inGame,
                            inDuckGame = info.inCurrentGame,
                            inMyLobby = info.inLobby
                        });
                }
                _users = _users.OrderBy(h => h, new CompareUsers()).ToList();
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
            if (open)
            {
                if (Input.Pressed(Triggers.MenuUp) && _selection > 0)
                {
                    _selection--;
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed(Triggers.MenuDown) && _selection < _users.Count - 1)
                {
                    _selection++;
                    SFX.Play("textLetter", 0.7f);
                }
                if (_selection >= _viewTop + _maxShow)
                    _viewTop = _selection - (_maxShow - 1);
                if (_selection < _viewTop)
                    _viewTop = _selection;
                if (Input.Pressed(Triggers.Cancel))
                {
                    _menuAction.Activate();
                    SFX.Play("resume", 0.6f);
                }
                if (_users.Count > 0 && Input.Pressed(Triggers.Menu1) && !_users[_selection].triedInvite)
                {
                    SFX.Play("rockHitGround", 0.8f);
                    _users[_selection].triedInvite = true;
                    if (DGRSettings.MidGameJoining && Level.current is not TeamSelect2)
                    {
                        Steam.InviteUser(_users[_selection].user, Steam.lobby);
                    }
                    else  TeamSelect2.InvitedFriend(_users[_selection].user);
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            int num1 = _users.Count;
            if (num1 > _maxShow)
                num1 = _maxShow;
            float num2 = 14 * num1 - 12;
            float num3 = 0f;
            bool flag = false;
            for (int viewTop = _viewTop; viewTop < _viewTop + _maxShow && viewTop < _users.Count; ++viewTop)
            {
                UIInviteUser user = _users[viewTop];
                float y = this.y - num2 / 2f + num3;
                float x = this.x - 68f;
                Sprite g = user.sprite ?? _noAvatar;
                g.depth = depth + 4;
                g.scale = new Vec2(0.25f);
                g.alpha = _selection == viewTop ? 1f : 0.3f;
                Graphics.Draw(g, x + 8f, y + 8f, new Rectangle(6f, 6f, 52f, 52f));
                _littleFont.Draw(user.name, new Vec2(x + 15f, y), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                if (user.triedInvite)
                    _littleFont.Draw("|LIME|@CHECK@INVITED", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                else if (user.inGame)
                {
                    if (user.inDuckGame)
                        _littleFont.Draw("@ITEMBOX@|DGBLUE|IN DUCK GAME!", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                    else
                        _littleFont.Draw("@USERONLINE@|YELLOW|IN SOME GAME", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                }
                else if (user.state == SteamUserState.Online)
                    _littleFont.Draw("@USERONLINE@|DGGREEN|ONLINE", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                else if (user.state == SteamUserState.Away)
                    _littleFont.Draw("@USERAWAY@|YELLOW|AWAY", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                else if (user.state == SteamUserState.Busy)
                    _littleFont.Draw("@USERBUSY@|YELLOW|BUSY", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                else if (user.state == SteamUserState.Snooze)
                    _littleFont.Draw("@USERBUSY@|YELLOW|SNOOZE", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                else if (user.state == SteamUserState.Offline)
                    _littleFont.Draw("@USEROFFLINE@|LIGHTGRAY|OFFLINE", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                else if (user.state == SteamUserState.LookingToPlay)
                    _littleFont.Draw("@USERONLINE@|DGGREEN|WANTS TO PLAY", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                else if (user.state == SteamUserState.LookingToTrade)
                    _littleFont.Draw("@USERONLINE@|DGGREEN|WANTS TO TRADE", new Vec2(x + 15f, y + 6f), Color.White * (_selection == viewTop ? 1f : 0.3f), depth + 4);
                Graphics.DrawRect(new Vec2(x, y), new Vec2(x + 135f, y + 13f), (flag ? Colors.BlueGray : Colors.BlueGray * 0.6f) * (_selection == viewTop ? 1f : 0.3f), depth + 2);
                num3 += 14f;
                flag = !flag;
            }
            if (_viewTop < _users.Count - _maxShow)
            {
                _moreArrow.depth = depth + 2;
                _moreArrow.flipV = false;
                Graphics.Draw(_moreArrow, x, (float)(y + num2 / 2f + 13));
            }
            if (_viewTop > 0)
            {
                _moreArrow.depth = depth + 2;
                _moreArrow.flipV = true;
                Graphics.Draw(_moreArrow, x, (float)(y - num2 / 2f - 2));
            }
            base.Draw();
        }
    }
}

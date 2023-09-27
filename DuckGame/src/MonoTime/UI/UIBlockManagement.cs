// Decompiled with JetBrains decompiler
// Type: DuckGame.UIBlockManagement
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using SDL2;
using System.Collections.Generic;

namespace DuckGame
{
    public class UIBlockManagement : UIMenu
    {
        private List<KeyValuePair<ulong, bool>> items = new List<KeyValuePair<ulong, bool>>();
        private Sprite _downArrow;
        private BitmapFont _littleFont;
        private UIMenu _openOnClose;
        private bool _opening;
        private int _topOffset;
        private readonly int kMaxInView = 13;

        public UIBlockManagement(UIMenu openOnClose)
          : base("BLOCKED USERS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 260f, 160f)
        {
            Add(new UIBox(0f, 0f, 100f, 130f, isVisible: false), true);
            _littleFont = new BitmapFont("smallBiosFont", 7, 6);
            _downArrow = new Sprite("cloudDown");
            _downArrow.CenterOrigin();
            _openOnClose = openOnClose;
        }

        //private void UnblockUsers()
        //{
        //}

        public override void Open()
        {
            HUD.CloseAllCorners();
            _opening = true;
            RebuildItemList();
            base.Open();
        }

        public void RebuildItemList()
        {
            _selection = 0;
            _topOffset = 0;
            items.Clear();
            foreach (ulong blockedPlayer in Options.Data.blockedPlayers)
                items.Add(new KeyValuePair<ulong, bool>(blockedPlayer, true));
            foreach (ulong unblockedPlayer in Options.Data.unblockedPlayers)
                items.Add(new KeyValuePair<ulong, bool>(unblockedPlayer, false));
            bool flag = false;
            foreach (ulong recentPlayer in Options.Data.recentPlayers)
            {
                if (!Options.Data.blockedPlayers.Contains(recentPlayer) && !Options.Data.unblockedPlayers.Contains(recentPlayer))
                {
                    if (!flag)
                    {
                        flag = true;
                        items.Add(new KeyValuePair<ulong, bool>(0UL, false));
                    }
                    items.Add(new KeyValuePair<ulong, bool>(recentPlayer, Options.Data.blockedPlayers.Contains(recentPlayer)));
                }
            }
        }

        public override void Close()
        {
            HUD.CloseAllCorners(true);
            base.Close();
        }

        public override void Update()
        {
            if (open && !_opening)
            {
                if (Input.Pressed(Triggers.MenuUp) && _selection > 0)
                {
                    --_selection;
                    if (items[_selection].Key == 0UL)
                        --_selection;
                    if (_selection < _topOffset)
                        _topOffset = _selection;
                    SFX.DontSave = 1;
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed(Triggers.MenuDown) && _selection < items.Count - 1)
                {
                    ++_selection;
                    if (items[_selection].Key == 0UL)
                        ++_selection;
                    if (_selection > _topOffset + kMaxInView)
                        ++_topOffset;
                    SFX.DontSave = 1;
                    SFX.Play("textLetter", 0.7f);
                }
                if (_selection >= 0 && _selection < items.Count && Input.Pressed(Triggers.Menu1))
                {
                    KeyValuePair<ulong, bool> keyValuePair = items[_selection];
                    if (!keyValuePair.Value)
                    {
                        if (!Options.Data.blockedPlayers.Contains(keyValuePair.Key))
                            Options.Data.blockedPlayers.Add(keyValuePair.Key);
                        Options.Data.unblockedPlayers.Remove(keyValuePair.Key);
                        Options.Data.muteSettings[keyValuePair.Key] = "CHR";
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                        MakeDirty();
                    }
                    else if (keyValuePair.Value)
                    {
                        Options.Data.blockedPlayers.Remove(keyValuePair.Key);
                        if (!Options.Data.unblockedPlayers.Contains(keyValuePair.Key))
                            Options.Data.unblockedPlayers.Add(keyValuePair.Key);
                        Options.Data.muteSettings[keyValuePair.Key] = "";
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                        MakeDirty();
                    }
                }
                if (Input.Pressed(Triggers.Select))
                    AddedContent.othello7.HelperMethods.OpenURL("http://steamcommunity.com/profiles/" + items[_selection].Key.ToString());
                if (Input.Pressed(Triggers.Cancel))
                {
                    if (_openOnClose != null)
                        new UIMenuActionOpenMenu(this, _openOnClose).Activate();
                    else
                        new UIMenuActionCloseMenu(this).Activate();
                }
            }
            _opening = false;
            base.Update();
        }

        private void MakeDirty()
        {
            foreach (Profile profile in Profiles.all)
                profile._blockStatusDirty = true;
            RebuildItemList();
            if (_selection < items.Count)
                return;
            _selection = items.Count;
        }

        public override void Draw()
        {
            if (open)
            {
                Vec2 vec2 = new Vec2(x - 124f, this.y - 56f);
                float y = 0f;
                int num1 = 0;
                int num2 = 0;
                if (items.Count == 0)
                    _littleFont.Draw("No blocked users! Happy day!", vec2 + new Vec2(8f, y), Color.White, (Depth)0.5f);
                foreach (KeyValuePair<ulong, bool> keyValuePair in items)
                {
                    if (num1 < _topOffset)
                    {
                        ++num1;
                    }
                    else
                    {
                        if (_topOffset > 0)
                        {
                            _downArrow.flipV = true;
                            Graphics.Draw(_downArrow, x, vec2.y - 2f, (Depth)0.5f);
                        }
                        if (num2 > kMaxInView)
                        {
                            _downArrow.flipV = false;
                            Graphics.Draw(_downArrow, x, vec2.y + y, (Depth)0.5f);
                            break;
                        }
                        string str1 = keyValuePair.Key.ToString();
                        string text;
                        if (keyValuePair.Key == 0UL)
                        {
                            text = " |DGBLUE|Recently played with:";
                        }
                        else
                        {
                            User user = User.GetUser(keyValuePair.Key);
                            if (user != null)
                                str1 = user.name;
                            if (str1.Length > 31)
                                str1 = str1.Substring(0, 30) + "..";
                            string str2 = (keyValuePair.Value ? "@DELETEFLAG_ON@" : "@DELETEFLAG_OFF@") + str1;
                            text = num1 != _selection ? " " + str2 : "@SELECTICON@" + str2;
                        }
                        if (keyValuePair.Value)
                            text = "|DGRED|" + text;
                        _littleFont.Draw(text, vec2 + new Vec2(0f, y), Color.White, (Depth)0.5f);
                        y += 8f;
                        ++num1;
                        ++num2;
                    }
                }
                string text1 = "@CANCEL@BACK";
                if (items.Count > 0 && _selection >= 0 && _selection < items.Count)
                    text1 = (!items[_selection].Value ? text1 + " @MENU1@BLOCK" : text1 + " @MENU1@UN-BLOCK") + " @SELECT@@STEAMICON@";
                _littleFont.Draw(text1, new Vec2(x - _littleFont.GetWidth(text1) / 2f, this.y + 64f), Color.White, (Depth)0.5f);
            }
            base.Draw();
        }
    }
}

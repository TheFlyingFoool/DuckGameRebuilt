// Decompiled with JetBrains decompiler
// Type: DuckGame.UIBlockManagement
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this.Add((UIComponent)new UIBox(0.0f, 0.0f, 100f, 130f, isVisible: false), true);
            this._littleFont = new BitmapFont("smallBiosFont", 7, 6);
            this._downArrow = new Sprite("cloudDown");
            this._downArrow.CenterOrigin();
            this._openOnClose = openOnClose;
        }

        private void UnblockUsers()
        {
        }

        public override void Open()
        {
            HUD.CloseAllCorners();
            this._opening = true;
            this.RebuildItemList();
            base.Open();
        }

        public void RebuildItemList()
        {
            this._selection = 0;
            this._topOffset = 0;
            this.items.Clear();
            foreach (ulong blockedPlayer in Options.Data.blockedPlayers)
                this.items.Add(new KeyValuePair<ulong, bool>(blockedPlayer, true));
            foreach (ulong unblockedPlayer in Options.Data.unblockedPlayers)
                this.items.Add(new KeyValuePair<ulong, bool>(unblockedPlayer, false));
            bool flag = false;
            foreach (ulong recentPlayer in Options.Data.recentPlayers)
            {
                if (!Options.Data.blockedPlayers.Contains(recentPlayer) && !Options.Data.unblockedPlayers.Contains(recentPlayer))
                {
                    if (!flag)
                    {
                        flag = true;
                        this.items.Add(new KeyValuePair<ulong, bool>(0UL, false));
                    }
                    this.items.Add(new KeyValuePair<ulong, bool>(recentPlayer, Options.Data.blockedPlayers.Contains(recentPlayer)));
                }
            }
        }

        public override void Close()
        {
            HUD.CloseAllCorners();
            base.Close();
        }

        public override void Update()
        {
            if (this.open && !this._opening)
            {
                if (Input.Pressed("MENUUP") && this._selection > 0)
                {
                    --this._selection;
                    if (this.items[this._selection].Key == 0UL)
                        --this._selection;
                    if (this._selection < this._topOffset)
                        this._topOffset = this._selection;
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("MENUDOWN") && this._selection < this.items.Count - 1)
                {
                    ++this._selection;
                    if (this.items[this._selection].Key == 0UL)
                        ++this._selection;
                    if (this._selection > this._topOffset + this.kMaxInView)
                        ++this._topOffset;
                    SFX.Play("textLetter", 0.7f);
                }
                if (this._selection >= 0 && this._selection < this.items.Count && Input.Pressed("MENU1"))
                {
                    KeyValuePair<ulong, bool> keyValuePair = this.items[this._selection];
                    if (!keyValuePair.Value)
                    {
                        if (!Options.Data.blockedPlayers.Contains(keyValuePair.Key))
                            Options.Data.blockedPlayers.Add(keyValuePair.Key);
                        Options.Data.unblockedPlayers.Remove(keyValuePair.Key);
                        Options.Data.muteSettings[keyValuePair.Key] = "CHR";
                        SFX.Play("textLetter", 0.7f);
                        this.MakeDirty();
                    }
                    else if (keyValuePair.Value)
                    {
                        Options.Data.blockedPlayers.Remove(keyValuePair.Key);
                        if (!Options.Data.unblockedPlayers.Contains(keyValuePair.Key))
                            Options.Data.unblockedPlayers.Add(keyValuePair.Key);
                        Options.Data.muteSettings[keyValuePair.Key] = "";
                        SFX.Play("textLetter", 0.7f);
                        this.MakeDirty();
                    }
                }
                if (Input.Pressed("SELECT"))
                    Steam.OverlayOpenURL("http://steamcommunity.com/profiles/" + this.items[this._selection].Key.ToString());
                if (Input.Pressed("CANCEL"))
                {
                    if (this._openOnClose != null)
                        new UIMenuActionOpenMenu((UIComponent)this, (UIComponent)this._openOnClose).Activate();
                    else
                        new UIMenuActionCloseMenu((UIComponent)this).Activate();
                }
            }
            this._opening = false;
            base.Update();
        }

        private void MakeDirty()
        {
            foreach (Profile profile in Profiles.all)
                profile._blockStatusDirty = true;
            this.RebuildItemList();
            if (this._selection < this.items.Count)
                return;
            this._selection = this.items.Count;
        }

        public override void Draw()
        {
            if (this.open)
            {
                Vec2 vec2 = new Vec2(this.x - 124f, this.y - 56f);
                float y = 0.0f;
                int num1 = 0;
                int num2 = 0;
                if (this.items.Count == 0)
                    this._littleFont.Draw("No blocked users! Happy day!", vec2 + new Vec2(8f, y), Color.White, (Depth)0.5f);
                foreach (KeyValuePair<ulong, bool> keyValuePair in this.items)
                {
                    if (num1 < this._topOffset)
                    {
                        ++num1;
                    }
                    else
                    {
                        if (this._topOffset > 0)
                        {
                            this._downArrow.flipV = true;
                            Graphics.Draw(this._downArrow, this.x, vec2.y - 2f, (Depth)0.5f);
                        }
                        if (num2 > this.kMaxInView)
                        {
                            this._downArrow.flipV = false;
                            Graphics.Draw(this._downArrow, this.x, vec2.y + y, (Depth)0.5f);
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
                            text = num1 != this._selection ? " " + str2 : "@SELECTICON@" + str2;
                        }
                        if (keyValuePair.Value)
                            text = "|DGRED|" + text;
                        this._littleFont.Draw(text, vec2 + new Vec2(0.0f, y), Color.White, (Depth)0.5f);
                        y += 8f;
                        ++num1;
                        ++num2;
                    }
                }
                string text1 = "@CANCEL@BACK";
                if (this.items.Count > 0 && this._selection >= 0 && this._selection < this.items.Count)
                    text1 = (!this.items[this._selection].Value ? text1 + " @MENU1@BLOCK" : text1 + " @MENU1@UN-BLOCK") + " @SELECT@@STEAMICON@";
                this._littleFont.Draw(text1, new Vec2(this.x - this._littleFont.GetWidth(text1) / 2f, this.y + 64f), Color.White, (Depth)0.5f);
            }
            base.Draw();
        }
    }
}

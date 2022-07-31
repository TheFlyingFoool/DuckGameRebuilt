// Decompiled with JetBrains decompiler
// Type: DuckGame.UIConnectionInfo
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIConnectionInfo : UIMenuItem
    {
        private UIMenu _kickMenu;
        private UIMenu _banMenu;
        private UIMenu _blockMenu;
        private UIMenu _rootMenu;
        private Profile _profile;
        private BitmapFont _littleFont;
        private string _nameText;
        private string _nameTextWithoutColor;
        //private string _name;
        private bool _showKickMenu;
        private bool _showMuteMenu;
        private int _additionalOptionIndex;
        private int _muteOptionIndex;
        private int _aoKickIndex;
        private int _aoBanIndex;
        private int _aoMuteIndex;
        private int _aoBlockIndex;
        private List<string> _additionalOptions = new List<string>()
    {
      "Kick",
      "Ban",
      "Mute",
      "Block"
    };
        private List<string> _muteOptions = new List<string>()
    {
      "Chat",
      "Hats",
      "Room",
      "Name"
    };

        public UIConnectionInfo(
          Profile p,
          UIMenu rootMenu,
          UIMenu kickMenu,
          UIMenu banMenu,
          UIMenu blockMenu)
          : base(p.nameUI)
        {
            this._profile = p;
            this._littleFont = new BitmapFont("smallBiosFontUI", 7, 5);
            this.SetFont(this._littleFont);
            this.UpdateName();
            this._kickMenu = kickMenu;
            this._banMenu = banMenu;
            this._blockMenu = blockMenu;
            this._rootMenu = rootMenu;
        }

        private void UpdateName()
        {
            Profile profile = this._profile;
            string str1 = "|" + profile.persona.colorUsable.r.ToString() + "," + profile.persona.colorUsable.g.ToString() + "," + profile.persona.colorUsable.b.ToString() + "|";
            if (profile.slotType == SlotType.Spectator)
                str1 = "|DGPURPLE|";
            string source1 = profile.nameUI;
            int num1 = source1.Count<char>();
            bool flag = false;
            if (profile.connection != null && profile.connection.isHost)
            {
                flag = true;
                ++num1;
            }
            int num2 = 17;
            if (flag)
                num2 = 16;
            if (num1 > num2)
            {
                source1 = source1.Substring(0, num2 - 1) + ".";
                num1 = num2;
            }
            for (; num1 < num2 + 2; ++num1)
                source1 += " ";
            if (flag)
                source1 = "@HOSTCROWN@" + source1;
            if (profile.slotType == SlotType.Spectator)
                source1 = "@SPECTATOR@" + source1;
            if (this._profile.muteChat || this._profile.muteHat || this._profile.muteName || this._profile.muteRoom)
                source1 = "@MUTEICON@" + source1;
            if (this._profile.blocked)
                source1 = "@BLOCKICON@" + source1;
            this._nameTextWithoutColor = source1;
            this._nameText = str1 + source1;
            int ping = this.GetPing();
            string source2 = ping.ToString() + "|WHITE|MS";
            int num3 = source2.Count<char>();
            string str2 = ping >= 150 ? (ping >= 250 ? (this._profile.connection == null ? "|DGRED|" + source2 + "@SIGNALDEAD@" : "|DGRED|" + source2 + "@SIGNALBAD@") : "|DGYELLOW|" + source2 + "@SIGNALNORMAL@") : "|DGGREEN|" + source2 + "@SIGNALGOOD@";
            for (; num3 < 5; ++num3)
                str2 = " " + str2;
            this._textElement.text = str1 + source1 + str2;
            this.controlString = "";
            if (profile.connection != DuckNetwork.localConnection)
            {
                if (Network.isServer)
                {
                    if (profile.blocked)
                        this._additionalOptions = new List<string>()
            {
              "Kick",
              "Ban",
              "Mute..",
              "|DGRED|Un-block"
            };
                    else
                        this._additionalOptions = new List<string>()
            {
              "Kick",
              "Ban",
              "Mute..",
              "|DGRED|Block"
            };
                    this._aoKickIndex = 0;
                    this._aoBanIndex = 1;
                    this._aoMuteIndex = 2;
                    this._aoBlockIndex = 3;
                    this.controlString = "@MENU2@KICK..";
                }
                else
                {
                    if (profile.blocked)
                        this._additionalOptions = new List<string>()
            {
              "Mute..",
              "|DGRED|Un-block"
            };
                    else
                        this._additionalOptions = new List<string>()
            {
              "Mute..",
              "|DGRED|Block"
            };
                    this._aoKickIndex = 99;
                    this._aoBanIndex = 99;
                    this._aoMuteIndex = 0;
                    this._aoBlockIndex = 1;
                    this.controlString = "@MENU2@MUTE..";
                }
            }
            if (Network.canSetObservers)
            {
                if (profile.slotType == SlotType.Spectator)
                    this.controlString += " @MENU1@PLAYER";
                else
                    this.controlString += " @MENU1@SPECTATOR";
            }
            if (!(profile.connection.data is User) && !NetworkDebugger.enabled)
                return;
            this.controlString += " @SELECT@@STEAMICON@";
        }

        public override void Activate(string trigger)
        {
            if (this._showKickMenu)
                return;
            if (this._profile.connection != null)
            {
                if (this._profile.connection != DuckNetwork.localConnection && trigger == "MENU2")
                {
                    this._additionalOptionIndex = 0;
                    this._showKickMenu = true;
                    UIMenu.globalUILock = true;
                    HUD.ClearCorners();
                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@SELECT@SELECT");
                    HUD.AddCornerControl(HUDCorner.BottomRight, "@CANCEL@BACK");
                }
                else if (Network.isServer && trigger == "MENU1" && Network.canSetObservers && this._profile.readyForSpectatorChange)
                {
                    if (this._profile.slotType != SlotType.Spectator)
                    {
                        DuckNetwork.MakeSpectator(this._profile);
                        SFX.Play("menuBlip01");
                        this.UpdateName();
                    }
                    else
                    {
                        DuckNetwork.MakePlayer(this._profile);
                        SFX.Play("menuBlip01");
                        this.UpdateName();
                    }
                }
                if (trigger == "SELECT")
                {
                    if (this._profile.connection.data is User)
                        Steam.OverlayOpenURL("http://steamcommunity.com/profiles/" + (this._profile.connection.data as User).id.ToString());
                    else if (NetworkDebugger.enabled && Steam.user != null)
                        Steam.OverlayOpenURL("http://steamcommunity.com/profiles/" + Steam.user.id.ToString());
                }
            }
            base.Activate(trigger);
        }

        public int GetPing()
        {
            int ping;
            if (this._profile.connection != null)
            {
                ping = this._profile.connection != DuckNetwork.localConnection ? (int)Math.Round(this._profile.connection.manager.ping * 1000.0) : 0;
                int status = (int)this._profile.connection.status;
            }
            else
                ping = 1000;
            return ping;
        }

        public override void Update()
        {
            if (this._showMuteMenu)
            {
                if (Input.Pressed("CANCEL"))
                    this._showMuteMenu = false;
                else if (Input.Pressed("UP"))
                {
                    if (this._muteOptionIndex > 0)
                    {
                        --this._muteOptionIndex;
                        SFX.Play("textLetter", 0.7f);
                    }
                }
                else if (Input.Pressed("DOWN"))
                {
                    if (this._muteOptionIndex < this._muteOptions.Count - 1)
                    {
                        ++this._muteOptionIndex;
                        SFX.Play("textLetter", 0.7f);
                    }
                }
                else if (Input.Pressed("SELECT"))
                {
                    if (this._muteOptionIndex == 0)
                        this._profile.muteChat = !this._profile.muteChat;
                    else if (this._muteOptionIndex == 1)
                        this._profile.muteHat = !this._profile.muteHat;
                    else if (this._muteOptionIndex == 2)
                        this._profile.muteRoom = !this._profile.muteRoom;
                    else if (this._muteOptionIndex == 3)
                        this._profile.muteName = !this._profile.muteName;
                    this._profile._blockStatusDirty = true;
                    SFX.Play("textLetter", 0.7f);
                }
            }
            else if (this._showKickMenu)
            {
                if (Input.Pressed("CANCEL"))
                {
                    HUD.ClearCorners();
                    this._showKickMenu = false;
                    UIMenu.globalUILock = false;
                    Options.Save();
                }
                else if (Input.Pressed("SELECT"))
                {
                    DuckNetwork.core.kickContext = this._profile;
                    bool flag = true;
                    if (this._additionalOptionIndex == this._aoKickIndex)
                    {
                        this._rootMenu.Close();
                        this._kickMenu.Open();
                        if (MonoMain.pauseMenu == this._rootMenu)
                            MonoMain.pauseMenu = _kickMenu;
                    }
                    else if (this._additionalOptionIndex == this._aoBanIndex)
                    {
                        this._rootMenu.Close();
                        this._banMenu.Open();
                        if (MonoMain.pauseMenu == this._rootMenu)
                            MonoMain.pauseMenu = _banMenu;
                    }
                    else if (this._additionalOptionIndex == this._aoMuteIndex)
                    {
                        this._showMuteMenu = true;
                        flag = false;
                    }
                    else if (this._additionalOptionIndex == this._aoBlockIndex)
                    {
                        if (this._profile.blocked)
                        {
                            DuckNetwork.UnblockPlayer(this._profile);
                            flag = true;
                            this.UpdateName();
                        }
                        else
                        {
                            this._rootMenu.Close();
                            this._blockMenu.Open();
                            if (MonoMain.pauseMenu == this._rootMenu)
                                MonoMain.pauseMenu = _blockMenu;
                        }
                    }
                    if (flag)
                    {
                        HUD.ClearCorners();
                        this._showKickMenu = false;
                        UIMenu.globalUILock = false;
                    }
                }
                else if (Input.Pressed("UP"))
                {
                    if (this._additionalOptionIndex > 0)
                    {
                        --this._additionalOptionIndex;
                        SFX.Play("textLetter", 0.7f);
                    }
                }
                else if (Input.Pressed("DOWN") && this._additionalOptionIndex < this._additionalOptions.Count - 1)
                {
                    ++this._additionalOptionIndex;
                    SFX.Play("textLetter", 0.7f);
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            this._textElement.text = "";
            this._littleFont.Draw(this._nameText, this.position + new Vec2(-88f, -3f), Color.White, this.depth + 10);
            int ping = this.GetPing();
            string source = ping.ToString() + "|WHITE|MS";
            source.Count<char>();
            string text = ping >= 150 ? (ping >= 250 ? (this._profile.connection == null ? "|DGRED|" + source + "@SIGNALDEAD@" : "|DGRED|" + source + "@SIGNALBAD@") : "|DGYELLOW|" + source + "@SIGNALNORMAL@") : "|DGGREEN|" + source + "@SIGNALGOOD@";
            this._littleFont.Draw(text, this.position + new Vec2(90f - this._littleFont.GetWidth(text), -3f), Color.White, this.depth + 10);
            if (this._showKickMenu)
            {
                Graphics.DrawRect(new Vec2(0f, 0f), new Vec2(Layer.HUD.width, Layer.HUD.height), Color.Black * 0.5f, (Depth)0.85f);
                Vec2 p1_1 = this.position + new Vec2(-60f, 4f);
                Vec2 p2_1 = p1_1 + new Vec2(76f, this._additionalOptions.Count * 8 + 3);
                Graphics.DrawRect(p1_1, p2_1, Color.Black, (Depth)0.9f);
                Graphics.DrawRect(p1_1, p2_1, Color.White, (Depth)0.9f, false);
                for (int index1 = 0; index1 < this._additionalOptions.Count; ++index1)
                {
                    this._littleFont.Draw(this._additionalOptions[index1], p1_1 + new Vec2(10f, 3 + index1 * 8), this._additionalOptionIndex == index1 ? Color.White : Color.White * 0.6f, (Depth)0.91f);
                    if (this._additionalOptionIndex == index1)
                        Graphics.Draw(this._arrow._image, p1_1.x + 4f, p1_1.y + 6f + index1 * 8, (Depth)0.91f);
                    if (index1 == this._aoMuteIndex && this._showMuteMenu)
                    {
                        Graphics.DrawRect(new Vec2(0f, 0f), new Vec2(Layer.HUD.width, Layer.HUD.height), Color.Black * 0.5f, (Depth)0.92f);
                        Vec2 p1_2 = p1_1 + new Vec2(8f, 26f);
                        Vec2 p2_2 = p1_2 + new Vec2(60f, this._muteOptions.Count * 8 + 4);
                        Graphics.DrawRect(p1_2, p2_2, Color.Black, (Depth)0.93f);
                        Graphics.DrawRect(p1_2, p2_2, Color.White, (Depth)0.93f, false);
                        for (int index2 = 0; index2 < this._muteOptions.Count; ++index2)
                        {
                            string muteOption = this._muteOptions[index2];
                            this._littleFont.Draw(index2 != 0 || !this._profile.muteChat ? (index2 != 1 || !this._profile.muteHat ? (index2 != 2 || !this._profile.muteRoom ? (index2 != 3 || !this._profile.muteName ? "@DELETEFLAG_OFF@" + muteOption : "@DELETEFLAG_ON@" + muteOption) : "@DELETEFLAG_ON@" + muteOption) : "@DELETEFLAG_ON@" + muteOption) : "@DELETEFLAG_ON@" + muteOption, p1_2 + new Vec2(10f, 4 + index2 * 8), this._muteOptionIndex == index2 ? Color.White : Color.White * 0.6f, (Depth)0.94f);
                            if (this._muteOptionIndex == index2)
                                Graphics.Draw(this._arrow._image, p1_2.x + 4f, p1_2.y + 6f + index2 * 8, (Depth)0.94f);
                        }
                    }
                }
            }
            base.Draw();
        }
    }
}

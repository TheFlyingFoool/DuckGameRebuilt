// Decompiled with JetBrains decompiler
// Type: DuckGame.UIConnectionInfo
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            _profile = p;
            _littleFont = new BitmapFont("smallBiosFontUI", 7, 5);
            SetFont(_littleFont);
            UpdateName();
            _kickMenu = kickMenu;
            _banMenu = banMenu;
            _blockMenu = blockMenu;
            _rootMenu = rootMenu;
        }

        private void UpdateName()
        {
            Profile profile = _profile;
            string colorPrefixString = "|" + profile.persona.colorUsable.r.ToString() + "," + profile.persona.colorUsable.g.ToString() + "," + profile.persona.colorUsable.b.ToString() + "|";
            if (profile.slotType == SlotType.Spectator)
                colorPrefixString = "|DGPURPLE|";
            string profileName = profile.nameUI;

            int colorTagsLength = profileName.Length - Program.RemoveColorTags(profileName).Length;
            
            int nameLength = profileName.Length - colorTagsLength;
            bool isHost = false;
            if (profile.connection != null && profile.connection.isHost)
            {
                isHost = true;
                ++nameLength;
            }

            int nameLengthLimit = 17;
            if (isHost)
                nameLengthLimit--;
            if (nameLength > nameLengthLimit)
            {
                profileName = profileName.Substring(0, nameLengthLimit - 1 + colorTagsLength) + $"{colorPrefixString}.";
                nameLength = nameLengthLimit;
            }
            for (; nameLength < nameLengthLimit + 2; ++nameLength)
                profileName += " ";

            if (isHost)
                profileName = "@HOSTCROWN@" + profileName;
            if (profile.slotType == SlotType.Spectator)
                profileName = "@SPECTATOR@" + profileName;
            if (_profile.muteChat || _profile.muteHat || _profile.muteName || _profile.muteRoom)
                profileName = "@MUTEICON@" + profileName;
            if (_profile.blocked)
                profileName = "@BLOCKICON@" + profileName;
            _nameTextWithoutColor = profileName;
            _nameText = colorPrefixString + profileName;
            int ping = GetPing();
            string source2 = ping.ToString() + "|WHITE|MS";
            int num3 = source2.Count();
            string str2 = ping >= 150 ? (ping >= 250 ? (_profile.connection == null ? "|DGRED|" + source2 + "@SIGNALDEAD@" : "|DGRED|" + source2 + "@SIGNALBAD@") : "|DGYELLOW|" + source2 + "@SIGNALNORMAL@") : "|DGGREEN|" + source2 + "@SIGNALGOOD@";
            for (; num3 < 5; ++num3)
                str2 = " " + str2;
            _textElement.text = colorPrefixString + profileName + str2;
            controlString = "";
            if (profile.connection != DuckNetwork.localConnection)
            {
                if (Network.isServer)
                {
                    if (profile.blocked)
                        _additionalOptions = new List<string>()
            {
              "Kick",
              "Ban",
              "Mute..",
              "|DGRED|Un-block"
            };
                    else
                        _additionalOptions = new List<string>()
            {
              "Kick",
              "Ban",
              "Mute..",
              "|DGRED|Block"
            };
                    _aoKickIndex = 0;
                    _aoBanIndex = 1;
                    _aoMuteIndex = 2;
                    _aoBlockIndex = 3;
                    controlString = "@MENU2@KICK..";
                }
                else
                {
                    if (profile.blocked)
                        _additionalOptions = new List<string>()
            {
              "Mute..",
              "|DGRED|Un-block"
            };
                    else
                        _additionalOptions = new List<string>()
            {
              "Mute..",
              "|DGRED|Block"
            };
                    _aoKickIndex = 99;
                    _aoBanIndex = 99;
                    _aoMuteIndex = 0;
                    _aoBlockIndex = 1;
                    controlString = "@MENU2@MUTE..";
                }
            }
            if (Network.canSetObservers)
            {
                if (profile.slotType == SlotType.Spectator)
                    controlString += " @MENU1@PLAYER";
                else
                    controlString += " @MENU1@SPECTATOR";
            }
            if (!(profile.connection.data is User) && !NetworkDebugger.enabled)
                return;
            controlString += " @SELECT@@STEAMICON@";
        }

        public override void Activate(string trigger)
        {
            if (_showKickMenu)
                return;
            if (_profile.connection != null)
            {
                if (_profile.connection != DuckNetwork.localConnection && trigger == Triggers.Menu2)
                {
                    _additionalOptionIndex = 0;
                    _showKickMenu = true;
                    UIMenu.globalUILock = true;
                    HUD.ClearCorners();
                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@SELECT@SELECT");
                    HUD.AddCornerControl(HUDCorner.BottomRight, "@CANCEL@BACK");
                }
                else if (Network.isServer && trigger == Triggers.Menu1 && Network.canSetObservers && _profile.readyForSpectatorChange)
                {
                    if (_profile.slotType != SlotType.Spectator)
                    {
                        DuckNetwork.MakeSpectator(_profile);
                        SFX.Play("menuBlip01");
                        UpdateName();
                    }
                    else
                    {
                        DuckNetwork.MakePlayer(_profile);
                        SFX.Play("menuBlip01");
                        UpdateName();
                    }
                }
                if (trigger == Triggers.Select)
                {
                    if (_profile.connection.data is User)
                        Steam.OverlayOpenURL("http://steamcommunity.com/profiles/" + (_profile.connection.data as User).id.ToString());
                    else if (NetworkDebugger.enabled && Steam.user != null)
                        Steam.OverlayOpenURL("http://steamcommunity.com/profiles/" + Steam.user.id.ToString());
                }
            }
            base.Activate(trigger);
        }

        public int GetPing()
        {
            int ping;
            if (_profile.connection != null)
            {
                ping = _profile.connection != DuckNetwork.localConnection ? (int)Math.Round(_profile.connection.manager.ping * 1000.0) : 0;
                int status = (int)_profile.connection.status;
            }
            else
                ping = 1000;
            return ping;
        }

        public override void Update()
        {
            if (_showMuteMenu)
            {
                if (Input.Pressed(Triggers.Cancel))
                    _showMuteMenu = false;
                else if (Input.Pressed(Triggers.Up))
                {
                    if (_muteOptionIndex > 0)
                    {
                        --_muteOptionIndex;
                        SFX.Play("textLetter", 0.7f);
                    }
                }
                else if (Input.Pressed(Triggers.Down))
                {
                    if (_muteOptionIndex < _muteOptions.Count - 1)
                    {
                        ++_muteOptionIndex;
                        SFX.Play("textLetter", 0.7f);
                    }
                }
                else if (Input.Pressed(Triggers.Select))
                {
                    if (_muteOptionIndex == 0)
                        _profile.muteChat = !_profile.muteChat;
                    else if (_muteOptionIndex == 1)
                        _profile.muteHat = !_profile.muteHat;
                    else if (_muteOptionIndex == 2)
                        _profile.muteRoom = !_profile.muteRoom;
                    else if (_muteOptionIndex == 3)
                        _profile.muteName = !_profile.muteName;
                    _profile._blockStatusDirty = true;
                    SFX.Play("textLetter", 0.7f);
                }
            }
            else if (_showKickMenu)
            {
                if (Input.Pressed(Triggers.Cancel))
                {
                    HUD.ClearCorners();
                    _showKickMenu = false;
                    UIMenu.globalUILock = false;
                    Options.Save();
                }
                else if (Input.Pressed(Triggers.Select))
                {
                    DuckNetwork.core.kickContext = _profile;
                    bool flag = true;
                    if (_additionalOptionIndex == _aoKickIndex)
                    {
                        _rootMenu.Close();
                        _kickMenu.Open();
                        if (MonoMain.pauseMenu == _rootMenu)
                            MonoMain.pauseMenu = _kickMenu;
                    }
                    else if (_additionalOptionIndex == _aoBanIndex)
                    {
                        _rootMenu.Close();
                        _banMenu.Open();
                        if (MonoMain.pauseMenu == _rootMenu)
                            MonoMain.pauseMenu = _banMenu;
                    }
                    else if (_additionalOptionIndex == _aoMuteIndex)
                    {
                        _showMuteMenu = true;
                        flag = false;
                    }
                    else if (_additionalOptionIndex == _aoBlockIndex)
                    {
                        if (_profile.blocked)
                        {
                            DuckNetwork.UnblockPlayer(_profile);
                            flag = true;
                            UpdateName();
                        }
                        else
                        {
                            _rootMenu.Close();
                            _blockMenu.Open();
                            if (MonoMain.pauseMenu == _rootMenu)
                                MonoMain.pauseMenu = _blockMenu;
                        }
                    }
                    if (flag)
                    {
                        HUD.ClearCorners();
                        _showKickMenu = false;
                        UIMenu.globalUILock = false;
                    }
                }
                else if (Input.Pressed(Triggers.Up))
                {
                    if (_additionalOptionIndex > 0)
                    {
                        --_additionalOptionIndex;
                        SFX.Play("textLetter", 0.7f);
                    }
                }
                else if (Input.Pressed(Triggers.Down) && _additionalOptionIndex < _additionalOptions.Count - 1)
                {
                    ++_additionalOptionIndex;
                    SFX.Play("textLetter", 0.7f);
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            _textElement.text = "";
            _littleFont.Draw(_nameText, position + new Vec2(-88f, -3f), Color.White, depth + 10);
            int ping = GetPing();
            string source = ping.ToString() + "|WHITE|MS";
            source.Count();
            string text = ping >= 150 ? (ping >= 250 ? (_profile.connection == null ? "|DGRED|" + source + "@SIGNALDEAD@" : "|DGRED|" + source + "@SIGNALBAD@") : "|DGYELLOW|" + source + "@SIGNALNORMAL@") : "|DGGREEN|" + source + "@SIGNALGOOD@";
            _littleFont.Draw(text, position + new Vec2(90f - _littleFont.GetWidth(text), -3f), Color.White, depth + 10);
            if (_showKickMenu)
            {
                Graphics.DrawRect(new Vec2(0f, 0f), new Vec2(Layer.HUD.width, Layer.HUD.height), Color.Black * 0.5f, (Depth)0.85f);
                Vec2 p1_1 = position + new Vec2(-60f, 4f);
                Vec2 p2_1 = p1_1 + new Vec2(76f, _additionalOptions.Count * 8 + 3);
                Graphics.DrawRect(p1_1, p2_1, Color.Black, (Depth)0.9f);
                Graphics.DrawRect(p1_1, p2_1, Color.White, (Depth)0.9f, false);
                for (int index1 = 0; index1 < _additionalOptions.Count; ++index1)
                {
                    _littleFont.Draw(_additionalOptions[index1], p1_1 + new Vec2(10f, 3 + index1 * 8), _additionalOptionIndex == index1 ? Color.White : Color.White * 0.6f, (Depth)0.91f);
                    if (_additionalOptionIndex == index1)
                        Graphics.Draw(_arrow._image, p1_1.x + 4f, p1_1.y + 6f + index1 * 8, (Depth)0.91f);
                    if (index1 == _aoMuteIndex && _showMuteMenu)
                    {
                        Graphics.DrawRect(new Vec2(0f, 0f), new Vec2(Layer.HUD.width, Layer.HUD.height), Color.Black * 0.5f, (Depth)0.92f);
                        Vec2 p1_2 = p1_1 + new Vec2(8f, 26f);
                        Vec2 p2_2 = p1_2 + new Vec2(60f, _muteOptions.Count * 8 + 4);
                        Graphics.DrawRect(p1_2, p2_2, Color.Black, (Depth)0.93f);
                        Graphics.DrawRect(p1_2, p2_2, Color.White, (Depth)0.93f, false);
                        for (int index2 = 0; index2 < _muteOptions.Count; ++index2)
                        {
                            string muteOption = _muteOptions[index2];
                            _littleFont.Draw(index2 != 0 || !_profile.muteChat ? (index2 != 1 || !_profile.muteHat ? (index2 != 2 || !_profile.muteRoom ? (index2 != 3 || !_profile.muteName ? "@DELETEFLAG_OFF@" + muteOption : "@DELETEFLAG_ON@" + muteOption) : "@DELETEFLAG_ON@" + muteOption) : "@DELETEFLAG_ON@" + muteOption) : "@DELETEFLAG_ON@" + muteOption, p1_2 + new Vec2(10f, 4 + index2 * 8), _muteOptionIndex == index2 ? Color.White : Color.White * 0.6f, (Depth)0.94f);
                            if (_muteOptionIndex == index2)
                                Graphics.Draw(_arrow._image, p1_2.x + 4f, p1_2.y + 6f + index2 * 8, (Depth)0.94f);
                        }
                    }
                }
            }
            base.Draw();
        }
    }
}

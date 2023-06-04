// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckNetworkCore
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class DuckNetworkCore
    {
        public List<MatchSetting> matchSettings = new List<MatchSetting>()
        {
            new MatchSetting()
            {
                id = "requiredwins",
                name = "Required Wins",
                value = 10,
                min = 1,
                max = 255,
                step = 5,
                altStep = 1,
                stepMap = new Dictionary<int, int>()
                {
                    {
                        2,
                        4
                    },
                    {
                        50,
                        5
                    },
                    {
                        100,
                        10
                    }
                }
            },
            new MatchSetting()
            {
                id = "restsevery",
                name = "Rests Every",
                value = 10,
                min = 1,
                max = 255,
                step = 5,
                altStep = 1,
                stepMap = new Dictionary<int, int>()
                {
                    {
                        2,
                        4
                    },
                    {
                        50,
                        5
                    },
                    {
                        100,
                        10
                    }
                }
            },
            new MatchSetting()
            {
                id = "wallmode",
                name = "Wall Mode",
                value = false
            },
            new MatchSetting()
            {
                id = "normalmaps",
                name = "@NORMALICON@|DGBLUE|Normal Levels",
                value = 90,
                suffix = "%",
                min = 0,
                max = 100,
                step = 5,
                altStep = 1,
                percentageLinks = new List<string>()
                {
                    "randommaps",
                    "custommaps",
                    "workshopmaps"
                },
                stepMap = new Dictionary<int, int>()
                {
                    { 100, 5 }
                }
            },
            new MatchSetting()
            {
                id = "randommaps",
                name = "@RANDOMICON@|DGBLUE|Random Levels",
                value = 10,
                suffix = "%",
                min = 0,
                max = 100,
                step = 5,
                altStep = 1,
                percentageLinks = new List<string>()
                {
                    "normalmaps",
                    "workshopmaps",
                    "custommaps"
                },
                stepMap = new Dictionary<int, int>()
                {
                    { 100, 5 }
                }
            },
            new MatchSetting()
            {
                id = "custommaps",
                name = "@CUSTOMICON@|DGBLUE|Custom Levels",
                value = 0,
                suffix = "%",
                min = 0,
                max = 100,
                step = 5,
                altStep = 1,
                percentageLinks = new List<string>()
                {
                    "normalmaps",
                    "randommaps",
                    "workshopmaps"
                },
                stepMap = new Dictionary<int, int>()
                {
                    { 100, 5 }
                }
            },
            new MatchSetting()
            {
                id = "workshopmaps",
                name = "@RAINBOWICON@|DGBLUE|Internet Levels",
                value = 0,
                suffix = "%",
                min = 0,
                max = 100,
                step = 5,
                altStep = 1,
                percentageLinks = new List<string>()
                {
                    "normalmaps",
                    "custommaps",
                    "randommaps"
                },
                stepMap = new Dictionary<int, int>()
                {
                    { 100, 5 }
                }
            },
            new MatchSetting()
            {
                id = "clientlevelsenabled",
                name = "Client Maps",
                value = false
            }
        };
        public List<MatchSetting> onlineSettings = new List<MatchSetting>()
        {
            new MatchSetting()
            {
                id = "maxplayers",
                name = "Max Players",
                value = 4,
                min = 2,
                max = 8,
                step = 1
            },
            new MatchSetting()
            {
                id = "teams",
                name = "Teams",
                value = false,
                filtered = false,
                filterOnly = true
            },
            new MatchSetting()
            {
                id = "customlevelsenabled",
                name = "Custom Levels",
                value = false,
                filtered = false,
                filterOnly = true
            },
            new MatchSetting()
            {
                id = "modifiers",
                name = "Modifiers",
                value = false,
                filtered = true,
                filterOnly = true
            },
            new MatchSetting()
            {
                id = "type",
                name = "Type",
                value = 2,
                min = 0,
                max = 3,
                createOnly = true,
                valueStrings = new List<string>()
                {
                    "PRIVATE",
                    "FRIENDS",
                    "PUBLIC",
                    "LAN"
                }
            },
            new MatchSetting()
            {
                id = "name",
                name = "Name",
                value = "",
                filtered = false,
                filterOnly = false,
                createOnly = true
            },
            new MatchSetting()
            {
                id = "password",
                name = "Password",
                value = "",
                filtered = false,
                filterOnly = false,
                createOnly = true
            },
            new MatchSetting()
            {
                id = "port",
                name = "Port",
                value = "1337",
                filtered = false,
                filterOnly = false,
                condition =  () => (int) TeamSelect2.GetOnlineSetting("type").value == 3
            },
            new MatchSetting()
            {
                id = "dedicated",
                name = "Dedicated",
                value = false,
                filtered = false,
                filterOnly = false,
                createOnly = true
            }
        };
        public Dictionary<string, XPPair> _xpEarned = new Dictionary<string, XPPair>();
        public int localDuckIndex; // for old mods
        public int levelTransferSize;
        public int levelTransferProgress;
        public int logTransferSize;
        public int logTransferProgress;
        public bool isDedicatedServer;
        public string serverPassword = "";
        public UIMenu xpMenu;
        public UIComponent ducknetUIGroup;
        public DuckNetwork.LobbyType lobbyType;
        public bool speedrunMode;
        public int speedrunMaxTrophy;
        public List<Profile> profiles = new List<Profile>();
        public List<Profile> profilesFixedOrder = new List<Profile>();
        public Profile localProfile;
        public Profile hostProfile;
        public List<NetMessage> pending = new List<NetMessage>();
        public MemoryStream compressedLevelData;
        public bool enteringText;
        public string currentEnterText = "";
        public int cursorFlash;
        public ushort chatIndex;
        public ushort levelTransferSession;
        public NetworkConnection localConnection = new NetworkConnection(null);
        public DuckNetStatus status;
        public FancyBitmapFont _builtInChatFont;
        public FancyBitmapFont _rasterChatFont;
        public bool initialized;
        public int randomID;
        public bool inGame;
        public bool stopEnteringText;
        public List<ChatMessage> chatMessages = new List<ChatMessage>();
        private int swearCharOffset;
        private string[] swearChars = new string[7]
        {
          "%",
          "+",
          "{",
          "}",
          "$",
          "!",
          "Z"
        };
        private string[] swearChars2 = new string[7]
        {
          "%",
          "+",
          "#",
          "$",
          "~",
          "!",
          "Z"
        };
        private int rainbowIndex;
        private string[] swearColors = new string[7]
        {
          "|RBOW_1|",
          "|RBOW_2|",
          "|RBOW_3|",
          "|RBOW_4|",
          "|RBOW_5|",
          "|RBOW_6|",
          "|RBOW_7|"
        };
        public static string filteredSpeech;
        public UIMenu _ducknetMenu;
        public UIMenu _optionsMenu;
        public UIMenu _confirmMenu;
        public UIMenu _confirmBlacklistMenu;
        public UIMenu _confirmBlock;
        public UIMenu _confirmReturnToLobby;
        public UIMenu _confirmKick;
        public UIMenu _confirmBan;
        public UIMenu _confirmEditSlots;
        public UIMenu _confirmMatchSettings;
        public MenuBoolean _quit = new MenuBoolean();
        public MenuBoolean _menuClosed = new MenuBoolean();
        public MenuBoolean _returnToLobby = new MenuBoolean();
        public UIMenu _levelSelectMenu;
        public Profile _menuOpenProfile;
        public Profile kickContext;
        public List<ulong> _invitedFriends = new List<ulong>();
        public MenuBoolean _inviteFriends = new MenuBoolean();
        public UIMenu _inviteMenu;
        public UIMenu _slotEditor;
        public UIMenu _lobbySettingMenu;
        public UIMenu _matchSettingMenu;
        public UIMenu _matchModifierMenu;
        public UIComponent _noModsUIGroup;
        public UIMenu _noModsMenu;
        public UIComponent _restartModsUIGroup;
        public UIMenu _restartModsMenu;
        public UIComponent _resUIGroup;
        public UIMenu _resMenu;
        public bool _pauseOpen;
        public string _settingsBeforeOpen = "";
        public bool _willOpenSettingsInfo;
        public int _willOpenSpectatorInfo;
        public bool startCountdown;
        public List<string> _activatedLevels = new List<string>();

        public void RecreateProfiles()
        {
            profiles.Clear();
            profilesFixedOrder.Clear();
            int num;
            for (int index = 0; index < DG.MaxPlayers; ++index)
            {
                num = index + 1;
                Profile profile = new Profile("Netduck" + num.ToString(), InputProfile.GetVirtualInput(index), varDefaultPersona: Persona.alllist[index], network: true);
                profile.SetNetworkIndex((byte)index);
                profile.SetFixedGhostIndex((byte)index);
                if (index > 3)
                    profile.Special_SetSlotType(SlotType.Closed);
                profiles.Add(profile);
                profilesFixedOrder.Add(profile);
            }
            for (int index = 0; index < DG.MaxSpectators; ++index)
            {
                num = index + 1;
                Profile profile = new Profile("Observer" + num.ToString(), InputProfile.GetVirtualInput(index + DG.MaxPlayers), varDefaultPersona: Persona.alllist[0], network: true);
                profile.SetNetworkIndex((byte)(index + DG.MaxPlayers));
                profile.SetFixedGhostIndex((byte)(index + DG.MaxPlayers));
                profile.slotType = SlotType.Spectator;
                profiles.Add(profile);
                profilesFixedOrder.Add(profile);
            }
        }

        public DuckNetworkCore()
        {
            RecreateProfiles();
            randomID = Rando.Int(2147483646);
        }

        public DuckNetworkCore(bool waitInit)
        {
            if (!waitInit)
                RecreateProfiles();
            randomID = Rando.Int(2147483646);
        }

        public void ReorderFixedList() => profilesFixedOrder = profiles.OrderBy(x => x.fixedGhostIndex).ToList();

        public FancyBitmapFont _chatFont => _rasterChatFont == null ? _builtInChatFont : _rasterChatFont;

        public string FilterText(string pText, User pUser)
        {
            if (Options.Data.languageFilter)
            {
                filteredSpeech = "";
                pText = pText.Replace("*", "@_sr_@");
                pText = Steam.FilterText(pText, pUser);
                swearCharOffset = 0;
                bool flag = false;
                string str1 = "";
                for (int index = 0; index < pText.Length; ++index)
                {
                    if (pText[index] == '*')
                    {
                        if (!flag)
                        {
                            flag = true;
                            filteredSpeech += "quack";
                        }
                        string str2 = str1 + swearColors[rainbowIndex];
                        rainbowIndex = (rainbowIndex + 1) % swearColors.Length;
                        if (_rasterChatFont == null)
                        {
                            str1 = str2 + swearChars[Rando.Int(swearChars.Length - 1)] + "|PREV|";
                            swearCharOffset = (swearCharOffset + 1) % swearChars2.Length;
                        }
                        else
                        {
                            str1 = str2 + swearChars2[Rando.Int(swearChars2.Length - 1)] + "|PREV|";
                            swearCharOffset = (swearCharOffset + 1) % swearChars2.Length;
                        }
                    }
                    else
                    {
                        flag = false;
                        swearCharOffset = 0;
                        str1 += pText[index].ToString();
                        filteredSpeech += pText[index].ToString();
                    }
                }
                pText = str1;
                pText = pText.Replace("@_sr_@", "*");
                filteredSpeech = filteredSpeech.Replace("@_sr_@", "*");
            }
            else
                filteredSpeech = pText;
            return pText;
        }

        public void AddChatMessage(ChatMessage pMessage)
        {
            if (pMessage.who == null)
                return;
            ChatMessage chatMessage = null;
            if (chatMessages.Count > 0)
                chatMessage = chatMessages[0];
            pMessage.text = FilterText(pMessage.text, null);
            if (Options.Data.textToSpeech)
            {
                if (Options.Data.textToSpeechReadNames)
                    SFX.Say(pMessage.who.nameUI + " says " + filteredSpeech);
                else
                    SFX.Say(filteredSpeech);
            }
            float chatScale = DuckNetwork.chatScale;
            _chatFont.scale = new Vec2(2f * pMessage.scale * chatScale);
            if (_chatFont is RasterFont)
            {
                FancyBitmapFont chatFont = _chatFont;
                chatFont.scale *= 0.5f;
            }
            try
            {
                pMessage.text = _chatFont.FormatWithNewlines(pMessage.text, 800f);
            }
            catch (Exception)
            {
                pMessage.text = "??????";
            }
            int num = pMessage.text.Count(x => x == '\n');
            if (chatMessage != null && num == 0 && chatMessage.newlines < 3 && chatMessage.timeout > 2 && chatMessage.who == pMessage.who)
            {
                pMessage.text = "|GRAY|" + pMessage.who.nameUIBodge + ": |BLACK|" + pMessage.text;
                chatMessage.timeout = 10f;
                chatMessage.text += "\n";
                chatMessage.text += pMessage.text;
                chatMessage.index = pMessage.index;
                chatMessage.slide = 0.5f;
                ++chatMessage.newlines;
            }
            else
            {
                pMessage.newlines = num + 1;
                pMessage.text = "|WHITE|" + pMessage.who.nameUIBodge + ": |BLACK|" + pMessage.text;
                chatMessages.Add(pMessage);
            }
            chatMessages = chatMessages.OrderBy(x => -x.index).ToList();
        }
    }
}

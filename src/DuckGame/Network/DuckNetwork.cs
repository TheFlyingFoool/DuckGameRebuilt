// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckNetwork
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DuckGame
{
    public class DuckNetwork
    {
        private static List<OnlineLevel> _levels = new List<OnlineLevel>()
    {
      new OnlineLevel() { num = 1, xpRequired = 0 },
      new OnlineLevel() { num = 2, xpRequired = 175 },
      new OnlineLevel() { num = 3, xpRequired = 400 },
      new OnlineLevel() { num = 4, xpRequired = 1200 },
      new OnlineLevel() { num = 5, xpRequired = 3500 },
      new OnlineLevel() { num = 6, xpRequired = 6500 },
      new OnlineLevel() { num = 7, xpRequired = 10000 },
      new OnlineLevel() { num = 8, xpRequired = 13000 },
      new OnlineLevel() { num = 9, xpRequired = 16000 },
      new OnlineLevel() { num = 10, xpRequired = 19000 },
      new OnlineLevel() { num = 11, xpRequired = 23000 },
      new OnlineLevel() { num = 12, xpRequired = 28000 },
      new OnlineLevel() { num = 13, xpRequired = 34000 },
      new OnlineLevel() { num = 14, xpRequired = 40000 },
      new OnlineLevel() { num = 15, xpRequired = 45000 },
      new OnlineLevel() { num = 16, xpRequired = 50000 },
      new OnlineLevel() { num = 17, xpRequired = 56000 },
      new OnlineLevel() { num = 18, xpRequired = 62000 },
      new OnlineLevel() { num = 19, xpRequired = 75000 },
      new OnlineLevel() { num = 20, xpRequired = 100000 }
    };
        public static int kills;
        public static int deaths;
        public static bool finishedMatch = false;
        private static DuckNetworkCore _core = new DuckNetworkCore();
        public static string compressedLevelName = null;
        public static int numSlots = 4;
        private static UIMenuActionCloseMenuCallFunction.Function _modsAcceptFunction;
        //private static UIMenu _ducknetMenu;
        private static UIComponent _uhOhGroup;
        private static UIMenu _uhOhMenu;
        public static bool invited;
        public static bool preparingProfiles;
        public static int joinPort = 0;
        private static List<Profile> _spectatorSwaps = new List<Profile>();
        private static List<NetworkConnection> _processedConnections = new List<NetworkConnection>();
        public static object potentialHostObject;
        private static string _currentTransferLevelName = null;
        public const string kServerIdentifier = "SERVER";
        public const string kServerLocalIdentifier = "SERVERLOCAL";

        public static void UpdateFont()
        {
            if (Options.Data.chatFont != "" && RasterFont.GetName(Options.Data.chatFont) != null)
            {
                DuckNetworkCore core = DuckNetwork._core;
                RasterFont rasterFont = new RasterFont(Options.Data.chatFont, Options.Data.chatFontSize)
                {
                    chatFont = true
                };
                core._rasterChatFont = rasterFont;
            }
            else
                DuckNetwork._core._rasterChatFont = null;
        }

        public static void Disconnect() => DuckNetwork._core.status = DuckNetStatus.Disconnecting;

        public static float chatScale => 1f;

        public static OnlineLevel GetLevel(int lev)
        {
            foreach (OnlineLevel level in DuckNetwork._levels)
            {
                if (level.num == lev)
                    return level;
            }
            return DuckNetwork._levels.Last<OnlineLevel>();
        }

        public static Dictionary<string, XPPair> _xpEarned
        {
            get => DuckNetwork._core._xpEarned;
            set => DuckNetwork._core._xpEarned = value;
        }

        public static void GiveXP(
          string category,
          int num,
          int xp,
          int type = 4,
          int firstCap = 9999999,
          int secondCap = 9999999,
          int finalCap = 9999999)
        {
            if (Profiles.experienceProfile == null || NetworkDebugger.enabled && DG.di != 0)
                return;
            if (!DuckNetwork._xpEarned.ContainsKey(category))
                DuckNetwork._xpEarned[category] = new XPPair();
            DuckNetwork._xpEarned[category].num += num;
            if (DuckNetwork._xpEarned[category].xp > secondCap)
                DuckNetwork._xpEarned[category].xp += xp / 4;
            else if (DuckNetwork._xpEarned[category].xp > firstCap)
                DuckNetwork._xpEarned[category].xp += xp / 2;
            else
                DuckNetwork._xpEarned[category].xp += xp;
            if (DuckNetwork._xpEarned[category].xp > finalCap)
                DuckNetwork._xpEarned[category].xp = finalCap;
            DuckNetwork._xpEarned[category].type = type;
        }

        private static UIMenu _xpMenu
        {
            get => DuckNetwork._core.xpMenu;
            set => DuckNetwork._core.xpMenu = value;
        }

        public static bool ShowUserXPGain()
        {
            if (!Level.core.gameFinished || DuckNetwork._xpEarned.Count <= 0)
                return false;
            DuckNetwork._xpMenu = new UILevelBox("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
            MonoMain.pauseMenu = _xpMenu;
            Level.core.gameFinished = false;
            return true;
        }

        public static KeyValuePair<string, XPPair> TakeXPStat()
        {
            if (DuckNetwork._xpEarned.Count == 0)
                return new KeyValuePair<string, XPPair>();
            KeyValuePair<string, XPPair> xpStat = DuckNetwork._xpEarned.ElementAt<KeyValuePair<string, XPPair>>(0);
            DuckNetwork._xpEarned.Remove(xpStat.Key);
            return xpStat;
        }

        public static int GetTotalXPEarned()
        {
            int totalXpEarned = 0;
            foreach (KeyValuePair<string, XPPair> keyValuePair in DuckNetwork._xpEarned)
                totalXpEarned += keyValuePair.Value.xp;
            return totalXpEarned;
        }

        public static DuckNetworkCore core
        {
            get => DuckNetwork._core;
            set => DuckNetwork._core = value;
        }

        public static NetworkConnection localConnection
        {
            get => DuckNetwork._core.localConnection;
            set => DuckNetwork._core.localConnection = value;
        }

        public static bool active => DuckNetwork._core.status != 0;

        public static byte levelIndex
        {
            get => DuckNetwork.localConnection.levelIndex;
            set => DuckNetwork.localConnection.levelIndex = value;
        }

        public static MemoryStream compressedLevelData
        {
            get => DuckNetwork._core.compressedLevelData;
            set => DuckNetwork._core.compressedLevelData = value;
        }

        public static List<Profile> profiles => DuckNetwork._core.profiles;

        public static List<Profile> profilesFixedOrder => DuckNetwork._core.profilesFixedOrder;

        public static Profile localProfile => DuckNetwork._core.localProfile;

        public static Profile hostProfile => DuckNetwork._core.hostProfile;

        public static int hostDuckIndex => DuckNetwork.hostProfile == null ? 0 : DuckNetwork.profiles.IndexOf(DuckNetwork.hostProfile);

        public static int localDuckIndex => DuckNetwork.localProfile == null ? 0 : DuckNetwork.profiles.IndexOf(DuckNetwork.localProfile);

        public static DuckNetStatus status => DuckNetwork._core.status;

        public static bool inGame
        {
            get => DuckNetwork._core.inGame;
            set => DuckNetwork._core.inGame = value;
        }

        public static bool enteringText => DuckNetwork._core.enteringText;

        private static UIComponent _ducknetUIGroup
        {
            get => DuckNetwork._core.ducknetUIGroup;
            set => DuckNetwork._core.ducknetUIGroup = value;
        }

        public static UIComponent duckNetUIGroup => DuckNetwork._ducknetUIGroup;

        public static void Initialize()
        {
            DuckNetwork._core._builtInChatFont = new FancyBitmapFont("smallFontChat")
            {
                chatFont = true
            };
            DuckNetwork._core.initialized = true;
        }

        public static void Kick(Profile p)
        {
            if (p.slotType == SlotType.Local)
            {
                DuckNetwork.SendToEveryone(new NMClientDisconnect(DuckNetwork.localConnection.identifier, p));
                DuckNetwork.ResetProfile(p, false);
                p.team = null;
                p.slotType = SlotType.Open;
                DuckNetwork.ChangeSlotSettings();
            }
            else
            {
                if (!Network.isServer || p == null || p.connection == null || p.connection == DuckNetwork.localConnection)
                    return;
                SFX.Play("little_punch");
                Send.Message(new NMKick(), p.connection);
                Send.Message(new NMKicked(p));
                DevConsole.Log(DCSection.DuckNet, "|DGRED|Kicking " + p.connection.ToString() + "...");
                p.connection.kicking = true;
                Network.activeNetwork.core.DisconnectClient(p.connection, new DuckNetErrorInfo(DuckNetError.Kicked, ""), true);
            }
        }

        public static void Ban(Profile p)
        {
            if (!Network.isServer || p == null || p.connection == null || p.connection == DuckNetwork.localConnection)
                return;
            SFX.Play("little_punch");
            Send.Message(new NMBan(), p.connection);
            Send.Message(new NMBanned(p));
            DevConsole.Log(DCSection.DuckNet, "|DGRED|Banning " + p.connection.ToString() + "...");
            p.connection.banned = true;
            p.connection.kicking = true;
            Network.activeNetwork.core.DisconnectClient(p.connection, new DuckNetErrorInfo(DuckNetError.Banned, ""), true);
        }

        private static bool ShouldKickForCustomContent() 
        {
            bool isActive = Network.isActive;
            return false;
        } //=> Network.isActive && ParentalControls.AreParentalControlsActive() && (int)TeamSelect2.GetMatchSetting("custommaps").value > 0 && TeamSelect2.customLevels > 0;

        public static void SetMatchSettings(
          bool initialSettings,
          int varWinsPerSet,
          int varRoundsPerIntermission,
          bool varTeams,
          bool varWallmode,
          int varNormalPercent,
          int varRandomPercent,
          int varWorkshopPercent,
          int varCustomPercent,
          int varCustomLevels,
          List<byte> enabledModifiers,
          bool varClientLevels)
        {
            TeamSelect2.GetMatchSetting("requiredwins").value = varWinsPerSet;
            TeamSelect2.GetMatchSetting("restsevery").value = varRoundsPerIntermission;
            TeamSelect2.GetMatchSetting("randommaps").value = varRandomPercent;
            TeamSelect2.GetMatchSetting("workshopmaps").value = varWorkshopPercent;
            TeamSelect2.GetMatchSetting("normalmaps").value = varNormalPercent;
            TeamSelect2.GetMatchSetting("custommaps").value = varCustomPercent;
            TeamSelect2.GetMatchSetting("wallmode").value = varWallmode;
            TeamSelect2.GetMatchSetting("clientlevelsenabled").value = varClientLevels;
            RockScoreboard.wallMode = varWallmode;
            TeamSelect2.GetOnlineSetting("teams").value = varTeams;
            TeamSelect2.prevCustomLevels = !initialSettings ? TeamSelect2.customLevels : varCustomLevels;
            TeamSelect2.customLevels = varCustomLevels;
            if (DuckNetwork.ShouldKickForCustomContent())
                Network.DisconnectClient(DuckNetwork.localConnection, new DuckNetErrorInfo(DuckNetError.ParentalControls, "Disconnected - Restricted Content"));
            int num1 = 0;
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                if (Unlocks.modifierToByte.ContainsKey(unlock.id))
                {
                    byte num2 = Unlocks.modifierToByte[unlock.id];
                    if (enabledModifiers.Contains(num2))
                    {
                        unlock.enabled = true;
                        ++num1;
                    }
                    else
                        unlock.enabled = false;
                    if (initialSettings)
                        unlock.prevEnabled = unlock.enabled;
                }
            }
            GameMode.roundsBetweenIntermission = varRoundsPerIntermission;
            GameMode.winsPerSet = varWinsPerSet;
            Deathmatch.userMapsPercent = varCustomPercent;
            TeamSelect2.randomMapPercent = varRandomPercent;
            TeamSelect2.normalMapPercent = varNormalPercent;
            TeamSelect2.workshopMapPercent = varWorkshopPercent;
            TeamSelect2.UpdateModifierStatus();
            if (!initialSettings)
                return;
            TeamSelect2.prevNumModifiers = num1;
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
                matchSetting.prevValue = matchSetting.value;
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
                onlineSetting.prevValue = onlineSetting.value;
        }

        public static DuckNetwork.LobbyType lobbyType => DuckNetwork._core.lobbyType;

        public static void ChangeSlotSettings() => DuckNetwork.ChangeSlotSettings(false);

        public static void ChangeSlotSettings(bool pInitializingClient)
        {
            DuckNetwork.numSlots = 0;
            DuckNetwork.LobbyType lobbyType = DuckNetwork.LobbyType.Private;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection != DuckNetwork.localConnection)
                {
                    if (profile.slotType == SlotType.Friend && lobbyType < DuckNetwork.LobbyType.FriendsOnly)
                        lobbyType = DuckNetwork.LobbyType.FriendsOnly;
                    if (profile.slotType == SlotType.Open && lobbyType < DuckNetwork.LobbyType.Public)
                        lobbyType = DuckNetwork.LobbyType.Public;
                }
                if (profile.slotType != SlotType.Closed && profile.slotType != SlotType.Spectator)
                    ++DuckNetwork.numSlots;
            }
            if (!Network.isServer)
                return;
            if (Steam.lobby != null)
            {
                if (Steam.lobby.type == SteamLobbyType.Private || Steam.lobby.type == SteamLobbyType.FriendsOnly)
                    DuckNetwork.invited = true;
                Steam.lobby.type = (SteamLobbyType)lobbyType;
                Steam.lobby.maxMembers = 32;
                Steam.lobby.SetLobbyData("numSlots", DuckNetwork.numSlots.ToString());
                TeamSelect2.GetOnlineSetting("maxplayers").value = numSlots;
                Steam.lobby.SetLobbyData("maxplayers", DuckNetwork.numSlots.ToString());
            }
            List<byte> pSlots = new List<byte>();
            for (int index = 0; index < DG.MaxPlayers; ++index)
                pSlots.Add((byte)DuckNetwork.profiles[index].slotType);
            Send.Message(new NMChangeSlots(pSlots, pInitializingClient));
        }

        public static void KickedPlayer()
        {
            if (DuckNetwork._core.kickContext == null)
                return;
            DuckNetwork.Kick(DuckNetwork._core.kickContext);
            DuckNetwork._core.kickContext = null;
        }

        public static void BannedPlayer()
        {
            if (DuckNetwork._core.kickContext == null)
                return;
            DuckNetwork.Ban(DuckNetwork._core.kickContext);
            DuckNetwork._core.kickContext = null;
        }

        public static void BlockedPlayer()
        {
            if (DuckNetwork._core.kickContext == null)
                return;
            if (Options.Data.blockedPlayers != null && !Options.Data.blockedPlayers.Contains(DuckNetwork._core.kickContext.steamID))
            {
                Options.Data.muteSettings[DuckNetwork._core.kickContext.steamID] = "CHR";
                Options.Data.blockedPlayers.Add(DuckNetwork._core.kickContext.steamID);
                Options.Data.unblockedPlayers.Remove(DuckNetwork._core.kickContext.steamID);
                Options.Save();
            }
            DuckNetwork._core.kickContext._blockStatusDirty = true;
            DuckNetwork.Ban(DuckNetwork._core.kickContext);
            DuckNetwork._core.kickContext = null;
        }

        public static void UnblockPlayer(Profile pProfile)
        {
            Options.Data.blockedPlayers.Remove(pProfile.steamID);
            if (!Options.Data.unblockedPlayers.Contains(pProfile.steamID))
                Options.Data.unblockedPlayers.Add(pProfile.steamID);
            Options.Data.muteSettings[pProfile.steamID] = "";
            pProfile._blockStatusDirty = true;
            SFX.Play("textLetter", 0.7f);
        }

        public static void ClosePauseMenu()
        {
            if (!Network.isActive || MonoMain.pauseMenu == null)
                return;
            MonoMain.pauseMenu.Close();
            MonoMain.pauseMenu = null;
            if (DuckNetwork._ducknetUIGroup == null)
                return;
            Level.Remove(_ducknetUIGroup);
            DuckNetwork._ducknetUIGroup = null;
        }

        public static void OpenMatchSettingsInfo() => DuckNetwork._core._willOpenSettingsInfo = true;

        public static void OpenSpectatorInfo(bool pSpectator) => DuckNetwork._core._willOpenSpectatorInfo = pSpectator ? 2 : 1;

        public static void OpenNoModsWindow(
          UIMenuActionCloseMenuCallFunction.Function acceptFunction)
        {
            float num1 = 320f;
            float num2 = 180f;
            DuckNetwork._core._noModsUIGroup = new UIComponent(num1 / 2f, num2 / 2f, 0.0f, 0.0f);
            DuckNetwork._core._noModsMenu = DuckNetwork.CreateNoModsOnlineWindow(acceptFunction);
            DuckNetwork._core._noModsUIGroup.Add(_core._noModsMenu, false);
            DuckNetwork._core._noModsUIGroup.Close();
            DuckNetwork._core._noModsUIGroup.Close();
            Level.Add(_core._noModsUIGroup);
            DuckNetwork._core._noModsUIGroup.Update();
            DuckNetwork._core._noModsUIGroup.Update();
            DuckNetwork._core._noModsUIGroup.Update();
            DuckNetwork._core._noModsUIGroup.Open();
            DuckNetwork._core._noModsMenu.Open();
            MonoMain.pauseMenu = DuckNetwork._core._noModsUIGroup;
            DuckNetwork._core._pauseOpen = true;
            SFX.Play("pause", 0.6f);
        }

        private static UIMenu CreateNoModsOnlineWindow(
          UIMenuActionCloseMenuCallFunction.Function acceptFunction)
        {
            DuckNetwork._modsAcceptFunction = acceptFunction;
            UIMenu modsOnlineWindow = new UIMenu("@LWING@YOU HAVE MODS ENABLED@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@BACK");
            BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIText component1 = new UIText("YOU WILL |DGRED|NOT|WHITE| BE ABLE TO PLAY", Color.White);
            component1.SetFont(f);
            modsOnlineWindow.Add(component1, true);
            UIText component2 = new UIText("ONLINE WITH ANYONE WHO DOES ", Color.White);
            component2.SetFont(f);
            modsOnlineWindow.Add(component2, true);
            UIText component3 = new UIText("NOT HAVE THE |DGRED|SAME MODS|WHITE|.     ", Color.White);
            component3.SetFont(f);
            modsOnlineWindow.Add(component3, true);
            UIText component4 = new UIText("", Color.White);
            component4.SetFont(f);
            modsOnlineWindow.Add(component4, true);
            UIText component5 = new UIText("WOULD YOU LIKE TO |DGGREEN|DISABLE|WHITE|   ", Color.White);
            component5.SetFont(f);
            modsOnlineWindow.Add(component5, true);
            UIText component6 = new UIText("MODS AND RESTART THE GAME?  ", Color.White);
            component6.SetFont(f);
            modsOnlineWindow.Add(component6, true);
            UIText component7 = new UIText("", Color.White);
            component7.SetFont(f);
            modsOnlineWindow.Add(component7, true);
            UIText component8 = new UIText("", Color.White);
            component8.SetFont(f);
            modsOnlineWindow.Add(component8, true);
            modsOnlineWindow.Add(new UIMenuItem("|DGGREEN|DISABLE MODS AND RESTART", new UIMenuActionCloseMenuCallFunction(DuckNetwork._core._noModsUIGroup, new UIMenuActionCloseMenuCallFunction.Function(ModLoader.DisableModsAndRestart)), c: Color.White), true);
            modsOnlineWindow.Add(new UIMenuItem("|DGYELLOW|I KNOW WHAT I'M DOING", new UIMenuActionCloseMenuCallFunction(DuckNetwork._core._noModsUIGroup, acceptFunction), c: Color.White), true);
            modsOnlineWindow.Add(new UIText(" ", Color.White), true);
            modsOnlineWindow.Add(new UIMenuItem("|DGPURPLE|STOP ASKING, I LOVE MODS!", new UIMenuActionCloseMenuCallFunction(DuckNetwork._core._noModsUIGroup, new UIMenuActionCloseMenuCallFunction.Function(DuckNetwork.QuitShowingModWindow)), c: Color.White), true);
            modsOnlineWindow.SetBackFunction(new UIMenuActionCloseMenu(DuckNetwork._core._noModsUIGroup));
            modsOnlineWindow.Close();
            return modsOnlineWindow;
        }

        public static void QuitShowingModWindow()
        {
            if (DuckNetwork._modsAcceptFunction == null)
                return;
            Options.Data.showNetworkModWarning = false;
            DuckNetwork._modsAcceptFunction();
            Options.Save();
        }

        public static UIComponent OpenModsRestartWindow(UIMenu openOnClose)
        {
            float num1 = 320f;
            float num2 = 180f;
            DuckNetwork._core._restartModsUIGroup = new UIComponent(num1 / 2f, num2 / 2f, 0.0f, 0.0f);
            DuckNetwork._core._restartModsMenu = DuckNetwork.CreateModsRestartWindow(openOnClose);
            DuckNetwork._core._restartModsUIGroup.Add(_core._restartModsMenu, false);
            DuckNetwork._core._restartModsUIGroup.Close();
            DuckNetwork._core._restartModsUIGroup.Close();
            Level.Add(_core._restartModsUIGroup);
            DuckNetwork._core._restartModsUIGroup.Update();
            DuckNetwork._core._restartModsUIGroup.Update();
            DuckNetwork._core._restartModsUIGroup.Update();
            DuckNetwork._core._restartModsUIGroup.Open();
            DuckNetwork._core._restartModsMenu.Open();
            MonoMain.pauseMenu = DuckNetwork._core._restartModsUIGroup;
            DuckNetwork._core._pauseOpen = true;
            SFX.Play("pause", 0.6f);
            return DuckNetwork._core._restartModsUIGroup;
        }

        public static UIComponent OpenResolutionRestartMenu(UIMenu openOnClose)
        {
            float num1 = 320f;
            float num2 = 180f;
            DuckNetwork._core._resUIGroup = new UIComponent(num1 / 2f, num2 / 2f, 0.0f, 0.0f);
            DuckNetwork._core._resMenu = DuckNetwork.CreateResolutionRestartWindow(openOnClose);
            DuckNetwork._core._resUIGroup.Add(_core._resMenu, false);
            DuckNetwork._core._resUIGroup.Close();
            DuckNetwork._core._resUIGroup.Close();
            Level.Add(_core._resUIGroup);
            DuckNetwork._core._resUIGroup.Update();
            DuckNetwork._core._resUIGroup.Update();
            DuckNetwork._core._resUIGroup.Update();
            DuckNetwork._core._resUIGroup.Open();
            DuckNetwork._core._resMenu.Open();
            MonoMain.pauseMenu = DuckNetwork._core._resUIGroup;
            DuckNetwork._core._pauseOpen = true;
            SFX.Play("pause", 0.6f);
            return DuckNetwork._core._restartModsUIGroup;
        }

        private static UIMenu CreateResolutionRestartWindow(UIMenu openOnClose)
        {
            UIMenu resolutionRestartWindow = new UIMenu("@LWING@GRAPHICS CHANGE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@BACK");
            BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIText component1 = new UIText("YOU NEED TO RESTART THE GAME", Color.White);
            component1.SetFont(f);
            resolutionRestartWindow.Add(component1, true);
            UIText component2 = new UIText("FOR CHANGES TO TAKE EFFECT. ", Color.White);
            component2.SetFont(f);
            resolutionRestartWindow.Add(component2, true);
            UIText component3 = new UIText("(ASPECT RATIO CHANGED)", Color.White);
            component3.SetFont(f);
            resolutionRestartWindow.Add(component3, true);
            UIText component4 = new UIText("", Color.White);
            component4.SetFont(f);
            resolutionRestartWindow.Add(component4, true);
            UIText component5 = new UIText("DO YOU WANT TO |DGGREEN|RESTART|WHITE| NOW? ", Color.White);
            component5.SetFont(f);
            resolutionRestartWindow.Add(component5, true);
            resolutionRestartWindow.Add(new UIMenuItem("|DGGREEN|RESTART NOW", new UIMenuActionCloseMenuCallFunction(DuckNetwork._core._resUIGroup, new UIMenuActionCloseMenuCallFunction.Function(ModLoader.RestartGame)), c: Color.White), true);
            resolutionRestartWindow.Add(new UIMenuItem("|DGYELLOW|RESTART LATER", new UIMenuActionOpenMenu(DuckNetwork._core._resUIGroup, openOnClose), c: Color.White), true);
            resolutionRestartWindow.Close();
            return resolutionRestartWindow;
        }

        private static UIMenu CreateModsRestartWindow(UIMenu openOnClose)
        {
            UIMenu modsRestartWindow = new UIMenu("@LWING@MODS CHANGED@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@BACK");
            BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIText component1 = new UIText("YOU NEED TO RESTART THE GAME", Color.White);
            component1.SetFont(f);
            modsRestartWindow.Add(component1, true);
            UIText component2 = new UIText("FOR CHANGES TO TAKE EFFECT. ", Color.White);
            component2.SetFont(f);
            modsRestartWindow.Add(component2, true);
            UIText component3 = new UIText("", Color.White);
            component3.SetFont(f);
            modsRestartWindow.Add(component3, true);
            UIText component4 = new UIText("DO YOU WANT TO |DGGREEN|RESTART|WHITE| NOW? ", Color.White);
            component4.SetFont(f);
            modsRestartWindow.Add(component4, true);
            modsRestartWindow.Add(new UIMenuItem("|DGGREEN|RESTART", new UIMenuActionCloseMenuCallFunction(DuckNetwork._core._restartModsUIGroup, new UIMenuActionCloseMenuCallFunction.Function(ModLoader.RestartGame)), c: Color.White), true);
            modsRestartWindow.Add(new UIMenuItem("|DGYELLOW|CONTINUE", new UIMenuActionOpenMenu(DuckNetwork._core._restartModsUIGroup, openOnClose), c: Color.White), true);
            modsRestartWindow.Close();
            return modsRestartWindow;
        }

        public static UIMenu CreateSpectatorWindow(bool isSpectator)
        {
            UIMenu spectatorWindow = new UIMenu("@LWING@SPECTATOR MODE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@BACK");
            BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
            if (isSpectator)
            {
                UIText component1 = new UIText("THE HOST HAS MADE YOU", Color.White);
                component1.SetFont(f);
                spectatorWindow.Add(component1, true);
                UIText component2 = new UIText("A SPECTATOR. YOU WILL", Color.White);
                component2.SetFont(f);
                spectatorWindow.Add(component2, true);
                UIText component3 = new UIText("BE ABLE TO WATCH AND CHAT", Color.White);
                component3.SetFont(f);
                spectatorWindow.Add(component3, true);
                UIText component4 = new UIText("BUT NOT PLAY.", Color.White);
                component4.SetFont(f);
                spectatorWindow.Add(component4, true);
            }
            else
            {
                UIText component5 = new UIText("THE HOST DISABLED SPECTATOR", Color.White);
                component5.SetFont(f);
                spectatorWindow.Add(component5, true);
                UIText component6 = new UIText("MODE. YOU WILL NOW BE ABLE", Color.White);
                component6.SetFont(f);
                spectatorWindow.Add(component6, true);
                UIText component7 = new UIText("TO PLAY.", Color.White);
                component7.SetFont(f);
                spectatorWindow.Add(component7, true);
            }
            UIText component = new UIText("", Color.White);
            component.SetFont(f);
            spectatorWindow.Add(component, true);
            spectatorWindow.SetBackFunction(new UIMenuActionCloseMenu(DuckNetwork._ducknetUIGroup));
            spectatorWindow.Close();
            return spectatorWindow;
        }

        private static UIMenu CreateMatchSettingsInfoWindow(UIMenu openOnClose = null)
        {
            UIMenu menu = openOnClose == null ? new UIMenu("@LWING@NEW SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK") : new UIMenu("@LWING@MATCH SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK");
            BitmapFont f = new BitmapFont("biosFontUI", 8, 7);
            new UIText("HOST CHANGED SETTINGS", Color.White).SetFont(f);
            int num1 = 16;
            int num2 = 5;
            MatchSetting onlineSetting = TeamSelect2.GetOnlineSetting("teams");
            string name1 = onlineSetting.name;
            string str1 = (bool)onlineSetting.value ? "ON" : "OFF";
            while (name1.Length < num1)
                name1 += " ";
            while (str1.Length < num2)
                str1 = " " + str1;
            string textVal1 = name1 + " " + str1;
            UIText component1 = onlineSetting.value.Equals(onlineSetting.prevValue) ? new UIText(textVal1, Colors.Silver) : new UIText(textVal1, Colors.DGBlue);
            onlineSetting.prevValue = onlineSetting.value;
            component1.SetFont(f);
            menu.Add(component1, true);
            MatchSetting matchSetting1 = TeamSelect2.GetMatchSetting("requiredwins");
            string name2 = matchSetting1.name;
            string str2 = matchSetting1.value.ToString();
            while (name2.Length < num1)
                name2 += " ";
            while (str2.Length < num2)
                str2 = " " + str2;
            string textVal2 = name2 + " " + str2;
            UIText component2 = matchSetting1.value.Equals(matchSetting1.prevValue) ? new UIText(textVal2, Colors.Silver) : new UIText(textVal2, Colors.DGBlue);
            matchSetting1.prevValue = matchSetting1.value;
            component2.SetFont(f);
            menu.Add(component2, true);
            MatchSetting matchSetting2 = TeamSelect2.GetMatchSetting("restsevery");
            string name3 = matchSetting2.name;
            string str3 = matchSetting2.value.ToString();
            while (name3.Length < num1)
                name3 += " ";
            while (str3.Length < num2)
                str3 = " " + str3;
            string textVal3 = name3 + " " + str3;
            UIText component3 = matchSetting2.value.Equals(matchSetting2.prevValue) ? new UIText(textVal3, Colors.Silver) : new UIText(textVal3, Colors.DGBlue);
            matchSetting2.prevValue = matchSetting2.value;
            component3.SetFont(f);
            menu.Add(component3, true);
            MatchSetting matchSetting3 = TeamSelect2.GetMatchSetting("wallmode");
            string name4 = matchSetting3.name;
            string str4 = (bool)matchSetting3.value ? "ON" : "OFF";
            while (name4.Length < num1)
                name4 += " ";
            while (str4.Length < num2)
                str4 = " " + str4;
            string textVal4 = name4 + " " + str4;
            UIText component4 = matchSetting3.value.Equals(matchSetting3.prevValue) ? new UIText(textVal4, Colors.Silver) : new UIText(textVal4, Colors.DGBlue);
            matchSetting3.prevValue = matchSetting3.value;
            component4.SetFont(f);
            menu.Add(component4, true);
            UIText component5 = new UIText(" ", Color.White);
            component5.SetFont(f);
            menu.Add(component5, true);
            MatchSetting matchSetting4 = TeamSelect2.GetMatchSetting("normalmaps");
            string name5 = matchSetting4.name;
            string str5 = matchSetting4.value.ToString() + "%";
            if (matchSetting4.minString != null && matchSetting4.value is int && (int)matchSetting4.value == matchSetting4.min)
                str5 = matchSetting4.minString;
            int startIndex1 = matchSetting4.name.LastIndexOf('|');
            for (string str6 = matchSetting4.name.Substring(startIndex1, matchSetting4.name.Count<char>() - startIndex1); str6.Length < num1; str6 += " ")
                name5 += " ";
            while (str5.Length < num2)
                str5 = " " + str5;
            string textVal5 = (name5 + " " + str5).Replace("|DGBLUE|", "");
            UIText component6 = matchSetting4.value.Equals(matchSetting4.prevValue) ? new UIText(textVal5, Colors.Silver) : new UIText(textVal5, Colors.DGBlue);
            matchSetting4.prevValue = matchSetting4.value;
            component6.SetFont(f);
            menu.Add(component6, true);
            MatchSetting matchSetting5 = TeamSelect2.GetMatchSetting("randommaps");
            string name6 = matchSetting5.name;
            string str7 = matchSetting5.value.ToString() + "%";
            int startIndex2 = matchSetting5.name.LastIndexOf('|');
            for (string str8 = matchSetting5.name.Substring(startIndex2, matchSetting5.name.Count<char>() - startIndex2); str8.Length < num1; str8 += " ")
                name6 += " ";
            while (str7.Length < num2)
                str7 = " " + str7;
            string textVal6 = (name6 + " " + str7).Replace("|DGBLUE|", "");
            UIText component7 = matchSetting5.value.Equals(matchSetting5.prevValue) ? new UIText(textVal6, Colors.Silver) : new UIText(textVal6, Colors.DGBlue);
            matchSetting5.prevValue = matchSetting5.value;
            component7.SetFont(f);
            menu.Add(component7, true);
            MatchSetting matchSetting6 = TeamSelect2.GetMatchSetting("custommaps"); //if (!ParentalControls.AreParentalControlsActive()) start of if
            string name7 = matchSetting6.name;
            string str9 = matchSetting6.value.ToString() + "%";
            if (matchSetting6.minString != null && matchSetting6.value is int && (int)matchSetting6.value == matchSetting6.min)
                str9 = matchSetting6.minString;
            int startIndex3 = matchSetting6.name.LastIndexOf('|');
            for (string str10 = matchSetting6.name.Substring(startIndex3, matchSetting6.name.Count<char>() - startIndex3); str10.Length < num1; str10 += " ")
                name7 += " ";
            while (str9.Length < num2)
                str9 = " " + str9;
            string textVal7 = (name7 + " " + str9).Replace("|DGBLUE|", "");
            UIText component8 = matchSetting6.value.Equals(matchSetting6.prevValue) ? new UIText(textVal7, Colors.Silver) : new UIText(textVal7, Colors.DGBlue);
            matchSetting6.prevValue = matchSetting6.value;
            component8.SetFont(f);
            menu.Add(component8, true); // end of all code if 
            MatchSetting matchSetting7 = TeamSelect2.GetMatchSetting("workshopmaps");
            string name8 = matchSetting7.name;
            string str11 = matchSetting7.value.ToString() + "%";
            int startIndex4 = matchSetting7.name.LastIndexOf('|');
            for (string str12 = matchSetting7.name.Substring(startIndex4, matchSetting7.name.Count<char>() - startIndex4); str12.Length < num1; str12 += " ")
                name8 += " ";
            while (str11.Length < num2)
                str11 = " " + str11;
            string textVal8 = (name8 + " " + str11).Replace("|DGBLUE|", "");
            UIText component9 = matchSetting7.value.Equals(matchSetting7.prevValue) ? new UIText(textVal8, Colors.Silver) : new UIText(textVal8, Colors.DGBlue);
            matchSetting7.prevValue = matchSetting7.value;
            component9.SetFont(f);
            menu.Add(component9, true);
            UIText component10 = new UIText(" ", Color.White);
            component10.SetFont(f);
            menu.Add(component10, true);
            string str13 = "CUSTOM LEVELS ";  //if (!ParentalControls.AreParentalControlsActive()) start
            int customLevelCount = Editor.customLevelCount;
            string str14 = customLevelCount.ToString();
            if (str14 == "0")
                str14 = "NONE";
            while (str13.Length < num1)
                str13 += " ";
            while (str14.Length < num2)
                str14 = " " + str14;
            string textVal9 = str13 + " " + str14;
            UIText component11 = customLevelCount == TeamSelect2.prevCustomLevels ? new UIText(textVal9, Colors.Silver) : new UIText(textVal9, Colors.DGBlue);
            TeamSelect2.prevCustomLevels = customLevelCount;
            component11.SetFont(f);
            menu.Add(component11, true);
            MatchSetting matchSetting8 = TeamSelect2.GetMatchSetting("clientlevelsenabled");
            string name9 = matchSetting8.name;
            string str15 = (bool)matchSetting8.value ? "ON" : "OFF";
            while (name9.Length < num1)
                name9 += " ";
            while (str15.Length < num2)
                str15 = " " + str15;
            string textVal10 = name9 + " " + str15;
            UIText component12 = matchSetting8.value.Equals(matchSetting8.prevValue) ? new UIText(textVal10, Colors.Silver) : new UIText(textVal10, Colors.DGBlue);
            matchSetting8.prevValue = matchSetting8.value;
            component12.SetFont(f);
            menu.Add(component12, true);
            UIText component13 = new UIText(" ", Color.White);
            component13.SetFont(f);
            menu.Add(component13, true); // end of old code if
            int num3 = 0;
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                if (unlock.onlineEnabled && unlock.enabled)
                    ++num3;
            }
            string str16 = "MODIFIERS ";
            string str17 = num3.ToString();
            if (num3 == 0)
                str17 = "NONE";
            while (str16.Length < num1)
                str16 += " ";
            while (str17.Length < num2)
                str17 = " " + str17;
            string textVal11 = str16 + " " + str17;
            UIText component14 = TeamSelect2.prevNumModifiers == num3 ? new UIText(textVal11, Colors.Silver) : new UIText(textVal11, Colors.DGBlue);
            TeamSelect2.prevNumModifiers = num3;
            component14.SetFont(f);
            menu.Add(component14, true);
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                if (unlock.onlineEnabled)
                {
                    string shortNameForDisplay = unlock.GetShortNameForDisplay();
                    while (shortNameForDisplay.Length < 20)
                        shortNameForDisplay += " ";
                    if (unlock.enabled != unlock.prevEnabled || unlock.enabled)
                    {
                        string textVal12 = !unlock.enabled ? "@USEROFFLINE@" + shortNameForDisplay : "@USERONLINE@" + shortNameForDisplay;
                        UIText component15 = unlock.enabled == unlock.prevEnabled ? new UIText(textVal12, unlock.enabled ? Color.White : Colors.Silver) : new UIText(textVal12, unlock.enabled ? Colors.DGGreen : Colors.DGRed);
                        component15.SetFont(f);
                        menu.Add(component15, true);
                    }
                    unlock.prevEnabled = unlock.enabled;
                }
            }
            if (openOnClose != null)
                menu.SetBackFunction(new UIMenuActionOpenMenu(menu, openOnClose));
            else
                menu.SetBackFunction(new UIMenuActionCloseMenu(DuckNetwork._ducknetUIGroup));
            menu.Close();
            return menu;
        }

        //private static void DoMatchSettingsInfoOpen()
        //{
        //    DuckNetwork._ducknetUIGroup = new UIComponent((float)(320.0 / 2.0), 180f / 2f, 0.0f, 0.0f);
        //    DuckNetwork._core._matchSettingMenu = DuckNetwork.CreateMatchSettingsInfoWindow();
        //    DuckNetwork._ducknetUIGroup.Add((UIComponent)DuckNetwork._core._matchSettingMenu, false);
        //    DuckNetwork._ducknetUIGroup.Close();
        //    DuckNetwork._ducknetUIGroup.Close();
        //    Level.Add((Thing)DuckNetwork._ducknetUIGroup);
        //    DuckNetwork._ducknetUIGroup.Update();
        //    DuckNetwork._ducknetUIGroup.Update();
        //    DuckNetwork._ducknetUIGroup.Update();
        //    DuckNetwork._ducknetUIGroup.Open();
        //    DuckNetwork._core._matchSettingMenu.Open();
        //    DuckNetwork._ducknetUIGroup.isPauseMenu = true;
        //    MonoMain.pauseMenu = DuckNetwork._ducknetUIGroup;
        //    DuckNetwork._core._pauseOpen = true;
        //    SFX.Play("pause", 0.6f);
        //}

        private static void DoSpectatorOpen(bool pSpectator)
        {
            DuckNetwork._ducknetUIGroup = new UIComponent((float)(320.0 / 2.0), 180f / 2f, 0.0f, 0.0f);
            UIMenu spectatorWindow = DuckNetwork.CreateSpectatorWindow(pSpectator);
            DuckNetwork._ducknetUIGroup.Add(spectatorWindow, false);
            DuckNetwork._ducknetUIGroup.Close();
            DuckNetwork._ducknetUIGroup.Close();
            Level.Add(_ducknetUIGroup);
            DuckNetwork._ducknetUIGroup.Update();
            DuckNetwork._ducknetUIGroup.Update();
            DuckNetwork._ducknetUIGroup.Update();
            DuckNetwork._ducknetUIGroup.Open();
            spectatorWindow.Open();
            DuckNetwork._ducknetUIGroup.isPauseMenu = true;
            MonoMain.pauseMenu = DuckNetwork._ducknetUIGroup;
            DuckNetwork._core._pauseOpen = true;
            SFX.Play("pause", 0.6f);
        }

        public static void ResetScores()
        {
            Main.ResetGameStuff();
            Main.ResetMatchStuff();
            if (Level.core.gameInProgress)
                Level.core.endedGameInProgress = true;
            Level.core.gameInProgress = false;
            Level.core.gameFinished = true;
            if (!Network.isServer)
                return;
            Send.Message(new NMResetGameSettings());
        }

        public static void CopyInviteLink()
        {
            if (Steam.user == null || Steam.lobby == null)
                return;
            Thread thread = new Thread(() =>
           {
               ulong id = Steam.lobby.id;
               string str1 = id.ToString();
               id = Steam.user.id;
               string str2 = id.ToString();
               Clipboard.SetText("steam://joinlobby/312530/" + str1 + "/" + str2);
           });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            HUD.AddPlayerChangeDisplay("@CLIPCOPY@Invite Link Copied!");
        }

        private static void OpenMenu(Profile whoOpen)
        {
            if (DuckNetwork._ducknetUIGroup != null)
                Level.Remove(_ducknetUIGroup);
            bool flag = Network.InLobby();
            DuckNetwork._core._menuOpenProfile = whoOpen;
            float num1 = 320f;
            float num2 = 180f;
            DuckNetwork._ducknetUIGroup = new UIComponent(num1 / 2f, num2 / 2f, 0.0f, 0.0f)
            {
                isPauseMenu = true
            };
            DuckNetwork.core._ducknetMenu = new UIMenu("@LWING@MULTIPLAYER@RWING@", num1 / 2f, num2 / 2f, 210f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
            //DuckNetwork._ducknetMenu = DuckNetwork.core._ducknetMenu;
            DuckNetwork.core._confirmMenu = whoOpen.slotType != SlotType.Local ? new UIMenu("REALLY QUIT?", num1 / 2f, num2 / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT") : new UIMenu("REALLY BACK OUT?", num1 / 2f, num2 / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
            DuckNetwork.core._confirmBlacklistMenu = new UIMenu("AVOID LEVEL?", num1 / 2f, num2 / 2f, 10f, conString: "@CANCEL@BACK @SELECT@SELECT");
            DuckNetwork.core._confirmKick = new UIMenu("REALLY KICK?", num1 / 2f, num2 / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
            DuckNetwork.core._confirmBan = new UIMenu("REALLY BAN?", num1 / 2f, num2 / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
            DuckNetwork.core._confirmBlock = new UIMenu("BLOCK PLAYER?", num1 / 2f, num2 / 2f, 280f, conString: "@CANCEL@BACK @SELECT@SELECT");
            DuckNetwork.core._confirmReturnToLobby = new UIMenu("RETURN TO LOBBY?", num1 / 2f, num2 / 2f, 230f, conString: "@CANCEL@BACK @SELECT@SELECT");
            DuckNetwork.core._confirmMatchSettings = new UIMenu("CHANGING SETTINGS", num1 / 2f, num2 / 2f, 230f, conString: "@CANCEL@BACK @SELECT@SELECT");
            DuckNetwork.core._confirmEditSlots = new UIMenu("CHANGING SETTINGS", num1 / 2f, num2 / 2f, 230f, conString: "@CANCEL@BACK @SELECT@SELECT");
            DuckNetwork.core._optionsMenu = Options.CreateOptionsMenu();
            DuckNetwork._core._settingsBeforeOpen = TeamSelect2.GetMatchSettingString();
            Main.SpecialCode = "men0";
            foreach (Profile p in (IEnumerable<Profile>)DuckNetwork.profiles.OrderBy<Profile, bool>(x => x.slotType == SlotType.Spectator))
            {
                if (p.connection != null)
                    DuckNetwork.core._ducknetMenu.Add(new UIConnectionInfo(p, DuckNetwork.core._ducknetMenu, DuckNetwork.core._confirmKick, DuckNetwork.core._confirmBan, DuckNetwork.core._confirmBlock), true);
            }
            Main.SpecialCode = "men1";
            DuckNetwork.core._ducknetMenu.Add(new UIText("", Color.White), true);
            DuckNetwork.core._ducknetMenu.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenuSetBoolean(DuckNetwork._ducknetUIGroup, DuckNetwork.core._menuClosed), UIAlign.Left, backButton: true), true);
            DuckNetwork.core._ducknetMenu.AssignDefaultSelection();
            if (whoOpen.slotType != SlotType.Local)
                DuckNetwork.core._ducknetMenu.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(core._ducknetMenu, core._optionsMenu), UIAlign.Left), true);
            if (whoOpen.slotType != SlotType.Local & flag && Network.isServer)
            {
                DuckNetwork._core._slotEditor = new UISlotEditor(DuckNetwork.core._ducknetMenu, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f);
                DuckNetwork._core._slotEditor.Close();
                DuckNetwork._ducknetUIGroup.Add(_core._slotEditor, false);
                DuckNetwork._core._matchSettingMenu = new UIMenu("@LWING@MATCH SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
                DuckNetwork._core._matchModifierMenu = new UIMenu("MODIFIERS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");
                DuckNetwork._core._levelSelectMenu = new LevelSelectCompanionMenu(num1 / 2f, num2 / 2f, DuckNetwork._core._matchSettingMenu);
                foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
                {
                    if (unlock.onlineEnabled)
                    {
                        if (unlock.unlocked)
                            DuckNetwork._core._matchModifierMenu.Add(new UIMenuItemToggle(unlock.GetShortNameForDisplay(), field: new FieldBinding(unlock, "enabled")), true);
                        else
                            DuckNetwork._core._matchModifierMenu.Add(new UIMenuItem("@TINYLOCK@LOCKED", c: Color.Red), true);
                    }
                }
                Main.SpecialCode = "men2";
                DuckNetwork._core._matchModifierMenu.SetBackFunction(new UIMenuActionOpenMenu(_core._matchModifierMenu, _core._matchSettingMenu));
                DuckNetwork._core._matchModifierMenu.Close();
                DuckNetwork._core._matchSettingMenu.AddMatchSetting(TeamSelect2.GetOnlineSetting("teams"), false);
                foreach (MatchSetting matchSetting in TeamSelect2.matchSettings) // removed ParentalControls.AreParentalControlsActive bs
                {
                    if (!(matchSetting.id == "workshopmaps") || Network.available)
                    { 
                        if (matchSetting.id != "partymode")
                        {
                            DuckNetwork._core._matchSettingMenu.AddMatchSetting(matchSetting, false, true);
                        }
                        if (matchSetting.id == "wallmode")
                        {
                            DuckNetwork._core._matchSettingMenu.Add(new UIText(" ", Color.White, UIAlign.Center, 0f, null), true);
                        }
                    }
                }
                Main.SpecialCode = "men3";
                DuckNetwork._core._matchSettingMenu.Add(new UIText(" ", Color.White), true);
               
                DuckNetwork._core._matchSettingMenu.Add(new UICustomLevelMenu(new UIMenuActionOpenMenu(_core._matchSettingMenu, _core._levelSelectMenu)), true); // ParentalControls.AreParentalControlsActive()
                DuckNetwork._core._matchSettingMenu.Add(new UIModifierMenuItem(new UIMenuActionOpenMenu(_core._matchSettingMenu, _core._matchModifierMenu)), true);
                DuckNetwork._core._matchSettingMenu.SetBackFunction(new UIMenuActionOpenMenu(_core._matchSettingMenu, core._ducknetMenu));
                DuckNetwork._core._matchSettingMenu.Close();
                DuckNetwork._ducknetUIGroup.Add(_core._matchSettingMenu, false);
                DuckNetwork._ducknetUIGroup.Add(_core._matchModifierMenu, false);
                DuckNetwork._ducknetUIGroup.Add(_core._levelSelectMenu, false);
                DuckNetwork._ducknetUIGroup.Close();
                Main.SpecialCode = "men4";
                if (Level.core.gameInProgress)
                {
                    DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGBLUE|MATCH SETTINGS", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._confirmMatchSettings), UIAlign.Left), true);
                    DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGBLUE|EDIT SLOTS", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._confirmEditSlots), UIAlign.Left), true);
                }
                else
                {
                    DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGBLUE|MATCH SETTINGS", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._matchSettingMenu), UIAlign.Left), true);
                    DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGBLUE|EDIT SLOTS", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._slotEditor), UIAlign.Left), true);
                }
            }
            Main.SpecialCode = "men5";
            if (Network.isClient && whoOpen.slotType != SlotType.Local || Network.isServer && !Network.InLobby())
            {
                UIMenu settingsInfoWindow = DuckNetwork.CreateMatchSettingsInfoWindow(DuckNetwork._core._ducknetMenu);
                DuckNetwork._ducknetUIGroup.Add(settingsInfoWindow, false);
                DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGBLUE|VIEW MATCH SETTINGS", new UIMenuActionOpenMenu(_core._ducknetMenu, settingsInfoWindow), UIAlign.Left), true);
                Main.SpecialCode = "men6";
                if ((bool)TeamSelect2.GetMatchSetting("clientlevelsenabled").value && Network.InLobby()) // removed && !ParentalControls.AreParentalControlsActive()
                {
                    DuckNetwork._core._levelSelectMenu = new LevelSelectCompanionMenu(num1 / 2f, num2 / 2f, DuckNetwork._core._ducknetMenu);
                    DuckNetwork._core._ducknetMenu.Add(new UICustomLevelMenu(new UIMenuActionOpenMenu(_core._ducknetMenu, _core._levelSelectMenu)), true);
                    DuckNetwork._ducknetUIGroup.Add(_core._levelSelectMenu, false);
                }
            }
            Main.SpecialCode = "men7";
            DuckNetwork._core._ducknetMenu.Add(new UIText("", Color.White), true);
            if (flag && whoOpen.slotType != SlotType.Local && Network.available)
            {
                Main.SpecialCode = "men8";
                DuckNetwork._core._inviteMenu = new UIInviteMenu("INVITE FRIENDS", null, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f);
                ((UIInviteMenu)DuckNetwork._core._inviteMenu).SetAction(new UIMenuActionOpenMenu(_core._inviteMenu, _core._ducknetMenu));
                DuckNetwork._core._inviteMenu.Close();
                DuckNetwork._ducknetUIGroup.Add(_core._inviteMenu, false);
                Main.SpecialCode = "men9";
                DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGGREEN|INVITE FRIENDS", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._inviteMenu), UIAlign.Left), true);
                DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGGREEN|COPY INVITE LINK", new UIMenuActionCloseMenuCallFunction(DuckNetwork._ducknetUIGroup, new UIMenuActionCloseMenuCallFunction.Function(DuckNetwork.CopyInviteLink)), UIAlign.Left), true);
            }
            Main.SpecialCode = "men10";
            if (Level.current is GameLevel && Level.current.isCustomLevel)
            {
                if ((Level.current as GameLevel).data.metaData.workshopID != 0UL && Steam.IsInitialized())
                {
                    Main.SpecialCode = "men11";
                    WorkshopItem workshopItem = WorkshopItem.GetItem((Level.current as GameLevel).data.metaData.workshopID);
                    if (workshopItem != null)
                    {
                        DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("@STEAMICON@|DGGREEN|VIEW", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(GameMode.View)), UIAlign.Left), true);
                        if ((workshopItem.stateFlags & WorkshopItemState.Subscribed) != WorkshopItemState.None)
                        {
                            DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("@STEAMICON@|DGRED|UNSUBSCRIBE", new UIMenuActionCloseMenuCallFunction(DuckNetwork._ducknetUIGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Subscribe)), UIAlign.Left), true);
                        }
                        else
                        {
                            DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("@STEAMICON@|DGGREEN|SUBSCRIBE", new UIMenuActionCloseMenuCallFunction(DuckNetwork._ducknetUIGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Subscribe)), UIAlign.Left), true);
                            if (Network.isServer)
                                DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("@blacklist@|DGRED|NEVER AGAIN", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._confirmBlacklistMenu), UIAlign.Left), true);
                        }
                    }
                }
                Main.SpecialCode = "men12";
                if (!(Level.current as GameLevel).matchOver && Network.isServer)
                    DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("@SKIPSPIN@|DGRED|SKIP", new UIMenuActionCloseMenuCallFunction(DuckNetwork._ducknetUIGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Skip)), UIAlign.Left), true);
                DuckNetwork._core._ducknetMenu.Add(new UIText(" ", Color.White), true);
            }
            Main.SpecialCode = "men13";
            if (whoOpen.slotType != SlotType.Local || Network.InLobby())
            {
                if (whoOpen.slotType == SlotType.Local)
                    DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGRED|BACK OUT", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._confirmMenu), UIAlign.Left), true);
                else
                    DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGRED|DISCONNECT", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._confirmMenu), UIAlign.Left), true);
            }
            Main.SpecialCode = "men14";
            if (Network.isServer && Level.current is GameLevel)
                DuckNetwork._core._ducknetMenu.Add(new UIMenuItem("|DGRED|BACK TO LOBBY", new UIMenuActionOpenMenu(_core._ducknetMenu, _core._confirmReturnToLobby), UIAlign.Left), true);
            DuckNetwork._ducknetUIGroup._closeFunction = new UIMenuActionCloseMenuSetBoolean(DuckNetwork._ducknetUIGroup, DuckNetwork._core._menuClosed);
            DuckNetwork._core._ducknetMenu.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._ducknetMenu, false);
            Options.AddMenus(DuckNetwork._ducknetUIGroup);
            Options.openOnClose = DuckNetwork._core._ducknetMenu;
            Main.SpecialCode = "men15";
            DuckNetwork._core._confirmReturnToLobby.Add(new UIText("YOU WILL BE ABLE TO RETURN", Color.White), true);
            DuckNetwork._core._confirmReturnToLobby.Add(new UIText("TO THE CURRENT GAME.", Color.White), true);
            DuckNetwork._core._confirmReturnToLobby.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_core._confirmReturnToLobby, _core._ducknetMenu), UIAlign.Left, backButton: true), true);
            DuckNetwork._core._confirmReturnToLobby.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(DuckNetwork._ducknetUIGroup, DuckNetwork._core._returnToLobby)), true);
            DuckNetwork._core._confirmReturnToLobby.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._confirmReturnToLobby, false);
            DuckNetwork._core._confirmEditSlots.Add(new UIText("WOULD YOU LIKE TO", Color.White), true);
            DuckNetwork._core._confirmEditSlots.Add(new UIText("RESET SCORES?", Color.White), true);
            DuckNetwork._core._confirmEditSlots.Add(new UIText("", Color.White), true);
            DuckNetwork._core._confirmEditSlots.Add(new UIMenuItem("KEEP SCORES", new UIMenuActionOpenMenu(_core._confirmEditSlots, _core._slotEditor)), true);
            DuckNetwork._core._confirmEditSlots.Add(new UIMenuItem("|DGRED|RESET SCORES", new UIMenuActionOpenMenuCallFunction(_core._confirmEditSlots, _core._slotEditor, new UIMenuActionOpenMenuCallFunction.Function(DuckNetwork.ResetScores))), true);
            DuckNetwork._core._confirmEditSlots.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._confirmEditSlots, false);
            DuckNetwork._core._confirmMatchSettings.Add(new UIText("WOULD YOU LIKE TO", Color.White), true);
            DuckNetwork._core._confirmMatchSettings.Add(new UIText("RESET SCORES?", Color.White), true);
            DuckNetwork._core._confirmMatchSettings.Add(new UIText("", Color.White), true);
            DuckNetwork._core._confirmMatchSettings.Add(new UIMenuItem("KEEP SCORES", new UIMenuActionOpenMenu(_core._confirmMatchSettings, _core._matchSettingMenu)), true);
            DuckNetwork._core._confirmMatchSettings.Add(new UIMenuItem("|DGRED|RESET SCORES", new UIMenuActionOpenMenuCallFunction(_core._confirmMatchSettings, _core._matchSettingMenu, new UIMenuActionOpenMenuCallFunction.Function(DuckNetwork.ResetScores))), true);
            DuckNetwork._core._confirmMatchSettings.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._confirmMatchSettings, false);
            Main.SpecialCode = "men16";
            DuckNetwork._core._confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_core._confirmMenu, _core._ducknetMenu), UIAlign.Left, backButton: true), true);
            DuckNetwork._core._confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(DuckNetwork._ducknetUIGroup, DuckNetwork._core._quit)), true);
            DuckNetwork._core._confirmMenu.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._confirmMenu, false);
            UIMenu confirmBlacklistMenu1 = DuckNetwork._core._confirmBlacklistMenu;
            UIText component1 = new UIText("", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            confirmBlacklistMenu1.Add(component1, true);
            UIMenu confirmBlacklistMenu2 = DuckNetwork._core._confirmBlacklistMenu;
            UIText component2 = new UIText("Are you sure you want to avoid", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            confirmBlacklistMenu2.Add(component2, true);
            UIMenu confirmBlacklistMenu3 = DuckNetwork._core._confirmBlacklistMenu;
            UIText component3 = new UIText("this level in the future?", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            confirmBlacklistMenu3.Add(component3, true);
            UIMenu confirmBlacklistMenu4 = DuckNetwork._core._confirmBlacklistMenu;
            UIText component4 = new UIText("", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            confirmBlacklistMenu4.Add(component4, true);
            DuckNetwork._core._confirmBlacklistMenu.Add(new UIMenuItem("|DGRED|@blacklist@YES!", new UIMenuActionCloseMenuCallFunction(DuckNetwork._ducknetUIGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Blacklist))), true);
            DuckNetwork._core._confirmBlacklistMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_core._confirmBlacklistMenu, _core._ducknetMenu), backButton: true), true);
            DuckNetwork._core._confirmBlacklistMenu.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._confirmBlacklistMenu, false);
            Main.SpecialCode = "men17";
            DuckNetwork._core._confirmKick.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_core._confirmKick, _core._ducknetMenu), UIAlign.Left, backButton: true), true);
            DuckNetwork._core._confirmKick.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuCallFunction(DuckNetwork._ducknetUIGroup, new UIMenuActionCloseMenuCallFunction.Function(DuckNetwork.KickedPlayer))), true);
            DuckNetwork._core._confirmKick.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._confirmKick, false);
            DuckNetwork._core._confirmBan.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_core._confirmBan, _core._ducknetMenu), UIAlign.Left, backButton: true), true);
            DuckNetwork._core._confirmBan.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuCallFunction(DuckNetwork._ducknetUIGroup, new UIMenuActionCloseMenuCallFunction.Function(DuckNetwork.BannedPlayer))), true);
            DuckNetwork._core._confirmBan.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._confirmBan, false);
            Main.SpecialCode = "men18";
            DuckNetwork._core._confirmBlock.Add(new UIText("I'm sorry it's come to this :(", Colors.DGBlue), true);
            DuckNetwork._core._confirmBlock.Add(new UIText("", Color.White), true);
            DuckNetwork._core._confirmBlock.Add(new UIText("Blocking this player will", Colors.DGBlue), true);
            DuckNetwork._core._confirmBlock.Add(new UIText("stop their interactions with", Colors.DGBlue), true);
            DuckNetwork._core._confirmBlock.Add(new UIText("you and prevent them from", Colors.DGBlue), true);
            DuckNetwork._core._confirmBlock.Add(new UIText("joining your games in the future.", Colors.DGBlue), true);
            DuckNetwork._core._confirmBlock.Add(new UIText("", Color.White), true);
            DuckNetwork._core._confirmBlock.Add(new UIText("Are you sure?", Colors.DGBlue), true);
            DuckNetwork._core._confirmBlock.Add(new UIText("", Color.White), true);
            DuckNetwork._core._confirmBlock.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_core._confirmBlock, _core._ducknetMenu), backButton: true), true);
            DuckNetwork._core._confirmBlock.Add(new UIMenuItem("YES, PLEASE BLOCK THEM!", new UIMenuActionCloseMenuCallFunction(DuckNetwork._ducknetUIGroup, new UIMenuActionCloseMenuCallFunction.Function(DuckNetwork.BlockedPlayer))), true);
            DuckNetwork._core._confirmBlock.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._confirmBlock, false);
            Main.SpecialCode = "men19";
            DuckNetwork._core._optionsMenu.SetBackFunction(new UIMenuActionOpenMenuCallFunction(_core._optionsMenu, _core._ducknetMenu, new UIMenuActionOpenMenuCallFunction.Function(Options.OptionsMenuClosed)));
            DuckNetwork._core._optionsMenu.Close();
            DuckNetwork._ducknetUIGroup.Add(_core._optionsMenu, false);
            DuckNetwork._ducknetUIGroup.Add(Options._lastCreatedGraphicsMenu, false);
            DuckNetwork._ducknetUIGroup.Add(Options._lastCreatedAudioMenu, false);
            if (Options._lastCreatedAccessibilityMenu != null)
                DuckNetwork._ducknetUIGroup.Add(Options._lastCreatedAccessibilityMenu, false);
            if (Options._lastCreatedTTSMenu != null)
                DuckNetwork._ducknetUIGroup.Add(Options._lastCreatedTTSMenu, false);
            if (Options._lastCreatedBlockMenu != null)
                DuckNetwork._ducknetUIGroup.Add(Options._lastCreatedBlockMenu, false);
            if (Options._lastCreatedControlsMenu != null)
                DuckNetwork._ducknetUIGroup.Add(Options._lastCreatedControlsMenu, false);
            Main.SpecialCode = "men20";
            DuckNetwork._ducknetUIGroup.Close();
            Level.Add(_ducknetUIGroup);
            DuckNetwork._ducknetUIGroup.Update();
            DuckNetwork._ducknetUIGroup.Update();
            DuckNetwork._ducknetUIGroup.Update();
            DuckNetwork._ducknetUIGroup.Open();
            DuckNetwork._core._ducknetMenu.Open();
            MonoMain.pauseMenu = DuckNetwork._ducknetUIGroup;
            HUD.AddCornerControl(HUDCorner.BottomRight, "@CHAT@CHAT");
            DuckNetwork._core._pauseOpen = true;
            SFX.Play("pause", 0.6f);
        }

        public static void SendCurrentLevelData(ushort session, NetworkConnection c)
        {
            int val2 = 512;
            MemoryStream compressedLevelData = DuckNetwork.compressedLevelData;
            long length1 = compressedLevelData.Length;
            int offset = 0;
            Math.Ceiling(length1 / (double)val2);
            compressedLevelData.Position = 0L;
            Send.Message(new NMLevelDataHeader(session, (int)length1, DuckNetwork.compressedLevelName), c);
            while (offset != length1)
            {
                BitBuffer dat = new BitBuffer();
                byte[] numArray = new byte[val2];
                int length2 = (int)Math.Min(length1 - offset, val2);
                dat.Write(compressedLevelData.GetBuffer(), offset, length2);
                offset += length2;
                Send.Message(new NMLevelDataChunk(session, dat), c);
            }
        }

        public static bool allClientsReady
        {
            get
            {
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    if (profile.connection != null && profile.connection.levelIndex != levelIndex)
                        return false;
                }
                return true;
            }
        }

        private static void OpenTeamSwitchDialogue(Profile p)
        {
            if (DuckNetwork._uhOhGroup != null && DuckNetwork._uhOhGroup.open)
                return;
            if (DuckNetwork._uhOhGroup != null)
                Level.Remove(_uhOhGroup);
            DuckNetwork.ClearTeam(p);
            DuckNetwork._uhOhGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
            DuckNetwork._uhOhMenu = new UIMenu("UH OH", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 210f, conString: "@SELECT@OK");
            float num = 190f;
            string str1 = "The host isn't allowing teams, and someone else is already wearing your hat :(";
            string textVal = "";
            string str2 = "";
            while (true)
            {
                if (str1.Length > 0 && str1[0] != ' ')
                {
                    str2 += str1[0].ToString();
                }
                else
                {
                    if ((textVal.Length + str2.Length) * 8 > (double)num)
                    {
                        DuckNetwork._uhOhMenu.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
                        textVal = "";
                    }
                    if (textVal.Length > 0)
                        textVal += " ";
                    textVal += str2;
                    str2 = "";
                }
                if (str1.Length != 0)
                    str1 = str1.Remove(0, 1);
                else
                    break;
            }
            if (str2.Length > 0)
            {
                if (textVal.Length > 0)
                    textVal += " ";
                textVal += str2;
            }
            if (textVal.Length > 0)
                DuckNetwork._uhOhMenu.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
            DuckNetwork._uhOhMenu.Add(new UIText(" ", Color.White), true);
            DuckNetwork._uhOhMenu.Add(new UIMenuItem("OH DEAR", new UIMenuActionCloseMenu(DuckNetwork._uhOhGroup), c: Colors.MenuOption, backButton: true), true);
            DuckNetwork._uhOhMenu.Close();
            DuckNetwork._uhOhGroup.Add(_uhOhMenu, false);
            DuckNetwork._uhOhGroup.Close();
            Level.Add(_uhOhGroup);
            DuckNetwork._uhOhGroup.Open();
            DuckNetwork._uhOhMenu.Open();
            MonoMain.pauseMenu = DuckNetwork._uhOhGroup;
            SFX.Play("pause", 0.6f);
        }

        public static void ClearTeam(Profile p)
        {
            if (!(Level.current is TeamSelect2))
                return;
            (Level.current as TeamSelect2).ClearTeam(p.networkIndex);
        }

        public static bool OnTeamSwitch(Profile p)
        {
            if (TeamSelect2.GetSettingBool("teams"))
                return true;
            Team team = p.team;
            bool flag = true;
            if (team != null)
            {
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    if (p.connection != null && p != profile && profile.team == p.team && profile.slotType != SlotType.Spectator && Network.isServer)
                    {
                        if (p.connection == DuckNetwork.localConnection)
                            DuckNetwork.OpenTeamSwitchDialogue(p);
                        else
                            Send.Message(new NMTeamSetDenied(p, p.team), p.connection);
                        flag = false;
                        return flag;
                    }
                }
            }
            return flag;
        }

        private static string GetNameForIndex(string pOriginalName, int pIndex)
        {
            if (pIndex == 0)
                return pOriginalName;
            if (pOriginalName.Length > 14)
                pOriginalName = pOriginalName.Substring(0, 14);
            pOriginalName = pOriginalName + "(" + (pIndex + 1).ToString() + ")";
            return pOriginalName;
        }

        private static void SendJoinMessage(NetworkConnection c)
        {
            DuckNetwork.invited = false;
            ulong pLocalID = 0;
            if (Steam.lobby != null && NCSteam.inviteLobbyID != 0UL && (long)NCSteam.inviteLobbyID == (long)Steam.lobby.id)
                DuckNetwork.invited = true;
            NCSteam.inviteLobbyID = 0UL;
            if (!Network.lanMode)
                pLocalID = DG.localID;
            List<string> pNames = new List<string>();
            List<byte> pPersonas = new List<byte>();
            int pIndex = 1;
            foreach (MatchmakingPlayer matchmakingProfile in UIMatchmakingBox.core.matchmakingProfiles)
            {
                if (matchmakingProfile.masterProfile != null)
                {
                    pNames.Add(DuckNetwork.GetNameForIndex(matchmakingProfile.masterProfile.name, pIndex));
                    ++pIndex;
                }
                else
                    pNames.Add(matchmakingProfile.originallySelectedProfile.name);
                pPersonas.Add((byte)matchmakingProfile.persona.index);
            }
            Send.Message(new NMRequestJoin(pNames, pPersonas, DuckNetwork.invited, DuckNetwork.core.serverPassword, pLocalID), NetMessagePriority.ReliableOrdered, c);
        }

        //private static void ClearAllNetworkData() => Network.Terminate();

        private static void EnsureMatchmakingProfileExperienceProfile(bool pLan)
        {
            if (pLan)
                return;
            foreach (MatchmakingPlayer matchmakingProfile in UIMatchmakingBox.core.matchmakingProfiles)
            {
                if (matchmakingProfile.isMaster && matchmakingProfile.originallySelectedProfile != Profiles.experienceProfile)
                {
                    Profile originallySelectedProfile = matchmakingProfile.originallySelectedProfile;
                    Profiles.experienceProfile.team = originallySelectedProfile.team;
                    Profiles.experienceProfile.inputProfile = originallySelectedProfile.inputProfile;
                    originallySelectedProfile.team = null;
                    originallySelectedProfile.inputProfile = null;
                    matchmakingProfile.originallySelectedProfile = Profiles.experienceProfile;
                }
            }
        }

        public static bool isDedicatedServer
        {
            get => DuckNetwork._core.isDedicatedServer;
            set => DuckNetwork._core.isDedicatedServer = value;
        }

        public static void Host(int maxPlayers, NetworkLobbyType lobbyType, bool useCurrentSettings = false)
        {
            DuckNetwork.isDedicatedServer = false;
            DuckNetwork.invited = false;
            Network.lanMode = lobbyType == NetworkLobbyType.LAN || lobbyType == NetworkLobbyType.Invisible;
            DuckNetwork._core.serverPassword = (string)TeamSelect2.GetOnlineSetting("password").value;
            if (DuckNetwork._core.status == DuckNetStatus.Disconnected && Network.connections.Count == 0)
            {
                DevConsole.Log(DCSection.DuckNet, "|LIME|Hosting new server. ");
                DuckNetwork.EnsureMatchmakingProfileExperienceProfile(Network.lanMode);
                DuckNetwork.Reset();
                foreach (Profile universalProfile in Profiles.universalProfileList)
                    universalProfile.team = null;
                if (!useCurrentSettings)
                    TeamSelect2.DefaultSettings();
                int lanPort = TeamSelect2.GetLANPort();
                Network.HostServer(lobbyType, maxPlayers, (string)TeamSelect2.GetOnlineSetting("name").value, lanPort);
                DuckNetwork.localConnection.BeginConnection();
                DuckNetwork.preparingProfiles = true;
                int num1 = 0;
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    switch (lobbyType)
                    {
                        case NetworkLobbyType.Private:
                            profile.slotType = SlotType.Invite;
                            break;
                        case NetworkLobbyType.FriendsOnly:
                            profile.slotType = SlotType.Friend;
                            break;
                        default:
                            profile.slotType = SlotType.Open;
                            break;
                    }
                    if (num1 >= maxPlayers)
                        profile.slotType = SlotType.Closed;
                    if (num1 >= DG.MaxPlayers)
                        profile.slotType = SlotType.Spectator;
                    profile.originalSlotType = profile.slotType;
                    ++num1;
                }
                DuckNetwork.preparingProfiles = false;
                DuckNetwork.ChangeSlotSettings();
                int num2 = 1;
                foreach (MatchmakingPlayer matchmakingProfile in UIMatchmakingBox.core.matchmakingProfiles)
                {
                    if (matchmakingProfile.spectator)
                        DuckNetwork.isDedicatedServer = true;
                    string pName = Network.activeNetwork.core.GetLocalName();
                    if (Network.activeNetwork.core is NCBasic)
                    {
                        pName = matchmakingProfile.originallySelectedProfile.name;
                        NCBasic._localName = pName;
                    }
                    DuckNetwork.ServerCreateProfile(DuckNetwork._core.localConnection, matchmakingProfile.inputProfile, matchmakingProfile.originallySelectedProfile, pName, true, false, matchmakingProfile.spectator).persona = matchmakingProfile.persona;
                    ++num2;
                }
                DuckNetwork._core.localConnection.levelIndex = 0;
                DuckNetwork._core.localConnection.isHost = true;
                DuckNetwork._core.status = DuckNetStatus.ConnectingToServer;
            }
            else
                DevConsole.Log(DCSection.DuckNet, "@error !!DuckNetwork.Host called while still connected!!@error");
        }

        public static void Join(string id, string ip = "localhost") => DuckNetwork.Join(id, ip, "");

        public static void Join(string id, string ip, string pPassword)
        {
            DuckNetwork.isDedicatedServer = false;
            if (pPassword == null)
                pPassword = "";
            DuckNetwork.core.serverPassword = pPassword;
            if (DuckNetwork._core.status == DuckNetStatus.Disconnected && Network.connections.Count == 0)
            {
                Network.lanMode = ip != "localhost";
                DevConsole.Log(DCSection.DuckNet, "|LIME|Attempting to join " + id);
                DuckNetwork.EnsureMatchmakingProfileExperienceProfile(Network.lanMode);
                DuckNetwork.Reset();
                foreach (Profile universalProfile in Profiles.universalProfileList)
                    universalProfile.team = null;
                for (int index = 0; index < DG.MaxPlayers; ++index)
                    Teams.all[index].customData = null;
                TeamSelect2.DefaultSettings();
                Network.JoinServer(id, 1337 + DuckNetwork.joinPort, ip);
                DuckNetwork.localConnection.BeginConnection();
                DuckNetwork._core.status = DuckNetStatus.EstablishingCommunicationWithServer;
            }
            else
                DevConsole.Log(DCSection.DuckNet, "@error !!DuckNetwork.Join called while still connected!!@error");
        }

        public static Profile JoinLocalDuck(InputProfile pInput)
        {
            if (!Network.isServer)
                return null;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.team != null && profile.inputProfile == pInput)
                    return profile;
            }
            int num = DuckNetwork.profiles.Where<Profile>(x => x.connection == DuckNetwork.localConnection).Count<Profile>();
            string pName = Network.activeNetwork.core.GetLocalName();
            if (num > 0)
                pName = pName + "(" + (num + 1).ToString() + ")";
            Profile pLocalProfile = null;
            if (pInput.mpIndex >= 0)
                pLocalProfile = Profile.defaultProfileMappings[pInput.mpIndex];
            Profile profile1 = DuckNetwork.ServerCreateProfile(DuckNetwork.localConnection, pInput, pLocalProfile, pName, true, false, false);
            if (profile1 == null)
                return null;
            profile1.isRemoteLocalDuck = true;
            Level.current.OnNetworkConnecting(profile1);
            DuckNetwork.Server_AcceptJoinRequest(new List<Profile>()
      {
        profile1
      }, true);
            return profile1;
        }

        private static IEnumerable<Profile> GetOpenProfiles(
          NetworkConnection pConnection,
          bool pInvited,
          bool pLocal,
          bool pSpectator)
        {
            bool pFriend = false;
            if (pConnection.data is User && (pConnection.data as User).id != 0UL)
            {
                if (DuckNetwork._core._invitedFriends.Contains((pConnection.data as User).id))
                    pInvited = true;
                if ((pConnection.data as User).relationship == FriendRelationship.Friend)
                    pFriend = true;
            }
            IEnumerable<Profile> source = DuckNetwork.profiles.Where<Profile>(x => x.connection == null && x.reservedUser != null && pConnection.data == x.reservedUser);
            if (source.Count<Profile>() == 0)
                source = !pSpectator ? DuckNetwork.profiles.Where<Profile>(x => x.connection == null && (x.slotType == SlotType.Invite && pInvited | pLocal || x.slotType == SlotType.Friend && pFriend | pLocal || pLocal && x.slotType == SlotType.Local || x.slotType == SlotType.Open) && x.slotType != SlotType.Spectator && x.networkIndex <= 7) : DuckNetwork.profiles.Where<Profile>(x => x.connection == null && x.slotType == SlotType.Spectator);
            return source;
        }

        private static Profile ServerCreateProfile(
          NetworkConnection pConnection,
          InputProfile pInput,
          Profile pLocalProfile,
          string pName,
          bool pLocal,
          bool pWasInvited,
          bool pSpectator)
        {
            Profile pProfile = DuckNetwork.GetOpenProfiles(pConnection, pWasInvited, pLocal, pSpectator).FirstOrDefault<Profile>();
            if (pProfile == null)
                return null;
            DuckNetwork.PrepareProfile(pProfile, pConnection, pInput, pLocalProfile, pName);
            pProfile.invited = pWasInvited;
            return pProfile;
        }

        private static void PrepareProfile(
          Profile pProfile,
          NetworkConnection pConnection,
          InputProfile pLocalInput,
          Profile pLocalProfile,
          string pName)
        {
            DevConsole.Log(DCSection.DuckNet, "PrepareProfile(" + pConnection.ToString() + ", " + pName + "," + pProfile.networkIndex.ToString() + ")");
            Team reservedTeam = pProfile.reservedTeam;
            sbyte spectatorPersona = pProfile.reservedSpectatorPersona;
            DuckNetwork.ResetProfile(pProfile);
            pProfile.linkedProfile = pLocalProfile;
            if (pConnection.profile == null)
            {
                pConnection.profile = pProfile;
                pConnection.name = pName;
            }
            pProfile.connection = pConnection;
            pProfile.name = pName;
            if (pConnection == DuckNetwork.localConnection)
            {
                if (DuckNetwork._core.localProfile == null)
                    DuckNetwork._core.localProfile = pProfile;
                if (Network.isServer && DuckNetwork._core.hostProfile == null)
                    DuckNetwork._core.hostProfile = pProfile;
                pProfile.networkStatus = DuckNetStatus.Connected;
                if (!Network.lanMode)
                    pProfile.steamID = DG.localID;
                if (DuckNetwork.profiles.Where<Profile>(x => x.connection == pConnection).Count<Profile>() > 1)
                {
                    pProfile.slotType = SlotType.Local;
                    pProfile.isRemoteLocalDuck = true;
                }
                pProfile.flagIndex = Global.data.flag;
                pProfile.inputProfile = pLocalInput;
            }
            else
                pProfile.inputProfile = InputProfile.GetVirtualInput(pProfile.networkIndex);
            pProfile.team = reservedTeam == null ? pProfile.networkDefaultTeam : reservedTeam;
            pProfile.reservedUser = null;
            pProfile.reservedTeam = null;
            if (pProfile.slotType == SlotType.Reserved)
                pProfile.slotType = SlotType.Invite;
            pProfile.persona = pProfile.networkDefaultPersona;
            if (spectatorPersona != -1)
            {
                pProfile.netData.Set<sbyte>("spectatorPersona", spectatorPersona);
                pProfile.reservedSpectatorPersona = spectatorPersona;
            }
            if (pProfile.steamID == 0UL || Options.Data.recentPlayers.Contains(pProfile.steamID))
                return;
            Options.Data.recentPlayers.Insert(0, pProfile.steamID);
            while (Options.Data.recentPlayers.Count > 10)
                Options.Data.recentPlayers.RemoveAt(Options.Data.recentPlayers.Count - 1);
            Options.flagForSave = 60;
        }

        public static void Reset()
        {
            foreach (Profile profile in DuckNetwork.profiles)
                DuckNetwork.ResetProfile(profile, false);
            Level.core.gameInProgress = false;
            DuckNetwork._core._invitedFriends.Clear();
            Main.ResetGameStuff();
            Main.ResetMatchStuff();
            DuckNetwork._core.RecreateProfiles();
            DuckNetwork._core.hostProfile = null;
            DuckNetwork._core.localProfile = null;
            DataLayerDebug.BadConnection pContext = null;
            if (DuckNetwork._core.localConnection != null)
                pContext = DuckNetwork._core.localConnection.debuggerContext;
            DuckNetwork._core.localConnection = new NetworkConnection(null);
            DuckNetwork._core.localConnection.SetDebuggerContext(pContext);
            DuckNetwork._core.inGame = false;
            DuckNetwork._core.status = DuckNetStatus.Disconnected;
            DuckNetwork._core.levelTransferSession = 0;
            DuckNetwork._core.levelTransferProgress = 0;
            DuckNetwork._core.levelTransferSize = 0;
            DuckNetwork._core.enteringText = false;
            DuckNetwork._core.localConnection.levelIndex = byte.MaxValue;
            DevConsole.Log(DCSection.DuckNet, "|LIME|----------------Duck Network has been RESET----------------");
        }

        public static void ResetProfile(Profile p) => DuckNetwork.ResetProfile(p, false);

        public static void ResetProfile(Profile p, bool reserve)
        {
            DevConsole.Log(DCSection.DuckNet, "|DGRED|Resetting profile (" + (p.connection != null ? p.connection.ToString() : "null") + ")");
            if (p.connection != null && p.connection.profile == p)
            {
                if (p.connection != DuckNetwork.localConnection)
                    Level.current.OnNetworkDisconnected(p);
                p.connection.profile = null;
            }
            p.reservedUser = reserve ? (p.connection != null ? p.connection.data : null) : null;
            p.reservedTeam = p.team == null || p.IndexOfCustomTeam(p.team) > 0 ? null : (reserve ? p.team : null);
            if (p.persona != null)
                p.reservedSpectatorPersona = reserve ? (sbyte)p.persona.index : (sbyte)-1;
            p.netData = new ProfileNetData();
            p.removedGhosts.Clear();
            p.spectatorChangeIndex = p.remoteSpectatorChangeIndex = 0;
            p.connection = null;
            p.team = null;
            p.networkStatus = DuckNetStatus.Disconnected;
            p.flippers = 0;
            p._blockStatusDirty = true;
            p.linkedProfile = null;
            p.preferredColor = -1;
            p.furniturePositions.Clear();
            p.ParentalControlsActive = false;
            p.flagIndex = -1;
            p.numClientCustomLevels = 0;
            p.keepSetName = false;
            p.customTeams.Clear();
            p.networkHatUnlockStatuses = null;
            p.isRemoteLocalDuck = false;
            p.steamID = 0UL;
            p.duck = null;
            p.inputProfile = InputProfile.GetVirtualInput(p.networkIndex);
            if (p.inputProfile == null)
                return;
            if (p.inputProfile.virtualDevice != null)
            {
                p.inputProfile.virtualDevice.SetState(0);
                p.inputProfile.virtualDevice.SetState(0);
            }
            p.inputProfile.lastActiveOverride = null;
            p.inputMappingOverrides.Clear();
        }

        public static void OnDisconnect(NetworkConnection connection, string reason, bool kicked = false)
        {
            if (DuckNetwork._core.localProfile == null || connection == DuckNetwork.localConnection)
            {
                if (!connection.isHost)
                    return;
                Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.EveryoneDisconnected, "Could not connect to server."));
            }
            else if (DuckNetwork.status == DuckNetStatus.ConnectingToClients && NCNetworkImplementation.currentError != null && NCNetworkImplementation.currentError.error == DuckNetError.ConnectionTimeout)
                Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.EveryoneDisconnected, "Could not connect to: {\n\n\n" + connection.name));
            else if (connection.isHost & kicked)
            {
                Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.Kicked, "|RED|Oh no! The host kicked you :("));
            }
            else
            {
                if (!kicked)
                {
                    byte pStationID = 99;
                    NetworkConnection networkConnection = null;
                    byte num = 99;
                    if (DuckNetwork.hostProfile == null)
                    {
                        DevConsole.Log(DCSection.General, "Host changed but I'm still in the process of connecting.  This is bad since I have incomplete data, disconnecting.");
                        Level.current = new DisconnectFromGame();
                        return;
                    }
                    if (DuckNetwork.hostProfile.connection == connection)
                    {
                        foreach (Profile profile in DuckNetwork.core.profiles)
                        {
                            if (!profile.isRemoteLocalDuck)
                            {
                                if (profile.connection != null && profile.connection != connection && profile.networkIndex < pStationID)
                                {
                                    pStationID = profile.networkIndex;
                                    networkConnection = profile.connection;
                                }
                                if (profile.connection == DuckNetwork.localConnection && profile.networkIndex < num)
                                    num = profile.networkIndex;
                            }
                        }
                        if (networkConnection == null)
                            throw new Exception("DuckNetwork.OnDisconnect index failure!");
                        DuckNetwork.OnHostChange(pStationID, num <= pStationID);
                    }
                }
                List<Profile> profiles = DuckNetwork.GetProfiles(connection);
                bool flag = false;
                foreach (Profile profile in profiles)
                {
                    flag = true;
                    if (profile.connection != DuckNetwork.localConnection)
                    {
                        if (Network.isServer)
                        {
                            if (MonoMain.closingGame)
                                DuckNetwork.SendToEveryone(new NMClientClosedGame());
                            else
                                DuckNetwork.SendToEveryone(new NMClientDisconnect(profile.connection.identifier, profile));
                        }
                        bool reserve = false;
                        if (Network.InMatch())
                        {
                            if (reason == "CRASH")
                                HUD.AddPlayerChangeDisplay("@UNPLUG@|RED|" + profile.nameUI + " Crashed!");
                            else if (reason == "CLOSED")
                                HUD.AddPlayerChangeDisplay("@UNPLUG@|RED|" + profile.nameUI + " Closed Duck Game!");
                            else
                                HUD.AddPlayerChangeDisplay("@UNPLUG@|RED|" + profile.nameUI + " Disconnected!");
                            if (profile.slotType != SlotType.Open && !kicked)
                                reserve = true;
                        }
                        DevConsole.Log(DCSection.DuckNet, profile.nameUI + " |RED|Has left the DuckNet.");
                        int networkStatus = (int)profile.networkStatus;
                        DuckNetwork.ResetProfile(profile, reserve);
                        if (reserve && profile.slotType != SlotType.Spectator)
                        {
                            profile.slotType = SlotType.Reserved;
                            DuckNetwork.ChangeSlotSettings();
                        }
                        if (Level.core.gameInProgress && Network.InLobby())
                        {
                            if (profile.slotType != SlotType.Spectator)
                                profile.slotType = SlotType.Closed;
                            DuckNetwork.ChangeSlotSettings();
                        }
                    }
                    else
                        profile.networkStatus = DuckNetStatus.Disconnected;
                }
                if (DuckNetwork.status == DuckNetStatus.ConnectingToClients && DuckNetwork.hostProfile != null && DuckNetwork.hostProfile.connection != null)
                    DuckNetwork.CheckConnectingStatus();
                if (!flag || DuckNetwork.status == DuckNetStatus.Disconnecting || DuckNetwork.status == DuckNetStatus.Disconnected || !Network.isServer || Level.current is TeamSelect2 || !Level.current._waitingOnTransition && Level.current._startCalled)
                    return;
                Level.current = new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel));
            }
        }

        public static bool IsEstablishingConnection() => DuckNetwork.status == DuckNetStatus.EstablishingCommunicationWithServer || DuckNetwork.status == DuckNetStatus.ConnectingToServer || DuckNetwork.status == DuckNetStatus.ConnectingToClients;

        public static void OnHostChange(ulong pStationID, bool pLocal)
        {
            if (DuckNetwork.IsEstablishingConnection())
            {
                DevConsole.Log(DCSection.General, "|DGRED|Host changed but I'm still in the process of connecting.  This is bad since I have incomplete data, disconnecting.");
                Network.DisconnectClient(DuckNetwork.localConnection, new DuckNetErrorInfo(DuckNetError.ConnectionTimeout, "Host disconnected."));
            }
            else if (DuckNetwork.status == DuckNetStatus.Disconnected || DuckNetwork.status == DuckNetStatus.Disconnecting)
            {
                DevConsole.Log(DCSection.General, "|DGRED|Skipping host migration (DuckNetwork.status = " + DuckNetwork.status.ToString() + ").");
            }
            else
            {
                bool flag1 = pLocal;
                Network.isServer = flag1;
                bool flag2 = false;
                foreach (Profile profile in DuckNetwork.core.profiles)
                {
                    if (profile.connection != null)
                    {
                        profile.connection.isHost = false;
                        if (profile.networkIndex == (long)pStationID || pLocal && profile.connection == DuckNetwork.localConnection)
                        {
                            DuckNetwork.core.hostProfile = profile;
                            profile.connection.isHost = true;
                            DevConsole.Log(DCSection.NetCore, "Host migrating, new host is " + profile.connection?.ToString() + (flag1 ? "(local)" : "(remote)"));
                            flag2 = true;
                        }
                    }
                }
                if (!flag2)
                {
                    DevConsole.Log(DCSection.NetCore, "@error@|DGRED|Host migration failed to find new host connection (" + pStationID.ToString() + ")");
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.HostDisconnected, "Game Over: migration failed!"));
                }
                else
                {
                    if (!Network.isServer)
                        return;
                    if ((Level.current._waitingOnTransition || !Level.current._startCalled) && !(Level.current is TeamSelect2))
                        Level.current = new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel));
                    if (!(Level.current is RockScoreboard current))
                        return;
                    int controlMessage = current.controlMessage;
                    if (controlMessage > 0)
                    {
                        current.controlMessage = -1;
                        current.controlMessage = controlMessage;
                    }
                    Thing.Fondle(current.netCountdown, DuckNetwork.localConnection);
                }
            }
        }

        public static void OnSessionEnded()
        {
            DevConsole.Log(DCSection.DuckNet, "----------------Duck Network Session ENDED----------------");
            DuckNetwork._core.enteringText = false;
            DuckNetwork.Reset();
        }

        private static void CheckConnectingStatus()
        {
            if (DuckNetwork.status != DuckNetStatus.ConnectingToClients || DuckNetwork.hostProfile == null)
                return;
            bool flag = true;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection != null && profile.connection.status != ConnectionStatus.Connected)
                    flag = false;
            }
            if (!flag)
                return;
            Send.Message(new NMAllClientsConnected(), DuckNetwork.hostProfile.connection);
        }

        public static void OnConnection(NetworkConnection c)
        {
            switch (DuckNetwork.status)
            {
                case DuckNetStatus.EstablishingCommunicationWithServer:
                    if (!c.isHost)
                        break;
                    DevConsole.Log(DCSection.DuckNet, "Host contacted. Sending join request. ");
                    DuckNetwork._core.status = DuckNetStatus.ConnectingToServer;
                    DuckNetwork.SendJoinMessage(c);
                    break;
                case DuckNetStatus.ConnectingToClients:
                    DuckNetwork.CheckConnectingStatus();
                    break;
                default:
                    if (!c.isHost)
                        break;
                    DevConsole.Log(DCSection.DuckNet, "@error Host contacted. Join request not sent due to wrong status (" + DuckNetwork.status.ToString() + ")@error");
                    break;
            }
        }

        public static void Update()
        {
            if (MonoMain.pauseMenu == null && DuckNetwork._core._pauseOpen)
            {
                HUD.CloseAllCorners();
                DuckNetwork._core._pauseOpen = false;
                if (Network.isServer)
                    TeamSelect2.UpdateModifierStatus();
            }
            if (MonoMain.pauseMenu == null && DuckNetwork._core._willOpenSettingsInfo)
            {
                if (!DuckNetwork.ShouldKickForCustomContent())
                    HUD.AddPlayerChangeDisplay("@SETTINGSCHANGED@NEW MATCH SETTINGS");
                DuckNetwork._core._willOpenSettingsInfo = false;
            }
            if (MonoMain.pauseMenu == null && DuckNetwork._core._willOpenSpectatorInfo > 0)
            {
                DuckNetwork.DoSpectatorOpen(DuckNetwork._core._willOpenSpectatorInfo != 1);
                DuckNetwork._core._willOpenSpectatorInfo = 0;
            }
            if (DuckNetwork._core.status == DuckNetStatus.Disconnected || DuckNetwork._core.status == DuckNetStatus.Disconnecting)
            {
                DuckNetwork._core._quit.value = false;
            }
            else
            {
                if (DuckNetwork._core._quit.value)
                {
                    if (DuckNetwork._core._menuOpenProfile != null && DuckNetwork._core._menuOpenProfile.slotType == SlotType.Local)
                    {
                        DuckNetwork.Kick(DuckNetwork._core._menuOpenProfile);
                    }
                    else
                    {
                        if (Steam.lobby != null)
                            UIMatchmakingBox.core.nonPreferredServers.Add(Steam.lobby.id);
                        if (!DuckNetwork.finishedMatch)
                            DuckNetwork._xpEarned.Clear();
                        Level.current = new DisconnectFromGame();
                    }
                    DuckNetwork._core._quit.value = false;
                }
                if (DuckNetwork._core._returnToLobby.value && Network.isServer)
                {
                    Level.current = new TeamSelect2(true);
                    DuckNetwork._core._returnToLobby.value = false;
                }
                if (DuckNetwork._core._menuClosed.value)
                {
                    if (Level.current is TeamSelect2)
                        Send.Message(new NMNumCustomLevels(Editor.activatedLevels.Count));
                    if (Level.current is TeamSelect2)
                    {
                        if (TeamSelect2.GetMatchSettingString() != DuckNetwork._core._settingsBeforeOpen)
                        {
                            TeamSelect2.SendMatchSettings();
                            Send.Message(new NMMatchSettingsChanged());
                        }
                        Network.activeNetwork.core.ApplyLobbyData();
                    }
                    DuckNetwork._core._menuClosed.value = false;
                }
                if (Keyboard.Pressed(Keys.F1) && !Keyboard.Down(Keys.LeftShift) && !Keyboard.Down(Keys.RightShift))
                    ConnectionStatusUI.Show();
                if (DuckNetwork.core.logTransferSize > 0)
                    ConnectionStatusUI.core.tempShow = 2;
                if (Keyboard.Released(Keys.F1))
                    ConnectionStatusUI.Hide();
                int num1;
                switch (MonoMain.pauseMenu)
                {
                    case null:
                    case UILevelBox _:
                    case UIGachaBoxNew _:
                    case UIFuneral _:
                        num1 = 1;
                        break;
                    default:
                        num1 = MonoMain.pauseMenu is UIGachaBox ? 1 : 0;
                        break;
                }
                bool flag1 = num1 != 0;
                if (!flag1)
                {
                    DuckNetwork._core.enteringText = false;
                    DuckNetwork._core.stopEnteringText = false;
                }
                bool flag2 = false;
                if (Network.isActive & flag1)
                {
                    List<ChatMessage> chatMessageList = new List<ChatMessage>();
                    foreach (ChatMessage chatMessage in DuckNetwork._core.chatMessages)
                    {
                        chatMessage.timeout -= 0.016f;
                        if (chatMessage.timeout < 0.0)
                            chatMessage.alpha -= 0.01f;
                        if (chatMessage.alpha < 0.0)
                            chatMessageList.Add(chatMessage);
                    }
                    foreach (ChatMessage chatMessage in chatMessageList)
                        DuckNetwork._core.chatMessages.Remove(chatMessage);
                    if (DuckNetwork._core.stopEnteringText)
                    {
                        DuckNetwork._core.enteringText = false;
                        DuckNetwork._core.stopEnteringText = false;
                    }
                    if (!DevConsole.open)
                    {
                        bool enteringText = DuckNetwork._core.enteringText;
                        DuckNetwork._core.enteringText = false;
                        int num2 = !Input.Pressed("CHAT") ? 0 : (!Keyboard.alt ? 1 : (!Keyboard.Pressed(Keys.Enter) ? 1 : 0));
                        DuckNetwork._core.enteringText = enteringText;
                        if (num2 != 0)
                        {
                            if (!DuckNetwork._core.enteringText)
                            {
                                DuckNetwork._core.enteringText = true;
                                DuckNetwork._core.currentEnterText = "";
                                Keyboard.keyString = "";
                            }
                            else
                            {
                                if (DuckNetwork._core.currentEnterText != "")
                                {
                                    string currentEnterText = DuckNetwork._core.currentEnterText;
                                    if (currentEnterText.StartsWith("/steal"))
                                    {
                                        string[] source = currentEnterText.Split(':');
                                        if (source.Count<string>() == 3)
                                            DuckFile.StealMoji(source[1]);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            foreach (KeyValuePair<string, Sprite> moji in DuckFile.mojis)
                                            {
                                                if (!moji.Key.EndsWith("!"))
                                                {
                                                    string str = ":" + moji.Key + ":";
                                                    if (!currentEnterText.Contains(str))
                                                        str = "@" + moji.Key + "@";
                                                    if (currentEnterText.Contains(str))
                                                    {
                                                        foreach (Profile profile in DuckNetwork.profiles)
                                                        {
                                                            if (profile.connection != null && profile.connection != DuckNetwork.localConnection && (!profile.isHost || profile == DuckNetwork.hostProfile) && !profile.sentMojis.Contains(str))
                                                            {
                                                                if (moji.Value.texture.width <= 28 && moji.Value.texture.height <= 28)
                                                                    Send.Message(new NMMojiData(Editor.TextureToString((Texture2D)moji.Value.texture), moji.Key), profile.connection);
                                                                profile.sentMojis.Add(str);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                        NMChatMessage nmChatMessage = new NMChatMessage(DuckNetwork._core.localProfile, currentEnterText, DuckNetwork._core.chatIndex);
                                        ++DuckNetwork._core.chatIndex;
                                        DuckNetwork.SendToEveryoneFiltered(nmChatMessage);
                                        DuckNetwork.ChatMessageReceived(nmChatMessage, DuckNetwork._core.currentEnterText);
                                    }
                                    DuckNetwork._core.currentEnterText = "";
                                }
                                DuckNetwork._core.stopEnteringText = true;
                            }
                        }
                        else if (DuckNetwork._core.enteringText && Keyboard.Pressed(Keys.Escape))
                        {
                            DuckNetwork._core.stopEnteringText = true;
                            flag2 = true;
                        }
                    }
                    if (DuckNetwork._core.enteringText)
                    {
                        Input._imeAllowed = true;
                        if (Keyboard.keyString.Length > 90)
                            Keyboard.keyString = Keyboard.keyString.Substring(0, 90);
                        Keyboard.keyString = Keyboard.keyString.Replace("\n", "");
                        DuckNetwork._core.currentEnterText = Keyboard.keyString;
                    }
                }
                bool flag3 = false;
                int num3 = 0;
                DuckNetwork._processedConnections.Clear();
                Profile profile1 = null;
                foreach (Profile profile2 in DuckNetwork.profiles)
                {
                    if (profile2.pendingSpectatorMode != SlotType.Max && Network.isServer && (Level.current is TeamSelect2 || VirtualTransition.active))
                        DuckNetwork._spectatorSwaps.Add(profile2);
                    if (NetworkDebugger.enabled && profile2.inputProfile is BullshitInput)
                        profile2.inputProfile.UpdateExtraInput();
                    profile2.TickConnectionTrouble();
                    if (profile2.connection == DuckNetwork.localConnection && profile2.inputProfile != null)
                    {
                        if (profile2.duck == null || profile2.duck.dead)
                        {
                            if (profile2.inputProfile.Pressed("QUACK"))
                                profile2.netData.Set<bool>("quack", true);
                            else if (profile2.inputProfile.Released("QUACK"))
                                profile2.netData.Set<bool>("quack", false);
                        }
                        if (profile2.slotType == SlotType.Spectator)
                        {
                            switch (Level.current)
                            {
                                case TeamSelect2 _:
                                case RockScoreboard _:
                                    if (MonoMain.pauseMenu == null)
                                    {
                                        float leftTrigger = profile2.inputProfile.leftTrigger;
                                        if (profile2.inputProfile.hasMotionAxis)
                                            leftTrigger += profile2.inputProfile.motionAxis;
                                        profile2.netData.Set<float>("quackPitch", leftTrigger);
                                        if (profile2.inputProfile.Pressed("RAGDOLL"))
                                            profile2.netData.Set<bool>("spectatorHat", !profile2.netData.Get<bool>("spectatorHat", true));
                                        if (profile2.inputProfile.Pressed("STRAFE") && !profile2.inputProfile.Down("RAGDOLL"))
                                            profile2.netData.Set<bool>("spectatorFlip", !profile2.netData.Get<bool>("spectatorFlip", false));
                                        if (profile2.inputProfile.Pressed("GRAB"))
                                        {
                                            profile2.netData.Set<sbyte>("spectatorBeverage", (sbyte)(profile2.netData.Get<sbyte>("spectatorBeverage", -1) + 1));
                                            if (profile2.netData.Get<sbyte>("spectatorBeverage", -1) > 13)
                                                profile2.netData.Set<sbyte>("spectatorBeverage", -1);
                                        }
                                        if (profile2.netData.Get<sbyte>("spectatorPersona", -1) == -1)
                                            profile2.netData.Set<sbyte>("spectatorPersona", (sbyte)profile2.persona.index);
                                        if (profile2.inputProfile.Down("RAGDOLL") && profile2.inputProfile.Pressed("STRAFE") || profile2.inputProfile.Down("STRAFE") && profile2.inputProfile.Pressed("RAGDOLL"))
                                        {
                                            profile2.netData.Set<sbyte>("spectatorPersona", (sbyte)(profile2.netData.Get<sbyte>("spectatorPersona", -1) + 1));
                                            if (profile2.netData.Get<sbyte>("spectatorPersona", 0) > 7)
                                                profile2.netData.Set<sbyte>("spectatorPersona", 0);
                                        }
                                        profile2.netData.Set<Vec2>("spectatorTongue", profile2.inputProfile.rightStick);
                                        Vec2 pValue = Vec2.Zero;
                                        if ((double)profile2.inputProfile.leftStick.length < 0.0500000007450581)
                                        {
                                            if (profile2.inputProfile.Down("LEFT"))
                                                pValue += new Vec2(-1f, 0.0f);
                                            if (profile2.inputProfile.Down("RIGHT"))
                                                pValue += new Vec2(1f, 0.0f);
                                            if (profile2.inputProfile.Down("DOWN"))
                                                pValue += new Vec2(0.0f, -1f);
                                            if (profile2.inputProfile.Down("UP"))
                                                pValue += new Vec2(0.0f, 1f);
                                        }
                                        else
                                            pValue = profile2.inputProfile.leftStick;
                                        if (profile2.inputProfile.Down("SHOOT"))
                                        {
                                            profile2.netData.Set<Vec2>("spectatorBob", pValue);
                                            break;
                                        }
                                        profile2.netData.Set<Vec2>("spectatorTilt", pValue);
                                        break;
                                    }
                                    break;
                            }
                        }
                        profile2.netData.Set<bool>("gamePaused", MonoMain.pauseMenu != null);
                        profile2.netData.Set<bool>("gameInFocus", DuckGame.Graphics.inFocus);
                        profile2.netData.SetFiltered<bool>("chatting", DuckNetwork._core.enteringText);
                        profile2.netData.Set<bool>("consoleOpen", DevConsole.open);
                        if (MonoMain.pauseMenu == null && (DuckNetwork._ducknetUIGroup == null || !DuckNetwork._ducknetUIGroup.open) && profile2.inputProfile.Pressed("START") && !flag2 && !flag3 && (Network.InLobby() || Network.InGameLevel()) && (!(Level.current is TeamSelect2) || !(Level.current as TeamSelect2).HasBoxOpen(profile2)))
                        {
                            DuckNetwork.OpenMenu(profile2);
                            flag3 = true;
                        }
                    }
                    if (profile2.connection != null && !DuckNetwork._processedConnections.Contains(profile2.connection))
                    {
                        DuckNetwork._processedConnections.Add(profile2.connection);
                        if (Network.isServer && profile2.connection != DuckNetwork.localConnection && profile2.connection.status == ConnectionStatus.Connected && profile2.networkStatus == DuckNetStatus.Connected && (profile1 == null || profile1.networkIndex > profile2.networkIndex))
                            profile1 = profile2;
                        ++num3;
                        if (profile2.connection.status == ConnectionStatus.Connected && profile2.networkStatus != DuckNetStatus.Disconnecting && profile2.networkStatus != DuckNetStatus.Disconnected && profile2.networkStatus != DuckNetStatus.Failure)
                            profile2.currentStatusTimeout -= Maths.IncFrameTimer();
                    }
                }
                if (profile1 != null)
                    DuckNetwork.potentialHostObject = profile1.connection.data;
                if (DuckNetwork._spectatorSwaps.Count <= 0)
                    return;
                if (Network.isServer)
                {
                    foreach (Profile spectatorSwap in DuckNetwork._spectatorSwaps)
                    {
                        if (spectatorSwap.pendingSpectatorMode == SlotType.Spectator && spectatorSwap.slotType != SlotType.Spectator)
                        {
                            Profile pReplacementSlot = DuckNetwork.profiles.FirstOrDefault<Profile>(x => x.slotType == SlotType.Spectator && x.connection == null);
                            if (pReplacementSlot != null)
                                DuckNetwork.MakeSpectator_Swap(spectatorSwap, pReplacementSlot);
                        }
                        else if (spectatorSwap.pendingSpectatorMode == SlotType.Open && spectatorSwap.slotType == SlotType.Spectator)
                        {
                            Profile pReplacementSlot = DuckNetwork.profiles.FirstOrDefault<Profile>(x => x.slotType != SlotType.Spectator && x.slotType != SlotType.Reserved && x.slotType != SlotType.Closed && x.connection == null) ?? DuckNetwork.profiles.FirstOrDefault<Profile>(x => x.slotType != SlotType.Spectator && x.slotType != SlotType.Reserved && x.connection == null);
                            if (pReplacementSlot != null)
                                DuckNetwork.MakePlayer_Swap(spectatorSwap, pReplacementSlot);
                        }
                        spectatorSwap.pendingSpectatorMode = SlotType.Max;
                    }
                    DuckNetwork.TryPeacefulResolution();
                }
                DuckNetwork._spectatorSwaps.Clear();
            }
        }

        public static bool TryPeacefulResolution(bool pDoLevelSwitch = true)
        {
            if (Network.isActive)
            {
                if (pDoLevelSwitch)
                {
                    switch (Level.current)
                    {
                        case TeamSelect2 _:
                        case IConnectionScreen _:
                        case TitleScreen _:
                            goto label_26;
                    }
                }
                int num = 0;
                foreach (Team team in Teams.all)
                {
                    if (team.activeProfiles.Count > 0)
                        ++num;
                }
                if (num <= 1)
                {
                    if (pDoLevelSwitch && Network.isServer)
                    {
                        bool pReturningFromGame = false;
                        foreach (Profile profile in DuckNetwork.profiles)
                        {
                            if (profile.reservedUser != null)
                                pReturningFromGame = true;
                        }
                        if (!pReturningFromGame)
                        {
                            DuckNetwork.ResetScores();
                            foreach (Profile profile in DuckNetwork.profiles)
                            {
                                if (profile.slotType != SlotType.Spectator)
                                    profile.slotType = profile.originalSlotType;
                            }
                        }
                        Level.current = new TeamSelect2(pReturningFromGame);
                    }
                    return true;
                }
            }
        label_26:
            return false;
        }

        public static void ChatMessageReceived(NMChatMessage message) => DuckNetwork.ChatMessageReceived(message, null);

        private static int FilterPlayer(Profile pProfile)
        {
            if (pProfile != null && pProfile.connection != DuckNetwork.localConnection && pProfile.connection != null)
            {
                int num = Options.Data.chatMode;
                if (DG.di == 1)
                    num = 0;
                bool flag1 = false;
                bool flag2 = false;
                if (NetworkDebugger.enabled)
                {
                    if (pProfile.muteChat)
                    {
                        flag1 = true;
                        flag2 = true;
                    }
                }
                else if (pProfile.connection.data is User && pProfile.muteChat)
                {
                    flag1 = true;
                    flag2 = true;
                }
                if (num > 0)
                {
                    bool flag3 = false;
                    if (pProfile.connection.data is User data && data.relationship == FriendRelationship.Friend)
                        flag3 = true;
                    if (num == 1 && !flag3 && !DuckNetwork.invited)
                        flag1 = true;
                    else if (num == 2 && !flag3)
                        flag1 = true;
                }
                if (flag1 & flag2)
                    return 2;
                if (flag1)
                    return 1;
            }
            return 0;
        }

        public static void ChatMessageReceived(NMChatMessage message, string realText)
        {
            if (message.profile == null)
                return;
            int num = DuckNetwork.FilterPlayer(message.profile);
            if (num > 0)
            {
                if (message.connection.sentFilterMessage)
                    return;
                message.connection.sentFilterMessage = true;
                realText = !(message is NMChatDisabledMessage) ? (num != 2 ? "@error@Chat |DGRED|disabled|PREV| in options. Ignoring messages..." : "@error@Player is |DGRED|muted|PREV|. Ignoring messages...") : message.text;
            }
            DuckNetwork._core.AddChatMessage(new ChatMessage(message.profile, realText != null ? realText : message.text, message.index));
            SFX.Play("chatmessage", 0.8f, Rando.Float(-0.15f, 0.15f));
        }

        public static List<Profile> GetProfiles(NetworkConnection connection)
        {
            List<Profile> profiles = new List<Profile>();
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection == connection)
                    profiles.Add(profile);
            }
            return profiles;
        }

        public static List<Profile> GetProfiles(string identifier)
        {
            List<Profile> profiles = new List<Profile>();
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection != null && profile.connection.identifier == identifier)
                    profiles.Add(profile);
            }
            return profiles;
        }

        public static int IndexOf(Profile p) => DuckNetwork.profiles.IndexOf(p);

        public static void SendSpecialHat(Team pTeam, NetworkConnection pTo)
        {
        }

        public static void SendAllPlayerMetaData(Profile who, NetworkConnection to)
        {
            if (who.team != null)
            {
                if (who.team.customData != null)
                    Send.Message(new NMSpecialHat(who.team, who, to.profile != null && to.profile.muteHat), to);
                Send.Message(new NMSetTeam(who, who.team, who.team.customData != null), to);
            }
            if (who.furniturePositions.Count > 0)
                Send.Message(new NMRoomData(who, who.furniturePositionData), to);
            Send.Message(new NMProfileInfo(who, who.stats.unloyalFans, who.stats.loyalFans, who.ParentalControlsActive, who.flagIndex, (ushort)Teams.core.extraTeams.Count, Teams.core.teams), to);
            Send.Message(new NMNumCustomLevels(Editor.activatedLevels.Count), to);
            Send.Message(new NMOldAngles(who, Options.Data.oldAngleCode), to);
        }

        public static void Server_SendProfile(Profile p, NetworkConnection to) => Send.Message(new NMNewDuckNetConnection(p, p.connection == DuckNetwork.localConnection ? (p.isRemoteLocalDuck ? "SERVERLOCAL" : "SERVER") : p.connection.identifier, p.name, p.team, p.flippers, p.ParentalControlsActive, p.flagIndex, p.steamID, (byte)p.persona.index), to);

        public static void Server_AcceptJoinRequest(List<Profile> pJoinedProfiles, bool pLocal = false)
        {
            if (UIMatchmakerMark2.instance != null)
                UIMatchmakerMark2.instance.Hook_OnDucknetJoined();
            Send.Message(new NMNetworkIndexSync());
            if (!pLocal)
            {
                Send.Message(new NMJoinDuckNetSuccess(pJoinedProfiles), pJoinedProfiles[0].connection);
                List<byte> byteList = new List<byte>();
                for (int index = 0; index < DG.MaxPlayers; ++index)
                    byteList.Add((byte)DuckNetwork.profiles[index].slotType);
                TeamSelect2.SendMatchSettings(pJoinedProfiles[0].connection, true);
                DuckNetwork.ChangeSlotSettings(true);
            }
            DuckNetwork.Server_SendAllProfiles(pJoinedProfiles, pLocal);
            foreach (Profile profile in DuckNetwork.GetProfiles(DuckNetwork.localConnection))
                DuckNetwork.SendAllPlayerMetaData(profile, pJoinedProfiles[0].connection);
            Send.Message(new NMEndOfDuckNetworkData(), pJoinedProfiles[0].connection);
        }

        public static void MakeSpectator(Profile pProfile) => pProfile.pendingSpectatorMode = SlotType.Spectator;

        public static void MakeSpectator_Swap(Profile pProfile, Profile pReplacementSlot)
        {
            if (pProfile.slotType == SlotType.Spectator || pReplacementSlot.slotType != SlotType.Spectator || pProfile.networkIndex >= DG.MaxPlayers)
                return;
            ++pProfile.spectatorChangeIndex;
            if (pProfile.connection == DuckNetwork.localConnection)
                pProfile.remoteSpectatorChangeIndex = pProfile.spectatorChangeIndex;
            if (Network.isServer)
                Send.Message(new NMMakeSpectator(pProfile, pReplacementSlot, pProfile.spectatorChangeIndex));
            if (Level.current is TeamSelect2)
            {
                ProfileBox2 profile = (Level.current as TeamSelect2)._profiles[pProfile.networkIndex];
                profile.Despawn();
                profile.SetProfile(pReplacementSlot);
            }
            DuckNetwork.SwapProfileIndices(pProfile, pReplacementSlot);
            pReplacementSlot.slotType = pProfile.slotType;
            pReplacementSlot.team = null;
            pReplacementSlot.duck = null;
            pReplacementSlot.persona = pReplacementSlot.networkDefaultPersona;
            pProfile.slotType = SlotType.Spectator;
            pProfile.team.Leave(pProfile, false);
            pProfile.duck = null;
        }

        public static void MakePlayer(Profile pProfile) => pProfile.pendingSpectatorMode = SlotType.Open;

        public static void MakePlayer_Swap(Profile pProfile, Profile pReplacementSlot)
        {
            if (pProfile.slotType != SlotType.Spectator || pReplacementSlot.slotType == SlotType.Spectator || pReplacementSlot.networkIndex >= DG.MaxPlayers)
                return;
            ++pProfile.spectatorChangeIndex;
            if (Network.isServer)
                Send.Message(new NMMakePlayer(pProfile, pReplacementSlot, pProfile.spectatorChangeIndex));
            if (Level.current is TeamSelect2)
                (Level.current as TeamSelect2)._profiles[pReplacementSlot.networkIndex].SetProfile(pProfile);
            DuckNetwork.SwapProfileIndices(pProfile, pReplacementSlot);
            pProfile.slotType = pReplacementSlot.slotType;
            if (pProfile.team != null && (pProfile.team.activeProfiles.Count == 0 || TeamSelect2.GetSettingBool("teams")) && !pProfile.team.defaultTeam)
                pProfile.team.Join(pProfile, false);
            else
                pProfile.team = pProfile.networkDefaultTeam;
            pProfile.persona = pProfile.networkDefaultPersona;
            pProfile.duck = null;
            pReplacementSlot.slotType = SlotType.Spectator;
            pReplacementSlot.team = null;
            pReplacementSlot.duck = null;
        }

        public static void SwapProfileIndices(Profile pProfile, Profile pReplacementSlot)
        {
            DuckNetwork.profiles[pProfile.networkIndex] = pReplacementSlot;
            DuckNetwork.profiles[pReplacementSlot.networkIndex] = pProfile;
            byte networkIndex = pProfile.networkIndex;
            pProfile.SetNetworkIndex(pReplacementSlot.networkIndex);
            pReplacementSlot.SetNetworkIndex(networkIndex);
        }

        public static void Server_SendAllProfiles(List<Profile> pTo, bool local = false)
        {
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.networkStatus != DuckNetStatus.Disconnecting && !pTo.Contains(profile) && profile.connection != null && (!local || profile.connection != DuckNetwork.localConnection))
                {
                    if (!local)
                        DuckNetwork.Server_SendProfile(profile, pTo[0].connection);
                    if (profile.connection != DuckNetwork.localConnection)
                    {
                        foreach (Profile p in pTo)
                            DuckNetwork.Server_SendProfile(p, profile.connection);
                    }
                }
            }
        }

        public static NMVersionMismatch.Type CheckVersion(string id)
        {
            string[] strArray = id.Split('.');
            NMVersionMismatch.Type type = NMVersionMismatch.Type.Match;
            if (strArray.Length == 4)
            {
                try
                {
                    int int32_1 = Convert.ToInt32(strArray[3]);
                    int int32_2 = Convert.ToInt32(strArray[2]);
                    int int32_3 = Convert.ToInt32(strArray[1]);
                    if (int32_2 < DG.versionHigh || int32_3 < DG.versionMajor)
                        type = NMVersionMismatch.Type.Older;
                    else if (int32_2 > DG.versionHigh || int32_3 > DG.versionMajor)
                        type = NMVersionMismatch.Type.Newer;
                    else if (int32_1 < DG.versionLow)
                        type = NMVersionMismatch.Type.Older;
                    else if (int32_1 > DG.versionLow)
                        type = NMVersionMismatch.Type.Newer;
                }
                catch
                {
                    type = NMVersionMismatch.Type.Error;
                }
            }
            return type;
        }

        public static NetMessage OnMessageFromNewClient(NetMessage m)
        {
            if (Network.isServer)
            {
                switch (m)
                {
                    case NMRequestJoin _:
                        if (!DuckNetwork.inGame)
                        {
                            switch (Level.current)
                            {
                                case TeamSelect2 _:
                                case IConnectionScreen _:
                                    NMRequestJoin nmRequestJoin = m as NMRequestJoin;
                                    if (nmRequestJoin.names == null || nmRequestJoin.names.Count == 0)
                                        return new NMErrorEmptyJoinMessage();
                                    DevConsole.Log(DCSection.DuckNet, "Join attempt from " + nmRequestJoin.names.First<string>());
                                    if (DuckNetwork.GetOpenProfiles(m.connection, nmRequestJoin.wasInvited, false, false).Count<Profile>() < nmRequestJoin.names.Count)
                                    {
                                        DevConsole.Log(DCSection.DuckNet, "@error " + nmRequestJoin.names[0] + " could not join, server is full.@error");
                                        return new NMServerFull();
                                    }
                                    if (nmRequestJoin.password != DuckNetwork.core.serverPassword && !DuckNetwork.core._invitedFriends.Contains(nmRequestJoin.localID))
                                    {
                                        DevConsole.Log(DCSection.DuckNet, "@error " + nmRequestJoin.names[0] + " could not join, password was incorrect.@error");
                                        return new NMInvalidPassword();
                                    }
                                    List<Profile> pJoinedProfiles = new List<Profile>();
                                    int index = 0;
                                    foreach (string name in nmRequestJoin.names)
                                    {
                                        Profile profile = DuckNetwork.ServerCreateProfile(m.connection, null, null, name, false, nmRequestJoin.wasInvited, false);
                                        profile.ParentalControlsActive = nmRequestJoin.info.parentalControlsActive;
                                        profile.flippers = nmRequestJoin.info.roomFlippers;
                                        profile.flagIndex = nmRequestJoin.info.flagIndex;
                                        profile.networkStatus = DuckNetStatus.Connected;
                                        profile.steamID = nmRequestJoin.localID;
                                        if (nmRequestJoin.personas.Count > index)
                                        {
                                            byte persona = nmRequestJoin.personas[index];
                                            if (persona >= 0 && persona < Persona.all.Count<DuckPersona>())
                                            {
                                                profile.preferredColor = persona;
                                                DuckNetwork.RequestPersona(profile, Persona.all.ElementAt<DuckPersona>(persona), false);
                                            }
                                        }
                                        DuckNetwork._core.status = DuckNetStatus.Connected;
                                        Level.current.OnNetworkConnecting(profile);
                                        pJoinedProfiles.Add(profile);
                                        ++index;
                                    }
                                    DuckNetwork.Server_AcceptJoinRequest(pJoinedProfiles);
                                    return null;
                            }
                        }
                        return new NMGameInProgress();
                    case NMMessageIgnored _:
                        return null;
                }
            }
            else
            {
                switch (m)
                {
                    case NMRequestJoin _:
                        DevConsole.Log(DCSection.DuckNet, "@error Received NMRequestJoin, but you're not the host!!.@error");
                        return new NMGameInProgress();
                    case NMMessageIgnored _:
                        return null;
                }
            }
            return new NMMessageIgnored();
        }

        private static void AttemptReconnect(NetworkConnection fromConnection, string toConnection)
        {
            List<Profile> profiles1 = DuckNetwork.GetProfiles(fromConnection);
            if (profiles1.Count > 0)
            {
                List<Profile> profiles2 = DuckNetwork.GetProfiles(toConnection);
                foreach (Profile p1 in profiles2)
                {
                    DuckNetwork.Server_SendProfile(p1, fromConnection);
                    if (p1.connection != DuckNetwork.localConnection)
                    {
                        foreach (Profile p2 in profiles1)
                            DuckNetwork.Server_SendProfile(p2, p1.connection);
                    }
                    DevConsole.Log(DCSection.DuckNet, fromConnection.name + " needs a connection to " + p1.connection.name + "...");
                }
                if (profiles2.Count != 0)
                    return;
                Send.Message(new NMNoConnectionExists(toConnection), fromConnection);
                DevConsole.Log(DCSection.DuckNet, "@error Client requested reconnect whith non-existing client!(to " + toConnection + ")@error", fromConnection);
            }
            else
            {
                Send.Message(new NMInvalidUser(), fromConnection);
                DevConsole.Log(DCSection.DuckNet, "@error An outside user not in this game requested a reconnect!?@error", fromConnection);
            }
        }

        public static bool HandleCoreConnectionMessages(NetMessage m)
        {
            if (m is NMAllClientsConnected)
            {
                if (Network.isServer)
                {
                    Send.Message(new NMRightOnManNiceJob(), m.connection);
                    Send.Message(new NMLevel(Level.current), m.connection);
                }
            }
            else if (m is NMRightOnManNiceJob)
            {
                if (DuckNetwork.status == DuckNetStatus.ConnectingToClients)
                {
                    DuckNetwork._core.status = DuckNetStatus.Connected;
                    if (UIMatchmakerMark2.instance != null)
                        UIMatchmakerMark2.instance.Hook_OnDucknetJoined();
                }
            }
            else
            {
                if (m is NMClientNeedsLevelData)
                {
                    NMClientNeedsLevelData clientNeedsLevelData = m as NMClientNeedsLevelData;
                    if (clientNeedsLevelData.levelIndex == levelIndex)
                    {
                        m.connection.dataTransferProgress = 0;
                        m.connection.dataTransferSize = 0;
                        DuckNetwork.SendCurrentLevelData(clientNeedsLevelData.transferSession, m.connection);
                        Level.current.ChecksumReplied(clientNeedsLevelData.connection);
                    }
                    return true;
                }
                if (m is NMLevelFileReady)
                {
                    NMLevelFileReady nmLevelFileReady = m as NMLevelFileReady;
                    if (Level.current != null && Level.current.networkIndex == nmLevelFileReady.levelIndex)
                        Level.current.ChecksumReplied(nmLevelFileReady.connection);
                    return true;
                }
                if (m is NMLevelDataHeader)
                {
                    NMLevelDataHeader nmLevelDataHeader = m as NMLevelDataHeader;
                    if (_core.levelTransferSession == nmLevelDataHeader.transferSession)
                    {
                        DuckNetwork._core.levelTransferProgress = 0;
                        DuckNetwork._core.levelTransferSize = nmLevelDataHeader.length;
                        DuckNetwork._currentTransferLevelName = nmLevelDataHeader.levelName;
                    }
                    return true;
                }
                if (m is NMLevelDataChunk)
                {
                    NMLevelDataChunk nmLevelDataChunk = m as NMLevelDataChunk;
                    if (_core.levelTransferSession == nmLevelDataChunk.transferSession)
                    {
                        DuckNetwork._core.levelTransferProgress += nmLevelDataChunk.GetBuffer().lengthInBytes;
                        if (DuckNetwork._core.compressedLevelData == null)
                            DuckNetwork._core.compressedLevelData = new MemoryStream();
                        DuckNetwork._core.compressedLevelData.Write(nmLevelDataChunk.GetBuffer().buffer, 0, nmLevelDataChunk.GetBuffer().lengthInBytes);
                        if (DuckNetwork._core.levelTransferProgress == DuckNetwork._core.levelTransferSize)
                        {
                            if (!(Level.core.nextLevel is XMLLevel xmlLevel))
                                xmlLevel = Level.core.currentLevel as XMLLevel;
                            if (xmlLevel == null)
                            {
                                DevConsole.Log("|DGRED|Tried to apply NMLevelDataChunk while connecting/disconnecting. Ignoring...");
                                return true;
                            }
                            xmlLevel.synchronizedLevelName = DuckNetwork._currentTransferLevelName;
                            if (!xmlLevel.ApplyLevelData(Editor.ReadCompressedLevelData(DuckNetwork._core.compressedLevelData)))
                                Network.DisconnectClient(DuckNetwork.localConnection, new DuckNetErrorInfo(DuckNetError.ParentalControls, "Disconnecting - Error storing custom map."));
                            xmlLevel.ChecksumReplied(nmLevelDataChunk.connection);
                            if (!Network.isServer)
                                Send.Message(new NMLevelFileReady(xmlLevel.networkIndex), DuckNetwork.hostProfile.connection);
                        }
                    }
                    return true;
                }
            }
            if (m is NMChatMessage)
            {
                NMChatMessage message = m as NMChatMessage;
                message.index = DuckNetwork._core.chatIndex;
                ++DuckNetwork._core.chatIndex;
                DuckNetwork.ChatMessageReceived(message);
                return true;
            }
            if (Network.isServer)
            {
                if (m is NMRequiresNewConnection)
                {
                    NMRequiresNewConnection requiresNewConnection = m as NMRequiresNewConnection;
                    DuckNetwork.AttemptReconnect(requiresNewConnection.connection, requiresNewConnection.toWhom);
                    return true;
                }
            }
            else if (m is NMKicked || m is NMBanned)
            {
                Profile profile = DuckNetwork.profiles.FirstOrDefault<Profile>(x => x == (m as NMKicked).profile);
                if (profile != null)
                {
                    if (profile.connection != null)
                    {
                        DevConsole.Log(DCSection.DuckNet, "|DGRED|" + profile.connection.ToString() + " has been " + (m is NMKicked ? "kicked" : "banned") + "...");
                        NetworkConnection connection = profile.connection;
                        if (m is NMBanned)
                            connection.banned = true;
                        profile.connection.kicking = true;
                        Network.DisconnectClient(profile.connection, new DuckNetErrorInfo(DuckNetError.Kicked, ""));
                    }
                }
            }
            else
            {
                if (m is NMKick)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    if (Steam.lobby != null)
                        UIMatchmakingBox.core.blacklist.Add(Steam.lobby.id);
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.Kicked, "|RED|Oh no! The host kicked you :("));
                    return true;
                }
                if (m is NMBan)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    if (Steam.lobby != null)
                    {
                        UIMatchmakingBox.core.nonPreferredServers.Add(Steam.lobby.id);
                        UIMatchmakingBox.core.blacklist.Add(Steam.lobby.id);
                    }
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.Kicked, "|RED|Oh no!! The host banned you :("));
                    return true;
                }
                if (m is NMClientDisconnect)
                {
                    NMClientDisconnect clientDisconnect = m as NMClientDisconnect;
                    if (clientDisconnect.profile != null && clientDisconnect.profile.connection != null)
                    {
                        bool flag = clientDisconnect.profile.connection.identifier == clientDisconnect.whom;
                        if (clientDisconnect.whom == "local")
                            flag = clientDisconnect.profile.connection == clientDisconnect.connection;
                        if (flag)
                        {
                            if (clientDisconnect.profile.connection == Network.host && clientDisconnect.profile != DuckNetwork.hostProfile)
                            {
                                clientDisconnect.profile.networkStatus = DuckNetStatus.Disconnected;
                                clientDisconnect.profile.connection = null;
                                clientDisconnect.profile.team = null;
                                DevConsole.Log(DCSection.DuckNet, "Host local duck left", m.connection);
                            }
                            else
                            {
                                Network.activeNetwork.core.DisconnectClient(clientDisconnect.profile.connection, new DuckNetErrorInfo(DuckNetError.ClientDisconnected, "Client disconnected."));
                                DevConsole.Log(DCSection.DuckNet, "Client disconnected", m.connection);
                            }
                        }
                    }
                    return true;
                }
                if (m is NMErrorEmptyJoinMessage)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.GameInProgress, "|RED|No ducks, please retry."));
                    return true;
                }
                if (m is NMGameInProgress)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.GameInProgress, "|RED|Game was already in progress."));
                    return true;
                }
                if (m is NMServerFull)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.FullServer, "|RED|The game was full!"));
                    return true;
                }
                if (m is NMInvalidPassword)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.InvalidPassword, "|RED|Password was incorrect!"));
                    return true;
                }
                if (m is NMInvalidLevel)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.InvalidLevel, "|RED|Level request was invalid!"));
                    return true;
                }
                if (m is NMInvalidUser)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.InvalidUser, "|RED|The host did not reconize you!"));
                    return true;
                }
                if (m is NMInvalidCustomHat)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.InvalidCustomHat, "|RED|Your custom hat was invalid!"));
                    return true;
                }
                if (m is NMVersionMismatch)
                {
                    DuckNetwork._core.status = DuckNetStatus.Failure;
                    NMVersionMismatch nmVersionMismatch = m as NMVersionMismatch;
                    DuckNetwork.FailWithVersionMismatch(nmVersionMismatch.serverVersion, nmVersionMismatch.GetCode());
                    return true;
                }
            }
            return false;
        }

        public static void FailWithVersionMismatch(string theirVersion, NMVersionMismatch.Type type)
        {
            Steam.MarkForUpdateCheck();
            DuckNetwork._core.status = DuckNetStatus.Failure;
            Network.EndNetworkingSession(DuckNetwork.AssembleMismatchError(theirVersion));
        }

        public static void FailWithDatahashMismatch()
        {
            Steam.MarkForUpdateCheck();
            DuckNetwork._core.status = DuckNetStatus.Failure;
            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ModsIncompatible, "|RED|Game data does not match the host's!\n    Please ensure game versions\n           are the same."));
        }

        public static void FailWithModDatahashMismatch(Mod pMod)
        {
            Steam.MarkForUpdateCheck();
            DuckNetwork._core.status = DuckNetStatus.Failure;
            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ModsIncompatible, "|RED|Mod data mismatch:\n   |DGBLUE|" + pMod.configuration.displayName + "|PREV|\n"));
        }

        public static void FailWithDifferentModsError()
        {
            DuckNetwork._core.status = DuckNetStatus.Failure;
            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ModsIncompatible, "Host has different Mods enabled!"));
        }

        public static void FailWithBlockedUser()
        {
            DuckNetwork._core.status = DuckNetStatus.Failure;
            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.HostIsABlockedUser, "|RED|Could not join lobby: \n   you have blocked\n         the host."));
        }

        public static DuckNetErrorInfo AssembleMismatchError(string theirVersion)
        {
            int num = (int)DuckNetwork.CheckVersion(theirVersion);
            string versionMismatchError = DuckNetwork.GetVersionMismatchError((NMVersionMismatch.Type)num, theirVersion);
            return new DuckNetErrorInfo(num == 0 ? DuckNetError.YourVersionTooNew : DuckNetError.YourVersionTooOld, versionMismatchError);
        }

        public static string GetVersionMismatchError(string theirVersion) => DuckNetwork.GetVersionMismatchError(DuckNetwork.CheckVersion(theirVersion), theirVersion);

        public static string GetVersionMismatchError(
          NMVersionMismatch.Type type,
          string theirVersion,
          bool shortMessage = false)
        {
            string versionMismatchError = "";
            switch (type)
            {
                case NMVersionMismatch.Type.Older:
                    versionMismatchError = "|RED|Your version is too new.\n\n|WHITE|HOST: |GREEN|" + theirVersion + "\n|WHITE|YOU:  |RED|" + DG.version;
                    break;
                case NMVersionMismatch.Type.Newer:
                    versionMismatchError = "|RED|Your version is too old.\n\n|WHITE|HOST: |GREEN|" + theirVersion + "\n|WHITE|YOU:  |RED|" + DG.version;
                    break;
                case NMVersionMismatch.Type.Error:
                    versionMismatchError = "|RED|Your game version caused an error.\n\n|WHITE|HOST: |GREEN|" + theirVersion + "\n|WHITE|YOU:  |RED|" + DG.version;
                    break;
            }
            return versionMismatchError;
        }

        public static DuckPersona NextFree(Profile pProfile, DuckPersona pRequested)
        {
            int index = pRequested.index;
            do
            {
                DuckPersona duckPersona = Persona.all.ElementAt<DuckPersona>(index);
                bool flag = false;
                foreach (Profile profile in Profiles.active)
                {
                    if (profile != pProfile && profile.persona == duckPersona)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    ++index;
                    if (index > Persona.all.Count<DuckPersona>() - 1)
                        index = 0;
                }
                else
                    break;
            }
            while (index != pRequested.index);
            return Persona.all.ElementAt<DuckPersona>(index);
        }

        public static void RequestPersona(Profile pProfile, DuckPersona pPersona, bool pSendMessages = true)
        {
            if (Network.isServer)
            {
                bool flag = false;
                foreach (Profile profile in Profiles.active)
                {
                    if (profile != pProfile && profile.persona == pPersona)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    pProfile.PersonaRequestResult(DuckNetwork.NextFree(pProfile, pProfile.persona));
                else
                    pProfile.PersonaRequestResult(pPersona);
                if (!pSendMessages)
                    return;
                Send.Message(new NMSetPersona(pProfile, pProfile.persona));
            }
            else
            {
                if (((DuckNetwork.hostProfile == null ? 0 : (DuckNetwork.hostProfile.connection != null ? 1 : 0)) & (pSendMessages ? 1 : 0)) == 0)
                    return;
                Send.Message(new NMRequestPersona(pProfile, pPersona), DuckNetwork.hostProfile.connection);
            }
        }

        public static void OnMessage(NetMessage m)
        {
            if (m != null && m.connection.status != ConnectionStatus.Connected)
            {
                if (m.connection.status == ConnectionStatus.Disconnecting || m.connection.status == ConnectionStatus.Disconnected)
                    DevConsole.Log(DCSection.NetCore, "|DGRED|@error DuckNet message |WHITE|" + m.ToString() + " while disconnecting!|PREV|");
                else
                    DevConsole.Log(DCSection.NetCore, "|DGRED|@error DuckNet message |WHITE|" + m.ToString() + " while still connecting!|PREV|");
            }
            if (m is NMJoinDuckNetSuccess || m is NMNewDuckNetConnection)
                DevConsole.Log(DCSection.DuckNet, "Received new DuckNet connection message (NMJoinDuckNetSuccess)!");
            if (DuckNetwork.status == DuckNetStatus.Disconnected)
                return;
            if (m is NMDuckNetworkEvent)
            {
                (m as NMDuckNetworkEvent).Activate();
            }
            else
            {
                UIMatchmakingBox.core.pulseNetwork = true;
                if (DuckNetwork.GetProfiles(m.connection).Count == 0 && m.connection != Network.host)
                {
                    NetMessage msg = DuckNetwork.OnMessageFromNewClient(m);
                    if (msg == null)
                        return;
                    Send.Message(msg, m.connection);
                }
                else
                {
                    if (DuckNetwork.HandleCoreConnectionMessages(m) || DuckNetwork.status == DuckNetStatus.Disconnecting)
                        return;
                    Main.codeNumber = 13373;
                    foreach (Profile profile in DuckNetwork.GetProfiles(m.connection))
                    {
                        if (profile.networkStatus == DuckNetStatus.Disconnecting || profile.networkStatus == DuckNetStatus.Disconnected || profile.networkStatus == DuckNetStatus.Failure)
                            return;
                    }
                    switch (m)
                    {
                        case NMMojiData _:
                            NMMojiData nmMojiData = m as NMMojiData;
                            try
                            {
                                DuckFile.RegisterMoji(nmMojiData.name, new Sprite((Tex2D)Editor.StringToTexture(nmMojiData.data)), nmMojiData.connection);
                                DevConsole.Log(DCSection.DuckNet, "|DGBLUE|Received Moji (" + nmMojiData.name + ")");
                                break;
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        case NMSpecialHat _:
                            NMSpecialHat nmSpecialHat = m as NMSpecialHat;
                            using (List<Profile>.Enumerator enumerator = DuckNetwork.profiles.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    Profile current = enumerator.Current;
                                    if (current.connection != null && current.connection == nmSpecialHat.connection)
                                    {
                                        while (current.customTeams.Count < nmSpecialHat.customTeamIndex)
                                            current.customTeams.Add(new Team("CUSTOM", "hats/cluehat")
                                            {
                                                owner = current
                                            });
                                        Team.deserializeInto = current.GetCustomTeam(nmSpecialHat.customTeamIndex);
                                        Team.networkDeserialize = true;
                                        if (nmSpecialHat.filtered)
                                        {
                                            Team.deserializeInto.customData = null;
                                            Team.deserializeInto.SetHatSprite(new SpriteMap("hats/default", 32, 32));
                                            Team.deserializeInto = null;
                                        }
                                        else
                                            Team.Deserialize(nmSpecialHat.GetData()).customConnection = nmSpecialHat.connection;
                                        Team.networkDeserialize = false;
                                    }
                                }
                                break;
                            }
                        case NMSetTeam _:
                            NMSetTeam nmSetTeam = m as NMSetTeam;
                            if (nmSetTeam.profile == null || nmSetTeam.profile.connection == null || nmSetTeam.profile.team == null)
                                break;
                            if (nmSetTeam.team == null)
                            {
                                DevConsole.Log("|DGRED|NMSetTeam.team is NULL.");
                                break;
                            }
                            if (nmSetTeam.profile.slotType == SlotType.Spectator)
                            {
                                if (nmSetTeam.profile.team != null)
                                    nmSetTeam.profile.team.Leave(nmSetTeam.profile, false);
                                nmSetTeam.profile.team = nmSetTeam.team;
                            }
                            else
                                nmSetTeam.profile.team = nmSetTeam.team;
                            if (!Network.isServer || !DuckNetwork.OnTeamSwitch(nmSetTeam.profile))
                                break;
                            Send.MessageToAllBut(new NMSetTeam(nmSetTeam.profile, nmSetTeam.team, nmSetTeam.custom), NetMessagePriority.ReliableOrdered, m.connection);
                            break;
                        case NMRoomData _:
                            NMRoomData nmRoomData = m as NMRoomData;
                            if (nmRoomData.profile == null || nmRoomData.profile.linkedProfile != null || nmRoomData.profile.connection == null || nmRoomData.profile.connection == DuckNetwork.localConnection)
                                break;
                            nmRoomData.profile.furniturePositionData = nmRoomData.data;
                            break;
                        default:
                            if (Network.isServer)
                            {
                                if (!(m is NMClientLoadedLevel))
                                    break;
                                if ((m as NMClientLoadedLevel).levelIndex == levelIndex)
                                {
                                    m.connection.wantsGhostData = (m as NMClientLoadedLevel).levelIndex;
                                    break;
                                }
                                DevConsole.Log(DCSection.DuckNet, "@error The client loaded the wrong level! (" + (m as NMClientLoadedLevel).levelIndex.ToString() + " VS " + DuckNetwork.levelIndex.ToString() + ")@error", m.connection);
                                break;
                            }
                            switch (m)
                            {
                                case NMJoinDuckNetSuccess _:
                                case NMNewDuckNetConnection _:
                                    if (m is NMJoinDuckNetSuccess)
                                    {
                                        DevConsole.Log(DCSection.DuckNet, "DuckNet |LIME|connection with host was established!");
                                        NMJoinDuckNetSuccess joinDuckNetSuccess = m as NMJoinDuckNetSuccess;
                                        int num = 0;
                                        byte localFlippers = Profile.CalculateLocalFlippers();
                                        using (List<Profile>.Enumerator enumerator = joinDuckNetSuccess.profiles.GetEnumerator())
                                        {
                                            while (enumerator.MoveNext())
                                            {
                                                Profile current = enumerator.Current;
                                                if (current.connection == DuckNetwork.localConnection)
                                                {
                                                    current.networkStatus = DuckNetStatus.Connected;
                                                }
                                                else
                                                {
                                                    InputProfile pLocalInput = InputProfile.defaultProfiles[num];
                                                    Profile pLocalProfile = null;
                                                    Profile profile = null;
                                                    if (UIMatchmakingBox.core != null && UIMatchmakingBox.core.matchmakingProfiles != null && UIMatchmakingBox.core.matchmakingProfiles.Count > 0)
                                                    {
                                                        pLocalInput = UIMatchmakingBox.core.matchmakingProfiles[num].inputProfile;
                                                        pLocalProfile = UIMatchmakingBox.core.matchmakingProfiles[num].originallySelectedProfile;
                                                        profile = UIMatchmakingBox.core.matchmakingProfiles[num].masterProfile ?? pLocalProfile;
                                                    }
                                                    DuckPersona persona = current.persona;
                                                    string pName = profile != null ? DuckNetwork.GetNameForIndex(profile.name, num) : DuckNetwork.GetNameForIndex(Network.activeNetwork.core.GetLocalName(), num);
                                                    DuckNetwork.PrepareProfile(current, DuckNetwork.localConnection, pLocalInput, pLocalProfile, pName);
                                                    current.keepSetName = num != 0;
                                                    current.flippers = localFlippers;
                                                    current.persona = persona;
                                                }
                                                ++num;
                                            }
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        NMNewDuckNetConnection duckNetConnection = m as NMNewDuckNetConnection;
                                        NetworkConnection networkConnection = duckNetConnection.connection;
                                        bool flag = duckNetConnection.identifier == "SERVER" || duckNetConnection.identifier == "SERVERLOCAL";
                                        if (!flag)
                                        {
                                            networkConnection = Network.activeNetwork.core.AttemptConnection(duckNetConnection.identifier);
                                            if (networkConnection == null)
                                            {
                                                Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.InvalidConnectionInformation, "Invalid connection information (" + duckNetConnection.identifier + ")"));
                                                return;
                                            }
                                        }
                                        Profile profile1 = duckNetConnection.profile;
                                        DuckNetwork.PrepareProfile(profile1, networkConnection, null, null, duckNetConnection.name);
                                        profile1.team = duckNetConnection.team;
                                        profile1.flippers = duckNetConnection.flippers;
                                        profile1.flagIndex = duckNetConnection.flagIndex;
                                        profile1.steamID = duckNetConnection.profileID;
                                        profile1.ParentalControlsActive = duckNetConnection.parentalControlsActive;
                                        profile1.networkStatus = DuckNetStatus.Connected;
                                        profile1.isRemoteLocalDuck = duckNetConnection.identifier == "SERVERLOCAL";
                                        profile1.latestGhostIndex = duckNetConnection.latestGhostIndex;
                                        profile1.persona = Persona.all.ElementAt<DuckPersona>(duckNetConnection.persona);
                                        DevConsole.Log(DCSection.DuckNet, "Queuing up join message payload for " + networkConnection.ToString());
                                        if (networkConnection.status != ConnectionStatus.Connected)
                                            DevConsole.Log(DCSection.DuckNet, "|DGBLUE|This Payload will be sent when a connection is established.");
                                        else
                                            DevConsole.Log(DCSection.DuckNet, "|DGBLUE|This Payload will be sent at once!");
                                        Send.Message(new NMLevelReady(DuckNetwork.levelIndex), networkConnection);
                                        foreach (Profile profile2 in DuckNetwork.GetProfiles(DuckNetwork.localConnection))
                                            DuckNetwork.SendAllPlayerMetaData(profile2, networkConnection);
                                        if (!flag || DuckNetwork._core.hostProfile != null)
                                            return;
                                        DuckNetwork._core.hostProfile = duckNetConnection.profile;
                                        return;
                                    }
                                case NMEndOfDuckNetworkData _:
                                    DuckNetwork._core.status = DuckNetStatus.ConnectingToClients;
                                    DuckNetwork.CheckConnectingStatus();
                                    return;
                                case NMTeamSetDenied _:
                                    NMTeamSetDenied nmTeamSetDenied = m as NMTeamSetDenied;
                                    if (nmTeamSetDenied.profile == null || nmTeamSetDenied.profile.connection != DuckNetwork.localConnection || nmTeamSetDenied.profile.team == null || nmTeamSetDenied.profile.team != nmTeamSetDenied.team)
                                        return;
                                    DuckNetwork.OpenTeamSwitchDialogue(nmTeamSetDenied.profile);
                                    return;
                                default:
                                    return;
                            }
                    }
                }
            }
        }

        public static void AssignToHost(Thing pThing)
        {
            if (DuckNetwork.hostProfile == null || DuckNetwork.hostProfile.connection == null)
                return;
            pThing.connection = DuckNetwork.hostProfile.connection;
        }

        public static void SendToEveryone(NetMessage m)
        {
            List<NetworkConnection> pConnections = new List<NetworkConnection>();
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection != null && profile.connection != DuckNetwork.localConnection && (!profile.isHost || profile == DuckNetwork.hostProfile) && !pConnections.Contains(profile.connection))
                    pConnections.Add(profile.connection);
            }
            Send.Message(m, NetMessagePriority.ReliableOrdered, pConnections);
        }

        public static void SendToEveryoneFiltered(NetMessage m)
        {
            List<NetworkConnection> pConnections = new List<NetworkConnection>();
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (DuckNetwork.FilterPlayer(profile) == 0 && profile.connection != null && profile.connection != DuckNetwork.localConnection && (!profile.isHost || profile == DuckNetwork.hostProfile) && !pConnections.Contains(profile.connection))
                    pConnections.Add(profile.connection);
            }
            Send.Message(m, NetMessagePriority.ReliableOrdered, pConnections);
        }

        public static void Draw()
        {
            if (DuckNetwork.localProfile == null)
                return;
            Vec2 vec2_1 = new Vec2(Layer.Console.width, Layer.Console.height);
            float num1 = 0.0f;
            int num2 = 8;
            float chatScale = DuckNetwork.chatScale;
            float num3 = Resolution.current.x > 1920 ? 2f : 1f;
            DuckNetwork._core._chatFont.scale = new Vec2(Resolution.fontSizeMultiplier * chatScale);
            DuckNetwork._core._chatFont.scale = new Vec2(num3, num3);
            if (DuckNetwork._core._chatFont is RasterFont)
                DuckNetwork._core._chatFont.scale = new Vec2(0.5f);
            float num4 = Options.Data.chatOpacity / 100f;
            if (DuckNetwork._core.enteringText && !DuckNetwork._core.stopEnteringText)
            {
                ++DuckNetwork._core.cursorFlash;
                if (DuckNetwork._core.cursorFlash > 30)
                    DuckNetwork._core.cursorFlash = 0;
                int num5 = DuckNetwork._core.cursorFlash >= 15 ? 1 : 0;
                Profile localProfile = DuckNetwork.localProfile;
                string text = localProfile.name + ": " + DuckNetwork._core.currentEnterText;
                string str = text;
                if (num5 != 0)
                    text += "_";
                float x = DuckNetwork._core._chatFont.GetWidth(str + "_") + 8f * chatScale;
                float y = (DuckNetwork._core._chatFont.characterHeight + 2) * DuckNetwork._core._chatFont.scale.y;
                Vec2 p1 = new Vec2(14f, num1 + (vec2_1.y - (DuckNetwork._core._chatFont.characterHeight + 10) * DuckNetwork._core._chatFont.scale.y));
                DuckGame.Graphics.DrawRect(p1 + new Vec2(-1f, -1f), p1 + new Vec2(x, y) + new Vec2(1f, 1f), Color.Black * num4, (Depth)0.7f, false, 1f * chatScale);
                Color color = Color.White;
                Color black = Color.Black;
                if (localProfile.persona != null)
                    color = localProfile.persona.colorUsable;
                if (localProfile.slotType == SlotType.Spectator)
                    color = Colors.DGPurple;
                DuckGame.Graphics.DrawRect(p1, p1 + new Vec2(x, y), color * 0.85f * num4, (Depth)0.8f);
                DuckNetwork._core._chatFont.symbolYOffset = 4f;
                DuckNetwork._core._chatFont.Draw(text, p1 + new Vec2(2f, 2f), black * num4, (Depth)1f);
                num1 -= y + 4f * DuckNetwork._core._chatFont.scale.y;
            }
            float num6 = 0.1f;
            foreach (ChatMessage chatMessage in DuckNetwork._core.chatMessages)
            {
                float num7 = 10 * (Options.Data.chatHeadScale + 1) * num3;
                DuckNetwork._core._chatFont._currentConnection = chatMessage.who.connection == DuckNetwork.localConnection ? null : chatMessage.who.connection;
                DuckNetwork._core._chatFont.scale = new Vec2(Resolution.fontSizeMultiplier * chatMessage.scale * chatScale);
                DuckNetwork._core._chatFont.scale = new Vec2(num3, num3) * chatMessage.scale;
                if (DuckNetwork._core._chatFont is RasterFont)
                    DuckNetwork._core._chatFont.scale = new Vec2(0.5f);
                float x = (float)((double)DuckNetwork._core._chatFont.GetWidth(chatMessage.text) + (double)num7 + 8.0 * (double)chatScale);
                if (chatMessage.who.slotType == SlotType.Spectator)
                {
                    if (DuckNetwork._core._chatFont is RasterFont)
                    {
                        float num8 = (float)((_core._chatFont as RasterFont).data.fontSize * (double)RasterFont.fontScaleFactor / 10.0);
                        x += 6f * num8;
                    }
                    else
                        x += 8f * DuckNetwork._core._chatFont.scale.x;
                }
                float y = chatMessage.newlines * (DuckNetwork._core._chatFont.characterHeight + 2) * DuckNetwork._core._chatFont.scale.y;
                Vec2 p1 = new Vec2(14f, num1 + (vec2_1.y - (y + 10f)));
                Vec2 p2 = p1 + new Vec2(x, y);
                DuckGame.Graphics.DrawRect(p1 + new Vec2(-1f, -1f), p2 + new Vec2(1f, 1f), Color.Black * 0.8f * chatMessage.alpha * num4, (Depth)(num6 - 0.0015f), false, 1f * chatScale);
                float num9 = (float)(0.300000011920929 + chatMessage.text.Length * 0.00700000021606684);
                if ((double)num9 > 0.5)
                    num9 = 0.5f;
                if (chatMessage.slide > 0.800000011920929)
                    chatMessage.scale = Lerp.FloatSmooth(chatMessage.scale, 1f, 0.1f, 1.1f);
                else if (chatMessage.slide > 0.5)
                    chatMessage.scale = Lerp.FloatSmooth(chatMessage.scale, 1f + num9, 0.1f, 1.1f);
                chatMessage.slide = Lerp.FloatSmooth(chatMessage.slide, 1f, 0.1f, 1.1f);
                Color color = Color.White;
                Color black = Color.Black;
                if (chatMessage.who.persona != null)
                {
                    color = chatMessage.who.persona.colorUsable;
                    if (chatMessage.who.persona == Persona.Duck2)
                    {
                        color.r += 30;
                        color.g += 30;
                        color.b += 30;
                    }
                    if (chatMessage.who.slotType == SlotType.Spectator)
                        color = Colors.DGPurple;
                    SpriteMap g = null;
                    SpriteMap chatBust = chatMessage.who.persona.chatBust;
                    Vec2 vec2_2 = Vec2.Zero;
                    if (chatMessage.who.team != null && chatMessage.who.team.hasHat && (chatMessage.who.connection != DuckNetwork.localConnection || !chatMessage.who.team.locked))
                    {
                        vec2_2 = chatMessage.who.team.hatOffset * num3 * (Options.Data.chatHeadScale + 1);
                        g = chatMessage.who.team.GetHat(chatMessage.who.persona);
                    }
                    bool flag = chatMessage.who.netData.Get<bool>("quack");
                    if (chatMessage.who.duck != null && !chatMessage.who.duck.dead && !chatMessage.who.duck.removeFromLevel)
                        flag = chatMessage.who.duck.quack > 0;
                    Vec2 vec2_3 = new Vec2(p1.x, p1.y + -2 * (Options.Data.chatHeadScale + 1));
                    if (g != null)
                    {
                        g.CenterOrigin();
                        g.depth = (Depth)(num6 - 1f / 1000f);
                        g.alpha = chatMessage.alpha * num4;
                        g.frame = !flag || g.texture == null || g.texture.width <= 32 ? 0 : 1;
                        g.scale = new Vec2(num3, num3) * (Options.Data.chatHeadScale + 1);
                        DuckGame.Graphics.Draw(g, vec2_3.x - vec2_2.x, vec2_3.y - vec2_2.y);
                        g.scale = new Vec2(1f, 1f);
                        g.alpha = 1f;
                    }
                    chatBust.frame = !flag ? 0 : 1;
                    chatBust.depth = (Depth)(num6 - 0.0015f);
                    chatBust.alpha = chatMessage.alpha * num4;
                    chatBust.scale = new Vec2(num3, num3) * (Options.Data.chatHeadScale + 1);
                    DuckGame.Graphics.Draw(chatBust, vec2_3.x + 2f * chatBust.scale.x, vec2_3.y + 5f * chatBust.scale.y);
                    color *= 0.85f;
                    color.a = byte.MaxValue;
                }
                DuckGame.Graphics.DrawRect(p1, p2, color * 0.85f * chatMessage.alpha * num4, (Depth)(num6 - 1f / 500f));
                DuckNetwork._core._chatFont.symbolYOffset = 4f;
                DuckNetwork._core._chatFont.lineGap = 2f;
                if (chatMessage.who.slotType == SlotType.Spectator)
                    DuckNetwork._core._chatFont.Draw("@SPECTATORBIG@" + chatMessage.text, p1 + new Vec2(2f + num7, 1f * DuckNetwork._core._chatFont.scale.y), black * chatMessage.alpha * num4, (Depth)1f);
                else
                    DuckNetwork._core._chatFont.Draw(chatMessage.text, p1 + new Vec2(2f + num7, 1f * DuckNetwork._core._chatFont.scale.y), black * chatMessage.alpha * num4, (Depth)1f);
                DuckNetwork._core._chatFont._currentConnection = null;
                num1 -= y + 4f;
                num6 -= 0.01f;
                if (num2 == 0)
                    break;
                --num2;
            }
        }

        public enum LobbyType
        {
            Private,
            FriendsOnly,
            Public,
            Invisible,
        }
    }
}

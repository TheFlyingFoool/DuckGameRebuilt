// Decompiled with JetBrains decompiler
// Type: DuckGame.Main
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using XnaToFna;
using static DuckGame.CMD;

namespace DuckGame
{
    public class Main : MonoMain
    {

        public static bool isDemo = false;
        public static DuckGameEditor editor;
        public static string lastLevel = "";
        public static string SpecialCode = "";
        public static string SpecialCode2 = "";
        public static int codeNumber = 0;
        private BitmapFont _font;
        public static ulong connectID = 0;
        public static bool foundPurchaseInfo = true;
        public static float price = 9.99f;//10f; removed SetPurchaseDetails and hardset
        public static string currencyType = "USD";
        public static bool stopForever = false;
        public static bool _gotHook = false;
        //private bool didHash;
        public bool joinedLobby;

        public static string GetPriceString() =>
            $"|GREEN|{price.ToString("0.00", CultureInfo.InvariantCulture)} {currencyType}|WHITE|";

        //public static void SetPurchaseDetails(float p, string ct)
        //{
        //    Main.price = p;
        //    Main.currencyType = ct;
        //    Main.foundPurchaseInfo = true;
        //}

        public static void ResetMatchStuff()
        {
            DevConsole.Log(DCSection.General, "ResetMatchStuff()");
            DuckFile.BeginDataCommit();
            PurpleBlock.Reset();
            Highlights.ClearHighlights();
            Crowd.GoHome();
            GameMode.lastWinners.Clear();
            Deathmatch.levelsSinceRandom = 0;
            Deathmatch.levelsSinceCustom = 0;
            GameMode.numMatchesPlayed = 0;
            GameMode.showdown = false;
            RockWeather.Reset();
            Music.Reset();
            if (!Program.crashed)
            {
                foreach (Team team in Teams.all)
                {
                    team.prevScoreboardScore = team.score = 0;
                    if (team.activeProfiles.Count > 0)
                    {
                        foreach (Profile activeProfile in team.activeProfiles)
                        {
                            activeProfile.stats.lastPlayed = activeProfile.stats.lastPlayed = DateTime.Now;
                            activeProfile.RecordPreviousStats();
                            Profiles.Save(activeProfile);
                        }
                    }
                }
                if (Profiles.experienceProfile != null)
                    Profiles.Save(Profiles.experienceProfile);
                if (Profiles.all != null)
                {
                    foreach (Profile profile in Profiles.all)
                        profile?.RecordPreviousStats();
                }
                Global.Save();
                Options.Save();
            }
            Crowd.InitializeCrowd();
            DuckFile.EndDataCommit();
            DuckFile.FlagForBackup();
        }

        public static void ResetGameStuff()
        {
            if (Profiles.all == null)
                return;
            foreach (Profile profile in Profiles.all)
            {
                if (profile != null)
                    profile.wins = 0;
            }
        }

        protected override void OnStart()
        {
            Options.Initialize();
            Teams.PostInitialize();
            Unlocks.Initialize();
            ConnectionStatusUI.Initialize();
            Unlocks.CalculateTreeValues();
            Profiles.Initialize();
            Challenges.InitializeChallengeData();
            ProfilesCore.TryAutomerge();
            Dialogue.Initialize();
            DuckTitle.Initialize();
            News.Initialize();
            Script.Initialize();
            DuckNews.Initialize();
            VirtualBackground.InitializeBack();
            AmmoType.InitializeTypes();
            DestroyType.InitializeTypes();
            VirtualTransition.Initialize();
            Unlockables.Initialize();
            UIInviteMenu.Initialize();
            LevelGenerator.Initialize();
            DuckFile.InitializeMojis();
            ResetMatchStuff();
            DuckFile._flaggedForBackup = false;
            foreach (Profile profile in Profiles.active)
                profile.RecordPreviousStats();
            editor = new DuckGameEditor();
            Input.devicesChanged = false;
            TeamSelect2.ControllerLayoutsChanged();
            //Main.SetPurchaseDetails(9.99f, "USD");
            if (connectID != 0UL)
            {
                SpecialCode = "Joining lobby on startup (" + connectID.ToString() + ")";
                NCSteam.PrepareProfilesForJoin();
                NCSteam.inviteLobbyID = connectID;
                Level.current = new JoinServer(connectID, lobbyPassword);
            }
            else if (Program.lanjoiner)
            {
                SpecialCode = "Joining lobby on startup (127.0.0.1:1337)";
                NCSteam.PrepareProfilesForJoin();
                //NCSteam.inviteLobbyID = Main.connectID;
                Level.current = new JoinServer("127.0.0.1:1337");
            }
            else if (Level.current == null)
            {
                if (networkDebugger)
                {
                    Level.core.currentLevel = new NetworkDebugger(startLayer: Layer.core);
                    Layer.core = new LayerCore();
                    Layer.core.InitializeLayers();
                    Level.core.nextLevel = null;
                    Level.current.DoInitialize();
                    Level.core.currentLevel.lowestPoint = 100000f;
                }
                else
                {
                    if (startInEditor)
                    {
                        Level.current = editor;
                    }
                    else if (Program.testServer)
                    {
                        new TitleScreen().Initialize();
                        for (int i = 1; i < Teams.all.Count; i++)
                        {
                            Teams.all[i].ClearProfiles();
                        }
                        Level.current = new TeamSelect2() { sign = true };
                    }
                    else if (startInLobby)
                    {
                        new TitleScreen().Initialize();
                        for (int i = 1; i < Teams.all.Count; i++)
                        {
                            Teams.all[i].ClearProfiles();
                        }
                        Level.current = new TeamSelect2() { sign = true };

                    }
                    else if (startInArcade)
                    {
                        new TitleScreen().Initialize();
                        for (int i = 1; i < Teams.all.Count; i++)
                        {
                            Teams.all[i].ClearProfiles();
                        }
                        Level.current = new ArcadeLevel(DuckGame.Content.GetLevelID("arcade", LevelLocation.Content)) { sign = true };
                    }
                    else if (!Program.intro || noIntro)
                    {
                        Level.current = (new TitleScreen());
                    }
                    else
                    {
                        Level.current = (new BIOSScreen());
                    }

                }
            }
            _font = new BitmapFont("biosFont", 8);
            if (DGRSettings.RPC)
            {
                DiscordRichPresence.Initialize();
            }
            ModLoader.Start();
        }

        protected override void OnUpdate()
        {
            if (DevConsole.startupCommands.Count > 0)
            {
                DevConsole.RunCommand(DevConsole.startupCommands[0]);
                DevConsole.startupCommands.RemoveAt(0);
            }
            //Main.isDemo = false;
            RockWeather.TickWeather();
            RandomLevelDownloader.Update();
            if (!NetworkDebugger.enabled)
                FireManager.Update();
            DamageManager.Update();
            if (!Network.isActive)
                NetRand.generator = Rando.generator;
            //if (joinedLobby || Network.isActive || !Steam.lobbySearchComplete)
            //    return;
            //if (Steam.lobbySearchResult != null)
            //{
            //    Network.JoinServer("", 0, Steam.lobbySearchResult.id.ToString());
            //    joinedLobby = true;
            //}
            //else
            //{
            //    User who = Steam.friends.Find(x => x.name == "superjoebob");
            //    if (who == null)
            //        return;
            //    Steam.SearchForLobby(who);
            //}
        }
        protected override void EndDraw()
        {
            try
            {
                base.EndDraw();
            }
            catch (InvalidOperationException Ex) // weird steam overlay sht calls a method it shouldnt doesnt really break anything but does cause a crash, handling it seems fine and FNA discord said the same
            {
                /*  DevConsole.Log("error log " + Ex.Message);
                    error log GL_INVALID_ENUM in glMatrixMode
	                Source: GL_DEBUG_SOURCE_API
	                Type: GL_DEBUG_TYPE_ERROR
	                Severity: GL_DEBUG_SEVERITY_HIGH
                */
            }
        }
        protected override void OnDraw()
        {
        }
    }
}

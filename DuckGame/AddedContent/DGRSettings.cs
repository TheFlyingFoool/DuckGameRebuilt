using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public static class DGRSettings
    {
        /*[Marker.AutoConfig]//TODO: this eventually -Lucky
        public static List<byte> room1 = new List<byte>();
        [Marker.AutoConfig]
        public static List<byte> room2 = new List<byte>();
        [Marker.AutoConfig]
        public static List<byte> room3 = new List<byte>();*/

        public static int mMatch = -1;
        public static BitBuffer MatchsettingsPreset1 = new BitBuffer();

        [Marker.AutoConfig]
        public static byte[] MatchSetSave1 //scuffed i know, fuck you -Lucky
        {
            get
            {
                return MatchsettingsPreset1.buffer;
            }
            set
            {
                MatchsettingsPreset1 = new BitBuffer(value);
            }
        }

        [Marker.AutoConfig] public static List<string> bMatchSetSave1 = new List<string>();
        public static BitBuffer MatchsettingsPreset2 = new BitBuffer();

        [Marker.AutoConfig]
        public static byte[] MatchSetSave2 //scuffed i know, fuck you -Lucky
        {
            get
            {
                return MatchsettingsPreset2.buffer;
            }
            set
            {
                MatchsettingsPreset2 = new BitBuffer(value);
            }
        }

        [Marker.AutoConfig] public static List<string> bMatchSetSave2 = new List<string>();
        public static BitBuffer MatchsettingsPreset3 = new BitBuffer();

        [Marker.AutoConfig]
        public static byte[] MatchSetSave3 //scuffed i know, fuck you -Lucky
        {
            get
            {
                return MatchsettingsPreset3.buffer;
            }
            set
            {
                MatchsettingsPreset3 = new BitBuffer(value);
            }
        }

        [Marker.AutoConfig] public static List<string> bMatchSetSave3 = new List<string>();


        [Marker.AutoConfig] public static List<string> favoriteHats = new List<string>();
        [Marker.AutoConfig] public static string arcadeHat = "";
        [Marker.AutoConfig] public static int arcadeDuckColor = 0;

        //this is ran everytime TeamSelect2.cs is initialized or hats are reloaded
        //it should probably be moved to be ran somewhere else but thats the solution i came up with ages ago and works
        //issue is people with insane amount of hats might suffer lag spikes every time teamselect2.cs is loaded even though
        //this code doens't need to be run everytime -Lucky
        public static void InitializeFavoritedHats()
        {
            List<string> rel = new List<string>();
            for (int i = 0; i < Teams.all.Count; i++)
            {
                Team t = Teams.all[i];
                if (t.defaultTeam)
                {
                    if (favoriteHats.Contains("D" + t.name))
                    {
                        t.favorited = true;
                        rel.Add("D" + t.name);
                    }
                }
                else
                {
                    if (favoriteHats.Contains("C" + t.name))
                    {
                        t.favorited = true;
                        rel.Add("C" + t.name);
                    }
                }
            }

            //If any hats have been renamed or deleted they get deleted from the list
            favoriteHats = rel;
        }

        public static void ReloadFavHats()
        {
            if (!Network.isActive)
            {
                List<Team> tts = new List<Team>();

                List<Team> laterer = new List<Team>();
                for (int i = 0; i < Teams.all.Count; i++)
                {
                    Team t = Teams.all[i];
                    if (t.favorited)
                    {
                        laterer.Add(t);
                    }
                    else
                    {
                        tts.Add(t);
                    }
                }

                tts.AddRange(laterer);

                HatSelector.remember = tts;
            }
            else
            {

            }

            favoriteHats.Clear();
            for (int i = 0; i < Teams.all.Count; i++)
            {
                Team t = Teams.all[i];
                if (t.favorited)
                {
                    if (t.defaultTeam)
                    {
                        favoriteHats.Add("D" + t.name);
                    }
                    else
                    {
                        favoriteHats.Add("C" + t.name);
                    }
                }
            }
        }

        public static void PrreloadLevels()
        {
            MonoMain.NloadMessage = "Pre-Loading Custom Levels";

            //steal code? we would never do such a thing -Lucky
            try
            {
                HUD.hide = true;
                //someone optimize the code later maybe lol
                new LevelSelect("", null, null, false).Initialize();
                HUD.CloseAllCorners();
                HUD.hide = false;
            }
            catch (Exception ex)
            {
                DevConsole.Log("Failed to preload levels: " + ex.ToString(), Colors.DGRed);
            }
        }
        public static bool DGROnly
        {
            get
            {
                return DGRSettings.DGRItems || DG.FiftyPlayerMode || DGRSettings.MidGameJoining;
            }
        }

        [Marker.AutoConfig] public static bool ExtraMusic = true;

        [Marker.AutoConfig] public static bool IgnoreLevRestrictions = false;

        [Marker.AutoConfig] public static bool CustomHatTeams = false;

        [Marker.AutoConfig] public static bool skipOnlineBumper = false;

        [Marker.AutoConfig] public static bool SwitchInput = false;

        [Marker.AutoConfig]
        public static bool HideFS = false;

        [Marker.AutoConfig]
        public static bool ReducedMovement = false;

        [Marker.AutoConfig] public static bool LoadMusic = true;

        [Marker.AutoConfig] public static bool FasterLoad = false;
        
        [Marker.AutoConfig] public static bool SequenceCrateRetexture = false;

        public static bool LoaderMusic; //this is so you wont crash immediately when changing the setting, only gets set on startup -Lucky

        public static bool DGRItems = false;

        public static bool MidGameJoining = false;

        [Marker.AutoConfig] public static string PreferredLevel = "";

        [Marker.AutoConfig] public static bool PreloadLevels;

        [Marker.AutoConfig]
        public static bool SortLevels = true;

        [Marker.AutoConfig] public static bool SpriteAtlas = true;

        [Marker.AutoConfig] public static bool EditorTimer = true;

        [Marker.AutoConfig] public static bool SkipXP = false;

        public static int ParticleMultiplier
        {
            get
            {
                return S_ParticleMultiplier;
            }
            set
            {
                S_ParticleMultiplier = value;
                //HELL INCOMING
                int sixtyd = (int)Maths.Clamp(64f * ActualParticleMultiplier, 64, 5120);
                BreathSmoke.kMaxObjects = sixtyd;
                BreathSmoke._objects = new BreathSmoke[sixtyd];
                BreathSmoke._lastActiveObject = (BreathSmoke._lastActiveObject) % BreathSmoke.kMaxObjects;

                TreeLeaf.kMaxObjects = sixtyd;
                TreeLeaf._objects = new TreeLeaf[sixtyd];
                TreeLeaf._lastActiveObject = (TreeLeaf._lastActiveObject) % TreeLeaf.kMaxObjects;

                ConfettiParticle.kMaxSparks = sixtyd;
                ConfettiParticle._sparks = new ConfettiParticle[sixtyd];
                ConfettiParticle._lastActiveSpark = (ConfettiParticle._lastActiveSpark) % ConfettiParticle.kMaxSparks;

                Feather.kMaxObjects = sixtyd;
                Feather._objects = new Feather[sixtyd];
                Feather._lastActiveObject = (Feather._lastActiveObject) % Feather.kMaxObjects;

                MetalRebound.kMaxObjects = sixtyd / 2;
                MetalRebound._objects = new MetalRebound[MetalRebound.kMaxObjects];
                MetalRebound._lastActiveObject = (MetalRebound._lastActiveObject) % MetalRebound.kMaxObjects;

                Spark.kMaxSparks = sixtyd;
                Spark._sparks = new Spark[sixtyd];
                Spark._lastActiveSpark = (Spark._lastActiveSpark) % Spark.kMaxSparks;

                WagnusChargeParticle.kMaxWagCharge = sixtyd;
                WagnusChargeParticle._sparks = new WagnusChargeParticle[sixtyd];
                WagnusChargeParticle._lastActiveWagCharge = (WagnusChargeParticle._lastActiveWagCharge) % WagnusChargeParticle.kMaxWagCharge;

                WoodDebris.kMaxObjects = sixtyd;
                WoodDebris._objects = new WoodDebris[sixtyd];
                WoodDebris._lastActiveObject = (WoodDebris._lastActiveObject) % WoodDebris.kMaxObjects;
            }
        }

        [Marker.AutoConfig] public static int S_ParticleMultiplier = 3;

        //listen if you wanna make better code go for it i cant bother to personally
        //-Lucky
        public static float ActualParticleMultiplier
        {
            get
            {
                return S_ParticleMultiplier switch
                {
                    1 => 0.3f, //MINIMUM 
                    2 => 0.6f, //LOW
                    3 => 1, //DEFAULT
                    4 => 2, //MANY
                    5 => 4, //EXTREME
                    6 => 8, //WUMBO
                    7 => 16, //UNCONTAUNBLE
                    _ => 0, //NONE
                };
            }
        }
        // Intentionally not saved for now - Tater
        public static bool TournamentMode = false;

        [Marker.AutoConfig] public static bool FixBulletPositions = false;

        [Marker.AutoConfig] public static int MaximumCorrectionTicks = 8;


        [Marker.AutoConfig] public static bool S_UncappedFPS = false;
        public static bool UncappedFPS
        {
            get
            {
                return S_UncappedFPS;
            }
            set
            {
                Program.main.UnFixedDraw = value;
                S_UncappedFPS = value;
            }
        }

        [Marker.AutoConfig] public static bool SingleLoadLine = false;

        [Marker.AutoConfig] public static bool S_UseVSync = true;
        public static bool UseVSync
        {
            get
            {
                return S_UseVSync;
            }
            set
            {
                MonoMain.graphics.SynchronizeWithVerticalRetrace = value;
                //when vsync is ON we never want to use the draw rate limiter because vsync will do that
                if (value)
                {
                    Program.main.UseDrawRateLimiter = false;
                }
                //when vsync is OFF we only want to use the rate limiter if:
                //Uncapped FPS is on && The user has set a target fps that is not 0
                else
                {
                    Program.main.UseDrawRateLimiter = (UncappedFPS && TargetFrameRate != 0);
                }

                S_UseVSync = value;
                MonoMain.graphics.ApplyChanges();
            }
        }

        public static bool ApplyOnNextFrame;

        //Tater: It's really annoying you can't make dynamic settings display based on other settings, so this is ugly.
        [Marker.AutoConfig] public static int S_TargetFrameRate = 0;
        public static int TargetFrameRate
        {
            get
            {
                return S_TargetFrameRate;
            }
            set
            {
                if (value >= 60)
                {
                    Program.main.DrawRateLimiterTarget = S_TargetFrameRate;
                    Program.main.UseDrawRateLimiter = true;
                    S_TargetFrameRate = value;
                }
                else
                {
                    Program.main.UseDrawRateLimiter = false;
                    S_TargetFrameRate = 0;
                }
            }
        }

        public static void InitalizeFPSThings()
        {
            MonoMain.graphics.SynchronizeWithVerticalRetrace = UseVSync;
            Program.main.UnFixedDraw = UncappedFPS;
            Program.main.DrawRateLimiterTarget = Math.Max(TargetFrameRate,60);
            Program.main.UseDrawRateLimiter = !UseVSync && UncappedFPS && TargetFrameRate >= 60;


            Program.main.TargetElapsedTime = TimeSpan.FromTicks(163934); // Default to 61ups -Lucky
            MonoMain.graphics.ApplyChanges();
        }

        [Marker.AutoConfig] public static bool TemporaryUnlockAll = false;


        [Marker.AutoConfig] public static float HeatWaveMultiplier = 0;

        [Marker.AutoConfig] public static bool AmbientParticles = true;

        [Marker.AutoConfig] public static bool EnhancedTextures = true;

        [Marker.AutoConfig] public static int StartIn = 0;

        [Marker.AutoConfig] public static bool Weather = true;

        [Marker.AutoConfig] public static bool CameraUnfollow = false;

        [Marker.AutoConfig] public static bool dubberspeed = false;

        [Marker.AutoConfig] public static bool MenuMouse = false;

        [Marker.AutoConfig] public static bool SkipExcessRounds = false;

        [Marker.AutoConfig] public static int RebuiltEffect = 1;

        [Marker.AutoConfig] public static bool StickyHats { get; set; }
        
        [Marker.AutoConfig] public static bool EditorMousePriority = false;

        [Marker.AutoConfig] public static bool ShowWorkshopModsInEditor = false;

        [Marker.AutoConfig] public static bool ConvertModdedCommands = true;

        [Marker.AutoConfig]
        private static bool _useEnabledModsConfig = false;
        public static bool UseEnabledModsConfig { get => _useEnabledModsConfig; set {
                if (value == true) { ModLoader.GenerateEnabledXml(); }; _useEnabledModsConfig = value; } }

        [Marker.AutoConfig] public static bool QOLScoreThingButWithoutScore { get; set; } //why. -Lucky

        [Marker.AutoConfig] public static int HSDSpacing = 5;
        [Marker.AutoConfig] public static int HSDXoffset = 2;
        [Marker.AutoConfig] public static int HSDYoffset = 2;
        [Marker.AutoConfig] public static int HSDOpacity = 100;
        [Marker.AutoConfig] public static int HSDFontScale = 50;
        [Marker.AutoConfig] public static int HSDSorting = 0;
        [Marker.AutoConfig] public static bool HSDHorizontal = false;
        [Marker.AutoConfig] public static bool HSDRightToLeft = false;
        [Marker.AutoConfig] public static bool HSDClearNames = true;
        [Marker.AutoConfig] public static bool HSDStandardizeNames = false;
        [Marker.AutoConfig] public static bool HSDShowScore = true;
        [Marker.AutoConfig] public static bool HSDShowColors = true;
        [Marker.AutoConfig] public static bool HSDShowRoundsLeft = false;
        [Marker.AutoConfig] public static bool HSDBlackOutline = true;
        [Marker.AutoConfig] public static bool HSDStarForHighestScore = true;

        [Marker.AutoConfig] public static bool NameTags { get; set; }

        [Marker.AutoConfig] public static bool LobbyNameOnPause = true;

        [Marker.AutoConfig] public static bool EditorOnlinePhysics = false;

        [Marker.AutoConfig] public static bool DrawOffscreenArrowsOnEditor = false;

        [Marker.AutoConfig] public static bool EditorLevelName = true;

        [Marker.AutoConfig] public static bool OpenURLsInBrowser = false;

        [Marker.AutoConfig] public static int DGRJoinLink = 0;

        [Marker.AutoConfig] public static bool UseDuckShell = true;

        private static bool s_duckShellAutoCompletion = true;
        
        [Marker.AutoConfig]
        public static bool DuckShellAutoCompletion 
        {
            get => s_duckShellAutoCompletion;
            set
            {
                s_duckShellAutoCompletion = value;
                
                if (!value && DevConsole.LatestPredictionSuggestions?.Length > 0)
                    DevConsole.LatestPredictionSuggestions = Array.Empty<string>();
            }
        }

        [Marker.AutoConfig] public static bool NoConsoleLineLimit = false;
    }
}

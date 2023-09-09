using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class DGRSettings
    {
        public static DGRSettings instance;
        public DGRSettings()
        {
            instance = this;
        }
        /*[AutoConfigField]//TODO: this eventually -NiK0
        public static List<byte> room1 = new List<byte>();
        [AutoConfigField]
        public static List<byte> room2 = new List<byte>();
        [AutoConfigField]
        public static List<byte> room3 = new List<byte>();*/

        public static int mMatch = -1;
        public static BitBuffer MatchsettingsPreset1 = new BitBuffer();

        [AutoConfigField]
        public static byte[] MatchSetSave1 //scuffed i know, fuck you -NiK0
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

        [AutoConfigField] public static List<string> bMatchSetSave1 = new List<string>();
        public static BitBuffer MatchsettingsPreset2 = new BitBuffer();

        [AutoConfigField]
        public static byte[] MatchSetSave2 //scuffed i know, fuck you -NiK0
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

        [AutoConfigField] public static List<string> bMatchSetSave2 = new List<string>();
        public static BitBuffer MatchsettingsPreset3 = new BitBuffer();

        [AutoConfigField]
        public static byte[] MatchSetSave3 //scuffed i know, fuck you -NiK0
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

        [AutoConfigField] public static List<string> bMatchSetSave3 = new List<string>();


        [AutoConfigField] public static List<string> favoriteHats = new List<string>();
        [AutoConfigField] public static string arcadeHat = "";
        [AutoConfigField] public static int arcadeDuckColor = 0;


        //this is ran everytime TeamSelect2.cs is initialized or hats are reloaded
        //it should probably be moved to be ran somewhere else but thats the solution i came up with ages ago and works
        //issue is people with insane amount of hats might suffer lag spikes every time teamselect2.cs is loaded even though
        //this code doens't need to be run everytime -NiK0
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

            //steal code? we would never do such a thing -NiK0
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

        [AutoConfigField] public static bool IgnoreLevRestrictions = false;

        [AutoConfigField] public static bool RememberMatchSettings = false;

        [AutoConfigField] public static bool CustomHatTeams = false;

        [AutoConfigField] public static bool skipOnlineBumper = false;

        [AutoConfigField]
        public static bool HideFS = false;

        [AutoConfigField]
        public static bool ReducedMovement = false;

        [AutoConfigField] public static bool LoadMusic = true;

        public static bool
            LoaderMusic; //this is so you wont crash immediately when changing the setting, this only gets set on startup -NiK0

        [AutoConfigField] public static bool DGRItems = false;

        [AutoConfigField] public static string PreferredLevel = "";

        [AutoConfigField] public static bool PreloadLevels;

        [AutoConfigField]
        public static bool SortLevels = true; //do you care about levels being sorted? no? turn this the fuck off for faster load times -NiK0

        //[AutoConfigField] nvm im not smart enough for async stuff -NiK0
        //public static bool ThreadedLevelLoading = true;

        [AutoConfigField] public static bool SpriteAtlas = true;

        [AutoConfigField] public static bool S_RPC = true;

        public static bool RPC
        {
            get => S_RPC;

            set
            {
                S_RPC = value;
                if (S_RPC)
                {
                    DiscordRichPresence.Initialize();
                }
                else
                {
                    DiscordRichPresence.Deinitialize();
                }
            }
        }

        public int ParticleMultiplier
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

        [AutoConfigField]
        public static int S_ParticleMultiplier = 3;

        //listen if you wanna make better code go for it i cant bother to personally
        //-NiK0
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

        [AutoConfigField] public static float WeatherMultiplier = 1;

        [AutoConfigField] public static float HeatWaveMultiplier = 1;

        [AutoConfigField] public static bool AmbientParticles = true;

        [AutoConfigField] public static bool GraphicsCulling = true;

        [AutoConfigField] public static int StartIn = 0;

        [AutoConfigField] public static float WeatherLighting = 1;

        [AutoConfigField] public static bool CameraUnfollow = false;

        [AutoConfigField] public static bool dubberspeed = false;

        [AutoConfigField] public static float RandomWeather = 1;

        [AutoConfigField] public static bool MenuMouse = false;

        [AutoConfigField] public static bool SkipExcessRounds = false;

        [AutoConfigField] public static int RebuiltEffect = 1;

        [AutoConfigField] public static bool StickyHats { get; set; }

        [AutoConfigField] public static bool QOLScoreThingButWithoutScore { get; set; }

        [AutoConfigField] public static bool NameTags { get; set; }

        [AutoConfigField] public static bool LobbyData { get; set; }

        [AutoConfigField] public static bool LobbyNameOnPause = true;

        [AutoConfigField]
        public static bool EditorOnlinePhysics = false; //for the love of god be off by default jesus christ -NiK0

        [AutoConfigField] public static bool EditorInstructions = true;

        [AutoConfigField] public static bool EditorLevelName = true;

        [AutoConfigField] public static bool OpenURLsInBrowser = false;

        [AutoConfigField] public static bool UseDGRJoinLink = false;
        
        // the 4chan disease..
        [AutoConfigField] public static bool GreenTextSupport = false;

        [AutoConfigField] public static bool becomegod = false;
    }
}

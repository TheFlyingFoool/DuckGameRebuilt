using System;
using System.Security.Policy;

namespace DuckGame
{
    public class DGRSettings
    {
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
        [AutoConfigField]
        public static bool skipOnlineBumper = false;

        [AutoConfigField]
        public static bool LoadMusic = true;
        public static bool LoaderMusic;//this is so you wont crash immediately when changing the setting, this only gets set on startup -NiK0

        [AutoConfigField]
        public static string PreferredLevel = "";

        [AutoConfigField]
        public static bool PreloadLevels;


        [AutoConfigField]
        public static bool SpriteAtlas = true;

        [AutoConfigField] 
        public static bool S_RPC = true;
        
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

        [AutoConfigField]
        public static float WeatherMultiplier = 1;

        [AutoConfigField]
        public static bool GraphicsCulling = true;

        [AutoConfigField]
        public static int StartIn = 0;

        [AutoConfigField]
        public static float WeatherLighting = 1;

        [AutoConfigField]
        public static bool CameraUnfollow = true;

        [AutoConfigField]
        public static bool dubberspeed = false;

        [AutoConfigField]
        public static float RandomWeather = 0.3f;

        [AutoConfigField]
        public static bool MenuMouse = false;

        [AutoConfigField]
        public static int RebuiltEffect = 1;

        [AutoConfigField]
        public static bool StickyHats { get; set; }

        [AutoConfigField]
        public static bool QOLScoreThingButWithoutScore { get; set; }

        [AutoConfigField]
        public static bool NameTags { get; set; }

        [AutoConfigField]
        public static bool LobbyData { get; set; }

        [AutoConfigField]
        public static bool LobbyNameOnPause = true;

        [AutoConfigField]
        public static bool EditorOnlinePhysics = false; //for the love of god be off by default jesus christ -NiK0

        [AutoConfigField]
        public static bool EditorInstructions = true;

        [AutoConfigField]
        public static bool EditorLevelName = true;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace DuckGame
{
    public class DGRSettings
    {
        public static void PrreloadLevels()
        {
            MonoMain.loadMessage = "Pre-Loading Custom Levels";

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
        public static string PreferredLevel = "";
        public bool PreloadLevels
        {
            get
            {
                return S_PreloadLevels;
            }
            set
            {
                S_PreloadLevels = value;
            }
        }
        [AutoConfigField]
        public static bool S_PreloadLevels;
        public bool SpriteAtlas
        {
            get
            {
                return S_SpriteAtlas;
            }
            set
            {
                S_SpriteAtlas = value;
            }
        }
        [AutoConfigField]
        public static bool S_SpriteAtlas = true;

        public bool RPC
        {
            get
            {
                return S_RPC;
            }
            set
            {
                S_RPC = value;
            }
        }
        [AutoConfigField]
        public static bool S_RPC = false;
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

        public float WeatherMultiplier
        {
            get
            {
                return S_WeatherMultiplier;
            }
            set
            {
                S_WeatherMultiplier = value;
            }
        }
        [AutoConfigField]
        public static float S_WeatherMultiplier = 1;

        public bool GraphicsCulling
        {
            get
            {
                return S_GraphicsCulling;
            }
            set
            {
                S_GraphicsCulling = value;
            }
        }
        [AutoConfigField]
        public static bool S_GraphicsCulling = true;
        public int StartIn
        {
            get
            {
                return S_StartIn;
            }
            set
            {
                S_StartIn = value;
            }
        }
        [AutoConfigField]
        public static int S_StartIn = 0;
        public float WeatherLighting
        {
            get
            {
                return S_WeatherLighting;
            }
            set
            {
                S_WeatherLighting = value;
            }
        }
        [AutoConfigField]
        public static float S_WeatherLighting = 1;

        public bool CameraUnfollow
        {
            get
            {
                return S_CameraUnfollow;
            }
            set
            {
                S_CameraUnfollow = value;
            }
        }
        [AutoConfigField]
        public static bool S_CameraUnfollow = true;


        public bool dubberspeed
        {
            get
            {
                return s_dubberspeed;
            }
            set
            {
                s_dubberspeed = value;
            }
        }
        [AutoConfigField]
        public static bool s_dubberspeed = false;

        public float RandomWeather
        {
            get
            {
                return S_RandomWeather;
            }
            set
            {
                S_RandomWeather = value;
            }
        }
        [AutoConfigField]
        public static float S_RandomWeather = 0.3f;
        public bool MenuMouse
        {
            get
            {
                return S_MenuMouse;
            }
            set
            {
                S_MenuMouse = value;
            }
        }
        [AutoConfigField]
        public static bool S_MenuMouse = false;
        public int RebuiltEffect
        {
            get
            {
                return S_RebuiltEffect;
            }
            set
            {
                S_RebuiltEffect = value;
            }
        }
        [AutoConfigField]
        public static int S_RebuiltEffect = 1;
    }
}

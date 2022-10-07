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
                BreathSmoke.kMaxObjects = (int)Maths.Clamp(64f * ActualParticleMultiplier, 64, 5120);
                BreathSmoke._objects = new BreathSmoke[BreathSmoke.kMaxObjects];

                ConfettiParticle.kMaxSparks = (int)Maths.Clamp(64f * ActualParticleMultiplier, 64, 5120);
                ConfettiParticle._sparks = new ConfettiParticle[ConfettiParticle.kMaxSparks];

                Feather.kMaxObjects = (int)Maths.Clamp(64f * ActualParticleMultiplier, 64, 5120);
                Feather._objects = new Feather[Feather.kMaxObjects];

                MetalRebound.kMaxObjects = (int)Maths.Clamp(32f * ActualParticleMultiplier, 32, 5120);
                MetalRebound._objects = new MetalRebound[MetalRebound.kMaxObjects];

                Spark.kMaxSparks = (int)Maths.Clamp(64f * ActualParticleMultiplier, 64, 5120);
                Spark._sparks = new Spark[Spark.kMaxSparks];

                WagnusChargeParticle.kMaxWagCharge = (int)Maths.Clamp(64f * ActualParticleMultiplier, 64, 5120);
                WagnusChargeParticle._sparks = new WagnusChargeParticle[WagnusChargeParticle.kMaxWagCharge];

                WoodDebris.kMaxObjects = (int)Maths.Clamp(64f * ActualParticleMultiplier, 64, 5120);
                WoodDebris._objects = new WoodDebris[WoodDebris.kMaxObjects];
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
                    6 => 8, //OMEGA
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

        public bool RandomWeather
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
        public static bool S_RandomWeather = true;
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
        public static bool S_MenuMouse = true;
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

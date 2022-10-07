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
                    6 => 8, //UNCONTAUNBLE
                    _ => 0, //NONE
                };
            }
        }

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

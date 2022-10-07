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
        public ParticleMultiplierFrequency ParticleMultiplier
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
        public static ParticleMultiplierFrequency S_ParticleMultiplier = ParticleMultiplierFrequency.Default;
        
        public static float ActualParticleMultiplier => (float) S_ParticleMultiplier * 0.01f;

        public enum ParticleMultiplierFrequency
        {
            // multipler percentage
            None = 0,
            Minimum = 30,
            Low = 60,
            Default = 100,
            Many = 200,
            Extreme = 400,
            Uncountable = 800,
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

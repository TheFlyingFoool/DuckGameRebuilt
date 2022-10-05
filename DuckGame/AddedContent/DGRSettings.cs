using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

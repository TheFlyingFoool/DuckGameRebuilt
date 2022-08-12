using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class RoomEditorExtra
    {
        [AutoConfigField]
        public static BitBuffer room1 = new BitBuffer();
        
        [AutoConfigField]
        public static List<int> favoriteHats = new List<int>();
    }
}

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
        public static List<byte> room1 = new List<byte>();

        [AutoConfigField]
        public static List<int> favoriteHats = new List<int>();
    }
}

using System.Collections.Generic;

namespace DuckGame
{
    public class HUDCore
    {
        public List<CornerDisplay> _cornerDisplays = new List<CornerDisplay>();
        public List<CornerDisplay> _inputChangeDisplays = new List<CornerDisplay>();
        public List<CornerDisplay> _playerChangeDisplays = new List<CornerDisplay>();
        public bool _hide;
    }
}

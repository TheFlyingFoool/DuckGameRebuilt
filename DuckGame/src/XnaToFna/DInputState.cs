using System.Collections.Generic;

namespace XnaToFna.ProxyDInput
{
    public class DInputState
    {
        public float leftX;
        public float leftY;
        public float leftZ;
        public float rightX;
        public float rightY;
        public float rightZ;
        public float slider1;
        public float slider2;
        public bool left;
        public bool right;
        public bool up;
        public bool down;
        public List<bool> buttons = new List<bool>();
        internal bool connected;
    }
}

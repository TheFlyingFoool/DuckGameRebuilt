namespace DuckGame
{
    public class CornerDisplay
    {
        public bool ischallenge;
        public HUDCorner corner;
        public float slide;
        public string text;
        public bool closing;
        public Timer timer;
        public int lowTimeTick = 4;
        public FieldBinding counter;
        public int curCount;
        public int realCount;
        public int addCount;
        public float addCountWait;
        public int maxCount;
        public bool animateCount;
        public bool isControl;
        public InputProfile profile;
        public bool stack;
        public float life = 1f;
        public bool willDie;
        public float scale = 1f;
    }
}

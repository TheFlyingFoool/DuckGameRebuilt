using System.Collections.Generic;

namespace DuckGame
{
    public class Slot3D
    {
        public RockThrow state;
        public Duck duck;
        public List<Duck> subDucks = new List<Duck>();
        public List<DuckAI> subAIs = new List<DuckAI>();
        public ScoreRock rock;
        public int slideWait;
        public DuckAI ai;
        public int slotIndex;
        public float startX;
        public bool follow;
        public bool showScore;
        public bool highestScore;

        public float scroll
        {
            get
            {
                if (slotIndex == 0)
                    return (float)(-duck.position.x * 0.665f + 100.0f);
                if (slotIndex == 1)
                    return (float)(-duck.position.x * 0.665f + 100.0f);
                return (float)(-duck.position.x * 0.665f + 100.0f);
            }
        }
    }
}

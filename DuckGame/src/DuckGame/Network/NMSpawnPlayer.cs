namespace DuckGame
{
    public class NMSpawnPlayer : NMObjectMessage
    {
        public float xpos;
        public float ypos;
        public int duckID;
        public bool isPlayerDuck;

        public NMSpawnPlayer()
        {
        }

        public NMSpawnPlayer(float xVal, float yVal, int duck, bool playerDuck, ushort objectID)
          : base(objectID)
        {
            xpos = xVal;
            ypos = yVal;
            duckID = duck;
            isPlayerDuck = playerDuck;
        }
    }
}

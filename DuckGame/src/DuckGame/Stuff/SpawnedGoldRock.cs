namespace DuckGame
{
    public class SpawnedGoldRock : Rock
    {
        public SpawnedGoldRock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            gold.value = true;
        }
    }
}

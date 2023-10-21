using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Terror.", IsCheat = true)]
        public static void DownPour()
        {
            float centerX = (float)(DuckGame.Level.current.bottomRight.x - (double)DuckGame.Level.current.topLeft.x + 128f);
            const int amount = 10;
            for (int y = 0; y < amount; ++y)
            {
                for (int x = 0; x < amount; ++x)
                {
                    PhysicsObject randomItem = ItemBoxRandom.GetRandomItem();
                    randomItem.position = DuckGame.Level.current.topLeft +
                                          new Vec2(
                                              (float)((double)centerX / amount * x +
                                                  Rando.Float(sbyte.MinValue, 128f) - 64f),
                                              DuckGame.Level.current.topLeft.y - 2000f - 512 * y +
                                              Rando.Float(-256f, 256f));
                    DuckGame.Level.Add(randomItem);
                }
            }
        }
    }
}
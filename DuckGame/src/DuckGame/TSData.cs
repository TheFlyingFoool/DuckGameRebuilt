namespace DuckGame
{
    public class TSData
    {
        public int fingerId;
        public Vec2 touchXY;
        public int diameterX;
        public int diameterY;
        public int rotationAngle;
        public long msTimeElapsed;

        public TSData(int initValue)
        {
            fingerId = initValue;
            touchXY.x = touchXY.y = initValue;
            diameterX = initValue;
            diameterY = initValue;
            rotationAngle = initValue;
            msTimeElapsed = initValue;
        }
    }
}

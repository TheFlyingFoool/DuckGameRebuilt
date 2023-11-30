namespace DuckGame
{
    public class NMSpawnObject : NMObjectMessage
    {
        public string name;
        public float xpos;
        public float ypos;

        public NMSpawnObject()
        {
        }

        public NMSpawnObject(string obj, float xVal, float yVal, ushort idVal)
          : base(idVal)
        {
            name = obj;
            xpos = xVal;
            ypos = yVal;
        }
    }
}

namespace DuckGame
{
    public class NMInputUpdate : NetMessage
    {
        public int id;
        public int state;
        public double time;

        public NMInputUpdate()
        {
        }

        public NMInputUpdate(int idVal, int stateVal, double t)
        {
            id = idVal;
            state = stateVal;
            time = t;
        }
    }
}

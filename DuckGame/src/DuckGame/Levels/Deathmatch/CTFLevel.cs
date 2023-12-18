namespace DuckGame
{
    public class CTFLevel : GameLevel
    {
        public CTFLevel(string lev)
          : base(lev)
        {
            _mode = new CTF();
        }
    }
}

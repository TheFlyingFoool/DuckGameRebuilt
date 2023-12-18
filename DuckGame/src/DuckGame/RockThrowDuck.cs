namespace DuckGame
{
    public class RockThrowDuck : Duck
    {
        public RockThrowDuck(float xval, float yval, Profile pro)
          : base(xval, yval, pro)
        {
            _isStateObject = false;
            _isStateObjectInitialized = true;
        }
    }
}

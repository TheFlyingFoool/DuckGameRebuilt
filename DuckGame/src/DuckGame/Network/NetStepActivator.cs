namespace DuckGame
{
    public class NetStepActivator
    {
        private Function _function;
        private int _index;

        public NetStepActivator(Function del) => _function = del;

        public int index
        {
            get => _index;
            set
            {
                _index = value;
                if (_index <= 3)
                    return;
                _index = 0;
            }
        }

        public void Activate()
        {
            if (_function != null)
                _function();
            Step();
        }

        public void Step() => ++index;

        public delegate void Function();
    }
}

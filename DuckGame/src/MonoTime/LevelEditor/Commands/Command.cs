namespace DuckGame
{
    public class Command
    {
        private bool _done;
        private bool _inverse;

        public Command Inverse()
        {
            _inverse = !_inverse;
            return this;
        }

        public void Do()
        {
            if (_done)
                return;
            if (_inverse)
                OnUndo();
            else
                OnDo();
            _done = true;
        }

        public void Undo()
        {
            if (!_done)
                return;
            if (_inverse)
                OnDo();
            else
                OnUndo();
            _done = false;
        }

        public virtual void OnDo()
        {
        }

        public virtual void OnUndo()
        {
        }
    }
}

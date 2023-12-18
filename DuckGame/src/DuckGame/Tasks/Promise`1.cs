using System;

namespace DuckGame
{
    public class Promise<T> : Promise
    {
        public T Result { get; private set; }

        public Promise(Func<T> function)
          : base(function)
        {
        }

        public override void Execute()
        {
            Result = (T)_delegate.Method.Invoke(_delegate.Target, null);
            Finished = true;
        }
    }
}

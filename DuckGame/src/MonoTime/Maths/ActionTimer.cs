using System;

namespace DuckGame
{
    public class ActionTimer : IAutoUpdate
    {
        private float _max;
        private float _inc;
        private float _val;
        private bool _hit;
        private bool _reset = true;
        private Thing parent;
        private WeakReference weakref;

        public bool hit => _hit;

        public ActionTimer(float inc, float max = 1f, bool reset = true)
        {
            _inc = inc;
            _max = max;
            _reset = reset;
            AutoUpdatables.Add(this);
        }
        public ActionTimer(Thing thing, float inc, float max = 1f, bool reset = true)
        {
            _inc = inc;
            _max = max;
            _reset = reset;
            parent = thing;
            weakref = AutoUpdatables.AddWithReturn(this);
        }
        public void Update()
        {
            if (parent != null && weakref != null && (parent.removeFromLevel || parent.level != Level.current))
            {
                AutoUpdatables.Remove(weakref);
            }
            if (_reset)
                _hit = false;
            _val += _inc;
            if (_val < _max)
                return;
            _val = 0f;
            _hit = true;
        }

        public void Reset() => _val = 0f;

        public void SetToEnd()
        {
            _val = 0f;
            _hit = true;
        }

        public static implicit operator bool(ActionTimer val) => val.hit;

        public static implicit operator ActionTimer(float val) => new ActionTimer(val);
    }
}

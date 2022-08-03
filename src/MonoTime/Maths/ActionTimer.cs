// Decompiled with JetBrains decompiler
// Type: DuckGame.ActionTimer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ActionTimer : IAutoUpdate
    {
        private float _max;
        private float _inc;
        private float _val;
        private bool _hit;
        private bool _reset = true;

        public bool hit => _hit;

        public ActionTimer(float inc, float max = 1f, bool reset = true)
        {
            _inc = inc;
            _max = max;
            _reset = reset;
            AutoUpdatables.Add(this);
        }

        public void Update()
        {
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

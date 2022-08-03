// Decompiled with JetBrains decompiler
// Type: DuckGame.Timer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Diagnostics;

namespace DuckGame
{
    public class Timer
    {
        private Stopwatch _timer = new Stopwatch();
        private TimeSpan _subtract;
        protected TimeSpan _maxTime;

        public virtual TimeSpan elapsed => _maxTime.TotalSeconds == 0.0 || _timer.Elapsed - _subtract < _maxTime ? _timer.Elapsed - _subtract : _maxTime;

        public TimeSpan maxTime
        {
            get => _maxTime;
            set => _maxTime = value;
        }

        public Timer(TimeSpan max = default(TimeSpan)) => _maxTime = max;

        public virtual void Start() => _timer.Start();

        public virtual void Stop() => _timer.Stop();

        public virtual void Reset() => _timer.Reset();

        public virtual void Restart() => _timer.Restart();

        public virtual void Subtract(TimeSpan s) => _subtract += s;
    }
}

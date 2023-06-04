// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeTimer
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ChallengeTimer : Timer
    {
        private float _time;
        private bool _active;

        public override TimeSpan elapsed
        {
            get
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, (int)(_time * 1000f));
                return _maxTime.TotalSeconds == 0f || timeSpan < _maxTime ? timeSpan : _maxTime;
            }
        }

        public ChallengeTimer(TimeSpan max = default(TimeSpan))
          : base()
        {
            _maxTime = max;
        }

        public void Update()
        {
            if (!_active)
                return;
            _time += Maths.IncFrameTimer();
        }

        public override void Start() => _active = true;

        public override void Stop() => _active = false;

        public override void Reset()
        {
            _time = 0f;
            _active = false;
        }

        public override void Restart() => _time = 0f;
    }
}

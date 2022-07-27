// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeTimer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
                TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, (int)(_time * 1000.0));
                return this._maxTime.TotalSeconds == 0.0 || timeSpan < this._maxTime ? timeSpan : this._maxTime;
            }
        }

        public ChallengeTimer(TimeSpan max = default(TimeSpan))
          : base()
        {
            this._maxTime = max;
        }

        public void Update()
        {
            if (!this._active)
                return;
            this._time += Maths.IncFrameTimer();
        }

        public override void Start() => this._active = true;

        public override void Stop() => this._active = false;

        public override void Reset()
        {
            this._time = 0.0f;
            this._active = false;
        }

        public override void Restart() => this._time = 0.0f;
    }
}

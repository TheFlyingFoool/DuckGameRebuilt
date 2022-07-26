// Decompiled with JetBrains decompiler
// Type: DuckGame.SinWaveManualUpdate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class SinWaveManualUpdate
    {
        private float _increment;
        private float _wave;
        private float _value;

        public float value
        {
            get => this._value;
            set => this._value = value;
        }

        public float normalized => (float)(((double)this._value + 1.0) / 2.0);

        public SinWaveManualUpdate(float inc, float start = 0.0f)
        {
            this._increment = inc;
            this._wave = start;
        }

        public SinWaveManualUpdate()
        {
            this._increment = 0.1f;
            this._wave = 0.0f;
        }

        public void Update()
        {
            this._wave += this._increment;
            this._value = (float)Math.Sin((double)this._wave);
        }

        public static implicit operator float(SinWaveManualUpdate val) => val.value;

        public static implicit operator SinWaveManualUpdate(float val) => new SinWaveManualUpdate(val);
    }
}

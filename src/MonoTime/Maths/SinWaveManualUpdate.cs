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
            get => _value;
            set => _value = value;
        }

        public float normalized => (float)((_value + 1.0) / 2.0);

        public SinWaveManualUpdate(float inc, float start = 0f)
        {
            _increment = inc;
            _wave = start;
        }

        public SinWaveManualUpdate()
        {
            _increment = 0.1f;
            _wave = 0f;
        }

        public void Update()
        {
            _wave += _increment;
            _value = (float)Math.Sin(_wave);
        }

        public static implicit operator float(SinWaveManualUpdate val) => val.value;

        public static implicit operator SinWaveManualUpdate(float val) => new SinWaveManualUpdate(val);
    }
}

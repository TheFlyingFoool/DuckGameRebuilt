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

        public float normalized => (float)((_value + 1) / 2);

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

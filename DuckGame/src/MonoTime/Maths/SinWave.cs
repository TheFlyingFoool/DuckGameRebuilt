using System;

namespace DuckGame
{
    public class SinWave : IAutoUpdate
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

        public SinWave(float inc, float start = 0f)
        {
            _increment = inc;
            _wave = start;
            AutoUpdatables.Add(this);
        }

        public SinWave()
        {
            _increment = 0.1f;
            _wave = 0f;
        }

        public void Update()
        {
            _wave += _increment;
            _value = (float)Math.Sin(_wave);
        }

        public static implicit operator float(SinWave val) => val.value;

        public static implicit operator SinWave(float val) => new SinWave(val);
    }
}

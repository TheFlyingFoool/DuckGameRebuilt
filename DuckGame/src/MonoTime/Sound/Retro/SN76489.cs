namespace DuckGame
{
    public class SN76489
    {
        private SN76489Core _chip;

        public SN76489() => _chip = new SN76489Core();

        public void Initialize(double clock) => _chip.clock((float)clock);

        public void Update(int[] buffer, int length)
        {
            for (int index = 0; index < length; ++index)
            {
                short num = (short)(_chip.render() * 8000);
                buffer[index * 2] = num;
                buffer[index * 2 + 1] = num;
            }
        }

        public void Write(int value) => _chip.write(value);
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.SN76489
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
                short num = (short)(_chip.render() * 8000.0);
                buffer[index * 2] = num;
                buffer[index * 2 + 1] = num;
            }
        }

        public void Write(int value) => _chip.write(value);
    }
}

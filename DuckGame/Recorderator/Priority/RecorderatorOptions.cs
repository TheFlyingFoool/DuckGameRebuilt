using System.Collections;
using System.IO;

namespace DuckGame
{
    public class RecorderatorOptions
    {
        public static string zePath = DuckFile.optionsDirectory;
        public static bool SlowSave;
        public static bool SaveSFX;

        public static void LoadFile()
        {
            if (File.Exists(zePath + "RecorderatorOptions.dat"))
            {
                BitArray b = new BitArray(File.ReadAllBytes(zePath + "RecorderatorOptions.dat"));
                SlowSave = b[0];
                SaveSFX = b[1];
            }
            else
            {
                SlowSave = true;
                SaveSFX = true;
            }
        }
        public static void SaveFile()
        {
            BitArray b = new BitArray(16);
            b[0] = SlowSave;
            b[1] = SaveSFX;
            File.WriteAllBytes(zePath + "RecorderatorOptions.dat", Extensions.BitArrayToBytes(b));
        }
    }
}

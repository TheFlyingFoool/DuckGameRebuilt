// Decompiled with JetBrains decompiler
// Type: DuckGame.Recorder
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Recorder
    {
        public static Recording _currentRecording;
        public static FileRecording _globalRecording;

        public static Recording currentRecording
        {
            get => _currentRecording;
            set => _currentRecording = value;
        }

        public static void LogVelocity(float velocity)
        {
            if (_currentRecording == null)
                return;
            _currentRecording.LogVelocity(velocity);
        }

        public static void LogCoolness(int val)
        {
            if (_currentRecording == null)
                return;
            _currentRecording.LogCoolness(val);
        }

        public static void LogDeath()
        {
            if (_currentRecording == null)
                return;
            _currentRecording.LogDeath();
        }

        public static void LogAction(int num = 1)
        {
            if (_currentRecording == null)
                return;
            _currentRecording.LogAction(num);
        }

        public static void LogBonus()
        {
            if (_currentRecording == null)
                return;
            _currentRecording.LogBonus();
        }

        public static FileRecording globalRecording
        {
            get => _globalRecording;
            set => _globalRecording = value;
        }
    }
}

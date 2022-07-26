// Decompiled with JetBrains decompiler
// Type: DuckGame.Recorder
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => Recorder._currentRecording;
            set => Recorder._currentRecording = value;
        }

        public static void LogVelocity(float velocity)
        {
            if (Recorder._currentRecording == null)
                return;
            Recorder._currentRecording.LogVelocity(velocity);
        }

        public static void LogCoolness(int val)
        {
            if (Recorder._currentRecording == null)
                return;
            Recorder._currentRecording.LogCoolness(val);
        }

        public static void LogDeath()
        {
            if (Recorder._currentRecording == null)
                return;
            Recorder._currentRecording.LogDeath();
        }

        public static void LogAction(int num = 1)
        {
            if (Recorder._currentRecording == null)
                return;
            Recorder._currentRecording.LogAction(num);
        }

        public static void LogBonus()
        {
            if (Recorder._currentRecording == null)
                return;
            Recorder._currentRecording.LogBonus();
        }

        public static FileRecording globalRecording
        {
            get => Recorder._globalRecording;
            set => Recorder._globalRecording = value;
        }
    }
}

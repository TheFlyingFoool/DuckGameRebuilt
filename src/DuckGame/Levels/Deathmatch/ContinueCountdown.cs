// Decompiled with JetBrains decompiler
// Type: DuckGame.ContinueCountdown
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ContinueCountdown : Thing
    {
        public StateBinding _timerBinding = new StateBinding(nameof(timer));
        public float timer = 5f;

        public ContinueCountdown(float time = 5f)
          : base()
        {
            timer = time;
        }

        public void UpdateTimer()
        {
            if (isServerForObject)
                timer -= Maths.IncFrameTimer();
            if (timer >= 0.0)
                return;
            timer = 0f;
        }
    }
}

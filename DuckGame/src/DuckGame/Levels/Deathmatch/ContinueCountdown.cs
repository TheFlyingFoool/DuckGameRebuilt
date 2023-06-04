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
            if (isServerForObject) timer -= Maths.IncFrameTimer();
            if (timer >= 0f) return;
            timer = 0f;
        }
    }
}

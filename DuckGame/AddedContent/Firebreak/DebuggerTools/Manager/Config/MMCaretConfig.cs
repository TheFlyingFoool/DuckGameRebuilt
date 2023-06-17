namespace DuckGame.MMConfig
{
    public sealed class MMCaretConfig
    {
        [ACMin(0)]
        public float BlinkSpeed = 1f;
        public bool IsHorizontal = false;
        [ACMin(0)]
        public float ThicknessPercentage = 0.2f;
        [ACMin(0)]
        public float MovementSmoothness = 20f;
    }
}
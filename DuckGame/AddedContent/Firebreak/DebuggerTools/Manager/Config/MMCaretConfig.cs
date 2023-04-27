namespace DuckGame.MMConfig
{
    public sealed class MMCaretConfig
    {
        [ACMin(0)]
        public float BlinkSpeed;
        public bool IsHorizontal;
        [ACMin(0)]
        public float ThicknessPercentage;
        [ACMin(0)]
        public float MovementSmoothness;
    }
}
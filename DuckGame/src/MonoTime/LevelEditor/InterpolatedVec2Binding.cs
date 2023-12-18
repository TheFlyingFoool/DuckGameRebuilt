namespace DuckGame
{
    public class InterpolatedVec2Binding : CompressedVec2Binding
    {
        public InterpolatedVec2Binding(string field, int range = 2147483647, bool real = true)
          : base(field, range)
        {
            _priority = GhostPriority.High;
        }
    }
}

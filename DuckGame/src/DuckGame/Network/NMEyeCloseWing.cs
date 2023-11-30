namespace DuckGame
{
    public class NMEyeCloseWing : NMEvent
    {
        public Vec2 position;
        public Duck closer;
        public Duck closee;

        public NMEyeCloseWing()
        {
        }

        public NMEyeCloseWing(Vec2 pPosition, Duck pCloser, Duck pClosee)
        {
            position = pPosition;
            closer = pCloser;
            closee = pClosee;
        }

        public override void Activate()
        {
            if (closer != null && closee != null && closee.ragdoll != null && closee.ragdoll.part1 != null)
                Level.Add(new EyeCloseWing(closee.ragdoll.part1.angle < 0f ? position.x - 4f : position.x - 11f, position.y + 7f, closee.ragdoll.part1.angle < 0f ? 1 : -1, closer._spriteArms, closer, closee));
            base.Activate();
        }
    }
}

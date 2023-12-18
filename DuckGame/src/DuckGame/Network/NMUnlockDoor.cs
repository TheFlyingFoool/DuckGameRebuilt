namespace DuckGame
{
    public class NMUnlockDoor : NMEvent
    {
        public Door door;

        public NMUnlockDoor()
        {
        }

        public NMUnlockDoor(Door d) => door = d;

        public override void Activate()
        {
            if (door == null)
                return;
            door.networkUnlockMessage = true;
        }
    }
}

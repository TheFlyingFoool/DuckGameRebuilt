namespace DuckGame
{
    public class NMPopShell : NMEvent
    {
        public Gun gun;

        public NMPopShell()
        {
        }

        public NMPopShell(Gun pGun) => gun = pGun;

        public override void Activate()
        {
            if (gun != null)
                gun.PopShell(true);
            base.Activate();
        }
    }
}

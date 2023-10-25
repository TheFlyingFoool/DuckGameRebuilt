namespace DuckGame
{
    public class DTShot : DestroyType
    {
        private Bullet _bullet;

        public Bullet bullet => _bullet;

        public Thing bulletOwner => _bullet == null ? null : _bullet.owner;

        public Thing bulletFiredFrom => _bullet == null ? null : _bullet.firedFrom;

        public DTShot(Bullet b)
          : base(b)
        {
            _bullet = b;
        }
    }
}

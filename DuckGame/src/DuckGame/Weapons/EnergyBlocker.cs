namespace DuckGame
{
    public class EnergyBlocker : MaterialThing
    {
        private OldEnergyScimi _parent;

        public EnergyBlocker(OldEnergyScimi pParent)
          : base(0f, 0f)
        {
            thickness = 100f;
            _editorCanModify = false;
            visible = false;
            _parent = pParent;
            weight = 0.01f;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (!_solid)
                return false;
            if (_parent != null)
                _parent.Shing();
            if (!(bullet.ammo is ATLaser))
                return base.Hit(bullet, hitPos);
            bullet.reboundOnce = true;
            return true;
        }
    }
}

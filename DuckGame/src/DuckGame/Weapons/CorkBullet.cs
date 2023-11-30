namespace DuckGame
{
    public class CorkBullet : Bullet
    {
        private Sprite _cork;
        private CorkObject _corkObject;

        public CorkBullet(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = false,
          bool network = false)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            _cork = new Sprite("cork")
            {
                center = new Vec2(3f, 2.5f)
            };
        }

        public override void Update()
        {
            base.Update();
            if (!doneTravelling || _corkObject != null)
                return;
            _corkObject = new CorkObject(drawEnd.x - travelDirNormalized.x * 4f, drawEnd.y - travelDirNormalized.y * 4f, firedFrom);
            if (firedFrom != null && firedFrom is CorkGun)
                (firedFrom as CorkGun).corkObject = _corkObject;
            Level.Add(_corkObject);
            Level.Remove(this);
        }

        public override void Draw()
        {
            Graphics.Draw(ref _cork, drawEnd.x, drawEnd.y);
            base.Draw();
        }
    }
}

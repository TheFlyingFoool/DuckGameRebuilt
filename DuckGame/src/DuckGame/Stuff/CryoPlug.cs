namespace DuckGame
{
    [EditorGroup("survival")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class CryoPlug : Holdable
    {
        private SpriteMap _sprite;
        private Rope _rope;

        public CryoPlug(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("survival/cryoPlug", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(12f, 12f);
            _collisionOffset = new Vec2(-6f, -6f);
            _sprite.frame = 0;
            depth = (Depth)0.9f;
        }

        public void AttachTo(Thing t)
        {
            _rope = new Rope(x, y, t, this);
            Level.Add(_rope);
        }

        public override void Terminate() => Level.Remove(_rope);

        public override void Update()
        {
            if (owner == null && _sprite.frame == 0)
            {
                foreach (PowerSocket powerSocket in Level.current.things[typeof(PowerSocket)])
                {
                    if ((powerSocket.position - position).length < 8f)
                    {
                        SFX.Play("equip");
                        _sprite.frame = 1;
                        _enablePhysics = false;
                        position = powerSocket.position;
                        depth = -0.8f;
                        return;
                    }
                }
            }
            else if (owner != null && _sprite.frame == 1)
            {
                _sprite.frame = 0;
                _enablePhysics = true;
                SFX.Play("equip");
            }
            base.Update();
        }
    }
}

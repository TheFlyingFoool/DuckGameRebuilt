namespace DuckGame
{
    public class TreeLeaf : Thing
    {
        public static int kMaxObjects = 64;
        public static TreeLeaf[] _objects = new TreeLeaf[kMaxObjects];
        public static int _lastActiveObject = 0;
        private SpriteMap _sprite;
        private bool _rested;

        public int leafType;
        public static TreeLeaf New(float xpos, float ypos, int lT = 0)
        {
            TreeLeaf feather;
            if (NetworkDebugger.enabled)
                feather = new TreeLeaf();
            else if (_objects[_lastActiveObject] == null)
            {
                feather = new TreeLeaf();
                _objects[_lastActiveObject] = feather;
            }
            else
                feather = _objects[_lastActiveObject];
            Level.Remove(feather);
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            feather.Init(xpos, ypos, lT);
            feather.ResetProperties();
            feather._sprite.globalIndex = GetGlobalIndex();
            feather.globalIndex = GetGlobalIndex();
            return feather;
        }

        private TreeLeaf()
          : base()
        {
            _sprite = new SpriteMap("treeLeaf", 8, 4)
            {
                speed = 0.3f
            };
            _sprite.AddAnimation("fall", 1f, true, 0, 1, 2, 1);
            graphic = _sprite;
            center = new Vec2(4f, 2f);
        }

        private void Init(float xpos, float ypos, int lT)
        {
            position.x = xpos;
            position.y = ypos;
            alpha = Rando.Float(8, 15);
            hSpeed = Rando.Float(-1, 1);
            switch (lT)
            {
                default:
                case 0:
                    _sprite = new SpriteMap("treeLeaf", 8, 4);
                    break;
                case 1:
                    _sprite = new SpriteMap("treeLeafDead", 8, 4);
                    break;
                case 2:
                    _sprite = new SpriteMap("treeLeafSummer", 8, 4);
                    break;
                case 3:
                    _sprite = new SpriteMap("treeLeafWinter", 8, 4);
                    break;
                case 4:
                    _sprite = new SpriteMap("treeLeafGold", 8, 4);
                    break;
            }
            _sprite.AddAnimation("fall", 1f, true, 0, 1, 2, 1);
            _sprite.SetAnimation("fall");
            _sprite.frame = Rando.Int(3);
            _sprite.flipH = Rando.Double() > 0.5;
            graphic = _sprite;
            _rested = false;
        }
        private Thing lastthing;
        public override void Update()
        {
            if (alpha < 0) Level.Remove(this);
            if (GameLevel.rainwind != 0)
            {
                hSpeed = Lerp.Float(hSpeed, GameLevel.rainwind * 0.5f, 0.04f);
            }
            if (_rested && lastthing != null & !lastthing.removeFromLevel)
            {
                alpha -= 0.4f;
                return;
            }
            hSpeed = Lerp.Float(hSpeed, 0, 0.01f);

            if (vSpeed < 1f) vSpeed += 0.06f;
            if (vSpeed < 0f)
            {
                _sprite.speed = 0f;
                if (Level.CheckPoint<Block>(x, y - 6f) is Thing thing && thing.solid && thing.y >= top)
                {
                    vSpeed = 0f;
                    lastthing = thing;
                }
            }
            else if (Level.CheckPoint<IPlatform>(x, y + 2f) is Thing thing && thing is not TreeTileset && thing is not CityTreeTileset && thing is not PineTreeTileset && thing.solid && thing.y >= top)
            {
                lastthing = thing;
                vSpeed = 0f;
                _sprite.speed = 0f;
                if (thing.isBlock) _rested = true;
            }
            else _sprite.speed = 0.3f;
            x += hSpeed;
            y += vSpeed;
            alpha -= 0.017f;
        }
    }
}

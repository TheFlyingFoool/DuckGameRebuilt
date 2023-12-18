namespace DuckGame
{
    [EditorGroup("Stuff|Pyramid", EditorItemType.Pyramid)]
    public class PyramidDoor : VerticalDoor, IPlatform
    {
        public PyramidDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("pyramidDoor", 16, 32);
            graphic = _sprite;
            center = new Vec2(8f, 24f);
            collisionSize = new Vec2(10f, 32f);
            collisionOffset = new Vec2(-5f, -24f);
            depth = -0.5f;
            _editorName = "Pyramid Door";
            thickness = 3f;
            physicsMaterial = PhysicsMaterial.Metal;
            _bottom = new Sprite("pyramidDoorBottom");
            _bottom.CenterOrigin();
            _top = new Sprite("pyramidDoorTop");
            _top.CenterOrigin();
        }

        public override void Update()
        {
            if (!_cornerInit)
            {
                _topLeft = topLeft;
                _topRight = topRight;
                _bottomLeft = bottomLeft;
                _bottomRight = bottomRight;
                _cornerInit = true;
            }
            if (Level.CheckRect<Duck>(_topLeft - new Vec2(18f, 0f), _bottomRight + new Vec2(18f, 0f)) != null)
                _desiredOpen = 1f;
            else if (Level.CheckRectFilter<PhysicsObject>(new Vec2(x - 4f, y - 24f), new Vec2(x + 4f, y + 8f), d => !(d is TeamHat)) == null)
                _desiredOpen = 0f;
            if (_desiredOpen > 0.5 && !_opened)
            {
                _opened = true;
                SFX.DontSave = 1;
                SFX.Play("pyramidOpen", 0.6f);
            }
            if (_desiredOpen < 0.5 && _opened)
            {
                _opened = false;
                SFX.DontSave = 1;
                SFX.Play("pyramidClose", 0.6f);
            }
            _open = Maths.LerpTowards(_open, _desiredOpen, 0.15f);
            _sprite.frame = (int)(_open * 32);
            _collisionSize.y = (float)((1 - _open) * 32);
        }
    }
}

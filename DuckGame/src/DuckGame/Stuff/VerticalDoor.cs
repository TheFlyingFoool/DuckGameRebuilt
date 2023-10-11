// Decompiled with JetBrains decompiler
// Type: DuckGame.VerticalDoor
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Doors")]
    public class VerticalDoor : Block, IPlatform
    {
        protected SpriteMap _sprite;
        protected SpriteMap _sensorSprite;
        protected SpriteMap _noSensorSprite;
        protected Sprite _bottom;
        protected Sprite _top;
        public float _open;
        protected float _desiredOpen;
        protected bool _opened;
        protected Vec2 _topLeft;
        protected Vec2 _topRight;
        protected Vec2 _bottomLeft;
        protected Vec2 _bottomRight;
        protected bool _cornerInit;
        public bool filterDefault;
        public bool slideLocked;
        public bool slideLockOpened;
        public bool stuck;
        private bool showedWarning;

        public VerticalDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sensorSprite = _sprite = new SpriteMap("verticalDoor", 16, 32);
            graphic = _sprite;
            center = new Vec2(8f, 24f);
            collisionSize = new Vec2(6f, 32f);
            collisionOffset = new Vec2(-3f, -24f);
            depth = -0.5f;
            _editorName = "Vertical Door";
            thickness = 3f;
            physicsMaterial = PhysicsMaterial.Metal;
            _bottom = new Sprite("verticalDoorBottom");
            _bottom.CenterOrigin();
            _top = new Sprite("verticalDoorTop");
            _top.CenterOrigin();
            editorTooltip = "One of them science fiction type doors.";
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
            if (!slideLocked)
            {
                _sprite = _sensorSprite;
                Duck duck = Level.CheckRect<Duck>(_topLeft - new Vec2(18f, 0f), _bottomRight + new Vec2(18f, 0f));
                if (duck != null)
                {
                    if (!filterDefault || !Profiles.IsDefault(duck.profile))
                        _desiredOpen = 1f;
                    else if (!showedWarning)
                    {
                        HUD.AddPlayerChangeDisplay("@UNPLUG@|GRAY|NO ARCADE (SELECT A PROFILE)");
                        showedWarning = true;
                    }
                }
                else if (_desiredOpen != 0f && Level.CheckRectFilter<PhysicsObject>(new Vec2(x - 4f, y - 24f), new Vec2(x + 4f, y + 8f), d => !(d is TeamHat)) == null)
                    _desiredOpen = 0f;
            }
            else
            {
                if (_noSensorSprite == null)
                    _noSensorSprite = new SpriteMap("verticalDoorNoSensor", 16, 32);
                _sprite = _noSensorSprite;
                _desiredOpen = slideLockOpened ? 1f : 0f;
                if (_opened && Level.CheckRectFilter<PhysicsObject>(new Vec2(x - 4f, y - 24f), new Vec2(x + 4f, y + 8f), d => !(d is TeamHat)) != null)
                    _desiredOpen = 1f;
            }
            if (_desiredOpen > 0.5 && !_opened)
            {
                _opened = true;
                SFX.DontSave = 1;
                SFX.Play("slideDoorOpen", 0.6f);
            }
            if (_desiredOpen < 0.5 && _opened)
            {
                _opened = false;
                SFX.DontSave = 1;
                SFX.Play("slideDoorClose", 0.6f);
            }
            graphic = _sprite;
            _open = Maths.LerpTowards(_open, _desiredOpen, 0.15f);
            _sprite.frame = (int)(_open * 32);
            _collisionSize.y = (float)((1 - _open) * 32);
        }

        public override void Draw()
        {
            base.Draw();
            _top.depth = depth + 1;
            _bottom.depth = depth + 1;
            Graphics.Draw(_top, x, y - 27f);
            Graphics.Draw(_bottom, x, y + 5f);
        }
    }
}

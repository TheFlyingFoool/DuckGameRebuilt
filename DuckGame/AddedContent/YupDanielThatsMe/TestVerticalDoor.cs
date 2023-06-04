// Decompiled with JetBrains decompiler
// Type: DuckGame.VerticalDoor
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [ClientOnly]
    //[EditorGroup("Stuff|Doors")]
    public class TestVerticalDoor : Block, IPlatform
    {
        protected SpriteMap _sprite;
        protected SpriteMap _sensorSprite;
        protected SpriteMap _noSensorSprite;
        //protected Sprite _bottom;
        //protected Sprite _top;
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

        public TestVerticalDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            thickness = float.PositiveInfinity;
            _sensorSprite = _sprite = new SpriteMap("testverticalDoor", 16, 32);
            graphic = _sprite;
            center = new Vec2(8f, 24f);
            collisionSize = new Vec2(16f, 32f);
            collisionOffset = new Vec2(-8f, -24f);
            depth = -0.5f;
            _editorName = "Test Vertical Door";
            physicsMaterial = PhysicsMaterial.Metal;
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
                Duck duck = Level.CheckRect<Duck>(_topLeft - new Vec2(18f + 4f, 0f), _bottomRight + new Vec2(18f + 4f, 0f));
                if (duck != null)
                {
                    if (!filterDefault || !Profiles.IsDefault(duck.profile))
                        _desiredOpen = 1f;
                }
                else if (_desiredOpen != 0f && Level.CheckRectFilter<PhysicsObject>(new Vec2(x - (4f + 4f), y - 24f), new Vec2(x + (4f + 4f), y + 8f), d => (d is Duck || d is RagdollPart)) == null)
                    _desiredOpen = 0f;
            }
            else
            {
                //if (_noSensorSprite == null)
                //    _noSensorSprite = new SpriteMap("verticalDoorNoSensor", 16, 32);
               //_sprite = _noSensorSprite;
                _desiredOpen = slideLockOpened ? 1f : 0f;
                if (_opened && Level.CheckRectFilter<PhysicsObject>(new Vec2(x - (4f + 4f), y - 24f), new Vec2(x + (4f + 4f), y + 8f), d => (d is Duck || d is RagdollPart)) != null)
                    _desiredOpen = 1f;
            }
            if (_desiredOpen > 0.5 && !_opened)
            {
                _opened = true;
                SFX.Play("slideDoorOpen", 0.6f);
            }
            if (_desiredOpen < 0.5 && _opened)
            {
                _opened = false;
                SFX.Play("slideDoorClose", 0.6f);
            }
            graphic = _sprite;
            _open = Maths.LerpTowards(_open, _desiredOpen, 0.15f);
            _sprite.frame = (int)(_open * 32f);
            _collisionSize.y = ((1f - _open) * 32f);
        }
        public override void Removed()
        {
            if (Level.current is not Editor)
            {
                Level.Add(this);
                return;
            }
            base.Removed();
        }
        public override bool Destroy(DestroyType type = null)
        {
            return false;// base.Destroy(type);
        }
        public override void Draw()
        {
            base.Draw();
            //_top.depth = depth + 1;
            //_bottom.depth = depth + 1;
            //Graphics.Draw(_top, x, y - 27f);
            //Graphics.Draw(_bottom, x, y + 5f);
        }
    }
}

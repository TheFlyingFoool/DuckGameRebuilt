// Decompiled with JetBrains decompiler
// Type: DuckGame.VerticalDoor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

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
            this._sensorSprite = this._sprite = new SpriteMap("verticalDoor", 16, 32);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 24f);
            this.collisionSize = new Vec2(6f, 32f);
            this.collisionOffset = new Vec2(-3f, -24f);
            this.depth = - 0.5f;
            this._editorName = "Vertical Door";
            this.thickness = 3f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._bottom = new Sprite("verticalDoorBottom");
            this._bottom.CenterOrigin();
            this._top = new Sprite("verticalDoorTop");
            this._top.CenterOrigin();
            this.editorTooltip = "One of them science fiction type doors.";
        }

        public override void Update()
        {
            if (!this._cornerInit)
            {
                this._topLeft = this.topLeft;
                this._topRight = this.topRight;
                this._bottomLeft = this.bottomLeft;
                this._bottomRight = this.bottomRight;
                this._cornerInit = true;
            }
            if (!this.slideLocked)
            {
                this._sprite = this._sensorSprite;
                Duck duck = Level.CheckRect<Duck>(this._topLeft - new Vec2(18f, 0.0f), this._bottomRight + new Vec2(18f, 0.0f));
                if (duck != null)
                {
                    if (!this.filterDefault || !Profiles.IsDefault(duck.profile))
                        this._desiredOpen = 1f;
                    else if (!this.showedWarning)
                    {
                        HUD.AddPlayerChangeDisplay("@UNPLUG@|GRAY|NO ARCADE (SELECT A PROFILE)");
                        this.showedWarning = true;
                    }
                }
                else if (Level.CheckRectFilter<PhysicsObject>(new Vec2(this.x - 4f, this.y - 24f), new Vec2(this.x + 4f, this.y + 8f), (Predicate<PhysicsObject>)(d => !(d is TeamHat))) == null)
                    this._desiredOpen = 0.0f;
            }
            else
            {
                if (this._noSensorSprite == null)
                    this._noSensorSprite = new SpriteMap("verticalDoorNoSensor", 16, 32);
                this._sprite = this._noSensorSprite;
                this._desiredOpen = this.slideLockOpened ? 1f : 0.0f;
                if (Level.CheckRectFilter<PhysicsObject>(new Vec2(this.x - 4f, this.y - 24f), new Vec2(this.x + 4f, this.y + 8f), (Predicate<PhysicsObject>)(d => !(d is TeamHat))) != null && this._opened)
                    this._desiredOpen = 1f;
            }
            if ((double)this._desiredOpen > 0.5 && !this._opened)
            {
                this._opened = true;
                SFX.Play("slideDoorOpen", 0.6f);
            }
            if ((double)this._desiredOpen < 0.5 && this._opened)
            {
                this._opened = false;
                SFX.Play("slideDoorClose", 0.6f);
            }
            this.graphic = (Sprite)this._sprite;
            this._open = Maths.LerpTowards(this._open, this._desiredOpen, 0.15f);
            this._sprite.frame = (int)((double)this._open * 32.0);
            this._collisionSize.y = (float)((1.0 - (double)this._open) * 32.0);
        }

        public override void Draw()
        {
            base.Draw();
            this._top.depth = this.depth + 1;
            this._bottom.depth = this.depth + 1;
            Graphics.Draw(this._top, this.x, this.y - 27f);
            Graphics.Draw(this._bottom, this.x, this.y + 5f);
        }
    }
}

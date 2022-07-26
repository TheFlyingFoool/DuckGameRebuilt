// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidDoor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Pyramid", EditorItemType.Pyramid)]
    public class PyramidDoor : VerticalDoor, IPlatform
    {
        public PyramidDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("pyramidDoor", 16, 32);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 24f);
            this.collisionSize = new Vec2(10f, 32f);
            this.collisionOffset = new Vec2(-5f, -24f);
            this.depth = - 0.5f;
            this._editorName = "Pyramid Door";
            this.thickness = 3f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._bottom = new Sprite("pyramidDoorBottom");
            this._bottom.CenterOrigin();
            this._top = new Sprite("pyramidDoorTop");
            this._top.CenterOrigin();
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
            if (Level.CheckRect<Duck>(this._topLeft - new Vec2(18f, 0.0f), this._bottomRight + new Vec2(18f, 0.0f)) != null)
                this._desiredOpen = 1f;
            else if (Level.CheckRectFilter<PhysicsObject>(new Vec2(this.x - 4f, this.y - 24f), new Vec2(this.x + 4f, this.y + 8f), (Predicate<PhysicsObject>)(d => !(d is TeamHat))) == null)
                this._desiredOpen = 0.0f;
            if ((double)this._desiredOpen > 0.5 && !this._opened)
            {
                this._opened = true;
                SFX.Play("pyramidOpen", 0.6f);
            }
            if ((double)this._desiredOpen < 0.5 && this._opened)
            {
                this._opened = false;
                SFX.Play("pyramidClose", 0.6f);
            }
            this._open = Maths.LerpTowards(this._open, this._desiredOpen, 0.15f);
            this._sprite.frame = (int)((double)this._open * 32.0);
            this._collisionSize.y = (float)((1.0 - (double)this._open) * 32.0);
        }
    }
}

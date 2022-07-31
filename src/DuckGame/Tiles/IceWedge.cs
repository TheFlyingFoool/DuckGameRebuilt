// Decompiled with JetBrains decompiler
// Type: DuckGame.IceWedge
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class IceWedge : MaterialThing
    {
        public IceWedge(float xpos, float ypos, int dir)
          : base(xpos, ypos)
        {
            this._canFlipVert = true;
            this.graphic = new SpriteMap("iceWedge", 17, 17);
            this.hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            this.center = new Vec2(8f, 14f);
            this.collisionSize = new Vec2(14f, 8f);
            this.collisionOffset = new Vec2(-7f, -6f);
            this.depth = -0.9f;
        }

        public override void EditorUpdate() => base.EditorUpdate();

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.flipVertical)
            {
                if ((double)with.vSpeed < -1.0 && (this.offDir > 0 && (double)with.hSpeed < 1.0 || this.offDir < 0 && (double)with.hSpeed >= -1.0))
                    with.hSpeed = (float)(-(double)with.vSpeed * 1.5) * offDir;
                else if ((this.offDir < 0 && (double)with.right > (double)this.left + 4.0 || this.offDir > 0 && (double)with.left < (double)this.right - 4.0) && (this.offDir > 0 && (double)with.hSpeed < -1.0 || this.offDir < 0 && (double)with.hSpeed > 1.0) && (double)with.vSpeed < 0.5)
                    with.vSpeed = Math.Abs(with.hSpeed * 1.6f);
            }
            else if ((double)with.vSpeed > 1.0 && (this.offDir > 0 && (double)with.hSpeed < 1.0 || this.offDir < 0 && (double)with.hSpeed >= -1.0))
                with.hSpeed = with.vSpeed * 1.5f * offDir;
            else if ((this.offDir < 0 && (double)with.right > (double)this.left + 4.0 || this.offDir > 0 && (double)with.left < (double)this.right - 4.0) && (this.offDir > 0 && (double)with.hSpeed < -1.0 || this.offDir < 0 && (double)with.hSpeed > 1.0) && (double)with.vSpeed > -0.5)
                with.vSpeed = -Math.Abs(with.hSpeed * 1.6f);
            base.OnSoftImpact(with, from);
        }

        public override void Draw()
        {
            this.hugWalls = WallHug.None;
            if (this.flipVertical)
                this.hugWalls |= WallHug.Ceiling;
            else
                this.hugWalls |= WallHug.Floor;
            if (this.flipHorizontal)
                this.hugWalls |= WallHug.Right;
            else
                this.hugWalls |= WallHug.Left;
            this.angleDegrees = 0f;
            if (this.flipVertical)
            {
                if (this.flipHorizontal)
                {
                    this.angleDegrees = 180f;
                    this.center = new Vec2(8f, 14f);
                    this.collisionSize = new Vec2(14f, 8f);
                    this.collisionOffset = new Vec2(-7f, -2f);
                }
                else
                {
                    this.angleDegrees = 90f;
                    this.center = new Vec2(3f, 9f);
                    this.collisionSize = new Vec2(14f, 8f);
                    this.collisionOffset = new Vec2(-7f, -2f);
                }
            }
            else if (this.flipHorizontal)
            {
                this.angleDegrees = 270f;
                this.center = new Vec2(3f, 9f);
                this.collisionSize = new Vec2(14f, 8f);
                this.collisionOffset = new Vec2(-7f, -6f);
            }
            else
            {
                this.angleDegrees = 0f;
                this.center = new Vec2(8f, 14f);
                this.collisionSize = new Vec2(14f, 8f);
                this.collisionOffset = new Vec2(-7f, -6f);
            }
            base.Draw();
        }
    }
}

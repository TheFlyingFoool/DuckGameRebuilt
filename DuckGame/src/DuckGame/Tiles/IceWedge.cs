// Decompiled with JetBrains decompiler
// Type: DuckGame.IceWedge
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _canFlipVert = true;
            graphic = new SpriteMap("iceWedge", 17, 17);
            hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            center = new Vec2(8f, 14f);
            collisionSize = new Vec2(14f, 8f);
            collisionOffset = new Vec2(-7f, -6f);
            depth = -0.9f;
            shouldbeinupdateloop = false;
        }

        public override void EditorUpdate() => base.EditorUpdate();

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (flipVertical)
            {
                if (with.vSpeed < -1.0 && (offDir > 0 && with.hSpeed < 1.0 || offDir < 0 && with.hSpeed >= -1.0))
                    with.hSpeed = (float)(-with.vSpeed * 1.5) * offDir;
                else if ((offDir < 0 && with.right > left + 4.0 || offDir > 0 && with.left < right - 4.0) && (offDir > 0 && with.hSpeed < -1.0 || offDir < 0 && with.hSpeed > 1.0) && with.vSpeed < 0.5)
                    with.vSpeed = Math.Abs(with.hSpeed * 1.6f);
            }
            else if (with.vSpeed > 1.0 && (offDir > 0 && with.hSpeed < 1.0 || offDir < 0 && with.hSpeed >= -1.0))
                with.hSpeed = with.vSpeed * 1.5f * offDir;
            else if ((offDir < 0 && with.right > left + 4.0 || offDir > 0 && with.left < right - 4.0) && (offDir > 0 && with.hSpeed < -1.0 || offDir < 0 && with.hSpeed > 1.0) && with.vSpeed > -0.5)
                with.vSpeed = -Math.Abs(with.hSpeed * 1.6f);
            base.OnSoftImpact(with, from);
        }

        public override void Draw()
        {
            hugWalls = WallHug.None;
            if (flipVertical)
                hugWalls |= WallHug.Ceiling;
            else
                hugWalls |= WallHug.Floor;
            if (flipHorizontal)
                hugWalls |= WallHug.Right;
            else
                hugWalls |= WallHug.Left;
            angleDegrees = 0f;
            if (flipVertical)
            {
                if (flipHorizontal)
                {
                    angleDegrees = 180f;
                    center = new Vec2(8f, 14f);
                    collisionSize = new Vec2(14f, 8f);
                    collisionOffset = new Vec2(-7f, -2f);
                }
                else
                {
                    angleDegrees = 90f;
                    center = new Vec2(3f, 9f);
                    collisionSize = new Vec2(14f, 8f);
                    collisionOffset = new Vec2(-7f, -2f);
                }
            }
            else if (flipHorizontal)
            {
                angleDegrees = 270f;
                center = new Vec2(3f, 9f);
                collisionSize = new Vec2(14f, 8f);
                collisionOffset = new Vec2(-7f, -6f);
            }
            else
            {
                angleDegrees = 0f;
                center = new Vec2(8f, 14f);
                collisionSize = new Vec2(14f, 8f);
                collisionOffset = new Vec2(-7f, -6f);
            }
            base.Draw();
        }
    }
}

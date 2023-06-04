// Decompiled with JetBrains decompiler
// Type: DuckGame.VerticalFunBeam
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class VerticalFunBeam : FunBeam
    {
        public VerticalFunBeam(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _editorName = "Fun Beam Vertical";
            editorTooltip = "Ever seen a fun beam? Now try tilting your head sideways.";
            hugWalls = WallHug.Ceiling;
            angleDegrees = 90f;
            collisionOffset = new Vec2(-5f, -2f);
            collisionSize = new Vec2(10f, 4f);
            _placementCost += 2;
            editorCycleType = typeof(FunBeam);
        }

        public override void Draw()
        {
            if (Editor.editorDraw)
                return;
            if (enabled)
            {
                if (_prev != position)
                {
                    _endPoint = Vec2.Zero;
                    for (int index = 0; index < 32; ++index)
                    {
                        Thing thing = Level.CheckLine<Block>(position + new Vec2(0f, 4 + index * 16), position + new Vec2(0f, (index + 1) * 16 - 6));
                        if (thing != null)
                        {
                            _endPoint = new Vec2(x, thing.top - 2f);
                            break;
                        }
                    }
                    _prev = position;
                }
                if (_endPoint != Vec2.Zero)
                {
                    graphic.flipH = true;
                    graphic.depth = depth;
                    graphic.angleDegrees = 90f;
                    Graphics.Draw(graphic, _endPoint.x, _endPoint.y);
                    graphic.flipH = false;
                    _beam.depth = depth - 2;
                    float y = _endPoint.y - this.y;
                    int num = (int)Math.Ceiling(y / 16);
                    for (int index = 0; index < num; ++index)
                    {
                        if (index == num - 1)
                            _beam.cutWidth = 16 - (int)(y % 16);
                        else
                            _beam.cutWidth = 0;
                        _beam.angleDegrees = 90f;
                        Graphics.Draw(_beam, x, this.y + index * 16);
                    }
                    collisionOffset = new Vec2(-4f, -1f);
                    collisionSize = new Vec2(8f, y);
                }
                else
                {
                    collisionOffset = new Vec2(-5f, -1f);
                    collisionSize = new Vec2(10f, 4f);
                }
            }
            base.Draw();
        }
    }
}

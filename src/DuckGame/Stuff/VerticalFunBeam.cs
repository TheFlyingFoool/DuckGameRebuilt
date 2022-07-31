// Decompiled with JetBrains decompiler
// Type: DuckGame.VerticalFunBeam
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._editorName = "Fun Beam Vertical";
            this.editorTooltip = "Ever seen a fun beam? Now try tilting your head sideways.";
            this.hugWalls = WallHug.Ceiling;
            this.angleDegrees = 90f;
            this.collisionOffset = new Vec2(-5f, -2f);
            this.collisionSize = new Vec2(10f, 4f);
            this._placementCost += 2;
        }

        public override void Draw()
        {
            if (Editor.editorDraw)
                return;
            if (this.enabled)
            {
                if (this._prev != this.position)
                {
                    this._endPoint = Vec2.Zero;
                    for (int index = 0; index < 32; ++index)
                    {
                        Thing thing = Level.CheckLine<Block>(this.position + new Vec2(0f, 4 + index * 16), this.position + new Vec2(0f, (index + 1) * 16 - 6));
                        if (thing != null)
                        {
                            this._endPoint = new Vec2(this.x, thing.top - 2f);
                            break;
                        }
                    }
                    this._prev = this.position;
                }
                if (this._endPoint != Vec2.Zero)
                {
                    this.graphic.flipH = true;
                    this.graphic.depth = this.depth;
                    this.graphic.angleDegrees = 90f;
                    Graphics.Draw(this.graphic, this._endPoint.x, this._endPoint.y);
                    this.graphic.flipH = false;
                    this._beam.depth = this.depth - 2;
                    float y = this._endPoint.y - this.y;
                    int num = (int)Math.Ceiling(y / 16.0);
                    for (int index = 0; index < num; ++index)
                    {
                        if (index == num - 1)
                            this._beam.cutWidth = 16 - (int)(y % 16.0);
                        else
                            this._beam.cutWidth = 0;
                        this._beam.angleDegrees = 90f;
                        Graphics.Draw(_beam, this.x, this.y + index * 16);
                    }
                    this.collisionOffset = new Vec2(-4f, -1f);
                    this.collisionSize = new Vec2(8f, y);
                }
                else
                {
                    this.collisionOffset = new Vec2(-5f, -1f);
                    this.collisionSize = new Vec2(10f, 4f);
                }
            }
            base.Draw();
        }
    }
}

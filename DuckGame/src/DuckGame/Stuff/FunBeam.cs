// Decompiled with JetBrains decompiler
// Type: DuckGame.FunBeam
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class FunBeam : MaterialThing
    {
        protected SpriteMap _beam;
        protected Vec2 _prev = Vec2.Zero;
        protected Vec2 _endPoint = Vec2.Zero;
        public bool enabled = true;

        public FunBeam(float xpos, float ypos)
          : base(xpos, ypos)
        {
            shouldbegraphicculled = false;
            _beam = new SpriteMap("funBeam", 16, 16);
            _beam.ClearAnimations();
            _beam.AddAnimation("idle", 1f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            _beam.SetAnimation("idle");
            _beam.speed = 0.2f;
            _beam.alpha = 0.3f;
            _beam.center = new Vec2(0f, 8f);
            graphic = new Sprite("funBeamer");
            center = new Vec2(9f, 8f);
            collisionOffset = new Vec2(-2f, -5f);
            collisionSize = new Vec2(4f, 10f);
            depth = -0.5f;
            _editorName = "Fun Beam";
            editorTooltip = "Place 2 generators near each other to create a beam that triggers weapons passing through.";
            hugWalls = WallHug.Left;
            editorCycleType = typeof(VerticalFunBeam);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!enabled || !(with is Gun gun))
                return;
            switch (gun)
            {
                case Sword _:
                    break;
                case SledgeHammer _:
                    break;
                default:
                    gun.PressAction();
                    break;
            }
        }

        public override void Draw()
        {
            if (Editor.editorDraw)
                return;

            if (enabled && GetType() == typeof(FunBeam))
            {
                if (_prev != position)
                {
                    _endPoint = Vec2.Zero;
                    for (int index = 0; index < 32; ++index)
                    {
                        Thing thing = Level.CheckLine<Block>(position + new Vec2(4 + index * 16, 0f), position + new Vec2((index + 1) * 16 - 6, 0f));
                        if (thing != null)
                        {
                            _endPoint = new Vec2(thing.left - 2f, y);
                            break;
                        }
                    }
                    _prev = position;
                }
                if (_endPoint != Vec2.Zero)
                {
                    graphic.flipH = true;
                    graphic.depth = depth;
                    Graphics.Draw(graphic, _endPoint.x, _endPoint.y);
                    graphic.flipH = false;
                    _beam.depth = depth - 2;
                    float x = _endPoint.x - this.x;
                    int num = (int)Math.Ceiling(x / 16);
                    for (int index = 0; index < num; ++index)
                    {
                        _beam.cutWidth = index != num - 1 ? 0 : 16 - (int)(x % 16);
                        Graphics.Draw(_beam, this.x + index * 16, y);
                    }
                    collisionOffset = new Vec2(-1f, -4f);
                    collisionSize = new Vec2(x, 8f);
                }
                else
                {
                    collisionOffset = new Vec2(-1f, -5f);
                    collisionSize = new Vec2(4f, 10f);
                }
            }
            base.DrawLerpLess();
        }
    }
}

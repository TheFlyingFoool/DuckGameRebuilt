// Decompiled with JetBrains decompiler
// Type: DuckGame.FunBeam
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._beam = new SpriteMap("funBeam", 16, 16);
            this._beam.ClearAnimations();
            this._beam.AddAnimation("idle", 1f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            this._beam.SetAnimation("idle");
            this._beam.speed = 0.2f;
            this._beam.alpha = 0.3f;
            this._beam.center = new Vec2(0.0f, 8f);
            this.graphic = new Sprite("funBeamer");
            this.center = new Vec2(9f, 8f);
            this.collisionOffset = new Vec2(-2f, -5f);
            this.collisionSize = new Vec2(4f, 10f);
            this.depth = -0.5f;
            this._editorName = "Fun Beam";
            this.editorTooltip = "Place 2 generators near each other to create a beam that triggers weapons passing through.";
            this.hugWalls = WallHug.Left;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!this.enabled || !(with is Gun gun))
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
            if (this.enabled && this.GetType() == typeof(FunBeam))
            {
                if (this._prev != this.position)
                {
                    this._endPoint = Vec2.Zero;
                    for (int index = 0; index < 32; ++index)
                    {
                        Thing thing = Level.CheckLine<Block>(this.position + new Vec2(4 + index * 16, 0.0f), this.position + new Vec2((index + 1) * 16 - 6, 0.0f));
                        if (thing != null)
                        {
                            this._endPoint = new Vec2(thing.left - 2f, this.y);
                            break;
                        }
                    }
                    this._prev = this.position;
                }
                if (this._endPoint != Vec2.Zero)
                {
                    this.graphic.flipH = true;
                    this.graphic.depth = this.depth;
                    Graphics.Draw(this.graphic, this._endPoint.x, this._endPoint.y);
                    this.graphic.flipH = false;
                    this._beam.depth = this.depth - 2;
                    float x = this._endPoint.x - this.x;
                    int num = (int)Math.Ceiling((double)x / 16.0);
                    for (int index = 0; index < num; ++index)
                    {
                        this._beam.cutWidth = index != num - 1 ? 0 : 16 - (int)((double)x % 16.0);
                        Graphics.Draw(_beam, this.x + index * 16, this.y);
                    }
                    this.collisionOffset = new Vec2(-1f, -4f);
                    this.collisionSize = new Vec2(x, 8f);
                }
                else
                {
                    this.collisionOffset = new Vec2(-1f, -5f);
                    this.collisionSize = new Vec2(4f, 10f);
                }
            }
            base.Draw();
        }
    }
}

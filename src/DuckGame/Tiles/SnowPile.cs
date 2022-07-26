// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowPile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class SnowPile : MaterialThing
    {
        public bool kill;
        private bool melt;
        private Predicate<MaterialThing> _collisionPred;

        public SnowPile(float xpos, float ypos, int dir)
          : base(xpos, ypos)
        {
            this.graphic = (Sprite)new SpriteMap("bigDrift", 32, 32);
            this.hugWalls = WallHug.Floor;
            this.center = new Vec2(12f, 24f);
            this.collisionSize = new Vec2(24f, 10f);
            this.collisionOffset = new Vec2(-12f, -2f);
            this.layer = Layer.Game;
            this.depth = (Depth)0.85f;
            this.editorTooltip = "A nice, big, fluffy sneaky snow pile.";
        }

        public override void Update()
        {
            if (this._collisionPred == null)
                this._collisionPred = (Predicate<MaterialThing>)(thing => thing == null || !Collision.Rect(this.topLeft, this.bottomRight, (Thing)thing));
            this.clip.RemoveWhere(this._collisionPred);
            if (this.melt)
            {
                this.alpha -= 0.0012f;
                this.yscale -= 0.015f;
                this.y += 0.13f;
            }
            if ((double)this.yscale < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!this.kill && ((double)with.impactPowerV > 2.0 || (double)with.impactPowerH > 2.0))
            {
                this.clip.Add(with);
                float num1 = with.impactPowerV;
                float num2 = -with.impactDirectionH;
                if ((double)num1 > 6.0)
                    num1 = 6f;
                if ((double)num2 > 6.0)
                    num2 = 6f;
                if ((double)num1 < 2.0)
                    num1 = 2f;
                for (int index = 0; index < 20; ++index)
                {
                    float num3 = 1f;
                    if (index < 10)
                        num3 = 0.7f;
                    Level.Add((Thing)new SnowFallParticle(this.x + Rando.Float(-9f, 9f), this.y + 7f + Rando.Float(-16f, 0.0f), new Vec2((float)((double)num2 * (double)num3 * 0.100000001490116) + Rando.Float((float)(-0.200000002980232 * ((double)num1 * (double)num3)), (float)(0.200000002980232 * ((double)num1 * (double)num3))), (float)(-(double)Rando.Float(0.8f, 1.5f) * ((double)num1 * (double)num3 * 0.150000005960464))), index < 6));
                }
            }
            base.OnSoftImpact(with, from);
        }

        public override void HeatUp(Vec2 location)
        {
            this.melt = true;
            base.HeatUp(location);
        }

        public override void Draw()
        {
            this.graphic.flipH = this.flipHorizontal;
            base.Draw();
        }
    }
}

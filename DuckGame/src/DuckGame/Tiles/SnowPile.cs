// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowPile
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            graphic = new SpriteMap("bigDrift", 32, 32);
            hugWalls = WallHug.Floor;
            center = new Vec2(12f, 24f);
            collisionSize = new Vec2(24f, 10f);
            collisionOffset = new Vec2(-12f, -2f);
            layer = Layer.Game;
            depth = (Depth)0.85f;
            editorTooltip = "A nice, big, fluffy sneaky snow pile.";
        }

        public override void Update()
        {
            if (_collisionPred == null)
                _collisionPred = thing => thing == null || !Collision.Rect(topLeft, bottomRight, thing);
            clip.RemoveWhere(_collisionPred);
            if (melt)
            {
                alpha -= 0.0012f;
                yscale -= 0.015f;
                y += 0.13f;
            }
            if (yscale < 0)
                Level.Remove(this);
            base.Update();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!kill && (with.impactPowerV > 2 || with.impactPowerH > 2))
            {
                clip.Add(with);
                float num1 = with.impactPowerV;
                float num2 = -with.impactDirectionH;
                if (num1 > 6)
                    num1 = 6f;
                if (num2 > 6)
                    num2 = 6f;
                if (num1 < 2)
                    num1 = 2f;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 20; ++index)
                {
                    float num3 = 1f;
                    if (index < 10)
                        num3 = 0.7f;
                    Level.Add(new SnowFallParticle(x + Rando.Float(-9f, 9f), y + 7f + Rando.Float(-16f, 0f), new Vec2((float)(num2 * num3 * 0.1f) + Rando.Float((float)(-0.2f * (num1 * num3)), (float)(0.2f * (num1 * num3))), (float)(-Rando.Float(0.8f, 1.5f) * (num1 * num3 * 0.15f))), index < 6));
                }
            }
            base.OnSoftImpact(with, from);
        }

        public override void HeatUp(Vec2 location)
        {
            melt = true;
            base.HeatUp(location);
        }

        public override void Draw()
        {
            graphic.flipH = flipHorizontal;
            base.Draw();
        }
    }
}

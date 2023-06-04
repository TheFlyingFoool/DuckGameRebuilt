// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowDrift
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class SnowDrift : MaterialThing
    {
        public EditorProperty<int> style;
        public bool kill;
        private bool melt;

        public override void EditorPropertyChanged(object property)
        {
            if ((int)style == -1)
                (graphic as SpriteMap).frame = Rando.Int(3);
            else
                (graphic as SpriteMap).frame = style.value;
        }

        public SnowDrift(float xpos, float ypos, int dir)
          : base(xpos, ypos)
        {
            style = new EditorProperty<int>(-1, this, -1f, 3f, 1f);
            graphic = new SpriteMap("drifts", 16, 17);
            if ((int)style == -1)
                (graphic as SpriteMap).frame = Rando.Int(3);
            hugWalls = WallHug.Floor;
            center = new Vec2(8f, 14f);
            collisionSize = new Vec2(14f, 4f);
            collisionOffset = new Vec2(-7f, -2f);
            layer = Layer.Blocks;
            depth = (Depth)0.5f;
            editorTooltip = "The safest drift of all!";
        }

        public override void Update()
        {
            if (kill)
            {
                alpha -= 0.012f;
                yscale -= 0.15f;
                xscale += 0.12f;
                y += 0.44f;
            }
            if (melt)
            {
                alpha -= 0.0036f;
                yscale -= 0.045f;
                xscale += 0.036f;
                y += 0.16f;
            }
            if (yscale < 1)
                Level.Remove(this);
            base.Update();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!kill && with.impactPowerV > 2f)
            {
                float vPower = with.impactPowerV;
                float hPower = with.impactDirectionH;
                if (vPower > 6f)
                {
                    vPower = 6f;
                }
                if (hPower > 6f)
                {
                    hPower = 6f;
                }
                for (int i = 0; i < DGRSettings.ActualParticleMultiplier * 12; i++)
                {
                    float mul = 1f;
                    if (i < 10)
                    {
                        mul = 0.7f;
                    }
                    Level.Add(new SnowFallParticle(x + Rando.Float(-8f, 8f), y + Rando.Float(-6f, 0f), new Vec2(hPower * mul * 0.1f + Rando.Float(-0.2f * (vPower * mul), 0.2f * (vPower * mul)), -Rando.Float(0.8f, 1.5f) * (vPower * mul * 0.15f)), i < 6));
                }
                kill = true;
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

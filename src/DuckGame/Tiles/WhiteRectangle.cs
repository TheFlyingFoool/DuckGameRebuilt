// Decompiled with JetBrains decompiler
// Type: DuckGame.WhiteRectangle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WhiteRectangle : Thing
    {
        public Vec2 size;
        public Sprite gradientLine;
        public Sprite edge;
        public Sprite edgeVert;
        public bool water;

        public WhiteRectangle(float xpos, float ypos, float wide, float high, bool waterVal = false)
          : base(xpos, ypos)
        {
            this.size = new Vec2(wide, high);
            this.layer = Layer.Lighting;
            this.gradientLine = new Sprite("lavaGlowLine");
            this.edge = new Sprite("lavaGlowEdge");
            this.edgeVert = new Sprite("lavaGlowEdgeVert");
            this.edge.center = new Vec2(32f, 32f);
            this.water = waterVal;
            if (!this.water)
                return;
            this.edge = new Sprite("waterLighting");
        }

        public override void Draw()
        {
            if (this.water)
            {
                Graphics.DrawTexturedLine(this.edge.texture, this.position + new Vec2(0.0f, this.size.y / 2f), this.position + new Vec2(this.size.x, this.size.y / 2f), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), this.size.y / 20f, (Depth)1f);
            }
            else
            {
                Graphics.DrawRect(this.position, this.position + this.size, new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), (Depth)1f);
                Graphics.DrawTexturedLine(this.gradientLine.texture, this.position + new Vec2(0.0f, 0.0f), this.position + new Vec2(this.size.x, 0.0f), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), depth: ((Depth)1f));
                Graphics.DrawTexturedLine(this.gradientLine.texture, this.position + new Vec2(0.0f, this.size.y + 1f), this.position + new Vec2(this.size.x, this.size.y + 1f), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), depth: ((Depth)1f));
                Graphics.DrawTexturedLine(this.gradientLine.texture, this.position + new Vec2(0.0f, 0.0f), this.position + new Vec2(0.0f, this.size.y), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), 0.5f, (Depth)1f);
                Graphics.DrawTexturedLine(this.gradientLine.texture, this.position + new Vec2(this.size.x - 1f, 0.0f), this.position + new Vec2(this.size.x - 1f, this.size.y), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), 0.5f, (Depth)1f);
                this.edgeVert.xscale = 0.5f;
                this.edge.xscale = 0.5f;
                this.edge.flipH = false;
                this.edge.color = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0);
                this.edgeVert.color = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0);
                Graphics.Draw(this.edge, this.x, this.y);
                this.edge.flipH = true;
                Graphics.Draw(this.edge, this.x + this.size.x, this.y);
                this.edgeVert.flipH = true;
                Graphics.Draw(this.edgeVert, (float)((double)this.x + (double)this.size.x + 16.0), this.y + this.size.y);
                this.edgeVert.flipH = false;
                Graphics.Draw(this.edgeVert, this.x - 16f, this.y + this.size.y);
                base.Draw();
            }
        }
    }
}

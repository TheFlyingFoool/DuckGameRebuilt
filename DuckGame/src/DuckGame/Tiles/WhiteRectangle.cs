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
            shouldbegraphicculled = false;
            size = new Vec2(wide, high);
            layer = Layer.Lighting;
            gradientLine = new Sprite("lavaGlowLine");
            edge = new Sprite("lavaGlowEdge");
            edgeVert = new Sprite("lavaGlowEdgeVert");
            edge.center = new Vec2(32f, 32f);
            water = waterVal;
            if (!water)
                return;
            edge = new Sprite("waterLighting");
        }

        public override void Draw()
        {
            if (water)
            {
                Graphics.DrawTexturedLine(edge.texture, position + new Vec2(0f, size.y / 2f), position + new Vec2(size.x, size.y / 2f), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), size.y / 20f, (Depth)1f);
            }
            else
            {
                Graphics.DrawRect(position, position + size, new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), (Depth)1f);
                Graphics.DrawTexturedLine(gradientLine.texture, position + new Vec2(0f, 0f), position + new Vec2(size.x, 0f), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), depth: ((Depth)1f));
                Graphics.DrawTexturedLine(gradientLine.texture, position + new Vec2(0f, size.y + 1f), position + new Vec2(size.x, size.y + 1f), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), depth: ((Depth)1f));
                Graphics.DrawTexturedLine(gradientLine.texture, position + new Vec2(0f, 0f), position + new Vec2(0f, size.y), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), 0.5f, (Depth)1f);
                Graphics.DrawTexturedLine(gradientLine.texture, position + new Vec2(size.x - 1f, 0f), position + new Vec2(size.x - 1f, size.y), new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0), 0.5f, (Depth)1f);
                edgeVert.xscale = 0.5f;
                edge.xscale = 0.5f;
                edge.flipH = false;
                edge.color = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0);
                edgeVert.color = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0);
                Graphics.Draw(edge, x, y);
                edge.flipH = true;
                Graphics.Draw(edge, x + size.x, y);
                edgeVert.flipH = true;
                Graphics.Draw(edgeVert, (float)(x + size.x + 16), y + size.y);
                edgeVert.flipH = false;
                Graphics.Draw(edgeVert, x - 16f, y + size.y);
                base.Draw();
            }
        }
    }
}

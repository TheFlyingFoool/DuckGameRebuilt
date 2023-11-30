using System.Collections.Generic;

namespace DuckGame
{
    public class ExplosionDecal : Thing
    {
        public ExplosionDecal(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("explosionDecal" + Rando.Int(1, 4));
            center = new Vec2(16);
            needRefreshRect = true;
            layer = Layer.Foreground;
            depth = 1;
            alpha = Rando.Float(0.2f, 0.3f);
            collisionSize = new Vec2(32);
            _collisionOffset = new Vec2(-16);
            angleDegrees = 180;
        }
        public bool needRefreshRect;
        public List<Rectangle> ontoDraw = new List<Rectangle>();
        public List<Thing> needBeCheck = new List<Thing>();
        public override void Initialize()
        {
            RefreshRects();
            base.Initialize();
        }
        public override void Update()
        {
            for (int i = 0; i < needBeCheck.Count; i++)
            {
                if (needBeCheck[i].removeFromLevel)
                {
                    needRefreshRect = true;
                    break;
                }
            }
            if (needRefreshRect) RefreshRects();
            if (ontoDraw.Count == 0 || !DGRSettings.ExplosionDecals) Level.Remove(this);
        }
        public void RefreshRects()
        {
            needRefreshRect = false;

            ontoDraw.Clear();

            foreach (Block b in Level.CheckRectAll<Block>(topLeft, bottomRight))
            {
                if (b is Door || b is VerticalDoor || b is PyramidDoor || b is Window || b is InvisibleBlock || b is ItemBox) continue;
                if (b is BlockGroup bg)
                {
                    for (int i = 0; i < bg.blocks.Count; i++)
                    {
                        Block block = bg.blocks[i];
                        if (Collision.Rect(rectangle, block.rectangle))
                        {
                            ontoDraw.Add(block.rectangle);
                        }
                    }
                }
                else
                {
                    ontoDraw.Add(b.rectangle);
                }
                needBeCheck.Add(b);
            }
        }
        public override void Draw()
        {
            graphic.alpha = alpha;
            graphic.center = center;
            graphic.depth = 1;
            graphic.texture.skipSpriteAtlas = true;
            graphic.SkipIntraTick = 10;
            graphic.angle = angle;
            for (int i = 0; i < ontoDraw.Count; i++)
            {
                Rectangle rect = ontoDraw[i];

                Vec2 vec = new Vec2(x - rect.x, y - rect.y);

                Rectangle r = new Rectangle(vec.x, vec.y, rect.width, rect.height);
                Graphics.Draw(graphic, rect.x + (rect.width - 16), rect.y + (rect.height - 16), r);
            }
            //base.Draw();
        }
    }
}

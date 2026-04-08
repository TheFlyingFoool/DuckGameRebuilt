using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    [ClientOnly]
    public class NMDuckShatter : NMEvent
    {
        public NMDuckShatter(Vec2 vec, Vec2 s, Duck d, int i)
        {
            v = vec;
            speed = s;
            duck = d;
            imageIndex = i;
        }

        public NMDuckShatter() { }

        public Vec2 v;
        public Vec2 speed;
        public Duck duck;
        public int imageIndex;

        public override void Activate()
        {
            SFX.Play("glassBreak");

            Sprite spr = duck.graphic.Clone();
            SpriteMap map = duck.graphic as SpriteMap;

            map.imageIndex = imageIndex;
            map.UpdateSpriteBox();

            Rectangle cutout = Extensions.GetPrivateFieldValue<Rectangle>(map, "_spriteBox");


            Texture2D baseTex = Extensions.GetPrivateFieldValue<Texture2D>(spr.texture, "_base");

            int texWidth = baseTex.Width;
            int texHeight = baseTex.Height;


            Color[] fullData = new Color[texWidth * texHeight];
            baseTex.GetData(fullData);

            int cutX = (int)cutout.x;
            int cutY = (int)cutout.y;
            int cutW = (int)cutout.width;
            int cutH = (int)cutout.height;

            Color[] cutData = new Color[cutW * cutH];

            for (int y = 0; y < cutH; y++)
            {
                for (int x = 0; x < cutW; x++)
                {
                    int srcIndex = (cutY + y) * texWidth + (cutX + x);
                    int dstIndex = y * cutW + x;

                    cutData[dstIndex] = fullData[srcIndex];
                }
            }

            int xM = cutW / 8;
            int yM = cutH / 8;
            for (int y = 0; y < yM; y++)
            {
                for (int x = 0; x < xM; x++)
                {
                    Color[] chunkData = new Color[8 * 8];

                    for (int cy = 0; cy < 8; cy++)
                    {
                        for (int cx = 0; cx < 8; cx++)
                        {
                            int srcIndex = (y * 8 + cy) * cutW + (x * 8 + cx);
                            int dstIndex = cy * 8 + cx;

                            chunkData[dstIndex] = cutData[srcIndex];
                        }
                    }

                    Texture2D chunkTex = new Texture2D(Graphics.device, 8, 8);
                    chunkTex.SetData(chunkData);

                    Sprite s = new Sprite(new Tex2D(chunkTex, "shatter"));

                    for (int i = 0; i < Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 64); i++)
                    {
                        Level.Add(new ShatterDuck(
                            v.x + 8 * x,
                            v.y + 8 * y,
                            s,
                            speed * 0.5f
                        ));
                    }
                }
            }
        }
    }
}
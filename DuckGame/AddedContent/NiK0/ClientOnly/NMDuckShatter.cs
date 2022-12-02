using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
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
        public NMDuckShatter()
        {
        }
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


            Bitmap bm;

            using (MemoryStream ms = new MemoryStream())
            {
                Texture2D bas = Extensions.GetPrivateFieldValue<Texture2D>(spr.texture, "_base");
                bas.SaveAsPng(ms, bas.Width, bas.Height);
                Bitmap m = new Bitmap(ms);
                bm = m.Clone(new System.Drawing.Rectangle((int)cutout.x, (int)cutout.y, (int)cutout.width, (int)cutout.height), PixelFormat.DontCare);
            }

            int xM = bm.Width / 8;
            int yM = bm.Height / 8;

            for (int y = 0; y < yM; y++)
            {
                for (int x = 0; x < xM; x++)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Bitmap botmop = bm.Clone(new System.Drawing.Rectangle(8 * x, 8 * y, 8, 8), PixelFormat.DontCare);
                        Sprite s = new Sprite(new Tex2D(TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, botmop, false), "shatter"));
                        for (int i = 0; i < Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 64); i++)
                        {
                            Level.Add(new ShatterDuck(v.x + 8 * x, v.y + 8 * y, s, speed * 0.5f));
                        }
                    }
                }
            }
        }
    }
}

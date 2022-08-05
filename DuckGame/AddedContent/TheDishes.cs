using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class TheDishes : Holdable
    {
        public const string DirtySprite = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAKpJREFUOE9jZKAQMOLSXx8i///Vdz64tBjnJ4bGNQ8x1KMIZHrr/gcpBAFsirFZBjcAZCMpGmGG4fQCum24LCDKAFh4IIcDSAzkTbABMA6+AEX3Hii8pm+9TJwBsMBFDlgUA9AVgPggG3FFHbKrsXqBGANQYgE5hJHTAq5oRQ4zeCzA/AQzDF+awGsAoawBi1JQDIDUoqQDYlIjepQTnZlgNqK7kKiUiM9bALxWgBFUCrntAAAAAElFTkSuQmCC";
        public const string BubblesSprite = "iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAYAAADgkQYQAAAAAXNSR0IArs4c6QAAAFtJREFUKFNjZEAChos+/Ydxz8fxMcLYYAZMElkCWQyuqNHqL1ijn4og3ASQQpBGRhhj0533YKuQFcFsgStCdhu6O7EqApkKMxFkE9xNyI6GmQR3E9G+Q9aJLZwAgwhFVjPCO4EAAAAASUVORK5CYIIA";
        public Sprite dirty;
        public Sprite bubbles;
        public TheDishes(float xpos, float ypos) : base(xpos, ypos)
        {
            bubbles = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(BubblesSprite))), "bouble"));
            bubbles.center = new Vec2(4.5f);
            dirty = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(DirtySprite))), "dirty"));
            dirty.center = new Vec2(8);
            graphic = new Sprite("furni/momento/plate");
            collisionSize = new Vec2(16);
            center = new Vec2(8);
            _collisionOffset = new Vec2(-8);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is IAmADuck iaad && isServerForObject && Math.Abs(hSpeed) > 4)
            {
                Duck d = Duck.GetAssociatedDuck((MaterialThing)iaad);
                if (d != null && !d.dead)
                {
                    if (iaad is Duck dd)
                    {
                        UnstoppableFondle(dd, DuckNetwork.localConnection);

                        Sprite spr = dd.graphic.Clone();
                        Rectangle cutout = Extensions.GetPrivateFieldValue<Rectangle>(dd.graphic as SpriteMap, "_spriteBox");


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
                                    Level.Add(new ShatterDuck(dd.x + 8 * x, dd.y + 8 * y, new Sprite(new Tex2D(TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, botmop, false), "shatter")), velocity * 0.5f));
                                }
                            }
                        }


                        dd.y = -9999;
                        dd.x = -5000;
                        dd.Kill(new DTImpact(this));
                        if (dd.ragdoll != null)
                        {
                            dd.ragdoll.part1.x = -5000;
                            dd.ragdoll.part2.x = -5000;
                            dd.ragdoll.part3.x = -5000;
                            dd.ragdoll.x = -5000;
                        }
                    }
                    else
                    {
                        UnstoppableFondle(d, DuckNetwork.localConnection);
                        d.Kill(new DTImpact(this));
                    }
                }
            }
            base.OnSoftImpact(with, from);
        }
        public float dirtyness;
        public override void Draw()
        {
            dirty.angle = angle;
            dirty.alpha = dirtyness;
            Graphics.Draw(dirty, x, y, depth + 2);
            base.Draw();
        }
        public override void Update()
        {
            if (isServerForObject)
            {
                FluidPuddle p = Level.CheckRect<FluidPuddle>(topLeft, bottomRight);
                if (p != null)
                {
                    if (p.data.color.x < 0.1f && p.data.color.y > 0.54f && p.data.color.y < 0.62f && p.data.color.z > 0.93f && p.data.color.z < 1.1f)
                    {
                        dirtyness -= 0.01f;
                    }
                    angleDegrees += hSpeed * 2;
                }
                else if (grounded)
                {
                    dirtyness += Math.Abs(hSpeed) / 250;
                    angleDegrees += hSpeed * 3;
                    if (dirtyness < 0.7f && Rando.Int(16 - (int)Math.Abs(hSpeed)) == 0)
                    {
                        Level.Add(new Bubble(x + Rando.Float(-6, 6), y + Rando.Float(-4, 4), bubbles));
                    }
                }
                dirtyness = Maths.Clamp(dirtyness, 0, 1);
            }
            friction = 0.03f + Maths.Clamp(dirtyness, 0, 0.2f);
            base.Update();
        }
        public StateBinding _dirtynessBinding = new StateBinding("dirtyness");
    }
}

using System;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Stuff|Props")]
    public class TheDishes : Holdable
    {
        public Sprite dirty;
        public Sprite bubbles;
        public TheDishes(float xpos, float ypos) : base(xpos, ypos)
        {
            bubbles = new Sprite("bouble")
            {
                center = new Vec2(4.5f)
            };
            dirty = new Sprite("dirtydishoverlay")
            {
                center = new Vec2(8)
            };
            graphic = new Sprite("furni/momento/plate");
            collisionSize = new Vec2(16);
            center = new Vec2(8);
            _collisionOffset = new Vec2(-8);
            collideSounds.Add("glassBump");
            bubbles.Namebase = "bouble";
            dirty.Namebase = "dirty";
            Content.textures[dirty.Namebase] = dirty.texture;
            Content.textures[bubbles.Namebase] = bubbles.texture;
            _editorName = "Dishes";
            editorTooltip = "To this day scientists don't know why dishes are unbreakable";
            //editorTooltip = "There was a typo when this virtual simulation was made that made it so dishes are unbreakable";
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
                                        dd.x + 8 * x,
                                        dd.y + 8 * y,
                                        s,
                                        velocity * 0.5f
                                    ));
                                }
                            }
                        }

                        Send.Message(new NMDuckShatter(dd.position, velocity, dd, ((SpriteMap)dd.graphic).imageIndex));

                        SFX.Play("glassBreak");

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
                    if (dirtyness < 0.7f && Rando.Int(Maths.Clamp(16 - (int)Math.Abs(hSpeed), 1, 16)) == 0)
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

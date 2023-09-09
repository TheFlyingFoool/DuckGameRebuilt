using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace DuckGame
{
    public class CrumbleShamble : Block, IDrawToDifferentLayers
    {
        public SpriteMap platform;
        public Sprite eddie;
        public CrumbleShamble(float xpos, float ypos) : base(xpos, ypos)
        {
            platform = new SpriteMap("title/beamPlatform2", 84, 22);
            eddie = new Sprite("title/eddie");
            graphic = platform;
            center = new Vec2(42, 11);
            _collisionSize = new Vec2(37f, 21);
            _collisionOffset = new Vec2(-18f, -9);
        }
        public bool broken;
        public void OnDrawLayer(Layer l)
        {
            if (l == Layer.Background && broken)
            {
                Graphics.Draw(eddie, 0, 0);
            }
        }
        public override void Update()
        {
            Duck d = Level.First<Duck>();
            if (d != null)
            {
                //is it jif or gif
                if (d.y > 174)
                {
                    Camera cam = Level.current.camera;
                    Level.current.camera.position = Lerp.Vec2Smooth(cam.position, new Vec2(cam.x, d.y - 100), 0.15f);
                    d.right = Maths.Clamp(d.right, 141, 183);
                    d.left = Maths.Clamp(d.left, 141, 183);
                    if (d.y > 600)
                    {
                        Graphics.fade = Lerp.Float(Graphics.fade, -0.6f, 0.01f);
                        if (d.y > 1550)
                        {
                            d.y -= 200;
                            Level.current.camera.position -= new Vec2(0, 200);
                        }
                        if (Graphics.fade <= -0.4f)
                        {
                            Level.current = new DGRDevHall(Level.First<Duck>());
                        }
                    }
                }
            }
            base.Update();
        }
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (platform.imageIndex < 5)
            {//179 148 9 21
             //179 155 22 14
                platform.imageIndex += 1;
                if (platform.imageIndex > 4)
                {
                    Graphics.FlashScreen();
                    Block b = Level.CheckPoint<Block>(134, 148);
                    Level.Remove(b);
                    Block b2 = Level.CheckPoint<Block>(120, 155);
                    Level.Remove(b2);
                    collisionSize = new Vec2(9, 21);
                    _collisionOffset = new Vec2(-28, -9);
                    Level.Add(new InvisibleBlock(179, 148, 9, 21));
                    Level.Add(new InvisibleBlock(180, 155, 22, 14));
                    Level.Add(new InvisibleBlock(118, 155, 22, 14));



                    foreach (SpaceTileset sp in Level.CheckLineAll<SpaceTileset>(new Vec2(-200, 176), new Vec2(138, 176)))
                    {
                        sp.x -= 8;
                        sp.PlaceBlock();
                    }

                    foreach (SpaceTileset sp2 in Level.CheckLineAll<SpaceTileset>(new Vec2(186, 176), new Vec2(500, 176)))
                    {
                        sp2.x += 8;
                        sp2.PlaceBlock();
                    }
                    foreach (SpaceTileset t3 in Level.CheckLineAll<SpaceTileset>(new Vec2(148, 176), new Vec2(176, 176)))
                    {
                        t3.shouldWreck = true;
                        Level.current.AddUpdateOnce(t3);
                    }

                    Level.Add(new TitleCore(x, y) { hSpeed = Rando.Float(-4, 4), vSpeed = Rando.Float(-2, -5) });
                    MultiBeam mb = Level.Nearest<MultiBeam>(x, y);
                    if (mb != null)
                    {
                        for (float xd = mb.left; xd < mb.right; xd += Rando.Float(3, 6))
                        {
                            for (float yd = mb.bottom; yd > mb.top; yd -= Rando.Float(3, 6))
                            {
                                Level.Add(new MultiBeamDebris(xd, yd, Rando.Float(-4, 4), Rando.Float(-4, 2)));
                            }
                        }
                    }
                    broken = true;
                    Level.Remove(mb);
                    SFX.Play("explode");
                    SFX.Play("balloonPop", 1, -3.3f);
                    Level.First<EditorBeam>().y = -10;
                    VersionSign vs = Level.First<VersionSign>();
                    if (vs != null)
                    {
                        vs.center = new Vec2(25.5f, -9);
                        vs.broke = true;
                        vs.x += 25.5f;
                    }
                }//134 148 120 155
                 //192 131
                 //138 176
            }
        }
    }
    [ClientOnly]
    public class TitleCore : PhysicsObject
    {
        public TitleCore(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("coreFly");
            center = new Vec2(6, 9.5f);
            bouncy = 0.4f;
        }
        public override void Update()
        {
            angleDegrees += hSpeed;
            base.Update();
        }
    }
    public class MultiBeamDebris : PhysicsParticle
    {
        public MultiBeamDebris(float xpos, float ypos, float h, float v) : base(xpos, ypos)
        {
            hSpeed = h;
            vSpeed = v;
            _sprite = new SpriteMap("beamParticles", 9, 10, false);
            _sprite.frame = Rando.Int(31);
            graphic = _sprite;
            center = new Vec2(4.5f, 5f);
            _bounceEfficiency = 0.3f;
            alpha = Rando.Float(1, 1.5f);
        }

        public override void Update()
        {
            angleDegrees += hSpeed;
            _sprite.color = new Color(0.3f, Rando.Float(0.3f, 0.5f), Rando.Float(0.5f, 0.8f));
            alpha -= 0.01f;
            if (alpha < 0f)
            {
                Level.Remove(this);
            }
            base.Update();
        }

        private SpriteMap _sprite;
    }
}
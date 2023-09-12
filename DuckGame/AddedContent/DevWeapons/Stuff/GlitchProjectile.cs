using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class GlitchProjectile : Thing
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _chargBinding = new StateBinding("charg");

        public LoopingSound soar;
        public GlitchProjectile(float xpos, float ypos, float charge = 1, Vec2 vs = default(Vec2)) : base(xpos, ypos)
        {
            soar = new LoopingSound("danProjectileLoop");
            soar.lerpVolume = 1;
            velocity = vs;
            collisionSize = new Vec2(20 * charge);
            _collisionOffset = new Vec2(-10 * charge);
            charg = charge;
        }
        public float charg;
        public List<Duck> hit = new List<Duck>();
        public override void Initialize()
        {
            SFX.Play("danShoot", 0.4f * charg, Rando.Float(0.1f));
            base.Initialize();
        }
        public override void Terminate()
        {
            soar.Kill();
            base.Terminate();
        }
        public override void Update()
        {
            IEnumerable<IAmADuck> near = Level.CheckCircleAll<IAmADuck>(position, charg * 70);

            float vol = 0;
            foreach (IAmADuck iaad in near)
            {
                MaterialThing mt = (MaterialThing)iaad;
                if (mt != null && mt.isServerForObject && Duck.GetAssociatedDuck(mt) != null && !Duck.GetAssociatedDuck(mt).dead)
                {
                    float fpl = (position - mt.position).length / 70;
                    if (fpl > vol) vol = fpl;
                }
            }
            soar.lerpVolume = Lerp.Float(soar.lerpVolume, vol + 0.1f, 0.02f);
            soar.Update();

            if (isServerForObject)
            {
                if (x < Level.current.topLeft.x - 600 || x > Level.current.bottomRight.x + 500 ||
                    y < Level.current.topLeft.y - 600 || y > Level.current.bottomRight.y + 500) Level.Remove(this);
                IEnumerable<IAmADuck> iaads = Level.CheckCircleAll<IAmADuck>(position, charg * 10);
                foreach (IAmADuck iaad in iaads)
                {
                    MaterialThing mt = (MaterialThing)iaad;
                    if (mt != null)
                    {
                        Duck d = Duck.GetAssociatedDuck(mt);
                        if (d != null && !d.dead && !hit.Contains(d))
                        {
                            hit.Add(d);
                            SFX.PlaySynchronized("glitch" + Rando.Int(1, 2), 1, Rando.Float(-0.2f, 0.3f));
                            if (d.isServerForObject)
                            {
                                Vec2 v = mt.position;
                                v = Layer.HUD.camera.transformInverse(Layer.Game.camera.transform(v));
                                Level.Add(new FakeCrashPopup(v.x + 8, v.y));
                            }
                            else Send.Message(new NMCrashPopup(d), d.connection);
                        }
                    }
                }
                position += velocity;
            }
            if (Rando.Float(1) < 0.33f * DGRSettings.ActualParticleMultiplier)
            {
                Level.Add(new DanCircParticle(x + Rando.Float(-charg * 10, charg * 10), y + Rando.Float(-charg * 10, charg * 10), charg * 8 * Rando.Float(1, 0.3f)));
            }
        }
        public override void Draw()
        {
            Graphics.DrawCircle(position, charg * 10, Color.Lime, 1, 1);
            for (int i = 0; i <Rando.Int(3, 6); i++)
            {
                Graphics.DrawCircle(position, charg * 10 * Rando.Float(0, 2), Color.Lime * Rando.Float(0.2f), Rando.Float(0.1f, 2f), 1);

            }
        }
    }
    public class DanCircParticle : Thing
    {
        public float charg;
        public DanCircParticle(float xpos, float ypos, float chs) : base(xpos, ypos)
        {
            mlt = 1;
            charg = chs;
            speed = 0.01f;
        }
        public override void Draw()
        {
            Graphics.DrawCircle(position, rCharg * 0.5f, Color.Lime, 1, 1);
        }
        public float rCharg;
        public float mlt;
        public float speed;
        public override void Update()
        {
            if (speed < 0) speed -= 0.01f;
            else speed += 0.01f;
            mlt += speed;
            if (speed > 0.1f) speed *= -1;
            rCharg = charg * mlt;

            if (rCharg < 1) Level.Remove(this);
        }
    }
}

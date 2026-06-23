using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class EnergyForceWave : Thing
    {
        public StateBinding _positionBinding = new InterpolatedVec2Binding("netPosition");
        public StateBinding _offDirBinding = new StateBinding(GhostPriority.High, "_offDir");
        public StateBinding _alphaBinding = new StateBinding(GhostPriority.High, "alpha");
        public StateBinding _waveOwnerBinding = new StateBinding(GhostPriority.High, nameof(_waveOwner));
        private Thing _waveOwner;
        private float _alphaSub;
        private float _speed;
        private float _speedv;
        private List<Thing> _hits = new List<Thing>();

        public EnergyForceWave(
          float xpos,
          float ypos,
          int dir,
          float alphaSub,
          float speed,
          float speedv,
          Duck own)
          : base(xpos, ypos)
        {
            shouldbegraphicculled = false; //theres a glitch that makes these invisible adding this just incase? -Lucky
            offDir = (sbyte)dir;
            graphic = new Sprite("energyHammerWave");
            center = new Vec2(graphic.w, graphic.h);
            _alphaSub = alphaSub;
            _speed = speed;
            _speedv = speedv;
            _collisionSize = new Vec2(6f, 30f);
            _collisionOffset = new Vec2(-3f, -15f);
            graphic.flipH = offDir <= 0;
            _waveOwner = own;
            depth = -0.7f;
        }
        
        public override void Update()
        {
            if (Rando.Float(Math.Abs(hSpeed)) > 3)
            {
                Spark spark = Spark.New(x, y, new Vec2(-hSpeed * 0.2f, Rando.Float(-3f, 3f)));
                spark._color = new Color(178, 220, 239);
                spark._width = 1f;
                Level.Add(spark);
            }
            graphic.flipH = offDir <= 0;
            if (alpha > 0.1f)
            {
                foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(topLeft, bottomRight))
                {
                    if (materialThing is Coin c && !c.used && isServerForObject && c.frames > 6)
                    {
                        SFX.PlaySynchronized("coin");
                        Fondle(c);
                        Duck d = null;
                        if (_waveOwner != null) d = (Duck)_waveOwner;
                        Vec2 v = c.TargetNear(d)[0];

                        HitscanBullet bb = new HitscanBullet(c.x, c.y, v);
                        bb.c = Color.Yellow;
                        IEnumerable<MaterialThing> mts = Level.CheckLineAll<MaterialThing>(c.position, v);
                        foreach (MaterialThing mt in mts)
                        {
                            if (mt == _waveOwner) continue;
                            if (mt is IAmADuck)
                            {
                                SuperFondle(mt, DuckNetwork.localConnection);
                                mt.Destroy(new DTShot(null));
                            }
                            else
                            {
                                Fondle(mt);
                                mt.Hurt(0.1f);
                            }
                        }

                        Level.Add(bb);
                        c.trail.Clear();
                        c.used = false;
                        c.position = v;
                        c.frames = 0;
                        if (c.coinFly != null) c.coinFly.Kill();
                        c.coinFly = null;
                        c.velocity = new Vec2(0, -6);
                        c.gravMultiplier = 0.8f;

                        continue;
                    }
                    if ((materialThing is PhysicsObject || materialThing is Icicles) && !_hits.Contains(materialThing) && materialThing != _waveOwner && materialThing.owner != _waveOwner && Duck.GetAssociatedDuck(materialThing) != _waveOwner)
                    {
                        if (materialThing.owner != null)
                        {
                            if (isServerForObject && !materialThing.isServerForObject)
                                continue;
                        }
                        else if (!isServerForObject)
                            continue;
                        if (_waveOwner != null)
                            Fondle(materialThing, _waveOwner.connection);
                        if (materialThing is Grenade grenade)
                            grenade.PressAction();
                        if (materialThing is PhysicsObject)
                        {
                            materialThing.hSpeed = (float)((_speed - 3) * offDir * 1.5 + offDir * 4) * alpha;
                            materialThing.vSpeed = (_speedv - 4.5f) * alpha;
                            materialThing.clip.Add(_waveOwner as MaterialThing);
                        }
                        if (!materialThing.destroyed && !(materialThing is Equipment))
                            materialThing.Destroy(new DTImpact(this));
                        _hits.Add(materialThing);
                    }
                }
                if (isServerForObject)
                {
                    foreach (Door t in Level.CheckRectAll<Door>(topLeft, bottomRight))
                    {
                        if (_waveOwner != null)
                            Fondle(t, _waveOwner.connection);
                        if (!t.destroyed)
                            t.Destroy(new DTImpact(this));
                    }
                    foreach (Window t in Level.CheckRectAll<Window>(topLeft, bottomRight))
                    {
                        if (_waveOwner != null)
                            Fondle(t, _waveOwner.connection);
                        if (!t.destroyed)
                            t.Destroy(new DTImpact(this));
                    }
                }
            }
            if (!isServerForObject)
                return;
            x += offDir * _speed;
            y += _speedv;
            alpha -= _alphaSub;
            if (alpha > 0)
                return;
            Level.Remove(this);
        }
    }
}

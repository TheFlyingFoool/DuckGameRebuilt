using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class SelectCircle : MaterialThing
    {
        public bool featherfashion;
        public SelectCircle(float xpos, float ypos, bool ff) : base(xpos, ypos)
        {
            featherfashion = ff;
            _selectBeam = new Sprite("SelectCircle", 0f, 0f);
            _selectBeam.alpha = 0.9f;
            _selectBeam.depth = -0.8f;
            _selectBeam.center = new Vec2(16, 16);
            depth = 0f;
            _collisionOffset = new Vec2(-16, -16);
            _collisionSize = new Vec2(32, 32);
            center = new Vec2(16, 16);
            thickness = 10f;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void TakeDuck(Duck d)
        {
            if (!_ducks.Any((BeamDuck t) => t.duck == d) && d._groundValid <= 0)
            {
                float entryHeight;
                if (d.y < 100f)
                {
                    entryHeight = 40f;
                }
                else if (d.y < 150f)
                {
                    entryHeight = 130f;
                }
                else
                {
                    entryHeight = 220f;
                }
                SFX.Play("stepInBeam", 1f, 0f, 0f, false);
                d.beammode = true;
                d.immobilized = true;
                d.crouch = false;
                d.sliding = false;
                if (d.holdObject != null)
                {
                    _guns.Add(d.holdObject);
                }
                d.ThrowItem(true);
                d.solid = false;
                d.grounded = false;
                d.offDir = 1;
                _ducks.Add(new BeamDuck
                {
                    duck = d,
                    entryHeight = entryHeight,
                    leaving = false,
                    entryDir = ((d.x < x) ? -1 : 1),
                    sin = new SinWave(0.03f, 0f),
                    sin2 = new SinWave(0.03f, 1.5707f)
                });
                if (_ducks.Count > 0)
                {
                    int currentIndex = NetworkDebugger.currentIndex;
                }
            }
        }

        public void ClearBeam()
        {
            foreach (BeamDuck beamDuck in _ducks)
            {
                beamDuck.leaving = true;
            }
        }

        public void RemoveDuck(Duck duck)
        {
            foreach (BeamDuck beamDuck in _ducks)
            {
                if (beamDuck.duck == duck)
                {
                    beamDuck.leaving = true;
                }
            }
        }

        public bool entered;
        public override void Update()
        {
            _selectBeam.color = new Color(0.3f, 0.3f + _wave2.normalized * 0.2f, 0.5f + _wave.normalized * 0.3f) * (1f + _flash);
            _flash = Maths.CountDown(_flash, 0.1f, 0f);
            _spawnWait -= 0.1f;
            if (_spawnWait < 0f)
            {
                Level.Add(new SelectCircleParticle(x, y, -0.8f - _wave.normalized, false, Color.Cyan * 0.8f));
                Level.Add(new SelectCircleParticle(x, y, -0.8f - _wave2.normalized, true, Color.LightBlue * 0.8f));
                _spawnWait = 2f;
            }
            waitFrames++;
            if (waitFrames > 5)
            {
                foreach (Duck duck in Level.CheckCircleAll<Duck>(position, 16))
                {
                    TakeDuck(duck);
                }
            }
            int num3 = 0;
            entered = false;
            for (int i = 0; i < _ducks.Count; i++)
            {
                entered = true;
                BeamDuck beamDuck3 = _ducks[i];
                if (beamDuck3.duck == null || beamDuck3.duck.removeFromLevel || !beamDuck3.duck.beammode)
                {
                    _ducks.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (beamDuck3.leaving)
                    {
                        beamDuck3.duck.solid = true;
                        beamDuck3.duck.gravMultiplier = 1f;
                        beamDuck3.duck.immobilized = false;
                        beamDuck3.duck.beammode = false;
                        beamDuck3.duck._groundValid = 30;
                        _ducks.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        beamDuck3.duck.beammode = true;
                        beamDuck3.duck.position = Lerp.Vec2Smooth(beamDuck3.duck.position, position + new Vec2(beamDuck3.sin * 13, beamDuck3.sin2 * 13), 0.1f);
                        beamDuck3.duck.vSpeed = 0f;
                        beamDuck3.duck.hSpeed = 0f;
                        beamDuck3.duck.gravMultiplier = 0f;
                    }
                    if (beamDuck3.duck.inputProfile != null && (beamDuck3.duck.inputProfile.Pressed("CANCEL", false) || beamDuck3.duck.inputProfile.Pressed("LEFT", false) || beamDuck3.duck.inputProfile.Pressed("RIGHT", false)))
                    {
                        beamDuck3.leaving = true;
                    }
                    if (beamDuck3.duck.profile == null)
                    {
                        beamDuck3.leaving = true;
                    }
                    num3++;
                }
            }
            base.Update();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            for (int i = 0; i < 6; i++)
            {
                Level.Add(new GlassParticle(hitPos.x, hitPos.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)), -1));
            }
            _flash = 1f;
            if (bullet != null)
            {
                bullet.hitArmor = true;
            }
            return true;
        }

        public override void Draw()
        {
            base.Draw();
            Graphics.Draw(_selectBeam, x, y);
        }

        private Sprite _selectBeam;

        private float _spawnWait;

        private SinWave _wave = 0.016f;

        private SinWave _wave2 = 0.02f;

        private List<BeamDuck> _ducks = new List<BeamDuck>();

        private List<Thing> _guns = new List<Thing>();

        private float _flash;

        private int waitFrames;
    }
}

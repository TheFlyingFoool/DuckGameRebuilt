using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class RecorderatorBeam : MaterialThing
    {
        private Sprite _selectBeam;
        private float _spawnWait;
        private SinWave _wave = (SinWave)0.012f;
        private SinWave _wave2 = (SinWave)0.025f;
        private List<BeamDuck> _ducks = new List<BeamDuck>();
        private List<Thing> _guns = new List<Thing>();
        private float _beamHeight = 180f;
        private float _flash;
        private bool _leaveLeft;
        public bool entered;

        public RecorderatorBeam(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _selectBeam = new Sprite("selectBeam")
            {
                alpha = 0.9f,
                depth = -0.8f
            };
            _selectBeam.center = new Vec2(_selectBeam.w / 2, 0f);
            depth = (Depth)0.5f;
            _collisionOffset = new Vec2((float)-(_selectBeam.w / 2 * 0.8f), 0f);
            _collisionSize = new Vec2(_selectBeam.w * 0.8f, 80);
            center = new Vec2(_selectBeam.w / 2);
            layer = Layer.Background;
            thickness = 10f;
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (Level.current is TitleScreen ts)
            {
                if (!ts.secondTitlescreen) return;
            }
            _selectBeam.color = new Color(0.5f, 0.2f + _wave2.normalized * 0.2f, 0.3f + _wave.normalized * 0.3f) * (1f + _flash);
            _flash = Maths.CountDown(_flash, 0.1f);
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                _spawnWait -= 0.025f * DGRSettings.ActualParticleMultiplier;
                if (_spawnWait < 0f)
                {
                    Level.Add(new MultiBeamParticle(x, y + 80, -0.8f - _wave.normalized, false, Color.Cyan * 0.8f));
                    Level.Add(new MultiBeamParticle(x, y + 80, -0.8f - _wave2.normalized, true, Color.LightBlue * 0.8f));
                    _spawnWait = 1f;
                }
            }
              
            foreach (Duck duck in Level.CheckRectAll<Duck>(position - center, position - center + new Vec2(_collisionSize.x, _collisionSize.y)))
            {
                Duck d = duck;
                if (!_ducks.Any(t => t.duck == d))
                {
                    float num = d.y >= 100f ? 130f : 40f;
                    SFX.Play("stepInBeam");
                    d.immobilized = true;
                    d.crouch = false;
                    d.sliding = false;
                    if (d.holdObject != null)
                        _guns.Add(d.holdObject);
                    d.ThrowItem();
                    d.solid = false;
                    d.grounded = false;
                    _ducks.Add(new BeamDuck()
                    {
                        duck = d,
                        entryHeight = num,
                        leaving = false,
                        entryDir = d.x < x ? -1 : 1,
                        sin = new SinWave(0.1f),
                        sin2 = new SinWave(0.05f)
                    });
                    entered = true;
                }
            }
            foreach (Holdable holdable in Level.CheckRectAll<Holdable>(position - center, position - center + new Vec2(_collisionSize.x, _collisionSize.y)))
            {
                if (holdable.owner == null && !_guns.Contains(holdable))
                    _guns.Add(holdable);
            }
            int count = _ducks.Count;
            int num1 = 0;
            float num2 = 214;
            float num3 = (float)((_beamHeight - num2 * 2f) / (count > 1 ? count - 1 : 1f));
            for (int index = 0; index < _ducks.Count; ++index)
            {
                BeamDuck duck = _ducks[index];
                if (duck.leaving)
                {
                    duck.duck.solid = true;
                    duck.duck.hSpeed = 4;
                    duck.duck.vSpeed = 0f;
                    if (Math.Abs(duck.duck.position.x - x) > 24f)
                    {
                        duck.duck.immobilized = false;
                        _ducks.RemoveAt(index);
                        --index;
                        continue;
                    }
                }
                else
                {
                    duck.duck.position.x = Lerp.FloatSmooth(duck.duck.position.x, position.x + (float)duck.sin2 * 1f, 0.2f);
                    duck.duck.position.y = Lerp.FloatSmooth(duck.duck.position.y, (float)(num2 + num3 * index + (float)duck.sin * 2f), 0.08f);
                    duck.duck.vSpeed = 0f;
                    duck.duck.hSpeed = 0f;
                }
                if (!TitleScreen.hasMenusOpen && duck.duck.inputProfile.Pressed(Triggers.Right))
                {
                    duck.leaving = true;
                    _leaveLeft = false;
                    duck.duck.offDir = 1;
                    entered = false;
                }
                ++num1;
            }
            for (int index = 0; index < _guns.Count; ++index)
            {
                Thing gun = _guns[index];
                gun.vSpeed = 0f;
                gun.hSpeed = 0f;
                if (Math.Abs(position.x - gun.position.x) < 6f)
                {
                    gun.position = Vec2.Lerp(gun.position, new Vec2(position.x, gun.position.y - 3f), 0.1f);
                    gun.alpha = Maths.LerpTowards(gun.alpha, 0f, 0.1f);
                    if (gun.alpha <= 0f)
                    {
                        gun.y = -200f;
                        _guns.RemoveAt(index);
                        --index;
                    }
                }
                else
                    gun.position = Vec2.Lerp(gun.position, new Vec2(position.x, gun.position.y), 0.2f);
            }
            base.Update();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                    Level.Add(new GlassParticle(hitPos.x, hitPos.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
            }
            _flash = 1f;
            return true;
        }

        public override void Draw()
        {
            base.Draw();
            _selectBeam.depth = depth;
            for (int index = 0; index < 2; ++index)
            {
                Graphics.Draw(_selectBeam, x, y + index * 32 + 12);
            }
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.MultiBeam
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class MultiBeam : MaterialThing
    {
        private Sprite _selectBeam;
        private float _spawnWait;
        private SinWave _wave = (SinWave)0.016f;
        private SinWave _wave2 = (SinWave)0.02f;
        private List<BeamDuck> _ducks = new List<BeamDuck>();
        private List<Thing> _guns = new List<Thing>();
        private float _beamHeight = 180f;
        private float _flash;
        private bool _leaveLeft;
        public bool entered;

        public MultiBeam(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._selectBeam = new Sprite("selectBeam")
            {
                alpha = 0.9f,
                depth = -0.8f
            };
            this._selectBeam.center = new Vec2(this._selectBeam.w / 2, 0f);
            this.depth = (Depth)0.5f;
            this._collisionOffset = new Vec2((float)-(this._selectBeam.w / 2 * 0.8f), 0f);
            this._collisionSize = new Vec2(_selectBeam.w * 0.8f, 180f);
            this.center = new Vec2(this._selectBeam.w / 2);
            this.layer = Layer.Background;
            this.thickness = 10f;
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            this._selectBeam.color = new Color(0.3f, (float)(0.3f + (double)this._wave2.normalized * 0.2f), (float)(0.5 + (double)this._wave.normalized * 0.3f)) * (1f + this._flash);
            this._flash = Maths.CountDown(this._flash, 0.1f);
            this._spawnWait -= 0.025f;
            if (_spawnWait < 0.0)
            {
                Level.Add(new MultiBeamParticle(this.x, this.y + 190f, -0.8f - this._wave.normalized, false, Color.Cyan * 0.8f));
                Level.Add(new MultiBeamParticle(this.x, this.y + 190f, -0.8f - this._wave2.normalized, true, Color.LightBlue * 0.8f));
                this._spawnWait = 1f;
            }
            foreach (Duck duck in Level.CheckRectAll<Duck>(this.position - this.center, this.position - this.center + new Vec2(this._collisionSize.x, this._collisionSize.y)))
            {
                Duck d = duck;
                if (!this._ducks.Any<BeamDuck>(t => t.duck == d))
                {
                    float num = (double)d.y >= 100.0 ? 130f : 40f;
                    SFX.Play("stepInBeam");
                    d.immobilized = true;
                    d.crouch = false;
                    d.sliding = false;
                    if (d.holdObject != null)
                        this._guns.Add(d.holdObject);
                    d.ThrowItem();
                    d.solid = false;
                    d.grounded = false;
                    this._ducks.Add(new BeamDuck()
                    {
                        duck = d,
                        entryHeight = num,
                        leaving = false,
                        entryDir = (double)d.x < (double)this.x ? -1 : 1,
                        sin = new SinWave(0.1f),
                        sin2 = new SinWave(0.05f)
                    });
                    this.entered = true;
                }
            }
            foreach (Holdable holdable in Level.CheckRectAll<Holdable>(this.position - this.center, this.position - this.center + new Vec2(this._collisionSize.x, this._collisionSize.y)))
            {
                if (holdable.owner == null && !this._guns.Contains(holdable))
                    this._guns.Add(holdable);
            }
            int count = this._ducks.Count;
            int num1 = 0;
            float num2 = (float)(_beamHeight / (double)count / 2.0 + 20.0 * (count > 1 ? 1.0 : 0.0));
            float num3 = (float)((_beamHeight - (double)num2 * 2.0) / (count > 1 ? count - 1 : 1.0));
            for (int index = 0; index < this._ducks.Count; ++index)
            {
                BeamDuck duck = this._ducks[index];
                if (duck.leaving)
                {
                    duck.duck.solid = true;
                    duck.duck.hSpeed = this._leaveLeft ? -4f : 4f;
                    duck.duck.vSpeed = 0.0f;
                    if ((double)Math.Abs(duck.duck.position.x - this.x) > 24.0)
                    {
                        duck.duck.immobilized = false;
                        this._ducks.RemoveAt(index);
                        --index;
                        continue;
                    }
                }
                else
                {
                    duck.duck.position.x = Lerp.FloatSmooth(duck.duck.position.x, this.position.x + (float)duck.sin2 * 1f, 0.2f);
                    duck.duck.position.y = Lerp.FloatSmooth(duck.duck.position.y, (float)((double)num2 + (double)num3 * index + (double)(float)duck.sin * 2.0), 0.08f);
                    duck.duck.vSpeed = 0.0f;
                    duck.duck.hSpeed = 0.0f;
                }
                if (duck.duck.inputProfile != null)
                {
                    if (!TitleScreen.hasMenusOpen && duck.duck.inputProfile.Pressed("LEFT"))
                    {
                        duck.leaving = true;
                        this._leaveLeft = true;
                        duck.duck.offDir = -1;
                        this.entered = false;
                    }
                    else if (!TitleScreen.hasMenusOpen && duck.duck.inputProfile.Pressed("RIGHT"))
                    {
                        duck.leaving = true;
                        this._leaveLeft = false;
                        duck.duck.offDir = 1;
                        this.entered = false;
                    }
                }
                ++num1;
            }
            for (int index = 0; index < this._guns.Count; ++index)
            {
                Thing gun = this._guns[index];
                gun.vSpeed = 0.0f;
                gun.hSpeed = 0.0f;
                if ((double)Math.Abs(this.position.x - gun.position.x) < 6.0)
                {
                    gun.position = Vec2.Lerp(gun.position, new Vec2(this.position.x, gun.position.y - 3f), 0.1f);
                    gun.alpha = Maths.LerpTowards(gun.alpha, 0.0f, 0.1f);
                    if ((double)gun.alpha <= 0.0)
                    {
                        gun.y = -200f;
                        this._guns.RemoveAt(index);
                        --index;
                    }
                }
                else
                    gun.position = Vec2.Lerp(gun.position, new Vec2(this.position.x, gun.position.y), 0.2f);
            }
            base.Update();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            for (int index = 0; index < 6; ++index)
                Level.Add(new GlassParticle(hitPos.x, hitPos.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
            this._flash = 1f;
            return true;
        }

        public override void Draw()
        {
            base.Draw();
            this._selectBeam.depth = this.depth;
            for (int index = 0; index < 6; ++index)
                Graphics.Draw(this._selectBeam, this.x, this.y + index * 32);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.TeamBeam
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class TeamBeam : MaterialThing
    {
        private Sprite _selectBeam;
        private float _spawnWait;
        private SinWave _wave = (SinWave)0.016f;
        private SinWave _wave2 = (SinWave)0.02f;
        private List<BeamDuck> _ducks = new List<BeamDuck>();
        private List<Thing> _guns = new List<Thing>();
        private float _beamHeight = 180f;
        private float _flash;
        private int waitFrames;
        private List<Duck> _networkDucks = new List<Duck>();

        public TeamBeam(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._selectBeam = new Sprite("selectBeam")
            {
                alpha = 0.9f,
                depth = -0.8f
            };
            this._selectBeam.center = new Vec2(this._selectBeam.w / 2, 0f);
            this.depth = (Depth)0f;
            this._collisionOffset = new Vec2((float)-(this._selectBeam.w / 2 * 0.800000011920929), 0f);
            this._collisionSize = new Vec2(_selectBeam.w * 0.8f, 180f);
            this.center = new Vec2(this._selectBeam.w / 2);
            this.thickness = 10f;
        }

        public override void Initialize() => base.Initialize();

        public void TakeDuck(Duck d)
        {
            if (this._ducks.Any<BeamDuck>(t => t.duck == d))
                return;
            float num = d.y >= 100.0 ? (d.y >= 150.0 ? 220f : 130f) : 40f;
            SFX.Play("stepInBeam");
            d.beammode = true;
            d.immobilized = true;
            d.crouch = false;
            d.sliding = false;
            if (d.holdObject != null)
                this._guns.Add(d.holdObject);
            d.ThrowItem();
            d.solid = false;
            d.grounded = false;
            d.offDir = 1;
            this._ducks.Add(new BeamDuck()
            {
                duck = d,
                entryHeight = num,
                leaving = false,
                entryDir = d.x < this.x ? -1 : 1,
                sin = new SinWave(0.1f),
                sin2 = new SinWave(0.05f)
            });
            if (this._ducks.Count <= 0)
                return;
            int currentIndex = NetworkDebugger.currentIndex;
        }

        public void ClearBeam()
        {
            foreach (BeamDuck duck in this._ducks)
                duck.leaving = true;
        }

        public void RemoveDuck(Duck duck)
        {
            foreach (BeamDuck duck1 in this._ducks)
            {
                if (duck1.duck == duck)
                    duck1.leaving = true;
            }
        }

        public override void Update()
        {
            if (TeamSelect2.zoomedOut)
            {
                this._beamHeight = 270f;
                this._collisionSize = new Vec2(_selectBeam.w * 0.8f, 270f);
            }
            else
            {
                this._beamHeight = 180f;
                this._collisionSize = new Vec2(_selectBeam.w * 0.8f, 180f);
            }
            this._selectBeam.color = new Color(0.3f, (float)(0.300000011920929 + this._wave2.normalized * 0.200000002980232), (float)(0.5 + this._wave.normalized * 0.300000011920929)) * (1f + this._flash);
            this._flash = Maths.CountDown(this._flash, 0.1f);
            this._spawnWait -= 0.1f;
            if (_spawnWait < 0.0)
            {
                Level.Add(new BeamParticle(this.x, this.y + 290f, -0.8f - this._wave.normalized, false, Color.Cyan * 0.8f));
                Level.Add(new BeamParticle(this.x, this.y + 290f, -0.8f - this._wave2.normalized, true, Color.LightBlue * 0.8f));
                this._spawnWait = 1f;
            }
            ++this.waitFrames;
            if (this.waitFrames > 5)
            {
                foreach (Duck d in Level.CheckRectAll<Duck>(this.topLeft, this.bottomRight))
                {
                    if (d.isServerForObject)
                        this.TakeDuck(d);
                }
            }
            foreach (Holdable holdable in Level.CheckRectAll<Holdable>(this.topLeft, this.bottomRight))
            {
                if (holdable.isServerForObject)
                {
                    if (holdable is RagdollPart)
                    {
                        Duck captureDuck = (holdable as RagdollPart).doll.captureDuck;
                        if (captureDuck != null)
                        {
                            (holdable as RagdollPart).doll.Unragdoll();
                            this.TakeDuck(captureDuck);
                        }
                    }
                    else if (holdable.owner == null && !this._guns.Contains(holdable))
                        this._guns.Add(holdable);
                }
            }
            int val2 = this._ducks.Count;
            if (Network.isActive)
            {
                foreach (BeamDuck duck in this._ducks)
                    duck.floatOrder = 0;
                if (Level.current is TeamSelect2 current)
                {
                    int val1 = 0;
                    foreach (ProfileBox2 profile in current._profiles)
                    {
                        if (profile.ready && profile.duck != null && Math.Abs(profile.duck.x - this.x) < 16.0)
                        {
                            if (profile.duck != null && profile.duck.profile != null)
                            {
                                foreach (BeamDuck duck in this._ducks)
                                {
                                    if (profile.duck.profile.networkIndex < duck.duck.profile.networkIndex)
                                        ++duck.floatOrder;
                                }
                            }
                            ++val1;
                        }
                    }
                    val2 = Math.Max(val1, val2);
                }
            }
            int num1 = 0;
            float num2 = (float)(_beamHeight / val2 / 2.0 + 20.0 * (val2 > 1 ? 1.0 : 0.0));
            float num3 = (float)((_beamHeight - num2 * 2.0) / (val2 > 1 ? val2 - 1 : 1.0));
            for (int index = 0; index < this._ducks.Count; ++index)
            {
                BeamDuck duck = this._ducks[index];
                if (duck.duck == null || duck.duck.removeFromLevel || !duck.duck.beammode)
                {
                    this._ducks.RemoveAt(index);
                    --index;
                }
                else
                {
                    if (duck.leaving)
                    {
                        duck.duck.solid = true;
                        duck.duck.y = MathHelper.Lerp(duck.duck.position.y, duck.entryHeight, 0.35f);
                        duck.duck.vSpeed = 0f;
                        if (Math.Abs(duck.duck.position.y - duck.entryHeight) < 4.0)
                        {
                            duck.duck.position.y = duck.entryHeight;
                            duck.duck.hSpeed = duck.entryDir * 3f;
                            duck.duck.vSpeed = 0f;
                        }
                        if (Math.Abs(duck.duck.position.x - this.x) > 24.0)
                        {
                            duck.duck.gravMultiplier = 1f;
                            duck.duck.immobilized = false;
                            duck.duck.beammode = false;
                            this._ducks.RemoveAt(index);
                            --index;
                            continue;
                        }
                    }
                    else
                    {
                        if (Math.Abs(duck.duck.position.x - this.x) <= 24.0)
                            duck.duck.beammode = true;
                        int num4 = index;
                        if (Network.isActive && duck.duck.profile != null)
                            num4 = duck.floatOrder;
                        duck.duck.position.x = Lerp.FloatSmooth(duck.duck.position.x, this.position.x + (float)duck.sin2 * 1f, 0.4f);
                        duck.duck.position.y = Lerp.FloatSmooth(duck.duck.position.y, (float)(num2 + num3 * num4 + (float)duck.sin * 2.0), 0.1f);
                        duck.duck.vSpeed = 0f;
                        duck.duck.hSpeed = 0f;
                        duck.duck.gravMultiplier = 0f;
                    }
                    if (duck.duck.inputProfile != null && duck.duck.inputProfile.Pressed("CANCEL") && Math.Abs(duck.duck.position.x - this.x) < 2.0)
                        duck.leaving = true;
                    if (duck.duck.profile == null)
                        duck.leaving = true;
                    if (Network.isActive && duck.duck.profile != null && (duck.duck.profile.connection == null || duck.duck.profile.connection.status == ConnectionStatus.Disconnected))
                        duck.leaving = true;
                    ++num1;
                }
            }
            for (int index = 0; index < this._guns.Count; ++index)
            {
                Thing gun = this._guns[index];
                gun.vSpeed = 0f;
                gun.hSpeed = 0f;
                if (Math.Abs(this.position.x - gun.position.x) < 6.0)
                {
                    gun.position = Vec2.Lerp(gun.position, new Vec2(this.position.x, gun.position.y - 3f), 0.1f);
                    gun.alpha = Maths.LerpTowards(gun.alpha, 0f, 0.1f);
                    if (gun.alpha <= 0.0)
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
            if (bullet != null)
                bullet.hitArmor = true;
            return true;
        }

        public override void Draw()
        {
            base.Draw();
            for (int index = 0; index < 10; ++index)
                Graphics.Draw(this._selectBeam, this.x, this.y - 32f + index * 32);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.Bullet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Bullet : Thing
    {
        private new NetworkConnection _connection;
        protected Teleporter _teleporter;
        public AmmoType ammo;
        public bool randomDir;
        public Vec2 start;
        public byte bulletIndex;
        public bool hitArmor;
        private Vec2 _realEnd;
        public Vec2 travelStart;
        public Vec2 travelEnd;
        public Vec2 travel;
        public Vec2 willCol;
        public bool create = true;
        public bool col;
        public bool traced;
        public Thing lastReboundSource;
        public bool rebound;
        protected bool _tracer;
        private bool _tracePhase;
        private bool _gravityAffected;
        public float gravityMultiplier = 1f;
        private float _travelTime;
        protected float _bulletDistance;
        protected float _bulletLength = 100f;
        protected float _bulletSpeed = 28f;
        public Vec2 _actualStart;
        private bool _didPenetrate;
        public Color color = Color.White;
        protected Thing _firedFrom;
        protected Profile _contributeToAccuracy;
        public static bool isRebound = false;
        public float range;
        private PhysicalBullet _physicalBullet;
        public bool trickshot;
        protected int timesRebounded;
        protected int reboundBulletsCreated;
        private Bullet _reboundedBullet;
        public bool reboundCalled;
        public Vec2 travelDirNormalized;
        public bool reboundOnce;
        private int _totalSteps;
        public List<MaterialThing> _currentlyImpacting = new List<MaterialThing>();
        private static List<MaterialThing> bulletImpactList = new List<MaterialThing>();
        private int hitsLogged;
        public Vec2 currentTravel;
        public bool renderedGhost;
        public float _totalLength;
        public Vec2 drawStart;
        protected Vec2 drawEnd;
        protected List<Vec2> prev = new List<Vec2>();
        protected List<float> vel = new List<float>();
        protected float _totalArc;
        protected bool doneTravelling;
        protected float startpoint;
        protected float drawdist;
        protected bool _initializedDraw;
        //private byte networkKillWait = 60;

        public new NetworkConnection connection
        {
            get => this._connection;
            set => this._connection = value;
        }

        public Vec2 end
        {
            get => this._realEnd;
            set => this._realEnd = value;
        }

        public bool gravityAffected
        {
            get => this._gravityAffected;
            set => this._gravityAffected = value;
        }

        public float travelTime => this._travelTime;

        public float bulletDistance => this._bulletDistance;

        public float bulletSpeed => this._bulletSpeed;

        public bool didPenetrate => this._didPenetrate;

        public Thing firedFrom
        {
            get => this._firedFrom;
            set => this._firedFrom = value;
        }

        public Profile contributeToAccuracy
        {
            get => this._contributeToAccuracy;
            set => this._contributeToAccuracy = value;
        }

        public Bullet(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = false,
          bool network = true)
          : base()
        {
            this._gravityAffected = type.affectedByGravity;
            this.gravityMultiplier = type.gravityMultiplier;
            this._bulletLength = type.bulletLength;
            this.depth = -0.1f;
            if (owner is Duck && (Math.Abs((owner as Duck).holdAngle) > 0.1f || (owner as Duck).holdObject is Gun && Math.Abs(((owner as Duck).holdObject as Gun).angleDegrees) > 20f && !this._gravityAffected))
                this.trickshot = true;
            if (!tracer)
            {
                this._tracePhase = true;
                if (owner != null && owner is Duck duck)
                {
                    this._contributeToAccuracy = duck.profile;
                    if (Highlights.highlightRatingMultiplier != 0f)
                        ++duck.profile.stats.bulletsFired;
                }
            }
            this.x = xval;
            this.y = yval;
            this.ammo = type;
            this.rebound = rbound;
            this._owner = owner;
            this.angle = ang;
            this._tracer = tracer;
            this.range = type.range - Rando.Float(type.rangeVariation);
            if (distance > 0f)
                this.range = distance;
            this._bulletSpeed = type.bulletSpeed + Rando.Float(type.speedVariation);
            if (!this.traced)
            {
                if (this.randomDir)
                    this.angle = Rando.Float(360f);
                this.angle += (Rando.Float(30f) - 15f) * (1f - ammo.accuracy);
                this.travel.x = (float)Math.Cos((double)Maths.DegToRad(this.angle)) * this.range;
                this.travel.y = (float)-Math.Sin((double)Maths.DegToRad(this.angle)) * this.range;
                this.start = new Vec2(this.x, this.y);
                this._actualStart = this.start;
                this.end = this.start + this.travel;
                this.travelDirNormalized = this.end - this.start;
                this.travelDirNormalized.Normalize();
                if (this._gravityAffected)
                {
                    this.hSpeed = this.travelDirNormalized.x * this._bulletSpeed;
                    this.vSpeed = this.travelDirNormalized.y * this._bulletSpeed;
                    this._physicalBullet = new PhysicalBullet
                    {
                        bullet = this,
                        weight = this.ammo.weight
                    };
                }
                if (this._tracer)
                {
                    this.TravelBullet();
                }
                else
                {
                    this.travelStart = this.start;
                    this.travelEnd = this.end;
                    this._totalLength = (this.end - this.start).length;
                    this._tracePhase = false;
                }
                this.traced = true;
            }
            if (PewPewLaser.inFire)
                return;
            double x = travelDirNormalized.x;
        }

        public Bullet ReverseTravel()
        {
            ++this.reboundBulletsCreated;
            Vec2 travelDirNormalized = this.travelDirNormalized;
            Vec2 vec2 = new Vec2(-Math.Sign(this.travelDirNormalized.x), 0.0f);
            float dir = Maths.PointDirection(Vec2.Zero, travelDirNormalized - vec2 * 2f * Vec2.Dot(travelDirNormalized, vec2));
            float length = (this._actualStart - this.start).length;
            if ((double)length > 2.0)
            {
                float rng = this._totalLength - length;
                this.Rebound(this.start, dir, rng);
                this.end = this.start;
                this.travelEnd = this.end;
                this.doneTravelling = true;
                this.position = this.start;
                this.drawStart = this.start;
                this.travelDirNormalized = Vec2.Zero;
                this.OnHit(true);
            }
            return this._reboundedBullet;
        }

        public virtual void DoRebound(Vec2 pos, float dir, float rng) => this.Rebound(pos, dir, rng);

        protected virtual void Rebound(Vec2 pos, float dir, float rng)
        {
            ++this.reboundBulletsCreated;
            Bullet bullet = this.ammo.GetBullet(pos.x, pos.y, angle: (-dir), firedFrom: this.firedFrom, distance: rng, tracer: this._tracer);
            bullet._teleporter = this._teleporter;
            bullet.timesRebounded = this.timesRebounded + 1;
            bullet.lastReboundSource = this.lastReboundSource;
            bullet.isLocal = this.isLocal;
            this._reboundedBullet = bullet;
            this.reboundCalled = true;
            Level.Add(bullet);
        }

        public virtual void OnCollide(Vec2 pos, Thing t, bool willBeStopped)
        {
        }

        protected virtual bool RaycastBullet(
          Vec2 p1,
          Vec2 p2,
          Vec2 dir,
          float length,
          List<MaterialThing> collideList)
        {
            int num1 = (int)Math.Ceiling((double)length);
            this.currentTravel = p1;
            Vec2 zero = Vec2.Zero;
            bool willBeStopped = false;
            this.reboundCalled = false;
            do
            {
                Bullet.bulletImpactList.Clear();
                --num1;
                --this._totalSteps;
                Level.current.CollisionBullet(this.currentTravel, Bullet.bulletImpactList);
                if (!this._tracer)
                {
                    for (int index = 0; index < this._currentlyImpacting.Count; ++index)
                    {
                        MaterialThing materialThing = this._currentlyImpacting[index];
                        if (!Bullet.bulletImpactList.Contains(materialThing))
                        {
                            if (this.ammo.deadly)
                                materialThing.DoExitHit(this, this.currentTravel);
                            this._currentlyImpacting.RemoveAt(index);
                            --index;
                        }
                    }
                }
                Duck owner = this._owner as Duck;
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    bool flag1 = index1 == 1;
                    for (int index2 = 0; index2 < Bullet.bulletImpactList.Count; ++index2)
                    {
                        MaterialThing bulletImpact = Bullet.bulletImpactList[index2];
                        if (flag1 == bulletImpact is IAmADuck && (bulletImpact != this._owner && (!(this._owner is Duck) || !(this._owner as Duck).ExtendsTo(bulletImpact)) || this.ammo.immediatelyDeadly) && (owner == null || bulletImpact != owner.holdObject) && bulletImpact != this._teleporter && (!(bulletImpact is Teleporter) || !this._tracer && this.ammo.canTeleport) && (this.ammo.ownerSafety <= 0 || _travelTime / (double)Maths.IncFrameTimer() >= ammo.ownerSafety || this.firedFrom == null || bulletImpact != this.firedFrom.owner))
                        {
                            bool flag2 = false;
                            if (DevConsole.shieldMode && bulletImpact is Duck && (bulletImpact as Duck)._shieldCharge > 0.6f)
                            {
                                flag2 = true;
                                willBeStopped = true;
                            }
                            if (bulletImpact is Duck && !this._tracer && this._contributeToAccuracy != null)
                            {
                                if (Highlights.highlightRatingMultiplier != 0.0)
                                    ++this._contributeToAccuracy.stats.bulletsThatHit;
                                this._contributeToAccuracy = null;
                            }
                            if (!flag2 && bulletImpact.thickness >= 0.0 && !this._currentlyImpacting.Contains(bulletImpact))
                            {
                                if (!this._tracer && !this._tracePhase)
                                {
                                    if (this.ammo.deadly)
                                    {
                                        willBeStopped = bulletImpact.DoHit(this, this.currentTravel);
                                        if (bulletImpact is Duck && (bulletImpact as Duck).dead && !(this.ammo is ATShrapnel) && this.trickshot)
                                        {
                                            ++Global.data.angleShots;
                                            if (this._owner != null && this._owner is Duck && (this._owner as Duck).profile != null)
                                                ++(this._owner as Duck).profile.stats.trickShots;
                                        }
                                    }
                                    else if (this._physicalBullet != null)
                                    {
                                        ImpactedFrom from = currentTravel.y < (double)bulletImpact.top + 1.0 || currentTravel.y > (double)bulletImpact.bottom - 1.0 ? (travelDirNormalized.y > 0.0 ? ImpactedFrom.Top : ImpactedFrom.Bottom) : (travelDirNormalized.x > 0.0 ? ImpactedFrom.Left : ImpactedFrom.Right);
                                        this._physicalBullet.position = this.currentTravel;
                                        this._physicalBullet.velocity = this.velocity;
                                        if (bulletImpact is Block || bulletImpact is IPlatform && travelDirNormalized.y > 0.0)
                                            bulletImpact.SolidImpact(_physicalBullet, from);
                                        else if (bulletImpact.thickness > (double)this.ammo.penetration)
                                            bulletImpact.Impact(_physicalBullet, from, false);
                                        this.velocity = this._physicalBullet.velocity;
                                        willBeStopped = bulletImpact.thickness > (double)this.ammo.penetration;
                                    }
                                    else
                                        willBeStopped = bulletImpact.thickness > (double)this.ammo.penetration;
                                    if (Recorder.currentRecording != null && this.hitsLogged < 1)
                                    {
                                        Recorder.currentRecording.LogAction();
                                        ++this.hitsLogged;
                                    }
                                }
                                else
                                    willBeStopped = bulletImpact.thickness > (double)this.ammo.penetration;
                                this.OnCollide(this.currentTravel, bulletImpact, willBeStopped);
                                this._currentlyImpacting.Add(bulletImpact);
                                if (bulletImpact.thickness > 1.5 && ammo.penetration >= (double)bulletImpact.thickness)
                                {
                                    this._didPenetrate = true;
                                    this.position = this.currentTravel;
                                    if (this.isLocal)
                                        this.OnHit(false);
                                }
                            }
                            bool flag3 = this.reboundCalled;
                            if (willBeStopped)
                            {
                                willBeStopped = true;
                                if (bulletImpact is Teleporter)
                                {
                                    this._teleporter = bulletImpact as Teleporter;
                                    if (this._teleporter.link != null)
                                    {
                                        float rng = this._totalLength - (this._actualStart - this.currentTravel).length;
                                        if ((double)rng > 0.0)
                                        {
                                            float dir1 = Maths.PointDirection(this._actualStart, this.currentTravel);
                                            if ((int)this._teleporter.teleHeight == 2 && (int)this._teleporter._link.teleHeight == 2)
                                            {
                                                Vec2 vec2 = this._teleporter.position - this.currentTravel;
                                                this._teleporter = this._teleporter.link;
                                                this.Rebound(this._teleporter.position - vec2, dir1, rng);
                                            }
                                            else
                                            {
                                                Vec2 currentTravel = this.currentTravel;
                                                if (_teleporter._dir.y == 0.0)
                                                    currentTravel.x = this._teleporter._link.x - (this._teleporter.x - this.currentTravel.x) + this.travelDirNormalized.x;
                                                else if (_teleporter._dir.x == 0.0)
                                                    currentTravel.y = this._teleporter._link.y - (this._teleporter.y - this.currentTravel.y) + this.travelDirNormalized.y;
                                                if ((bool)this._teleporter._link.horizontal)
                                                {
                                                    if (currentTravel.x < (double)this._teleporter._link.left + 2.0)
                                                        currentTravel.x = this._teleporter._link.left + 2f;
                                                    if (currentTravel.x > (double)this._teleporter._link.right - 2.0)
                                                        currentTravel.x = this._teleporter._link.right - 2f;
                                                }
                                                else
                                                {
                                                    if (currentTravel.y < (double)this._teleporter._link.top + 2.0)
                                                        currentTravel.y = this._teleporter._link.top + 2f;
                                                    if (currentTravel.y > (double)this._teleporter._link.bottom - 2.0)
                                                        currentTravel.y = this._teleporter._link.bottom - 2f;
                                                }
                                                this._teleporter = this._teleporter.link;
                                                this.Rebound(currentTravel, dir1, rng);
                                            }
                                        }
                                        flag3 = true;
                                    }
                                }
                                else if (!flag3 && (this.rebound && (!this.ammo.softRebound || bulletImpact.physicsMaterial != PhysicsMaterial.Wood) && bulletImpact is Block || this.reboundOnce))
                                {
                                    float length1 = (this._actualStart - this.currentTravel).length;
                                    if ((double)length1 > 2.0)
                                    {
                                        float rng = this._totalLength - length1;
                                        if ((double)rng > 0.0)
                                        {
                                            Vec2 vec2_1 = Vec2.Zero;
                                            Vec2 pos = this.currentTravel;
                                            Vec2 vec2_2 = this.currentTravel - this.travelDirNormalized;
                                            float num2 = 0.0f;
                                            float num3 = 999.9f;
                                            if (currentTravel.y >= (double)bulletImpact.top && vec2_2.y < (double)bulletImpact.top)
                                            {
                                                num2 = Math.Abs(this.currentTravel.y - vec2_2.y);
                                                if ((double)num2 < (double)num3)
                                                {
                                                    vec2_1 = new Vec2(0.0f, -1f);
                                                    pos = new Vec2(this.currentTravel.x, bulletImpact.top - 1f);
                                                    num3 = num2;
                                                }
                                            }
                                            if (currentTravel.y <= (double)bulletImpact.bottom && vec2_2.y > (double)bulletImpact.bottom)
                                            {
                                                num2 = Math.Abs(this.currentTravel.y - vec2_2.y);
                                                if ((double)num2 < (double)num3)
                                                {
                                                    vec2_1 = new Vec2(0.0f, 1f);
                                                    pos = new Vec2(this.currentTravel.x, bulletImpact.bottom + 1f);
                                                    num3 = num2;
                                                }
                                            }
                                            if (currentTravel.x >= (double)bulletImpact.left && vec2_2.x < (double)bulletImpact.left)
                                            {
                                                num2 = Math.Abs(this.currentTravel.x - vec2_2.x);
                                                if ((double)num2 < (double)num3)
                                                {
                                                    vec2_1 = new Vec2(1f, 0.0f);
                                                    pos = new Vec2(bulletImpact.left - 1f, this.currentTravel.y);
                                                    num3 = num2;
                                                }
                                            }

                                            if (currentTravel.x <= (double)bulletImpact.right && vec2_2.x > (double)bulletImpact.right)
                                            {
                                                num2 = Math.Abs(this.currentTravel.x - vec2_2.x);
                                                if ((double)num2 < (double)num3)
                                                {
                                                    vec2_1 = new Vec2(-1f, 0.0f);
                                                    pos = new Vec2(bulletImpact.right + 1f, this.currentTravel.y);
                                                }
                                            }
                                            if (vec2_1 == Vec2.Zero)
                                            {
                                                vec2_1 = new Vec2(0.0f, -1f);
                                                pos = new Vec2(this.currentTravel.x, bulletImpact.top - 1f);
                                            }
                                            Vec2 travelDirNormalized = this.travelDirNormalized;
                                            Vec2 p2_1 = travelDirNormalized - vec2_1 * 2f * Vec2.Dot(travelDirNormalized, vec2_1);
                                            this.lastReboundSource = bulletImpact;
                                            if (this.reboundOnce)
                                                pos += p2_1.normalized * 3f;
                                            float dir2 = Maths.PointDirection(Vec2.Zero, p2_1);
                                            this.Rebound(pos, dir2, rng);
                                        }
                                        flag3 = true;
                                    }
                                    else
                                        willBeStopped = false;
                                    this.reboundOnce = false;
                                }
                                this.end = this.currentTravel;
                                this.travelEnd = this.end;
                                this.doneTravelling = true;
                                this.position = this.currentTravel;
                                this.OnHit(!flag3);
                                if (this.hitArmor)
                                {
                                    index1 = 1;
                                    break;
                                }
                                break;
                            }
                        }
                    }
                }

                this.currentTravel += this.travelDirNormalized;
            }
            while (!(num1 <= 0 | willBeStopped));
            return willBeStopped;
        }

        protected virtual void OnHit(bool destroyed) => this.ammo.OnHit(destroyed, this);

        protected virtual void CheckTravelPath(Vec2 pStart, Vec2 pEnd)
        {
        }

        private void TravelBullet()
        {
            this.travelDirNormalized = this.end - this.start;
            if (travelDirNormalized.x == double.NaN || travelDirNormalized.y == double.NaN)
            {
                this.travelDirNormalized = Vec2.One;
            }
            else
            {
                float length = this.travelDirNormalized.length;
                if ((double)length <= 1.0 / 1000.0)
                    return;
                this.travelDirNormalized.Normalize();
                this._totalSteps = (int)Math.Ceiling((double)length);
                List<MaterialThing> collideList = new List<MaterialThing>();
                Stack<TravelInfo> travelInfoStack = new Stack<TravelInfo>();
                this.CheckTravelPath(this.start, this.end);
                travelInfoStack.Push(new TravelInfo(this.start, this.end, length));
                int num = 0;
                while (travelInfoStack.Count > 0 && num < 128)
                {
                    ++num;
                    TravelInfo travelInfo = travelInfoStack.Pop();
                    if (Level.current.CollisionLine<MaterialThing>(travelInfo.p1, travelInfo.p2) != null)
                    {
                        if (travelInfo.length < 8.0)
                        {
                            if (this.RaycastBullet(travelInfo.p1, travelInfo.p2, this.travelDirNormalized, travelInfo.length, collideList))
                                break;
                        }
                        else
                        {
                            float len = travelInfo.length * 0.5f;
                            Vec2 vec2 = travelInfo.p1 + this.travelDirNormalized * len;
                            travelInfoStack.Push(new TravelInfo(vec2, travelInfo.p2, len));
                            travelInfoStack.Push(new TravelInfo(travelInfo.p1, vec2, len));
                        }
                    }
                }
            }
        }

        public override void Update()
        {
            if (this._tracer)
                Level.Remove(this);
            if (!this._initializedDraw)
            {
                this.prev.Add(this.start);
                this.vel.Add(0.0f);
                this._initializedDraw = true;
            }
            this._travelTime += Maths.IncFrameTimer();
            this._bulletDistance += this._bulletSpeed;
            this.startpoint = Maths.Clamp(this._bulletDistance - this._bulletLength, 0.0f, 99999f);
            float num = this._bulletDistance;
            if (this._gravityAffected)
            {
                this.end = this.start + this.velocity;
                this.vSpeed += PhysicsObject.gravity * this.gravityMultiplier;
                this.hSpeed *= this.ammo.airFrictionMultiplier;
                if ((double)this.vSpeed > 8.0)
                    this.vSpeed = 8f;
                if (!this.doneTravelling)
                {
                    this.prev.Add(this.end);
                    float length = (this.end - this.start).length;
                    this._totalArc += length;
                    this.vel.Add(length);
                }
            }
            else
                this.end = this.start + this.travelDirNormalized * this._bulletSpeed;
            if (!this.doneTravelling)
            {
                this.TravelBullet();
                this._totalLength = (this.travelStart - this.travelEnd).length;
                if (_bulletDistance >= (double)this._totalLength)
                {
                    this.doneTravelling = true;
                    this.travelEnd = this.end;
                    this._totalLength = (this.travelStart - this.end).length;
                }
                if (this._gravityAffected && this.doneTravelling)
                {
                    this.prev[this.prev.Count - 1] = this.travelEnd;
                    float length = (this.travelEnd - this.start).length;
                    this._totalArc += length;
                    this.vel[this.vel.Count - 1] = length;
                }
            }
            else
            {
                this.alpha -= 0.1f;
                if ((double)this.alpha <= 0.0)
                    Level.Remove(this);
            }
            this.start = this.end;
            if ((double)num > _totalLength)
                num = this._totalLength;
            if (startpoint > (double)num)
                this.startpoint = num;
            this.drawStart = this.travelStart + this.travelDirNormalized * this.startpoint;
            this.drawEnd = this.travelStart + this.travelDirNormalized * num;
            this.drawdist = num;
        }

        public Vec2 GetPointOnArc(float distanceBack)
        {
            float num1 = 0.0f;
            Vec2 pointOnArc = this.prev.Last<Vec2>();
            for (int index = this.prev.Count - 1; index > 0; --index)
            {
                if (index == 0)
                    return this.prev[index];
                float num2 = num1;
                num1 += this.vel[index];
                if ((double)num1 >= (double)distanceBack)
                {
                    if (index == 1)
                        return this.prev[index - 1];
                    float num3 = (distanceBack - num2) / this.vel[index];
                    return this.prev[index] + (this.prev[index - 1] - this.prev[index]) * num3;
                }
                pointOnArc = this.prev[index];
            }
            return pointOnArc;
        }

        public override void Draw()
        {
            if (this._tracer || _bulletDistance <= 0.1f)
                return;
            if (this.gravityAffected)
            {
                if (this.prev.Count < 1)
                    return;
                int num = (int)Math.Ceiling((drawdist - (double)this.startpoint) / 8.0);
                Vec2 p2 = this.prev.Last<Vec2>();
                for (int index = 0; index < num; ++index)
                {
                    Vec2 pointOnArc = this.GetPointOnArc(index * 8);
                    Graphics.DrawLine(pointOnArc, p2, this.color * (float)(1.0 - index / (double)num) * this.alpha, this.ammo.bulletThickness, (Depth)0.9f);
                    if (pointOnArc == this.prev.First<Vec2>())
                        break;
                    p2 = pointOnArc;
                    if (index == 0 && this.ammo.sprite != null && !this.doneTravelling)
                    {
                        this.ammo.sprite.depth = (Depth)1f;
                        this.ammo.sprite.angleDegrees = -Maths.PointDirection(Vec2.Zero, this.travelDirNormalized);
                        Graphics.Draw(this.ammo.sprite, p2.x, p2.y);
                    }
                }
            }
            else
            {
                if (this.ammo.sprite != null && !this.doneTravelling)
                {
                    this.ammo.sprite.depth = this.depth + 10;
                    this.ammo.sprite.angleDegrees = -Maths.PointDirection(Vec2.Zero, this.travelDirNormalized);
                    Graphics.Draw(this.ammo.sprite, this.drawEnd.x, this.drawEnd.y);
                }
                float length = (this.drawStart - this.drawEnd).length;
                float val = 0.0f;
                float num1 = (float)(1.0 / ((double)length / 8.0));
                float num2 = 1f;
                float num3 = 8f;
                while (true)
                {
                    bool flag = false;
                    if ((double)val + (double)num3 > (double)length)
                    {
                        num3 = length - Maths.Clamp(val, 0.0f, 99f);
                        flag = true;
                    }
                    num2 -= num1;
                    --Graphics.currentDrawIndex;
                    Graphics.DrawLine(this.drawStart + this.travelDirNormalized * length - this.travelDirNormalized * val, this.drawStart + this.travelDirNormalized * length - this.travelDirNormalized * (val + num3), this.color * num2, this.ammo.bulletThickness, this.depth);
                    if (!flag)
                        val += 8f;
                    else
                        break;
                }
            }
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.Bullet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public static int bulletcolorindex;
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
        protected Bullet _reboundedBullet;
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
            get => _connection;
            set => _connection = value;
        }

        public Vec2 end
        {
            get => _realEnd;
            set
            {
                _realEnd = value;
            }
        }

        public bool gravityAffected
        {
            get => _gravityAffected;
            set => _gravityAffected = value;
        }

        public float travelTime => _travelTime;

        public float bulletDistance => _bulletDistance;

        public float bulletSpeed => _bulletSpeed;

        public bool didPenetrate => _didPenetrate;

        public Thing firedFrom
        {
            get => _firedFrom;
            set => _firedFrom = value;
        }

        public Profile contributeToAccuracy
        {
            get => _contributeToAccuracy;
            set => _contributeToAccuracy = value;
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
            _gravityAffected = type.affectedByGravity;
            gravityMultiplier = type.gravityMultiplier;
            _bulletLength = type.bulletLength;
            depth = -0.1f;
            if (owner is Duck && (Math.Abs((owner as Duck).holdAngle) > 0.1f || (owner as Duck).holdObject is Gun && Math.Abs(((owner as Duck).holdObject as Gun).angle) > 0.349066f && !_gravityAffected))
                trickshot = true;
            if (!tracer)
            {
                _tracePhase = true;
                if (owner != null && owner is Duck duck)
                {
                    _contributeToAccuracy = duck.profile;
                    if (Highlights.highlightRatingMultiplier != 0f)
                        ++duck.profile.stats.bulletsFired;
                }
            }
            this.x = xval;
            y = yval;
            ammo = type;
            rebound = rbound;
            _owner = owner;
            angle = ang;
            _tracer = tracer;
            range = type.range - Rando.Float(type.rangeVariation);
            if (distance > 0f)
                range = distance;
            _bulletSpeed = type.bulletSpeed + Rando.Float(type.speedVariation);
            if (!traced)
            {
                if (randomDir)
                {
                    angle = Rando.Float(360f);
                }
                angle += (Rando.Float(30f) - 15f) * (1f - ammo.accuracy);
                travel.x = (float)Math.Cos(Maths.DegToRad(angle)) * range;
                travel.y = (float)-Math.Sin(Maths.DegToRad(angle)) * range;
                start = new Vec2(this.x, y);
                _actualStart = start;
                end = start + travel;
                travelDirNormalized = end - start;
                travelDirNormalized.Normalize();
                if (_gravityAffected)
                {
                    hSpeed = travelDirNormalized.x * _bulletSpeed;
                    vSpeed = travelDirNormalized.y * _bulletSpeed;
                    _physicalBullet = new PhysicalBullet
                    {
                        bullet = this,
                        weight = ammo.weight
                    };
                }
                if (_tracer)
                {
                    TravelBullet();
                }
                else
                {
                    travelStart = start;
                    travelEnd = end;
                    _totalLength = (end - start).length;
                    _tracePhase = false;
                }
                traced = true;
            }
            if (PewPewLaser.inFire)
                return;
            double x = travelDirNormalized.x;
        }
        public override void Initialize()
        {
            if (Program.gay)
            {
                color = Colors.Rainbow[bulletcolorindex];
                bulletcolorindex += 1;
                if (bulletcolorindex >= Colors.Rainbow.Length)
                {
                    bulletcolorindex = 0;
                }
            }
            base.Initialize();
        }

        public Bullet ReverseTravel()
        {
            ++reboundBulletsCreated;
            Vec2 travelDirNormalized = this.travelDirNormalized;
            Vec2 vec2 = new Vec2(-Math.Sign(this.travelDirNormalized.x), 0f);
            float dir = Maths.PointDirection(Vec2.Zero, travelDirNormalized - vec2 * 2f * Vec2.Dot(travelDirNormalized, vec2));
            float length = (_actualStart - start).length;
            if (length > 2f)
            {
                float rng = _totalLength - length;
                Rebound(start, dir, rng);
                end = start;
                travelEnd = end;
                doneTravelling = true;
                position = start;
                drawStart = start;
                this.travelDirNormalized = Vec2.Zero;
                OnHit(true);
            }
            return _reboundedBullet;
        }

        public virtual void DoRebound(Vec2 pos, float dir, float rng) => Rebound(pos, dir, rng);

        protected virtual void Rebound(Vec2 pos, float dir, float rng)
        {
            ++reboundBulletsCreated;
            Bullet bullet = ammo.GetBullet(pos.x, pos.y, angle: (-dir), firedFrom: firedFrom, distance: rng, tracer: _tracer);
            bullet._teleporter = _teleporter;
            bullet.timesRebounded = timesRebounded + 1;
            bullet.lastReboundSource = lastReboundSource;
            bullet.isLocal = isLocal;
            _reboundedBullet = bullet;
            reboundCalled = true;
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
            int num1 = (int)Math.Ceiling(length);
            currentTravel = p1;
            Vec2 zero = Vec2.Zero;
            bool willBeStopped = false;
            reboundCalled = false;
            do
            {
                bulletImpactList.Clear();
                --num1;
                --_totalSteps;
                Level.current.CollisionBullet(currentTravel, bulletImpactList);
                if (!_tracer)
                {
                    for (int index = 0; index < _currentlyImpacting.Count; ++index)
                    {
                        MaterialThing materialThing = _currentlyImpacting[index];
                        if (!bulletImpactList.Contains(materialThing))
                        {
                            if (ammo.deadly)
                                materialThing.DoExitHit(this, currentTravel);
                            _currentlyImpacting.RemoveAt(index);
                            --index;
                        }
                    }
                }
                Duck owner = _owner as Duck;
                for (int i = 0; i < 2; ++i)
                {
                    for (int index2 = 0; index2 < bulletImpactList.Count; ++index2)
                    {
                        MaterialThing bulletImpact = bulletImpactList[index2];
                        if (i == 1 == bulletImpact is IAmADuck && (bulletImpact != _owner && (!(_owner is Duck) || !(_owner as Duck).ExtendsTo(bulletImpact)) || ammo.immediatelyDeadly) && (owner == null || bulletImpact != owner.holdObject) && bulletImpact != _teleporter && (!(bulletImpact is Teleporter) || !_tracer && ammo.canTeleport) && (ammo.ownerSafety <= 0 || _travelTime / Maths.IncFrameTimer() >= ammo.ownerSafety || firedFrom == null || bulletImpact != firedFrom.owner))
                        {
                            bool shield = false;
                            if (DevConsole.shieldMode && bulletImpact is Duck && (bulletImpact as Duck)._shieldCharge > 0.6f)
                            {
                                shield = true;
                                willBeStopped = true;
                            }
                            if (bulletImpact is Duck && !_tracer && _contributeToAccuracy != null)
                            {
                                if (Highlights.highlightRatingMultiplier != 0f)
                                    ++_contributeToAccuracy.stats.bulletsThatHit;
                                _contributeToAccuracy = null;
                            }
                            if (!shield && bulletImpact.thickness >= 0f && !_currentlyImpacting.Contains(bulletImpact))
                            {
                                if (!_tracer && !_tracePhase)
                                {
                                    if (ammo.deadly)
                                    {
                                        willBeStopped = bulletImpact.DoHit(this, currentTravel);
                                        if (bulletImpact is Duck && (bulletImpact as Duck).dead && !(ammo is ATShrapnel) && trickshot)
                                        {
                                            ++Global.data.angleShots;
                                            if (_owner != null && _owner is Duck && (_owner as Duck).profile != null)
                                                ++(_owner as Duck).profile.stats.trickShots;
                                        }
                                    }
                                    else if (_physicalBullet != null)
                                    {
                                        ImpactedFrom from = currentTravel.y < bulletImpact.top + 1f || currentTravel.y > bulletImpact.bottom - 1f ? (travelDirNormalized.y > 0f ? ImpactedFrom.Top : ImpactedFrom.Bottom) : (travelDirNormalized.x > 0f ? ImpactedFrom.Left : ImpactedFrom.Right);
                                        _physicalBullet.position = currentTravel;
                                        _physicalBullet.velocity = velocity;
                                        if (bulletImpact is Block || bulletImpact is IPlatform && travelDirNormalized.y > 0f)
                                            bulletImpact.SolidImpact(_physicalBullet, from);
                                        else if (bulletImpact.thickness > ammo.penetration)
                                            bulletImpact.Impact(_physicalBullet, from, false);
                                        velocity = _physicalBullet.velocity;
                                        willBeStopped = bulletImpact.thickness > ammo.penetration;
                                    }
                                    else willBeStopped = bulletImpact.thickness > ammo.penetration;
                                    if (Recorder.currentRecording != null && hitsLogged < 1)
                                    {
                                        Recorder.currentRecording.LogAction();
                                        ++hitsLogged;
                                    }
                                }
                                else willBeStopped = bulletImpact.thickness > ammo.penetration;
                                OnCollide(currentTravel, bulletImpact, willBeStopped);
                                _currentlyImpacting.Add(bulletImpact);
                                if (bulletImpact.thickness > 1.5f && ammo.penetration >= bulletImpact.thickness)
                                {
                                    _didPenetrate = true;
                                    position = currentTravel;
                                    if (isLocal)
                                        OnHit(false);
                                }
                            }
                            bool flag3 = reboundCalled;
                            if (willBeStopped)
                            {
                                willBeStopped = true;
                                if (bulletImpact is Teleporter t)
                                {
                                    WumpTeleporter wt = t as WumpTeleporter;
                                    _teleporter = bulletImpact as Teleporter;
                                    if (_teleporter.link != null)
                                    {
                                        if (wt == null || wt.charge <= 0)
                                        {
                                            if (wt != null)
                                            {
                                                Fondle(wt);
                                                wt.charge = wt.chargeTime;
                                                if (wt._link is WumpTeleporter wtt)
                                                {
                                                    Fondle(wtt);
                                                    wtt.charge = wt.chargeTime;
                                                }
                                            }
                                            float rng = _totalLength - (_actualStart - currentTravel).length;
                                            if (rng > 0f)
                                            {
                                                float dir1 = Maths.PointDirection(_actualStart, currentTravel);
                                                if ((int)_teleporter.teleHeight == 2 && (int)_teleporter._link.teleHeight == 2)
                                                {
                                                    Vec2 vec2 = _teleporter.position - currentTravel;
                                                    _teleporter = _teleporter.link;
                                                    Rebound(_teleporter.position - vec2, dir1, rng);
                                                }
                                                else
                                                {
                                                    Vec2 currentTravel = this.currentTravel;
                                                    if (_teleporter._dir.y == 0f)
                                                        currentTravel.x = _teleporter._link.x - (_teleporter.x - this.currentTravel.x) + travelDirNormalized.x;
                                                    else if (_teleporter._dir.x == 0f)
                                                        currentTravel.y = _teleporter._link.y - (_teleporter.y - this.currentTravel.y) + travelDirNormalized.y;
                                                    if ((bool)_teleporter._link.horizontal)
                                                    {
                                                        if (currentTravel.x < _teleporter._link.left + 2f)
                                                            currentTravel.x = _teleporter._link.left + 2f;
                                                        if (currentTravel.x > _teleporter._link.right - 2f)
                                                            currentTravel.x = _teleporter._link.right - 2f;
                                                    }
                                                    else
                                                    {
                                                        if (currentTravel.y < _teleporter._link.top + 2f)
                                                            currentTravel.y = _teleporter._link.top + 2f;
                                                        if (currentTravel.y > _teleporter._link.bottom - 2f)
                                                            currentTravel.y = _teleporter._link.bottom - 2f;
                                                    }
                                                    _teleporter = _teleporter.link;
                                                    Rebound(currentTravel, dir1, rng);
                                                }
                                            }
                                            flag3 = true;
                                        }
                                        else if (wt.charge > 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                                else if (!flag3 && (rebound && (!ammo.softRebound || bulletImpact.physicsMaterial != PhysicsMaterial.Wood) && bulletImpact is Block || reboundOnce))
                                {
                                    float length1 = (_actualStart - currentTravel).length;
                                    if (length1 > 2f)
                                    {
                                        float rng = _totalLength - length1;
                                        if (rng > 0f)
                                        {
                                            Vec2 vec2_1 = Vec2.Zero;
                                            Vec2 pos = currentTravel;
                                            Vec2 vec2_2 = currentTravel - this.travelDirNormalized;
                                            float num2 = 0f;
                                            float num3 = 999.9f;
                                            if (currentTravel.y >= bulletImpact.top && vec2_2.y < bulletImpact.top)
                                            {
                                                num2 = Math.Abs(currentTravel.y - vec2_2.y);
                                                if (num2 < num3)
                                                {
                                                    vec2_1 = new Vec2(0f, -1f);
                                                    pos = new Vec2(currentTravel.x, bulletImpact.top - 1f);
                                                    num3 = num2;
                                                }
                                            }
                                            if (currentTravel.y <= bulletImpact.bottom && vec2_2.y > bulletImpact.bottom)
                                            {
                                                num2 = Math.Abs(currentTravel.y - vec2_2.y);
                                                if (num2 < num3)
                                                {
                                                    vec2_1 = new Vec2(0f, 1f);
                                                    pos = new Vec2(currentTravel.x, bulletImpact.bottom + 1f);
                                                    num3 = num2;
                                                }
                                            }
                                            if (currentTravel.x >= bulletImpact.left && vec2_2.x < bulletImpact.left)
                                            {
                                                num2 = Math.Abs(currentTravel.x - vec2_2.x);
                                                if (num2 < num3)
                                                {
                                                    vec2_1 = new Vec2(1f, 0f);
                                                    pos = new Vec2(bulletImpact.left - 1f, currentTravel.y);
                                                    num3 = num2;
                                                }
                                            }

                                            if (currentTravel.x <= bulletImpact.right && vec2_2.x > bulletImpact.right)
                                            {
                                                num2 = Math.Abs(currentTravel.x - vec2_2.x);
                                                if (num2 < num3)
                                                {
                                                    vec2_1 = new Vec2(-1f, 0f);
                                                    pos = new Vec2(bulletImpact.right + 1f, currentTravel.y);
                                                }
                                            }
                                            if (vec2_1 == Vec2.Zero)
                                            {
                                                vec2_1 = new Vec2(0f, -1f);
                                                pos = new Vec2(currentTravel.x, bulletImpact.top - 1f);
                                            }
                                            Vec2 travelDirNormalized = this.travelDirNormalized;
                                            Vec2 p2_1 = travelDirNormalized - vec2_1 * 2f * Vec2.Dot(travelDirNormalized, vec2_1);
                                            lastReboundSource = bulletImpact;
                                            if (reboundOnce)
                                                pos += p2_1.normalized * 3f;
                                            float dir2 = Maths.PointDirection(Vec2.Zero, p2_1);
                                            Rebound(pos, dir2, rng);
                                        }
                                        flag3 = true;
                                    }
                                    else
                                        willBeStopped = false;
                                    reboundOnce = false;
                                }
                                end = currentTravel;
                                travelEnd = end;
                                doneTravelling = true;
                                position = currentTravel;
                                OnHit(!flag3);
                                if (hitArmor)
                                {
                                    i = 1;
                                    break;
                                }
                                break;
                            }
                        }
                    }
                }

                currentTravel += travelDirNormalized;
            }
            while (!(num1 <= 0 | willBeStopped));
            return willBeStopped;
        }

        protected virtual void OnHit(bool destroyed) => ammo.OnHit(destroyed, this);

        protected virtual void CheckTravelPath(Vec2 pStart, Vec2 pEnd)
        {
        }

        private void TravelBullet()
        {
            travelDirNormalized = end - start;
            if (travelDirNormalized.x == double.NaN || travelDirNormalized.y == double.NaN)
            {
                travelDirNormalized = Vec2.One;
            }
            else
            {
                float length = travelDirNormalized.length;
                if (length <= 1f / 1000f)
                    return;
                travelDirNormalized.Normalize();
                _totalSteps = (int)Math.Ceiling(length);
                List<MaterialThing> collideList = new List<MaterialThing>();
                Stack<TravelInfo> travelInfoStack = new Stack<TravelInfo>();
                CheckTravelPath(start, end);
                travelInfoStack.Push(new TravelInfo(start, end, length));
                int num = 0;
                while (travelInfoStack.Count > 0 && num < 128)
                {
                    ++num;
                    TravelInfo travelInfo = travelInfoStack.Pop();
                    if (Level.current.CollisionLine<MaterialThing>(travelInfo.p1, travelInfo.p2) != null)
                    {
                        if (travelInfo.length < 8f)
                        {
                            if (RaycastBullet(travelInfo.p1, travelInfo.p2, travelDirNormalized, travelInfo.length, collideList))
                                break;
                        }
                        else
                        {
                            float len = travelInfo.length * 0.5f;
                            Vec2 vec2 = travelInfo.p1 + travelDirNormalized * len;
                            travelInfoStack.Push(new TravelInfo(vec2, travelInfo.p2, len));
                            travelInfoStack.Push(new TravelInfo(travelInfo.p1, vec2, len));
                        }
                    }
                }
            }
        }

        public override void Update()
        {
            if (_tracer)
                Level.Remove(this);
            if (!_initializedDraw)
            {
                prev.Add(start);
                vel.Add(0f);
                _initializedDraw = true;
            }
            _travelTime += Maths.IncFrameTimer();
            _bulletDistance += _bulletSpeed;
            startpoint = Maths.Clamp(_bulletDistance - _bulletLength, 0f, 99999f);
            float num = _bulletDistance;
            if (_gravityAffected)
            {
                end = start + velocity;
                vSpeed += PhysicsObject.gravity * gravityMultiplier;
                hSpeed *= ammo.airFrictionMultiplier;
                if (vSpeed > 8f)
                    vSpeed = 8f;
                if (!doneTravelling)
                {
                    prev.Add(end);
                    float length = (end - start).length;
                    _totalArc += length;
                    vel.Add(length);
                }
            }
            else
                end = start + travelDirNormalized * _bulletSpeed;
            if (!doneTravelling)
            {
                TravelBullet();
                _totalLength = (travelStart - travelEnd).length;
                if (_bulletDistance >= _totalLength)
                {
                    doneTravelling = true;
                    travelEnd = end;
                    _totalLength = (travelStart - end).length;
                }
                if (_gravityAffected && doneTravelling)
                {
                    prev[prev.Count - 1] = travelEnd;
                    float length = (travelEnd - start).length;
                    _totalArc += length;
                    vel[vel.Count - 1] = length;
                }
            }
            else
            {
                alpha -= 0.1f;
                if (alpha <= 0f)
                    Level.Remove(this);
            }
            start = end;
            if (num > _totalLength)
                num = _totalLength;
            if (startpoint > num)
                startpoint = num;
            drawStart = travelStart + travelDirNormalized * startpoint;
            drawEnd = travelStart + travelDirNormalized * num;
            drawdist = num;
        }

        public Vec2 GetPointOnArc(float distanceBack)
        {
            float num1 = 0f;
            Vec2 pointOnArc = prev.Last();
            for (int index = prev.Count - 1; index > 0; --index)
            {
                if (index == 0)
                    return prev[index];
                float num2 = num1;
                num1 += vel[index];
                if (num1 >= distanceBack)
                {
                    if (index == 1)
                        return prev[index - 1];
                    float num3 = (distanceBack - num2) / vel[index];
                    return prev[index] + (prev[index - 1] - prev[index]) * num3;
                }
                pointOnArc = prev[index];
            }
            return pointOnArc;
        }

        public override void Draw()
        {
            if (_tracer || _bulletDistance <= 0.1f)
                return;
            if (gravityAffected)
            {
                if (prev.Count < 1)
                    return;

                //Fixed some shit here no touchy as for it is very fragile <3
                //-NiK0
                float num = (int)Math.Ceiling((drawdist - startpoint) / 8f);
                Vec2 p2 = prev.Last();
                for (int index = 0; index < num; ++index)
                {
                    Vec2 pointOnArc = GetPointOnArc(index * 8);

                    Graphics.DrawLine(pointOnArc, p2, color * (1f - index / num) * alpha, ammo.bulletThickness, (Depth)0.9f);
                    if (pointOnArc == prev[0])
                        break;
                    p2 = pointOnArc;
                    if (index == 0 && ammo.sprite != null && !doneTravelling)
                    {
                        ammo.sprite.depth = (Depth)1f;

                        //very slight optimization here, before it was setting ammo.sprite.angleDegrees 
                        //to Maths.PointDirection making it do extra operations when it
                        //could just be setting the angle direction skipping two operations 
                        //-NiK0
                        ammo.sprite.angle = -Maths.PointDirectionRad(Vec2.Zero, travelDirNormalized);
                        Graphics.Draw(ammo.sprite, p2.x, p2.y);
                    }
                }
            }
            else
            {
                if (ammo.sprite != null && !doneTravelling)
                {
                    //same optimization here
                    ammo.sprite.depth = depth + 10;
                    ammo.sprite.angle = -Maths.PointDirectionRad(Vec2.Zero, travelDirNormalized);
                    Graphics.Draw(ammo.sprite, drawEnd.x, drawEnd.y);
                }
                float length = (drawStart - drawEnd).length;
                float val = 0f;
                float num1 = (1f / (length / 8f));
                float num2 = 1f;
                float num3 = 8f;
                while (true)
                {
                    bool flag = false;
                    if (val + num3 > length)
                    {
                        num3 = length - Maths.Clamp(val, 0f, 99f);
                        flag = true;
                    }
                    num2 -= num1;
                    --Graphics.currentDrawIndex;
                    Graphics.DrawLine(drawStart + travelDirNormalized * length - travelDirNormalized * val, drawStart + travelDirNormalized * length - travelDirNormalized * (val + num3), color * num2, ammo.bulletThickness, depth);
                    if (!flag)
                        val += 8f;
                    else
                        break;
                }
            }
        }
    }
}

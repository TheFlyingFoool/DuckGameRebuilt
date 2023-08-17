using Microsoft.Xna.Framework.Design;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DuckGame
{
    [ClientOnly]
    public class WarpSword : Gun
    {
        public StateBinding _heatBinding = new StateBinding("heat");
        public StateBinding _holdBinding = new StateBinding("_hold");
        public StateBinding _TPForwardBinding = new StateBinding("TPForward");
        public StateBinding _drawnPowerFromBinding = new StateBinding("drawnPowerFrom");
        public WarpSword(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("wagsword");
            center = new Vec2(8.5f, 18.5f);//17 37

            ammo = 8;
            _ammoType = new ATLaser();

            physicsMaterial = PhysicsMaterial.Metal;

            _holdOffset = new Vec2(-6, 4.5f);
        }

        private bool _drawing;
        public override float angle
        {
            get => _drawing ? _angle : base.angle + _hold * offDir;
            set => _angle = value;
        }
        public float _hold;
        public float _throwSpin;

        public List<Vec2> oldPos = new List<Vec2>();
        public List<float> oldAng = new List<float>();

        public int charges = 2;

        public MaterialThing drawnPowerFrom;
        public override void Update()
        {
            base.Update();


            if (isServerForObject)
            {
                float rang = 50;
                if (owner != null) rang = 30;
                IPlatform plt = Level.CheckCircle<IPlatform>(position, rang);
                if (plt is Nubber n)
                {
                    plt = Level.CheckCircle<IPlatform>(position, rang, n);
                }
                if (plt != null && plt is MaterialThing mt)
                {
                    if (charges == 0)
                    {
                        if (mt is BlockGroup bg)
                        {
                            float dis = 123123123;
                            MaterialThing end = mt;
                            for (int i = 0; i < bg.blocks.Count; i++)
                            {
                                Block b = bg.blocks[i];
                                float farg = (position - b.position).length;
                                if (farg < dis)
                                {
                                    dis = farg;
                                    end = b;
                                }
                            }

                            mt = end;
                        }
                        mult = Rando.Float(1, 1.2f);
                        drawnPowerFrom = mt;
                        SFX.PlaySynchronized("laserChargeTeeny", 0.8f, 0.3f);
                    }
                    charges = 2;
                }
            }

            if (duck != null)
            {
                center = new Vec2(8.5f, 37f);
                collisionSize = new Vec2(4);
                _collisionOffset = new Vec2(0, 0);

                if (duck.inputProfile.Down("DOWN"))
                {
                    _hold = Lerp.Float(_hold, 0, 0.3f);
                }
                else
                {
                    if (TPForward && owner != null)
                    {
                        _hold = Lerp.FloatSmooth(_hold, 1.57f, 0.3f);
                        if (_hold < 1.5f)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Level.Add(WagnusChargeParticle.New(x + Rando.Float(-6, 6), y + Rando.Float(-12, 6), this));

                            }
                            Vec2 diff = owner.position;
                            if (raised)
                            {
                                heat += 0.02f;
                                for (int i = 0; i < 12; i++)
                                { //jank but it'll do
                                    owner.y -= 2;
                                    ReturnItemToWorld(owner);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 12; i++)
                                { //jank but it'll do
                                    owner.x += 2 * offDir;
                                    ReturnItemToWorld(owner);
                                }
                            }
                            diff -= owner.position;
                            if (isServerForObject) owner.vSpeed += 0.1f;

                            oldPos.Add(position);
                            oldAng.Add(angle);
                            position -= diff / 2;
                            oldPos.Add(position);
                            oldAng.Add(angle);
                            position -= diff / 2;
                            if (isServerForObject)
                            {
                                Vec2 vec2_1 = barrelPosition + barrelVector * 3f;
                                QuadLaserBullet quadLaserBullet = Level.CheckRect<QuadLaserBullet>(new Vec2(position.x < vec2_1.x ? position.x : vec2_1.x, position.y < vec2_1.y ? position.y : vec2_1.y), new Vec2(position.x > vec2_1.x ? position.x : vec2_1.x, position.y > vec2_1.y ? position.y : vec2_1.y));
                                if (quadLaserBullet != null)
                                {
                                    SFX.Play("swordClash", Rando.Float(0.6f, 0.7f), Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f));
                                    Fondle(quadLaserBullet);
                                    quadLaserBullet.safeFrames = 8;
                                    quadLaserBullet.safeDuck = duck;
                                    Vec2 vec2_2 = quadLaserBullet.travel;
                                    float length = vec2_2.length;
                                    float num = 1.5f;
                                    vec2_2 = offDir <= 0 ? new Vec2(-length * num, 0f) : new Vec2(length * num, 0f);
                                    quadLaserBullet.travel = vec2_2;
                                    quadLaserBullet.position -= diff;
                                }
                            }
                        }
                        else if (isServerForObject)
                        {
                            Vec2 vec2_1 = barrelPosition + barrelVector * 3f;
                            QuadLaserBullet quadLaserBullet = Level.CheckRect<QuadLaserBullet>(new Vec2(position.x < vec2_1.x ? position.x : vec2_1.x, position.y < vec2_1.y ? position.y : vec2_1.y), new Vec2(position.x > vec2_1.x ? position.x : vec2_1.x, position.y > vec2_1.y ? position.y : vec2_1.y));
                            if (quadLaserBullet != null)
                            {
                                SFX.Play("swordClash", Rando.Float(0.6f, 0.7f), Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f));
                                Fondle(quadLaserBullet);
                                quadLaserBullet.safeFrames = 8;
                                quadLaserBullet.safeDuck = duck;
                                Vec2 vec2_2 = quadLaserBullet.travel;
                                float length = vec2_2.length;
                                float num = 1.5f;
                                vec2_2 = offDir <= 0 ? new Vec2(-length * num, 0f) : new Vec2(length * num, 0f);
                                quadLaserBullet.travel = vec2_2;
                            }
                        }
                        if (!duck.inputProfile.Down("SHOOT") && isServerForObject)
                        {
                            TPForward = false;
                        }

                        if (isServerForObject)
                        {
                            foreach (IAmADuck iaad in Level.CheckLineAll<IAmADuck>(Offset(new Vec2(1.5f, -7)), Offset(new Vec2(1.5f, -30))))
                            {
                                if (iaad != duck && iaad is MaterialThing materialThing)
                                {
                                    bool flag = materialThing != prevOwner;
                                    if (flag && materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck == prevOwner) flag = false;
                                    if (flag)
                                    {
                                        materialThing.Destroy(new DTImpale(this));
                                        if (Recorder.currentRecording != null)
                                            Recorder.currentRecording.LogBonus();
                                    }
                                }
                            }
                        }
                    }
                    else _hold = Lerp.Float(_hold, -0.4f, 0.3f);
                }
            }
            else
            {
                center = new Vec2(8.5f, 18.5f);//17 37
                collisionSize = new Vec2(10, 4);
                _collisionOffset = new Vec2(-5, -0.5f);
                _hold = 0;

                bool flag1 = false;
                bool flag2 = false;
                if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 2.0f || !grounded)
                {
                    if (!grounded && Level.CheckRect<Block>(position + new Vec2(-6f, -6f), position + new Vec2(6f, -2f)) != null)
                    {
                        flag2 = true;
                    }
                    if (!flag2 && !_grounded && !initemspawner && (Level.CheckPoint<IPlatform>(position + new Vec2(0f, 8f)) == null || vSpeed < 0.0f))
                    {
                        PerformAirSpin();
                        flag1 = true;
                    }
                }
                if (!flag1 | flag2)
                {
                    _throwSpin %= 360f;
                    if (flag2)
                        _throwSpin = Math.Abs(_throwSpin - 90f) >= Math.Abs(_throwSpin + 90f) ? Lerp.Float(-90f, 0f, 16f) : Lerp.Float(_throwSpin, 90f, 16f);
                    else if (_throwSpin > 90.0f && _throwSpin < 270.0f)
                    {
                        _throwSpin = Lerp.Float(_throwSpin, 180f, 14f);
                    }
                    else
                    {
                        if (_throwSpin > 180.0f)
                            _throwSpin -= 360f;
                        else if (_throwSpin < -180.0f)
                            _throwSpin += 360f;
                        _throwSpin = Lerp.Float(_throwSpin, 0f, 14f);
                    }
                }
                angleDegrees = _throwSpin + 90;
            }
        }
        public void PerformAirSpin()
        {
            if (hSpeed > 0f)
            {
                _throwSpin += (Math.Abs(hSpeed) + Math.Abs(vSpeed)) * 2f + 4f;
                return;
            }
            _throwSpin -= (Math.Abs(hSpeed) + Math.Abs(vSpeed)) * 2f + 4f;
        }
        public override void Fire()
        {
        }
        public bool TPForward;
        public override void OnPressAction()
        {
            if (_hold < 0 && charges > 0 && isServerForObject)
            {
                charges--;
                if (charges == 0)
                {
                    SFX.PlaySynchronized("laserUnchargeShortLoud", pitch: 0.7f);
                }
                if (heat < 1.5f)
                {
                    heat += 0.1f;
                }
                SFX.PlaySynchronized("swipe", 1, Rando.Float(0.2f, 0.4f));
                SFX.PlaySynchronized("warpgun", 1, Rando.Float(0.6f, 0.8f));
                TPForward = true;
            }
        }
        public override void OnHoldAction()
        {
        }
        public override void OnReleaseAction()
        {
        }
        public int del;
        public float mult;
        public override void Draw()
        {
            if (drawnPowerFrom != null)
            {
                mult = Lerp.Float(mult, 0, 0.03f);

                Material m = Graphics.material;
                Graphics.material = null;
                for (int x = 0; x < 3; x++)
                {
                    int numLines = Rando.Int(3, 5);

                    Vec2 currentPos = position;

                    float dis = (currentPos - drawnPowerFrom.position).length / numLines;

                    Vec2 vec = (currentPos - drawnPowerFrom.position).normalized.Rotate(1.57f, Vec2.Zero);

                    for (int i = 0; i < numLines; i++)
                    {
                        Vec2 ti = Lerp.Vec2(currentPos, drawnPowerFrom.position, dis);

                        if (i < numLines - 1)
                        {
                            ti += vec * Rando.Float(-12, 12) * (mult + 0.1f);

                        }
                        Graphics.DrawLine(currentPos, ti, new Color(147, 64, 221) * 0.7f, 1, depth - 1);
                        currentPos = ti;
                    }
                }
                Graphics.material = null;
                if (mult <= 0) drawnPowerFrom = null;
            }
            if (DevConsole.showCollision)
            {
                Graphics.DrawLine(Offset(new Vec2(1.5f, -7)), Offset(new Vec2(1.5f, -30)), Color.Red, 1, 1);
            }
            if (oldPos.Count > 0)
            {
                _drawing = true;
                Vec2 p = position;
                float a = angle;
                Color c = graphic.color;
                float ap = alpha;
                for (int i = oldPos.Count - 1; i > 0; i--)
                {
                    alpha -= 0.05f;
                    if (alpha <= 0)
                    {
                        break;
                    }
                    position = oldPos[i];
                    angle = oldAng[i];
                    graphic.color = Color.Purple;
                    base.Draw();
                }

                alpha = ap;
                position = p;
                angle = a;
                graphic.color = c;

                _drawing = false;

                oldPos.RemoveAt(0);
                oldAng.RemoveAt(0);
            }
            
            base.Draw();
        }
    }
}

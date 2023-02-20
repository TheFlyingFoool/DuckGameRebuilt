// Decompiled with JetBrains decompiler
// Type: DuckGame.ControlWave
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ControlWave : Thing, ITeleport
    {
        private MindControlRay _owner;
        private float _fade = 1f;
        private bool _isNotControlRay;
        private bool _isLocalWave = true;

        public ControlWave(float xpos, float ypos, float dir, MindControlRay owner, bool local = true)
          : base(xpos, ypos)
        {
            _owner = owner;
            graphic = new Sprite("controlWave")
            {
                flipH = offDir < 0
            };
            center = new Vec2(8f, 8f);
            xscale = yscale = 0.2f;
            angle = dir;
            _isLocalWave = local;
        }

        public override void Update()
        {
            if (isServerForObject)
            {
                xscale = yscale = Maths.CountUp(yscale, 0.05f);
                _fade -= 0.05f;
                if (_fade < 0.0)
                    Level.Remove(this);
                alpha = Maths.NormalizeSection(_fade, 0.2f, 0.3f);
                Vec2 p2 = Vec2.Zero;
                if (_owner.controlledDuck == null && !_isNotControlRay)
                {
                    p2 = new Vec2((float)Math.Cos(angle), (float)-Math.Sin(angle));
                    if (_isLocalWave)
                    {
                        foreach (IAmADuck amAduck in Level.CheckCircleAll<IAmADuck>(position, 3f))
                        {
                            Duck d = amAduck as Duck;
                            switch (amAduck)
                            {
                                case RagdollPart _ when (amAduck as RagdollPart).doll.captureDuck != null:
                                    d = (amAduck as RagdollPart).doll.captureDuck;
                                    break;
                                case TrappedDuck _ when (amAduck as TrappedDuck).captureDuck != null:
                                    d = (amAduck as TrappedDuck).captureDuck;
                                    break;
                            }
                            if (d != null && d.mindControl == null && !d.HasEquipment(typeof(TinfoilHat)) && !(d.holdObject is MindControlRay))
                                _owner.ControlDuck(d);
                        }
                    }
                }
                else
                {
                    if (_owner.controlledDuck != null)
                    {
                        p2 = _owner.controlledDuck.cameraPosition - position;
                        p2.Normalize();
                        angleDegrees = -Maths.PointDirection(Vec2.Zero, p2);
                    }
                    _isNotControlRay = true;
                }
                position += p2 * 2.6f;
            }
            else
            {
                xscale = yscale = 1f;
                position += new Vec2((float)Math.Cos(angle), (float)-Math.Sin(angle)) * 2.6f;
            }
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.SmallFire
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class SmallFire : PhysicsParticle, ITeleport
    {
        public static int kMaxObjects = 256;
        public float waitToHurt;
        public Duck whoWait;
        private SpriteMap _sprite;
        private SpriteMap _airFire;
        private float _airFireScale;
        private float _spinSpeed;
        private bool _multiplied;
        private byte _groundLife = 125;
        private Vec2 _stickOffset;
        private MaterialThing _stick;
        private Thing _firedFrom;
        private static bool kAlternate = false;
        private bool _alternate;
        private bool _alternateb;
        public bool doFloat;
        private int _fireID;
        private bool _canMultiply = true;
        private bool didRemove;

        public static SmallFire New(
          float xpos,
          float ypos,
          float hspeed,
          float vspeed,
          bool shortLife = false,
          MaterialThing stick = null,
          bool canMultiply = true,
          Thing firedFrom = null,
          bool network = false)
        {
            SmallFire p;
            if (Network.isActive)
                p = new SmallFire();
            else if (Level.core.firePool[Level.core.firePoolIndex] == null)
            {
                p = new SmallFire();
                Level.core.firePool[Level.core.firePoolIndex] = p;
            }
            else
                p = Level.core.firePool[Level.core.firePoolIndex];
            Level.core.firePoolIndex = (Level.core.firePoolIndex + 1) % SmallFire.kMaxObjects;
            if (p != null)
            {
                p.ResetProperties();
                p.Init(xpos, ypos, hspeed, vspeed, shortLife, stick, canMultiply);
                p._sprite.globalIndex = Thing.GetGlobalIndex();
                p._airFire.globalIndex = Thing.GetGlobalIndex();
                p._firedFrom = firedFrom;
                p.needsSynchronization = true;
                p.isLocal = !network;
                if (Network.isActive && !network)
                    GhostManager.context.particleManager.AddLocalParticle(p);
                if (float.IsNaN(p.position.x) || float.IsNaN(p.position.y))
                {
                    if (p.stick != null)
                    {
                        p.position.x = 0f;
                        p.position.y = 0f;
                    }
                    else
                    {
                        p.position.x = Vec2.NetMin.x;
                        p.position.y = Vec2.NetMin.y;
                    }
                }
            }
            return p;
        }

        public override void NetSerialize(BitBuffer b)
        {
            if (stick != null && stick.ghostObject != null)
            {
                b.Write(true);
                b.Write((ushort)(int)stick.ghostObject.ghostObjectIndex);
                b.Write((sbyte)stickOffset.x);
                b.Write((sbyte)stickOffset.y);
            }
            else
            {
                b.Write(false);
                b.Write((short)x);
                b.Write((short)y);
            }
        }

        public override void NetDeserialize(BitBuffer d)
        {
            if (d.ReadBool())
            {
                GhostObject ghost = GhostManager.context.GetGhost((NetIndex16)d.ReadUShort());
                if (ghost != null && ghost.thing != null)
                    stick = ghost.thing as MaterialThing;
                stickOffset = new Vec2(d.ReadSByte(), d.ReadSByte());
                UpdateStick();
                hSpeed = 0f;
                vSpeed = 0f;
            }
            else
                netLerpPosition = new Vec2(d.ReadShort(), d.ReadShort());
        }

        public byte groundLife
        {
            get => _groundLife;
            set => _groundLife = value;
        }

        public Vec2 stickOffset
        {
            get => _stickOffset;
            set => _stickOffset = value;
        }

        public MaterialThing stick
        {
            get => _stick;
            set => _stick = value;
        }

        public Thing firedFrom => _firedFrom;

        public int fireID => _fireID;

        private SmallFire()
          : base(0f, 0f)
        {
            _bounceEfficiency = 0.2f;
            _sprite = new SpriteMap("smallFire", 16, 16);
            _sprite.AddAnimation("burn", (float)(0.2f + Rando.Float(0.2f)), true, 0, 1, 2, 3, 4);
            graphic = _sprite;
            center = new Vec2(8f, 14f);
            _airFire = new SpriteMap("airFire", 16, 16);
            _airFire.AddAnimation("burn", (float)(0.2f + Rando.Float(0.2f)), true, 0, 1, 2, 1);
            _airFire.center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(12f, 12f);
            _collisionOffset = new Vec2(-6f, -6f);
        }

        private void Init(
          float xpos,
          float ypos,
          float hspeed,
          float vspeed,
          bool shortLife = false,
          MaterialThing stick = null,
          bool canMultiply = true)
        {
            if (xpos == 0.0 && ypos == 0.0 && stick == null)
            {
                xpos = Vec2.NetMin.x;
                ypos = Vec2.NetMin.y;
            }
            position.x = xpos;
            position.y = ypos;
            _airFireScale = 0f;
            _multiplied = false;
            _groundLife = 125;
            doFloat = false;
            hSpeed = hspeed;
            vSpeed = vspeed;
            _sprite.SetAnimation("burn");
            _sprite.imageIndex = Rando.Int(4);
            xscale = yscale = 0.8f + Rando.Float(0.6f);
            angleDegrees = Rando.Float(20f) - 10f;
            _airFire.SetAnimation("burn");
            _airFire.imageIndex = Rando.Int(2);
            _airFire.xscale = _airFire.yscale = 0f;
            _spinSpeed = 0.1f + Rando.Float(0.1f);
            _airFire.color = Color.Orange * (0.8f + Rando.Float(0.2f));
            _gravMult = 0.7f;
            _sticky = 0.6f;
            _life = 100f;
            if (Network.isActive)
                _sticky = 0f;
            _fireID = FireManager.GetFireID();
            needsSynchronization = true;
            if (shortLife)
                _groundLife = 31;
            depth = (Depth)0.6f;
            _stick = stick;
            _stickOffset = new Vec2(xpos, ypos);
            UpdateStick();
            _alternate = SmallFire.kAlternate;
            SmallFire.kAlternate = !SmallFire.kAlternate;
            _canMultiply = canMultiply;
        }

        public void UpdateStick()
        {
            if (_stick == null)
                return;
            position = _stick.Offset(_stickOffset);
        }

        public void SuckLife(float l) => _life -= l;

        public override void Removed()
        {
            if (Network.isActive && !didRemove && isLocal && GhostManager.context != null)
            {
                didRemove = true;
                GhostManager.context.particleManager.RemoveParticle(this);
            }
            base.Removed();
        }

        public override void Update()
        {
            if (waitToHurt > 0.0)
                waitToHurt -= Maths.IncFrameTimer();
            else
                whoWait = null;
            if (!isLocal)
            {
                if (_stick != null)
                    UpdateStick();
                else
                    base.Update();
            }
            else
            {
                if (_airFireScale < 1.2f)
                    _airFireScale += 0.15f;
                if (_grounded && _stick == null)
                {
                    _airFireScale -= 0.3f;
                    if (_airFireScale < 0.9f)
                        _airFireScale = 0.9f;
                    _spinSpeed -= 0.01f;
                    if (_spinSpeed < 0.05f)
                        _spinSpeed = 0.05f;
                }
                if (_grounded)
                {
                    if (_groundLife <= 0)
                    {
                        alpha -= 0.04f;
                        if (alpha < 0.0)
                            Level.Remove(this);
                    }
                    else
                        --_groundLife;
                }
                if (y > Level.current.bottomRight.y + 200.0)
                    Level.Remove(this);
                _airFire.xscale = _airFire.yscale = _airFireScale;
                _airFire.depth = depth - 1;
                _airFire.alpha = 0.5f;
                _airFire.angle += hSpeed * _spinSpeed;
                if (isLocal && _canMultiply && !_multiplied && Rando.Float(310f) < 1.0 && y > level.topLeft.y - 500.0)
                {
                    Level.Add(SmallFire.New(x, y, Rando.Float(1f) - 0.5f, (float)-(0.5 + Rando.Float(0.5f))));
                    _multiplied = true;
                }
                if (_stick == null)
                {
                    if (level != null && y < level.topLeft.y - 1500.0)
                        Level.Remove(this);
                    base.Update();
                }
                else
                {
                    _grounded = true;
                    if (_stick.destroyed)
                    {
                        _stick = null;
                        _grounded = false;
                    }
                    else
                    {
                        UpdateStick();
                        stick.UpdateFirePosition(this);
                        if (!_stick.onFire || _stick.removeFromLevel || _stick.alpha <  0.01f)
                        {
                            if (DGRSettings.S_ParticleMultiplier != 0) Level.Add(SmallSmoke.New(x, y));
                            Level.Remove(this);
                        }
                    }
                }
                _alternateb = !_alternateb;
                if (!_alternateb)
                    return;
                _alternate = !_alternate;
            }
        }

        public override void Draw() => base.Draw();
    }
}

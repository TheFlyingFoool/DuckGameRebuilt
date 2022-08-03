// Decompiled with JetBrains decompiler
// Type: DuckGame.RagdollPart
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isInDemo", true)]
    public class RagdollPart : Holdable, IAmADuck
    {
        public StateBinding _dollBinding = new StateBinding(nameof(_doll));
        public StateBinding _connectBinding = new StateBinding(nameof(connect));
        public StateBinding _jointBinding = new StateBinding(nameof(_joint));
        public StateBinding _partBinding = new StateBinding(nameof(netPart), 2);
        public StateBinding _framesSinceGroundedBinding = new StateBinding("framesSinceGrounded", 4);
        private bool _setting;
        public float extraGravMultiplier = 1f;
        public Vec2 _lastReasonablePosition;
        private SpriteMap _sprite;
        private SpriteMap _quackSprite;
        public RagdollPart _joint;
        public RagdollPart connect;
        public int clipFrames;
        private DuckPersona _prevPersona;
        private int _part;
        public float addWeight;
        private bool _zekeBear;
        private DuckPersona _rlPersona;
        public Ragdoll _doll;
        private SpriteMap _campDuck;
        private bool extinguishing;
        private bool _setSkipClip;
        private int _ownTime;
        private Vec2 _stickLerp;
        private Vec2 _stickSlowLerp;

        public override NetworkConnection connection
        {
            get => _doll != null ? _doll.connection : base.connection;
            set
            {
                if (_setting)
                    return;
                _setting = true;
                if (_doll != null)
                    _doll.connection = value;
                base.connection = value;
                _setting = false;
            }
        }

        public override NetIndex8 authority
        {
            get => _doll != null ? _doll.authority : base.authority;
            set
            {
                if (_setting)
                    return;
                _setting = true;
                if (_doll != null)
                    _doll.authority = value;
                base.authority = value;
                _setting = false;
            }
        }

        public override float currentGravity => PhysicsObject.gravity * gravMultiplier * floatMultiplier * extraGravMultiplier;

        public byte netPart
        {
            get => (byte)_part;
            set => part = value;
        }

        public RagdollPart joint
        {
            get => _joint;
            set => _joint = value;
        }

        public int part
        {
            get => _part;
            set
            {
                int part = _part;
                _part = value;
                if (_doll != null && _doll._duck != null)
                    _persona = _doll._duck.persona;
                if (this.part == 0)
                    center = new Vec2(16f, 13f);
                else if (this.part == 1)
                    center = new Vec2(16f, 13f);
                else if (this.part == 3)
                    center = new Vec2(6f, 8f);
                else
                    center = new Vec2(8f, 8f);
                if (this.part == 0 || this.part == 1)
                {
                    if (this.part == 0)
                    {
                        collisionOffset = new Vec2(-4f, -5f);
                        collisionSize = new Vec2(8f, 10f);
                    }
                    else
                    {
                        collisionOffset = new Vec2(-4f, -5f);
                        collisionSize = new Vec2(8f, 10f);
                    }
                }
                else
                {
                    collisionOffset = new Vec2(-1f, -1f);
                    collisionSize = new Vec2(2f, 2f);
                }
                if (_persona == null || _prevPersona == _persona && part == _part)
                    return;
                _quackSprite = _persona.quackSprite.CloneMap();
                _sprite = _persona.sprite.CloneMap();
                _quackSprite.frame = 18;
                _sprite.frame = 18;
                if (part != _part || graphic == null)
                    graphic = _sprite;
                _quackSprite.frame = _part == 0 ? 18 : 19;
                _sprite.frame = _part == 0 ? 18 : 19;
                if (doll != null && doll.captureDuck != null && doll.captureDuck.eyesClosed)
                {
                    _quackSprite.frame = _part == 0 ? 20 : 19;
                    _sprite.frame = _part == 0 ? 20 : 19;
                }
                _prevPersona = _persona;
            }
        }

        public override float weight
        {
            get => _weight + addWeight;
            set => _weight = value;
        }

        public override void Zap(Thing zapper)
        {
            if (_doll != null)
                _doll.Zap(zapper);
            base.Zap(zapper);
        }

        public void MakeZekeBear()
        {
            _quackSprite = new SpriteMap("teddy", 32, 32);
            _sprite = _quackSprite;
            _quackSprite.frame = 0;
            _sprite.frame = 0;
            graphic = _sprite;
            _quackSprite.frame = _part == 0 ? 0 : 1;
            _sprite.frame = _part == 0 ? 0 : 1;
            if (part == 0)
                center = new Vec2(16f, 16f);
            else if (part == 1)
                center = new Vec2(16f, 13f);
            else if (part == 3)
                center = new Vec2(6f, 8f);
            else
                center = new Vec2(8f, 8f);
            _zekeBear = true;
        }

        public DuckPersona _persona
        {
            get => _rlPersona;
            set => _rlPersona = value;
        }

        public Ragdoll doll
        {
            get => _doll;
            set => _doll = value;
        }

        public RagdollPart(
          float xpos,
          float ypos,
          int p,
          DuckPersona persona,
          int off,
          Ragdoll doll)
          : base(xpos, ypos)
        {
            if (persona == null)
                persona = Persona.Duck1;
            _sprite = new SpriteMap("crate", 16, 16);
            _campDuck = new SpriteMap("campduck", 32, 32);
            graphic = _sprite;
            _editorName = "Crate";
            thickness = 0.5f;
            weight = 0.05f;
            bouncy = 0.6f;
            _holdOffset = new Vec2(2f, 0f);
            flammable = 0.3f;
            tapeable = false;
            SortOutDetails(xpos, ypos, p, persona, off, doll);
        }

        public override void Extinquish()
        {
            if (extinguishing)
                return;
            extinguishing = true;
            if (doll != null && doll.captureDuck != null)
                doll.captureDuck.Extinquish();
            base.Extinquish();
            extinguishing = false;
        }

        public void SortOutDetails(
          float xpos,
          float ypos,
          int p,
          DuckPersona persona,
          int off,
          Ragdoll doll)
        {
            x = xpos;
            y = ypos;
            hSpeed = 0f;
            vSpeed = 0f;
            _part = part;
            offDir = (sbyte)off;
            airFrictionMult = 0.3f;
            _persona = persona;
            _doll = doll;
            part = p;
        }

        public override void OnTeleport()
        {
            position.x += Math.Sign(hSpeed) * 8;
            doll.part1.position = position;
            doll.part2.position = position;
            doll.part3.position = position;
            doll.part1.hSpeed = hSpeed;
            doll.part2.hSpeed = hSpeed;
            doll.part3.hSpeed = hSpeed;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_doll == null)
                return false;
            if (bullet.isLocal && owner == null)
                Thing.Fondle(_doll, DuckNetwork.localConnection);
            if (bullet.isLocal && _doll.captureDuck != null)
            {
                Duck captureDuck = _doll.captureDuck;
                Equipment equipment1 = captureDuck.GetEquipment(typeof(ChestPlate));
                if (equipment1 != null && Collision.Point(hitPos, equipment1))
                {
                    equipment1.UnEquip();
                    SFX.Play("ting2");
                    captureDuck.Unequip(equipment1);
                    equipment1.hSpeed = bullet.travelDirNormalized.x;
                    equipment1.vSpeed = -2f;
                    equipment1.Destroy(new DTShot(bullet));
                    equipment1.solid = false;
                    return true;
                }
                Equipment equipment2 = captureDuck.GetEquipment(typeof(Helmet));
                if (equipment2 != null && Collision.Point(hitPos, equipment2))
                {
                    equipment2.UnEquip();
                    SFX.Play("ting2");
                    captureDuck.Unequip(equipment2);
                    equipment2.hSpeed = bullet.travelDirNormalized.x;
                    equipment2.vSpeed = -2f;
                    equipment2.Destroy(new DTShot(bullet));
                    equipment2.solid = false;
                    return true;
                }
            }
            Feather feather = Feather.New(0f, 0f, _persona);
            feather.hSpeed = (float)(-bullet.travelDirNormalized.x * (1.0 + Rando.Float(1f)));
            feather.vSpeed = -Rando.Float(2f);
            feather.position = hitPos;
            Level.Add(feather);
            if (bullet.isLocal)
            {
                hSpeed += bullet.travelDirNormalized.x * bullet.ammo.impactPower;
                vSpeed += bullet.travelDirNormalized.y * bullet.ammo.impactPower;
                SFX.Play("thwip", pitch: Rando.Float(-0.1f, 0.1f));
                _doll.Shot(bullet);
            }
            return base.Hit(bullet, hitPos);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (_doll == null)
                return false;
            if (type is DTIncinerate)
            {
                if (!_doll.removeFromLevel && _doll.captureDuck != null && _doll.captureDuck.dead)
                {
                    CookedDuck t = new CookedDuck(_doll.x, _doll.y);
                    Level.Add(SmallSmoke.New(_doll.x + Rando.Float(-4f, 4f), _doll.y + Rando.Float(-4f, 4f)));
                    Level.Add(SmallSmoke.New(_doll.x + Rando.Float(-4f, 4f), _doll.y + Rando.Float(-4f, 4f)));
                    Level.Add(SmallSmoke.New(_doll.x + Rando.Float(-4f, 4f), _doll.y + Rando.Float(-4f, 4f)));
                    ReturnItemToWorld(t);
                    t.vSpeed = vSpeed - 2f;
                    t.hSpeed = hSpeed;
                    Level.Add(t);
                    SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                    Level.Remove(_doll);
                    _doll.captureDuck._cooked = t;
                }
                else
                {
                    if (_doll.captureDuck == null)
                        return false;
                    _doll.captureDuck.Kill(type);
                    return true;
                }
            }
            if (!destroyed)
                _doll.Killed(type);
            return false;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (_doll.captureDuck != null && !_doll.captureDuck.dead && (part == 0 || part == 1) && totalImpactPower > 5.0 && _doll.captureDuck.quack < 0.25)
            {
                _doll.captureDuck.Swear();
                float intensityToSet = Math.Min(totalImpactPower * 0.01f, 1f);
                if (intensityToSet > 0.0500000007450581)
                    RumbleManager.AddRumbleEvent(_doll.captureDuck.profile, new RumbleEvent(intensityToSet, 0.05f, 0.6f));
            }
            base.OnSolidImpact(with, from);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (_doll == null || !isServerForObject || !with.isServerForObject || with is RagdollPart || with is FeatherVolume || with == owner || with == _doll.holdingOwner || with == _doll.captureDuck)
                return;
            if (with is Duck)
            {
                Holdable lastHoldItem = (with as Duck)._lastHoldItem;
                if ((with as Duck)._timeSinceThrow < 15 && (lastHoldItem == _doll.part1 || lastHoldItem == _doll.part2 || lastHoldItem == _doll.part3))
                    return;
            }
            if (_doll.captureDuck == null)
                return;
            Vec2 position = _doll.captureDuck.position;
            _doll.captureDuck.collisionOffset = collisionOffset;
            _doll.captureDuck.collisionSize = collisionSize;
            _doll.captureDuck.position = this.position;
            _doll.captureDuck.OnSoftImpact(with, from);
            _doll.captureDuck.position = position;
        }

        public void UpdateLastReasonablePosition(Vec2 pPosition)
        {
            if (pPosition.y <= -7000.0 || pPosition.y >= Level.activeLevel.lowestPoint + 400.0)
                return;
            _lastReasonablePosition = pPosition;
        }

        public override void Update()
        {
            if (_doll == null || y > Level.activeLevel.lowestPoint + 1000.0 && isOffBottomOfLevel)
                return;
            UpdateLastReasonablePosition(position);
            if (clipFrames > 0)
                --clipFrames;
            if (owner != null && _doll != null && !doll.inSleepingBag)
            {
                ++_ownTime;
                if (_ownTime > 20)
                {
                    _doll.ShakeOutOfSleepingBag();
                    _ownTime = 0;
                }
            }
            if (_doll.captureDuck != null)
            {
                if (_zekeBear)
                {
                    if (_part == 0)
                        depth = _doll.captureDuck.depth + 2;
                    else
                        depth = _doll.captureDuck.depth;
                }
                else if (_part == 0)
                {
                    depth = _doll.captureDuck.depth - 10;
                    if (_doll.part3 != null)
                        depth = _doll.part3.depth - 10;
                }
                else
                    depth = _doll.captureDuck.depth;
                canPickUp = true;
                if (_doll.captureDuck.HasEquipment(typeof(ChokeCollar)) && _part != 0)
                    canPickUp = false;
            }
            if (_joint != null && connect != null)
            {
                if (owner == null && prevOwner != null)
                {
                    clip.Add(prevOwner as PhysicsObject);
                    connect.clip.Add(prevOwner as PhysicsObject);
                    _joint.clipFrames = 12;
                    _joint.clipThing = _prevOwner;
                    _prevOwner = null;
                }
                if (owner != null)
                {
                    _joint.clipFrames = 0;
                    _joint.depth = depth;
                }
                if (owner != null || _joint.owner != null)
                    weight = 0.1f;
                else
                    weight = 4f;
                if (_zekeBear)
                {
                    if (owner != null || _joint.owner != null)
                        weight = 0.1f;
                    else
                        weight = 0.2f;
                }
                if (_joint.clipFrames > 0)
                {
                    skipClip = true;
                    _setSkipClip = true;
                }
                else if (_setSkipClip)
                {
                    skipClip = false;
                    _setSkipClip = false;
                }
            }
            if (_part > 1)
                canPickUp = false;
            base.Update();
            if (_doll.captureDuck != null && _doll.captureDuck.HasEquipment(typeof(FancyShoes)) && _part == 0 && _doll.captureDuck.holdObject != null)
            {
                _doll.captureDuck.holdObject.position = Offset(new Vec2(3f, 5f) + _doll.captureDuck.holdObject.holdOffset);
                _doll.captureDuck.holdObject.angle = angle;
                if (_doll.captureDuck.holdObject != null && _doll.captureDuck.isServerForObject)
                {
                    _doll.captureDuck.holdObject.isLocal = isLocal;
                    _doll.captureDuck.holdObject.UpdateAction();
                }
            }
            if (doll != null && doll.part3 != null)
            {
                offDir = doll.part3.offDir;
                if (doll.captureDuck != null && enablePhysics)
                {
                    doll.captureDuck.offDir = offDir;
                    if (owner != null)
                        doll.captureDuck._prevOwner = owner;
                }
            }
            FluidPuddle fluidPuddle = Level.CheckPoint<FluidPuddle>(position + new Vec2(0f, 4f));
            if (fluidPuddle != null)
            {
                if (y + 4.0 - fluidPuddle.top > 8.0)
                {
                    gravMultiplier = -0.5f;
                    grounded = false;
                }
                else
                {
                    if (y + 4.0 - fluidPuddle.top < 3.0)
                    {
                        gravMultiplier = 0.2f;
                        grounded = true;
                    }
                    else if (y + 4.0 - fluidPuddle.top > 4.0)
                    {
                        gravMultiplier = -0.2f;
                        grounded = true;
                    }
                    grounded = true;
                }
            }
            else
                gravMultiplier = 1f;
            if (_joint != null)
            {
                if (_doll.captureDuck != null && _doll.captureDuck.IsQuacking())
                    graphic = _quackSprite;
                else
                    graphic = _sprite;
                if (isServerForObject)
                {
                    if (offDir < 0)
                        angleDegrees = (float)(-Maths.PointDirection(position, _joint.position) + 180.0 + 90.0);
                    else
                        angleDegrees = (float)(-Maths.PointDirection(position, _joint.position) - 90.0);
                }
            }
            if (_part == 3 && connect != null)
            {
                angleDegrees = (float)(-Maths.PointDirection(position, connect.position) + 180.0);
                depth = connect.depth + 2;
            }
            visible = _part != 2;
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (!_onFire)
            {
                SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                for (int index = 0; index < 2; ++index)
                    Level.Add(SmallFire.New(Rando.Float(6f) - 3f, Rando.Float(2f) - 2f, 0f, 0f, stick: this));
                _onFire = true;
                _doll.LitOnFire(litBy);
            }
            return true;
        }

        public override void Draw()
        {
            addWeight = 0f;
            extraGravMultiplier = 1f;
            if (_part == 2 || _joint == null)
                return;
            Vec2 position = this.position;
            Vec2 vec2_1 = this.position - _joint.position;
            float num1 = vec2_1.length;
            if (num1 > 8.0)
                num1 = 8f;
            this.position = _joint.position + vec2_1.normalized * num1;
            if (_part == 0 && _doll != null && _doll.captureDuck != null && (_doll.captureDuck.quack > 0 || doll != null && doll.tongueStuck != Vec2.Zero))
            {
                Vec2 tounge = _doll.captureDuck.tounge;
                _stickLerp = Lerp.Vec2Smooth(_stickLerp, tounge, 0.2f);
                _stickSlowLerp = Lerp.Vec2Smooth(_stickSlowLerp, tounge, 0.1f);
                Vec2 stickLerp = _stickLerp;
                Vec2 vec = Maths.AngleToVec(angle);
                Vec2 vec2_2 = offDir >= 0 ? stickLerp * Maths.Clamp(1f - (vec - stickLerp).length, 0f, 1f) : stickLerp * Maths.Clamp(1f - (vec - stickLerp * -1f).length, 0f, 1f);
                vec2_2.y *= -1f;
                Vec2 vec2_3 = _stickSlowLerp;
                vec2_3.y *= -1f;
                float num2 = vec2_2.length;
                bool flag = false;
                if (doll != null && doll.tongueStuck != Vec2.Zero)
                {
                    flag = true;
                    num2 = 1f;
                }
                if (num2 > 0.0500000007450581 | flag)
                {
                    Vec2 vec2_4 = this.position - (this.position - _joint.position).normalized * 3f;
                    if (flag)
                    {
                        vec2_2 = (doll.tongueStuck - vec2_4) / 6f;
                        Vec2 vec2_5 = (doll.tongueStuck - vec2_4) / 6f / 2f;
                        vec2_3 = (Offset(new Vec2((doll.tongueStuck - vec2_4).length / 2f, 2f)) - vec2_4) / 6f;
                    }
                    List<Vec2> vec2List = Curve.Bezier(8, vec2_4, vec2_4 + vec2_3 * 6f, vec2_4 + vec2_2 * 6f);
                    Vec2 vec2_6 = Vec2.Zero;
                    float num3 = 1f;
                    foreach (Vec2 p2 in vec2List)
                    {
                        if (vec2_6 != Vec2.Zero)
                        {
                            Vec2 vec2_7 = vec2_6 - p2;
                            Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_6 + vec2_7.normalized * 0.4f, p2, new Color(223, 30, 30), 0.15f * num3, depth + 1);
                            Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_6 + vec2_7.normalized * 0.4f, p2 - vec2_7.normalized * 0.4f, Color.Black, 0.3f * num3, depth - 1);
                        }
                        num3 -= 0.1f;
                        vec2_6 = p2;
                        if (doll != null && doll.captureDuck != null)
                            doll.captureDuck.tongueCheck = p2;
                    }
                    if (_graphic != null && _graphic == _quackSprite)
                    {
                        SpriteMap graphic = this.graphic as SpriteMap;
                        if (doll != null && doll.inSleepingBag)
                            _graphic = _campDuck;
                        if (_offDir < 0)
                            _graphic.flipH = true;
                        else
                            _graphic.flipH = false;
                        _graphic.position = this.position;
                        _graphic.alpha = alpha;
                        _graphic.angle = angle;
                        _graphic.depth = depth + 4;
                        _graphic.scale = scale;
                        _graphic.center = center;
                        if (_graphic == _campDuck)
                        {
                            (_graphic as SpriteMap).frame = 4;
                            _graphic.Draw();
                        }
                        else
                        {
                            (_graphic as SpriteMap).frame += 36;
                            _graphic.Draw();
                            (_graphic as SpriteMap).frame -= 36;
                        }
                        _graphic = graphic;
                    }
                }
                else if (doll != null && doll.captureDuck != null)
                    doll.captureDuck.tongueCheck = Vec2.Zero;
            }
            else if (doll != null && doll.captureDuck != null)
                doll.captureDuck.tongueCheck = Vec2.Zero;
            SpriteMap graphic1 = graphic as SpriteMap;
            if (graphic1 != null && doll != null && doll.inSleepingBag)
            {
                if (graphic1.frame == 18)
                    _campDuck.frame = 0;
                else if (graphic1.frame == 19)
                    _campDuck.frame = 1;
                else if (graphic1.frame == 20)
                    _campDuck.frame = 2;
                if (graphic1 == _quackSprite && graphic1.frame == 18)
                    _campDuck.frame = 3;
                graphic = _campDuck;
            }
            float angleDegrees = this.angleDegrees;
            if (offDir < 0)
                this.angleDegrees = (float)(-Maths.PointDirection(this.position, _joint.position) + 180.0 + 90.0);
            else
                this.angleDegrees = (float)(-Maths.PointDirection(this.position, _joint.position) - 90.0);
            base.Draw();
            this.angleDegrees = angleDegrees;
            graphic = graphic1;
            this.position = position;
        }
    }
}

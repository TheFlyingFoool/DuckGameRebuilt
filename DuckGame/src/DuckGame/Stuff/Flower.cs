// Decompiled with JetBrains decompiler
// Type: DuckGame.Flower
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    [BaggedProperty("isInDemo", true)]
    public class Flower : Holdable
    {
        private Sprite _burnt;
        public bool _picked;
        private int framesSinceThrown = 1000;
        public Gun _stuck;
        public float _prevBarrelHeat;

        public Flower(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("flower");
            _burnt = new Sprite("flower_burned");
            center = new Vec2(8f, 12f);
            collisionOffset = new Vec2(-3f, -12f);
            collisionSize = new Vec2(6f, 14f);
            _holdOffset = new Vec2(-2f, 2f);
            depth = -0.5f;
            weight = 1f;
            flammable = 0.3f;
            hugWalls = WallHug.Floor;
            editorTooltip = "It's beautiful.";
        }

        protected override bool OnDestroy(DestroyType type = null) => false;

        public static void PoofEffect(Vec2 pPosition)
        {
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
            {
                ConfettiParticle confettiParticle = new ConfettiParticle();
                confettiParticle.Init(pPosition.x + Rando.Float(-4f, 0f), pPosition.y + Rando.Float(-4f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)));
                confettiParticle._color = new Color(49, 163, 242);
                Level.Add(confettiParticle);
            }
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
            {
                ConfettiParticle confettiParticle = new ConfettiParticle();
                confettiParticle.Init(pPosition.x + Rando.Float(-4f, 0f), pPosition.y + Rando.Float(-4f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)));
                confettiParticle._color = new Color(163, 206, 39);
                Level.Add(confettiParticle);
            }
        }

        public override void Update()
        {
            if (burnt >= 1)
            {
                if (graphic != _burnt)
                {
                    SFX.Play("flameExplode");
                    Level.Add(SmallFire.New(x + Rando.Float(-2f, 2f), y + Rando.Float(-2f, 2f), Rando.Float(2f) - 1f, Rando.Float(2f) - 1f, firedFrom: this));
                    Level.Add(SmallFire.New(x + Rando.Float(-2f, 2f), y + Rando.Float(-2f, 2f), Rando.Float(2f) - 1f, Rando.Float(2f) - 1f, firedFrom: this));
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 3; ++index)
                        Level.Add(SmallSmoke.New(x + Rando.Float(-2f, 2f), y + Rando.Float(-2f, 2f)));
                }
                graphic = _burnt;
            }
            if (_stuck != null)
            {
                if (held || graphic == _burnt)
                {
                    _stuck.plugged = false;
                    _stuck = null;
                }
                else
                {
                    _stuck.plugged = true;
                    if (Network.isActive && _stuck.isServerForObject)
                        _stuck.Fondle(this);
                    if (_stuck.removeFromLevel && isServerForObject)
                    {
                        if (_stuck is DuelingPistol)
                            vSpeed -= 2f;
                        _stuck = null;
                    }
                    else
                    {
                        position = _stuck.Offset(_stuck.barrelOffset + _stuck.barrelInsertOffset + new Vec2(1f, 1f));
                        offDir = _stuck.offDir;
                        angleDegrees = _stuck.angleDegrees + 90 * offDir;
                        depth = _stuck.depth - 4;
                        velocity = Vec2.Zero;
                        if (_stuck._barrelHeat < _prevBarrelHeat)
                            _prevBarrelHeat = _stuck._barrelHeat;
                        if (!isServerForObject || _stuck._barrelHeat <= _prevBarrelHeat + 0.01f)
                            return;
                        PoofEffect(position);
                        if (Network.isActive)
                            Send.Message(new NMFlowerPoof(position));
                        Level.Remove(this);
                        return;
                    }
                }
            }
            if (Math.Abs(hSpeed) > 0.2f || !_picked && owner != null)
                _picked = true;
            if (_picked)
            {
                if (owner != null)
                {
                    framesSinceThrown = 0;
                    center = new Vec2(8f, 12f);
                    collisionOffset = new Vec2(-3f, -12f);
                    collisionSize = new Vec2(6f, 14f);
                    angleDegrees = 0f;
                    graphic.flipH = offDir < 0;
                }
                else
                {
                    depth = -0.5f;
                    if (framesSinceThrown < 15)
                    {
                        Gun gun = Level.current.NearestThing<Gun>(position);
                        if (gun != null && (gun.barrelPosition - position).length < 4 && gun.held && gun.wideBarrel && (gun.offDir > 0 && hSpeed < 0 || gun.offDir < 0 && hSpeed > 0))
                        {
                            _stuck = gun;
                            _prevBarrelHeat = _stuck._barrelHeat;
                            SFX.PlaySynchronized("pipeOut", pitch: 0.2f);
                        }
                    }
                    ++framesSinceThrown;
                    center = new Vec2(8f, 8f);
                    collisionOffset = new Vec2(-7f, -5f);
                    collisionSize = new Vec2(14f, 6f);
                    angleDegrees = 90f;
                    graphic.flipH = true;
                    depth = (Depth)0.4f;
                }
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (graphic == _burnt)
                return;
            if (Network.isActive)
            {
                if (isServerForObject)
                    NetSoundEffect.Play("flowerHappyQuack");
            }
            else
                SFX.Play("happyQuack01", pitch: Rando.Float(-0.1f, 0.1f));
            if (duck != null)
            {
                duck.quack = 20;
            }
            else
            {
                Level.Remove(this);
                SFX.Play("flameExplode");
                for (int index = 0; index < 8; ++index)
                    Level.Add(SmallFire.New(x + Rando.Float(-8f, 8f), y + Rando.Float(-8f, 8f), Rando.Float(6f) - 3f, Rando.Float(6f) - 3f, firedFrom: this));
            }
        }

        public override void OnReleaseAction()
        {
            if (duck == null || graphic == _burnt)
                return;
            duck.quack = 0;
        }

        public override void Draw()
        {
            if (_stuck != null)
            {
                position = _stuck.Offset(_stuck.barrelOffset + _stuck.barrelInsertOffset + new Vec2(1f, 1f));
                offDir = _stuck.offDir;
                angleDegrees = _stuck.angleDegrees + 90 * offDir;
                depth = _stuck.depth - 4;
                velocity = Vec2.Zero;
            }
            base.Draw();
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.Flower
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.graphic = new Sprite("flower");
            this._burnt = new Sprite("flower_burned");
            this.center = new Vec2(8f, 12f);
            this.collisionOffset = new Vec2(-3f, -12f);
            this.collisionSize = new Vec2(6f, 14f);
            this._holdOffset = new Vec2(-2f, 2f);
            this.depth = -0.5f;
            this.weight = 1f;
            this.flammable = 0.3f;
            this.hugWalls = WallHug.Floor;
            this.editorTooltip = "It's beautiful.";
        }

        protected override bool OnDestroy(DestroyType type = null) => false;

        public static void PoofEffect(Vec2 pPosition)
        {
            for (int index = 0; index < 4; ++index)
            {
                ConfettiParticle confettiParticle = new ConfettiParticle();
                confettiParticle.Init(pPosition.x + Rando.Float(-4f, 0f), pPosition.y + Rando.Float(-4f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)));
                confettiParticle._color = new Color(49, 163, 242);
                Level.Add(confettiParticle);
            }
            for (int index = 0; index < 2; ++index)
            {
                ConfettiParticle confettiParticle = new ConfettiParticle();
                confettiParticle.Init(pPosition.x + Rando.Float(-4f, 0f), pPosition.y + Rando.Float(-4f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)));
                confettiParticle._color = new Color(163, 206, 39);
                Level.Add(confettiParticle);
            }
        }

        public override void Update()
        {
            if (burnt >= 1.0)
            {
                if (this.graphic != this._burnt)
                {
                    SFX.Play("flameExplode");
                    Level.Add(SmallFire.New(this.x + Rando.Float(-2f, 2f), this.y + Rando.Float(-2f, 2f), Rando.Float(2f) - 1f, Rando.Float(2f) - 1f, firedFrom: this));
                    Level.Add(SmallFire.New(this.x + Rando.Float(-2f, 2f), this.y + Rando.Float(-2f, 2f), Rando.Float(2f) - 1f, Rando.Float(2f) - 1f, firedFrom: this));
                    for (int index = 0; index < 3; ++index)
                        Level.Add(SmallSmoke.New(this.x + Rando.Float(-2f, 2f), this.y + Rando.Float(-2f, 2f)));
                }
                this.graphic = this._burnt;
            }
            if (this._stuck != null)
            {
                if (this.held || this.graphic == this._burnt)
                {
                    this._stuck.plugged = false;
                    this._stuck = null;
                }
                else
                {
                    this._stuck.plugged = true;
                    if (Network.isActive && this._stuck.isServerForObject)
                        this._stuck.Fondle(this);
                    if (this._stuck.removeFromLevel && this.isServerForObject)
                    {
                        if (this._stuck is DuelingPistol)
                            this.vSpeed -= 2f;
                        this._stuck = null;
                    }
                    else
                    {
                        this.position = this._stuck.Offset(this._stuck.barrelOffset + this._stuck.barrelInsertOffset + new Vec2(1f, 1f));
                        this.offDir = this._stuck.offDir;
                        this.angleDegrees = this._stuck.angleDegrees + 90 * offDir;
                        this.depth = this._stuck.depth - 4;
                        this.velocity = Vec2.Zero;
                        if (_stuck._barrelHeat < this._prevBarrelHeat)
                            this._prevBarrelHeat = this._stuck._barrelHeat;
                        if (!this.isServerForObject || _stuck._barrelHeat <= _prevBarrelHeat + 0.01f)
                            return;
                        Flower.PoofEffect(this.position);
                        if (Network.isActive)
                            Send.Message(new NMFlowerPoof(this.position));
                        Level.Remove(this);
                        return;
                    }
                }
            }
            if (Math.Abs(this.hSpeed) > 0.2f || !this._picked && this.owner != null)
                this._picked = true;
            if (this._picked)
            {
                if (this.owner != null)
                {
                    this.framesSinceThrown = 0;
                    this.center = new Vec2(8f, 12f);
                    this.collisionOffset = new Vec2(-3f, -12f);
                    this.collisionSize = new Vec2(6f, 14f);
                    this.angleDegrees = 0f;
                    this.graphic.flipH = this.offDir < 0;
                }
                else
                {
                    this.depth = -0.5f;
                    if (this.framesSinceThrown < 15)
                    {
                        Gun gun = Level.current.NearestThing<Gun>(this.position);
                        if (gun != null && (double)(gun.barrelPosition - this.position).length < 4.0 && gun.held && gun.wideBarrel && (gun.offDir > 0 && (double)this.hSpeed < 0.0 || gun.offDir < 0 && (double)this.hSpeed > 0.0))
                        {
                            this._stuck = gun;
                            this._prevBarrelHeat = this._stuck._barrelHeat;
                            SFX.PlaySynchronized("pipeOut", pitch: 0.2f);
                        }
                    }
                    ++this.framesSinceThrown;
                    this.center = new Vec2(8f, 8f);
                    this.collisionOffset = new Vec2(-7f, -5f);
                    this.collisionSize = new Vec2(14f, 6f);
                    this.angleDegrees = 90f;
                    this.graphic.flipH = true;
                    this.depth = (Depth)0.4f;
                }
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.graphic == this._burnt)
                return;
            if (Network.isActive)
            {
                if (this.isServerForObject)
                    NetSoundEffect.Play("flowerHappyQuack");
            }
            else
                SFX.Play("happyQuack01", pitch: Rando.Float(-0.1f, 0.1f));
            if (this.duck != null)
            {
                this.duck.quack = 20;
            }
            else
            {
                Level.Remove(this);
                SFX.Play("flameExplode");
                for (int index = 0; index < 8; ++index)
                    Level.Add(SmallFire.New(this.x + Rando.Float(-8f, 8f), this.y + Rando.Float(-8f, 8f), Rando.Float(6f) - 3f, Rando.Float(6f) - 3f, firedFrom: this));
            }
        }

        public override void OnReleaseAction()
        {
            if (this.duck == null || this.graphic == this._burnt)
                return;
            this.duck.quack = 0;
        }

        public override void Draw()
        {
            if (this._stuck != null)
            {
                this.position = this._stuck.Offset(this._stuck.barrelOffset + this._stuck.barrelInsertOffset + new Vec2(1f, 1f));
                this.offDir = this._stuck.offDir;
                this.angleDegrees = this._stuck.angleDegrees + 90 * offDir;
                this.depth = this._stuck.depth - 4;
                this.velocity = Vec2.Zero;
            }
            base.Draw();
        }
    }
}

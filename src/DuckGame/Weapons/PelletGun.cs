// Decompiled with JetBrains decompiler
// Type: DuckGame.PelletGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Rifles")]
    public class PelletGun : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding(nameof(_loadState));
        public StateBinding _firesTillFailBinding = new StateBinding(nameof(firesTillFail));
        public StateBinding _aimAngleBinding = new StateBinding(nameof(_aimAngle));
        private SpriteMap _sprite;
        private Sprite _spring;
        public int _loadState = -1;
        public float _angleOffset;
        public float _angleOffset2;
        public float _aimAngle;
        public int _aimWait;
        private Vec2 _posOffset;
        public int firesTillFail = 8;
        private Vec2 springPos = Vec2.Zero;
        private Vec2 springVel = Vec2.Zero;
        private bool _rising;

        public override float angle
        {
            get => base.angle - Math.Max(this._aimAngle, -0.2f) * offDir;
            set => base.angle = value;
        }

        public PelletGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 2;
            this._ammoType = new ATPellet();
            this._type = "gun";
            this._sprite = new SpriteMap("pelletGun", 31, 7);
            this.graphic = _sprite;
            this.center = new Vec2(15f, 2f);
            this.collisionOffset = new Vec2(-8f, -2f);
            this.collisionSize = new Vec2(16f, 5f);
            this._spring = new Sprite("dandiSpring");
            this._barrelOffsetTL = new Vec2(30f, 2f);
            this._fireSound = "pelletgun";
            this._kickForce = 0.0f;
            this._manualLoad = true;
            this._holdOffset = new Vec2(-2f, -1f);
            this.editorTooltip = "Careful with that thing, you'll lose an eye!";
            this._editorName = "Dandylion";
        }

        public override Vec2 Offset(Vec2 pos) => this.position + this._posOffset + this.OffsetLocal(pos);

        public override void Update()
        {
            if ((bool)this.infinite)
                this.firesTillFail = 100;
            if (this.firesTillFail == 1)
                this._fireSound = "pelletgunFail";
            if (this.firesTillFail <= 0)
            {
                this._fireSound = "pelletgunBad";
                if (!(this.ammoType is ATFailedPellet))
                    this._ammoType = new ATFailedPellet();
                this.springVel += (this.Offset(new Vec2(0.0f, -8f)) - this.springPos) * 0.15f;
                this.springVel *= 0.9f;
                this.springPos += this.springVel;
            }
            else
                this.springPos = this.position;
            ++this._aimWait;
            if (this._aimWait > 0)
            {
                this._aimAngle = Lerp.Float(this._aimAngle, this._rising ? 0.4f : 0.0f, 0.05f);
                this._aimWait = 0;
            }
            if (this._rising && _aimAngle > 0.344999998807907)
                this.OnReleaseAction();
            if (this.held)
                this.center = new Vec2(11f, 2f);
            else
                this.center = new Vec2(15f, 2f);
            if (this._loadState > -1)
            {
                if (this.owner == null)
                {
                    if (this._loadState == 3)
                        this.loaded = true;
                    this._loadState = -1;
                    this._angleOffset = 0.0f;
                    this._posOffset = Vec2.Zero;
                    this.handOffset = Vec2.Zero;
                    this._aimAngle = 0.0f;
                    this._angleOffset2 = 0.0f;
                }
                this._posOffset = this._loadState <= 0 || this._loadState >= 4 ? Lerp.Vec2(this._posOffset, new Vec2(0.0f, 0.0f), 0.24f) : Lerp.Vec2(this._posOffset, new Vec2(2f, 2f), 0.2f);
                this._angleOffset2 = this._loadState < 2 || this._loadState >= 3 ? Lerp.Float(this._angleOffset2, 0.0f, 0.02f) : Lerp.Float(this._angleOffset2, -0.17f, 0.04f);
                if (this._loadState == 0)
                {
                    if (Network.isActive)
                    {
                        if (this.isServerForObject)
                            NetSoundEffect.Play("pelletGunSwipe");
                    }
                    else
                        SFX.Play("swipe", 0.4f, 0.3f);
                    ++this._loadState;
                }
                else if (this._loadState == 1)
                {
                    if (_angleOffset < 0.159999996423721)
                        this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.2f, 0.11f);
                    else
                        ++this._loadState;
                }
                else if (this._loadState == 2)
                {
                    this.handOffset.x += 0.31f;
                    if (handOffset.x > 4.0)
                    {
                        ++this._loadState;
                        this.ammo = 2;
                        this.loaded = false;
                        if (Network.isActive)
                        {
                            if (this.isServerForObject)
                                NetSoundEffect.Play("pelletGunLoad");
                        }
                        else
                            SFX.Play("loadLow", 0.7f, Rando.Float(-0.05f, 0.05f));
                    }
                }
                else if (this._loadState == 3)
                {
                    this.handOffset.x -= 0.2f;
                    if (handOffset.x <= 0.0)
                    {
                        ++this._loadState;
                        this.handOffset.x = 0.0f;
                        if (Network.isActive)
                        {
                            if (this.isServerForObject)
                                NetSoundEffect.Play("pelletGunSwipe2");
                        }
                        else
                            SFX.Play("swipe", 0.5f, 0.4f);
                    }
                }
                else if (this._loadState == 4)
                {
                    if (_angleOffset > 0.0299999993294477)
                    {
                        this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.0f, 0.09f);
                    }
                    else
                    {
                        this._loadState = -1;
                        this.loaded = true;
                        this._angleOffset = 0.0f;
                        if (Network.isActive)
                        {
                            if (this.isServerForObject)
                                NetSoundEffect.Play("pelletGunClick");
                        }
                        else
                            SFX.Play("click", pitch: 0.5f);
                    }
                }
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.isServerForObject)
            {
                if (this.loaded && this.ammo > 1)
                {
                    this._rising = true;
                    this._aimAngle = -0.3f;
                    this._aimWait = 0;
                }
                else
                {
                    if (this._loadState != -1)
                        return;
                    this._loadState = 0;
                }
            }
            else
                base.OnPressAction();
        }

        private void RunFireCode()
        {
            base.OnPressAction();
            for (int index = 0; index < 4; ++index)
                Level.Add(SmallSmoke.New(this.barrelPosition.x + offDir * 4f, this.barrelPosition.y));
            Level.Add(SmallSmoke.New(this.position.x, this.position.y));
        }

        public override void OnReleaseAction()
        {
            if (this.receivingPress)
            {
                this.RunFireCode();
            }
            else
            {
                if (!this._rising)
                    return;
                if (this.loaded && this.ammo > 1)
                {
                    this.RunFireCode();
                    --this.firesTillFail;
                    this.ammo = 1;
                }
                this._rising = false;
            }
        }

        public override void Draw()
        {
            this._sprite.center = this.center;
            this._sprite.depth = this.depth;
            this._sprite.angle = this.angle;
            this._sprite.frame = 0;
            this._sprite.alpha = this.alpha;
            if (this.owner != null && this.owner.graphic != null && (this.duck == null || !(this.duck.holdObject is TapedGun)))
                this._sprite.flipH = this.owner.graphic.flipH;
            else
                this._sprite.flipH = this.offDir <= 0;
            if (this.offDir > 0)
                this._sprite.angle = this.angle - this._angleOffset - this._angleOffset2;
            else
                this._sprite.angle = this.angle + this._angleOffset + this._angleOffset2;
            Vec2 vec2 = this.Offset(this._posOffset);
            Graphics.Draw(_sprite, vec2.x, vec2.y);
            this._sprite.frame = 1;
            if (this.offDir > 0)
                this._sprite.angle = this.angle + this._angleOffset * 3f - this._angleOffset2;
            else
                this._sprite.angle = this.angle - this._angleOffset * 3f + this._angleOffset2;
            Graphics.Draw(_sprite, vec2.x, vec2.y);
            if (this.firesTillFail > 0)
                return;
            this._spring.depth = this.depth - 5;
            this._spring.center = new Vec2(4f, 7f);
            this._spring.angleDegrees = Maths.PointDirection(this.position + this._posOffset, this.springPos) - 90f;
            this._spring.yscale = (float)((position.y + (double)this._posOffset.y - springPos.y) / 8.0);
            this._spring.flipH = this.offDir < 0;
            if ((double)this._spring.yscale > 1.20000004768372)
                this._spring.yscale = 1.2f;
            if ((double)this._spring.yscale < -1.20000004768372)
                this._spring.yscale = -1.2f;
            this._spring.alpha = this.alpha;
            Graphics.Draw(this._spring, vec2.x, vec2.y);
        }
    }
}

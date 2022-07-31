// Decompiled with JetBrains decompiler
// Type: DuckGame.TampingWeapon
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class TampingWeapon : Gun
    {
        public StateBinding _tampedBinding = new TampingFlagBinding();
        public StateBinding _tampIncBinding = new StateBinding(nameof(_tampInc));
        public StateBinding _tampTimeBinding = new StateBinding(nameof(_tampTime));
        public StateBinding _offsetYBinding = new StateBinding(nameof(_offsetY));
        public StateBinding _rotAngleBinding = new StateBinding(nameof(_rotAngle));
        public bool _tamped = true;
        public float _tampInc;
        public float _tampTime;
        public bool _rotating;
        public float _offsetY;
        public float _rotAngle;
        public float _tampBoost = 1f;
        private Sprite _tampingHand;
        private bool _puffed;
        private Duck _prevDuckOwner;

        public override float angle
        {
            get => base.angle + Maths.DegToRad(-this._rotAngle);
            set => this._angle = value;
        }

        public TampingWeapon(float xval, float yval)
          : base(xval, yval)
        {
            this._tampingHand = new Sprite("tampingHand")
            {
                center = new Vec2(4f, 8f)
            };
        }

        public override void Update()
        {
            base.Update();
            this._tampBoost = Lerp.Float(this._tampBoost, 1f, 0.01f);
            if (this.owner is Duck owner && owner.inputProfile != null && this.duck != null && this.duck.profile != null)
            {
                this._prevDuckOwner = owner;
                if (owner.inputProfile.Pressed("SHOOT"))
                    this._tampBoost += 0.14f;
                if (this.duck.immobilized)
                    this.duck.profile.stats.timeSpentReloadingOldTimeyWeapons += Maths.IncFrameTimer();
                if (this._rotating)
                {
                    if (this.offDir < 0)
                    {
                        if (_rotAngle > -90.0)
                            this._rotAngle -= 3f;
                        if (_rotAngle <= -90.0)
                        {
                            this.tamping = true;
                            this._tampInc += 0.2f * this._tampBoost;
                            this.tampPos = (float)Math.Sin(_tampInc) * 2f;
                            if (tampPos < -1.0 && !this._puffed)
                            {
                                Vec2 vec2 = this.Offset(this.barrelOffset) - this.barrelVector * 8f;
                                Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                                this._puffed = true;
                            }
                            if (tampPos > -1.0)
                                this._puffed = false;
                            this._tampTime += 0.005f * this._tampBoost;
                        }
                        if (_tampTime >= 1.0)
                        {
                            this._rotAngle += 8f;
                            if (_offsetY > 0.0)
                                this._offsetY -= 2f;
                            this.tamping = false;
                            if (_rotAngle >= 0.0)
                            {
                                this._rotAngle = 0f;
                                this._rotating = false;
                                this._tamped = true;
                                this._offsetY = 0f;
                                owner.immobilized = false;
                            }
                        }
                    }
                    else
                    {
                        if (_rotAngle < 90.0)
                            this._rotAngle += 3f;
                        if (_rotAngle >= 90.0)
                        {
                            this.tamping = true;
                            this._tampInc += 0.2f * this._tampBoost;
                            this.tampPos = (float)Math.Sin(_tampInc) * 2f;
                            if (tampPos < -1.0 && !this._puffed)
                            {
                                Vec2 vec2 = this.Offset(this.barrelOffset) - this.barrelVector * 8f;
                                Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                                this._puffed = true;
                            }
                            if (tampPos > -1.0)
                                this._puffed = false;
                            this._tampTime += 0.005f * this._tampBoost;
                        }
                        if (_tampTime >= 1.0)
                        {
                            this._rotAngle -= 8f;
                            if (_offsetY > 0.0)
                                this._offsetY -= 2f;
                            this.tamping = false;
                            if (_rotAngle <= 0.0)
                            {
                                this._rotAngle = 0f;
                                this._rotating = false;
                                this._tamped = true;
                                this._offsetY = 0f;
                                owner.immobilized = false;
                            }
                        }
                    }
                    if (_offsetY >= 10.0)
                        return;
                    ++this._offsetY;
                }
                else
                    this._tampBoost = 1f;
            }
            else
            {
                if (this._prevDuckOwner == null)
                    return;
                this._prevDuckOwner.immobilized = false;
                this.tamping = false;
                this._rotAngle = 0f;
                this._rotating = false;
                this._offsetY = 0f;
                this._prevDuckOwner = null;
            }
        }

        public override void Draw()
        {
            this.y += this._offsetY;
            base.Draw();
            if (this.duck != null && this.tamping)
            {
                if (this.offDir < 0)
                {
                    this._tampingHand.x = this.x + 3f;
                    this._tampingHand.y = this.y - 16f + this.tampPos;
                    this._tampingHand.flipH = true;
                }
                else
                {
                    this._tampingHand.x = this.x - 3f;
                    this._tampingHand.y = this.y - 16f + this.tampPos;
                    this._tampingHand.flipH = false;
                }
                this._tampingHand.depth = this.depth - 1;
                float angle = this.duck._spriteArms.angle;
                Vec2 vec2 = this.Offset(this.barrelOffset);
                Vec2 p2 = vec2 + this.barrelVector * (float)(tampPos * 2.0 + 3.0);
                Graphics.DrawLine(vec2 - this.barrelVector * 6f, p2, Color.Gray, depth: (this.depth - 2));
                this.duck._spriteArms.depth = this.depth - 1;
                Graphics.Draw(duck._spriteArms, p2.x, p2.y);
                this.duck._spriteArms.angle = angle;
            }
            this.position = new Vec2(this.position.x, this.position.y - this._offsetY);
        }
    }
}

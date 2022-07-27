// Decompiled with JetBrains decompiler
// Type: DuckGame.Basketball
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    public class Basketball : Holdable
    {
        private SpriteMap _sprite;
        private int _framesInHand;
        private int _walkFrames;
        private Duck _bounceDuck;

        public Basketball(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("basketBall", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(15f, 15f);
            this.depth = - 0.5f;
            this.thickness = 1f;
            this.weight = 3f;
            this.flammable = 0.3f;
            this.collideSounds.Add("basketball");
            this.physicsMaterial = PhysicsMaterial.Rubber;
            this._bouncy = 0.8f;
            this.friction = 0.03f;
            this._impactThreshold = 0.1f;
            this._holdOffset = new Vec2(6f, 0.0f);
            this.handOffset = new Vec2(0.0f, -0.0f);
            this.editorTooltip = "Perfect for playing the world's greatest sport! Also basketball.";
        }

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            owner.holdObstructed = false;
            if (!owner.action)
                return;
            owner.holdObstructed = true;
        }

        public override void OnPressAction()
        {
        }

        public override void Update()
        {
            if (!this.isServerForObject)
                this._bounceDuck = null;
            else if (this.owner == null)
            {
                this._walkFrames = 0;
                --this._framesInHand;
                if (this._framesInHand < -60)
                    this._bounceDuck = null;
                if (this._bounceDuck != null)
                {
                    float length = (this._bounceDuck.position - this.position).length;
                    if ((double)length < 16.0)
                        this.hSpeed = this._bounceDuck.hSpeed;
                    if (this._bounceDuck.holdObject == null && (double)this.vSpeed < 1.0 && (double)this._bounceDuck.top + 8.0 > (double)this.y && (double)length < 16.0)
                    {
                        this._bounceDuck.GiveHoldable(this);
                        this._framesInHand = 0;
                    }
                }
            }
            else if (this.duck != null && this.duck.holdObject == this)
            {
                if (this._framesInHand < 0)
                    this._framesInHand = 0;
                if (!this.owner.action && (double)Math.Abs(this.owner.hSpeed) > 0.5 && this._framesInHand > 6)
                {
                    this._bounceDuck = this.duck;
                    float hSpeed = this.duck.hSpeed;
                    this.duck.ThrowItem(false);
                    this.vSpeed = 2f;
                    this.hSpeed = hSpeed * 1.1f;
                    this._framesInHand = 0;
                }
                else
                {
                    if ((double)Math.Abs(this.owner.hSpeed) > 0.5 && this.duck.grounded)
                        ++this._walkFrames;
                    else if (this.duck.grounded)
                        --this._walkFrames;
                    if (this._walkFrames < 0)
                        this._walkFrames = 0;
                    if (this._walkFrames > 20)
                    {
                        SFX.PlaySynchronized("basketballWhistle");
                        this.duck.ThrowItem(false);
                        this._walkFrames = 0;
                    }
                    this._bounceDuck = null;
                    ++this._framesInHand;
                }
            }
            base.Update();
        }
    }
}

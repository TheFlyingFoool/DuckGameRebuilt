// Decompiled with JetBrains decompiler
// Type: DuckGame.ExtinguisherSmoke
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ExtinguisherSmoke : PhysicsParticle, ITeleport
    {
        private SpriteMap _sprite;
        private SinWaveManualUpdate _moveWave = new SinWaveManualUpdate(Rando.Float(0.15f, 0.17f), Rando.Float(6.28f));
        private SinWaveManualUpdate _moveWave2 = new SinWaveManualUpdate(Rando.Float(0.15f, 0.17f), Rando.Float(6.28f));
        private float _fullScale = Rando.Float(1.1f, 1.5f);
        private int _smokeID;
        private float _orbitInc = Rando.Float(5f);
        private SpriteMap _sprite2;
        private SpriteMap _orbiter;
        private float _rotSpeed = Rando.Float(0.01f, 0.02f);
        private float _distPulseSpeed = Rando.Float(0.01f, 0.02f);
        private float _distPulse = Rando.Float(5f);
        private float s1 = 1f;
        private float s2 = 1f;
        private bool didRemove;
        private float _groundedTime;
        //private float lifeTake = 0.05f;

        public int smokeID => this._smokeID;

        public ExtinguisherSmoke(float xpos, float ypos, bool network = false)
          : base(xpos, ypos)
        {
            this.center = new Vec2(8f, 8f);
            this.hSpeed = Rando.Float(-0.2f, 0.2f);
            this.vSpeed = Rando.Float(-0.2f, 0.2f);
            this._life += Rando.Float(0.2f);
            this.angleDegrees = Rando.Float(360f);
            this._gravMult = 0.8f;
            this._sticky = 0.2f;
            this._life = 3f;
            this._bounceEfficiency = 0.2f;
            this.xscale = this.yscale = Rando.Float(0.4f, 0.5f);
            this._smokeID = FireManager.GetFireID();
            this._collisionSize = new Vec2(4f, 4f);
            this._collisionOffset = new Vec2(-2f, -2f);
            this.needsSynchronization = true;
            this._sprite = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num1 = Rando.Int(3) * 4;
            this._sprite.AddAnimation("idle", 0.1f, true, num1);
            this._sprite.AddAnimation("puff", Rando.Float(0.15f, 0.25f), false, num1, 1 + num1, 2 + num1, 3 + num1);
            this._orbiter = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num2 = Rando.Int(3) * 4;
            this._orbiter.AddAnimation("idle", 0.1f, true, num2);
            this._orbiter.AddAnimation("puff", Rando.Float(0.15f, 0.25f), false, num2, 1 + num2, 2 + num2, 3 + num2);
            this._sprite2 = new SpriteMap("tinySmokeTestBack", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            if (Network.isActive && !network)
                GhostManager.context.particleManager.AddLocalParticle(this);
            this.isLocal = !network;
            this._orbitInc += 0.2f;
            this._sprite.SetAnimation("idle");
            this._sprite.angleDegrees = Rando.Float(360f);
            this._orbiter.angleDegrees = Rando.Float(360f);
            this.s1 = Rando.Float(0.8f, 1.1f);
            this.s2 = Rando.Float(0.8f, 1.1f);
            float num3 = 0.6f - Rando.Float(0.2f);
            float num4 = 1f;
            this._sprite.color = new Color(num4, num4, num4);
            this.depth = (Depth)0.8f;
            this.alpha = 1f;
            this.layer = Layer.Game;
            this.s1 = this.xscale;
            this.s2 = this.xscale;
        }

        public override void Removed()
        {
            if (Network.isActive && !this.didRemove && this.isLocal && GhostManager.context != null)
            {
                this.didRemove = true;
                GhostManager.context.particleManager.RemoveParticle(this);
            }
            base.Removed();
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            this._moveWave.Update();
            this._moveWave2.Update();
            this._orbitInc += this._rotSpeed;
            this._distPulse += this._distPulseSpeed;
            if (_life < 0.300000011920929)
                this.xscale = this.yscale = Maths.LerpTowards(this.xscale, 0.1f, 0.015f);
            else if (this._grounded)
                this.xscale = this.yscale = Maths.LerpTowards(this.xscale, this._fullScale, 0.01f);
            else
                this.xscale = this.yscale = Maths.LerpTowards(this.xscale, this._fullScale * 0.8f, 0.04f);
            this.s1 = this.xscale;
            this.s2 = this.xscale;
            if (!this.isLocal)
            {
                base.Update();
            }
            else
            {
                if (this._grounded)
                {
                    this._groundedTime += 0.01f;
                    ExtinguisherSmoke extinguisherSmoke = Level.CheckCircle<ExtinguisherSmoke>(new Vec2(this.x, this.y + 4f), 6f);
                    if (extinguisherSmoke != null && _groundedTime < extinguisherSmoke._groundedTime - 0.100000001490116)
                        extinguisherSmoke.y -= 0.1f;
                }
                if (_life < 0.0 && this._sprite.currentAnimation != "puff")
                    this._sprite.SetAnimation("puff");
                if (this._sprite.currentAnimation == "puff" && this._sprite.finished)
                    Level.Remove(this);
                base.Update();
            }
        }

        public override void Draw()
        {
            float num1 = (float)Math.Sin(_distPulse);
            float num2 = (float)-(Math.Sin(_orbitInc) * (double)num1) * this.s1;
            float num3 = (float)Math.Cos(_orbitInc) * num1 * this.s1;
            this._sprite.imageIndex = this._sprite.imageIndex;
            this._sprite.depth = this.depth;
            this._sprite.scale = new Vec2(this.s1);
            this._sprite.center = this.center;
            Graphics.Draw(_sprite, this.x + num2, this.y + num3);
            this._sprite2.imageIndex = this._sprite.imageIndex;
            this._sprite2.angle = this._sprite.angle;
            this._sprite2.depth = - 0.5f;
            this._sprite2.scale = this._sprite.scale;
            this._sprite2.center = this.center;
            float num4 = 0.6f - Rando.Float(0.2f);
            float num5 = 0.4f;
            this._sprite2.color = new Color(num5, num5, num5);
            Graphics.Draw(_sprite2, this.x + num2, this.y + num3);
            this._orbiter.imageIndex = this._sprite.imageIndex;
            this._orbiter.color = this._sprite.color;
            this._orbiter.depth = this.depth;
            this._orbiter.scale = new Vec2(this.s2);
            this._orbiter.center = this.center;
            Graphics.Draw(_orbiter, this.x - num2, this.y - num3);
            this._sprite2.imageIndex = this._orbiter.imageIndex;
            this._sprite2.angle = this._orbiter.angle;
            this._sprite2.depth = - 0.5f;
            this._sprite2.scale = this._orbiter.scale;
            this._sprite2.center = this.center;
            this._sprite2.color = new Color(num5, num5, num5);
            Graphics.Draw(_sprite2, this.x - num2, this.y - num3);
        }
    }
}

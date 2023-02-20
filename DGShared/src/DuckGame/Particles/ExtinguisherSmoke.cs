// Decompiled with JetBrains decompiler
// Type: DuckGame.ExtinguisherSmoke
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        private static int colorindex;
        private int _colorindex;
        public Color edgecolor = new Color(0.4f, 0.4f, 0.4f);
        public Color maincolor = Color.White;
        //private float lifeTake = 0.05f;

        public int smokeID => _smokeID;

        public ExtinguisherSmoke(float xpos, float ypos, bool network = false)
          : base(xpos, ypos)
        {
            center = new Vec2(8f, 8f);
            hSpeed = Rando.Float(-0.2f, 0.2f);
            vSpeed = Rando.Float(-0.2f, 0.2f);
            _life += Rando.Float(0.2f);
            angle = Rando.Float(6.28319f);
            _gravMult = 0.8f;
            _sticky = 0.2f;
            _life = 3f;
            _bounceEfficiency = 0.2f;
            xscale = yscale = Rando.Float(0.4f, 0.5f);
            _smokeID = FireManager.GetFireID();
            _collisionSize = new Vec2(4f, 4f);
            _collisionOffset = new Vec2(-2f, -2f);
            needsSynchronization = true;
            _sprite = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num1 = Rando.Int(3) * 4;
            _sprite.AddAnimation("idle", 0.1f, true, num1);
            _sprite.AddAnimation("puff", Rando.Float(0.15f, 0.25f), false, num1, 1 + num1, 2 + num1, 3 + num1);
            _orbiter = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num2 = Rando.Int(3) * 4;
            _orbiter.AddAnimation("idle", 0.1f, true, num2);
            _orbiter.AddAnimation("puff", Rando.Float(0.15f, 0.25f), false, num2, 1 + num2, 2 + num2, 3 + num2);
            _sprite2 = new SpriteMap("tinySmokeTestBack", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            if (Network.isActive && !network)
                GhostManager.context.particleManager.AddLocalParticle(this);
            isLocal = !network;
            _orbitInc += 0.2f;
            _sprite.SetAnimation("idle");
            _sprite.angle = Rando.Float(6.28319f);
            _orbiter.angle = Rando.Float(6.28319f);
            s1 = Rando.Float(0.8f, 1.1f);
            s2 = Rando.Float(0.8f, 1.1f);
            float num3 = 0.6f - Rando.Float(0.2f);
            float num4 = 1f;
            _sprite.color = new Color(num4, num4, num4);
            depth = (Depth)0.8f;
            alpha = 1f;
            layer = Layer.Game;
            s1 = xscale;
            s2 = xscale;
            if (Program.gay) //Program.gay
            {
                _colorindex = colorindex;
                Color selectedcolor = Colors.Rainbow[colorindex];
                edgecolor = new Color((byte)(selectedcolor.r / 2.5f), (byte)(selectedcolor.g / 2.5f), (byte)(selectedcolor.b / 2.5f));
                maincolor = selectedcolor;
                colorindex += 1;
                if (colorindex >= Colors.Rainbow.Length)
                {
                    colorindex = 0;
                }
                _orbiter.color = edgecolor;
                _sprite.color = maincolor;
            }
        }

        public override void Removed()
        {
            if (Network.isActive && !didRemove && isLocal && GhostManager.context != null)
            {
                didRemove = true;
                GhostManager.context.particleManager.RemoveParticle(this);
            }
            base.Removed();
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            _moveWave.Update();
            _moveWave2.Update();
            _orbitInc += _rotSpeed;
            _distPulse += _distPulseSpeed;
            if (_life < 0.3f)
                xscale = yscale = Maths.LerpTowards(xscale, 0.1f, 0.015f);
            else if (_grounded)
                xscale = yscale = Maths.LerpTowards(xscale, _fullScale, 0.01f);
            else
                xscale = yscale = Maths.LerpTowards(xscale, _fullScale * 0.8f, 0.04f);
            s1 = xscale;
            s2 = xscale;
            if (!isLocal)
            {
                base.Update();
            }
            else
            {
                if (_grounded)
                {
                    _groundedTime += 0.01f;
                    ExtinguisherSmoke extinguisherSmoke = Level.CheckCircle<ExtinguisherSmoke>(new Vec2(x, y + 4f), 6f);
                    if (extinguisherSmoke != null && _groundedTime < extinguisherSmoke._groundedTime - 0.1f)
                        extinguisherSmoke.y -= 0.1f;
                }
                if (_life < 0f && _sprite.currentAnimation != "puff")
                    _sprite.SetAnimation("puff");
                if (_sprite.currentAnimation == "puff" && _sprite.finished)
                    Level.Remove(this);
                base.Update();
            }
        }

        public override void Draw()
        {
            if (Program.nikogay)
            {
                Color selectedcolor = Colors.Rainbow[_colorindex];
                edgecolor = new Color((byte)(selectedcolor.r / 2.5f), (byte)(selectedcolor.g / 2.5f), (byte)(selectedcolor.b / 2.5f));
                maincolor = selectedcolor;
                _orbiter.color = edgecolor;
                _sprite.color = maincolor;
            }
            float num1 = (float)Math.Sin(_distPulse);
            float num2 = (float)-(Math.Sin(_orbitInc) * num1) * s1;
            float num3 = (float)Math.Cos(_orbitInc) * num1 * s1;
            _sprite.imageIndex = _sprite.imageIndex;
            _sprite.depth = depth;
            _sprite.scale = new Vec2(s1);
            _sprite.center = center;
            Graphics.Draw(_sprite, x + num2, y + num3);
            _sprite2.imageIndex = _sprite.imageIndex;
            _sprite2.angle = _sprite.angle;
            _sprite2.depth = -0.5f;
            _sprite2.scale = _sprite.scale;
            _sprite2.center = center;
            _sprite2.color = edgecolor;
            Graphics.Draw(_sprite2, x + num2, y + num3);
            _orbiter.imageIndex = _sprite.imageIndex;
            _orbiter.color = _sprite.color;
            _orbiter.depth = depth;
            _orbiter.scale = new Vec2(s2);
            _orbiter.center = center;
            Graphics.Draw(_orbiter, x - num2, y - num3);
            _sprite2.imageIndex = _orbiter.imageIndex;
            _sprite2.angle = _orbiter.angle;
            _sprite2.depth = -0.5f;
            _sprite2.scale = _orbiter.scale;
            _sprite2.center = center;
            _sprite2.color = edgecolor;
            Graphics.Draw(_sprite2, x - num2, y - num3);
        }
    }
}

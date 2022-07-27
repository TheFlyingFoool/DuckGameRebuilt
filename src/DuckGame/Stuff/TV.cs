// Decompiled with JetBrains decompiler
// Type: DuckGame.TV
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    public class TV : Holdable, IPlatform
    {
        private SpriteMap _sprite;
        private Sprite _frame;
        private Sprite _damaged;
        public StateBinding _ruinedBinding = new StateBinding("_ruined");
        public StateBinding _channelBinding = new StateBinding(nameof(channel));
        private float _ghostWait = 1f;
        private bool _madeGhost;
        public bool channel;
        private int _switchFrames;
        private Sprite _rainbow;
        private Cape _cape;
        public bool jumpReady;
        private List<Vec2> trail = new List<Vec2>();
        private SpriteMap _channels;
        private SpriteMap _tvNoise;
        //private int wait;
        private bool fakeGrounded;
        private float prevVSpeed;

        public TV(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("plasma2", 16, 16)
            {
                speed = 0.2f
            };
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -7f);
            this.collisionSize = new Vec2(16f, 14f);
            this.depth = - 0.5f;
            this._editorName = nameof(TV);
            this.thickness = 2f;
            this.weight = 5f;
            this.holsterAngle = 0.0f;
            this.flammable = 0.3f;
            this._frame = new Sprite("tv2");
            this._frame.CenterOrigin();
            this._damaged = new Sprite("tvBroken");
            this._damaged.CenterOrigin();
            this._holdOffset = new Vec2(2f, 0.0f);
            this._breakForce = 4f;
            this.collideSounds.Add("landTV");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._channels = new SpriteMap("channels", 8, 6)
            {
                depth = this.depth + 5
            };
            this._tvNoise = new SpriteMap("tvnoise", 8, 6);
            this._tvNoise.AddAnimation("noise", 0.6f, true, 0, 1, 2);
            this._tvNoise.currentAnimation = "noise";
            this._rainbow = new Sprite("rainbowGradient");
            this.editorTooltip = "Your source for breaking Duck Channel news.";
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                this._cape = new Cape(this.x, this.y, this, true);
                this._cape.metadata.CapeIsTrail.value = true;
                this._cape._capeTexture = new Sprite("rainbowCarp").texture;
                Level.Add(_cape);
            }
            base.Initialize();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!this.isServerForObject || type.thing != null && type.thing is Duck || this._ruined)
                return false;
            this._ruined = true;
            this.graphic = this._damaged;
            SFX.Play("breakTV");
            for (int index = 0; index < 8; ++index)
                Level.Add(new GlassParticle(this.x + Rando.Float(-8f, 8f), this.y + Rando.Float(-8f, 8f), new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
            this.collideSounds.Clear();
            this.collideSounds.Add("deadTVLand");
            this._sendDestroyMessage = true;
            return false;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            if (!this.isServerForObject || this._ruined || !bullet.isLocal)
                return base.Hit(bullet, hitPos);
            this.OnDestroy(new DTShot(bullet));
            return base.Hit(bullet, hitPos);
        }

        public override void Update()
        {
            if (this._switchFrames > 0)
                --this._switchFrames;
            if (this._ruined)
            {
                if (this._cape != null)
                {
                    Level.Remove(_cape);
                    this._cape = null;
                }
                this.graphic = this._damaged;
                if (_ghostWait > 0.0)
                {
                    this._ghostWait -= 0.4f;
                }
                else
                {
                    if (!this._madeGhost)
                    {
                        Level.Add(new EscapingGhost(this.x, this.y - 6f));
                        for (int index = 0; index < 8; ++index)
                            Level.Add(Spark.New(this.x + Rando.Float(-8f, 8f), this.y + Rando.Float(-8f, 8f), new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
                    }
                    this._madeGhost = true;
                }
            }
            Duck duck = this.duck;
            if (this._owner is SpikeHelm && this._owner.owner is Duck)
                duck = this._owner.owner as Duck;
            if (duck != null)
            {
                if ((double)duck.vSpeed < -1.0 && prevVSpeed > 0.0 && !duck.tvJumped)
                    this.fakeGrounded = true;
                this.jumpReady = this.jumpReady || duck.grounded || this.fakeGrounded || duck._vine != null;
                this.prevVSpeed = duck.vSpeed;
            }
            this.fakeGrounded = false;
            base.Update();
        }

        public override void OnTeleport()
        {
            this.fakeGrounded = true;
            base.OnTeleport();
        }

        [NetworkAction]
        public void SwitchChannelEffect()
        {
            this._switchFrames = 8;
            SFX.Play("switchchannel", 0.7f, 0.5f);
        }

        public override void OnPressAction()
        {
            if (!this._ruined)
            {
                this.channel = !this.channel;
                this.SyncNetworkAction(new PhysicsObject.NetAction(this.SwitchChannelEffect));
            }
            base.OnPressAction();
        }

        public override void Draw()
        {
            base.Draw();
            if (this._ruined)
                return;
            this._frame.angle = this._channels.angle = this._tvNoise.angle = this.angle;
            this._frame.flipH = this._channels.flipH = this._tvNoise.flipH = this.offDir < 0;
            this._frame.depth = this.depth + 1;
            Graphics.Draw(this._frame, this.x, this.y);
            this._channels.alpha = Lerp.Float(this._channels.alpha, this.owner != null ? 1f : 0.0f, 0.1f);
            this._channels.depth = this.depth + 4;
            this._channels.frame = this.channel ? (this.jumpReady ? 1 : 2) : 0;
            Vec2 vec2 = this.Offset(new Vec2(-4f, -4f));
            Graphics.Draw(_channels, vec2.x, vec2.y);
            if (this.owner != null)
            {
                Vec2 p1 = Vec2.Zero;
                bool flag = false;
                foreach (Vec2 p2 in this.trail)
                {
                    if (!flag)
                        flag = true;
                    else
                        Graphics.DrawTexturedLine(this._rainbow.texture, p1, p2, Color.White, depth: (this.depth - 10));
                    p1 = p2;
                }
            }
            if (this._switchFrames > 0)
                this._tvNoise.alpha = 1f;
            else
                this._tvNoise.alpha = 0.2f;
            this._tvNoise.depth = this.depth + 8;
            Graphics.Draw(_tvNoise, vec2.x, vec2.y);
        }
    }
}

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
            _sprite = new SpriteMap("plasma2", 16, 16)
            {
                speed = 0.2f
            };
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -7f);
            collisionSize = new Vec2(16f, 14f);
            depth = -0.5f;
            _editorName = nameof(TV);
            thickness = 2f;
            weight = 5f;
            holsterAngle = 0f;
            flammable = 0.3f;
            _frame = new Sprite("tv2");
            _frame.CenterOrigin();
            _damaged = new Sprite("tvBroken");
            _damaged.CenterOrigin();
            _holdOffset = new Vec2(2f, 0f);
            _breakForce = 4f;
            collideSounds.Add("landTV");
            physicsMaterial = PhysicsMaterial.Metal;
            _channels = new SpriteMap("channels", 8, 6)
            {
                depth = depth + 5
            };
            _tvNoise = new SpriteMap("tvnoise", 8, 6);
            _tvNoise.AddAnimation("noise", 0.6f, true, 0, 1, 2);
            _tvNoise.currentAnimation = "noise";
            _rainbow = new Sprite("rainbowGradient");
            editorTooltip = "Your source for breaking Duck Channel news.";
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                _cape = new Cape(x, y, this, true);
                _cape.metadata.CapeIsTrail.value = true;
                _cape._capeTexture = new Sprite("rainbowCarp").texture;
                Level.Add(_cape);
            }
            base.Initialize();
        }

        public bool playedBroken;
        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!isServerForObject || (type.thing != null && type.thing is Duck)) return false;
            if (!_ruined)
            {
                _ruined = true;
                graphic = _damaged;
                playedBroken = true;
                SFX.Play("breakTV", 1f, 0f, 0f, false);
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index) Level.Add(new GlassParticle(x + Rando.Float(-8f, 8f), y + Rando.Float(-8f, 8f), new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
                collideSounds.Clear();
                collideSounds.Add("deadTVLand");
                _sendDestroyMessage = true;
            }
            return false;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            if (!isServerForObject || _ruined || !bullet.isLocal)
                return base.Hit(bullet, hitPos);
            OnDestroy(new DTShot(bullet));
            return base.Hit(bullet, hitPos);
        }

        public override void Update()
        {
            if (_ruined && !playedBroken)
            {
                playedBroken = true;
                SFX.Play("breakTV", 1f, 0f, 0f, false);
                collideSounds.Clear();
                collideSounds.Add("deadTVLand");
            }
            if (_switchFrames > 0)
                --_switchFrames;
            if (_ruined)
            {
                if (_cape != null)
                {
                    Level.Remove(_cape);
                    _cape = null;
                }
                graphic = _damaged;
                if (_ghostWait > 0)
                {
                    _ghostWait -= 0.4f;
                }
                else
                {
                    if (!_madeGhost)
                    {
                        if (DGRSettings.ActualParticleMultiplier > 0)
                        {
                            Level.Add(new EscapingGhost(x, y - 6f));
                            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index)
                                Level.Add(Spark.New(x + Rando.Float(-8f, 8f), y + Rando.Float(-8f, 8f), new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
                        }
                    }
                    _madeGhost = true;
                }
            }
            Duck duck = this.duck;
            if (_owner is SpikeHelm && _owner.owner is Duck)
                duck = _owner.owner as Duck;
            if (duck != null)
            {
                if (duck.vSpeed < -1 && prevVSpeed > 0 && !duck.tvJumped)
                    fakeGrounded = true;
                jumpReady = jumpReady || duck.grounded || fakeGrounded || duck._vine != null;
                prevVSpeed = duck.vSpeed;
            }
            fakeGrounded = false;
            base.Update();
        }

        public override void OnTeleport()
        {
            fakeGrounded = true;
            base.OnTeleport();
        }

        [NetworkAction]
        public void SwitchChannelEffect()
        {
            _switchFrames = 8;
            SFX.Play("switchchannel", 0.7f, 0.5f);
        }

        public override void OnPressAction()
        {
            if (!_ruined)
            {
                channel = !channel;
                SyncNetworkAction(new NetAction(SwitchChannelEffect));
            }
            base.OnPressAction();
        }

        public override void Draw()
        {
            base.Draw();
            if (_ruined)
                return;
            _frame.angle = _channels.angle = _tvNoise.angle = angle;
            _frame.flipH = _channels.flipH = _tvNoise.flipH = offDir < 0;
            _frame.depth = depth + 1;
            _frame.SkipIntraTick = SkipIntratick;
            Graphics.Draw(ref _frame, x, y);
            _channels.alpha = Lerp.Float(_channels.alpha, owner != null ? 1f : 0f, 0.1f);
            _channels.depth = depth + 4;
            _channels.frame = channel ? (jumpReady ? 1 : 2) : 0;
            _channels.SkipIntraTick = SkipIntratick;
            Vec2 vec2 = Offset(new Vec2(-4f, -4f));
            Graphics.Draw(ref _channels, vec2.x, vec2.y);
            if (owner != null)
            {
                Vec2 p1 = Vec2.Zero;
                bool flag = false;
                foreach (Vec2 p2 in trail)
                {
                    if (!flag)
                        flag = true;
                    else
                        Graphics.DrawTexturedLine(_rainbow.texture, p1, p2, Color.White, depth: (depth - 10));
                    p1 = p2;
                }
            }
            if (_switchFrames > 0)
                _tvNoise.alpha = 1f;
            else
                _tvNoise.alpha = 0.2f;
            _tvNoise.depth = depth + 8;
            _tvNoise.SkipIntraTick = SkipIntratick;
            Graphics.Draw(ref _tvNoise, vec2.x, vec2.y);
        }
    }
}

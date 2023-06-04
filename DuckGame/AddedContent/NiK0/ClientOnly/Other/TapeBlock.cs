namespace DuckGame
{
    [ClientOnly]
#if DEBUG
    [EditorGroup("Rebuilt|Stuff")]
#endif
    public class TapeBlock : ItemBox, IDrawToDifferentLayers
    {
        private Sprite _scanner;
        private Sprite _projector;
        private Sprite _none;
        private Sprite _projectorGlitch;
        private SinWave _wave = (SinWave)1f;
        private SinWave _projectionWave = (SinWave)0.04f;
        private SinWave _projectionWave2 = (SinWave)0.05f;
        private SinWave _projectionFlashWave = (SinWave)0.8f;
        private float _projectorAlpha;
        private bool _useWave;
        public static Material _grayscale = new Material("Shaders/greyscale");
        public StateBinding leftStoreBinding = new StateBinding("leftStore");
        public StateBinding rightStoreBinding = new StateBinding("rightStore");

        public float _double;
        private float _glitch;
        private bool _alternate;
        public TapeBlock(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("TapeBlock");
            center = new Vec2(16, 8);
            collisionSize = new Vec2(32, 16);
            _collisionOffset = new Vec2(-16, -8);

            editorOffset = new Vec2(8, 0);

            _scanner = new Sprite("tapeScanner")
            {
                center = new Vec2(4f, 1f),
                xscale = 1.5f,
                alpha = 0.7f,
                depth = (Depth)0.9f
            };
            _projector = new Sprite("purpleProjector")
            {
                center = new Vec2(8f, 16f),
                xscale = 1.5f,
                alpha = 0.7f,
                depth = (Depth)0.9f
            };
            _none = new Sprite("none")
            {
                center = new Vec2(8f, 8f),
                alpha = 0.7f
            };
            _projectorGlitch = new Sprite("projectorGlitch")
            {
                center = new Vec2(8f, 8f),
                alpha = 0.7f,
                depth = (Depth)0.91f
            };
            _editorName = "Tape Block";
            editorTooltip = "Put 2 and 2 together and you might end with a weapon of mass destruction! or a bomb.";
        }
        public override void Update()
        {
            _alternate = !_alternate;
            _scanner.alpha = (float)(0.4f + _wave.normalized * 0.6f);
            _projector.alpha = (float)(0.4f + _wave.normalized * 0.6f) * _projectorAlpha;
            _double = Maths.CountDown(_double, 0.15f);
            _glitch = Maths.CountDown(_glitch, 0.1f);
            if (Rando.Float(1f) < 0.01f)
            {
                _glitch = 0.3f;
                _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
            }
            if (Rando.Float(1f) < 0.005f)
            {
                _glitch = 0.3f;
                _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
                _useWave = !_useWave;
            }
            if (Rando.Float(1f) < 0.008f)
            {
                _glitch = 0.3f;
                _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
                _useWave = !_useWave;
                _double = 0.6f + Rando.Float(0.6f);
            }


            _glitch = 0.3f;
            _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
            _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
            _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
            _useWave = !_useWave;
            //_double = 0.6f + Rando.Float(0.6f);

            _projectorAlpha = Maths.CountUp(_projectorAlpha, 0.1f);


            _projectorGlitch.alpha = _glitch * _projectorAlpha;

            if (charging > 0)
            {
                alpha = Lerp.Float(alpha, 0.6f, 0.03f);
            }
            else
            {
                if (alpha != 1) SFX.Play("tapeBeep");
                alpha = 1;
            }

            base.Update();
        }
        public void OnDrawLayer(Layer pLayer)
        {
            if (pLayer != Layer.Background)
                return;

            if (_alternate)
                Graphics.Draw(_scanner, x, y + 9f);
            if (!_alternate)
                Graphics.Draw(_projector, x, y - 8f, -1);
            float num = (float)(_useWave ? _projectionWave : _projectionWave2);

            //weird workaround so you cannot tape invis stuff because duct tape can still tape non active and non visible shit for some reason -NiK0
            if (leftStore != null) leftStore.tapeable = false;
            if (rightStore != null) rightStore.tapeable = false;

            if (_double > 0f)
            {
                if (leftStore != null)
                {
                    Duck.renderingIcon = true;
                    Material material = Graphics.material;
                    Graphics.material = _grayscale;
                    leftStore.depth = depth - 5;
                    leftStore.x = x - _double * 2f - 10;
                    leftStore.y = y - 16f - num;
                    leftStore.Draw();
                    leftStore.x = x + _double * 2f - 10;
                    leftStore.y = y - 16f - num;
                    leftStore.Draw();
                    leftStore.position = new Vec2(-2000);
                    Graphics.material = material;
                    Duck.renderingIcon = false;
                }
                else
                {
                    _none.alpha = (0.2f + _projectionFlashWave.normalized * 0.2f + _glitch * 1f) * _projectorAlpha;
                    Graphics.Draw(_none, x - _double * 2f, y - 16f - num);
                    Graphics.Draw(_none, x + _double * 2f, y - 16f - num);
                }
            }
            else if (leftStore != null)
            {
                Duck.renderingIcon = true;
                Material material = Graphics.material;
                Graphics.material = _grayscale;
                leftStore.depth = depth - 5;
                leftStore.x = x - 10;
                leftStore.y = y - 16f - num;
                leftStore.Draw();
                leftStore.position = new Vec2(-2000);
                Graphics.material = material;
                Duck.renderingIcon = false;
            }
            else Graphics.Draw(_none, x - 10, y - 16f - num);


            if (_double > 0f)
            {
                if (rightStore != null)
                {
                    Duck.renderingIcon = true;
                    Material material = Graphics.material;
                    Graphics.material = _grayscale;
                    rightStore.depth = depth - 5;
                    rightStore.x = x - _double * 2f + 10;
                    rightStore.y = y - 16f - num;
                    rightStore.Draw();
                    rightStore.x = x + _double * 2f + 10;
                    rightStore.y = y - 16f - num;
                    rightStore.Draw();
                    rightStore.position = new Vec2(-2000);
                    Graphics.material = material;
                    Duck.renderingIcon = false;
                }
                else
                {
                    _none.alpha = (0.2f + _projectionFlashWave.normalized * 0.2f + _glitch * 1f) * _projectorAlpha;
                    Graphics.Draw(_none, x - _double * 2f, y - 16f - num);
                    Graphics.Draw(_none, x + _double * 2f, y - 16f - num);
                }
            }
            else if (rightStore != null)
            {
                Duck.renderingIcon = true;
                Material material = Graphics.material;
                Graphics.material = _grayscale;
                rightStore.depth = depth - 5;
                rightStore.x = x + 10;
                rightStore.y = y - 16f - num;
                rightStore.Draw();
                rightStore.position = new Vec2(-2000);
                Graphics.material = material;
                Duck.renderingIcon = false;
            }
            else Graphics.Draw(_none, x + 10, y - 16f - num);

            if (_glitch <= 0f)
                return;
            Graphics.Draw(_projectorGlitch, x, y - 16f);
        }

        public Holdable leftStore;
        public Holdable rightStore;
        public void StoreItem(Thing t, bool left)
        {
            switch (t)
            {
                case RagdollPart _:
                case TrappedDuck _:
                case WeightBall _:
                case Holdable _ when !(t as Holdable).canStore || !(t as Holdable).tapeable || t is TapedGun:
                    return;
            }
            if (left)
            {
                if (leftStore == null)
                {
                    leftStore = (Holdable)LoadThing(t.Serialize());
                    if (Network.isActive)
                    {
                        leftStore.position = new Vec2(-2000);
                        leftStore.active = false;
                        leftStore.visible = false;
                        Level.Add(leftStore);
                    }
                    SFX.PlaySynchronized("scanBeep", 1, 0.2f);
                }
                else
                {
                    SFX.PlaySynchronized("scanFail", 1, 0.2f);

                }
            }
            else
            {
                if (rightStore == null)
                {
                    rightStore = (Holdable)LoadThing(t.Serialize());
                    if (Network.isActive)
                    {
                        rightStore.position = new Vec2(-2000);
                        rightStore.active = false;
                        rightStore.visible = false;
                        Level.Add(rightStore);
                    }
                    SFX.PlaySynchronized("scanBeep", 1, 0.2f);
                }
                else
                {
                    SFX.PlaySynchronized("scanFail", 1, 0.2f);
                     
                }
            }
        }
        public override PhysicsObject GetSpawnItem()
        {
            if (leftStore == null || rightStore == null) return null;
            TapedGun tp = new TapedGun(0, 0);

            //this is not a great way to do it but idc
            tp.gun1 = (Holdable)LoadThing(leftStore.Serialize());
            tp.gun2 = (Holdable)LoadThing(rightStore.Serialize());

            Holdable tms = tp.gun1.BecomeTapedMonster(tp);
            if (tms != null)
            {
                return tms;
            }

            return tp;
        }
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (from == ImpactedFrom.Bottom && with.isServerForObject)
                with.Fondle(this);
            if (!isServerForObject || !with.isServerForObject || from != ImpactedFrom.Bottom)
                return;
            if (leftStore != null && rightStore != null)
            {
                if (!Network.isActive)
                {
                    containContext = GetSpawnItem();
                }
                Pop();
            }
            else
            {
                switch (with)
                {
                    case Holdable holdable when holdable.lastThrownBy != null || holdable is RagdollPart && !Network.isActive:
                        Duck lastThrownBy = holdable.lastThrownBy as Duck;
                        if (holdable is RagdollPart)
                            break;
                        StoreItem(with, holdable.x < x);
                        Bounce();
                        break;
                    case Duck pDuck:
                        Bounce();
                        if (pDuck.holdObject != null)
                        {
                            StoreItem(pDuck.holdObject, pDuck.x < x);
                        }
                        break;
                }
            }
        }
    }
}

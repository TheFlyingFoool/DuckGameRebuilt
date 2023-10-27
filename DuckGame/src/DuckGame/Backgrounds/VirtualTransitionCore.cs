using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class VirtualTransitionCore
    {
        private Sprite _scanner;
        private BitmapFont _smallBios;
        private BackgroundUpdater _realBackground;
        public int _scanStage = -1;
        public float _stick;
        public bool _fullyVirtual = true;
        public bool _fullyNonVirtual;
        public bool _virtualMode;
        public bool _visible = true;
        private ParallaxBackground _parallax;
        private Color _backgroundColor;
        protected float _lastCameraX;
        private Rectangle _scissor = new Rectangle(0f, 0f, 0f, 0f);
        private bool _done = true;
        private bool _incStage;
        private bool _decStage;
        private Color _curBackgroundColor;
        public Level _transitionLevel;
        private Vec2 _position;

        public void Initialize()
        {
            _fullyVirtual = false;
            _fullyNonVirtual = false;
            _virtualMode = false;
            _smallBios = new BitmapFont("smallBiosFont", 7, 6);
            _parallax = new ParallaxBackground("background/virtual", 0f, 0f, 3);
            float speed1 = 0.4f;
            float distance1 = 0.8f;
            _parallax.AddZone(0, distance1, speed1);
            _parallax.AddZone(1, distance1, speed1);
            _parallax.AddZone(2, distance1, speed1);
            _parallax.AddZone(3, distance1, speed1);
            float distance2 = 0.6f;
            float num = (float)((distance1 - distance2) / 4f);
            float speed2 = 0.6f;
            _parallax.AddZone(4, distance1 - num * 1f, speed2, true);
            _parallax.AddZone(5, distance1 - num * 2f, -speed2, true);
            _parallax.AddZone(6, distance1 - num * 3f, speed2, true);
            _parallax.AddZone(7, distance2, speed1);
            _parallax.AddZone(8, distance2, speed1);
            _parallax.AddZone(19, distance2, speed1);
            _parallax.AddZone(20, distance2, speed1);
            _parallax.AddZone(21, distance1 - num * 3f, -speed2, true);
            _parallax.AddZone(22, distance1 - num * 2f, speed2, true);
            _parallax.AddZone(23, distance1 - num * 1f, -speed2, true);
            _parallax.AddZone(24, distance1, speed1);
            _parallax.AddZone(25, distance1, speed1);
            _parallax.AddZone(26, distance1, speed1);
            _parallax.AddZone(27, distance1, speed1);
            _parallax.AddZone(28, distance1, speed1);
            _parallax.AddZone(29, distance1, speed1);
            _parallax.AddZone(30, distance1, speed1);
            _parallax.AddZone(31, distance1, speed1);
            _parallax.AddZone(32, distance1, speed1);
            _parallax.AddZone(33, distance1, speed1);
            _parallax.AddZone(34, distance1, speed1);
            _parallax.restrictBottom = false;
            _visible = true;
            _parallax.y = 0f;
            _scanner = new Sprite("background/scanbeam");
            _backgroundColor = Color.Black;
            _parallax.layer = Layer.Virtual;
        }

        public void SetVisible(bool vis)
        {
            _parallax.scissor = _scissor;
            _parallax.visible = vis;
            if (_scissor.width == 0f)
                return;
            _parallax.layer.scissor = _scissor;
        }

        public bool doingVirtualTransition => _virtualMode && !_done;

        public void GoVirtual()
        {
            if (_virtualMode)
                return;
            _scanStage = 2;
            _stick = 1f;
            _virtualMode = true;
            _done = false;
            if (_realBackground == null)
            {
                using (IEnumerator<Thing> enumerator = Level.activeLevel.things[typeof(BackgroundUpdater)].GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        BackgroundUpdater current = (BackgroundUpdater)enumerator.Current;
                        _realBackground = current;
                        _curBackgroundColor = current.backgroundColor;
                    }
                }
            }
            _transitionLevel = Level.activeLevel;
        }

        public void GoUnVirtual()
        {
            if (!_virtualMode)
                return;
            _realBackground = null;
            _virtualMode = false;
            _scanStage = 0;
            _stick = 0f;
            _done = false;
        }

        public bool active => !_done;

        public void Update()
        {
            if (_done && !Level.current._waitingOnTransition)
            {
                Layer.doVirtualEffect = false;
                if (_realBackground == null)
                    return;
                Level.activeLevel.backgroundColor = _realBackground.backgroundColor;
                _realBackground.scissor = new Rectangle(0f, 0f, Resolution.current.x, Resolution.current.y);
                _realBackground = null;
            }
            else
            {
                if (Level.current._waitingOnTransition)
                    _realBackground = null;
                if (_realBackground == null)
                {
                    using (IEnumerator<Thing> enumerator = Level.activeLevel.things[typeof(BackgroundUpdater)].GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                            _realBackground = (BackgroundUpdater)enumerator.Current;
                    }
                }
                float num = _stick;
                if (_scanStage == 2 && _virtualMode)
                {
                    _backgroundColor = _curBackgroundColor;
                    Level.activeLevel.backgroundColor = Lerp.ColorSmoothNoAlpha(_backgroundColor, _curBackgroundColor, _stick);
                    Layer.Glow.fade = Lerp.FloatSmooth(Layer.Glow.fade, 0f, _stick);
                }
                if (_scanStage == 0 && !_virtualMode && _realBackground != null)
                {
                    Level.activeLevel.backgroundColor = Lerp.ColorSmoothNoAlpha(_backgroundColor, _realBackground.backgroundColor, _stick);
                    Layer.Glow.fade = Lerp.FloatSmooth(Layer.Glow.fade, 1f, _stick);
                }
                if (_scanStage == -1)
                    Level.activeLevel.backgroundColor = Lerp.ColorSmoothNoAlpha(_backgroundColor, Color.Black, 0.1f);
                if (_scanStage < 2)
                    num = 0f;
                Rectangle rectangle1 = new Rectangle((int)((1f - num) * Resolution.current.x), 0f, Resolution.current.x - (int)((1f - num) * Resolution.current.x), Resolution.current.y);
                if (_realBackground != null)
                {
                    if (rectangle1.width == 0f)
                    {
                        _realBackground.SetVisible(false);
                    }
                    else
                    {
                        _realBackground.scissor = rectangle1;
                        _realBackground.SetVisible(true);
                    }
                }
                Rectangle rectangle2 = new Rectangle(0f, 0f, Resolution.current.x - rectangle1.width, Resolution.current.y);
                if (rectangle2.width == 0f)
                {
                    SetVisible(false);
                    _visible = false;
                }
                else
                {
                    _scissor = rectangle2;
                    SetVisible(true);
                    _visible = true;
                }
                float amount1 = 0.04f;
                float amount2 = 0.06f;
                if (Level.activeLevel != null)
                {
                    amount1 *= Level.activeLevel.transitionSpeedMultiplier;
                    amount2 *= Level.activeLevel.transitionSpeedMultiplier;
                }
                if (!_virtualMode)
                {
                    if (_scanStage == 0)
                    {
                        _stick = Lerp.Float(_stick, 1f, amount1);
                        if (_stick > 0.99f)
                        {
                            _stick = 1f;
                            _incStage = true;
                        }
                    }
                    else if (_scanStage == 1)
                    {
                        _stick = Lerp.Float(_stick, 0f, amount1);
                        if (_stick < 0.01f)
                        {
                            _stick = 0f;
                            _incStage = true;
                        }
                    }
                    else if (_scanStage == 2)
                    {
                        Layer.basicWireframeEffect.effect.Parameters["screenCross"].SetValue(0f);
                        if (Layer.basicWireframeTex)
                            Layer.basicWireframeEffect.effect.Parameters["scanMul"].SetValue(0f);
                        _stick = Lerp.Float(_stick, 1f, amount1);
                        if (_stick > 0.99f)
                        {
                            _stick = 1f;
                            _incStage = true;
                            _done = true;
                        }
                    }
                }
                else if (_scanStage == 2)
                {
                    Layer.basicWireframeEffect.effect.Parameters["screenCross"].SetValue(0f);
                    if (Layer.basicWireframeTex)
                        Layer.basicWireframeEffect.effect.Parameters["scanMul"].SetValue(0f);
                    _stick = Lerp.Float(_stick, 0f, amount2);
                    if (_stick < 0.01f)
                    {
                        _stick = 0f;
                        _decStage = true;
                    }
                }
                else if (_scanStage == 1)
                {
                    _stick = Lerp.Float(_stick, 1f, amount2);
                    if (_stick > 0.99f)
                    {
                        _stick = 1f;
                        _decStage = true;
                    }
                }
                else if (_scanStage == 0)
                {
                    _stick = Lerp.Float(_stick, 0f, amount2);
                    if (_stick < 0.01f)
                    {
                        _stick = 0f;
                        _decStage = true;
                        _done = true;
                    }
                }
                if (_incStage)
                {
                    _incStage = false;
                    ++_scanStage;
                }
                if (_decStage)
                {
                    _decStage = false;
                    --_scanStage;
                }
                if (_scanStage < 2)
                {
                    Layer.doVirtualEffect = true;
                    Layer.basicWireframeTex = _scanStage == 1;
                    Layer.basicWireframeEffect.effect.Parameters["screenCross"].SetValue(_stick);
                    if (Layer.basicWireframeTex)
                        Layer.basicWireframeEffect.effect.Parameters["scanMul"].SetValue(1f);
                }
                _fullyVirtual = false;
                _fullyNonVirtual = false;
                if (_scanStage == 3)
                    _fullyNonVirtual = true;
                else if (_scanStage == -1)
                    _fullyVirtual = true;
                _lastCameraX = Level.activeLevel.camera.centerX;
                if (_scissor.width == 0f)
                    return;
                _parallax.scissor = _scissor;
            }
        }

        public void Draw()
        {
            if (_done && (!_virtualMode || _transitionLevel != Level.activeLevel) && !Level.current._waitingOnTransition || _parallax == null)
                return;
            if (!_visible)
            {
                _parallax.visible = false;
            }
            else
            {
                Graphics.PushMarker("TransitionDraw");
                _position = _parallax.position;
                float num1 = _stick * 300f;
                float x = (float)(360f - _stick * 400f);
                Vec2 vec2_1 = new Vec2(_position.x + num1, _position.y + 72f);
                Graphics.Draw(_scanner, vec2_1.x, vec2_1.y);
                float num2 = Math.Abs(_stick - 0.5f);
                float num3 = 0.5f - num2;
                Graphics.DrawLine(vec2_1 + new Vec2(18f, 20f), new Vec2(x, (float)(vec2_1.y - 100f + num2 * 250f)), Color.Red * num3, 2f, (Depth)0.9f);
                Graphics.DrawLine(vec2_1 + new Vec2(18f, 34f), new Vec2(x, (float)(vec2_1.y - 10f + 80f * num2)), Color.Red * num3, 2f, (Depth)0.9f);
                Vec2 vec2_2 = vec2_1 + new Vec2(0f, _scanner.height);
                Graphics.DrawLine(vec2_2 + new Vec2(18f, -20f), new Vec2(x, (float)(vec2_2.y + 100f - num2 * 250f)), Color.Red * num3, 2f, (Depth)0.9f);
                Graphics.DrawLine(vec2_2 + new Vec2(18f, -34f), new Vec2(x, (float)(vec2_2.y + 10f - 80f * num2)), Color.Red * num3, 2f, (Depth)0.9f);
                if(MonoMain.UpdateLerpState)
                    _parallax.Update();
                _parallax.Draw();
                Graphics.PopMarker();
            }
        }
    }
}

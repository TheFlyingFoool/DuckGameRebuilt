// Decompiled with JetBrains decompiler
// Type: DuckGame.VirtualTransitionCore
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._fullyVirtual = false;
            this._fullyNonVirtual = false;
            this._virtualMode = false;
            this._smallBios = new BitmapFont("smallBiosFont", 7, 6);
            this._parallax = new ParallaxBackground("background/virtual", 0f, 0f, 3);
            float speed1 = 0.4f;
            float distance1 = 0.8f;
            this._parallax.AddZone(0, distance1, speed1);
            this._parallax.AddZone(1, distance1, speed1);
            this._parallax.AddZone(2, distance1, speed1);
            this._parallax.AddZone(3, distance1, speed1);
            float distance2 = 0.6f;
            float num = (float)(((double)distance1 - (double)distance2) / 4.0);
            float speed2 = 0.6f;
            this._parallax.AddZone(4, distance1 - num * 1f, speed2, true);
            this._parallax.AddZone(5, distance1 - num * 2f, -speed2, true);
            this._parallax.AddZone(6, distance1 - num * 3f, speed2, true);
            this._parallax.AddZone(7, distance2, speed1);
            this._parallax.AddZone(8, distance2, speed1);
            this._parallax.AddZone(19, distance2, speed1);
            this._parallax.AddZone(20, distance2, speed1);
            this._parallax.AddZone(21, distance1 - num * 3f, -speed2, true);
            this._parallax.AddZone(22, distance1 - num * 2f, speed2, true);
            this._parallax.AddZone(23, distance1 - num * 1f, -speed2, true);
            this._parallax.AddZone(24, distance1, speed1);
            this._parallax.AddZone(25, distance1, speed1);
            this._parallax.AddZone(26, distance1, speed1);
            this._parallax.AddZone(27, distance1, speed1);
            this._parallax.AddZone(28, distance1, speed1);
            this._parallax.AddZone(29, distance1, speed1);
            this._parallax.AddZone(30, distance1, speed1);
            this._parallax.AddZone(31, distance1, speed1);
            this._parallax.AddZone(32, distance1, speed1);
            this._parallax.AddZone(33, distance1, speed1);
            this._parallax.AddZone(34, distance1, speed1);
            this._parallax.restrictBottom = false;
            this._visible = true;
            this._parallax.y = 0f;
            this._scanner = new Sprite("background/scanbeam");
            this._backgroundColor = Color.Black;
            this._parallax.layer = Layer.Virtual;
        }

        public void SetVisible(bool vis)
        {
            this._parallax.scissor = this._scissor;
            this._parallax.visible = vis;
            if (_scissor.width == 0.0)
                return;
            this._parallax.layer.scissor = this._scissor;
        }

        public bool doingVirtualTransition => this._virtualMode && !this._done;

        public void GoVirtual()
        {
            if (this._virtualMode)
                return;
            this._scanStage = 2;
            this._stick = 1f;
            this._virtualMode = true;
            this._done = false;
            if (this._realBackground == null)
            {
                using (IEnumerator<Thing> enumerator = Level.activeLevel.things[typeof(BackgroundUpdater)].GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        BackgroundUpdater current = (BackgroundUpdater)enumerator.Current;
                        this._realBackground = current;
                        this._curBackgroundColor = current.backgroundColor;
                    }
                }
            }
            this._transitionLevel = Level.activeLevel;
        }

        public void GoUnVirtual()
        {
            if (!this._virtualMode)
                return;
            this._realBackground = null;
            this._virtualMode = false;
            this._scanStage = 0;
            this._stick = 0f;
            this._done = false;
        }

        public bool active => !this._done;

        public void Update()
        {
            if (this._done && !Level.current._waitingOnTransition)
            {
                Layer.doVirtualEffect = false;
                if (this._realBackground == null)
                    return;
                Level.activeLevel.backgroundColor = this._realBackground.backgroundColor;
                this._realBackground.scissor = new Rectangle(0f, 0f, Resolution.current.x, Resolution.current.y);
                this._realBackground = null;
            }
            else
            {
                if (Level.current._waitingOnTransition)
                    this._realBackground = null;
                if (this._realBackground == null)
                {
                    using (IEnumerator<Thing> enumerator = Level.activeLevel.things[typeof(BackgroundUpdater)].GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                            this._realBackground = (BackgroundUpdater)enumerator.Current;
                    }
                }
                float num = this._stick;
                if (this._scanStage == 2 && this._virtualMode)
                {
                    this._backgroundColor = this._curBackgroundColor;
                    Level.activeLevel.backgroundColor = Lerp.ColorSmoothNoAlpha(this._backgroundColor, this._curBackgroundColor, this._stick);
                    Layer.Glow.fade = Lerp.FloatSmooth(Layer.Glow.fade, 0f, this._stick);
                }
                if (this._scanStage == 0 && !this._virtualMode && this._realBackground != null)
                {
                    Level.activeLevel.backgroundColor = Lerp.ColorSmoothNoAlpha(this._backgroundColor, this._realBackground.backgroundColor, this._stick);
                    Layer.Glow.fade = Lerp.FloatSmooth(Layer.Glow.fade, 1f, this._stick);
                }
                if (this._scanStage == -1)
                    Level.activeLevel.backgroundColor = Lerp.ColorSmoothNoAlpha(this._backgroundColor, Color.Black, 0.1f);
                if (this._scanStage < 2)
                    num = 0f;
                Rectangle rectangle1 = new Rectangle((int)((1.0 - (double)num) * Resolution.current.x), 0f, Resolution.current.x - (int)((1.0 - (double)num) * Resolution.current.x), Resolution.current.y);
                if (this._realBackground != null)
                {
                    if (rectangle1.width == 0.0)
                    {
                        this._realBackground.SetVisible(false);
                    }
                    else
                    {
                        this._realBackground.scissor = rectangle1;
                        this._realBackground.SetVisible(true);
                    }
                }
                Rectangle rectangle2 = new Rectangle(0f, 0f, Resolution.current.x - rectangle1.width, Resolution.current.y);
                if (rectangle2.width == 0.0)
                {
                    this.SetVisible(false);
                    this._visible = false;
                }
                else
                {
                    this._scissor = rectangle2;
                    this.SetVisible(true);
                    this._visible = true;
                }
                float amount1 = 0.04f;
                float amount2 = 0.06f;
                if (Level.activeLevel != null)
                {
                    amount1 *= Level.activeLevel.transitionSpeedMultiplier;
                    amount2 *= Level.activeLevel.transitionSpeedMultiplier;
                }
                if (!this._virtualMode)
                {
                    if (this._scanStage == 0)
                    {
                        this._stick = Lerp.Float(this._stick, 1f, amount1);
                        if (_stick > 0.990000009536743)
                        {
                            this._stick = 1f;
                            this._incStage = true;
                        }
                    }
                    else if (this._scanStage == 1)
                    {
                        this._stick = Lerp.Float(this._stick, 0f, amount1);
                        if (_stick < 0.00999999977648258)
                        {
                            this._stick = 0f;
                            this._incStage = true;
                        }
                    }
                    else if (this._scanStage == 2)
                    {
                        Layer.basicWireframeEffect.effect.Parameters["screenCross"].SetValue(0f);
                        if (Layer.basicWireframeTex)
                            Layer.basicWireframeEffect.effect.Parameters["scanMul"].SetValue(0f);
                        this._stick = Lerp.Float(this._stick, 1f, amount1);
                        if (_stick > 0.990000009536743)
                        {
                            this._stick = 1f;
                            this._incStage = true;
                            this._done = true;
                        }
                    }
                }
                else if (this._scanStage == 2)
                {
                    Layer.basicWireframeEffect.effect.Parameters["screenCross"].SetValue(0f);
                    if (Layer.basicWireframeTex)
                        Layer.basicWireframeEffect.effect.Parameters["scanMul"].SetValue(0f);
                    this._stick = Lerp.Float(this._stick, 0f, amount2);
                    if (_stick < 0.00999999977648258)
                    {
                        this._stick = 0f;
                        this._decStage = true;
                    }
                }
                else if (this._scanStage == 1)
                {
                    this._stick = Lerp.Float(this._stick, 1f, amount2);
                    if (_stick > 0.990000009536743)
                    {
                        this._stick = 1f;
                        this._decStage = true;
                    }
                }
                else if (this._scanStage == 0)
                {
                    this._stick = Lerp.Float(this._stick, 0f, amount2);
                    if (_stick < 0.00999999977648258)
                    {
                        this._stick = 0f;
                        this._decStage = true;
                        this._done = true;
                    }
                }
                if (this._incStage)
                {
                    this._incStage = false;
                    ++this._scanStage;
                }
                if (this._decStage)
                {
                    this._decStage = false;
                    --this._scanStage;
                }
                if (this._scanStage < 2)
                {
                    Layer.doVirtualEffect = true;
                    Layer.basicWireframeTex = this._scanStage == 1;
                    Layer.basicWireframeEffect.effect.Parameters["screenCross"].SetValue(this._stick);
                    if (Layer.basicWireframeTex)
                        Layer.basicWireframeEffect.effect.Parameters["scanMul"].SetValue(1f);
                }
                this._fullyVirtual = false;
                this._fullyNonVirtual = false;
                if (this._scanStage == 3)
                    this._fullyNonVirtual = true;
                else if (this._scanStage == -1)
                    this._fullyVirtual = true;
                this._lastCameraX = Level.activeLevel.camera.centerX;
                if (_scissor.width == 0.0)
                    return;
                this._parallax.scissor = this._scissor;
            }
        }

        public void Draw()
        {
            if (this._done && (!this._virtualMode || this._transitionLevel != Level.activeLevel) && !Level.current._waitingOnTransition || this._parallax == null)
                return;
            if (!this._visible)
            {
                this._parallax.visible = false;
            }
            else
            {
                Graphics.PushMarker("TransitionDraw");
                this._position = this._parallax.position;
                float num1 = this._stick * 300f;
                float x = (float)(360.0 - _stick * 400.0);
                Vec2 vec2_1 = new Vec2(this._position.x + num1, this._position.y + 72f);
                Graphics.Draw(this._scanner, vec2_1.x, vec2_1.y);
                float num2 = Math.Abs(this._stick - 0.5f);
                float num3 = 0.5f - num2;
                Graphics.DrawLine(vec2_1 + new Vec2(18f, 20f), new Vec2(x, (float)(vec2_1.y - 100.0 + (double)num2 * 250.0)), Color.Red * num3, 2f, (Depth)0.9f);
                Graphics.DrawLine(vec2_1 + new Vec2(18f, 34f), new Vec2(x, (float)(vec2_1.y - 10.0 + 80.0 * (double)num2)), Color.Red * num3, 2f, (Depth)0.9f);
                Vec2 vec2_2 = vec2_1 + new Vec2(0f, _scanner.height);
                Graphics.DrawLine(vec2_2 + new Vec2(18f, -20f), new Vec2(x, (float)(vec2_2.y + 100.0 - (double)num2 * 250.0)), Color.Red * num3, 2f, (Depth)0.9f);
                Graphics.DrawLine(vec2_2 + new Vec2(18f, -34f), new Vec2(x, (float)(vec2_2.y + 10.0 - 80.0 * (double)num2)), Color.Red * num3, 2f, (Depth)0.9f);
                this._parallax.Update();
                this._parallax.Draw();
                Graphics.PopMarker();
            }
        }
    }
}

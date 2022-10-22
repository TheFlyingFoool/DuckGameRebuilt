// Decompiled with JetBrains decompiler
// Type: DuckGame.VirtualBackground
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    public class VirtualBackground : BackgroundUpdater
    {
        private Sprite _scanner;
        private BackgroundUpdater _realBackground;
        private int scanStage = -1;
        private float stick;
        public bool needsWireframe;
        public bool fullyVirtual = true;
        public bool fullyNonVirtual;
        public bool virtualMode = true;
        public bool visible = true;
        private bool _foreground;
        private static ParallaxBackground _para;
        private bool done;
        private bool incStage;
        private bool decStage;

        public void SetLayer(Layer l)
        {
            layer = l;
            _parallax.layer = l;
        }

        public VirtualBackground(float xpos, float ypos, BackgroundUpdater realBackground, bool fore = false)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 2
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _editorName = "Virtual";
            _realBackground = realBackground;
            _foreground = fore;
        }

        public static void InitializeBack()
        {
            VirtualBackground._para = new ParallaxBackground("background/virtual", 0f, 0f, 3);
            float speed1 = 0.4f;
            float distance1 = 0.8f;
            VirtualBackground._para.AddZone(0, distance1, speed1);
            VirtualBackground._para.AddZone(1, distance1, speed1);
            VirtualBackground._para.AddZone(2, distance1, speed1);
            VirtualBackground._para.AddZone(3, distance1, speed1);
            float distance2 = 0.6f;
            float num = (float)((distance1 - distance2) / 4.0);
            float speed2 = 1f;
            VirtualBackground._para.AddZone(4, distance1 - num * 1f, speed2, true);
            VirtualBackground._para.AddZone(5, distance1 - num * 2f, -speed2, true);
            VirtualBackground._para.AddZone(6, distance1 - num * 3f, speed2, true);
            VirtualBackground._para.AddZone(7, distance2, speed1);
            VirtualBackground._para.AddZone(8, distance2, speed1);
            VirtualBackground._para.AddZone(19, distance2, speed1);
            VirtualBackground._para.AddZone(20, distance2, speed1);
            VirtualBackground._para.AddZone(21, distance1 - num * 3f, -speed2, true);
            VirtualBackground._para.AddZone(22, distance1 - num * 2f, speed2, true);
            VirtualBackground._para.AddZone(23, distance1 - num * 1f, -speed2, true);
            VirtualBackground._para.AddZone(24, distance1, speed1);
            VirtualBackground._para.AddZone(25, distance1, speed1);
            VirtualBackground._para.AddZone(26, distance1, speed1);
            VirtualBackground._para.AddZone(27, distance1, speed1);
            VirtualBackground._para.AddZone(28, distance1, speed1);
            VirtualBackground._para.AddZone(29, distance1, speed1);
            VirtualBackground._para.AddZone(30, distance1, speed1);
            VirtualBackground._para.AddZone(31, distance1, speed1);
            VirtualBackground._para.AddZone(32, distance1, speed1);
            VirtualBackground._para.AddZone(33, distance1, speed1);
            VirtualBackground._para.AddZone(34, distance1, speed1);
            VirtualBackground._para.restrictBottom = false;
            VirtualBackground._para.layer = new Layer("VIRTUALPARALLAX", 95, new Camera(0f, 0f, 320f, 320f * Graphics.aspect));
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            fullyVirtual = true;
            fullyNonVirtual = false;
            virtualMode = true;
            needsWireframe = true;
            backgroundColor = new Color(0, 0, 0);
            Level.current.backgroundColor = backgroundColor;
            _parallax = VirtualBackground._para;
            Layer.Add(_parallax.layer);
            _parallax.layer.Clear();
            Level.Add(_parallax);
            visible = true;
            parallax.y = 0f;
            layer = _parallax.layer;
            layer.fade = 1f;
            _scanner = new Sprite("background/scanbeam");
            _skipMovement = true;
        }

        public override void Terminate()
        {
            fullyVirtual = false;
            fullyNonVirtual = true;
            virtualMode = false;
            needsWireframe = false;
        }

        public override void Update()
        {
            float num = stick;
            if (scanStage < 2)
            {
                Level.current.backgroundColor = Lerp.Color(Level.current.backgroundColor, backgroundColor, 0.04f);
                num = 0f;
            }
            else if (_realBackground != null)
                Level.current.backgroundColor = Lerp.Color(Level.current.backgroundColor, _realBackground.backgroundColor, 0.04f);
            Rectangle rectangle1 = new Rectangle((int)((1.0 - num) * Resolution.current.x), 0f, Resolution.current.x - (int)((1.0 - num) * Resolution.current.x), Resolution.current.y);
            if (_realBackground != null)
            {
                if (rectangle1.width == 0.0)
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
            if (rectangle2.width == 0.0)
            {
                SetVisible(false);
                visible = false;
            }
            else
            {
                scissor = rectangle2;
                SetVisible(true);
                visible = true;
            }
            if (virtualMode && done && scanStage == 3)
                --scanStage;
            else if (!virtualMode && !done && scanStage == -1)
                ++scanStage;
            float amount1 = 0.04f;
            float amount2 = 0.06f;
            if (Level.current != null)
            {
                amount1 *= Level.current.transitionSpeedMultiplier;
                amount2 *= Level.current.transitionSpeedMultiplier;
            }
            if (!done)
            {
                if (scanStage == 0)
                {
                    stick = Lerp.Float(stick, 1f, amount1);
                    if (stick > 0.95f)
                    {
                        stick = 1f;
                        incStage = true;
                    }
                }
                else if (scanStage == 1)
                {
                    stick = Lerp.Float(stick, 0f, amount1);
                    if (stick < 0.05f)
                    {
                        stick = 0f;
                        incStage = true;
                    }
                }
                else if (scanStage == 2)
                {
                    stick = Lerp.Float(stick, 1f, amount1);
                    if (stick > 0.95f)
                    {
                        stick = 1f;
                        incStage = true;
                        done = true;
                    }
                }
            }
            else if (scanStage == 2)
            {
                stick = Lerp.Float(stick, 0f, amount2);
                if (stick < 0.05f)
                {
                    stick = 0f;
                    decStage = true;
                }
            }
            else if (scanStage == 1)
            {
                stick = Lerp.Float(stick, 1f, amount2);
                if (stick > 0.95f)
                {
                    stick = 1f;
                    decStage = true;
                }
            }
            else if (scanStage == 0)
            {
                stick = Lerp.Float(stick, 0f, amount2);
                if (stick < 0.05f)
                {
                    stick = 0f;
                    decStage = true;
                    done = false;
                }
            }
            if (scanStage < 2)
            {
                Layer.basicWireframeEffect.effect.Parameters["screenCross"].SetValue(stick);
                Layer.basicWireframeTex = scanStage == 1;
            }
            if (incStage)
            {
                incStage = false;
                ++scanStage;
            }
            if (decStage)
            {
                decStage = false;
                --scanStage;
            }
            fullyVirtual = false;
            fullyNonVirtual = false;
            if (scanStage == 3)
            {
                needsWireframe = false;
                fullyNonVirtual = true;
            }
            else
            {
                needsWireframe = true;
                if (scanStage == -1)
                    fullyVirtual = true;
            }
            base.Update();
        }

        public override void Draw()
        {
            if (_parallax == null)
                return;
            if (!visible)
            {
                _parallax.visible = false;
            }
            else
            {
                position = _parallax.position;
                float num1 = stick * 300f;
                float x = (float)(360.0 - stick * 400.0);
                Vec2 vec2_1 = new Vec2(this.x + num1, y + 72f);
                Graphics.Draw(_scanner, vec2_1.x, vec2_1.y);
                float num2 = Math.Abs(stick - 0.5f);
                float num3 = 0.5f - num2;
                Graphics.DrawLine(vec2_1 + new Vec2(18f, 20f), new Vec2(x, (float)(vec2_1.y - 100.0 + num2 * 250.0)), Color.Red * num3, 2f, (Depth)0.9f);
                Graphics.DrawLine(vec2_1 + new Vec2(18f, 34f), new Vec2(x, (float)(vec2_1.y - 10.0 + 80.0 * num2)), Color.Red * num3, 2f, (Depth)0.9f);
                Vec2 vec2_2 = vec2_1 + new Vec2(0f, _scanner.height);
                Graphics.DrawLine(vec2_2 + new Vec2(18f, -20f), new Vec2(x, (float)(vec2_2.y + 100.0 - num2 * 250.0)), Color.Red * num3, 2f, (Depth)0.9f);
                Graphics.DrawLine(vec2_2 + new Vec2(18f, -34f), new Vec2(x, (float)(vec2_2.y + 10.0 - 80.0 * num2)), Color.Red * num3, 2f, (Depth)0.9f);
            }
        }
    }
}

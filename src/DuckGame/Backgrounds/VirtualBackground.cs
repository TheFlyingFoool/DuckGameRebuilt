// Decompiled with JetBrains decompiler
// Type: DuckGame.VirtualBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.layer = l;
            this._parallax.layer = l;
        }

        public VirtualBackground(float xpos, float ypos, BackgroundUpdater realBackground, bool fore = false)
          : base(xpos, ypos)
        {
            this.graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 2
            };
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._visibleInGame = false;
            this._editorName = "Virtual";
            this._realBackground = realBackground;
            this._foreground = fore;
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
            float num = (float)(((double)distance1 - (double)distance2) / 4.0);
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
            this.fullyVirtual = true;
            this.fullyNonVirtual = false;
            this.virtualMode = true;
            this.needsWireframe = true;
            this.backgroundColor = new Color(0, 0, 0);
            Level.current.backgroundColor = this.backgroundColor;
            this._parallax = VirtualBackground._para;
            Layer.Add(this._parallax.layer);
            this._parallax.layer.Clear();
            Level.Add(_parallax);
            this.visible = true;
            this.parallax.y = 0f;
            this.layer = this._parallax.layer;
            this.layer.fade = 1f;
            this._scanner = new Sprite("background/scanbeam");
            this._skipMovement = true;
        }

        public override void Terminate()
        {
            this.fullyVirtual = false;
            this.fullyNonVirtual = true;
            this.virtualMode = false;
            this.needsWireframe = false;
        }

        public override void Update()
        {
            float num = this.stick;
            if (this.scanStage < 2)
            {
                Level.current.backgroundColor = Lerp.Color(Level.current.backgroundColor, this.backgroundColor, 0.04f);
                num = 0f;
            }
            else if (this._realBackground != null)
                Level.current.backgroundColor = Lerp.Color(Level.current.backgroundColor, this._realBackground.backgroundColor, 0.04f);
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
                this.visible = false;
            }
            else
            {
                this.scissor = rectangle2;
                this.SetVisible(true);
                this.visible = true;
            }
            if (this.virtualMode && this.done && this.scanStage == 3)
                --this.scanStage;
            else if (!this.virtualMode && !this.done && this.scanStage == -1)
                ++this.scanStage;
            float amount1 = 0.04f;
            float amount2 = 0.06f;
            if (Level.current != null)
            {
                amount1 *= Level.current.transitionSpeedMultiplier;
                amount2 *= Level.current.transitionSpeedMultiplier;
            }
            if (!this.done)
            {
                if (this.scanStage == 0)
                {
                    this.stick = Lerp.Float(this.stick, 1f, amount1);
                    if (stick > 0.949999988079071)
                    {
                        this.stick = 1f;
                        this.incStage = true;
                    }
                }
                else if (this.scanStage == 1)
                {
                    this.stick = Lerp.Float(this.stick, 0f, amount1);
                    if (stick < 0.0500000007450581)
                    {
                        this.stick = 0f;
                        this.incStage = true;
                    }
                }
                else if (this.scanStage == 2)
                {
                    this.stick = Lerp.Float(this.stick, 1f, amount1);
                    if (stick > 0.949999988079071)
                    {
                        this.stick = 1f;
                        this.incStage = true;
                        this.done = true;
                    }
                }
            }
            else if (this.scanStage == 2)
            {
                this.stick = Lerp.Float(this.stick, 0f, amount2);
                if (stick < 0.0500000007450581)
                {
                    this.stick = 0f;
                    this.decStage = true;
                }
            }
            else if (this.scanStage == 1)
            {
                this.stick = Lerp.Float(this.stick, 1f, amount2);
                if (stick > 0.949999988079071)
                {
                    this.stick = 1f;
                    this.decStage = true;
                }
            }
            else if (this.scanStage == 0)
            {
                this.stick = Lerp.Float(this.stick, 0f, amount2);
                if (stick < 0.0500000007450581)
                {
                    this.stick = 0f;
                    this.decStage = true;
                    this.done = false;
                }
            }
            if (this.scanStage < 2)
            {
                Layer.basicWireframeEffect.effect.Parameters["screenCross"].SetValue(this.stick);
                Layer.basicWireframeTex = this.scanStage == 1;
            }
            if (this.incStage)
            {
                this.incStage = false;
                ++this.scanStage;
            }
            if (this.decStage)
            {
                this.decStage = false;
                --this.scanStage;
            }
            this.fullyVirtual = false;
            this.fullyNonVirtual = false;
            if (this.scanStage == 3)
            {
                this.needsWireframe = false;
                this.fullyNonVirtual = true;
            }
            else
            {
                this.needsWireframe = true;
                if (this.scanStage == -1)
                    this.fullyVirtual = true;
            }
            base.Update();
        }

        public override void Draw()
        {
            if (this._parallax == null)
                return;
            if (!this.visible)
            {
                this._parallax.visible = false;
            }
            else
            {
                this.position = this._parallax.position;
                float num1 = this.stick * 300f;
                float x = (float)(360.0 - stick * 400.0);
                Vec2 vec2_1 = new Vec2(this.x + num1, this.y + 72f);
                Graphics.Draw(this._scanner, vec2_1.x, vec2_1.y);
                float num2 = Math.Abs(this.stick - 0.5f);
                float num3 = 0.5f - num2;
                Graphics.DrawLine(vec2_1 + new Vec2(18f, 20f), new Vec2(x, (float)(vec2_1.y - 100.0 + (double)num2 * 250.0)), Color.Red * num3, 2f, (Depth)0.9f);
                Graphics.DrawLine(vec2_1 + new Vec2(18f, 34f), new Vec2(x, (float)(vec2_1.y - 10.0 + 80.0 * (double)num2)), Color.Red * num3, 2f, (Depth)0.9f);
                Vec2 vec2_2 = vec2_1 + new Vec2(0f, _scanner.height);
                Graphics.DrawLine(vec2_2 + new Vec2(18f, -20f), new Vec2(x, (float)(vec2_2.y + 100.0 - (double)num2 * 250.0)), Color.Red * num3, 2f, (Depth)0.9f);
                Graphics.DrawLine(vec2_2 + new Vec2(18f, -34f), new Vec2(x, (float)(vec2_2.y + 10.0 - 80.0 * (double)num2)), Color.Red * num3, 2f, (Depth)0.9f);
            }
        }
    }
}

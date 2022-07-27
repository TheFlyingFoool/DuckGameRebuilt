// Decompiled with JetBrains decompiler
// Type: DuckGame.PlugMachine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Special|Arcade", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class PlugMachine : Thing
    {
        private SpriteMap _sprite;
        private float _hoverFade;
        private Sprite _hoverSprite;
        private Sprite _duckSprite;
        private SpriteMap _ledStrip;
        private SpriteMap _screen;
        public bool hover;
        private Thing _lighting;
        private DustSparkleEffect _dust;

        public override Vec2 cameraPosition => this.position + new Vec2(-16f, 0.0f);

        public PlugMachine(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("arcade/plug_machine", 38, 36);
            this._sprite.AddAnimation("idle", 0.5f, true, 0, 1, 2, 3);
            this._sprite.SetAnimation("idle");
            this._screen = new SpriteMap("arcade/plug_machine_monitor", 11, 8);
            this._screen.AddAnimation("idle", 0.2f, true, 0, 1, 2);
            this._screen.SetAnimation("idle");
            this.graphic = _sprite;
            this.depth = - 0.5f;
            this.center = new Vec2(this._sprite.width / 2, this._sprite.h / 2);
            this._collisionSize = new Vec2(16f, 15f);
            this._collisionOffset = new Vec2(-8f, 2f);
            this._hoverSprite = new Sprite("arcade/plug_hover");
            this._duckSprite = new Sprite("arcade/plug_duck");
            this._ledStrip = new SpriteMap("arcade/led_strip", 14, 1);
            this._ledStrip.AddAnimation("idle", 0.3f, true, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            this._ledStrip.SetAnimation("idle");
            this.hugWalls = WallHug.Floor;
        }

        public override void Initialize()
        {
            if (Level.current is Editor || this.level == null || this.level.bareInitialize)
                return;
            this._dust = new DustSparkleEffect(this.x - 34f, this.y - 40f, false, false);
            Level.Add(_dust);
            this._dust.depth = this.depth - 2;
            this._lighting = new ArcadeScreen(this.x, this.y);
            Level.Add(this._lighting);
        }

        public override void Update()
        {
            Vec2 p = this.position + new Vec2(-20f, 0.0f);
            Duck duck = Level.Nearest<Duck>(p);
            if (duck != null)
            {
                if (duck.grounded && (double)(duck.position - p).length < 16.0)
                {
                    this._hoverFade = Lerp.Float(this._hoverFade, 1f, 0.2f);
                    this.hover = true;
                }
                else
                {
                    this._hoverFade = Lerp.Float(this._hoverFade, 0.0f, 0.2f);
                    this.hover = false;
                }
            }
            this._dust.fade = 0.7f;
            this._dust.visible = this._lighting.visible = this.visible;
        }

        public override void Draw()
        {
            this.graphic.color = Color.White;
            if (!(Level.current is Editor))
            {
                Vec2 vec2 = new Vec2(-24f, -8f);
                this._duckSprite.depth = this.depth + 16;
                Graphics.Draw(this._duckSprite, this.x + vec2.x, this.y + vec2.y);
                this._ledStrip.alpha = 1f;
                this._ledStrip.depth = this.depth + 10;
                Graphics.Draw(_ledStrip, this.x - 16f, this.y + 9f);
                this._ledStrip.alpha = 0.25f;
                this._ledStrip.depth = this.depth + 10;
                Graphics.Draw(_ledStrip, this.x - 16f, this.y + 10f);
                this._screen.depth = this.depth + 5;
                Graphics.Draw(_screen, this.x - 9f, this.y - 7f);
                this._hoverSprite.alpha = Lerp.Float(this._hoverSprite.alpha, this._hoverFade, 0.05f);
                if ((double)this._hoverSprite.alpha > 0.00999999977648258)
                {
                    this._hoverSprite.depth = this.depth + 6;
                    Graphics.Draw(this._hoverSprite, (float)((double)this.x + vec2.x - 1.0), (float)((double)this.y + vec2.y - 1.0));
                }
            }
            base.Draw();
        }
    }
}
